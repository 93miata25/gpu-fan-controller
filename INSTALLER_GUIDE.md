# GPU Fan Controller - Installer Creation Guide

## 🎁 What This Creates

A professional Windows installer that includes:
- ✅ Desktop shortcuts (optional)
- ✅ Start Menu folder with all apps
- ✅ Uninstaller in Control Panel
- ✅ Administrator privilege elevation
- ✅ Both GUI and Console versions
- ✅ All documentation files
- ✅ License agreement display

## 📋 Prerequisites

### Required:
1. **.NET 6.0 SDK** - https://dotnet.microsoft.com/download/dotnet/6.0
2. **Inno Setup** - https://jrsoftware.org/isdl.php (free, ~5 MB)

### Optional:
3. **Icon file** (app_icon.ico) - For professional appearance

---

## 🚀 Quick Start - Create Installer

### Step 1: Install Inno Setup
```
1. Download from: https://jrsoftware.org/isdl.php
2. Run the installer (InnoSetup-6.x.x.exe)
3. Follow installation wizard
4. Keep default settings
```

### Step 2: (Optional) Add Custom Icon
```
1. Create or find a GPU fan icon (PNG/JPG)
2. Convert to .ico format:
   - Online: https://www.icoconverter.com/
   - Sizes: 16x16, 32x32, 48x48, 256x256
3. Save as: app_icon.ico (replace placeholder file)
```

### Step 3: Build the Installer
```batch
# Simply double-click:
build-installer.bat

# Or run from command prompt:
build-installer.bat
```

### What Happens:
1. ✅ Builds GUI application (self-contained)
2. ✅ Builds Console application (self-contained)
3. ✅ Creates installer with Inno Setup
4. ✅ Output: `installer_output\GPUFanController_Setup_v2.0.exe`

---

## 📦 Installer Features

### What Gets Installed:
```
C:\Program Files\GPU Fan Controller\
├── GPUFanController.exe          (GUI version)
├── console\
│   └── GPUFanControllerConsole.exe (Console version)
├── README.md
├── FEATURES.md
├── QUICKSTART.md
└── PROJECT_SUMMARY.md
```

### Start Menu Folder:
```
Start > All Programs > GPU Fan Controller
├── GPU Fan Controller             (Runs GUI)
├── GPU Fan Controller Console     (Runs Console)
├── Quick Start Guide              (Opens QUICKSTART.md)
├── Documentation                  (Opens README.md)
└── Uninstall GPU Fan Controller   (Uninstaller)
```

### Optional Desktop Shortcuts:
- During installation, user can choose:
  - ☐ Create desktop icon for GUI version
  - ☐ Create desktop icon for Console version

### Administrator Privileges:
- Installer automatically requests admin rights
- Applications run with admin privileges (required for hardware access)
- UAC prompt appears when launching apps

---

## 🎨 Customizing the Installer

### Change App Version:
Edit `installer.iss`, line 6:
```pascal
#define MyAppVersion "2.0"  → Change to "2.1", "3.0", etc.
```

### Change Publisher Name:
Edit `installer.iss`, line 7:
```pascal
#define MyAppPublisher "GPU Fan Controller"  → Change to your name
```

### Change Install Directory:
Edit `installer.iss`, line 15:
```pascal
DefaultDirName={autopf}\GPU Fan Controller  → Change folder name
```

### Change Website URL:
Edit `installer.iss`, line 8:
```pascal
#define MyAppURL "https://github.com/yourusername/GPUFanController"
```

### Add/Remove Desktop Icons:
Edit `installer.iss`, line 30-31:
```pascal
; Change "unchecked" to "checked" to enable by default
Name: "desktopicon"; Description: "Create Desktop Icon"; Flags: checked
```

---

## 🔧 Manual Installer Creation

If you prefer to build manually:

### Option 1: Using Inno Setup IDE
```
1. Open Inno Setup
2. File > Open > installer.iss
3. Build > Compile
4. Installer created in: installer_output\
```

### Option 2: Command Line
```batch
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer.iss
```

---

## 📤 Distributing the Installer

### File to Distribute:
```
installer_output\GPUFanController_Setup_v2.0.exe
```

### File Size:
- Approximately **50-80 MB** (includes all dependencies)
- Self-contained (no .NET installation needed)

### Distribution Methods:
1. **Direct Download** - Host on your website or file sharing
2. **GitHub Releases** - Upload to GitHub Releases section
3. **USB Drive** - Copy to USB for offline installation
4. **Network Share** - Place on company network drive

