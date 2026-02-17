# ServUO Build and Copy Script
# Automates: Build Scripts.dll + Copy to root directory

param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  ServUO Build and Copy" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Paths
$ScriptsProject = "Scripts\Scripts.csproj"
$BuildOutput = "Scripts\bin\$Configuration\Scripts.dll"
$RootDll = "Scripts.dll"

# Step 1: Check if server is running and kill it if needed
Write-Host "[1/3] Checking if ServUO is running..." -ForegroundColor Yellow
$servuoProcess = Get-Process -Name "ServUO" -ErrorAction SilentlyContinue
if ($servuoProcess) {
    Write-Host "  ⚠️  ServUO is running. Stopping it..." -ForegroundColor Yellow
    try {
        Stop-Process -Name "ServUO" -Force -ErrorAction Stop
        Start-Sleep -Milliseconds 500  # Give it a moment to fully stop
        Write-Host "  ✅ ServUO process stopped`n" -ForegroundColor Green
    }
    catch {
        Write-Host "  ❌ Failed to stop ServUO: $_" -ForegroundColor Red
        Write-Host "  Please stop it manually and try again.`n" -ForegroundColor Yellow
        exit 1
    }
}
else {
    Write-Host "  ✅ ServUO is not running`n" -ForegroundColor Green
}

# Step 2: Build
Write-Host "[2/3] Building Scripts project..." -ForegroundColor Yellow
Write-Host "  Configuration: $Configuration" -ForegroundColor Gray
Write-Host "  Project: $ScriptsProject`n" -ForegroundColor Gray

try {
    dotnet build $ScriptsProject -c $Configuration --no-incremental
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`n  ❌ Build failed!`n" -ForegroundColor Red
        exit 1
    }
    Write-Host "`n  ✅ Build succeeded`n" -ForegroundColor Green
}
catch {
    Write-Host "`n  ❌ Build error: $_`n" -ForegroundColor Red
    exit 1
}

# Step 3: Copy DLLs to root
Write-Host "[3/3] Copying DLLs to root..." -ForegroundColor Yellow

if (-not (Test-Path $BuildOutput)) {
    Write-Host "  ❌ Build output not found: $BuildOutput" -ForegroundColor Red
    Write-Host "  Build may have failed.`n" -ForegroundColor Yellow
    exit 1
}

try {
    # Copy Scripts.dll
    Copy-Item -Path $BuildOutput -Destination $RootDll -Force
    Write-Host "  ✅ Copied: $BuildOutput -> $RootDll" -ForegroundColor Green
    
    # Copy SQLite DLLs (required for SQLite to work)
    $nugetPath = "$env:USERPROFILE\.nuget\packages"
    $sqliteDll = "$nugetPath\stub.system.data.sqlite.core.netframework\1.0.118\lib\net46\System.Data.SQLite.dll"
    $sqliteInteropX64 = "$nugetPath\stub.system.data.sqlite.core.netframework\1.0.118\build\net46\x64\SQLite.Interop.dll"
    
    if (Test-Path $sqliteDll) {
        Copy-Item -Path $sqliteDll -Destination "System.Data.SQLite.dll" -Force -ErrorAction SilentlyContinue
        Write-Host "  ✅ Copied: System.Data.SQLite.dll" -ForegroundColor Green
    }
    
    if (Test-Path $sqliteInteropX64) {
        Copy-Item -Path $sqliteInteropX64 -Destination "SQLite.Interop.dll" -Force -ErrorAction SilentlyContinue
        Write-Host "  ✅ Copied: SQLite.Interop.dll (x64)" -ForegroundColor Green
    }
    
    # Verify
    $fileInfo = Get-Item $RootDll
    Write-Host "`n  File Info:" -ForegroundColor Gray
    Write-Host "    Size: $([math]::Round($fileInfo.Length / 1KB, 2)) KB" -ForegroundColor Gray
    Write-Host "    Modified: $($fileInfo.LastWriteTime)`n" -ForegroundColor Gray
}
catch {
    Write-Host "  ❌ Copy failed: $_`n" -ForegroundColor Red
    exit 1
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ✅ Build and Copy Complete!" -ForegroundColor Green
Write-Host "========================================`n" -ForegroundColor Cyan
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Start ServUO server" -ForegroundColor White
Write-Host "  2. Check console for initialization messages" -ForegroundColor White
Write-Host "  3. Test your changes in-game`n" -ForegroundColor White

