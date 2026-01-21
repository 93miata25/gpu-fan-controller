# How to Deploy GPU Fan Controller Website to Hostinger

## Prerequisites

- ✅ Hostinger account with active hosting plan
- ✅ Domain name (or use Hostinger's free subdomain)
- ✅ FTP client (FileZilla recommended) or use Hostinger File Manager

---

## Method 1: Using Hostinger File Manager (Easiest)

### Step 1: Login to Hostinger

1. Go to https://www.hostinger.com
2. Click **"Login"**
3. Enter your credentials
4. Click on **"Hosting"** in the dashboard

### Step 2: Access File Manager

1. Find your hosting plan
2. Click **"Manage"**
3. In the left sidebar, click **"File Manager"** or **"Files"**
4. Click **"File Manager"** button to open it

### Step 3: Navigate to public_html

1. In File Manager, open the **`public_html`** folder
2. This is your website's root directory
3. Delete any default files (like `index.html` if it exists)

### Step 4: Upload Your Website Files

**Files to Upload:**
1. Click **"Upload Files"** button
2. Select and upload these files:
   - `index.html`
   - `favicon.ico`
   - `favicon-16x16.png`
   - `favicon-32x32.png`
   - `apple-touch-icon.png`
   - `gpu_fan_controller_screenshot.png`
   - `GPUFanController_Setup_v2.3.1.exe` (68.7 MB)
   - `gpu-fan-controller-2.3.1-linux-x64.tar.gz` (28.15 MB)
   - `INSTALL_WINDOWS.md`
   - `INSTALL_LINUX.md`

**Note**: Large files (installers) may take several minutes to upload.

### Step 5: Verify Upload

1. Check that all files are in `public_html`
2. Make sure `index.html` is at the root level (not in a subfolder)

### Step 6: Test Your Website

1. Go to your domain: `http://yourdomain.com`
2. Or use Hostinger preview URL (found in hosting dashboard)
3. Your website should now be live! 🎉

---

## Method 2: Using FTP (FileZilla)

### Step 1: Get FTP Credentials

1. Login to Hostinger dashboard
2. Go to **Hosting** → **Manage**
3. Find **"FTP Accounts"** in the left sidebar
4. You'll see:
   - **FTP Host**: `ftp.yourdomain.com` or IP address
   - **FTP Username**: Usually your main account username
   - **FTP Port**: 21
   - **Password**: Click "Change Password" if you don't know it

### Step 2: Download FileZilla

1. Go to https://filezilla-project.org/
2. Download **FileZilla Client** (free)
3. Install it

### Step 3: Connect to Hostinger

1. Open FileZilla
2. Enter your FTP credentials:
   - **Host**: ftp.yourdomain.com
   - **Username**: Your FTP username
   - **Password**: Your FTP password
   - **Port**: 21
3. Click **"Quickconnect"**

### Step 4: Upload Files

1. **Left side**: Your local computer files
2. **Right side**: Your server (navigate to `public_html`)
3. Drag and drop all website files from left to right:
   - `index.html`
   - All favicon files
   - Screenshot
   - Installers
   - Installation guides

### Step 5: Verify and Test

1. Wait for upload to complete (check progress at bottom)
2. Visit your domain in browser
3. Website should be live! 🎉

---

## Method 3: Using Git/SSH (Advanced)

If your Hostinger plan includes SSH access:

### Step 1: Enable SSH

1. Hostinger dashboard → Hosting → Manage
2. Find **"SSH Access"**
3. Enable SSH if not already enabled
4. Note your SSH credentials

### Step 2: Connect via SSH

```bash
ssh username@yourdomain.com
# Or use IP: ssh username@123.456.789.0
```

### Step 3: Navigate and Upload

```bash
# Go to public_html
cd public_html

# Remove default files
rm -f index.html

# Upload files (from your local machine, use SCP)
# On your local machine:
scp -r /path/to/your/website/* username@yourdomain.com:~/public_html/
```

---

## Post-Deployment Checklist

### ✅ Test All Features

1. **Homepage loads**: Visit your domain
2. **Favicon appears**: Check browser tab
3. **Windows download works**: Click Windows download button
4. **Linux download works**: Click Linux download button
5. **Installation guides display**: Scroll to guides section
6. **Links work**: Test all navigation links
7. **Mobile responsive**: Check on phone

### ✅ File Permissions (if needed)

If files don't display properly:

1. In File Manager or FTP, right-click files
2. Set permissions to:
   - **Files**: 644 (rw-r--r--)
   - **Folders**: 755 (rwxr-xr-x)

### ✅ Set Up SSL (HTTPS)

**Important for downloads!**

1. Hostinger dashboard → Hosting → Manage
2. Click **"SSL"** in sidebar
3. Enable **"Free SSL Certificate"**
4. Wait 10-15 minutes for activation
5. Your site will be accessible via `https://yourdomain.com`

### ✅ Update Download Links (if using HTTPS)

If you set up SSL, update the Linux wget command in your HTML:

Change:
```
wget http://your-site.com/gpu-fan-controller-2.3.1-linux-x64.tar.gz
```

To:
```
wget https://your-site.com/gpu-fan-controller-2.3.1-linux-x64.tar.gz
```

---

## Common Issues & Solutions

### Issue: "404 Not Found"

**Solution**:
- Make sure `index.html` is in `public_html` root
- Check filename is exactly `index.html` (lowercase)
- Clear browser cache (Ctrl+F5)

### Issue: Downloads Don't Work

**Solution**:
- Verify installer files are fully uploaded
- Check file permissions (644)
- Make sure filenames match exactly in HTML

### Issue: Favicon Doesn't Show

**Solution**:
- Clear browser cache (Ctrl+Shift+Delete)
- Wait 5-10 minutes for CDN cache to clear
- Check favicon files are in root directory

### Issue: Large Files Timeout During Upload

**Solution**:
- Use FTP (FileZilla) instead of web uploader
- Split uploads: first small files, then large installers
- Check your upload speed is stable

### Issue: Website Shows "Coming Soon" Page

**Solution**:
- Delete default Hostinger landing page
- Make sure `index.html` is in correct location
- May need to disable "Coming Soon" in Hostinger settings

---

## Optimizations (Optional)

### Enable Gzip Compression

Create `.htaccess` file in `public_html`:

```apache
# Enable Gzip Compression
<IfModule mod_deflate.c>
  AddOutputFilterByType DEFLATE text/html text/plain text/xml text/css text/javascript application/javascript
</IfModule>

# Browser Caching
<IfModule mod_expires.c>
  ExpiresActive On
  ExpiresByType image/png "access plus 1 month"
  ExpiresByType image/x-icon "access plus 1 month"
  ExpiresByType text/html "access plus 1 hour"
</IfModule>
```

### Create robots.txt

Create `robots.txt` in `public_html`:

```
User-agent: *
Allow: /

Sitemap: https://yourdomain.com/sitemap.xml
```

### Add Custom Domain (if needed)

1. Hostinger dashboard → Domains
2. Click **"Add Domain"**
3. Follow wizard to connect your domain
4. Update nameservers at your domain registrar
5. Wait 24-48 hours for DNS propagation

---

## Quick Upload Script (PowerShell)

If you have FTP credentials, save this as `upload-to-hostinger.ps1`:

```powershell
# FTP Upload Script
$ftpHost = "ftp://ftp.yourdomain.com/public_html/"
$ftpUser = "your-username"
$ftpPass = "your-password"

$files = @(
    "index.html",
    "favicon.ico",
    "favicon-16x16.png",
    "favicon-32x32.png",
    "apple-touch-icon.png",
    "gpu_fan_controller_screenshot.png",
    "GPUFanController_Setup_v2.3.1.exe",
    "gpu-fan-controller-2.3.1-linux-x64.tar.gz",
    "INSTALL_WINDOWS.md",
    "INSTALL_LINUX.md"
)

foreach ($file in $files) {
    Write-Host "Uploading $file..." -ForegroundColor Yellow
    $webclient = New-Object System.Net.WebClient
    $webclient.Credentials = New-Object System.Net.NetworkCredential($ftpUser, $ftpPass)
    $uri = New-Object System.Uri($ftpHost + $file)
    $webclient.UploadFile($uri, $file)
    Write-Host "  ✓ Uploaded $file" -ForegroundColor Green
}

Write-Host ""
Write-Host "✅ All files uploaded!" -ForegroundColor Green
```

**Usage**:
1. Edit FTP credentials in script
2. Run: `.\upload-to-hostinger.ps1`

---

## Summary

### Recommended Method for Beginners:
✅ **Hostinger File Manager** (Method 1)
- No extra software needed
- Simple drag-and-drop
- Built into Hostinger dashboard

### Recommended Method for Regular Updates:
✅ **FileZilla FTP** (Method 2)
- Faster uploads
- Better for large files
- Can resume interrupted uploads

---

## Support

- **Hostinger Help**: https://support.hostinger.com
- **FileZilla Guide**: https://filezilla-project.org/
- **Your Website**: Will be live at your domain!

---

**Good luck with your deployment!** 🚀

Once uploaded, your website will be accessible at:
- `http://yourdomain.com` (or your Hostinger subdomain)
- `https://yourdomain.com` (after enabling SSL)
