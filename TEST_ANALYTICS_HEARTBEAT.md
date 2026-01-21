# Analytics Heartbeat Testing Guide

## ✅ Build Status
**Status**: ✅ Compiled Successfully  
**Warnings**: 40 (non-critical, nullable reference warnings only)  
**Errors**: 0

## 🧪 How to Test the Heartbeat Feature

### Method 1: Run the Application and Monitor GA4

1. **Start the Application**
   ```powershell
   .\bin\Release\net6.0-windows\GPUFanController.exe
   ```

2. **Open Google Analytics 4 Dashboard**
   - Go to: https://analytics.google.com/
   - Navigate to: **Reports → Realtime → Overview**

3. **Monitor Events Timeline**
   ```
   Time 00:00 → You should see: "app_start" event
   Time 05:00 → You should see: "user_engagement" event (1st heartbeat)
   Time 10:00 → You should see: "user_engagement" event (2nd heartbeat)
   Time 15:00 → You should see: "user_engagement" event (3rd heartbeat)
   ```

4. **Test Tray Icon Behavior**
   - Minimize the app to system tray (click X or minimize)
   - Wait 5 minutes
   - Check GA4 → "user_engagement" should still appear!
   - **This proves tracking works even when minimized** ✅

### Method 2: Debug Mode with Console Output

For more detailed testing, you can add temporary debug output:

1. **Add Debug Logging** (optional, for verification)
   
   In `AnalyticsService.cs`, modify `TrackHeartbeatAsync()`:
   ```csharp
   public static async Task TrackHeartbeatAsync()
   {
       if (!_isEnabled || string.IsNullOrEmpty(_measurementId)) return;

       try
       {
           if (DateTime.UtcNow - _lastHeartbeatTime < _heartbeatInterval)
           {
               System.Diagnostics.Debug.WriteLine($"[Analytics] Heartbeat skipped. Next in {(_heartbeatInterval - (DateTime.UtcNow - _lastHeartbeatTime)).TotalMinutes:F1} minutes");
               return;
           }

           _lastHeartbeatTime = DateTime.UtcNow;
           System.Diagnostics.Debug.WriteLine($"[Analytics] Sending heartbeat at {DateTime.UtcNow}");

           await SendEventAsync("user_engagement", new
           {
               app_version = UpdateChecker.CurrentVersion,
               session_id = _sessionId ?? GenerateSessionId(),
               engagement_time_msec = (int)_heartbeatInterval.TotalMilliseconds
           });
           
           System.Diagnostics.Debug.WriteLine($"[Analytics] Heartbeat sent successfully");
       }
       catch (Exception ex)
       {
           System.Diagnostics.Debug.WriteLine($"[Analytics] Heartbeat error: {ex.Message}");
       }
   }
   ```

2. **Run in Debug Mode**
   ```powershell
   dotnet run --project GPUFanController.csproj
   ```

3. **Watch Output Window** (in Visual Studio or console)
   - You'll see debug messages showing when heartbeats are sent

### Method 3: Network Traffic Monitoring

1. **Open Browser DevTools** (if using embedded analytics viewer)
   - Press F12
   - Go to Network tab
   - Filter by "google-analytics.com"

2. **Watch for POST Requests**
   - Every 5 minutes you should see POST to:
     ```
     https://www.google-analytics.com/mp/collect?measurement_id=G-NMNS09L9FJ&api_secret=...
     ```

3. **Inspect Payload**
   - Click on the request
   - Go to Payload tab
   - You should see:
     ```json
     {
       "client_id": "...",
       "events": [{
         "name": "user_engagement",
         "params": {
           "app_version": "2.3.1",
           "session_id": "...",
           "engagement_time_msec": 300000
         }
       }]
     }
     ```

## 📊 Expected Results in GA4

### Real-Time View (Within 30 seconds)
- **Active Users**: Should show your active session
- **Events by Event name**: 
  - `app_start`: 1 event when you launch
  - `user_engagement`: New event every 5 minutes

### Events Report (Next day)
- Go to: **Reports → Engagement → Events**
- You should see:
  ```
  Event Name          Event Count    Total Users
  user_engagement     12             1          (if you ran app for 1 hour)
  app_start           1              1
  app_install         1              1          (first time only)
  ```

