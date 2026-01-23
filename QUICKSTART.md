# Quick Start Guide - GPU Fan Controller

## 🎁 For End Users (Using Installer)

**If you have the installer file (GPUFanController_Setup_v2.0.exe):**

### Installation (30 seconds)
1. **Double-click** `GPUFanController_Setup_v2.0.exe`
2. Click **"Yes"** on UAC prompt (needs admin rights)
3. Click **"Next"** through the wizard
4. Choose if you want desktop shortcuts (optional)
5. Click **"Install"**
6. Click **"Finish"**

### First Use
1. **Open Start Menu** → "GPU Fan Controller"
2. **Click "Yes"** on UAC prompt (required for hardware access)
3. **Select your GPU** from dropdown (if multiple)
4. **Choose control mode:**
   - **Auto Mode** (Recommended): Check "Enable Auto Mode" → Select "Balanced" → Click "Start Auto"
   - **Manual Mode**: Check "Enable Manual Control" → Move slider → Click "Apply"
5. **Monitor temperature** - keep below 85°C

### Uninstall
- Settings → Apps → GPU Fan Controller → Uninstall
- OR: Start Menu → GPU Fan Controller → Uninstall

---

## 👨‍💻 For Developers (Building from Source)

### Prerequisites

**Download and install .NET 6.0 SDK:**
- Visit: https://dotnet.microsoft.com/download/dotnet/6.0
- Download the SDK (not just runtime) for Windows x64
- Install and restart your computer if prompted

### Build and Run (3 Simple Steps)

#### Step 1: Build the Application
Double-click `build.bat` to compile the application.

#### Step 2: Run as Administrator
Right-click `run.bat` → Select **"Run as Administrator"**

#### Step 3: Use the Application
1. View your GPU information in the "GPU Status" section
2. **Auto Mode** (Recommended):
   - Check "Enable Auto Mode (Fan Curves)"
   - Select profile (Silent/Balanced/Performance/Aggressive)
   - Click "Start Auto"
3. **Manual Mode** (Advanced):
   - Check "Enable Manual Control"
   - Move the slider to adjust fan speed (30-100% recommended)
   - Click "Apply" to set the speed
4. Monitor temperature - keep it below 85°C

### Create Windows Installer

**Requires Inno Setup** (free): https://jrsoftware.org/isdl.php

1. Install Inno Setup
2. Double-click `build-installer.bat`
3. Find installer at: `installer_output\\GPUFanController_Setup_v2.x.x.exe`
4. Distribute the installer to users!

## Important Notes

⚠️ **SAFETY FIRST:**
- Never set fan speed below 30%
- Watch your GPU temperature (displayed in the app)
- If temp goes above 85°C, increase fan speed immediately
- When in doubt, click "Reset to Auto"

✅ **Must Run as Administrator:**
The application REQUIRES administrator privileges to access GPU hardware. If you don't run as admin, it won't detect your GPU.

## Troubleshooting

### "dotnet is not recognized"
- Install .NET 6.0 SDK from the link above
- Restart your command prompt/PowerShell

### "No compatible GPU detected"
- Make sure you run as Administrator
- Update your GPU drivers
- Check Device Manager to ensure GPU is working

### "Failed to set fan speed"
- Some GPUs don't support manual fan control
- Try using manufacturer software (MSI Afterburner, ASUS GPU Tweak, etc.)
- Laptop GPUs often have locked fan curves

## Support

For detailed information, see `README.md`
