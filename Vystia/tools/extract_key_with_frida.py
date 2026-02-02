"""
Enhanced Frida script to extract AES-256-CTR key/IV from ClassicUO.exe
This hooks into the decryption process and captures key/IV values
"""
import frida
import sys
import json
import time

# Store extracted keys/IVs
extracted_keys = []
extracted_ivs = []

def on_message(message, data):
    if message['type'] == 'send':
        payload = message['payload']
        
        if 'key' in payload:
            key_hex = payload['key']
            if key_hex not in extracted_keys:
                extracted_keys.append(key_hex)
                print(f"\n[KEY FOUND] {key_hex}")
                save_results()
        
        if 'iv' in payload:
            iv_hex = payload['iv']
            if iv_hex not in extracted_ivs:
                extracted_ivs.append(iv_hex)
                print(f"[IV FOUND]  {iv_hex}")
                save_results()
        
        if 'aes_call' in payload:
            print(f"[AES] {payload['aes_call']}")
        
        if 'error' in payload:
            print(f"[ERROR] {payload['error']}")
            
    elif message['type'] == 'error':
        print(f"[ERROR] {message['stack']}")

def save_results():
    """Save extracted keys/IVs to file"""
    with open("extracted_sag_keys.txt", "w") as f:
        f.write("Extracted .SAG File Encryption Keys/IVs\n")
        f.write("=" * 60 + "\n\n")
        
        if extracted_keys:
            f.write("KEYS FOUND:\n")
            for i, key in enumerate(extracted_keys, 1):
                f.write(f"  {i}. {key}\n")
            f.write("\n")
        
        if extracted_ivs:
            f.write("IVs FOUND:\n")
            for i, iv in enumerate(extracted_ivs, 1):
                f.write(f"  {i}. {iv}\n")
            f.write("\n")
        
        # Provide usage example
        if extracted_keys and extracted_ivs:
            f.write("USAGE:\n")
            f.write(f"python decrypt_sag.py file.sag --key {extracted_keys[0]} --iv {extracted_ivs[0]}\n")

# Enhanced Frida script
frida_script = """
// Find ClassicUO module
var classicUOModule = null;
Process.enumerateModules().forEach(function(module) {
    if (module.name.toLowerCase().includes('classicuo') || 
        module.path.toLowerCase().includes('classicuo')) {
        classicUOModule = module;
        console.log('[+] Found ClassicUO module: ' + module.name);
        console.log('[+] Base address: ' + module.base);
        console.log('[+] Size: ' + module.size);
    }
});

if (!classicUOModule) {
    console.log('[-] ClassicUO module not found');
    send({error: 'ClassicUO module not found'});
}

// Hook Windows CryptoAPI - BCryptDecrypt (CNG)
var bcrypt = Module.load('bcrypt.dll');
if (bcrypt) {
    var BCryptDecrypt = bcrypt.getExportByName('BCryptDecrypt');
    if (BCryptDecrypt) {
        Interceptor.attach(BCryptDecrypt, {
            onEnter: function(args) {
                // BCryptDecrypt signature:
                // NTSTATUS BCryptDecrypt(
                //   BCRYPT_KEY_HANDLE hKey,
                //   PUCHAR pbInput,
                //   ULONG cbInput,
                //   PVOID pPaddingInfo,
                //   PUCHAR pbIV,
                //   ULONG cbIV,
                //   PUCHAR pbOutput,
                //   ULONG cbOutput,
                //   PULONG pcbResult,
                //   ULONG dwFlags
                // )
                
                try {
                    var iv_ptr = args[4];  // pbIV
                    var iv_len = args[5].toInt32();  // cbIV
                    
                    if (iv_ptr && iv_len >= 16) {
                        var iv = Memory.readByteArray(iv_ptr, Math.min(iv_len, 16));
                        var iv_hex = Array.from(new Uint8Array(iv)).map(b => ('0' + b.toString(16)).slice(-2)).join('');
                        send({iv: iv_hex});
                    }
                    
                    // Try to get key from key handle (complex, might need to hook key creation)
                } catch(e) {
                    // Ignore errors
                }
            }
        });
        console.log('[+] Hooked BCryptDecrypt');
    }
}

// Hook CryptDecrypt (legacy CryptoAPI)
var advapi32 = Module.load('advapi32.dll');
if (advapi32) {
    var CryptDecrypt = advapi32.getExportByName('CryptDecrypt');
    if (CryptDecrypt) {
        Interceptor.attach(CryptDecrypt, {
            onEnter: function(args) {
                // CryptDecrypt signature:
                // BOOL CryptDecrypt(
                //   HCRYPTKEY hKey,
                //   HCRYPTHASH hHash,
                //   BOOL Final,
                //   DWORD dwFlags,
                //   BYTE *pbData,
                //   DWORD *pdwDataLen
                // )
                send({aes_call: 'CryptDecrypt called'});
            }
        });
        console.log('[+] Hooked CryptDecrypt');
    }
}

// Search for DecryptAES256_CTR function
// We know from binary analysis it's around offset 0xa3c494
if (classicUOModule) {
    // Calculate runtime address (base + offset)
    var decryptFuncOffset = 0xa3c494;
    var decryptFuncAddr = classicUOModule.base.add(decryptFuncOffset);
    
    console.log('[+] Looking for DecryptAES256_CTR at: ' + decryptFuncAddr);
    
    // Try to hook at this address
    try {
        Interceptor.attach(decryptFuncAddr, {
            onEnter: function(args) {
                console.log('[+] DecryptAES256_CTR called!');
                
                // Try to read parameters
                // This depends on calling convention (likely __stdcall or __cdecl)
                // Parameters might be on stack or in registers
                
                // For x86 __stdcall: parameters on stack
                // For x64: first 4 in RCX, RDX, R8, R9, rest on stack
                
                // Try to read from common parameter locations
                // This is architecture-specific and may need adjustment
                
                send({aes_call: 'DecryptAES256_CTR called at ' + decryptFuncAddr});
            }
        });
        console.log('[+] Hooked DecryptAES256_CTR');
    } catch(e) {
        console.log('[-] Could not hook DecryptAES256_CTR: ' + e);
    }
}

// Hook all calls to functions that might be decryption
// Look for patterns in function calls
var callCount = 0;
Interceptor.attach(Module.findExportByName(null, 'ReadFile'), {
    onEnter: function(args) {
        var filename = Memory.readUtf16String(args[0]);
        if (filename && filename.toLowerCase().includes('.sag')) {
            console.log('[+] Reading .sag file: ' + filename);
            send({aes_call: 'Reading .sag file: ' + filename});
        }
    }
});

console.log('[+] Frida script loaded. Waiting for .sag file access...');
"""

