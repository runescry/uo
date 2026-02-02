"""
Convert decrypted .sag files to .mul format for use with UO Fiddler and CentrED
This tool handles batch conversion and preserves .idx files
"""
import os
import sys
import shutil
import argparse
from pathlib import Path

def convert_sag_to_mul(sag_path, mul_path=None, preserve_idx=True):
    """
    Convert a decrypted .sag file to .mul format
    
    Args:
        sag_path: Path to decrypted .sag file (or .mul file that was decrypted from .sag)
        mul_path: Output .mul path (default: same as sag_path but with .mul extension)
        preserve_idx: If True, copy corresponding .idx file
    
    Returns:
        Path to created .mul file
    """
    if not os.path.exists(sag_path):
        raise FileNotFoundError(f"File not found: {sag_path}")
    
    # Determine output path
    if not mul_path:
        if sag_path.endswith('.sag'):
            mul_path = sag_path[:-4] + '.mul'
        elif sag_path.endswith('.decrypted'):
            mul_path = sag_path[:-10] + '.mul'
        else:
            mul_path = sag_path + '.mul'
    
    # Copy file (it's already decrypted, just needs renaming)
    shutil.copy2(sag_path, mul_path)
    print(f"Created: {mul_path}")
    
    # Handle .idx file
    if preserve_idx:
        idx_path = None
        # Try to find corresponding .idx file
        base_name = os.path.basename(sag_path)
        
        # Check various .idx naming patterns
        possible_idx_names = [
            base_name.replace('.sag', '.idx'),
            base_name.replace('.mul', '.idx'),
            base_name.replace('.decrypted', '.idx'),
        ]
        
        # Also try with different case
        for name in possible_idx_names:
            possible_paths = [
                os.path.join(os.path.dirname(sag_path), name),
                os.path.join(os.path.dirname(sag_path), name.lower()),
                os.path.join(os.path.dirname(sag_path), name.upper()),
            ]
            for path in possible_paths:
                if os.path.exists(path):
                    idx_path = path
                    break
            if idx_path:
                break
        
        if idx_path:
            # .idx files are already in correct format, just ensure they exist
            output_idx = os.path.join(os.path.dirname(mul_path), os.path.basename(mul_path).replace('.mul', '.idx'))
            if not os.path.exists(output_idx) or os.path.abspath(idx_path) != os.path.abspath(output_idx):
                shutil.copy2(idx_path, output_idx)
                print(f"Copied index: {output_idx}")
        else:
            print(f"Warning: Could not find corresponding .idx file for {sag_path}")
    
    return mul_path

def batch_convert(input_dir, output_dir=None, pattern='*.sag', preserve_idx=True):
    """
    Batch convert multiple .sag files to .mul format
    
    Args:
        input_dir: Directory containing .sag files
        output_dir: Output directory (default: same as input_dir)
        pattern: File pattern to match (default: *.sag)
        preserve_idx: If True, copy corresponding .idx files
    
    Returns:
        List of converted file paths
    """
    if not os.path.exists(input_dir):
        raise FileNotFoundError(f"Directory not found: {input_dir}")
    
    if not output_dir:
        output_dir = input_dir
    
    os.makedirs(output_dir, exist_ok=True)
    
    # Find all matching files
    input_path = Path(input_dir)
    sag_files = list(input_path.glob(pattern))
    
    # Also look for .decrypted files
    decrypted_files = list(input_path.glob('*.decrypted'))
    all_files = sag_files + decrypted_files
    
    if not all_files:
        print(f"No files found matching pattern '{pattern}' in {input_dir}")
        return []
    
    print(f"Found {len(all_files)} files to convert")
    print()
    
    converted = []
    for file_path in all_files:
        try:
            output_mul = os.path.join(output_dir, file_path.stem + '.mul')
            mul_path = convert_sag_to_mul(str(file_path), output_mul, preserve_idx)
            converted.append(mul_path)
        except Exception as e:
            print(f"Error converting {file_path}: {e}", file=sys.stderr)
    
    print(f"\nConverted {len(converted)} files")
    return converted

def main():
    parser = argparse.ArgumentParser(description='Convert decrypted .sag files to .mul format')
    parser.add_argument('input', help='Input .sag file or directory')
    parser.add_argument('--output', help='Output .mul file or directory')
    parser.add_argument('--batch', action='store_true', help='Batch convert all .sag files in directory')
    parser.add_argument('--pattern', default='*.sag', help='File pattern for batch mode (default: *.sag)')
    parser.add_argument('--no-idx', action='store_true', help='Do not copy .idx files')
    
    args = parser.parse_args()
    
    try:
        if args.batch or os.path.isdir(args.input):
            # Batch mode
            converted = batch_convert(args.input, args.output, args.pattern, not args.no_idx)
            if converted:
                print(f"\nSuccessfully converted {len(converted)} files")
        else:
            # Single file mode
            mul_path = convert_sag_to_mul(args.input, args.output, not args.no_idx)
            print(f"\nSuccessfully converted to: {mul_path}")
    
    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)

if __name__ == "__main__":
    main()
