"""
Hook BCrypt functions to capture the AES key.
"""
import frida
import sys

HOOK_SCRIPT = r"""
send("============================================================");
send("Frida BCrypt Hook for ClassicUO");
send("============================================================");

try {
    var addr1 = Module.findExportByName("bcrypt.dll", "BCryptOpenAlgorithmProvider");
    var addr2 = Module.findExportByName("bcrypt.dll", "BCryptGenerateSymmetricKey");
    var addr3 = Module.findExportByName("bcrypt.dll", "BCryptSetProperty");
    var addr4 = Module.findExportByName("bcrypt.dll", "BCryptDecrypt");

    send("BCryptOpenAlgorithmProvider: " + addr1);
    send("BCryptGenerateSymmetricKey: " + addr2);
    send("BCryptSetProperty: " + addr3);
    send("BCryptDecrypt: " + addr4);

    if (addr1) {
        Interceptor.attach(addr1, {
            onEnter: function(args) {
                try {
                    var algId = args[1].readUtf16String();
                    send("[BCryptOpenAlgorithmProvider] Algorithm: " + algId);
                } catch(e) {}
            }
        });
        send("Hooked BCryptOpenAlgorithmProvider");
    }

    if (addr2) {
        Interceptor.attach(addr2, {
            onEnter: function(args) {
                var keyPtr = args[4];
                var keyLen = args[5].toInt32();

                send("!!! BCryptGenerateSymmetricKey - Key length: " + keyLen);

                if (keyLen > 0 && keyLen <= 64) {
                    try {
                        var keyBytes = keyPtr.readByteArray(keyLen);
                        var arr = new Uint8Array(keyBytes);
                        var keyHex = "";
                        for (var i = 0; i < arr.length; i++) {
                            keyHex += ('0' + arr[i].toString(16)).slice(-2);
                        }
                        send("*** AES KEY: " + keyHex + " ***");
                    } catch(e) {
                        send("Error reading key: " + e);
                    }
                }
            }
        });
        send("Hooked BCryptGenerateSymmetricKey");
    }

    if (addr3) {
        Interceptor.attach(addr3, {
            onEnter: function(args) {
                try {
                    var propName = args[1].readUtf16String();
                    var valueLen = args[3].toInt32();
                    send("[BCryptSetProperty] " + propName + " len=" + valueLen);

                    if (valueLen > 0 && valueLen <= 32) {
                        var valueBytes = args[2].readByteArray(valueLen);
                        var arr = new Uint8Array(valueBytes);
                        var hex = "";
                        for (var i = 0; i < arr.length; i++) {
                            hex += ('0' + arr[i].toString(16)).slice(-2);
                        }
                        send("  Value: " + hex);
                    }
                } catch(e) {}
            }
        });
        send("Hooked BCryptSetProperty");
    }

    if (addr4) {
        Interceptor.attach(addr4, {
            onEnter: function(args) {
                send("[BCryptDecrypt] called");
            }
        });
        send("Hooked BCryptDecrypt");
    }

    send("============================================================");
    send("All hooks installed! Restart ClassicUO to capture keys.");
    send("============================================================");

} catch(e) {
    send("Error: " + e);
}
"""

def main():
    print("Attaching to ClassicUO.exe...")

    try:
        session = frida.attach("ClassicUO.exe")
    except frida.ProcessNotFoundError:
        print("ClassicUO.exe not found!")
        sys.exit(1)

    print("Attached!")

    script = session.create_script(HOOK_SCRIPT)

    def on_message(message, data):
        if message['type'] == 'send':
            print(message['payload'])
        else:
            print(message)

    script.on('message', on_message)
    script.load()

    print("\nWaiting... Restart ClassicUO to capture keys.")
    print("Press Enter to quit.\n")

    try:
        input()
    except:
        pass

    session.detach()

if __name__ == "__main__":
    main()
