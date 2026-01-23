# GPU Fan Controller - Windows Installation Guide

## Package Information

- **Version**: 2.3.1
- **Architecture**: x64 (64-bit)
- **Installer**: GPUFanController_Setup_v2.3.1.exe

---

## Installation

### Step 1: Run the Installer

1. **Double-click** `GPUFanController_Setup_v2.3.1.exe`
2. **Windows SmartScreen** may show a warning (because the app is not digitally signed):
   - Click **"More info"**
   - Click **"Run anyway"**
3. **Follow the setup wizard**:
   - Accept the license agreement
   - Choose installation folder (default: `C:\Program Files\GPU Fan Controller`)
   - Select optional desktop shortcuts
   - Click **"Install"**

### Step 2: Grant Administrator Permissions

The installer will request **Administrator privileges** - this is required to access GPU hardware.

### Step 3: First Run

After installation completes:
- Check **"Launch GPU Fan Controller"** to start immediately
- Or launch from Start Menu: **GPU Fan Controller**

---

## What Gets Installed

### Applications
- **GPU Fan Controller** (GUI) - Visual interface with graphs and controls
- **GPU Fan Controller Console** - Command-line interface for advanced users

### Locations
- **Installation**: `C:\Program Files\GPU Fan Controller\`
- **Start Menu**: `GPU Fan Controller` folder
- **Desktop**: Optional shortcut (if selected)

### Documentation
- README.md - Full documentation
- FEATURES.md - Feature list
- QUICKSTART.md - Quick start guide
- README.md - Project overview and documentation

---

## Usage

### GUI Application

**Start Menu**: GPU Fan Controller → GPU Fan Controller

**Features**:
- 🎮 Multi-GPU support (select from dropdown)
- 🌡️ Real-time temperature monitoring
- 💨 Fan speed display (RPM and %)
- 🎚️ Manual fan control slider
- 🤖 4 automatic fan curve profiles:
  - Silent (quiet, higher temps)
  - Balanced (default, good for most)
  - Performance (cooler, louder)
  - Aggressive (maximum cooling)
- 💾 Save/Load custom fan curve presets
- 🔧 Settings and diagnostics

### Console Application

**Start Menu**: GPU Fan Controller → GPU Fan Controller Console

**Command Line**:
```powershell
"C:\Program Files\GPU Fan Controller\console\GPUFanControllerConsole.exe"
```

---

## First Time Setup

### 1. Select Your GPU
If you have multiple GPUs, select the one you want to control from the dropdown.

### 2. Choose Control Mode

#### Automatic Mode (Recommended)
1. Check **"Enable Auto Mode"**
2. Select a profile:
   - **Silent** - Quiet operation, temps up to 75°C
   - **Balanced** - Good balance, temps around 65-70°C
   - **Performance** - Aggressive cooling, temps 55-65°C
   - **Aggressive** - Maximum cooling, temps below 55°C
3. Click **"Start Auto"**

#### Manual Mode (Advanced)
1. Check **"Enable Manual Control"**
2. Move the slider to set fan speed (0-100%)
3. Click **"Apply"**
4. ⚠️ **Warning**: Never set below 30% for extended periods!

---

## Features

### Multi-GPU Support
- Control each GPU independently
- Simultaneous monitoring of all GPUs
- Per-GPU auto mode settings

### System Tray
- Minimize to system tray
- Quick access to restore window
- Shows GPU temperature in tray icon
- Right-click for quick menu

### Startup Options
- **Start with Windows** - Auto-launch on boot
- **Start minimized** - Launch to system tray

### Presets
- **Save** custom fan curves
- **Load** saved presets instantly
- **Delete** unwanted presets
- Presets stored in: `%AppData%\GPUFanController\`

---

## Requirements

- **OS**: Windows 10/11 (64-bit)
- **GPU**: NVIDIA, AMD, or Intel GPU with fan control
- **Permissions**: Administrator privileges required
- **Disk Space**: ~50 MB

---

## Supported GPUs

### ✅ NVIDIA
- Desktop GPUs (most models)
- Some laptop GPUs (check compatibility)
- Requires NVIDIA drivers installed

### ✅ AMD
- Desktop Radeon cards
- Requires AMD drivers installed

### ✅ Intel
- Arc GPUs
- Some integrated graphics with fan sensors

---

## Troubleshooting

### "No compatible GPU detected"
1. **Install latest GPU drivers**:
   - NVIDIA: https://www.nvidia.com/drivers
   - AMD: https://www.amd.com/en/support
   - Intel: https://www.intel.com/content/www/us/en/download-center/home.html
2. **Run as Administrator** (right-click → Run as Administrator)
3. Click **"Diagnostics"** button to see detailed GPU info

### "Failed to set fan speed"
- Your GPU may not support manual fan control
- Some laptop GPUs have locked fan curves
- Click **"Diagnostics"** to check fan control support

### Windows SmartScreen Warning
- Normal for unsigned applications
- Click **"More info"** → **"Run anyway"**
- The app is safe - source code available on GitHub

### App Crashes on Startup
1. Check Windows Event Viewer for errors
2. Try running as Administrator
3. Update GPU drivers
4. Check `%AppData%\GPUFanController\` for error logs

---

## Updating

When a new version is available:
1. Click **"Check Update"** button in the app
2. Or re-run the installer - it will upgrade automatically
3. Your settings and presets are preserved

---

## Uninstallation

### Method 1: Start Menu
1. **Start Menu** → **GPU Fan Controller** → **"Uninstall GPU Fan Controller"**
2. Follow the uninstall wizard
3. Choose to keep or remove settings

### Method 2: Windows Settings
1. **Settings** → **Apps** → **Installed Apps**
2. Find **"GPU Fan Controller"**
3. Click **"..."** → **"Uninstall"**

### Clean Uninstall
To remove all settings and presets:
```powershell
Remove-Item "$env:APPDATA\GPUFanController" -Recurse -Force
```

---

## Safety & Best Practices

### ⚠️ Important Safety Information

**DO:**
- ✅ Monitor temperatures regularly
- ✅ Use automatic mode when unsure
- ✅ Keep fan speed above 30% under load
- ✅ Test new settings while monitoring temps
- ✅ Have good case airflow

**DON'T:**
- ❌ Set fan to 0% or very low speeds
- ❌ Ignore high temperature warnings
- ❌ Leave manual mode unattended for hours
- ❌ Block GPU air vents

### Safe Temperature Ranges
- **Normal**: 40-75°C
- **Warning**: 75-85°C
- **Dangerous**: 85°C+

If temps exceed 85°C:
1. Increase fan speed immediately
2. Check for dust/blockage
3. Consider better cooling solution

---

## Analytics & Privacy

The application collects **anonymous usage analytics**:
- ✅ App launches
- ✅ Active usage (heartbeat every 5 minutes)
- ❌ NO personal information
- ❌ NO GPU serial numbers
- ❌ NO fan speed settings

Analytics help improve the app. Data is collected via Google Analytics 4.

---

## Support & Documentation

### Documentation Included
- `README.md` - Complete documentation
- `FEATURES.md` - Full feature list
- `QUICKSTART.md` - Quick setup guide
- Location: `C:\Program Files\GPU Fan Controller\`

### Online Resources
- GitHub: https://github.com/93miata25/GPUFanController
- Issues: Report bugs via GitHub Issues

---

## Keyboard Shortcuts

- **F11** - Toggle fullscreen
- **ESC** - Exit fullscreen

---

## Command Line Arguments

### Start Minimized
```powershell
GPUFanController.exe --minimized
```

Useful for startup shortcuts.

---

**Enjoy controlling your GPU fans!** 🚀

**Remember**: Always monitor temperatures when using manual fan control!
