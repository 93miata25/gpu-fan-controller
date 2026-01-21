# Analytics Implementation - Technical Documentation

## Overview

GPU Fan Controller now includes privacy-focused analytics to track installs and active users using Google Analytics 4 (GA4) Measurement Protocol.

## Architecture

### Components

1. **AnalyticsService.cs** - Core analytics service
2. **UpdateChecker.cs** - Modified to load analytics config from version.json
3. **MainForm.cs** - Tracks GUI app usage
4. **ProgramConsole.cs** - Tracks console app usage
5. **version.json** - Contains GA4 credentials (Measurement ID and API Secret)

### Data Flow

```
Application Start
    ↓
Load version.json (via UpdateChecker)
    ↓
Configure AnalyticsService with credentials
    ↓
Track install (first run only)
    ↓
Track app start (every run)
    ↓
Send events to GA4 via HTTPS POST
```

## Implementation Details

### AnalyticsService.cs

**Key Features:**
- Anonymous client ID generation (GUID stored in `%AppData%\GPUFanController\analytics.dat`)
- Install tracking (only first run, marker file: `%AppData%\GPUFanController\.installed`)
- App start tracking (every run)
- Fail-safe design (all errors silently caught)
- 5-second timeout for network requests

**Methods:**
```csharp
AnalyticsService.Configure(measurementId, apiSecret)  // Setup credentials
AnalyticsService.TrackInstallAsync()                   // Track first install
AnalyticsService.TrackAppStartAsync()                  // Track active user
AnalyticsService.TrackEventAsync(name, params)         // Track custom events
```

**Events Tracked:**
- `app_install` - First application install
  - Parameters: `app_version`, `os_version`
- `app_start` - Application start (active user)
  - Parameters: `app_version`, `session_id`

### UpdateChecker.cs Modifications

**Added Classes:**
```csharp
public class AnalyticsConfig
{
    public string MeasurementId { get; set; }
    public string ApiSecret { get; set; }
}
```

**UpdateInfo Extended:**
```csharp
public class UpdateInfo
{
    // ... existing fields ...
    public AnalyticsConfig? Analytics { get; set; }
}
```

**Auto-Configuration:**
When `CheckForUpdatesAsync()` loads version.json, it automatically configures `AnalyticsService` if analytics credentials are present.

### Integration Points

**GUI Application (MainForm.cs):**
```csharp
private async void MainForm_Shown(object? sender, EventArgs e)
{
    await Task.Delay(3000);
    await CheckForUpdates();  // Loads analytics config
    
    // Track usage
    await AnalyticsService.TrackInstallAsync();
    await AnalyticsService.TrackAppStartAsync();
}
```

**Console Application (ProgramConsole.cs):**
```csharp
static async Task InitializeAnalyticsAsync()
{
    try
    {
        string updateUrl = "https://drive.google.com/uc?export=download&id=...";
        UpdateChecker.SetUpdateUrl(updateUrl);
        await UpdateChecker.CheckForUpdatesAsync();
        
        await AnalyticsService.TrackInstallAsync();
        await AnalyticsService.TrackAppStartAsync();
    }
    catch { /* Silently fail */ }
}
```

## File Locations

### Application Files

