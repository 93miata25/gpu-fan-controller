@echo off
echo ===============================================
echo GPU Fan Controller Console - Build Script
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

echo [1/3] Restoring NuGet packages...
dotnet restore GPUFanControllerConsole.csproj
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)

echo.
echo [2/3] Building Release version...
dotnet build GPUFanControllerConsole.csproj -c Release
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo [3/3] Build completed successfully!
echo.
echo Executable location:
echo bin\Release\net6.0\GPUFanControllerConsole.exe
echo.
echo IMPORTANT: Run the application as Administrator!
echo.
pause
