# Deployment Guide - Linux Package & Website

## ✅ What Was Completed

### 1. Linux Package Created
- **File**: `installer_output/linux/GPUFanController-2.3.1-linux-x64.zip`
- **Size**: 28.17 MB
- **Contents**:
  - GPUFanControllerConsole (executable)
  - install.sh (installation script)
  - README.md, INSTALL.txt, LINUX_INSTALLATION_GUIDE.md
  - All required .NET runtime dependencies

### 2. Website Updated
- **File**: `index.html`
- **Changes**:
  - Added Linux download button (blue colored)
  - Updated header to show "Windows & Linux" support
  - Dual download buttons with platform indicators
  - Clear file size and platform information

## 📦 Package Structure

```
GPUFanController-2.3.1-linux-x64.zip
├── GPUFanControllerConsole         # Main executable
├── install.sh                       # Installation script
├── README.md                        # Main documentation
├── INSTALL.txt                      # Quick start guide
├── LINUX_INSTALLATION_GUIDE.md     # Detailed guide
└── [dependencies]                   # .NET runtime files
```

## 🌐 Website Deployment

### Files to Upload
1. **index.html** (updated with Linux download)
2. **installer_output/linux/GPUFanController-2.3.1-linux-x64.zip** (Linux package)
3. **GPUFanController_Setup_v2.3.1.exe** (Windows installer)

### Recommended Directory Structure
```
your-website/
├── index.html
├── GPUFanController_Setup_v2.3.1.exe
└── installer_output/
    └── linux/
        └── GPUFanController-2.3.1-linux-x64.zip
```

## 🚀 Deployment Steps

### Option 1: Static File Hosting (GitHub Pages, Netlify, etc.)

1. **Commit files to repository**:
   ```bash
   git add index.html installer_output/linux/
   git commit -m "Add Linux support and download"
   git push
   ```

2. **Deploy to hosting service**:
   - GitHub Pages: Enable in repository settings
   - Netlify: Connect repository and deploy
   - Vercel: Connect and auto-deploy

### Option 2: Manual Upload (FTP/SFTP)

1. **Connect to your web server**:
   ```bash
   sftp user@yourserver.com
   ```

2. **Upload files**:
   ```bash
   put index.html
   put GPUFanController_Setup_v2.3.1.exe
   put -r installer_output/linux/
   ```

### Option 3: Using Git and Web Server

1. **On your server**:
   ```bash
   cd /var/www/html/gpufancontroller
   git pull origin main
   ```

## 🧪 Testing Before Deployment

### Test Locally
1. **Open index.html in browser**:
   - Windows: Double-click index.html
   - Linux/Mac: `open index.html` or `xdg-open index.html`

2. **Verify**:
   - ✓ Both download buttons visible
   - ✓ Windows button (orange) links to .exe
   - ✓ Linux button (blue) links to .zip
   - ✓ File sizes displayed correctly
   - ✓ Responsive on mobile devices

### Test Downloads
1. Click Windows button → Should download .exe
2. Click Linux button → Should download .zip
3. Extract zip on Linux and verify contents

## 📝 Linux Installation Instructions for Users

Share these instructions with Linux users:

```bash
# Download the package
wget [YOUR_URL]/installer_output/linux/GPUFanController-2.3.1-linux-x64.zip

# Extract
unzip GPUFanController-2.3.1-linux-x64.zip
cd GPUFanController-2.3.1-linux-x64

# Install (requires sudo)
sudo ./install.sh

# Run
sudo gpufancontroller
```

## 🔗 Update Download Links

If you change hosting location, update these links in `index.html`:

1. **Windows download** (line ~1073):
   ```html
   <a href="GPUFanController_Setup_v2.3.1.exe" class="download-btn">
   ```

2. **Linux download** (line ~1076):
   ```html
   <a href="installer_output/linux/GPUFanController-2.3.1-linux-x64.zip" class="download-btn">
   ```

## 📊 Analytics (Optional)

Consider adding download tracking:

### Google Analytics
```html
<script async src="https://www.googletagmanager.com/gtag/js?id=YOUR-ID"></script>
<script>
  window.dataLayer = window.dataLayer || [];
  function gtag(){dataLayer.push(arguments);}
  gtag('js', new Date());
  gtag('config', 'YOUR-ID');
</script>
```

### Track Downloads
```javascript
// Add to download buttons
onclick="gtag('event', 'download', {'platform': 'windows'});"
onclick="gtag('event', 'download', {'platform': 'linux'});"
```

## 🔒 Security Considerations

### For Linux Package
- ✓ Package is self-contained (no external dependencies)
- ✓ Installation script requires sudo (expected)
- ✓ Users can review install.sh before running

### For Website
- Use HTTPS for all downloads
- Consider adding SHA256 checksums
- Sign packages (optional but recommended)

## 📋 Checklist

Before going live:

- [ ] Test index.html in multiple browsers
- [ ] Verify both download links work
- [ ] Test mobile responsiveness
- [ ] Check file permissions on server
- [ ] Verify package extracts correctly
- [ ] Test installation on clean Linux system
- [ ] Update README.md with download links
- [ ] Create GitHub release (optional)
- [ ] Announce on social media/forums

## 🆘 Troubleshooting

### Download buttons not working
- Check file paths in href attributes
- Verify files are uploaded to correct location
- Check web server directory structure

### Linux package won't extract
- Verify zip file isn't corrupted
- Check file size matches (28.17 MB)
- Use unzip command on Linux, not tar

### Installation script fails
- Ensure script has execute permissions: `chmod +x install.sh`
- Run with sudo: `sudo ./install.sh`
- Check system has .NET support

## 🎉 Success Criteria

Your deployment is successful when:
- ✅ Website loads without errors
- ✅ Windows download works
- ✅ Linux download works
- ✅ Linux package installs correctly
- ✅ Application runs on both platforms

## 📞 Support

For deployment issues:
- Check web server logs
- Verify file permissions
- Test in different browsers
- Consult hosting provider documentation

---

**Version**: 2.3.1  
**Last Updated**: 2026-01-19  
**Platform Support**: Windows 10/11, Linux (x86_64)
