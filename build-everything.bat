@echo off
echo ===============================================
echo GPU Fan Controller - Complete Build Script
echo ===============================================
echo.
echo This will build EVERYTHING:
echo   1. GUI Application
echo   2. Console Application
echo   3. Standalone Executables
echo   4. Windows Installer (if Inno Setup installed)
echo.
pause
echo.

REM Check if dotnet is installed
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: .NET SDK is not installed!
    echo.
    echo Please download and install .NET 6.0 SDK from:
    echo https://dotnet.microsoft.com/download/dotnet/6.0
    echo.
    pause
    exit /b 1
)

echo ===============================================
echo STEP 1: Restoring NuGet Packages
echo ===============================================
echo.
dotnet restore GPUFanController.csproj
dotnet restore GPUFanControllerConsole.csproj
echo.

echo ===============================================
echo STEP 2: Building GUI Application (Debug)
echo ===============================================
echo.
dotnet build GPUFanController.csproj -c Debug
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: GUI Debug build failed
    pause
    exit /b 1
)
echo GUI Debug build successful!
echo.

echo ===============================================
echo STEP 3: Building Console Application (Debug)
echo ===============================================
echo.
dotnet build GPUFanControllerConsole.csproj -c Debug
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Console Debug build failed
    pause
    exit /b 1
)
echo Console Debug build successful!
echo.

echo ===============================================
echo STEP 4: Building GUI Application (Release)
echo ===============================================
echo.
dotnet build GPUFanController.csproj -c Release
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: GUI Release build failed
    pause
    exit /b 1
)
echo GUI Release build successful!
echo.

echo ===============================================
echo STEP 5: Building Console Application (Release)
echo ===============================================
echo.
dotnet build GPUFanControllerConsole.csproj -c Release
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Console Release build failed
    pause
    exit /b 1
)
echo Console Release build successful!
echo.

echo ===============================================
echo STEP 6: Publishing Standalone GUI Executable
echo ===============================================
echo.
dotnet publish GPUFanController.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishReadyToRun=true
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: GUI publish failed
    pause
    exit /b 1
)
echo GUI standalone executable created!
echo Location: bin\Release\net6.0-windows\win-x64\publish\GPUFanController.exe
echo.

echo ===============================================
echo STEP 7: Publishing Standalone Console Executable
echo ===============================================
echo.
dotnet publish GPUFanControllerConsole.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishReadyToRun=true
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Console publish failed
    pause
    exit /b 1
)
echo Console standalone executable created!
echo Location: bin\Release\net6.0\win-x64\publish\GPUFanControllerConsole.exe
echo.

echo ===============================================
echo STEP 8: Creating Windows Installer
echo ===============================================
echo.

REM Check for Inno Setup
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
    echo WARNING: Inno Setup not found - Skipping installer creation
    echo.
    echo To create installer, install Inno Setup from:
    echo https://jrsoftware.org/isdl.php
    echo Then run: build-installer.bat
    echo.
    goto :skip_installer
)

echo Inno Setup found! Creating installer...
%INNO_PATH% installer.iss
if %ERRORLEVEL% NEQ 0 (
    echo WARNING: Installer creation failed
    goto :skip_installer
)

echo.
echo ✅ INSTALLER CREATED SUCCESSFULLY!
echo Location: installer_output\GPUFanController_Setup_v2.0.exe
echo.

:skip_installer

echo ===============================================
echo ✅ BUILD COMPLETE! 
echo ===============================================
echo.
echo What was built:
echo ---------------
echo.
echo 1. GUI Application (Debug):
echo    bin\Debug\net6.0-windows\GPUFanController.exe
echo.
echo 2. Console Application (Debug):
echo    bin\Debug\net6.0\GPUFanControllerConsole.exe
echo.
echo 3. GUI Application (Release):
echo    bin\Release\net6.0-windows\GPUFanController.exe
echo.
echo 4. Console Application (Release):
echo    bin\Release\net6.0\GPUFanControllerConsole.exe
echo.
echo 5. Standalone GUI Executable:
echo    bin\Release\net6.0-windows\win-x64\publish\GPUFanController.exe
echo.
echo 6. Standalone Console Executable:
echo    bin\Release\net6.0\win-x64\publish\GPUFanControllerConsole.exe
echo.

if not %INNO_PATH%=="" (
    echo 7. Windows Installer:
    echo    installer_output\GPUFanController_Setup_v2.0.exe
    echo.
)

echo ===============================================
echo Next Steps:
echo ===============================================
echo.
echo TO RUN (GUI):
echo   Right-click run.bat and select "Run as Administrator"
echo.
echo TO RUN (Console):
echo   Right-click run-console.bat and select "Run as Administrator"
echo.
echo TO INSTALL:
if not %INNO_PATH%=="" (
    echo   Double-click: installer_output\GPUFanController_Setup_v2.0.exe
) else (
    echo   First install Inno Setup, then run build-installer.bat
)
echo.
echo ===============================================
pause
