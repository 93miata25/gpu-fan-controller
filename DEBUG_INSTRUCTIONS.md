# Analytics Debug Instructions

## 🔍 Debug Logging Added

I've added a debug log file that will help us see exactly what's happening with the heartbeat.

## 📝 Log File Location

The debug log is saved here:
```
%APPDATA%\GPUFanController\analytics_debug.log
```

Full path:
```
C:\Users\93mia\AppData\Roaming\GPUFanController\analytics_debug.log
```

## 🧪 Testing Steps

### Step 1: Close Current App
1. Close all running GPUFanController instances
2. Right-click on the tray icon → Exit

### Step 2: Clear Old Log (Optional)
```powershell
Remove-Item "$env:APPDATA\GPUFanController\analytics_debug.log" -ErrorAction SilentlyContinue
```

### Step 3: Start New Version
```powershell
.\bin\Release\net6.0-windows\GPUFanController.exe
```

### Step 4: Monitor the Log File
Wait 5-6 minutes, then check the log:
```powershell
Get-Content "$env:APPDATA\GPUFanController\analytics_debug.log"
```

Or monitor it in real-time:
```powershell
Get-Content "$env:APPDATA\GPUFanController\analytics_debug.log" -Wait
```

## 📊 What to Look For

### Expected Log Entries:

**Successful Heartbeat:**
```
[16:XX:XX] Sending heartbeat at 16:XX:XX
[16:XX:XX] Heartbeat sent successfully at 16:XX:XX
```

**Heartbeat Disabled:**
```
[16:XX:XX] Heartbeat skipped - Enabled: False, MeasurementId: True
```

**Configuration Missing:**
```
[16:XX:XX] Heartbeat skipped - Enabled: True, MeasurementId: False
```

**Error:**
```
[16:XX:XX] Heartbeat error: [error message]
```

## 🎯 Quick Test Command

Run this to test everything:
```powershell
# Clear log
Remove-Item "$env:APPDATA\GPUFanController\analytics_debug.log" -ErrorAction SilentlyContinue

# Launch app
Start-Process ".\bin\Release\net6.0-windows\GPUFanController.exe"

# Wait and monitor
Write-Host "Waiting 6 minutes for heartbeat..." -ForegroundColor Yellow
Start-Sleep -Seconds 360

# Check log
Write-Host "`nLog Contents:" -ForegroundColor Cyan
Get-Content "$env:APPDATA\GPUFanController\analytics_debug.log"
```

## 📍 Log File Path
Once you run the app, the log will be at:
```
C:\Users\93mia\AppData\Roaming\GPUFanController\analytics_debug.log
```
