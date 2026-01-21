# Update Checker - Hostinger Setup Guide

## What Was Changed

Your update checker now pulls updates from your **Hostinger website** instead of Google Drive.

### Changes Made:

1. ✅ **version.json** - Updated with platform-specific download URLs
2. ✅ **UpdateChecker.cs** - Added support for Windows/Linux specific downloads
3. ✅ **MainForm.cs** - Changed update URL to Hostinger

---

## How It Works Now

### Update Flow:

```
App Starts
    ↓
Wait 2 seconds
    ↓
Check: https://yourdomain.com/version.json
    ↓
Compare versions (Current: 2.3.1 vs Remote version)
    ↓
If newer version exists → Show update dialog
    ↓
User clicks "Yes"
    ↓
Opens correct download URL:
  - Windows: https://yourdomain.com/GPUFanController_Setup_v2.3.1.exe
  - Linux: https://yourdomain.com/gpu-fan-controller-2.3.1-linux-x64.tar.gz
```

---

## Setup Instructions

### Step 1: Replace "yourdomain.com" with Your Actual Domain

You need to update **TWO files**:

#### File 1: `version.json`

**Current placeholders:**
```json
"DownloadUrlWindows": "https://yourdomain.com/GPUFanController_Setup_v2.3.1.exe",
"DownloadUrlLinux": "https://yourdomain.com/gpu-fan-controller-2.3.1-linux-x64.tar.gz",
"DownloadUrl": "https://yourdomain.com/GPUFanController_Setup_v2.3.1.exe",
```

**Replace with your actual domain:**
```json
"DownloadUrlWindows": "https://mysite.com/GPUFanController_Setup_v2.3.1.exe",
"DownloadUrlLinux": "https://mysite.com/gpu-fan-controller-2.3.1-linux-x64.tar.gz",
"DownloadUrl": "https://mysite.com/GPUFanController_Setup_v2.3.1.exe",
```

#### File 2: `MainForm.cs` (Line 102)

**Current:**
```csharp
string updateUrl = "https://yourdomain.com/version.json";
```

**Replace with:**
```csharp
string updateUrl = "https://mysite.com/version.json";
```

---

### Step 2: Upload Files to Hostinger

Upload these files to your Hostinger `public_html` folder:

**Required Files:**
1. ✅ `version.json` (Updated with your domain)
2. ✅ `GPUFanController_Setup_v2.3.1.exe` (68.7 MB)
3. ✅ `gpu-fan-controller-2.3.1-linux-x64.tar.gz` (28.15 MB)

**Your Hostinger directory structure:**
```
public_html/
├── index.html
├── version.json ⭐ (NEW - for update checker)
├── GPUFanController_Setup_v2.3.1.exe
├── gpu-fan-controller-2.3.1-linux-x64.tar.gz
├── favicon files...
└── installation guides...
```

---

### Step 3: Rebuild Your Application

After updating the domain names:

```powershell
# Rebuild Windows version
dotnet build GPUFanController.csproj -c Release

# Rebuild Linux version
dotnet build GPUFanControllerConsole.csproj -c Release
```

---

### Step 4: Test Update Checker

#### Test with Current Version (2.3.1):
1. Run your app
2. Wait 2 seconds
3. No update notification should appear (version is same)

#### Test with Newer Version:
1. Edit `version.json` on Hostinger
2. Change `"Version": "2.3.1"` to `"Version": "2.3.2"`
3. Save and wait a few minutes for changes to propagate
4. Run your app
5. After 2 seconds, you should see update notification!

---

## Releasing a New Version

When you want to release version 2.4.0:

### Step 1: Update Version Number in Code
```csharp
// UpdateChecker.cs
public static readonly string CurrentVersion = "2.4.0";
```

### Step 2: Build New Installers
```powershell
# Build and create installers
.\build-installer.bat
```

### Step 3: Upload New Files to Hostinger
Upload to `public_html`:
- `GPUFanController_Setup_v2.4.0.exe`
- `gpu-fan-controller-2.4.0-linux-x64.tar.gz`

### Step 4: Update version.json on Hostinger
```json
{
  "Version": "2.4.0",
  "DownloadUrlWindows": "https://mysite.com/GPUFanController_Setup_v2.4.0.exe",
  "DownloadUrlLinux": "https://mysite.com/gpu-fan-controller-2.4.0-linux-x64.tar.gz",
  "DownloadUrl": "https://mysite.com/GPUFanController_Setup_v2.4.0.exe",
  "ReleaseNotes": "• NEW: Amazing new feature\n• FIXED: Bug fixes\n• IMPROVED: Better performance",
  "ReleaseDate": "2026-02-15",
  "IsCritical": false,
  "Analytics": {
    "MeasurementId": "G-NMNS09L9FJ",
    "ApiSecret": "0YpvAQeEQX-zZMCwc7qKpw"
  }
}
```

