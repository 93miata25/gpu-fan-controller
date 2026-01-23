# GPU Fan Controller

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Linux-green.svg)]()
[![.NET](https://img.shields.io/badge/.NET-6.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/6.0)

A powerful, open-source GPU fan controller with automatic temperature-based fan curves, multi-GPU support, GUI (Windows) and Console (Linux/Windows) interfaces.

---

## ✨ Features

### 🎮 Multi-GPU Support
- Automatically detects NVIDIA, AMD, and Intel GPUs
- Control each GPU independently
- Monitor all GPUs simultaneously

### 🌡️ Automatic Fan Control
- **Smart temperature-based fan curves**
- **4 built-in profiles**: Silent, Balanced, Performance, Aggressive
- Real-time adjustment every 2 seconds
- Set it and forget it - maintains optimal temperatures

### 🎛️ Manual Control
- Precise fan speed control (0-100%)
- Real-time fan RPM monitoring
- Safety warnings for dangerous settings

### 📊 Real-time Monitoring
- GPU temperature with color-coded warnings
- Fan RPM display
- Current fan speed percentage
- Auto-refresh every 2 seconds

### 🖥️ Interfaces
- **GUI Version** (Windows): Full-featured Windows Forms application with system tray support
- **Console Version** (Windows/Linux): Lightweight terminal interface

### 🛡️ Safety Features
- Automatic safety warnings
- Temperature-based color coding
- Auto-reset to driver defaults on exit
- Requires administrator privileges

---

## 🚀 Quick Start

### Windows (GUI)

1. **Run installer**: `GPUFanController_Setup.exe`
2. **Launch** from Start Menu or Desktop

### Windows/Linux (Console)

```bash
# Download and extract
wget https://github.com/93miata25/gpu-fan-controller/releases/latest/download/GPUFanController-Console-linux.tar.gz
tar -xzf GPUFanController-Console-linux.tar.gz

# Run with sudo (required for hardware access)
sudo ./GPUFanControllerConsole
```

---

## 🛠️ Building from Source

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- Windows: Visual Studio 2022 (optional but recommended)
- Linux: Build essentials

### Windows Build

```batch
# Clone repository
git clone https://github.com/93miata25/gpu-fan-controller.git
cd gpu-fan-controller

# Build GUI version
dotnet publish GPUFanController.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

# Build Console version
dotnet publish GPUFanControllerConsole.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

# Output: bin/Release/net6.0-windows/win-x64/publish/
```

### Linux Build

```bash
# Clone repository
git clone https://github.com/93miata25/gpu-fan-controller.git
cd gpu-fan-controller

# Build Console version (GUI not available on Linux)
dotnet publish GPUFanControllerConsole.csproj -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true

# Output: bin/Release/net6.0/linux-x64/publish/
```

### Build Scripts

**Windows:**
```batch
build.bat              # Build GUI version
build-console.bat      # Build console version
build-everything.bat   # Build both versions
```

**Linux:**
```bash
chmod +x build-linux.sh
./build-linux.sh       # Build console version
```

---

## 📖 Usage Guide

### Automatic Mode (Recommended)

1. Launch application
2. Check **"Enable Auto Mode"**
3. Select a profile:
   - **Silent**: 30-60% fan speed (40-80°C)
   - **Balanced**: 40-75% fan speed (45-85°C) ⭐ Recommended
   - **Performance**: 50-85% fan speed (50-90°C)
   - **Aggressive**: 60-100% fan speed (55-95°C)
4. Click **"Start Auto"**
5. Minimize to system tray - it runs in the background!

### Manual Mode

1. Launch application
2. Check **"Enable Manual Control"**
3. Use slider or type value (0-100%)
4. Click **"Apply"**
5. ⚠️ **Monitor temperature closely!**

### Console Commands

```
1. View GPU Status
2. Set Manual Fan Speed
3. Set Automatic Mode
4. Multi-GPU Management
5. Exit
```

---

## ⚠️ Important Safety Information

### ⚠️ WARNING: Hardware Control

This software directly controls GPU hardware. **Improper use can cause:**
- 🔥 GPU overheating and permanent damage
- 💥 System crashes and instability
- 🚫 Voided hardware warranties
- 💾 Data loss

### 🛡️ Safety Guidelines

✅ **DO:**
- Use automatic mode for daily use
- Monitor temperatures regularly
- Keep fan speed above 30%
- Set aggressive profile if temps exceed 80°C
- Reset to auto when not monitoring

❌ **DON'T:**
- Set fans below 30% without constant monitoring
- Leave manual mode running unattended
- Ignore temperature warnings (>85°C is dangerous!)
- Use on laptops (usually not supported)

### 🚨 Emergency

**If GPU temperature exceeds 85°C:**
1. Immediately set fan speed to 100%
2. Or click "Reset to Auto"
3. Close demanding applications
4. Check for dust/airflow issues

---

## 🔧 Configuration

### Presets

Save and load custom fan curves:
- Located in: `%AppData%/GPUFanController/`
- JSON format for easy editing
- Share presets with others

### Startup Options

- **Start with Windows**: Auto-launch on boot
- **Start minimized**: Launch to system tray
- **Restore last profile**: Remember your settings

### Analytics (Optional)

This project includes privacy-focused analytics to track installs and active users:
- ✅ **Completely anonymous** - No personal data
- ✅ **Open source** - Review the code yourself
- ✅ **Optional** - Works fine without it

**For developers**: See [Analytics Setup](#analytics-setup-for-developers) below.

---

## 🖥️ System Requirements

### Windows
- Windows 10/11 (64-bit)
- .NET 6.0 Runtime (included in installer)
- Administrator privileges
- Supported GPU (NVIDIA, AMD, or Intel)

### Linux
- Ubuntu 20.04+ / Fedora 35+ / Arch Linux
- .NET 6.0 Runtime
- Root/sudo access
- Supported GPU (NVIDIA, AMD, or Intel)

### Supported GPUs

✅ **NVIDIA**: Most GeForce, Quadro, RTX cards
✅ **AMD**: Most Radeon RX, Vega, RDNA cards
✅ **Intel**: Arc, Xe, integrated GPUs

❌ **Not Supported**: Most laptop GPUs (locked by manufacturer)

---

## 📦 Project Structure

```
gpu-fan-controller/
├── MainForm.cs              # GUI interface
├── ConsoleApp.cs            # Console interface
├── MultiGPUController.cs    # Multi-GPU management
├── AutoFanController.cs     # Automatic fan curves
├── FanCurveProfile.cs       # Profile definitions
├── AnalyticsService.cs      # Privacy-focused analytics
├── UpdateChecker.cs         # Version checking
├── GPUFanController.csproj  # GUI project
├── GPUFanControllerConsole.csproj  # Console project
├── build.bat                # Windows build script
├── build-linux.sh           # Linux build script
└── installer.iss            # Inno Setup installer script
```

---

## 🔐 Analytics Setup (For Developers)

If you fork this project and want to enable analytics:

### 1. Get Google Analytics 4 Credentials

1. Go to [Google Analytics](https://analytics.google.com/)
2. Create a GA4 property
3. Get your **Measurement ID** (G-XXXXXXXXXX)
4. Get your **API Secret** from Admin → Data Streams → Measurement Protocol API secrets

### 2. Set Environment Variables

**Windows (PowerShell):**
```powershell
[System.Environment]::SetEnvironmentVariable('GA_MEASUREMENT_ID', 'G-XXXXXXXXXX', 'User')
[System.Environment]::SetEnvironmentVariable('GA_API_SECRET', 'your_api_secret', 'User')
```

**Windows (CMD):**
```batch
setx GA_MEASUREMENT_ID "G-XXXXXXXXXX"
setx GA_API_SECRET "your_api_secret"
```

**Linux/macOS:**
```bash
export GA_MEASUREMENT_ID="G-XXXXXXXXXX"
export GA_API_SECRET="your_api_secret"

# Add to ~/.bashrc or ~/.zshrc for persistence
echo 'export GA_MEASUREMENT_ID="G-XXXXXXXXXX"' >> ~/.bashrc
echo 'export GA_API_SECRET="your_api_secret"' >> ~/.bashrc
```

### 3. Rebuild Project

The application will automatically use environment variables if they're set.

**Without analytics:** App works perfectly fine, analytics just won't send data.

---

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

### 🐛 Report Bugs

[Open an issue](https://github.com/93miata25/gpu-fan-controller/issues) with:
- Your GPU model and driver version
- Operating system
- Steps to reproduce
- Screenshots if applicable

### 💡 Suggest Features

[Open an issue](https://github.com/93miata25/gpu-fan-controller/issues) with:
- Clear description of the feature
- Why it would be useful
- Possible implementation ideas

### 🔧 Submit Pull Requests

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### 📝 Improve Documentation

Documentation improvements are always appreciated!

---

## 🧪 Testing

### Run Unit Tests
```bash
dotnet test
```

### Test on Real Hardware

**Before submitting GPU-related changes:**
1. Test on multiple GPU brands (NVIDIA, AMD, Intel)
2. Test temperature monitoring accuracy
3. Test fan speed control responsiveness
4. Verify safety warnings work correctly

---

## 📜 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

### What This Means

✅ Commercial use allowed
✅ Modification allowed
✅ Distribution allowed
✅ Private use allowed
❌ No warranty provided
❌ No liability accepted

---

## ⚠️ Disclaimer

**USE AT YOUR OWN RISK**

This software controls hardware and can cause damage if used improperly. The authors are not responsible for:
- Hardware damage or failure
- Data loss
- System instability
- Voided warranties
- Any other consequences of using this software

**Always monitor your GPU temperatures and use caution when adjusting fan speeds manually.**

---

## 🙏 Acknowledgments

### Dependencies

- [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) - Hardware monitoring library
- [HidSharp](https://github.com/IntergatedCircuits/HidSharpCore) - USB HID library
- [.NET 6.0](https://dotnet.microsoft.com/) - Application framework

### Inspiration

Thanks to all the open-source GPU monitoring and control projects that inspired this tool!

---

## 📞 Support

- 🐛 [Issue Tracker](https://github.com/93miata25/gpu-fan-controller/issues)
- 💬 [Discussions](https://github.com/93miata25/gpu-fan-controller/discussions)
- 📖 [Installation Guides](INSTALL_WINDOWS.md) | [Linux Guide](INSTALL_LINUX.md)
- 🚀 [Quick Start](QUICKSTART.md) | [Features](FEATURES.md)

---

## 🔗 Links

- **Releases**: [Latest version](https://github.com/93miata25/gpu-fan-controller/releases)
- **Contributing**: [CONTRIBUTING.md](CONTRIBUTING.md)
- **License**: [MIT License](LICENSE)

---

## ⭐ Star History

If you find this project useful, please consider giving it a star! ⭐

---

**Made with ❤️ for the PC enthusiast community**

*Stay cool, overclock responsibly!* 🚀
