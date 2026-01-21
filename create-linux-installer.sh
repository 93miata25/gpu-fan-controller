#!/bin/bash
# GPU Fan Controller - Create Linux Installation Package
# This script creates a distributable tarball for Linux

set -e

VERSION="2.3.1"
PACKAGE_NAME="GPUFanController-${VERSION}-linux-x64"
BUILD_DIR="bin/Release/net6.0/linux-x64/publish"
PACKAGE_DIR="installer_output/linux"
TEMP_DIR="$PACKAGE_DIR/tmp/$PACKAGE_NAME"

echo "=========================================="
echo "Creating Linux Installation Package"
echo "=========================================="
echo ""

# Check if build exists
if [ ! -d "$BUILD_DIR" ]; then
    echo "❌ Build not found. Running build script first..."
    ./build-linux.sh
fi

# Create package directory
echo "Creating package directory..."
mkdir -p "$TEMP_DIR"
mkdir -p "$PACKAGE_DIR"

# Copy application files
echo "Copying application files..."
cp -r "$BUILD_DIR"/* "$TEMP_DIR/"

# Copy installation scripts
echo "Copying installation scripts..."
cp install.sh "$TEMP_DIR/"
chmod +x "$TEMP_DIR/install.sh"

# Copy documentation
echo "Copying documentation..."
cp README.md "$TEMP_DIR/" 2>/dev/null || true
cp LICENSE.txt "$TEMP_DIR/" 2>/dev/null || true
cp QUICKSTART.md "$TEMP_DIR/" 2>/dev/null || true

# Create Linux-specific README
cat > "$TEMP_DIR/README-LINUX.txt" << 'EOF'
GPU Fan Controller - Linux Installation Guide
==============================================

SYSTEM REQUIREMENTS
-------------------
- Linux kernel 4.0 or later
- x86_64 architecture
- Root/sudo access
- GPU drivers installed:
  * NVIDIA: nvidia-driver and nvidia-utils
  * AMD: amdgpu driver and ROCm tools (optional)
  * Intel: i915 driver

QUICK INSTALLATION
------------------
1. Extract this package
2. Open terminal in extracted directory
3. Run: sudo ./install.sh
4. Follow the prompts

USAGE
-----
Run with sudo privileges:
  sudo gpufancontroller

The application will detect your GPU(s) and provide an interactive menu.

FEATURES
--------
✓ Multi-GPU support (NVIDIA, AMD, Intel)
✓ Automatic fan curves (4 profiles: Silent, Balanced, Performance, Aggressive)
✓ Manual fan control
✓ Real-time temperature monitoring
✓ System service support (optional)

RUNNING AS SERVICE
------------------
During installation, you can choose to install as a system service.
If installed as service:
  
  Start:   sudo systemctl start gpufancontroller
  Stop:    sudo systemctl stop gpufancontroller
  Enable:  sudo systemctl enable gpufancontroller
  Status:  sudo systemctl status gpufancontroller
  Logs:    sudo journalctl -u gpufancontroller -f

UNINSTALLATION
--------------
  sudo /opt/gpufancontroller/uninstall.sh

Or manually:
  sudo rm -rf /opt/gpufancontroller
  sudo rm /usr/local/bin/gpufancontroller
  sudo rm /etc/systemd/system/gpufancontroller.service
  sudo systemctl daemon-reload

TROUBLESHOOTING
---------------
1. "No GPU detected"
   - Ensure GPU drivers are installed
   - Run: lspci | grep VGA
   - Check: nvidia-smi (for NVIDIA) or rocm-smi (for AMD)

2. "Permission denied"
   - Must run with sudo: sudo gpufancontroller

3. Hardware monitoring not working
   - Load kernel modules: sudo modprobe i2c-dev
   - For NVIDIA: Ensure nvidia-smi works
   - For AMD: Check amdgpu driver is loaded

4. Fan control not working
   - Some GPUs may not support software fan control
   - Check GPU specifications
   - Verify driver version supports fan control

IMPORTANT WARNINGS
------------------
⚠️  Always monitor GPU temperatures when using custom fan curves
⚠️  Setting fan speed too low may cause overheating
⚠️  The application requires root access to control hardware
⚠️  Test custom curves under load before leaving unattended

CONFIGURATION
-------------
Configuration files are stored in:
  ~/.config/gpufancontroller/

SUPPORT
-------
For issues, please visit:
  https://github.com/yourusername/GPUFanController

VERSION: 2.3.1
EOF

# Create installation instructions
cat > "$TEMP_DIR/INSTALL.txt" << 'EOF'
INSTALLATION INSTRUCTIONS
=========================

1. Open a terminal in this directory

2. Run the installation script:
   sudo ./install.sh

3. Follow the on-screen prompts

4. After installation, run:
   sudo gpufancontroller

For detailed information, see README-LINUX.txt
EOF

# Create tarball
echo "Creating tarball..."
cd "$PACKAGE_DIR/tmp"
tar -czf "../${PACKAGE_NAME}.tar.gz" "$PACKAGE_NAME"
cd - > /dev/null

# Cleanup
echo "Cleaning up..."
rm -rf "$PACKAGE_DIR/tmp"

# Create checksums
echo "Generating checksums..."
cd "$PACKAGE_DIR"
sha256sum "${PACKAGE_NAME}.tar.gz" > "${PACKAGE_NAME}.tar.gz.sha256"
cd - > /dev/null

echo ""
echo "✅ Package created successfully!"
echo ""
echo "Package location:"
echo "  $PACKAGE_DIR/${PACKAGE_NAME}.tar.gz"
echo ""
echo "Checksum:"
cat "$PACKAGE_DIR/${PACKAGE_NAME}.tar.gz.sha256"
echo ""
echo "Distribution instructions:"
echo "  1. Share the .tar.gz file with users"
echo "  2. Users should extract: tar -xzf ${PACKAGE_NAME}.tar.gz"
echo "  3. Users should run: cd $PACKAGE_NAME && sudo ./install.sh"
echo ""
