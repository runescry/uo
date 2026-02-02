/*
 * Intel Pin Tool to Extract AES-256-CTR Key/IV from ClassicUO.exe
 * 
 * Compile with:
 *   cd D:\Tools\IntelPin\source\tools\pin_extract_key
 *   make
 * 
 * Or manually:
 *   cl /LD /I"D:\Tools\IntelPin\source\include\pin" pin_extract_key.cpp /link /LIBPATH:"D:\Tools\IntelPin\ia32\lib" /LIBPATH:"D:\Tools\IntelPin\ia32\lib-ext" pin.lib
 */

#include "pin.H"
#include <iostream>
#include <fstream>
#include <iomanip>
#include <string>

// Log file
ofstream log_file;

// Function to log extracted keys/IVs
VOID LogKeyIV(const char* type, ADDRINT addr, UINT8* data, UINT32 len)
{
    log_file << "[" << type << "] Address: 0x" << hex << addr << dec << ", Data: ";
    for (UINT32 i = 0; i < len; i++) {
        log_file << hex << setfill('0') << setw(2) << (UINT32)data[i];
    }
    log_file << dec << endl;
    log_file.flush();
    
    // Also print to console
    cout << "[" << type << "] ";
    for (UINT32 i = 0; i < len; i++) {
        cout << hex << setfill('0') << setw(2) << (UINT32)data[i];
    }
    cout << dec << endl;
}

// Hook for function calls that might be decryption
VOID OnFunctionCall(ADDRINT func_addr, ADDRINT return_addr)
{
    // Check if this is near the DecryptAES256_CTR offset we found (0xa3c494)
    // We'll need to adjust this based on actual runtime address
    static bool found_decrypt = false;
    
    if (!found_decrypt) {
        log_file << "Function call to: 0x" << hex << func_addr << dec << endl;
    }
}

// Instrumentation function - called for each instruction
VOID Instruction(INS ins, VOID *v)
{
    // Look for AES-NI instructions (if hardware acceleration is used)
    if (INS_Opcode(ins) == XED_ICLASS_AESDEC ||
        INS_Opcode(ins) == XED_ICLASS_AESENC ||
        INS_Opcode(ins) == XED_ICLASS_AESKEYGENASSIST) {
        
        // Insert call before instruction to log context
        INS_InsertCall(ins, IPOINT_BEFORE, (AFUNPTR)LogKeyIV,
            IARG_PTR, "AES_INSTR",
            IARG_INST_PTR,
            IARG_END);
    }
    
    // Look for function calls
    if (INS_IsCall(ins)) {
        // Get target address
        if (INS_IsDirectCall(ins)) {
            ADDRINT target = INS_DirectBranchOrCallTargetAddress(ins);
            INS_InsertCall(ins, IPOINT_BEFORE, (AFUNPTR)OnFunctionCall,
                IARG_ADDRINT, target,
                IARG_RETURN_IP,
                IARG_END);
        }
    }
}

// Hook memory reads that might be reading keys/IVs
VOID OnMemoryRead(VOID *ip, VOID *addr, UINT32 size)
{
    // Check if reading 32 bytes (key) or 16 bytes (IV)
    if (size == 32 || size == 16) {
        UINT8* data = new UINT8[size];
        PIN_SafeCopy(data, (VOID*)addr, size);
        
        const char* type = (size == 32) ? "POTENTIAL_KEY" : "POTENTIAL_IV";
        LogKeyIV(type, (ADDRINT)addr, data, size);
        
        delete[] data;
    }
}

// Instrument memory reads
VOID MemoryRead(INS ins, VOID *v)
{
    // Instrument memory reads of 16 or 32 bytes
    if (INS_IsMemoryRead(ins)) {
        UINT32 memOps = INS_MemoryOperandCount(ins);
        for (UINT32 i = 0; i < memOps; i++) {
            if (INS_MemoryOperandIsRead(ins, i)) {
                UINT32 size = INS_MemoryOperandSize(ins, i);
                if (size == 16 || size == 32) {
                    INS_InsertPredicatedCall(
                        ins, IPOINT_BEFORE, (AFUNPTR)OnMemoryRead,
                        IARG_INST_PTR,
                        IARG_MEMORYOP_EA, i,
                        IARG_UINT32, size,
                        IARG_END);
                }
            }
        }
    }
}

// Called when image is loaded
VOID ImageLoad(IMG img, VOID *v)
{
    string img_name = IMG_Name(img);
    log_file << "Image loaded: " << img_name << " at 0x" << hex << IMG_LowAddress(img) << dec << endl;
    
    // Check if this is ClassicUO.exe
    if (img_name.find("ClassicUO") != string::npos) {
        log_file << "*** Found ClassicUO module ***" << endl;
        
        // Try to find DecryptAES256_CTR function
        // We know from binary analysis it's at offset 0xa3c494
        ADDRINT base_addr = IMG_LowAddress(img);
        ADDRINT decrypt_offset = 0xa3c494;
        ADDRINT decrypt_addr = base_addr + decrypt_offset;
        
        log_file << "Looking for DecryptAES256_CTR at: 0x" << hex << decrypt_addr << dec << endl;
        
        // Try to set a breakpoint/analysis routine at this address
        RTN rtn = RTN_FindByAddress(decrypt_addr);
        if (RTN_Valid(rtn)) {
            log_file << "Found routine at target address: " << RTN_Name(rtn) << endl;
            
            RTN_Open(rtn);
            
            // Instrument the routine
            for (INS ins = RTN_InsHead(rtn); INS_Valid(ins); ins = INS_Next(ins)) {
                // Log all instructions in this routine
                INS_InsertCall(ins, IPOINT_BEFORE, (AFUNPTR)LogKeyIV,
                    IARG_PTR, "DECRYPT_ROUTINE",
                    IARG_INST_PTR,
                    IARG_END);
            }
            
            RTN_Close(rtn);
        } else {
            log_file << "Routine not found at target address, will monitor all calls" << endl;
        }
    }
}

// Called when program exits
VOID Fini(INT32 code, VOID *v)
{
    log_file << "Pin tool finished" << endl;
    log_file.close();
}

// Main function
int main(int argc, char *argv[])
{
    // Initialize Pin
    if (PIN_Init(argc, argv)) {
        cerr << "Usage: pin -t pin_extract_key.dll -- <program>" << endl;
        return 1;
    }
    
    // Open log file
    log_file.open("pin_key_extraction.log");
    log_file << "Intel Pin Key/IV Extractor Started" << endl;
    log_file << "===================================" << endl << endl;
    
    // Register callbacks
    IMG_AddInstrumentFunction(ImageLoad, 0);
    INS_AddInstrumentFunction(Instruction, 0);
    INS_AddInstrumentFunction(MemoryRead, 0);
    PIN_AddFiniFunction(Fini, 0);
    
    // Start program
    PIN_StartProgram();
    
    return 0;
}