### Step 5: Users Get Auto-Update Notification
All users running version 2.3.1 (or older) will:
1. See update notification on app startup
2. Click "Yes" to download
3. Get taken to the correct installer for their platform

---

## Update Notification Details

### What Users See:

```
🔔 Update Available!

New Version: 2.4.0
Released: 2026-02-15

What's New:
• NEW: Amazing new feature
• FIXED: Bug fixes
• IMPROVED: Better performance

Would you like to download the update now?

[Yes]  [No]
```

### Platform-Specific Downloads:

- **Windows users** → Opens `GPUFanController_Setup_v2.4.0.exe` in browser
- **Linux users** → Opens `gpu-fan-controller-2.4.0-linux-x64.tar.gz` in browser

---

## Important Notes

### Version Comparison Logic:
- Uses semantic versioning (Major.Minor.Patch)
- `2.4.0` > `2.3.1` ✅ (shows update)
- `2.3.1` = `2.3.1` ❌ (no update)
- `2.3.0` < `2.3.1` ❌ (no update)

### Critical Updates:
Set `"IsCritical": true` in version.json for urgent updates:
```json
{
  "Version": "2.3.2",
  "IsCritical": true,
  ...
}
```
This shows ⚠️ warning icon in the update dialog.

### Analytics Auto-Update:
The version.json includes analytics config. If you change:
```json
"Analytics": {
  "MeasurementId": "G-NEWID",
  "ApiSecret": "new-secret"
}
```
All users will automatically use new analytics credentials on next app launch!

---

## Troubleshooting

### "No update notification appears"

**Check:**
1. Is `version.json` publicly accessible?
   - Test: Open `https://yourdomain.com/version.json` in browser
   - Should show JSON content
2. Is version number newer?
   - Current app: 2.3.1
   - version.json: Must be > 2.3.1
3. Check app logs for errors

### "Download link doesn't work"

**Check:**
1. Are installer files uploaded to Hostinger?
2. Are filenames exactly matching in version.json?
3. Are files in `public_html` root (not in subfolder)?
4. Is SSL enabled? Use `https://` not `http://`

### "Update checker is slow"

**Current timeout: 10 seconds**

To change timeout, edit `UpdateChecker.cs`:
```csharp
client.Timeout = TimeSpan.FromSeconds(5); // Faster timeout
```

---

## File Permissions

Make sure files are publicly accessible on Hostinger:

```
version.json         → 644 (rw-r--r--)
*.exe               → 644 (rw-r--r--)
*.tar.gz            → 644 (rw-r--r--)
```

---

## Example version.json Configurations

### Regular Update:
```json
{
  "Version": "2.4.0",
  "DownloadUrlWindows": "https://mysite.com/GPUFanController_Setup_v2.4.0.exe",
  "DownloadUrlLinux": "https://mysite.com/gpu-fan-controller-2.4.0-linux-x64.tar.gz",
  "DownloadUrl": "https://mysite.com/GPUFanController_Setup_v2.4.0.exe",
  "ReleaseNotes": "Minor improvements and bug fixes",
  "ReleaseDate": "2026-02-15",
  "IsCritical": false,
  "Analytics": {
    "MeasurementId": "G-NMNS09L9FJ",
    "ApiSecret": "0YpvAQeEQX-zZMCwc7qKpw"
  }
}
```

### Critical Security Update:
```json
{
  "Version": "2.3.2",
  "DownloadUrlWindows": "https://mysite.com/GPUFanController_Setup_v2.3.2.exe",
  "DownloadUrlLinux": "https://mysite.com/gpu-fan-controller-2.3.2-linux-x64.tar.gz",
  "DownloadUrl": "https://mysite.com/GPUFanController_Setup_v2.3.2.exe",
  "ReleaseNotes": "⚠️ SECURITY: Critical security patch\n• Fixed vulnerability in fan control\n• All users should update immediately",
  "ReleaseDate": "2026-01-21",
  "IsCritical": true,
  "Analytics": {
    "MeasurementId": "G-NMNS09L9FJ",
    "ApiSecret": "0YpvAQeEQX-zZMCwc7qKpw"
  }
}
```

---

## Benefits of Hostinger vs Google Drive

✅ **No rate limits** - Google Drive has download quotas
✅ **Faster downloads** - Direct file serving
✅ **Your control** - No dependency on Google Drive API
✅ **Professional** - Your own domain
✅ **Reliable** - Web hosting uptime
✅ **Simple** - Direct URL, no conversion needed

---

## Summary

Your update checker now works like this:

1. **App checks**: `https://yourdomain.com/version.json`
2. **Compares**: Remote version vs Current version (2.3.1)
3. **If newer**: Shows update dialog
4. **User clicks Yes**: Opens platform-specific installer URL
5. **Downloads**: Windows .exe or Linux .tar.gz

**Next steps:**
1. Replace "yourdomain.com" in version.json and MainForm.cs
2. Upload version.json to Hostinger
3. Rebuild your application
4. Test update checker
5. Done! ✅
