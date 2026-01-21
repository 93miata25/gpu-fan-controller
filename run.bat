@echo off
echo ===============================================
echo GPU Fan Controller - Run Script
echo ===============================================
echo.

REM Check if dotnet is installed
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: .NET SDK/Runtime is not installed!
    echo.
    echo Please download and install .NET 6.0 Runtime or later from:
    echo https://dotnet.microsoft.com/download/dotnet/6.0
    echo.
    pause
    exit /b 1
)

REM Check for admin privileges
net session >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo WARNING: Not running as Administrator!
    echo.
    echo The application requires Administrator privileges to access GPU hardware.
    echo Please right-click this script and select "Run as Administrator"
    echo.
    pause
    exit /b 1
)

echo Running application...
echo.
dotnet run

pause
