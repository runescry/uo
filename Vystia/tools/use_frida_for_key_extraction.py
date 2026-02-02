"""
Alternative approach: Use Frida to hook into ClassicUO.exe and extract key/IV
Frida is easier to use than DynamoRIO for this purpose
"""
import frida
import sys
import json

def on_message(message, data):
    if message['type'] == 'send':
        payload = message['payload']
        if 'key' in payload:
            print(f"\n[KEY FOUND] {payload['key']}")
        if 'iv' in payload:
            print(f"[IV FOUND]  {payload['iv']}")
        if 'decrypt_call' in payload:
            print(f"[DECRYPT] Called at {payload['address']}")
    elif message['type'] == 'error':
        print(f"[ERROR] {message['stack']}")

# Frida script to hook AES decryption
frida_script = """
// Hook Windows CryptoAPI functions
var crypt32 = Module.load('crypt32.dll');
var advapi32 = Module.load('advapi32.dll');

// Hook CryptDecrypt
var CryptDecrypt = advapi32.getExportByName('CryptDecrypt');
if (CryptDecrypt) {
    Interceptor.attach(CryptDecrypt, {
        onEnter: function(args) {
            console.log('[CryptDecrypt] Called');
            // Try to read key material
            // This depends on the crypto provider used
        }
    });
}

// Hook BCryptDecrypt (CNG - newer Windows crypto)
var bcrypt = Module.load('bcrypt.dll');
var BCryptDecrypt = bcrypt.getExportByName('BCryptDecrypt');
if (BCryptDecrypt) {
    Interceptor.attach(BCryptDecrypt, {
        onEnter: function(args) {
            console.log('[BCryptDecrypt] Called');
            // args[3] might be the key handle
            // args[4] might be the IV
        },
        onLeave: function(retval) {
            console.log('[BCryptDecrypt] Returned');
        }
    });
}

// Look for custom AES implementation
// Search for the DecryptAES256_CTR function we found in binary analysis
Process.enumerateModules().forEach(function(module) {
    if (module.name.toLowerCase().includes('classicuo')) {
        console.log('Found ClassicUO module: ' + module.name);
        console.log('Base: ' + module.base);
        console.log('Size: ' + module.size);
        
        // Try to find DecryptAES256_CTR by searching for the string
        var decryptFunc = null;
        try {
            // Search for the function name in memory
            var pattern = 'DecryptAES256_CTR';
            var results = Memory.scan(module.base, module.size, pattern);
            results.forEach(function(match) {
                console.log('Found DecryptAES256_CTR reference at: ' + match.address);
            });
        } catch(e) {
            console.log('Error searching: ' + e);
        }
    }
});

// Hook all function calls that might be decryption
// This is a broad approach - hook everything and filter
var mainModule = Process.enumerateModules()[0];
console.log('Main module: ' + mainModule.name);

// Try to hook at the offset we found (0xa3c494)
// This would need to be adjusted based on actual runtime address
"""

def main():
    try:
        # Attach to ClassicUO.exe
        print("Attaching to ClassicUO.exe...")
        session = frida.attach("ClassicUO.exe")
        
        print("Creating script...")
        script = session.create_script(frida_script)
        script.on('message', on_message)
        
        print("Loading script...")
        script.load()
        
        print("Monitoring for key/IV extraction...")
        print("Launch Sagas ClassicUO and load a .sag file...")
        print("Press Ctrl+C to stop")
        
        sys.stdin.read()
        
    except frida.ProcessNotFoundError:
        print("Error: ClassicUO.exe not found. Please start the Sagas client first.")
        sys.exit(1)
    except Exception as e:
        print(f"Error: {e}")
        sys.exit(1)

if __name__ == "__main__":
    main()
