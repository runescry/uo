"""
Check configuration files, registry, and environment variables for encryption keys
"""
import os
import json
import winreg
import base64

def check_settings_json():
    """Check settings.json for potential keys"""
    config_path = r"D:\Ultima Online - Sagas\ClassicUO\settings.json"
    
    print("="*60)
    print("Checking settings.json...")
    print("="*60)
    
    if not os.path.exists(config_path):
        print("settings.json not found")
        return []
    
    keys = []
    try:
        with open(config_path, 'r') as f:
            config = json.load(f)
        
        print("Config contents:")
        for key, value in config.items():
            print(f"  {key}: {value}")
            if isinstance(value, str) and len(value) >= 16:
                # Try to decode as base64
                try:
                    decoded = base64.b64decode(value)
                    if len(decoded) == 32 or len(decoded) == 16:
                        keys.append(decoded)
                        print(f"    -> Decoded as base64: {decoded.hex()}")
                except:
                    pass
                # Try to decode as hex
                try:
                    if len(value) == 64:  # 32 bytes in hex
                        decoded = bytes.fromhex(value)
                        keys.append(decoded)
                        print(f"    -> Decoded as hex (32 bytes): {decoded.hex()}")
                    elif len(value) == 32:  # 16 bytes in hex
                        decoded = bytes.fromhex(value)
                        keys.append(decoded)
                        print(f"    -> Decoded as hex (16 bytes): {decoded.hex()}")
                except:
                    pass
    except Exception as e:
        print(f"Error reading config: {e}")
    
    return keys

def check_registry():
    """Check Windows registry for keys"""
    print("\n" + "="*60)
    print("Checking Windows Registry...")
    print("="*60)
    
    keys = []
    registry_paths = [
        (winreg.HKEY_CURRENT_USER, r"Software\UOSagas"),
        (winreg.HKEY_CURRENT_USER, r"Software\Ultima Online Sagas"),
        (winreg.HKEY_LOCAL_MACHINE, r"Software\UOSagas"),
        (winreg.HKEY_LOCAL_MACHINE, r"Software\Ultima Online Sagas"),
        (winreg.HKEY_CURRENT_USER, r"Software\ClassicUO"),
    ]
    
    for hkey, path in registry_paths:
        try:
            with winreg.OpenKey(hkey, path) as key:
                print(f"\nFound registry key: {path}")
                for i in range(winreg.QueryInfoKey(key)[1]):
                    try:
                        name, value, value_type = winreg.EnumValue(key, i)
                        print(f"  {name}: {value} (type: {value_type})")
                        
                        if isinstance(value, (str, bytes)):
                            val_bytes = value.encode('utf-8') if isinstance(value, str) else value
                            if len(val_bytes) >= 16:
                                keys.append(val_bytes)
                    except:
                        pass
        except FileNotFoundError:
            print(f"Registry key not found: {path}")
        except Exception as e:
            print(f"Error accessing {path}: {e}")
    
    return keys

def check_environment_variables():
    """Check environment variables for keys"""
    print("\n" + "="*60)
    print("Checking Environment Variables...")
    print("="*60)
    
    keys = []
    relevant_vars = [
        'UOSAGAS_KEY',
        'SAGAS_KEY',
        'UO_KEY',
        'ENCRYPTION_KEY',
        'SAGAS_ENCRYPTION_KEY',
        'UOSAGAS_ENCRYPTION_KEY',
    ]
    
    for var in relevant_vars:
        value = os.environ.get(var)
        if value:
            print(f"Found {var}: {value}")
            try:
                # Try base64
                decoded = base64.b64decode(value)
                if len(decoded) == 32 or len(decoded) == 16:
                    keys.append(decoded)
                    print(f"  -> Decoded as base64: {decoded.hex()}")
            except:
                pass
            try:
                # Try hex
                if len(value) == 64 or len(value) == 32:
                    decoded = bytes.fromhex(value)
                    keys.append(decoded)
                    print(f"  -> Decoded as hex: {decoded.hex()}")
            except:
                pass
        else:
            print(f"{var}: Not set")
    
    return keys

def check_other_config_files():
    """Check for other config files in Sagas directory"""
    print("\n" + "="*60)
    print("Checking for other config files...")
    print("="*60)
    
    base_dir = r"D:\Ultima Online - Sagas"
    config_extensions = ['.json', '.xml', '.ini', '.cfg', '.conf', '.txt']
    keys = []
    
    if os.path.exists(base_dir):
        for root, dirs, files in os.walk(base_dir):
            for file in files:
                if any(file.endswith(ext) for ext in config_extensions):
                    filepath = os.path.join(root, file)
                    try:
                        with open(filepath, 'r', encoding='utf-8', errors='ignore') as f:
                            content = f.read(10000)  # Read first 10KB
                            # Look for key-like strings
                            if 'key' in content.lower() or 'encrypt' in content.lower():
                                print(f"Found potential config: {filepath}")
                                # Try to extract base64 or hex strings
                                import re
                                # Base64 patterns
                                b64_pattern = r'[A-Za-z0-9+/]{32,}={0,2}'
                                for match in re.finditer(b64_pattern, content):
                                    try:
                                        decoded = base64.b64decode(match.group())
                                        if len(decoded) == 32 or len(decoded) == 16:
                                            keys.append(decoded)
                                            print(f"  -> Found base64 key: {decoded.hex()}")
                                    except:
                                        pass
                                # Hex patterns
                                hex_pattern = r'[0-9a-fA-F]{32,64}'
                                for match in re.finditer(hex_pattern, content):
                                    try:
                                        if len(match.group()) == 64:  # 32 bytes
                                            decoded = bytes.fromhex(match.group())
                                            keys.append(decoded)
                                            print(f"  -> Found hex key (32 bytes): {decoded.hex()}")
                                        elif len(match.group()) == 32:  # 16 bytes
                                            decoded = bytes.fromhex(match.group())
                                            keys.append(decoded)
                                            print(f"  -> Found hex IV (16 bytes): {decoded.hex()}")
                                    except:
                                        pass
                    except:
                        pass
    
    return keys

if __name__ == "__main__":
    all_keys = []
    
    all_keys.extend(check_settings_json())
    all_keys.extend(check_registry())
    all_keys.extend(check_environment_variables())
    all_keys.extend(check_other_config_files())
    
    print("\n" + "="*60)
    print("Summary")
    print("="*60)
    print(f"Found {len(all_keys)} potential keys/IVs from config sources")
    
    if all_keys:
        print("\nPotential keys/IVs to test:")
        for i, key in enumerate(all_keys):
            print(f"  {i+1}. {key.hex()} ({len(key)} bytes)")