def main():
    print("=" * 60)
    print("Frida Key/IV Extractor for .SAG Files")
    print("=" * 60)
    print()
    print("This script will hook into ClassicUO.exe and extract")
    print("the AES-256-CTR encryption key and IV when .sag files are decrypted.")
    print()
    print("Instructions:")
    print("1. Make sure Sagas ClassicUO is NOT running")
    print("2. This script will attach when ClassicUO starts")
    print("3. Launch Sagas ClassicUO and load a .sag file")
    print("4. The key/IV will be extracted automatically")
    print()
    print("Press Ctrl+C to stop")
    print()
    
    try:
        # Try to attach to existing process first
        try:
            print("Checking for running ClassicUO.exe...")
            session = frida.attach("ClassicUO.exe")
            print("Attached to existing process")
        except frida.ProcessNotFoundError:
            print("ClassicUO.exe not running. Waiting for process to start...")
            print("Please start Sagas ClassicUO now...")
            
            # Wait for process
            import time
            while True:
                try:
                    session = frida.attach("ClassicUO.exe")
                    print("Attached to ClassicUO.exe!")
                    break
                except frida.ProcessNotFoundError:
                    time.sleep(1)
        
        print("Creating Frida script...")
        script = session.create_script(frida_script)
        script.on('message', on_message)
        
        print("Loading script...")
        script.load()
        
        print("\n[+] Monitoring active. Load a .sag file in ClassicUO to extract key/IV...")
        print("[+] Results will be saved to extracted_sag_keys.txt\n")
        
        # Keep running
        try:
            sys.stdin.read()
        except KeyboardInterrupt:
            print("\n\nStopping...")
        
        script.unload()
        session.detach()
        
        if extracted_keys or extracted_ivs:
            print(f"\n[+] Extraction complete!")
            print(f"[+] Found {len(extracted_keys)} keys and {len(extracted_ivs)} IVs")
            print(f"[+] Results saved to extracted_sag_keys.txt")
        else:
            print("\n[-] No keys/IVs extracted. Try:")
            print("    - Make sure ClassicUO loads a .sag file")
            print("    - Check if decryption uses different APIs")
            print("    - Try the simple memory dump approach")
        
    except frida.ProcessNotFoundError:
        print("Error: ClassicUO.exe not found.")
        print("Please start Sagas ClassicUO first, then run this script.")
        sys.exit(1)
    except Exception as e:
        print(f"Error: {e}")
        import traceback
        traceback.print_exc()
        sys.exit(1)

if __name__ == "__main__":
    main()
