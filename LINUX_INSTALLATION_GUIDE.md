# GPU Fan Controller - Linux Installation Guide

## Overview

GPU Fan Controller is now available for Linux! This guide covers installation, usage, and troubleshooting for Linux systems.

## System Requirements

- **Operating System**: Linux kernel 4.0 or later
- **Architecture**: x86_64 (64-bit)
- **Privileges**: Root/sudo access required
- **GPU Support**: NVIDIA, AMD, or Intel GPUs
- **Dependencies**: GPU drivers must be installed

### GPU-Specific Requirements

#### NVIDIA GPUs
- NVIDIA proprietary driver (nvidia-driver)
- nvidia-utils package
- Verify with: `nvidia-smi`

#### AMD GPUs
- amdgpu driver (usually built-in to kernel)
- ROCm tools (optional, for advanced features)
- Verify with: `lspci | grep VGA`

#### Intel GPUs
- i915 driver (usually built-in to kernel)
- intel-gpu-tools (optional)
- Verify with: `lspci | grep VGA`

---

## Installation Methods

### Method 1: Using Pre-built Package (Recommended)

#### For All Linux Distributions (Tarball)

1. **Download the package**:
   ```bash
   wget https://github.com/93miata25/GPUFanController/releases/download/v2.3.1/GPUFanController-2.3.1-linux-x64.tar.gz
   ```

2. **Extract**:
   ```bash
   tar -xzf GPUFanController-2.3.1-linux-x64.tar.gz
   cd GPUFanController-2.3.1-linux-x64
   ```

3. **Install**:
   ```bash
   sudo ./install.sh
   ```

4. **Follow the prompts** to complete installation.

#### For Debian/Ubuntu (DEB Package)

1. **Download the .deb package**:
   ```bash
   wget https://github.com/93miata25/GPUFanController/releases/download/v2.3.1/gpufancontroller_2.3.1_amd64.deb
   ```

2. **Install**:
   ```bash
   sudo apt install ./gpufancontroller_2.3.1_amd64.deb
   ```
   
   Or:
   ```bash
   sudo dpkg -i gpufancontroller_2.3.1_amd64.deb
   sudo apt-get install -f  # Fix any dependencies
   ```

### Method 2: Build from Source

1. **Clone the repository**:
   ```bash
   git clone https://github.com/93miata25/GPUFanController.git
   cd GPUFanController
   ```

2. **Build for Linux**:
   ```bash
   chmod +x build-linux.sh
   ./build-linux.sh
   ```

3. **Create installer package** (optional):
   ```bash
   chmod +x create-linux-installer.sh
   ./create-linux-installer.sh
   ```

4. **Install**:
   ```bash
   sudo ./install.sh
   ```

---

## Usage

### Running Interactively

```bash
sudo gpufancontroller
```

You must use `sudo` because hardware access requires root privileges.

### Running as a System Service

If you enabled the service during installation:

```bash
# Start the service
sudo systemctl start gpufancontroller

# Enable auto-start on boot
sudo systemctl enable gpufancontroller

# Check status
sudo systemctl status gpufancontroller

# View live logs
sudo journalctl -u gpufancontroller -f

# Stop the service
sudo systemctl stop gpufancontroller
```

---

## Features

### Interactive Console Menu

The console application provides:

1. **View GPU Status** - Real-time monitoring of all GPUs
2. **Manual Fan Control** - Set fan speed percentage
3. **Automatic Fan Curves** - Choose from 4 profiles:
   - Silent (30-60%)
   - Balanced (40-80%) - Recommended
   - Performance (50-90%)
   - Aggressive (60-100%)
4. **Multi-GPU Support** - Control multiple GPUs independently

### Fan Curve Profiles

| Temperature | Silent | Balanced | Performance | Aggressive |
|------------|--------|----------|-------------|------------|
| < 50°C     | 30%    | 40%      | 50%         | 60%        |
| 50-60°C    | 40%    | 50%      | 60%         | 70%        |
| 60-70°C    | 50%    | 65%      | 75%         | 85%        |
| 70-80°C    | 60%    | 80%      | 90%         | 100%       |
| > 80°C     | 70%    | 100%     | 100%        | 100%       |

---

## Configuration

Configuration files are stored in:
```
~/.config/gpufancontroller/
```

When running as a service, configuration is stored in:
```
/root/.config/gpufancontroller/
```

---

## Troubleshooting

### Issue: "No GPU detected"

**Solutions**:
1. Verify GPU is recognized:
   ```bash
   lspci | grep -E "VGA|3D"
   ```

2. For NVIDIA, check driver:
   ```bash
   nvidia-smi
   ```

