#!/bin/bash
# GPU Fan Controller - Debian/Ubuntu Package Creator
# Creates a .deb package for easy installation on Debian-based systems

set -e

VERSION="2.3.1"
PACKAGE_NAME="gpufancontroller"
ARCH="amd64"
BUILD_DIR="bin/Release/net6.0/linux-x64/publish"
DEB_DIR="installer_output/linux/deb"
PKG_DIR="$DEB_DIR/${PACKAGE_NAME}_${VERSION}_${ARCH}"

echo "=========================================="
echo "Creating Debian Package (.deb)"
echo "=========================================="
echo ""

# Check if build exists
if [ ! -d "$BUILD_DIR" ]; then
    echo "❌ Build not found. Running build script first..."
    ./build-linux.sh
fi

# Clean previous package builds
rm -rf "$PKG_DIR"

# Create package structure
echo "Creating package structure..."
mkdir -p "$PKG_DIR/DEBIAN"
mkdir -p "$PKG_DIR/opt/gpufancontroller"
mkdir -p "$PKG_DIR/usr/local/bin"
mkdir -p "$PKG_DIR/etc/systemd/system"
mkdir -p "$PKG_DIR/usr/share/doc/gpufancontroller"
mkdir -p "$PKG_DIR/usr/share/applications"

# Copy application files
echo "Copying application files..."
cp -r "$BUILD_DIR"/* "$PKG_DIR/opt/gpufancontroller/"
chmod +x "$PKG_DIR/opt/gpufancontroller/GPUFanControllerConsole"

# Copy documentation
echo "Copying documentation..."
cp README.md "$PKG_DIR/usr/share/doc/gpufancontroller/" 2>/dev/null || true
cp LICENSE.txt "$PKG_DIR/usr/share/doc/gpufancontroller/copyright" 2>/dev/null || true
cp QUICKSTART.md "$PKG_DIR/usr/share/doc/gpufancontroller/" 2>/dev/null || true

# Create systemd service file
echo "Creating systemd service..."
cat > "$PKG_DIR/etc/systemd/system/gpufancontroller.service" << 'EOF'
[Unit]
Description=GPU Fan Controller Service
After=network.target

[Service]
Type=simple
ExecStart=/opt/gpufancontroller/GPUFanControllerConsole
Restart=on-failure
RestartSec=10
User=root
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=multi-user.target
EOF

# Create control file
echo "Creating control file..."
cat > "$PKG_DIR/DEBIAN/control" << EOF
Package: gpufancontroller
Version: ${VERSION}
Section: utils
Priority: optional
Architecture: ${ARCH}
Maintainer: GPU Fan Controller <contact@example.com>
Description: Advanced GPU fan control utility
 GPU Fan Controller is a powerful utility for controlling GPU fan speeds
 on Linux systems. It supports NVIDIA, AMD, and Intel GPUs.
 .
 Features:
  * Multi-GPU support
  * Automatic fan curves (4 profiles)
  * Manual fan control
  * Real-time temperature monitoring
  * Systemd service support
Depends: libc6 (>= 2.31)
Recommends: nvidia-driver-525 | amdgpu-dkms
Homepage: https://github.com/yourusername/GPUFanController
EOF

# Create postinst script (runs after installation)
echo "Creating post-installation script..."
cat > "$PKG_DIR/DEBIAN/postinst" << 'EOF'
#!/bin/bash
set -e

# Create symbolic link
ln -sf /opt/gpufancontroller/GPUFanControllerConsole /usr/local/bin/gpufancontroller

# Reload systemd
if command -v systemctl &> /dev/null; then
    systemctl daemon-reload
fi

echo ""
echo "=========================================="
echo "GPU Fan Controller installed successfully!"
echo "=========================================="
echo ""
echo "Usage:"
echo "  sudo gpufancontroller"
echo ""
echo "To run as a service:"
echo "  sudo systemctl enable gpufancontroller"
echo "  sudo systemctl start gpufancontroller"
echo ""
echo "⚠️  Important: This application requires root privileges"
echo "⚠️  Always monitor GPU temperatures when using custom curves"
echo ""

exit 0
EOF
chmod +x "$PKG_DIR/DEBIAN/postinst"

# Create prerm script (runs before uninstallation)
echo "Creating pre-removal script..."
cat > "$PKG_DIR/DEBIAN/prerm" << 'EOF'
#!/bin/bash
set -e

# Stop service if running
if systemctl is-active --quiet gpufancontroller; then
    systemctl stop gpufancontroller
fi

# Disable service if enabled
if systemctl is-enabled --quiet gpufancontroller 2>/dev/null; then
    systemctl disable gpufancontroller
fi

exit 0
EOF
chmod +x "$PKG_DIR/DEBIAN/prerm"

# Create postrm script (runs after uninstallation)
echo "Creating post-removal script..."
cat > "$PKG_DIR/DEBIAN/postrm" << 'EOF'
#!/bin/bash
set -e

# Remove symbolic link
rm -f /usr/local/bin/gpufancontroller

# Reload systemd
if command -v systemctl &> /dev/null; then
    systemctl daemon-reload
fi

echo "GPU Fan Controller has been removed."
echo "Configuration files in ~/.config/gpufancontroller have been preserved."

exit 0
EOF
chmod +x "$PKG_DIR/DEBIAN/postrm"

# Create desktop entry (optional, for GUI launchers)
cat > "$PKG_DIR/usr/share/applications/gpufancontroller.desktop" << 'EOF'
[Desktop Entry]
Type=Application
Name=GPU Fan Controller
Comment=Control GPU fan speeds
Exec=sudo gpufancontroller
Icon=utilities-system-monitor
Terminal=true
Categories=System;Monitor;
Keywords=gpu;fan;temperature;nvidia;amd;intel;
EOF

# Build the package
echo "Building .deb package..."
dpkg-deb --build "$PKG_DIR"

# Move to final location
mv "$PKG_DIR.deb" "$DEB_DIR/"

# Generate checksums
echo "Generating checksums..."
cd "$DEB_DIR"
sha256sum "${PACKAGE_NAME}_${VERSION}_${ARCH}.deb" > "${PACKAGE_NAME}_${VERSION}_${ARCH}.deb.sha256"
cd - > /dev/null

# Cleanup
rm -rf "$PKG_DIR"

echo ""
echo "✅ Debian package created successfully!"
echo ""
echo "Package location:"
echo "  $DEB_DIR/${PACKAGE_NAME}_${VERSION}_${ARCH}.deb"
echo ""
echo "Checksum:"
cat "$DEB_DIR/${PACKAGE_NAME}_${VERSION}_${ARCH}.deb.sha256"
echo ""
echo "Installation instructions:"
echo "  sudo dpkg -i ${PACKAGE_NAME}_${VERSION}_${ARCH}.deb"
echo "  sudo apt-get install -f  # Fix any dependency issues"
echo ""
echo "Or for Ubuntu 22.04+:"
echo "  sudo apt install ./${PACKAGE_NAME}_${VERSION}_${ARCH}.deb"
echo ""
