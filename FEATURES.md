# GPU Fan Controller - Feature List

## 🎮 Multi-GPU Support

### Features
- **Detect Multiple GPUs**: Automatically detects all NVIDIA, AMD, and Intel GPUs in your system
- **Individual Control**: Control each GPU independently with separate fan curves or manual settings
- **GPU Selector**: Dropdown menu to switch between GPUs in the GUI version
- **Unified Monitoring**: View status of all GPUs simultaneously in console mode

### How to Use
- **GUI**: Select GPU from the dropdown at the top of the window
- **Console**: Menu option [5] shows all GPU details, other operations prompt for GPU selection

---

## 🤖 Auto Fan Control (Fan Curves)

### Features
- **Temperature-Based Control**: Automatically adjusts fan speed based on GPU temperature
- **4 Built-in Profiles**:
  - **Silent**: Prioritizes quiet operation (30-100% fan speed)
  - **Balanced**: Good balance between noise and cooling (35-100% fan speed)
  - **Performance**: Aggressive cooling for gaming/workloads (40-100% fan speed)
  - **Aggressive**: Maximum cooling, higher noise (50-100% fan speed)
- **Real-time Adjustment**: Updates every 2 seconds based on temperature changes
- **Hysteresis**: Prevents rapid fan speed changes for stable operation

### Fan Curve Details

#### Silent Profile
- 0°C → 30% | 50°C → 35% | 60°C → 40% | 70°C → 50%
- 75°C → 60% | 80°C → 75% | 85°C → 90% | 90°C → 100%

#### Balanced Profile
- 0°C → 35% | 50°C → 40% | 60°C → 50% | 70°C → 65%
- 75°C → 75% | 80°C → 85% | 85°C → 95% | 90°C → 100%

#### Performance Profile
- 0°C → 40% | 50°C → 50% | 60°C → 60% | 70°C → 75%
- 75°C → 85% | 80°C → 95% | 85°C → 100% | 90°C → 100%

#### Aggressive Profile
- 0°C → 50% | 50°C → 60% | 60°C → 70% | 70°C → 85%
- 75°C → 95% | 80°C → 100% | 85°C → 100% | 90°C → 100%

### How to Use
- **GUI**: Check "Enable Auto Mode", select profile, click "Start Auto"
- **Console**: Menu option [3] → Select GPU → Select profile → Monitor in real-time

---

## 🎛️ Manual Fan Control

### Features
- **Slider Control (GUI)**: Smooth slider from 0-100%
- **Direct Input (Console)**: Type exact percentage value
- **Safety Warnings**: Alerts for fan speeds below 30%
- **Real-time Feedback**: See changes immediately in RPM and temperature
- **Per-GPU Control**: Set different speeds for different GPUs

### Safety Features
- Warning dialog for speeds below 30%
- Temperature color coding (Green < 60°C, Yellow < 75°C, Orange < 85°C, Red ≥ 85°C)
- Automatic reset to driver defaults on exit
- Confirmation required for dangerous settings

### How to Use
- **GUI**: Check "Enable Manual Control", move slider, click "Apply"
- **Console**: Menu option [2] → Select GPU → Enter fan speed

---

## 📊 Real-time Monitoring

### Monitored Parameters
- **GPU Temperature**: Core temperature in Celsius
- **Fan Speed**: Current fan control percentage
- **Fan RPM**: Actual fan rotation speed
- **GPU Name**: Full GPU model name
- **GPU Type**: NVIDIA, AMD, or Intel

### Display Features
- **Auto-refresh**: Updates every 1-2 seconds
- **Color Coding**: Temperature warnings with color indicators
- **Status Display**: Shows active profile and target fan speed
- **Multi-GPU View**: See all GPUs at once (console mode)

### How to Use
- **GUI**: Always displayed in the "GPU Status" section
- **Console**: Menu option [1] for continuous monitoring (Press ESC to exit)

---

## 🖥️ Two Application Versions

### GUI Version (Windows Forms)
**Best for**: Users who prefer visual interface and want to leave it running in the background

