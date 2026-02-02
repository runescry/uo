"""
Use frida-trace to hook BCrypt functions by spawning ClassicUO.
"""
import subprocess
import sys
import os

def main():
    classicuo_path = r"D:\Ultima Online - Sagas\ClassicUO\ClassicUO.exe"

    if not os.path.exists(classicuo_path):
        print(f"ERROR: ClassicUO not found at {classicuo_path}")
        sys.exit(1)

    print("Spawning ClassicUO with BCrypt hooks...")
    print("=" * 60)
    print("Look for '*** AES KEY: ***' in the output!")
    print("=" * 60)
    print()

    # Run frida-trace with spawn mode (-f)
    cmd = ['frida-trace', '-f', classicuo_path, '-i', 'BCrypt*']
    print(f"Command: {' '.join(cmd)}\n")

    try:
        subprocess.run(cmd)
    except FileNotFoundError:
        # Try with full path
        import glob
        patterns = [
            os.path.expanduser(r'~\AppData\Local\Packages\*Python*\LocalCache\local-packages\Python*\Scripts\frida-trace.exe'),
        ]
        for pattern in patterns:
            matches = glob.glob(pattern)
            if matches:
                subprocess.run([matches[0], '-f', classicuo_path, '-i', 'BCrypt*'])
                return
        print("Could not find frida-trace")

if __name__ == "__main__":
    main()