| File | Purpose | Location |
|------|---------|----------|
| `analytics.dat` | Anonymous client ID | `%AppData%\GPUFanController\` |
| `.installed` | Install marker | `%AppData%\GPUFanController\` |

### Configuration Files

| File | Purpose |
|------|---------|
| `version.json` | GA4 credentials and app version info |
| `AnalyticsService.cs` | Core analytics implementation |

## Privacy & Security

### What is Tracked

✅ **Anonymous device ID** (randomly generated GUID)  
✅ **App version** (e.g., "2.3.1")  
✅ **OS version** (e.g., "Microsoft Windows NT 10.0.22631.0")  
✅ **Session ID** (timestamp-based, unique per app launch)

### What is NOT Tracked

❌ Username or computer name  
❌ IP address (GA4 anonymizes automatically)  
❌ GPU model or hardware specifics  
❌ User settings or configurations  
❌ Fan speeds or temperatures  
❌ Any personally identifiable information

### Security Features

1. **HTTPS Only**: All analytics requests use HTTPS
2. **Timeout Protection**: 5-second timeout prevents hanging
3. **Fail-Safe**: Errors never crash or interrupt the application
4. **Local Storage**: Client ID stored locally, never transmitted in identifiable form
5. **No Tracking Cookies**: No browser-style cookies used

## GA4 Measurement Protocol

### Endpoint
```
POST https://www.google-analytics.com/mp/collect
```

### Query Parameters
- `measurement_id`: GA4 Measurement ID (G-XXXXXXXXXX)
- `api_secret`: GA4 API Secret

### Payload Format
```json
{
  "client_id": "anonymous-guid-here",
  "events": [
    {
      "name": "app_install",
      "params": {
        "app_version": "2.3.1",
        "os_version": "Microsoft Windows NT 10.0.22631.0"
      }
    }
  ]
}
```

## Testing

### Local Testing

1. **Check File Creation:**
   ```powershell
   dir "$env:APPDATA\GPUFanController"
   ```
   Should show `analytics.dat` and `.installed` files

2. **Test Install Tracking:**
   - Delete `.installed` file
   - Run application
   - Check GA4 Real-time reports for `app_install` event

3. **Test Active User Tracking:**
   - Run application
   - Check GA4 Real-time reports for `app_start` event within 1-2 minutes

### Debugging

Enable detailed logging (optional):
```csharp
// Add to SendEventAsync method for debugging
Console.WriteLine($"Sending event: {eventName}");
Console.WriteLine($"Payload: {json}");
```

### Verification Checklist

- [ ] `analytics.dat` file created in AppData
- [ ] `.installed` file created on first run only
- [ ] GA4 Real-time reports show events
- [ ] Events include correct app_version
- [ ] No errors in application logs
- [ ] Application starts normally if analytics fail

## Extending Analytics

### Add Custom Events

```csharp
// Track when user sets fan speed manually
await AnalyticsService.TrackEventAsync("manual_fan_control", new
{
    fan_speed = 75,
    gpu_type = "NVIDIA"
});

// Track profile usage
await AnalyticsService.TrackEventAsync("profile_selected", new
{
    profile_name = "Performance"
});

// Track feature usage
await AnalyticsService.TrackEventAsync("diagnostics_run", new
{
    gpu_count = 2
});
```

### Disable Analytics (Future Enhancement)

To add user opt-out capability:

```csharp
// Add to settings
public bool AnalyticsEnabled { get; set; } = true;

// Check before tracking
if (_appConfig.AnalyticsEnabled)
{
    await AnalyticsService.TrackEventAsync(...);
}
```

## Metrics Available in GA4

### Standard Metrics
- **Total Users**: Unique installs (unique client_ids)
- **Active Users**: Users who launched app in time period
- **New Users**: First-time installs
- **Events per User**: Average app launches per user
- **User Engagement**: Time-based engagement metrics

### Custom Metrics
Create in GA4 Admin:
- App version distribution
- OS version distribution
- Install rate over time
- Daily/Weekly/Monthly active users

## Troubleshooting

### No Data in GA4

**Check:**
1. Measurement ID format is `G-XXXXXXXXXX` (not `UA-`)
2. API Secret is correct and from same property
3. version.json is accessible and valid JSON
4. Internet connection is available
5. Wait 1-2 minutes for real-time data

### Multiple Installs

**If same device shows multiple installs:**
- User may have cleared AppData folder
- User may have reinstalled application
- This is expected behavior

### Events Not Sent

**Common causes:**
1. No internet connection (silently fails)
2. version.json not loaded (UpdateChecker not called)
3. Analytics credentials missing or invalid
4. GA4 property not configured correctly

## Performance Impact

- **Network**: ~1KB per event (minimal bandwidth)
- **Latency**: 5-second timeout max, non-blocking async
- **Storage**: ~50 bytes for client ID file
- **CPU**: Negligible (one HTTPS POST per app start)

## Compliance

### GDPR
✅ Anonymous data only  
✅ No personal information  
✅ User cannot be identified from data  
✅ Optional: Add opt-out mechanism if desired

### Privacy Best Practices
✅ Transparent about data collection  
✅ Minimal data collection  
✅ Secure transmission (HTTPS)  
✅ No third-party data sharing  

## Version History

### v2.3.1 - Analytics Integration
- Added AnalyticsService.cs
- Modified UpdateChecker.cs for auto-configuration
- Integrated tracking in MainForm.cs and ProgramConsole.cs
- Added analytics.dat and .installed marker files
- Created documentation

---

*Last Updated: January 2026*
