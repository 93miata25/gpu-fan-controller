# GPU Fan Controller - Linux Installation Guide

## Package Information

- **Version**: 2.3.1
- **Architecture**: x64 (amd64)
- **Package Type**: TAR.GZ or DEB (if available)

---

## Installation Methods

### Method 1: Using TAR.GZ (Universal)

1. **Extract the package**:
   ```bash
   tar -xzf gpu-fan-controller-2.3.1-linux-x64.tar.gz
   ```

2. **Install manually**:
   ```bash
   sudo cp usr/local/bin/gpu-fan-controller /usr/local/bin/
   sudo chmod +x /usr/local/bin/gpu-fan-controller
   sudo ln -s /usr/local/bin/gpu-fan-controller /usr/bin/gpu-fan-controller
   ```

3. **Run the application**:
   ```bash
   sudo gpu-fan-controller
   ```

---

### Method 2: Using DEB Package (Debian/Ubuntu)

If you have the `.deb` package:

1. **Install the package**:
   ```bash
   sudo dpkg -i gpu-fan-controller_2.3.1_amd64.deb
   ```

2. **Install dependencies** (if needed):
   ```bash
   sudo apt-get install -f
   ```

3. **Run the application**:
   ```bash
   sudo gpu-fan-controller
   ```

---

## Usage

### Basic Usage

```bash
sudo gpu-fan-controller
```

**Important**: Must run as root/sudo to access GPU hardware!

### Interactive Menu

Once started, you'll see:
```
=== GPU Fan Controller ===
1. View GPU Status
2. Set Manual Fan Speed
3. Enable Auto Mode
4. Exit
```

---

## Features

- ✅ Multi-GPU support (NVIDIA, AMD, Intel)
- ✅ Automatic fan curves (Silent, Balanced, Performance, Aggressive)
- ✅ Manual fan speed control
- ✅ Real-time temperature monitoring
- ✅ Command-line interface

---

## Requirements

- **OS**: Linux x64 (tested on Ubuntu 20.04+)
- **Permissions**: Root/sudo access required
- **GPU**: NVIDIA, AMD, or Intel GPU with fan control support

---

## Supported GPUs

### NVIDIA
- Desktop GPUs (most models with NVAPI support)
- Some laptop GPUs (check compatibility)

### AMD
- Desktop GPUs with ADL/ROCm support
- Modern Radeon cards

### Intel
- Integrated graphics with fan sensors
- Arc GPUs

---

## Troubleshooting

### "Permission denied"
```bash
sudo chmod +x /usr/local/bin/gpu-fan-controller
```

### "GPU not detected"
1. Make sure you're running with `sudo`
2. Install GPU drivers if not already installed:
   - NVIDIA: `sudo apt install nvidia-driver-xxx`
   - AMD: Install AMDGPU drivers
   - Intel: Usually included in kernel

### "Command not found"
Add to PATH:
```bash
export PATH=$PATH:/usr/local/bin
```

---

## Uninstallation

### If installed via DEB:
```bash
sudo apt remove gpu-fan-controller
```

### If installed manually:
```bash
sudo rm /usr/local/bin/gpu-fan-controller
sudo rm /usr/bin/gpu-fan-controller
sudo rm -rf /usr/share/doc/gpu-fan-controller
```

---

## Documentation

More documentation available in:
- `/usr/share/doc/gpu-fan-controller/README.md`
- `/usr/share/doc/gpu-fan-controller/FEATURES.md`
- `/usr/share/doc/gpu-fan-controller/QUICKSTART.md`

Or in the extracted package folder.

---

## Support

For issues and questions:
- GitHub: https://github.com/yourusername/GPUFanController
- Documentation: See included markdown files

---

## Safety Warning

⚠️ **Important**: Setting fan speeds too low can cause GPU overheating!
- Always monitor temperatures
- Use automatic mode when unsure
- Default GPU fan control will resume after closing the app

---

**Enjoy controlling your GPU fans!** 🚀
