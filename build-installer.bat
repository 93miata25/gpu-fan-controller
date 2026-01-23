@echo off
echo ===============================================
echo GPU Fan Controller - Installer Build Script
echo ===============================================
echo.

REM Check if dotnet is installed
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: .NET SDK is not installed!
    echo.
    echo Please download and install .NET 6.0 SDK or later from:
    echo https://dotnet.microsoft.com/download/dotnet/6.0
    echo.
    pause
    exit /b 1
)

echo Step 1: Building GUI application...
echo -----------------------------------
dotnet publish GPUFanController.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishReadyToRun=true
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: GUI build failed
    pause
    exit /b 1
)
echo GUI build successful!
echo.

echo Step 2: Building Console application...
echo ----------------------------------------
dotnet publish GPUFanControllerConsole.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishReadyToRun=true
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Console build failed
    pause
    exit /b 1
)
echo Console build successful!
echo.

echo Step 3: Checking for Inno Setup...
echo -----------------------------------
REM Check common Inno Setup installation paths
set INNO_PATH=""
if exist "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" (
    set INNO_PATH="C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
)
if exist "C:\Program Files\Inno Setup 6\ISCC.exe" (
    set INNO_PATH="C:\Program Files\Inno Setup 6\ISCC.exe"
)
if exist "C:\Program Files (x86)\Inno Setup 5\ISCC.exe" (
    set INNO_PATH="C:\Program Files (x86)\Inno Setup 5\ISCC.exe"
)

if %INNO_PATH%=="" (
    echo.
    echo WARNING: Inno Setup not found!
    echo.
    echo To create the installer, you need to install Inno Setup:
    echo.
    echo 1. Download from: https://jrsoftware.org/isdl.php
    echo 2. Install Inno Setup (free)
    echo 3. Run this script again
    echo.
    echo OR manually compile:
    echo    - Open installer.iss in Inno Setup
    echo    - Click Build ^> Compile
    echo.
    echo Applications are built and ready in:
    echo   - bin\Release\net6.0-windows\win-x64\publish\GPUFanController.exe
    echo   - bin\Release\net6.0\win-x64\publish\GPUFanControllerConsole.exe
    echo.
    pause
    exit /b 0
)

echo Inno Setup found at: %INNO_PATH%
echo.

echo Step 4: Creating installer package...
echo --------------------------------------
%INNO_PATH% installer.iss
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Installer creation failed
    pause
    exit /b 1
)

echo.
echo ===============================================
echo SUCCESS! Installer created successfully!
echo ===============================================
echo.
echo Installer location:
echo   installer_output\GPUFanController_Setup_v2.3.2.exe
echo.
echo You can now distribute this installer to users.
echo Double-click the installer to test installation.
echo.
pause
