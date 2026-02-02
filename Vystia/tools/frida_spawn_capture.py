"""
Spawn ClassicUO suspended, hook immediately, then resume.
"""
import frida
import sys
import time

SCRIPT = r"""
send('=== Frida hooks loading ===');

// Hook BCrypt (in case it's used)
try {
    var bcrypt = Module.findExportByName('bcrypt.dll', 'BCryptGenerateSymmetricKey');
    if (bcrypt) {
        Interceptor.attach(bcrypt, {
            onEnter: function(args) {
                var keyLen = args[5].toInt32();
                send('[BCrypt] Key len=' + keyLen);
                if (keyLen > 0 && keyLen <= 64) {
                    var arr = new Uint8Array(args[4].readByteArray(keyLen));
                    var hex = Array.from(arr).map(b => ('0'+b.toString(16)).slice(-2)).join('');
                    send('*** BCRYPT KEY: ' + hex + ' ***');
                }
            }
        });
        send('BCrypt hook installed');
    }
} catch(e) {}

// Hook file reads to see when .sag files are accessed
try {
    var CreateFileW = Module.findExportByName('kernel32.dll', 'CreateFileW');
    if (CreateFileW) {
        Interceptor.attach(CreateFileW, {
            onEnter: function(args) {
                try {
                    var filename = args[0].readUtf16String();
                    if (filename && filename.indexOf('.sag') !== -1) {
                        send('[FILE] Opening: ' + filename);
                    }
                } catch(e) {}
            }
        });
        send('File hook installed');
    }
} catch(e) {}

// Hook ReadFile to see data being read
try {
    var ReadFile = Module.findExportByName('kernel32.dll', 'ReadFile');
    if (ReadFile) {
        var sagReads = 0;
        Interceptor.attach(ReadFile, {
            onEnter: function(args) {
                this.buffer = args[1];
                this.bytesToRead = args[2].toInt32();
            },
            onLeave: function(retval) {
                // Only log first few reads
                if (sagReads < 5 && this.bytesToRead > 1000) {
                    sagReads++;
                    send('[READ] ' + this.bytesToRead + ' bytes');
                }
            }
        });
        send('ReadFile hook installed');
    }
} catch(e) {}

send('=== All hooks ready ===');
"""

def main():
    exe = r"D:\Ultima Online - Sagas\ClassicUO\ClassicUO.exe"

    print("=" * 60)
    print("Spawning ClassicUO with immediate hooks...")
    print("=" * 60)

    device = frida.get_local_device()

    try:
        # Spawn suspended
        print("Spawning (suspended)...")
        pid = device.spawn([exe])
        print(f"PID: {pid}")

        # Attach
        print("Attaching...")
        session = device.attach(pid)

        # Load script
        print("Loading hooks...")
        script = session.create_script(SCRIPT)

        def on_message(msg, data):
            if msg['type'] == 'send':
                print(msg['payload'])
            elif msg['type'] == 'error':
                print(f"[ERR] {msg.get('description', msg)}")

        script.on('message', on_message)
        script.load()

        # Resume
        print("Resuming process...")
        device.resume(pid)
        print("\n*** ClassicUO starting - watch for keys! ***\n")

        # Wait
        input("Press Enter to quit...\n")

    except Exception as e:
        print(f"Error: {e}")
        import traceback
        traceback.print_exc()

if __name__ == "__main__":
    main()
