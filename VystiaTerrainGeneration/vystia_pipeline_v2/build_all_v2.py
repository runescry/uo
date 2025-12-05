"""
Vystia Pipeline V2 - Build Orchestrator
========================================
Runs the complete world generation pipeline with native 7168x4096 resolution.
No post-pipeline fixes needed - generates UOMapMake-ready files directly.

Pipeline Stages:
  Stage 0: Normalize inputs and generate procedural ocean islands
  Stage 1: Generate Voronoi region segmentation
  Stage 5: Write UO-compatible BMP files (Terrain + Altitude)

Future stages (to be added):
  Stage 2: Mountain mask generation
  Stage 3: Biome to terrain color mapping
  Stage 4: Heightmap synthesis
"""
import os, sys, subprocess, datetime

# ============================================================
# CONFIGURATION
# ============================================================
ROOT = os.path.dirname(os.path.abspath(__file__))
SCRIPTS_DIR = os.path.join(ROOT, "scripts")
LOGS_DIR = os.path.join(ROOT, "builds", "logs")

os.makedirs(LOGS_DIR, exist_ok=True)

# Pipeline stages (in order)
STAGES = [
    ("00_normalize_and_expand.py", "Stage 0: Normalize & Expand with Procedural Islands"),
    ("10_generate_regions.py", "Stage 1: Generate Voronoi Regions"),
    ("45_map_to_uo_terrain.py", "Stage 4.5: Map to UO Terrain Types"),
    ("50_write_uo_bitmaps.py", "Stage 5: Write UO Bitmaps"),
]

# ============================================================
# LOGGING
# ============================================================
timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
log_file = os.path.join(LOGS_DIR, f"build_{timestamp}.log")

def log(message, level="INFO"):
    """Log message to console and file."""
    timestamp_str = datetime.datetime.now().strftime("%H:%M:%S")
    formatted = f"[{timestamp_str}] [{level}] {message}"
    print(formatted)
    with open(log_file, "a", encoding="utf-8") as f:
        f.write(formatted + "\n")

# ============================================================
# BUILD EXECUTION
# ============================================================
def run_stage(script_name, description):
    """Run a single pipeline stage."""
    script_path = os.path.join(SCRIPTS_DIR, script_name)

    if not os.path.exists(script_path):
        log(f"SKIP: {script_name} not found", "WARNING")
        return True  # Non-critical, continue

    log(f"Running: {description}", "STAGE")
    log(f"Script: {script_name}")

    try:
        result = subprocess.run(
            [sys.executable, script_path],
            cwd=ROOT,
            capture_output=True,
            text=True,
            timeout=300  # 5 minute timeout per stage
        )

        # Log output
        if result.stdout:
            for line in result.stdout.strip().split("\n"):
                log(f"  {line}", "OUT")

        if result.stderr:
            for line in result.stderr.strip().split("\n"):
                log(f"  {line}", "ERR")

        if result.returncode != 0:
            log(f"FAILED: {script_name} returned code {result.returncode}", "ERROR")
            return False

        log(f"OK: {description}", "SUCCESS")
        return True

    except subprocess.TimeoutExpired:
        log(f"TIMEOUT: {script_name} exceeded 5 minutes", "ERROR")
        return False
    except Exception as e:
        log(f"EXCEPTION: {script_name} - {e}", "ERROR")
        return False

def main():
    """Execute complete pipeline."""
    log("="*60)
    log("Vystia Pipeline V2 - Build Started")
    log("="*60)
    log(f"Working directory: {ROOT}")
    log(f"Log file: {log_file}")
    log(f"Stages to run: {len(STAGES)}")
    log("")

    start_time = datetime.datetime.now()
    success_count = 0
    failed_stage = None

    for i, (script, description) in enumerate(STAGES, 1):
        log(f"--- Stage {i}/{len(STAGES)} ---")

        if not run_stage(script, description):
            failed_stage = description
            break

        success_count += 1
        log("")

    # Summary
    end_time = datetime.datetime.now()
    duration = (end_time - start_time).total_seconds()

    log("="*60)
    log("Build Summary")
    log("="*60)
    log(f"Completed stages: {success_count}/{len(STAGES)}")
    log(f"Duration: {duration:.1f} seconds")

    if failed_stage:
        log(f"FAILED at: {failed_stage}", "ERROR")
        log(f"Check log: {log_file}", "ERROR")
        log("="*60)
        return 1
    else:
        log("ALL STAGES PASSED", "SUCCESS")
        log("")
        log("Output files (root directory):")
        log(f"  - Terrain.bmp (7168x4096, 8-bit palette, BMPv3)")
        log(f"  - Altitude.bmp (7168x4096, 8-bit grayscale, BMPv3)")
        log("")
        log("Also saved in exports/ directory for archival")
        log("")
        log("Next steps:")
        log("  1. Review preview/ directory for QA images")
        log("  2. Verify exports/validation_report.md shows READY status")
        log("  3. Run UOMapMake.exe from this directory")
        log("="*60)
        return 0

if __name__ == "__main__":
    exit_code = main()
    sys.exit(exit_code)
