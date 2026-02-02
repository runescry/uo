/* DynamoRIO client to extract AES keys from XMM registers
 *
 * When AES-NI instructions execute, the key is in XMM registers.
 * This client hooks AESENC/AESDEC instructions and dumps the registers.
 *
 * Build with DynamoRIO's cmake system.
 */

#include "dr_api.h"
#include "drmgr.h"
#include "drreg.h"
#include <stdio.h>
#include <string.h>

static file_t logfile;
static void *mutex;
static int aes_count = 0;

/* Check if instruction is an AES-NI instruction */
static bool is_aes_instruction(instr_t *instr) {
    int opcode = instr_get_opcode(instr);
    switch (opcode) {
        case OP_aesenc:
        case OP_aesenclast:
        case OP_aesdec:
        case OP_aesdeclast:
        case OP_aeskeygenassist:
        case OP_aesimc:
            return true;
        default:
            return false;
    }
}

/* Get opcode name for logging */
static const char* get_aes_opcode_name(int opcode) {
    switch (opcode) {
        case OP_aesenc: return "AESENC";
        case OP_aesenclast: return "AESENCLAST";
        case OP_aesdec: return "AESDEC";
        case OP_aesdeclast: return "AESDECLAST";
        case OP_aeskeygenassist: return "AESKEYGENASSIST";
        case OP_aesimc: return "AESIMC";
        default: return "UNKNOWN_AES";
    }
}

/* Called before each AES instruction - dump XMM registers */
static void aes_callback(app_pc pc, int opcode) {
    dr_mcontext_t mc;
    mc.size = sizeof(mc);
    mc.flags = DR_MC_ALL;  /* Get all registers including XMM */

    void *drcontext = dr_get_current_drcontext();
    dr_get_mcontext(drcontext, &mc);

    dr_mutex_lock(mutex);
    aes_count++;

    /* Only log first 100 AES operations to avoid huge files */
    if (aes_count <= 100) {
        dr_fprintf(logfile, "\n========== AES #%d ==========\n", aes_count);
        dr_fprintf(logfile, "PC: " PFX "\n", pc);
        dr_fprintf(logfile, "Opcode: %s\n", get_aes_opcode_name(opcode));

        /* Dump XMM0-XMM15 (potential key locations) */
        for (int i = 0; i < 16; i++) {
            unsigned char *xmm = (unsigned char *)&mc.simd[i];
            dr_fprintf(logfile, "XMM%02d: ", i);
            for (int j = 0; j < 16; j++) {
                dr_fprintf(logfile, "%02x", xmm[j]);
            }
            dr_fprintf(logfile, "\n");
        }

        /* Also show as potential 32-byte key (XMM0+XMM1, etc.) */
        dr_fprintf(logfile, "\nPotential 256-bit keys:\n");
        for (int i = 0; i < 15; i += 2) {
            unsigned char *xmm0 = (unsigned char *)&mc.simd[i];
            unsigned char *xmm1 = (unsigned char *)&mc.simd[i+1];
            dr_fprintf(logfile, "XMM%d+XMM%d: ", i, i+1);
            for (int j = 0; j < 16; j++) dr_fprintf(logfile, "%02x", xmm0[j]);
            for (int j = 0; j < 16; j++) dr_fprintf(logfile, "%02x", xmm1[j]);
            dr_fprintf(logfile, "\n");
        }
    }

    dr_mutex_unlock(mutex);
}

/* Instrument each basic block */
static dr_emit_flags_t event_app_instruction(void *drcontext, void *tag,
    instrlist_t *bb, instr_t *instr, bool for_trace, bool translating,
    void *user_data)
{
    if (!instr_is_app(instr))
        return DR_EMIT_DEFAULT;

    /* Check if this is an AES instruction */
    if (is_aes_instruction(instr)) {
        int opcode = instr_get_opcode(instr);
        app_pc pc = instr_get_app_pc(instr);

        /* Insert clean call to our callback BEFORE the AES instruction */
        dr_insert_clean_call(drcontext, bb, instr, (void *)aes_callback,
                             false, 2,
                             OPND_CREATE_INTPTR(pc),
                             OPND_CREATE_INT32(opcode));
    }

    return DR_EMIT_DEFAULT;
}

static void event_exit(void) {
    dr_fprintf(logfile, "\n\n=== SUMMARY ===\n");
    dr_fprintf(logfile, "Total AES instructions intercepted: %d\n", aes_count);
    dr_close_file(logfile);

    drmgr_unregister_bb_insertion_event(event_app_instruction);
    dr_mutex_destroy(mutex);
    drmgr_exit();
}

DR_EXPORT void dr_client_main(client_id_t id, int argc, const char *argv[]) {
    dr_set_client_name("AES Key Extractor", "https://github.com/example");

    if (!drmgr_init())
        DR_ASSERT(false);

    /* Open log file */
    logfile = dr_open_file("aes_keys.log", DR_FILE_WRITE_OVERWRITE);
    if (logfile == INVALID_FILE) {
        dr_fprintf(STDERR, "Failed to open log file!\n");
        DR_ASSERT(false);
    }

    dr_fprintf(logfile, "AES Key Extractor started\n");
    dr_fprintf(logfile, "Looking for AES-NI instructions...\n");

    mutex = dr_mutex_create();

    dr_register_exit_event(event_exit);
    drmgr_register_bb_instrumentation_event(NULL, event_app_instruction, NULL);

    dr_log(NULL, DR_LOG_ALL, 1, "AES Key Extractor initialized\n");
}
