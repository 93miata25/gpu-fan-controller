# GPU Fan Controller - Enhanced Edition

A comprehensive cross-platform application to control GPU fan speed with advanced features including multi-GPU support, automatic fan curves, and both GUI (Windows) and Console (Windows/Linux) interfaces.

## 📊 Analytics & Privacy

This application includes **privacy-focused analytics** to help track adoption and usage:
- ✅ **Completely anonymous** - Uses random device IDs, no personal information collected
- ✅ **Privacy-first** - Only tracks: install count, app starts, app version
- ✅ **Non-intrusive** - Never interrupts user experience, fails silently if unavailable
- ✅ **Transparent** - Full source code available for review

**For developers**: To set up analytics tracking for your fork, see **[Analytics Setup Guide](ANALYTICS_SETUP_GUIDE.md)**.

## 🌟 Key Features

### Multi-GPU Support
- 🎮 **Detect Multiple GPUs**: Automatically detects all NVIDIA, AMD, and Intel GPUs
- 🔀 **Independent Control**: Control each GPU separately
- 📊 **Unified Monitoring**: View all GPUs at once

### Automatic Fan Control (NEW!)
- 🤖 **Smart Fan Curves**: Temperature-based automatic fan speed adjustment
- 📈 **4 Built-in Profiles**: Silent, Balanced, Performance, Aggressive
- ⚡ **Real-time Adjustment**: Updates every 2 seconds based on temperature
- 🎯 **Optimized Cooling**: Each profile balances noise vs. cooling differently

### Manual Control
- 🎚️ **Slider Control**: Easy-to-use slider for adjusting fan speed (0-100%)
- 🎯 **Per-GPU Control**: Set different speeds for different GPUs
- ✋ **Direct Input**: Type exact values in console mode

### Real-time Monitoring
- 🌡️ **Temperature Display**: GPU core temperature with color coding
- 🌀 **Fan RPM**: Actual fan rotation speed
- 📊 **Control Percentage**: Current fan speed setting
- 🔄 **Auto-refresh**: Updates every 1-2 seconds

### Two Application Versions
- 🖥️ **GUI Version**: Windows Forms interface with visual controls
- ⌨️ **Console Version**: Terminal-based interface for power users

### Safety Features
- ⚠️ **Safety Warnings**: Alerts for dangerous fan speeds (<30%)
- 🎨 **Temperature Color Coding**: Green/Yellow/Orange/Red indicators
- 🔄 **Auto-reset on Exit**: Returns to driver defaults automatically
- 🔒 **Administrator Requirement**: Prevents unauthorized access

## Requirements

### Windows
- **Operating System**: Windows 10/11 (64-bit)
- **GPU**: NVIDIA, AMD, or Intel GPU with fan control support
- **.NET Runtime**: .NET 6.0 or later (included in installer)
- **Privileges**: Must run as Administrator

### Linux
- **Operating System**: Linux kernel 4.0+ (x86_64)
- **GPU**: NVIDIA, AMD, or Intel GPU with fan control support
- **GPU Drivers**: nvidia-driver (NVIDIA), amdgpu (AMD), i915 (Intel)
- **.NET Runtime**: Included in package (self-contained)
- **Privileges**: Must run with sudo/root

## 📦 What's Included

```
GPUFanController/
├── GUI Application (Windows Forms)
│   ├── GPUFanController.csproj      # GUI project file
│   ├── Program.cs                    # GUI entry point
│   ├── MainForm.cs                   # Main GUI window
│   ├── build.bat                     # Build GUI version
│   ├── run.bat                       # Run GUI version
│   └── publish.bat                   # Create standalone GUI executable
│
├── Console Application
│   ├── GPUFanControllerConsole.csproj  # Console project file
│   ├── ProgramConsole.cs               # Console entry point
│   ├── ConsoleApp.cs                   # Console UI logic
│   ├── build-console.bat               # Build console version
│   ├── run-console.bat                 # Run console version
│   └── publish-console.bat             # Create standalone console executable
│
├── Shared Components
│   ├── GPUController.cs              # Single GPU controller
│   ├── MultiGPUController.cs         # Multi-GPU support
│   ├── FanCurveProfile.cs            # Fan curve profiles
│   ├── AutoFanController.cs          # Automatic fan control
│   └── app.manifest                  # Admin privileges
│
└── Documentation
    ├── README.md                     # This file
    ├── FEATURES.md                   # Detailed feature list
    ├── QUICKSTART.md                 # Quick start guide
    └── .gitignore                    # Git configuration
```

