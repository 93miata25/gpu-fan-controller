@echo off
echo ===============================================
echo GPU Fan Controller Console - Publish Script
echo ===============================================
echo.
echo Creating standalone executable...
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

echo [1/2] Publishing single-file executable...
dotnet publish GPUFanControllerConsole.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Publish failed
    pause
    exit /b 1
)

echo.
echo [2/2] Publish completed successfully!
echo.
echo Standalone executable location:
echo bin\Release\net6.0\win-x64\publish\GPUFanControllerConsole.exe
echo.
echo This executable can be copied to any Windows PC without requiring .NET installation.
echo IMPORTANT: Always run as Administrator!
echo.
pause