**Features**:
- Visual slider control
- Always-on monitoring display
- Easy GPU switching
- Profile selection with dropdown
- Single window interface

**Launch**: `run.bat` (or double-click executable)

### Console Version
**Best for**: Power users, remote access, scripting, or minimal resource usage

**Features**:
- Interactive menu system
- Real-time monitoring mode
- Color-coded temperature display
- All GPUs visible simultaneously
- Lower memory footprint

**Launch**: `run-console.bat` (or run executable)

---

## 🛡️ Safety Features

### Hardware Protection
1. **Administrator Requirement**: Prevents unauthorized hardware access
2. **Auto-reset on Exit**: Returns to driver defaults when application closes
3. **Warning System**: Multiple warnings for dangerous settings
4. **Temperature Monitoring**: Continuous temperature tracking with color alerts
5. **Hysteresis in Auto Mode**: Prevents rapid fan speed oscillations

### User Warnings
- Pop-up dialogs for fan speeds below 30%
- Red temperature display above 85°C
- Confirmation required for dangerous operations
- Clear safety messages throughout interface

---

## 🔄 Reset to Auto

### Features
- **One-Click Reset**: Return control to GPU drivers
- **All GPUs or Individual**: Reset all GPUs or just selected one
- **Stops Active Profiles**: Disables any running auto-control
- **Driver Defaults**: Returns to manufacturer fan curve

### How to Use
- **GUI**: Click "Reset to Auto" button
- **Console**: Menu option [4] resets all GPUs

---

## 💾 Technical Details

### Architecture
- **LibreHardwareMonitor**: Hardware access library
- **.NET 6.0**: Modern, cross-platform framework
- **Windows Forms**: GUI framework
- **Console App**: Terminal-based interface

### Compatibility
- **GPUs**: NVIDIA, AMD, Intel with fan control support
- **OS**: Windows 10/11 (64-bit)
- **Requirements**: Administrator privileges, .NET 6.0 Runtime

### Performance
- **Memory**: ~50-100 MB (GUI), ~20-40 MB (Console)
- **CPU**: <1% usage during normal operation
- **Update Rate**: 1-2 seconds for temperature/fan readings

---

## 📋 Feature Comparison

| Feature | GUI Version | Console Version |
|---------|-------------|-----------------|
| Multi-GPU Support | ✅ | ✅ |
| Auto Fan Curves | ✅ | ✅ |
| Manual Control | ✅ | ✅ |
| Real-time Monitoring | ✅ | ✅ |
| Visual Slider | ✅ | ❌ |
| All-GPU View | ❌ | ✅ |
| Menu System | ❌ | ✅ |
| Background Running | ✅ | ⚠️ Limited |
| Resource Usage | Higher | Lower |

---

## 🎯 Use Cases

### Gaming
- **Recommended**: Performance or Aggressive profile in auto mode
- **Why**: Keeps GPU cool during intense gaming sessions
- **Alternative**: Manual control at 70-80% during gameplay

### Video Editing / 3D Rendering
- **Recommended**: Performance profile in auto mode
- **Why**: Sustained high GPU usage needs consistent cooling
- **Alternative**: Monitor temperature and adjust manually

### Daily Use / Web Browsing
- **Recommended**: Silent or Balanced profile in auto mode
- **Why**: Minimal noise while maintaining safe temperatures
- **Alternative**: Manual control at 40-50%

### Overnight Rendering / Mining
- **Recommended**: Performance or Aggressive profile
- **Why**: Long-term high load requires aggressive cooling
- **Monitor**: Check temperatures regularly

### Testing / Benchmarking
- **Recommended**: Console version with real-time monitoring
- **Why**: See immediate impact of fan speed on temperature
- **Alternative**: GUI with manual control for fine-tuning

---

## 🔮 Future Enhancement Ideas

While not implemented, potential features could include:
- Custom fan curve creation
- Temperature logging and graphs
- Per-application profiles
- System tray integration
- Remote control via web interface
- Multi-monitor support for GPU selection
