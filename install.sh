#!/bin/bash
# GPU Fan Controller - Linux Installation Script
# This script installs the GPU Fan Controller on Linux systems

set -e  # Exit on error

APP_NAME="gpufancontroller"
INSTALL_DIR="/opt/gpufancontroller"
BIN_LINK="/usr/local/bin/gpufancontroller"
SERVICE_FILE="/etc/systemd/system/gpufancontroller.service"
CONFIG_DIR="$HOME/.config/gpufancontroller"

echo "=========================================="
echo "GPU Fan Controller - Linux Installer"
echo "=========================================="
echo ""

# Check if running as root
if [ "$EUID" -ne 0 ]; then 
    echo "❌ This script must be run as root (use sudo)"
    echo "Example: sudo ./install.sh"
    exit 1
fi

# Get the actual user (in case of sudo)
ACTUAL_USER=${SUDO_USER:-$USER}
ACTUAL_HOME=$(eval echo ~$ACTUAL_USER)

echo "Installing for user: $ACTUAL_USER"
echo "Installation directory: $INSTALL_DIR"
echo ""

# Check if LibreHardwareMonitor dependencies are available
echo "Checking system dependencies..."
if ! command -v lsmod &> /dev/null; then
    echo "⚠️  Warning: lsmod not found. Hardware monitoring may not work properly."
fi

# Create installation directory
echo "Creating installation directory..."
mkdir -p "$INSTALL_DIR"

# Copy application files
echo "Copying application files..."
if [ -d "bin/Release/net6.0/linux-x64/publish" ]; then
    cp -r bin/Release/net6.0/linux-x64/publish/* "$INSTALL_DIR/"
    chmod +x "$INSTALL_DIR/GPUFanControllerConsole"
elif [ -f "GPUFanControllerConsole" ]; then
    # If running from extracted package
    cp GPUFanControllerConsole "$INSTALL_DIR/"
    cp *.dll "$INSTALL_DIR/" 2>/dev/null || true
    cp *.so "$INSTALL_DIR/" 2>/dev/null || true
    chmod +x "$INSTALL_DIR/GPUFanControllerConsole"
else
    echo "❌ Error: Application files not found!"
    echo "Please run './build-linux.sh' first or extract the installation package."
    exit 1
fi

# Create symbolic link
echo "Creating symbolic link..."
ln -sf "$INSTALL_DIR/GPUFanControllerConsole" "$BIN_LINK"

# Create config directory for user
echo "Creating configuration directory..."
sudo -u "$ACTUAL_USER" mkdir -p "$CONFIG_DIR"

# Copy documentation if available
if [ -f "README.md" ]; then
    cp README.md "$INSTALL_DIR/" 2>/dev/null || true
fi
if [ -f "LICENSE.txt" ]; then
    cp LICENSE.txt "$INSTALL_DIR/" 2>/dev/null || true
fi

# Create systemd service file (optional)
echo ""
echo "Would you like to install as a system service? (y/n)"
read -r response
if [[ "$response" =~ ^[Yy]$ ]]; then
    echo "Creating systemd service..."
    cat > "$SERVICE_FILE" << EOF
[Unit]
Description=GPU Fan Controller Service
After=network.target

[Service]
Type=simple
ExecStart=$INSTALL_DIR/GPUFanControllerConsole
Restart=on-failure
RestartSec=10
User=root
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=multi-user.target
EOF

    # Reload systemd
    systemctl daemon-reload
    
    echo ""
    echo "Service installed. To enable and start:"
    echo "  sudo systemctl enable gpufancontroller"
    echo "  sudo systemctl start gpufancontroller"
    echo ""
    echo "To view logs:"
    echo "  sudo journalctl -u gpufancontroller -f"
fi

echo ""
echo "✅ Installation complete!"
echo ""
echo "Usage:"
echo "  Run directly: sudo gpufancontroller"
echo "  Or: sudo $INSTALL_DIR/GPUFanControllerConsole"
echo ""
echo "⚠️  Important Notes:"
echo "  • This application requires root/sudo privileges to access GPU hardware"
echo "  • For NVIDIA GPUs: Ensure nvidia-smi and nvidia drivers are installed"
echo "  • For AMD GPUs: Ensure amdgpu drivers and rocm tools are installed"
echo "  • Always monitor GPU temperatures when using custom fan curves"
echo ""
echo "Configuration will be saved to: $CONFIG_DIR"
echo ""
echo "To uninstall, run: sudo $INSTALL_DIR/uninstall.sh"
echo ""

# Create uninstall script
cat > "$INSTALL_DIR/uninstall.sh" << 'EOF'
#!/bin/bash
# GPU Fan Controller - Uninstall Script

set -e

APP_NAME="gpufancontroller"
INSTALL_DIR="/opt/gpufancontroller"
BIN_LINK="/usr/local/bin/gpufancontroller"
SERVICE_FILE="/etc/systemd/system/gpufancontroller.service"

echo "=========================================="
echo "GPU Fan Controller - Uninstaller"
echo "=========================================="
echo ""

if [ "$EUID" -ne 0 ]; then 
    echo "❌ This script must be run as root (use sudo)"
    exit 1
fi

echo "This will remove GPU Fan Controller from your system."
echo "Configuration files in ~/.config/gpufancontroller will be preserved."
echo ""
echo "Continue? (y/n)"
read -r response

if [[ ! "$response" =~ ^[Yy]$ ]]; then
    echo "Uninstallation cancelled."
    exit 0
fi

# Stop and disable service if it exists
if [ -f "$SERVICE_FILE" ]; then
    echo "Stopping and disabling service..."
    systemctl stop gpufancontroller 2>/dev/null || true
    systemctl disable gpufancontroller 2>/dev/null || true
    rm -f "$SERVICE_FILE"
    systemctl daemon-reload
fi

# Remove symbolic link
echo "Removing symbolic link..."
rm -f "$BIN_LINK"

# Remove installation directory
echo "Removing application files..."
rm -rf "$INSTALL_DIR"

echo ""
echo "✅ Uninstallation complete!"
echo ""
echo "Configuration files preserved in: ~/.config/gpufancontroller"
echo "To remove config: rm -rf ~/.config/gpufancontroller"
echo ""
EOF

chmod +x "$INSTALL_DIR/uninstall.sh"

echo "Uninstall script created at: $INSTALL_DIR/uninstall.sh"
