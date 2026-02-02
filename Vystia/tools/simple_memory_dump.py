"""
Simple approach: Use Windows API to read process memory and search for key patterns
This is simpler than DynamoRIO/Frida but requires the process to be running
"""
import ctypes
from ctypes import wintypes
import struct

# Windows API constants
PROCESS_QUERY_INFORMATION = 0x0400
PROCESS_VM_READ = 0x0010
MEM_COMMIT = 0x1000
PAGE_READONLY = 0x02
PAGE_READWRITE = 0x04

# Windows API functions
kernel32 = ctypes.windll.kernel32
OpenProcess = kernel32.OpenProcess
ReadProcessMemory = kernel32.ReadProcessMemory
CloseHandle = kernel32.CloseHandle
VirtualQueryEx = kernel32.VirtualQueryEx

class MEMORY_BASIC_INFORMATION(ctypes.Structure):
    _fields_ = [
        ("BaseAddress", ctypes.c_void_p),
        ("AllocationBase", ctypes.c_void_p),
        ("AllocationProtect", wintypes.DWORD),
        ("RegionSize", ctypes.c_size_t),
        ("State", wintypes.DWORD),
        ("Protect", wintypes.DWORD),
        ("Type", wintypes.DWORD),
    ]

def find_process_by_name(process_name):
    """Find process ID by name"""
    import psutil
    for proc in psutil.process_iter(['pid', 'name']):
        if proc.info['name'].lower() == process_name.lower():
            return proc.info['pid']
    return None

def read_process_memory(process_handle, address, size):
    """Read memory from process"""
    buffer = ctypes.create_string_buffer(size)
    bytes_read = ctypes.c_size_t(0)
    
    if ReadProcessMemory(process_handle, address, buffer, size, ctypes.byref(bytes_read)):
        return buffer.raw[:bytes_read.value]
    return None

def search_memory_for_key_pattern(process_handle, base_address, size):
    """Search memory region for potential encryption keys"""
    # Read memory chunk
    data = read_process_memory(process_handle, base_address, min(size, 1024*1024))  # 1MB chunks
    
    if not data:
        return []
    
    keys_found = []
    
    # Look for 32-byte sequences that could be keys
    # Keys should have reasonable entropy (not all zeros, not all same byte)
    for i in range(len(data) - 32):
        key_candidate = data[i:i+32]
        
        # Skip if too repetitive
        if len(set(key_candidate)) < 4:
            continue
        
        # Skip if all zeros or all 0xFF
        if key_candidate == b'\x00' * 32 or key_candidate == b'\xFF' * 32:
            continue
        
        # Calculate entropy
        byte_counts = [0] * 256
        for byte in key_candidate:
            byte_counts[byte] += 1
        
        import math
        entropy = 0
        for count in byte_counts:
            if count > 0:
                p = count / 32
                entropy += -p * math.log2(p)
        
        # Keys should have entropy between 3.0 and 8.0
        if 3.0 < entropy < 8.0:
            keys_found.append((base_address + i, key_candidate, entropy))
    
    return keys_found

def dump_process_memory(process_name="ClassicUO.exe"):
    """Dump and analyze process memory for encryption keys"""
    pid = find_process_by_name(process_name)
    
    if not pid:
        print(f"Process {process_name} not found")
        return
    
    print(f"Found process {process_name} (PID: {pid})")
    
    # Open process
    process_handle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, False, pid)
    
    if not process_handle:
        print(f"Failed to open process. Error: {ctypes.get_last_error()}")
        return
    
    print("Scanning process memory for encryption keys...")
    
    address = 0
    mbi = MEMORY_BASIC_INFORMATION()
    all_keys = []
    
    while VirtualQueryEx(process_handle, address, ctypes.byref(mbi), ctypes.sizeof(mbi)):
        # Only scan committed, readable memory
        if (mbi.State == MEM_COMMIT and 
            (mbi.Protect == PAGE_READONLY or mbi.Protect == PAGE_READWRITE)):
            
            # Search this region
            keys = search_memory_for_key_pattern(process_handle, mbi.BaseAddress, mbi.RegionSize)
            all_keys.extend(keys)
            
            if keys:
                print(f"Found {len(keys)} potential keys in region 0x{address:x}")
        
        address = mbi.BaseAddress + mbi.RegionSize
    
    print(f"\nTotal potential keys found: {len(all_keys)}")
    
    # Save to file
    with open("extracted_keys.txt", "w") as f:
        f.write("Potential Encryption Keys Found in Memory\n")
        f.write("=" * 60 + "\n\n")
        
        for addr, key, entropy in all_keys[:100]:  # Save first 100
            f.write(f"Address: 0x{addr:x}\n")
            f.write(f"Key: {key.hex()}\n")
            f.write(f"Entropy: {entropy:.4f}\n")
            f.write("\n")
    
    print("Results saved to extracted_keys.txt")
    
    CloseHandle(process_handle)

if __name__ == "__main__":
    try:
        dump_process_memory()
    except Exception as e:
        print(f"Error: {e}")
        import traceback
        traceback.print_exc()
