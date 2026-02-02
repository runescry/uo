"""
Frida script to hook AES decryption in ClassicUO and capture the key.

Run with: python frida_hook_aes.py
Make sure ClassicUO.exe is running first.
"""
import frida
import sys

# JavaScript code to inject into the process
HOOK_SCRIPT = """
// Find the AES S-box - this is always present in AES implementations
const SBOX_START = [0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5];

// Search for potential AES-related functions
function findAesImplementation() {
    var modules = Process.enumerateModules();
    var mainModule = modules[0]; // ClassicUO.exe

    console.log("Main module: " + mainModule.name + " at " + mainModule.base);
    console.log("Size: " + mainModule.size);

    // Search for AES S-box in memory
    var pattern = "";
    for (var i = 0; i < SBOX_START.length; i++) {
        pattern += (SBOX_START[i] < 16 ? "0" : "") + SBOX_START[i].toString(16) + " ";
    }

    console.log("\\nSearching for AES S-box pattern: " + pattern);

    Memory.scan(mainModule.base, mainModule.size, pattern.trim(), {
        onMatch: function(address, size) {
            console.log("*** Found AES S-box at: " + address);
            // This is where the AES tables are - the implementation is nearby
        },
        onComplete: function() {
            console.log("S-box scan complete");
        }
    });
}

// Hook file operations to detect when .sag files are read
function hookFileReads() {
    var CreateFileW = Module.findExportByName("kernel32.dll", "CreateFileW");
    var ReadFile = Module.findExportByName("kernel32.dll", "ReadFile");

    if (CreateFileW) {
        Interceptor.attach(CreateFileW, {
            onEnter: function(args) {
                var filename = args[0].readUtf16String();
                if (filename && filename.indexOf(".sag") !== -1) {
                    console.log("\\n*** Opening .sag file: " + filename);
                    this.isSagFile = true;
                }
            },
            onLeave: function(retval) {
                if (this.isSagFile) {
                    console.log("File handle: " + retval);
                }
            }
        });
    }
}

// Look for "DecryptAES256_CTR" string and hook nearby functions
function findDecryptFunction() {
    var modules = Process.enumerateModules();
    var mainModule = modules[0];

    // Search for the string
    var searchStr = "DecryptAES256_CTR";
    var pattern = "";
    for (var i = 0; i < searchStr.length; i++) {
        pattern += searchStr.charCodeAt(i).toString(16) + " ";
    }

    console.log("\\nSearching for '" + searchStr + "'...");

    Memory.scan(mainModule.base, mainModule.size, pattern.trim(), {
        onMatch: function(address, size) {
            console.log("Found string at: " + address);

            // Look for function pointers near this string
            // In NativeAOT, the code reference is usually within 4KB
            var searchStart = address.sub(4096);
            var searchEnd = address.add(4096);

            console.log("Examining memory region around string...");

            // Dump some bytes around the string for analysis
            try {
                var before = address.sub(64).readByteArray(64);
                var after = address.add(searchStr.length).readByteArray(64);
                console.log("64 bytes before: " + hexdump(before, {length: 64}));
            } catch(e) {}
        },
        onComplete: function() {
            console.log("String search complete");
        }
    });
}

// Hook any function that receives a 32-byte aligned buffer
function monitorMemoryAccess() {
    // This is expensive but can help find the key
    console.log("\\nMonitoring started. Play the game and let it load files...");
}

// Main
console.log("=".repeat(60));
console.log("Frida AES Key Capture for ClassicUO");
console.log("=".repeat(60));

findAesImplementation();
findDecryptFunction();
hookFileReads();

console.log("\\n[*] Hooks installed. Waiting for activity...");
console.log("[*] If you restart ClassicUO, it will capture .sag file reads");
"""

def main():
    process_name = "ClassicUO.exe"

    print(f"Attaching to {process_name}...")

    try:
        session = frida.attach(process_name)
    except frida.ProcessNotFoundError:
        print(f"ERROR: {process_name} not found!")
        print("Please start ClassicUO first, then run this script.")
        sys.exit(1)

    print("Attached! Injecting hooks...")

    script = session.create_script(HOOK_SCRIPT)

    def on_message(message, data):
        if message['type'] == 'send':
            print(f"[FRIDA] {message['payload']}")
        elif message['type'] == 'error':
            print(f"[ERROR] {message['stack']}")
        else:
            print(f"[MSG] {message}")

    script.on('message', on_message)
    script.load()

    print("\n" + "="*60)
    print("Script running. Press Ctrl+C to stop.")
    print("="*60 + "\n")

    try:
        sys.stdin.read()
    except KeyboardInterrupt:
        print("\nDetaching...")
        session.detach()

if __name__ == "__main__":
    main()