## 🚀 Quick Start

### For End Users (Installer)

#### Windows Installation

**Easiest Method - Use the Installer:**

1. Download `GPUFanController_Setup_v2.3.1.exe`
2. Double-click to install
3. Follow the installation wizard
4. Launch from Start Menu or desktop shortcut
5. Grant administrator privileges when prompted

**What the installer includes:**
- ✅ GUI and Console versions
- ✅ Start Menu shortcuts
- ✅ Optional desktop shortcuts
- ✅ All documentation
- ✅ Uninstaller
- ✅ No .NET installation needed

#### Linux Installation

**Option 1: Universal Package (All Distributions)**

```bash
# Download and extract
wget https://github.com/yourusername/GPUFanController/releases/download/v2.3.1/GPUFanController-2.3.1-linux-x64.tar.gz
tar -xzf GPUFanController-2.3.1-linux-x64.tar.gz
cd GPUFanController-2.3.1-linux-x64

# Install
sudo ./install.sh
```

**Option 2: Debian/Ubuntu Package**

```bash
# Download and install
wget https://github.com/yourusername/GPUFanController/releases/download/v2.3.1/gpufancontroller_2.3.1_amd64.deb
sudo apt install ./gpufancontroller_2.3.1_amd64.deb
```

**Usage on Linux:**
```bash
sudo gpufancontroller
```

📖 **See [LINUX_INSTALLATION_GUIDE.md](LINUX_INSTALLATION_GUIDE.md) for complete Linux documentation**

---

### For Developers (Build from Source)

#### Prerequisites
1. **Download .NET 6.0 SDK**: https://dotnet.microsoft.com/download/dotnet/6.0
2. **Ensure Administrator Privileges**: Required for hardware access

#### GUI Version (Recommended for Most Users)
```batch
# Build
build.bat

# Run (as Administrator)
Right-click run.bat → "Run as Administrator"
```

#### Console Version (For Power Users)
```batch
# Build
build-console.bat

# Run (as Administrator)
Right-click run-console.bat → "Run as Administrator"
```

#### Create Standalone Executables
```batch
# GUI version
publish.bat
# Output: bin\Release\net6.0-windows\win-x64\publish\GPUFanController.exe

# Console version
publish-console.bat
# Output: bin\Release\net6.0\win-x64\publish\GPUFanControllerConsole.exe
```

#### Create Windows Installer
```batch
# Requires Inno Setup (https://jrsoftware.org/isdl.php)
build-installer.bat
# Output: installer_output\GPUFanController_Setup_v2.0.exe
```

**See `INSTALLER_GUIDE.md` for detailed instructions on creating the installer.**

## 💡 Usage Guide

### GUI Version

#### 1. Select Your GPU
- Use the dropdown at the top to select which GPU to control
- Each GPU can have different settings

#### 2. Monitor Status
- View real-time temperature, fan RPM, and fan speed percentage
- Temperature color coding: 🟢 Green (<60°C) → 🟡 Yellow (<75°C) → 🟠 Orange (<85°C) → 🔴 Red (≥85°C)

#### 3. Choose Control Mode

**Option A: Automatic Fan Control (Recommended)**
1. Check "Enable Auto Mode (Fan Curves)"
2. Select a profile:
   - **Silent**: Quiet operation, moderate cooling
   - **Balanced**: Good compromise (default)
   - **Performance**: Aggressive cooling
   - **Aggressive**: Maximum cooling, louder
3. Click "Start Auto"
4. Fan speed automatically adjusts based on temperature

