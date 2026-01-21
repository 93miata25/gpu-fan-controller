# Analytics Heartbeat Implementation Guide

## 🎯 Overview

The GPU Fan Controller now includes **active user tracking** through periodic heartbeat events sent to Google Analytics 4. This allows you to track live users even when the application is minimized to the system tray.

## ✨ What's New

### Heartbeat Tracking
- **Event Name**: `user_engagement`
- **Frequency**: Every 5 minutes
- **Works When**: Application is running (even minimized to tray)
- **Stops When**: Application is closed completely

### Key Features
1. ✅ Tracks active users in real-time
2. ✅ Works even when app is in system tray
3. ✅ Sends engagement time (5 minutes = 300,000 milliseconds)
4. ✅ Includes session ID for user journey tracking
5. ✅ Privacy-friendly (anonymous tracking)

## 🔧 Technical Implementation

### Changes Made

#### 1. AnalyticsService.cs
Added three new components:

```csharp
// Session tracking
private static string? _sessionId;
private static DateTime _lastHeartbeatTime = DateTime.MinValue;
private static readonly TimeSpan _heartbeatInterval = TimeSpan.FromMinutes(5);

// New method: TrackHeartbeatAsync()
public static async Task TrackHeartbeatAsync()
{
    // Sends user_engagement event every 5 minutes
    // Includes: app_version, session_id, engagement_time_msec
}
```

#### 2. MainForm.cs
Modified the update timer to send heartbeats:

```csharp
private void UpdateTimer_Tick(object? sender, EventArgs e)
{
    // ... existing GPU monitoring code ...
    
    // Send analytics heartbeat every 5 minutes (even when minimized to tray)
    Task.Run(async () => await AnalyticsService.TrackHeartbeatAsync());
}
```

## 📊 Google Analytics Dashboard

### Viewing Active Users

#### Real-Time Reports
1. Go to **Reports → Realtime → Overview**
2. You'll see "Active users right now" with live count
3. Events will show `user_engagement` appearing every 5 minutes per user

#### Engagement Reports
1. Go to **Reports → Engagement → Events**
2. Look for these events:
   - `app_start`: When user opens the app
   - `user_engagement`: Every 5 minutes while app is running
   - `app_install`: First time app is installed

### Key Metrics You Can Track

| Metric | Description |
|--------|-------------|
| **Active Users (Real-time)** | Users with app open right now |
| **Active Users (1-day)** | Users who ran app in last 24 hours |
| **Active Users (7-day)** | Weekly active users |
| **Active Users (30-day)** | Monthly active users |
| **Average Engagement Time** | How long users keep app running |
| **Sessions per User** | How often users open the app |

### Example Analytics Query

**Total Active Time per User:**
```
Event: user_engagement
Metric: engagement_time_msec (sum)
Dimension: client_id
```

**Daily Active Users:**
```
Event: user_engagement
Metric: Unique users
Dimension: Date
```

## 🧪 Testing the Implementation

### Manual Testing Steps

1. **Build and Run the Application**
   ```powershell
   dotnet build GPUFanController.csproj -c Release
   ```

2. **Monitor Console Output** (if using debug build)
   - You should see heartbeat events being sent every 5 minutes

3. **Check Google Analytics Real-Time**
   - Open GA4 Dashboard → Realtime
   - Run the application
   - You should see `app_start` event immediately
   - Wait 5 minutes → `user_engagement` event appears
   - Wait another 5 minutes → another `user_engagement` event

4. **Test Tray Icon Behavior**
   - Minimize app to system tray
   - Wait 5 minutes
   - Check GA4 → heartbeat should still be sent
   - This proves tracking works even when minimized!

### Expected Event Timeline

```
00:00 → app_start (session begins)
05:00 → user_engagement (first heartbeat)
10:00 → user_engagement (second heartbeat)
15:00 → user_engagement (third heartbeat)
...and so on every 5 minutes
```

## 🔐 Privacy & Performance

### Privacy Features
- ✅ **Anonymous Tracking**: Only tracks anonymous device ID
- ✅ **No Personal Data**: No usernames, emails, or IP addresses stored
- ✅ **Session-Based**: Each app session gets unique ID
- ✅ **Opt-Out Ready**: Easy to disable via `AnalyticsService.SetEnabled(false)`

