# Linux Quick Reference Card

## 🚀 For Developers - Building Packages

```bash
# One command to build everything
chmod +x build-everything-linux.sh
./build-everything-linux.sh
```

**Output:**
- `installer_output/linux/GPUFanController-2.3.1-linux-x64.tar.gz`
- `installer_output/linux/deb/gpufancontroller_2.3.1_amd64.deb`

---

## 👥 For Users - Installation

### Ubuntu/Debian (.deb package)
```bash
wget [URL]/gpufancontroller_2.3.1_amd64.deb
sudo apt install ./gpufancontroller_2.3.1_amd64.deb
```

### All Linux Distributions (.tar.gz)
```bash
wget [URL]/GPUFanController-2.3.1-linux-x64.tar.gz
tar -xzf GPUFanController-2.3.1-linux-x64.tar.gz
cd GPUFanController-2.3.1-linux-x64
sudo ./install.sh
```

---

## 💻 Usage

### Basic
```bash
sudo gpufancontroller
```

### As Service
```bash
sudo systemctl enable --now gpufancontroller  # Enable and start
sudo systemctl status gpufancontroller         # Check status
sudo journalctl -u gpufancontroller -f         # View logs
```

---

## 🗑️ Uninstallation

```bash
# If installed from tarball
sudo /opt/gpufancontroller/uninstall.sh

# If installed from .deb
sudo apt remove gpufancontroller
```

---

## 📚 Full Documentation

| Document | Purpose |
|----------|---------|
| [LINUX_INSTALLATION_GUIDE.md](LINUX_INSTALLATION_GUIDE.md) | Complete user guide |
| [BUILD_LINUX_QUICK_START.md](BUILD_LINUX_QUICK_START.md) | Developer build guide |

---

## ⚡ System Requirements

- Linux kernel 4.0+ (x86_64)
- GPU with fan control support (NVIDIA/AMD/Intel)
- GPU drivers installed
- Root/sudo access
- ~180MB disk space

---

## ⚠️ Important

- **Console only** - No GUI on Linux
- **Requires sudo** - Hardware access needs root
- **GPU drivers** - Must be installed and working
- **Monitor temps** - Always watch temperatures with custom curves
