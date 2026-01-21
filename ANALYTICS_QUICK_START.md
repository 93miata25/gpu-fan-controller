# Analytics Quick Start - 5 Minute Setup

Get your download/user counter running in 5 minutes!

## Step 1: Google Analytics (2 minutes)

1. Go to https://analytics.google.com
2. Create a new GA4 Property called "GPU Fan Controller"
3. Create a "Web" data stream
4. Copy your **Measurement ID** (looks like `G-XXXXXXXXXX`)
5. Click "Measurement Protocol API secrets" → Create
6. Copy your **API Secret**

## Step 2: Update Configuration (1 minute)

Edit your `version.json` file:

```json
{
  "Version": "2.3.1",
  "DownloadUrl": "https://your-download-link.com",
  "ReleaseNotes": "Your release notes",
  "ReleaseDate": "2026-01-19",
  "IsCritical": false,
  "Analytics": {
    "MeasurementId": "G-XXXXXXXXXX",
    "ApiSecret": "your-api-secret-here"
  }
}
```

## Step 3: Upload & Test (2 minutes)

1. Upload your `version.json` to Google Drive (make it public)
2. Run your application
3. Go to GA4 → Reports → Realtime
4. You should see activity within 1-2 minutes!

## What You'll See

### In Google Analytics:

**Realtime Report:**
- Active users right now

**Engagement → Events:**
- `app_install` - Total installs
- `app_start` - Total app launches

**Reports → User:**
- Total users (unique installs)
- Active users (7-day, 30-day)
- New users (new installs)

### On User's Computer:

**Files created:**
- `%AppData%\GPUFanController\analytics.dat` - Anonymous ID
- `%AppData%\GPUFanController\.installed` - Install marker

**No UI changes** - Everything runs silently in the background!

## Troubleshooting

**No data showing up?**
- Wait 2-3 minutes (real-time data has slight delay)
- Check Measurement ID starts with "G-" not "UA-"
- Verify version.json is publicly accessible
- Check internet connection

**Multiple installs from same user?**
- Normal if user reinstalls or clears AppData folder

## Privacy

✅ **No personal data collected**  
✅ **Anonymous device IDs only**  
✅ **GDPR compliant**  
✅ **User can delete files to reset**

## Need More Details?

See full guides:
- **[ANALYTICS_SETUP_GUIDE.md](ANALYTICS_SETUP_GUIDE.md)** - Complete setup instructions
- **[ANALYTICS_IMPLEMENTATION.md](ANALYTICS_IMPLEMENTATION.md)** - Technical documentation

---

That's it! You now have download and user tracking! 🎉