### Performance Impact
- ✅ **Minimal Network Usage**: Small HTTP POST every 5 minutes (~500 bytes)
- ✅ **Non-Blocking**: Runs asynchronously, doesn't affect UI
- ✅ **Fail-Safe**: Errors are silently caught, never crash the app
- ✅ **Throttled**: Only sends once per 5 minutes (prevents spam)

## 📈 Use Cases

### Why Track Active Users?

1. **Usage Patterns**: Understand when users run your app
2. **Engagement Metrics**: See how long users keep app running
3. **Crash Detection**: If heartbeats stop suddenly, users may have crashed
4. **Feature Planning**: High engagement = users find value in your app
5. **Updates Impact**: Compare engagement before/after updates

### Business Insights

```
Example Analytics Results:
- 150 app_start events/day → 150 daily app opens
- 450 user_engagement events/day → Average 30 minutes per session
- 50 unique users/day → 50 daily active users
- 300 unique users/month → 300 monthly active users
```

## 🛠️ Customization Options

### Change Heartbeat Interval

In `AnalyticsService.cs`:
```csharp
// Change from 5 minutes to 10 minutes
private static readonly TimeSpan _heartbeatInterval = TimeSpan.FromMinutes(10);

// Or 1 minute for more frequent tracking
private static readonly TimeSpan _heartbeatInterval = TimeSpan.FromMinutes(1);
```

**Recommended**: 5-10 minutes (balances accuracy with network usage)

### Add Custom Parameters

You can add more data to heartbeat events:

```csharp
await SendEventAsync("user_engagement", new
{
    app_version = UpdateChecker.CurrentVersion,
    session_id = _sessionId ?? GenerateSessionId(),
    engagement_time_msec = (int)_heartbeatInterval.TotalMilliseconds,
    // Add custom data:
    is_minimized = form.WindowState == FormWindowState.Minimized,
    gpu_temperature = gpuController.GetTemperature(),
    fan_speed = gpuController.GetFanSpeed()
});
```

### Disable Tracking

Users can opt-out by adding this to your settings:

```csharp
// In MainForm.cs or settings UI
AnalyticsService.SetEnabled(false);
```

## 🐛 Troubleshooting

### Heartbeats Not Appearing in GA4

**Problem**: `app_start` works but no `user_engagement` events

**Solutions**:
1. ✅ Wait at least 5 minutes after app starts
2. ✅ Check Real-Time reports (not historical)
3. ✅ Ensure app is still running (not closed)
4. ✅ Check if ad blocker is blocking GA requests

### Too Many/Too Few Events

**Problem**: Events sent more/less frequently than 5 minutes

**Solution**: The `_lastHeartbeatTime` check ensures only one event per 5 minutes. If you see issues:
```csharp
// Add debug logging
Console.WriteLine($"Last heartbeat: {_lastHeartbeatTime}");
Console.WriteLine($"Time since: {DateTime.UtcNow - _lastHeartbeatTime}");
```

### Events Stop When Minimized

**Problem**: No heartbeats when app is in tray

**Solution**: This implementation specifically handles this! The `UpdateTimer_Tick` runs even when minimized. If it's not working:
- Ensure timer is not stopped when minimizing
- Check that `_updateTimer.Start()` is called
- Verify timer interval (currently 1000ms)

## 📚 Related Documentation

- `ANALYTICS_GUIDE.md` - General analytics setup
- `ANALYTICS_IMPLEMENTATION.md` - Technical details
- `AnalyticsService.cs` - Source code

## 🎉 Summary

Your application now tracks:
- ✅ **App Installs**: First-time installations
- ✅ **App Starts**: Every time app is opened
- ✅ **Active Users**: Real-time user count (NEW!)
- ✅ **Engagement Time**: How long users run the app (NEW!)
- ✅ **Tray Activity**: Tracking even when minimized (NEW!)

### Before vs After

**Before:**
```
GA4 only knew when app started, not if user kept it running
```

**After:**
```
GA4 tracks continuous usage with 5-minute heartbeats
Can see: "15 active users right now" in real-time
Can measure: Average session length, daily active users, etc.
```

---

**Version**: 2.3.1  
**Last Updated**: 2026-01-20  
**Feature**: Active User Heartbeat Tracking  
**Status**: ✅ Implemented and Ready
