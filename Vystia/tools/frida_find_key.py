"""
Frida script to find AES key by searching for key-related patterns.
Based on finding "MatchKey" and "SelectKey" near DecryptAES256_CTR.
"""
import frida
import sys

HOOK_SCRIPT = """
// Search for key-related strings and memory patterns
function searchForKeys() {
    var modules = Process.enumerateModules();
    var mainModule = modules[0];
    var base = mainModule.base;
    var size = mainModule.size;

    console.log("Module: " + mainModule.name);
    console.log("Base: " + base);
    console.log("Size: " + size);

    // Key-related strings to search for
    var keyStrings = [
        "MatchKey",
        "SelectKey",
        "DecryptAES256_CTR",
        "AES256_CTR",
        "OpenAesAlgorithm",
        "UOFileSag"
    ];

    console.log("\\n=== Searching for key-related strings ===");

    keyStrings.forEach(function(searchStr) {
        var pattern = "";
        for (var i = 0; i < searchStr.length; i++) {
            pattern += searchStr.charCodeAt(i).toString(16) + " ";
        }

        Memory.scan(base, size, pattern.trim(), {
            onMatch: function(address, matchSize) {
                console.log("\\n'" + searchStr + "' at " + address);

                // Dump surrounding bytes
                try {
                    var before = address.sub(32);
                    var region = before.readByteArray(128);
                    console.log(hexdump(region, {offset: 0, length: 128, header: true, ansi: false}));
                } catch(e) {
                    console.log("  (cannot read memory)");
                }
            },
            onComplete: function() {}
        });
    });
}

// Search for 32-byte high-entropy sequences (potential keys)
function searchForKeyBytes() {
    var modules = Process.enumerateModules();
    var mainModule = modules[0];

    console.log("\\n=== Searching for potential 32-byte keys ===");

    // Known location: DecryptAES256_CTR is at 0x7ff706fb7694
    // Search in the .rdata section (typically after code)
    var searchBase = mainModule.base.add(0x800000); // Start from ~8MB into module
    var searchSize = 0x400000; // Search 4MB

    console.log("Searching from " + searchBase + " for " + searchSize + " bytes");

    // Look for high-entropy 32-byte sequences
    var candidates = [];
    var ptr = searchBase;
    var endPtr = searchBase.add(searchSize);

    while (ptr.compare(endPtr) < 0) {
        try {
            var bytes = ptr.readByteArray(32);
            var arr = new Uint8Array(bytes);

            // Check if it looks like a key
            var zeros = 0;
            var unique = new Set();
            for (var i = 0; i < 32; i++) {
                if (arr[i] === 0) zeros++;
                unique.add(arr[i]);
            }

            // Good key candidates: few zeros, many unique bytes
            if (zeros < 4 && unique.size > 20) {
                // Check it's not ASCII text
                var nonPrintable = 0;
                for (var j = 0; j < 32; j++) {
                    if (arr[j] < 32 || arr[j] > 126) nonPrintable++;
                }

                if (nonPrintable > 16) {
                    candidates.push({
                        address: ptr,
                        hex: Array.from(arr).map(b => ('0' + b.toString(16)).slice(-2)).join('')
                    });

                    if (candidates.length <= 10) {
                        console.log("Candidate at " + ptr + ": " + candidates[candidates.length-1].hex);
                    }
                }
            }
        } catch(e) {}

        ptr = ptr.add(16); // Check every 16 bytes
    }

    console.log("\\nFound " + candidates.length + " potential key candidates");
    return candidates;
}

// Look at the specific area around DecryptAES256_CTR
function examineDecryptArea() {
    console.log("\\n=== Examining area around DecryptAES256_CTR ===");

    // The string was found at 0x7ff706fb7694
    // Let's look at a wider region
    var decryptStringAddr = ptr("0x7ff706fb7694");

    console.log("DecryptAES256_CTR string location: " + decryptStringAddr);

    // Dump 512 bytes before and after
    try {
        var start = decryptStringAddr.sub(256);
        var region = start.readByteArray(512);
        console.log("\\n256 bytes before to 256 bytes after:");
        console.log(hexdump(region, {offset: 0, length: 512, header: true, ansi: false}));
    } catch(e) {
        console.log("Error reading memory: " + e);
    }

    // Look for patterns that might be key bytes (32-byte aligned data)
    console.log("\\n=== Looking for aligned data near DecryptAES256_CTR ===");
    for (var offset = -1024; offset <= 1024; offset += 32) {
        try {
            var checkAddr = decryptStringAddr.add(offset);
            var bytes = checkAddr.readByteArray(32);
            var arr = new Uint8Array(bytes);

            var zeros = 0;
            var unique = new Set();
            for (var i = 0; i < 32; i++) {
                if (arr[i] === 0) zeros++;
                unique.add(arr[i]);
            }

            if (zeros < 4 && unique.size > 16) {
                var hex = Array.from(arr).map(b => ('0' + b.toString(16)).slice(-2)).join('');
                console.log("Offset " + offset + " (" + checkAddr + "): " + hex);
            }
        } catch(e) {}
    }
}

// Main execution
console.log("=".repeat(60));
console.log("Frida Key Finder for ClassicUO");
console.log("=".repeat(60));

searchForKeys();
examineDecryptArea();
// searchForKeyBytes(); // Uncomment for broader search

console.log("\\n" + "=".repeat(60));
console.log("Analysis complete!");
console.log("=".repeat(60));
"""

def main():
    process_name = "ClassicUO.exe"
    print(f"Attaching to {process_name}...")

    try:
        session = frida.attach(process_name)
    except frida.ProcessNotFoundError:
        print(f"ERROR: {process_name} not found!")
        print("Please start ClassicUO first.")
        sys.exit(1)

    print("Attached! Running analysis...")

    script = session.create_script(HOOK_SCRIPT)

    def on_message(message, data):
        if message['type'] == 'send':
            print(message['payload'])
        elif message['type'] == 'error':
            print(f"[ERROR] {message['stack']}")

    script.on('message', on_message)
    script.load()

    print("\nPress Enter to detach...")
    input()
    session.detach()

if __name__ == "__main__":
    main()