**Option B: Manual Control**
1. Check "Enable Manual Control"
2. Move the slider to desired fan speed (30-100% recommended)
3. Click "Apply"
4. Monitor temperature to ensure safe operation

#### 4. Reset to Driver Defaults
- Click "Reset to Auto" to return control to GPU drivers
- This stops any active auto or manual control

### Console Version

#### Main Menu Options
```
[1] Monitor All GPUs       - Real-time monitoring (Press ESC to exit)
[2] Manual Fan Control     - Set specific fan speed
[3] Auto Fan Control       - Start temperature-based fan curves
[4] Reset All to Auto      - Return all GPUs to driver control
[5] View GPU Details       - Show detailed GPU information
[0] Exit                   - Exit application (resets to auto)
```

#### Monitoring Mode
- Shows all GPUs simultaneously with color-coded temperatures
- Updates every second
- Press ESC to return to menu

#### Manual Control
- Select GPU
- Enter fan speed percentage
- Confirms dangerous values (<30%)

#### Auto Fan Control
- Select GPU
- Choose profile (Silent/Balanced/Performance/Aggressive)
- Monitor in real-time
- Press ESC to stop

## Safety Warnings

⚠️ **IMPORTANT**: Manual fan control can be dangerous!

- **Never set fan speed below 30%** unless you know what you're doing
- **Monitor temperatures** - High temps (>85°C) can damage your GPU
- **Test gradually** - Start with higher speeds and work down
- **Use at your own risk** - Improper fan control can cause hardware damage

## Troubleshooting

### "No compatible GPU detected"

**Solutions:**
- Ensure you're running as Administrator (required for hardware access)
- Update your GPU drivers to the latest version
- Check that your GPU is properly installed and recognized by Windows

### "Failed to set fan speed"

**Possible causes:**
- Your GPU may not support software fan control
- Some GPUs require manufacturer-specific tools (MSI Afterburner, EVGA Precision, etc.)
- Driver restrictions may prevent fan control
- Try restarting the application as Administrator

### Application won't start