3. For AMD, check driver:
   ```bash
   lsmod | grep amdgpu
   ```

4. Ensure you're running with sudo:
   ```bash
   sudo gpufancontroller
   ```

### Issue: "Permission denied"

**Solution**: Always run with sudo:
```bash
sudo gpufancontroller
```

### Issue: Hardware monitoring not working

**Solutions**:
1. Load kernel modules:
   ```bash
   sudo modprobe i2c-dev
   sudo modprobe i2c-i801
   ```

2. Check LibreHardwareMonitor requirements:
   ```bash
   # For monitoring to work, kernel modules must be loaded
   lsmod | grep i2c
   ```

### Issue: Fan control not responding

**Possible causes**:
- Some GPUs don't support software fan control
- Driver version may not support fan control
- GPU may be in a locked state

**Solutions**:
1. Check if your GPU supports fan control:
   ```bash
   # For NVIDIA
   nvidia-settings -q GPUCurrentFanSpeed
   
   # For AMD (if using ROCm)
   rocm-smi --showfanctrl
   ```

2. Update GPU drivers to the latest version

3. Some laptop GPUs may have fan control disabled by BIOS

### Issue: Service fails to start

**Check logs**:
```bash
sudo journalctl -u gpufancontroller -n 50 --no-pager
```

**Common fixes**:
1. Ensure application has execute permissions:
   ```bash
   sudo chmod +x /opt/gpufancontroller/GPUFanControllerConsole
   ```

2. Check if another instance is running:
   ```bash
   ps aux | grep GPUFanController
   ```

---

## Uninstallation

### Using Debian Package

```bash
sudo apt remove gpufancontroller
```

### Using Install Script

```bash
sudo /opt/gpufancontroller/uninstall.sh
```

### Manual Uninstallation

```bash
# Stop and disable service
sudo systemctl stop gpufancontroller
sudo systemctl disable gpufancontroller

# Remove files
sudo rm -rf /opt/gpufancontroller
sudo rm /usr/local/bin/gpufancontroller
sudo rm /etc/systemd/system/gpufancontroller.service

# Reload systemd
sudo systemctl daemon-reload

# Remove configuration (optional)
rm -rf ~/.config/gpufancontroller
```

---

## Important Warnings

⚠️ **Always monitor GPU temperatures when using custom fan curves**

⚠️ **Setting fan speed too low can cause overheating and permanent GPU damage**

⚠️ **This application requires root access to control hardware**

⚠️ **Test custom curves under load before leaving unattended**

⚠️ **Some GPUs may not support software fan control**

---

## Building Packages

### Build Tarball Package

```bash
chmod +x build-linux.sh create-linux-installer.sh
./create-linux-installer.sh
```

Output: `installer_output/linux/GPUFanController-2.3.1-linux-x64.tar.gz`

### Build Debian Package

```bash
chmod +x build-linux.sh create-deb-package.sh
./create-deb-package.sh
```

Output: `installer_output/linux/deb/gpufancontroller_2.3.1_amd64.deb`

---

## Distribution-Specific Notes

### Ubuntu / Debian
- Use .deb package for easiest installation
- Drivers: `sudo apt install nvidia-driver-525` (or latest)

### Fedora / RHEL / CentOS
- Use tarball package
- Drivers: `sudo dnf install akmod-nvidia` (RPMFusion required)

### Arch Linux
- Use tarball package
- Drivers: `sudo pacman -S nvidia nvidia-utils`

### openSUSE
- Use tarball package  
- Drivers: `sudo zypper install nvidia-glG06`

---

## FAQ

**Q: Does this work on laptops?**
A: Yes, but some laptop GPUs have BIOS-locked fan control. It depends on the manufacturer.

**Q: Can I run this without sudo?**
A: No, hardware access requires root privileges. However, you can configure sudo to allow it without a password (not recommended for security).

**Q: Will this work with Wayland?**
A: Yes, the console application works with any display server.

**Q: Does this support ARM processors (Raspberry Pi)?**
A: Not currently. The application is built for x86_64 only.

**Q: Can I use this with dual GPUs (integrated + discrete)?**
A: Yes! The multi-GPU controller detects all compatible GPUs.

**Q: Does this interfere with other fan control software?**
A: It may conflict. Disable other GPU fan control software when using this tool.

---

## Support

- **Issues**: https://github.com/93miata25/GPUFanController/issues
- **Documentation**: See README.md and FEATURES.md
- **Discussions**: https://github.com/93miata25/GPUFanController/discussions

---

## License

This project is licensed under the MIT License. See LICENSE for details.

---

## Version

Current Version: **2.3.1**

For Windows installation, see [README.md](README.md)
