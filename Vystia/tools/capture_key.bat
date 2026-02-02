@echo off
echo Starting ClassicUO and attaching frida-trace...
echo.

set FRIDA_PATH=%LOCALAPPDATA%\Packages\PythonSoftwareFoundation.Python.3.11_qbz5n2kfra8p0\LocalCache\local-packages\Python311\Scripts\frida-trace.exe

start "" "D:\Ultima Online - Sagas\ClassicUO\ClassicUO.exe"
timeout /t 3 /nobreak >nul
"%FRIDA_PATH%" -n ClassicUO.exe -i "BCrypt*"

pause
