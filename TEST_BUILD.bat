@echo off
echo ========================================
echo GPU Fan Controller - Setup Test
echo ========================================
echo.

echo Checking .NET SDK...
dotnet --version
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo WARNING: .NET SDK not found in PATH
    echo.
    echo If you just installed .NET SDK:
    echo   1. Close this window
    echo   2. Restart your computer (or log out and back in)
    echo   3. Run this script again
    echo.
    echo If .NET SDK is not installed:
    echo   Download from: https://dotnet.microsoft.com/download/dotnet/6.0
    echo.
    pause
    exit /b 1
)

echo SUCCESS: .NET SDK is installed!
echo.

echo Checking Inno Setup...
set INNO_FOUND=0
if exist "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" (
    echo SUCCESS: Inno Setup found at C:\Program Files (x86)\Inno Setup 6\
    set INNO_FOUND=1
)
if exist "C:\Program Files\Inno Setup 6\ISCC.exe" (
    echo SUCCESS: Inno Setup found at C:\Program Files\Inno Setup 6\
    set INNO_FOUND=1
)
if exist "C:\Program Files (x86)\Inno Setup 5\ISCC.exe" (
    echo SUCCESS: Inno Setup found at C:\Program Files (x86)\Inno Setup 5\
    set INNO_FOUND=1
)

if %INNO_FOUND%==0 (
    echo WARNING: Inno Setup not found in default locations
    echo.
    echo If you just installed Inno Setup:
    echo   1. Verify it installed correctly
    echo   2. Check installation path
    echo.
    echo If Inno Setup is not installed:
    echo   Download from: https://jrsoftware.org/isdl.php
    echo.
    echo Note: You can still build the applications without the installer
    pause
    exit /b 0
)

echo.
echo ========================================
echo ✓ All prerequisites are ready!
echo ========================================
echo.
echo You can now run:
echo   build-everything.bat  - Build everything including installer
echo   build.bat             - Build GUI only
echo   build-console.bat     - Build Console only
echo.
pause