### User Engagement Report
- Go to: **Reports → User → User Engagement**
- You should see:
  ```
  Average Engagement Time: ~X minutes (depends on how long you ran it)
  Engaged Sessions: 1
  ```

## ✅ Success Criteria

Your implementation is working correctly if:

- ✅ `app_start` event appears immediately when app launches
- ✅ `user_engagement` events appear every 5 minutes
- ✅ Heartbeats continue even when app is minimized to tray
- ✅ No crashes or errors in the application
- ✅ Events stop appearing after you close the app completely
- ✅ Real-time user count shows you as active

## 🐛 Troubleshooting

### Issue: No events appearing in GA4

**Check:**
1. ✅ Measurement ID is correct: `G-NMNS09L9FJ`
2. ✅ API Secret is correct: `0YpvAQeEQX-zZMCwc7qKpw`
3. ✅ Internet connection is active
4. ✅ Firewall isn't blocking Google Analytics
5. ✅ Using Real-Time reports (not historical)

### Issue: Events appear but then stop

**Possible causes:**
1. App crashed or closed
2. System went to sleep
3. Network connection lost

**Solution:** This is expected behavior - heartbeats only sent while app is running

### Issue: Heartbeats sent more frequently than 5 minutes

**Check:**
- The `_heartbeatInterval` setting in `AnalyticsService.cs`
- The throttling logic should prevent this
- Check if timer interval was modified

### Issue: No heartbeats when minimized

**This should NOT happen** - if it does:
1. Verify `UpdateTimer_Tick` is still running when minimized
2. Check that timer wasn't stopped in `MainForm_Resize`
3. Ensure `Task.Run(async () => await AnalyticsService.TrackHeartbeatAsync())` is being called

## 📈 Sample Test Session

Here's what a 15-minute test session should look like:

```
Time      Event                   GA4 Real-Time
------------------------------------------------------
00:00     App launched            "app_start" appears
00:05     First heartbeat         "user_engagement" #1
00:07     Minimized to tray       (no event)
00:10     Heartbeat while min.    "user_engagement" #2 ✅
00:12     Restored window         (no event)
00:15     Heartbeat continues     "user_engagement" #3 ✅
00:15     App closed              Events stop
```

## 🎯 Quick Verification Steps

**30-Second Test:**
1. Run the app
2. Check GA4 Real-Time → See "app_start" ✅
3. Done!

**5-Minute Test:**
1. Run the app
2. Wait 5 minutes (browse the app, change settings)
3. Check GA4 Real-Time → See "user_engagement" ✅
4. Done!

**10-Minute Test (Recommended):**
1. Run the app
2. Wait 5 minutes → Check GA4 → See heartbeat #1 ✅
3. Minimize to tray
4. Wait 5 more minutes → Check GA4 → See heartbeat #2 ✅
5. Close app completely
6. Wait 5 minutes → No more heartbeats ✅
7. Done!

## 📋 Test Checklist

Use this checklist to verify everything works:

- [ ] Build succeeds without errors
- [ ] App launches successfully
- [ ] `app_start` event appears in GA4 Real-Time
- [ ] Wait 5 minutes
- [ ] `user_engagement` event #1 appears
- [ ] Minimize app to tray
- [ ] Wait 5 minutes
- [ ] `user_engagement` event #2 appears (while minimized) ✅
- [ ] Restore window
- [ ] Wait 5 minutes
- [ ] `user_engagement` event #3 appears
- [ ] Close app
- [ ] Confirm no more events after closing

## 🎉 Expected Outcome

After successful implementation, you will be able to:

1. **See Active Users in Real-Time**
   - "15 active users right now"
   - Updated continuously as users run your app

2. **Track Engagement Metrics**
   - Average session length
   - Daily active users
   - Monthly active users

3. **Monitor Tray Usage**
   - Users who keep app running in background
   - Long-running sessions

4. **Identify Usage Patterns**
   - Peak usage times
   - Session duration trends
   - User retention

---

**Testing Status**: ✅ Ready to Test  
**Build Status**: ✅ Compiled Successfully  
**Feature Status**: ✅ Implemented  
**Documentation**: ✅ Complete
