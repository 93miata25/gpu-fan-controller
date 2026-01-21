#!/bin/bash
# GPU Fan Controller - Linux Build Script
# This script builds the console application for Linux (x64)

set -e  # Exit on error

echo "=========================================="
echo "GPU Fan Controller - Linux Build Script"
echo "=========================================="
echo ""

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ Error: .NET SDK not found!"
    echo "Please install .NET 6.0 SDK or later:"
    echo "  https://dotnet.microsoft.com/download"
    exit 1
fi

echo "✓ .NET SDK found: $(dotnet --version)"
echo ""

# Clean previous builds
echo "Cleaning previous builds..."
rm -rf bin/Release/net6.0/linux-x64/
echo "✓ Clean complete"
echo ""

# Build console application for Linux
echo "Building console application for Linux (x64)..."
dotnet publish GPUFanControllerConsole.csproj \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:PublishTrimmed=false

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Build successful!"
    echo ""
    echo "Output location:"
    echo "  bin/Release/net6.0/linux-x64/publish/"
    echo ""
    echo "Next steps:"
    echo "  1. Run './create-linux-installer.sh' to create installation package"
    echo "  2. Or manually copy files from publish directory"
else
    echo ""
    echo "❌ Build failed!"
    exit 1
fi
