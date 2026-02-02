/*
 * Frida script to find AES key by hooking memory reads near AES operations.
 *
 * Since AES-NI loads the key from memory into XMM registers before encryption,
 * we can try to find where 32-byte aligned reads happen.
 */

// Known IV from tiledata.sag - if we see this, we're near the decryption
var knownIV = "ce0842d2405328b7f5a190af9e2fe0d2";

// Search for any function that looks like it handles encryption
var modules = Process.enumerateModules();

console.log("=".repeat(60));
console.log("Searching for AES-related functions...");
console.log("=".repeat(60));

modules.forEach(function(mod) {
    if (mod.name.toLowerCase().includes("classicuo")) {
        console.log("\nScanning " + mod.name + " at " + mod.base);

        // Look for the known strings we found earlier
        var patterns = [
            "55 4F 46 69 6C 65 53 61 67",  // "UOFileSag"
            "44 65 63 72 79 70 74",         // "Decrypt"
        ];

        patterns.forEach(function(pattern) {
            try {
                var matches = Memory.scanSync(mod.base, mod.size, pattern);
                matches.forEach(function(match) {
                    console.log("  Found pattern at: " + match.address);

                    // Dump surrounding bytes
                    var context = match.address.sub(16).readByteArray(64);
                    console.log("  Context: " + hexdump(context, {offset: 0, length: 64}));
                });
            } catch(e) {}
        });
    }
});

// Try to intercept any function that reads our known IV
console.log("\n" + "=".repeat(60));
console.log("Setting up memory access monitoring...");
console.log("=".repeat(60));

// Convert IV to bytes for comparison
var ivBytes = [];
for (var i = 0; i < knownIV.length; i += 2) {
    ivBytes.push(parseInt(knownIV.substr(i, 2), 16));
}

// Hook VirtualAlloc to catch when large buffers are allocated (might be for file data)
var virtualAlloc = Module.findExportByName("kernel32.dll", "VirtualAlloc");
if (virtualAlloc) {
    Interceptor.attach(virtualAlloc, {
        onEnter: function(args) {
            this.size = args[1].toInt32();
        },
        onLeave: function(retval) {
            if (this.size > 1000000 && this.size < 10000000) {
                console.log("VirtualAlloc: " + this.size + " bytes at " + retval);
            }
        }
    });
}

// Hook CreateFileW to see when .sag files are opened
var createFileW = Module.findExportByName("kernel32.dll", "CreateFileW");
if (createFileW) {
    Interceptor.attach(createFileW, {
        onEnter: function(args) {
            var filename = args[0].readUtf16String();
            if (filename && filename.toLowerCase().includes(".sag")) {
                console.log("\n*** Opening .sag file: " + filename);
            }
        }
    });
}

// Hook ReadFile to catch when .sag data is read
var readFile = Module.findExportByName("kernel32.dll", "ReadFile");
if (readFile) {
    Interceptor.attach(readFile, {
        onEnter: function(args) {
            this.buffer = args[1];
            this.size = args[2].toInt32();
        },
        onLeave: function(retval) {
            if (this.size >= 16 && this.size <= 4096) {
                try {
                    var data = this.buffer.readByteArray(Math.min(this.size, 32));
                    var hex = Array.from(new Uint8Array(data)).map(b => ('0' + b.toString(16)).slice(-2)).join('');

                    // Check if this looks like our IV
                    if (hex.startsWith(knownIV.substring(0, 16))) {
                        console.log("\n*** FOUND IV in ReadFile! ***");
                        console.log("  Buffer: " + this.buffer);
                        console.log("  Data: " + hex);

                        // Dump the call stack
                        console.log("  Backtrace:");
                        console.log(Thread.backtrace(this.context, Backtracer.ACCURATE)
                            .map(DebugSymbol.fromAddress).join('\n'));
                    }
                } catch(e) {}
            }
        }
    });
}

console.log("\nHooks installed. Use the game to trigger .sag file loading...");
console.log("Look for '*** FOUND IV ***' messages.\n");