- Install [.NET 6.0 Runtime](https://dotnet.microsoft.com/download/dotnet/6.0)
- Right-click the executable and select "Run as Administrator"

## 📊 Fan Curve Profiles Explained

### Profile Comparison Chart

| Temperature | Silent | Balanced | Performance | Aggressive |
|-------------|--------|----------|-------------|------------|
| 0-50°C      | 30-35% | 35-40%   | 40-50%      | 50-60%     |
| 60°C        | 40%    | 50%      | 60%         | 70%        |
| 70°C        | 50%    | 65%      | 75%         | 85%        |
| 80°C        | 75%    | 85%      | 95%         | 100%       |
| 85°C+       | 90-100%| 95-100%  | 100%        | 100%       |

### When to Use Each Profile

- **Silent**: Office work, web browsing, light gaming (noise-sensitive environments)
- **Balanced**: General gaming, video streaming (default recommendation)
- **Performance**: Heavy gaming, 3D rendering, video editing
- **Aggressive**: Overclocking, sustained maximum load, stress testing

## 🔧 Technical Details

### Architecture
- **Hardware Access**: LibreHardwareMonitorLib for sensor reading and fan control
- **Framework**: .NET 6.0 (cross-platform, modern C#)
- **GUI**: Windows Forms (mature, reliable UI framework)
- **Console**: Native terminal interface with color support

### How it Works
1. Application opens hardware interface with administrator privileges
2. Detects all GPUs (NVIDIA, AMD, Intel) in the system
3. Locates temperature, fan speed (RPM), and fan control sensors
4. In manual mode: Directly sets fan control percentage
5. In auto mode: Monitors temperature and adjusts fan speed per curve
6. Updates readings every 1-2 seconds
7. On exit: Returns all GPUs to driver default control

### Compatibility

**Supported GPUs:**
- ✅ NVIDIA GeForce (most models with fan control)
- ✅ AMD Radeon (most models with fan control)
- ✅ Intel Arc (with fan control support)

**Limitations:**
- ⚠️ Some laptop GPUs have hardware-locked fan curves
- ⚠️ Fan control availability depends on GPU model and BIOS
- ⚠️ Some OEM cards may require manufacturer software
- ⚠️ Requires Windows with administrator privileges
- ⚠️ Virtual machines may not have hardware access

## 🛠️ Development

### Project Structure (Detailed)

```
GPUFanController/
├── Core Components
│   ├── GPUController.cs           - Single GPU control logic
│   ├── MultiGPUController.cs      - Multi-GPU management
│   ├── FanCurveProfile.cs         - Fan curve definitions
│   └── AutoFanController.cs       - Automatic adjustment engine
│
├── GUI Application
│   ├── GPUFanController.csproj    - GUI project configuration
│   ├── Program.cs                 - GUI entry point
│   └── MainForm.cs                - Main window (680px tall)
│
├── Console Application
│   ├── GPUFanControllerConsole.csproj - Console project config
│   ├── ProgramConsole.cs          - Console entry point
│   └── ConsoleApp.cs              - Menu system and UI logic
│
├── Configuration
│   ├── app.manifest               - Admin privileges requirement
│   └── .gitignore                 - Git exclusions
│
├── Build Scripts
│   ├── build.bat                  - Build GUI version
│   ├── run.bat                    - Run GUI with admin check
│   ├── publish.bat                - Create standalone GUI exe
│   ├── build-console.bat          - Build console version
│   ├── run-console.bat            - Run console with admin check
│   └── publish-console.bat        - Create standalone console exe
│
└── Documentation
    ├── README.md                  - Main documentation (this file)
    ├── FEATURES.md                - Detailed feature descriptions
    └── QUICKSTART.md              - Quick start guide
```

### Building from Source

#### Windows

```batch
# GUI Version
build.bat                          # Builds to bin/Release/
dotnet build -c Debug              # Debug build

# Console Version
build-console.bat                  # Builds to bin/Release/
dotnet build GPUFanControllerConsole.csproj -c Debug

# Both with dotnet CLI
dotnet build --configuration Release
```

#### Linux

```bash
# Console Version (Linux only supports console)
chmod +x build-linux.sh
./build-linux.sh

# Create distribution packages
chmod +x build-everything-linux.sh
./build-everything-linux.sh        # Creates .tar.gz and .deb packages
```

📖 **See [BUILD_LINUX_QUICK_START.md](BUILD_LINUX_QUICK_START.md) for detailed Linux build instructions**

### Running in Development

```batch
# GUI (with auto admin elevation)
run.bat

# Console (with auto admin elevation)
run-console.bat

# Direct dotnet run (must be admin already)
dotnet run --project GPUFanController.csproj
dotnet run --project GPUFanControllerConsole.csproj
```

### Creating Standalone Executables

```batch
# Single-file executables (no .NET required)
publish.bat                        # GUI → 50-80MB exe
publish-console.bat                # Console → 50-80MB exe

# Manual publish commands
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## 📈 Version History

### v2.0 - Enhanced Edition (Current)
- ✅ Multi-GPU support
- ✅ Automatic fan curves (4 profiles)
- ✅ Console application version
- ✅ Temperature-based auto adjustment
- ✅ Enhanced GUI with profile selection
- ✅ Per-GPU independent control

### v1.0 - Initial Release
- ✅ Single GPU manual control
- ✅ GUI with slider interface
- ✅ Basic temperature monitoring
- ✅ Safety warnings

## 🤝 Contributing

Contributions are welcome! Here are some ideas:
- Custom fan curve editor
- Temperature logging and graphs
- System tray integration
- Additional GPU vendors support
- Per-application profiles
- Remote control interface

Feel free to submit issues and pull requests!

## License

This project is provided as-is for educational purposes.

## Disclaimer

**USE AT YOUR OWN RISK**

This software modifies hardware settings and could potentially cause:
- Hardware damage from overheating
- System instability
- Warranty void

The authors are not responsible for any damage caused by using this software.
Always monitor your GPU temperatures and use safe fan speed values.

## Credits

- Built with [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor)
- Developed for Windows desktop environments
