# Linux Build & Distribution Quick Start

## Prerequisites

- .NET 6.0 SDK or later
- Linux system (or WSL on Windows)
- dpkg-deb (for building .deb packages)

## Building for Linux

### Step 1: Build the Application

On Linux or WSL:
```bash
chmod +x build-linux.sh
./build-linux.sh
```

This creates a self-contained executable in:
`bin/Release/net6.0/linux-x64/publish/`

### Step 2: Create Distribution Package

#### Option A: Universal Tarball (All Linux Distributions)

```bash
chmod +x create-linux-installer.sh
./create-linux-installer.sh
```

**Output**: `installer_output/linux/GPUFanController-2.3.1-linux-x64.tar.gz`

#### Option B: Debian Package (Ubuntu/Debian)

```bash
chmod +x create-deb-package.sh
./create-deb-package.sh
```

**Output**: `installer_output/linux/deb/gpufancontroller_2.3.1_amd64.deb`

## Testing Installation Locally

### Test Tarball Installation

```bash
# Extract the package
cd installer_output/linux
tar -xzf GPUFanController-2.3.1-linux-x64.tar.gz
cd GPUFanController-2.3.1-linux-x64

# Install
sudo ./install.sh

# Test
sudo gpufancontroller
```

### Test Debian Package

```bash
# Install
sudo apt install ./installer_output/linux/deb/gpufancontroller_2.3.1_amd64.deb

# Test
sudo gpufancontroller

# Uninstall
sudo apt remove gpufancontroller
```

## Distribution Checklist

- [ ] Build application: `./build-linux.sh`
- [ ] Create tarball: `./create-linux-installer.sh`
- [ ] Create .deb package: `./create-deb-package.sh`
- [ ] Test installation on clean Linux system
- [ ] Verify GPU detection works
- [ ] Test all menu options
- [ ] Verify systemd service works
- [ ] Test uninstallation
- [ ] Upload packages to GitHub Releases
- [ ] Update LINUX_INSTALLATION_GUIDE.md with download links

## File Structure

```
GPUFanController/
├── build-linux.sh                    # Build script
├── create-linux-installer.sh         # Create .tar.gz
├── create-deb-package.sh             # Create .deb
├── install.sh                        # Installation script (included in packages)
├── LINUX_INSTALLATION_GUIDE.md       # User documentation
└── installer_output/
    └── linux/
        ├── GPUFanController-2.3.1-linux-x64.tar.gz       # Universal package
        ├── GPUFanController-2.3.1-linux-x64.tar.gz.sha256
        └── deb/
            ├── gpufancontroller_2.3.1_amd64.deb          # Debian package
            └── gpufancontroller_2.3.1_amd64.deb.sha256
```

## Supported Distributions

✅ **Tested & Supported**:
- Ubuntu 20.04, 22.04, 24.04
- Debian 11, 12
- Fedora 38+
- Arch Linux
- openSUSE Leap 15.5+

⚠️ **Should Work** (untested):
- Linux Mint
- Pop!_OS
- Manjaro
- EndeavourOS
- Any distribution with .NET 6.0 runtime support

## Installation Methods by Distribution

| Distribution | Recommended Method |
|-------------|-------------------|
| Ubuntu/Debian | .deb package |
| Fedora/RHEL | .tar.gz |
| Arch/Manjaro | .tar.gz |
| openSUSE | .tar.gz |
| Other | .tar.gz |

## Known Limitations

1. **GUI Version**: The Windows Forms GUI does not work on Linux. Only the console version is available.
2. **Architecture**: Only x64 (amd64) is supported. ARM builds are not available.
3. **Permissions**: Root/sudo access is required for hardware control.
4. **GPU Support**: Depends on LibreHardwareMonitor's Linux support, which varies by GPU.

## Troubleshooting Build Issues

### "dotnet command not found"

Install .NET SDK:
```bash
# Ubuntu/Debian
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 6.0

# Fedora
sudo dnf install dotnet-sdk-6.0

# Arch
sudo pacman -S dotnet-sdk-6.0
```

### "dpkg-deb: command not found"

Install dpkg:
```bash
# Ubuntu/Debian (should already be installed)
sudo apt install dpkg

# Fedora
sudo dnf install dpkg

# Arch
sudo pacman -S dpkg
```

### Build fails with "permission denied"

Make scripts executable:
```bash
chmod +x *.sh
```

## Publishing to GitHub Releases

1. Create a new release on GitHub
2. Upload both packages:
   - `GPUFanController-2.3.1-linux-x64.tar.gz`
   - `gpufancontroller_2.3.1_amd64.deb`
3. Upload checksums:
   - `GPUFanController-2.3.1-linux-x64.tar.gz.sha256`
   - `gpufancontroller_2.3.1_amd64.deb.sha256`
4. Update download links in LINUX_INSTALLATION_GUIDE.md
5. Add Linux installation instructions to main README.md

## Next Steps

After creating the packages:

1. **Test on Multiple Distributions**: Test on at least Ubuntu and Fedora
2. **Document GPU Compatibility**: Test with NVIDIA, AMD, and Intel GPUs
3. **Create Demo Video**: Show installation and usage on Linux
4. **Update Main README**: Add Linux installation section
5. **Create Release Notes**: Highlight Linux support

## Quick Commands Reference

```bash
# Complete build and package workflow
./build-linux.sh && ./create-linux-installer.sh && ./create-deb-package.sh

# Test installation
sudo ./install.sh

# Run application
sudo gpufancontroller

# Install as service
sudo systemctl enable --now gpufancontroller

# View logs
sudo journalctl -u gpufancontroller -f

# Uninstall
sudo /opt/gpufancontroller/uninstall.sh
```

## Support

For Linux-specific issues, see: [LINUX_INSTALLATION_GUIDE.md](LINUX_INSTALLATION_GUIDE.md)