### What Users Need:
- ✅ Windows 10/11 (64-bit)
- ✅ Administrator privileges
- ❌ NO .NET installation required (self-contained)
- ❌ NO additional dependencies

---

## 👤 User Installation Experience

### Installation Steps (User Perspective):
```
1. Double-click: GPUFanController_Setup_v2.0.exe
2. UAC prompt: "Do you want to allow changes?" → Click Yes
3. Welcome screen with app information → Click Next
4. License agreement (scroll and read) → Click I Agree
5. Select destination folder (default: C:\Program Files\...) → Click Next
6. Select Start Menu folder → Click Next
7. Select additional tasks:
   ☐ Create a desktop icon
   ☐ Create a desktop icon for Console version
8. Ready to install confirmation → Click Install
9. Installation progress bar (10-30 seconds)
10. Completion screen with option to launch → Click Finish
```

### After Installation:
- Find in Start Menu: "GPU Fan Controller"
- Desktop icons (if selected)
- Can uninstall via Control Panel > Programs

---

## 🗑️ Uninstallation

### Methods to Uninstall:
1. **Control Panel**:
   - Settings > Apps > GPU Fan Controller > Uninstall
   
2. **Start Menu**:
   - Start > GPU Fan Controller > Uninstall GPU Fan Controller

3. **Programs Folder**:
   - C:\Program Files\GPU Fan Controller\unins000.exe

### What Gets Removed:
- ✅ All application files
- ✅ All shortcuts (desktop, Start Menu)
- ✅ Installation directory
- ✅ Registry entries (if any)

### What Stays:
- ❌ Nothing! Clean uninstall

---

## 🔍 Troubleshooting

### "Inno Setup not found"
**Solution:**
1. Download from: https://jrsoftware.org/isdl.php
2. Install Inno Setup
3. Run `build-installer.bat` again

### "Build failed" error
**Solution:**
1. Ensure .NET 6.0 SDK is installed
2. Run `dotnet --version` to verify
3. Try running `dotnet clean` then rebuild

### "Cannot find app_icon.ico"
**Solution:**
- This is just a warning
- Installer will build without icon
- Add real .ico file for professional appearance

### Installer won't run on user's PC
**Possible causes:**
1. **Windows 7/8**: Not supported (requires Windows 10+)
2. **32-bit Windows**: Not supported (64-bit only)
3. **Antivirus blocking**: Whitelist the installer
4. **Missing permissions**: Run as Administrator

---

## 📊 Installer Script Details

### What `installer.iss` Contains:

**[Setup] Section:**
- App metadata (name, version, publisher)
- Installation defaults (directory, Start Menu)
- Compression settings (LZMA)
- Administrator privileges requirement
- Wizard style (modern)

**[Files] Section:**
- Which files to include in installer
- Where to copy them on target system
- Exclusions (.pdb debug files)

**[Icons] Section:**
- Start Menu shortcuts
- Desktop shortcuts (optional)
- Quick Launch icons (Windows 7)
- Uninstaller shortcut

**[Run] Section:**
- Option to launch after installation

**[Code] Section:**
- Custom Pascal scripts for:
  - Welcome message
  - Uninstall confirmation
  - Post-install/uninstall actions

---

## 🎯 Advanced Customization

### Add Custom Welcome Message:
Edit the `InitializeSetup()` function in `installer.iss`

### Add Custom Uninstall Confirmation:
Edit the `InitializeUninstall()` function

### Include Additional Files:
Add to `[Files]` section:
```pascal
Source: "MyFile.txt"; DestDir: "{app}"; Flags: ignoreversion
```

### Create Additional Shortcuts:
Add to `[Icons]` section:
```pascal
Name: "{group}\My Shortcut"; Filename: "{app}\MyApp.exe"
```

---

## ✅ Testing Checklist

Before distributing installer:

- [ ] Build completes without errors
- [ ] Installer file created in `installer_output\`
- [ ] Install on test machine
- [ ] Desktop shortcuts work (if selected)
- [ ] Start Menu shortcuts work
- [ ] Applications launch with admin privileges
- [ ] Applications detect GPU correctly
- [ ] Documentation files accessible
- [ ] Uninstaller works correctly
- [ ] Uninstaller removes all files
- [ ] No leftover files after uninstall

---

## 🎉 You're Ready!

Run `build-installer.bat` and distribute your professional installer!

**Questions?** Check the documentation files or visit the project repository.
