#!/bin/bash
# GPU Fan Controller - Build Everything for Linux
# One-click script to build and package everything for Linux distribution

set -e

echo "╔═══════════════════════════════════════════════════════════╗"
echo "║    GPU Fan Controller - Complete Linux Build Script      ║"
echo "╚═══════════════════════════════════════════════════════════╝"
echo ""

# Check prerequisites
echo "Checking prerequisites..."
if ! command -v dotnet &> /dev/null; then
    echo "❌ Error: .NET SDK not found!"
    echo "Install from: https://dotnet.microsoft.com/download"
    exit 1
fi
echo "✓ .NET SDK: $(dotnet --version)"

if ! command -v dpkg-deb &> /dev/null; then
    echo "⚠️  Warning: dpkg-deb not found. .deb package will not be built."
    echo "  Install with: sudo apt install dpkg (on Debian/Ubuntu)"
    BUILD_DEB=false
else
    echo "✓ dpkg-deb found"
    BUILD_DEB=true
fi

echo ""
echo "═══════════════════════════════════════════════════════════"
echo "Step 1/3: Building Console Application for Linux"
echo "═══════════════════════════════════════════════════════════"
echo ""

chmod +x build-linux.sh
./build-linux.sh

if [ $? -ne 0 ]; then
    echo ""
    echo "❌ Build failed!"
    exit 1
fi

echo ""
echo "═══════════════════════════════════════════════════════════"
echo "Step 2/3: Creating Universal Tarball Package"
echo "═══════════════════════════════════════════════════════════"
echo ""

chmod +x create-linux-installer.sh
./create-linux-installer.sh

if [ $? -ne 0 ]; then
    echo ""
    echo "❌ Tarball creation failed!"
    exit 1
fi

echo ""
echo "═══════════════════════════════════════════════════════════"
echo "Step 3/3: Creating Debian Package"
echo "═══════════════════════════════════════════════════════════"
echo ""

if [ "$BUILD_DEB" = true ]; then
    chmod +x create-deb-package.sh
    ./create-deb-package.sh
    
    if [ $? -ne 0 ]; then
        echo ""
        echo "⚠️  Warning: Debian package creation failed (non-critical)"
    fi
else
    echo "⚠️  Skipping Debian package (dpkg-deb not available)"
fi

echo ""
echo "╔═══════════════════════════════════════════════════════════╗"
echo "║                 ✅ BUILD COMPLETE!                        ║"
echo "╚═══════════════════════════════════════════════════════════╝"
echo ""
echo "📦 Packages created:"
echo ""

if [ -f "installer_output/linux/GPUFanController-2.3.1-linux-x64.tar.gz" ]; then
    SIZE=$(du -h "installer_output/linux/GPUFanController-2.3.1-linux-x64.tar.gz" | cut -f1)
    echo "  ✓ Universal Package (all Linux distributions)"
    echo "    📁 installer_output/linux/GPUFanController-2.3.1-linux-x64.tar.gz"
    echo "    📊 Size: $SIZE"
    echo ""
fi

if [ -f "installer_output/linux/deb/gpufancontroller_2.3.1_amd64.deb" ]; then
    SIZE=$(du -h "installer_output/linux/deb/gpufancontroller_2.3.1_amd64.deb" | cut -f1)
    echo "  ✓ Debian Package (Ubuntu/Debian/Mint)"
    echo "    📁 installer_output/linux/deb/gpufancontroller_2.3.1_amd64.deb"
    echo "    📊 Size: $SIZE"
    echo ""
fi

echo "📋 Checksums:"
echo ""
cat installer_output/linux/*.sha256 2>/dev/null || true
cat installer_output/linux/deb/*.sha256 2>/dev/null || true

echo ""
echo "📖 Documentation:"
echo "  • LINUX_INSTALLATION_GUIDE.md - Complete user guide"
echo "  • BUILD_LINUX_QUICK_START.md  - Developer guide"
echo ""
echo "🧪 Testing:"
echo "  Tarball:  cd installer_output/linux && tar -xzf *.tar.gz && cd GPUFanController-* && sudo ./install.sh"
echo "  Debian:   sudo apt install ./installer_output/linux/deb/*.deb"
echo ""
echo "🚀 Distribution:"
echo "  1. Test installation on a clean Linux system"
echo "  2. Upload packages to GitHub Releases"
echo "  3. Update download links in documentation"
echo ""
echo "⚠️  Remember: Application requires sudo/root to access GPU hardware"
echo ""
