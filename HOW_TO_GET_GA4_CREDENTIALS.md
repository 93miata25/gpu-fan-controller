# How to Get Your Google Analytics 4 Measurement ID and API Secret

## Complete Step-by-Step Guide (5-10 minutes)

### Step 1: Go to Google Analytics

1. Open your browser and go to: **https://analytics.google.com**
2. Sign in with your Google account (Gmail)

---

### Step 2: Create a Google Analytics Account (if you don't have one)

**If you already have a Google Analytics account, skip to Step 3.**

1. Click the **"Start measuring"** button
2. Fill in **Account details**:
   - Account name: `GPU Fan Controller` (or any name you want)
   - Check the boxes for data sharing settings (recommended)
3. Click **"Next"**

---

### Step 3: Create a Property

1. Click **"Admin"** (gear icon at bottom left)
2. In the **Account** column, select your account
3. In the **Property** column, click **"+ Create Property"**

4. Fill in **Property details**:
   - Property name: `GPU Fan Controller`
   - Reporting time zone: Select your timezone
   - Currency: Select your currency
5. Click **"Next"**

6. Fill in **Business details** (optional, choose any):
   - Industry category: `Computers & Electronics`
   - Business size: Choose any option
7. Click **"Next"**

8. Select **Business objectives**:
   - Check `Examine user behavior` or any option
9. Click **"Create"**

10. **Accept** the Terms of Service

---

### Step 4: Create a Data Stream

1. You'll see "Choose a platform" screen
2. Select **"Web"** (even though it's a desktop app, GA4 works via Measurement Protocol)

3. Fill in **Stream details**:
   - **Website URL**: `https://github.com/yourusername/gpu-fan-controller` 
     - (Use your GitHub repo URL, or any URL - it's just for reference)
   - **Stream name**: `Desktop App` or `GPU Fan Controller`
4. Click **"Create stream"**

---

### Step 5: Get Your Measurement ID ✅

After creating the stream, you'll see the **Stream details** page.

**Look for a box at the top that says:**

```
Measurement ID
G-XXXXXXXXXX
```

**This is your Measurement ID!**

📋 **Copy this ID** - You'll need it for your `version.json` file.

Example: `G-1A2B3C4D5E`

---

### Step 6: Generate API Secret ✅

On the same **Stream details** page:

1. Scroll down to the **"Measurement Protocol API secrets"** section
2. Click **"Create"**
3. Fill in:
   - **Nickname**: `Desktop App Secret` (or any name)
4. Click **"Create"**

5. **You'll see a popup with your API Secret**:
   ```
   Secret value
   ABCdef123XYZ456_your_secret_here
   ```

   ⚠️ **IMPORTANT**: Copy this secret NOW! You can only see it once!
   
   📋 **Copy and save this secret somewhere safe** (notepad, password manager, etc.)

6. Click **"Close"**

---

### Step 7: Verify Your Credentials

You should now have:

✅ **Measurement ID**: `G-XXXXXXXXXX`  
✅ **API Secret**: `ABCdef123XYZ456_your_secret_here`

---

### Step 8: Add to version.json

Open your `version.json` file and update it:

```json
{
  "Version": "2.3.1",
  "DownloadUrl": "https://drive.google.com/uc?export=download&id=YOUR_FILE_ID",
  "ReleaseNotes": "Your release notes here",
  "ReleaseDate": "2026-01-19",
  "IsCritical": false,
  "Analytics": {
    "MeasurementId": "G-XXXXXXXXXX",
    "ApiSecret": "ABCdef123XYZ456_your_secret_here"
  }
}
```

Replace:
- `G-XXXXXXXXXX` with YOUR Measurement ID
- `ABCdef123XYZ456_your_secret_here` with YOUR API Secret

---

### Step 9: Upload version.json

Upload your updated `version.json` to your hosting location:
- Google Drive (make it publicly accessible)
- GitHub
- Your own server
- Any public URL

Make sure the URL is publicly accessible!

---

### Step 10: Test It!

1. **Build your application**:
   ```powershell
   dotnet build GPUFanController.csproj
   ```

2. **Run your application**:
   ```powershell
   .\bin\Debug\net6.0-windows\GPUFanController.exe
   ```

3. **Check GA4 Real-time Reports**:
   - Go to Google Analytics
   - Click **"Reports"** (left sidebar)
   - Click **"Realtime"**
   - Wait 1-2 minutes
   - You should see activity! 🎉

4. **Verify files were created**:
   ```powershell
   dir $env:APPDATA\GPUFanController
   ```
   Should show:
   - `analytics.dat` (your anonymous ID)
   - `.installed` (install marker)

---

## Visual Guide to Finding Things

### Finding Admin Panel
```
Google Analytics Homepage
└── Bottom left corner → Click ⚙️ "Admin"
```

### Finding Data Streams
```
Admin Panel
├── Account column (left)
└── Property column (middle)
    └── "Data Streams" (click it)
        └── Click your stream name
            └── See Measurement ID at top!
```

### Finding API Secrets
```
Data Stream Details Page
└── Scroll down to "Measurement Protocol API secrets"
    └── Click "Create"
        └── Copy the secret value!
```

---

## Troubleshooting

### Problem: "I don't see Measurement Protocol API secrets"

**Solution**: 
- Make sure you created a **Web** data stream (not iOS or Android)
- Scroll down on the stream details page - it's near the bottom

### Problem: "I lost my API Secret"

**Solution**: 
- You can't view it again, but you can create a new one
- Go to Data Stream → Measurement Protocol API secrets → Create new one
- Update your `version.json` with the new secret

### Problem: "Where do I find my Measurement ID later?"

**Solution**:
1. Go to Admin → Property → Data Streams
2. Click your stream name
3. Measurement ID is at the top of the page

### Problem: "Can I use UA- ID instead of G- ID?"

**Solution**: 
- No! `UA-` is Universal Analytics (old version, being sunset)
- You MUST use `G-` which is Google Analytics 4 (GA4)
- If you only have UA-, create a new GA4 property

---

## Quick Reference

| What | Where to Find It | Looks Like |
|------|------------------|------------|
| **Measurement ID** | Data Stream details (top) | `G-XXXXXXXXXX` |
| **API Secret** | Data Stream → API secrets → Create | `ABCdef123XYZ_long_string` |
| **Data Streams** | Admin → Property → Data Streams | List of streams |
| **Realtime Reports** | Reports → Realtime | Live activity dashboard |

---

## Security Notes

🔒 **Keep your API Secret secure!**
- Don't commit it to public GitHub repos
- Store it in `version.json` on your private server
- Only share `version.json` with your app users (it's okay, they need it)

🔒 **API Secret vs Measurement ID:**
- **Measurement ID**: Semi-public (users will see it)
- **API Secret**: Should be kept reasonably private, but it's okay if users see it in `version.json` - it only allows sending events to YOUR analytics, not reading data

---

## What's Next?

After you have your credentials:

1. ✅ Update `version.json` with Measurement ID and API Secret
2. ✅ Upload `version.json` to public URL
3. ✅ Update your app's UpdateChecker URL if needed
4. ✅ Build and test your app
5. ✅ Check GA4 Realtime reports
6. ✅ Distribute your app to users!

---

## Need More Help?

- **Google Analytics Help**: https://support.google.com/analytics
- **GA4 Setup Guide**: https://support.google.com/analytics/answer/9304153
- **Measurement Protocol**: https://developers.google.com/analytics/devguides/collection/protocol/ga4

---

**You're all set!** Once you have both credentials, your analytics will work automatically! 🚀

---

*Last updated: January 2026*
