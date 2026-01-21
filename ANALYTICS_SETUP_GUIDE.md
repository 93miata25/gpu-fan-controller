# Analytics Setup Guide

This guide explains how to set up Google Analytics 4 (GA4) to track downloads and active users for GPU Fan Controller.

## Overview

The GPU Fan Controller now includes built-in analytics tracking to help you understand:
- **Total Installs**: How many times the application has been installed
- **Active Users**: How many users are actively using the application
- **Version Distribution**: Which versions are being used

### Privacy-First Approach
- ✅ **Anonymous tracking only** - Uses randomly generated device IDs
- ✅ **No personal information** - No usernames, emails, or identifying data collected
- ✅ **Minimal data collection** - Only tracks: install event, app start event, app version, OS version
- ✅ **Silent operation** - Analytics never interrupt the user experience
- ✅ **Fail-safe** - If analytics fail, the app continues working normally

---

## Step 1: Create a Google Analytics 4 Property

1. **Go to Google Analytics**: https://analytics.google.com
2. **Sign in** with your Google account
3. **Create an Account** (if you don't have one):
   - Click "Start measuring"
   - Enter account name: "GPU Fan Controller"
   - Configure account settings

4. **Create a Property**:
   - Property name: "GPU Fan Controller"
   - Reporting time zone: Your timezone
   - Currency: Your currency
   - Click "Next"

5. **Business Details**:
   - Industry: "Computers & Electronics"
   - Business size: Select appropriate size
   - Usage: "Measure app activity"
   - Click "Create"

6. **Accept Terms of Service**

---

## Step 2: Set Up Data Stream

1. **Choose Platform**: Select "Web" (GA4 can track desktop apps via Measurement Protocol)
2. **Stream Details**:
   - Website URL: `https://github.com/yourusername/gpu-fan-controller` (or your project URL)
   - Stream name: "Desktop App"
   - Click "Create stream"

3. **Find Your Measurement ID**:
   - You'll see a **Measurement ID** like `G-XXXXXXXXXX`
   - **Save this ID** - you'll need it later

---

## Step 3: Generate API Secret

1. In the **Data Stream details** page, scroll down to "Measurement Protocol API secrets"
2. Click **"Create"**
3. **Secret nickname**: "Desktop App Secret"
4. Click **"Create"**
5. **Copy the API secret** - You'll need this (you can only see it once!)

---

## Step 4: Configure Your Application

### Update version.json

Open your `version.json` file and add the analytics configuration:

```json
{
  "Version": "2.3.1",
  "DownloadUrl": "https://drive.google.com/uc?export=download&id=YOUR_FILE_ID_HERE",
  "ReleaseNotes": "Your release notes here",
  "ReleaseDate": "2026-01-19",
  "IsCritical": false,
  "Analytics": {
    "MeasurementId": "G-XXXXXXXXXX",
    "ApiSecret": "YOUR_API_SECRET_HERE"
  }
}
```

Replace:
- `G-XXXXXXXXXX` with your **Measurement ID** from Step 2
- `YOUR_API_SECRET_HERE` with your **API secret** from Step 3

### Upload version.json

Upload your updated `version.json` file to your Google Drive (or wherever you host your update check file).

---

## Step 5: View Analytics Data

### Real-Time Reports

1. Go to **Google Analytics** → Your Property
2. Navigate to **Reports** → **Realtime**
3. You'll see active users in real-time when people start the application

### Custom Reports

1. Navigate to **Reports** → **Engagement** → **Events**
2. You can see:
   - `app_install` - New installations
   - `app_start` - Application starts (active users)

### Create Custom Dashboards

1. Go to **Explore** → **Create a new exploration**
2. Add metrics:
   - **Total users**: Unique device IDs (installs)
   - **Active users**: Users who started the app recently
   - **Event count**: Number of times events occurred

### Useful Metrics to Track

- **Total Users (All Time)**: Shows total unique installs
- **Active Users (Last 7 days)**: Shows weekly active users
- **Active Users (Last 30 days)**: Shows monthly active users
- **Events per User**: How often users start the app

---

## Step 6: Advanced Analytics (Optional)

### Track Additional Events

You can track custom events by modifying the code. For example:

```csharp
// Track when user applies a custom fan profile
await AnalyticsService.TrackEventAsync("profile_applied", new
{
    profile_name = "Custom",
    gpu_type = "NVIDIA"
});

// Track when user enables manual control
await AnalyticsService.TrackEventAsync("manual_control_enabled", new
{
    fan_speed = 75
});
```

### Create Custom Dimensions

In GA4, you can create custom dimensions to segment your data:
1. **Admin** → **Data display** → **Custom definitions**
2. Create dimensions like:
   - `app_version` - Track which versions are most used
   - `os_version` - See which Windows versions your users have
   - `gpu_type` - Track NVIDIA vs AMD vs Intel distribution

---

## Troubleshooting

### Analytics Not Working?

1. **Check version.json format**: Ensure JSON is valid
2. **Verify credentials**: Make sure Measurement ID and API Secret are correct
3. **Check internet connection**: Analytics requires internet access
4. **Wait for data**: Real-time data may take 1-2 minutes to appear
5. **Check Debug View**: In GA4, use DebugView to see events in real-time

### Test Analytics Locally

Run your application and check:
1. File created: `%AppData%\GPUFanController\analytics.dat` (contains anonymous client ID)
2. File created: `%AppData%\GPUFanController\.installed` (marks first install)
3. GA4 Real-time reports show activity within 1-2 minutes

### Common Issues

**Issue**: Events not appearing in GA4
- **Solution**: Check that your Measurement ID starts with "G-" (not "UA-")
- **Solution**: Verify API secret is from the correct property

**Issue**: Multiple installs being tracked for same device
- **Solution**: This is normal if user reinstalls or clears AppData folder

**Issue**: No data after 24 hours
- **Solution**: Check that your Google Drive version.json is publicly accessible
- **Solution**: Verify UpdateChecker URL is correct in MainForm.cs

---

## Privacy Compliance

### GDPR / Privacy Laws

The current implementation is privacy-friendly:
- ✅ Anonymous tracking (random UUIDs)
- ✅ No personal information collected
- ✅ No cookies or persistent identifiers beyond local anonymous ID

### Optional: Add Privacy Notice

Consider adding a privacy notice to your README or installer:

> **Privacy Notice**: This application uses anonymous analytics to track install counts and active users. No personal information is collected. A random anonymous ID is generated locally to prevent duplicate counting.

---

## Summary

✅ You've set up Google Analytics 4 tracking  
✅ Your application now tracks installs and active users  
✅ All tracking is anonymous and privacy-friendly  
✅ You can view your metrics in the GA4 dashboard  

### Quick Reference

- **Measurement ID**: Found in GA4 → Admin → Data Streams
- **API Secret**: Found in GA4 → Data Stream → Measurement Protocol API secrets
- **Configuration**: Add to `version.json` under "Analytics" section
- **View Data**: GA4 → Reports → Realtime and Events

---

## Need Help?

- **Google Analytics Help**: https://support.google.com/analytics
- **GA4 Measurement Protocol**: https://developers.google.com/analytics/devguides/collection/protocol/ga4
- **GPU Fan Controller Issues**: Create an issue on GitHub

---

*Generated: January 2026*
*Version: 1.0*
