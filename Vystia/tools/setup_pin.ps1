# PowerShell script to set up Intel Pin and compile the tool

Write-Host "Intel Pin Setup Script" -ForegroundColor Green
Write-Host "=====================" -ForegroundColor Green
Write-Host ""

$pinDir = "D:\Tools\IntelPin"
$pinZip = "$pinDir\pin-3.28-windows.zip"
$pinExtracted = "$pinDir\pin-3.28-98754-gc2c769c81-msvc-windows"

# Check if Pin is already extracted
if (Test-Path "$pinDir\pin.exe") {
    Write-Host "Intel Pin already extracted" -ForegroundColor Yellow
    $pinPath = (Get-Item "$pinDir\pin.exe").DirectoryName
} elseif (Test-Path $pinZip) {
    Write-Host "Extracting Intel Pin..." -ForegroundColor Yellow
    Expand-Archive -Path $pinZip -DestinationPath $pinDir -Force
    
    # Find the extracted directory
    $extractedDirs = Get-ChildItem $pinDir -Directory | Where-Object { $_.Name -like "pin-*" }
    if ($extractedDirs) {
        $pinPath = $extractedDirs[0].FullName
        Write-Host "Pin extracted to: $pinPath" -ForegroundColor Green
    } else {
        Write-Host "Error: Could not find extracted Pin directory" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "Intel Pin not found. Please download:" -ForegroundColor Red
    Write-Host "  https://software.intel.com/sites/landingpage/pintool/downloads/pin-3.28-98754-gc2c769c81-msvc-windows.zip" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Or use alternative download methods:" -ForegroundColor Yellow
    Write-Host "  1. Download manually from Intel website" -ForegroundColor Cyan
    Write-Host "  2. Use browser to download" -ForegroundColor Cyan
    Write-Host "  3. Save to: $pinZip" -ForegroundColor Cyan
    exit 1
}

Write-Host ""
Write-Host "Pin Path: $pinPath" -ForegroundColor Green
Write-Host ""

# Create tool directory
$toolDir = "$pinPath\source\tools\pin_extract_key"
if (-not (Test-Path $toolDir)) {
    New-Item -ItemType Directory -Path $toolDir -Force | Out-Null
    Write-Host "Created tool directory: $toolDir" -ForegroundColor Green
}

# Copy source files
$sourceFile = "Vystia\tools\pin_extract_key.cpp"
$makefile = "Vystia\tools\pin_makefile"

if (Test-Path $sourceFile) {
    Copy-Item $sourceFile "$toolDir\pin_extract_key.cpp" -Force
    Write-Host "Copied source file" -ForegroundColor Green
} else {
    Write-Host "Error: Source file not found: $sourceFile" -ForegroundColor Red
    exit 1
}

if (Test-Path $makefile) {
    Copy-Item $makefile "$toolDir\Makefile" -Force
    Write-Host "Copied Makefile" -ForegroundColor Green
} else {
    Write-Host "Warning: Makefile not found, will need manual compilation" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Setup complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Open Visual Studio Developer Command Prompt" -ForegroundColor Cyan
Write-Host "  2. cd $toolDir" -ForegroundColor Cyan
Write-Host "  3. nmake (or use the Makefile)" -ForegroundColor Cyan
Write-Host "  4. Run: $pinPath\pin.exe -t $toolDir\pin_extract_key.dll -- `"D:\Ultima Online - Sagas\ClassicUO\ClassicUO.exe`"" -ForegroundColor Cyan
