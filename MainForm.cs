using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace GPUFanController
{
    public partial class MainForm : Form
    {
        private MultiGPUController _multiGPUController;
        private Dictionary<int, AutoFanController> _autoControllers;
        private System.Windows.Forms.Timer _updateTimer;
        private bool _isManualControl = false;
        private int _selectedGPUIndex = 0;
        private PresetManager _presetManager;
        private AppConfig _appConfig;

        // UI Controls
        private TrackBar fanSpeedSlider;
        private Label lblTitle;
        private Label lblGPUName;
        private Label lblGPUType;
        private Label lblCurrentSpeed;
        private Label lblTemperature;
        private Label lblFanRPM;
        private Label lblSliderValue;
        private Button btnApply;
        private Button btnAuto;
        private CheckBox chkManualControl;
        private GroupBox grpStatus;
        private GroupBox grpControl;
        private GroupBox grpAutoControl;
        private GroupBox grpSettings;
        private Label lblWarning;
        private ComboBox cmbGPUSelect;
        private ComboBox cmbFanProfile;
        private CheckBox chkAutoMode;
        private Button btnApplyProfile;
        private Label lblAutoStatus;
        private Button btnSavePreset;
        private Button btnLoadPreset;
        private Button btnDeletePreset;
        private CheckBox chkStartWithWindows;
        private CheckBox chkStartMinimized;
        private Button btnDiagnostics;
        private SystemTrayManager _trayManager;

        public MainForm(bool startMinimized = false)
        {
            _autoControllers = new Dictionary<int, AutoFanController>();
            _presetManager = new PresetManager();
            _appConfig = _presetManager.LoadConfig();
            InitializeComponent();
            InitializeGPUController();
            SetupUpdateTimer();
            LoadSettings();
            _trayManager = new SystemTrayManager(this);
            
            // Handle resize events
            this.Resize += MainForm_Resize;
            this.Load += MainForm_Load;
            
            // Add keyboard shortcuts
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
            
            // Check for updates after form loads
            this.Shown += MainForm_Shown;
            
            // Start minimized if requested
            if (startMinimized)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
        }

        private async void MainForm_Shown(object? sender, EventArgs e)
        {
            // Configure analytics with environment variables (secure for open source)
            var gaId = Environment.GetEnvironmentVariable("GA_MEASUREMENT_ID");
            var gaSecret = Environment.GetEnvironmentVariable("GA_API_SECRET");
            if (!string.IsNullOrEmpty(gaId) && !string.IsNullOrEmpty(gaSecret))
            {
                AnalyticsService.Configure(gaId, gaSecret);
            }
            
            // Run analytics and update check in background without blocking UI
            _ = Task.Run(async () =>
            {
                // Track install and app start
                await AnalyticsService.TrackInstallAsync();
                await AnalyticsService.TrackAppStartAsync();
                
                // Wait a bit before checking for updates
                await Task.Delay(3000);
                
                // Check for updates on UI thread
                this.Invoke(new Action(async () => await CheckForUpdates()));
            });
        }

        private async Task CheckForUpdates(bool showNoUpdateMessage = false)
        {
            try
            {
                // Set your Google Drive direct link here
                // To get direct link: Share file → Anyone with the link → Copy link
                // Then convert: https://drive.google.com/file/d/FILE_ID/view?usp=sharing
                // To: https://drive.google.com/uc?export=download&id=FILE_ID
                
                string updateUrl = "https://gpufancontroller.com/version.json";
                
                UpdateChecker.SetUpdateUrl(updateUrl);
                var updateInfo = await UpdateChecker.CheckForUpdatesAsync();

                if (updateInfo != null)
                {
                    ShowUpdateNotification(updateInfo);
                }
                else if (showNoUpdateMessage)
                {
                    MessageBox.Show(
                        $"✅ You're up to date!\n\n" +
                        $"Current Version: {UpdateChecker.CurrentVersion}\n" +
                        $"Latest Version: {UpdateChecker.CurrentVersion}\n\n" +
                        "You're running the latest version of GPU Fan Controller.",
                        "No Updates Available",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                if (showNoUpdateMessage)
                {
                    MessageBox.Show(
                        $"❌ Failed to check for updates.\n\n" +
                        $"Error: {ex.Message}\n\n" +
                        "Please check your internet connection and try again.",
                        "Update Check Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                // Silently fail on automatic checks
            }
        }

        private void ShowUpdateNotification(UpdateInfo updateInfo)
        {
            // Show dialog only (no tray notification)
            var result = System.Windows.Forms.MessageBox.Show(
                $"🎉 New Update Available!\n\n" +
                $"Current Version: {UpdateChecker.CurrentVersion}\n" +
                $"New Version: {updateInfo.Version}\n" +
                $"Released: {updateInfo.ReleaseDate}\n\n" +
                $"What's New:\n{updateInfo.ReleaseNotes}\n\n" +
                (updateInfo.IsCritical ? "⚠️ This is a critical update!\n\n" : "") +
                $"Would you like to download the update now?",
                "Update Available",
                MessageBoxButtons.YesNo,
                updateInfo.IsCritical ? MessageBoxIcon.Warning : MessageBoxIcon.Information
            );

            if (result == DialogResult.Yes)
            {
                // Use platform-specific download URL
                string downloadUrl = updateInfo.GetPlatformDownloadUrl();
                UpdateChecker.OpenDownloadUrl(downloadUrl);
            }
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            // Adjust layout based on window size
            AdjustLayoutForSize();
        }

        private void MainForm_Resize(object? sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                _trayManager?.HideToTray();
                // Don't show balloon here - already shown in FormClosing
            }
            else
            {
                // Adjust layout when resizing
                AdjustLayoutForSize();
            }
        }

        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            // F11 for fullscreen toggle
            if (e.KeyCode == Keys.F11)
            {
                ToggleFullscreen();
                e.Handled = true;
            }
            // ESC to exit fullscreen
            else if (e.KeyCode == Keys.Escape && this.FormBorderStyle == FormBorderStyle.None)
            {
                ToggleFullscreen();
                e.Handled = true;
            }
        }

        private bool _isFullscreen = false;
        private FormBorderStyle _previousBorderStyle;
        private FormWindowState _previousWindowState;
        private Rectangle _previousBounds;

        private bool _hasShownFullscreenNotification = false;

        private void ToggleFullscreen()
        {
            if (!_isFullscreen)
            {
                // Enter fullscreen
                _previousBorderStyle = this.FormBorderStyle;
                _previousWindowState = this.WindowState;
                _previousBounds = this.Bounds;

                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                _isFullscreen = true;

                // Show notification only once
                if (!_hasShownFullscreenNotification)
                {
                    _trayManager?.ShowBalloonTip(
                        "Fullscreen Mode",
                        "Press F11 or ESC to exit",
                        ToolTipIcon.Info
                    );
                    _hasShownFullscreenNotification = true;
                }
            }
            else
            {
                // Exit fullscreen
                this.FormBorderStyle = _previousBorderStyle;
                this.WindowState = _previousWindowState;
                this.Bounds = _previousBounds;
                _isFullscreen = false;
            }

            AdjustLayoutForSize();
        }

        private void AdjustLayoutForSize()
        {
            if (this.WindowState == FormWindowState.Minimized)
                return;

            int padding = 20;
            int controlWidth = this.ClientSize.Width - (padding * 2);
            
            // Adjust control widths to fill available space
            if (grpStatus != null)
            {
                grpStatus.Width = controlWidth;
            }
            if (grpAutoControl != null)
            {
                grpAutoControl.Width = controlWidth;
            }
            if (grpControl != null)
            {
                grpControl.Width = controlWidth;
            }
            if (grpSettings != null)
            {
                grpSettings.Width = controlWidth;
            }
            if (lblTitle != null)
            {
                lblTitle.Width = controlWidth;
            }
            if (cmbGPUSelect != null && cmbGPUSelect.Left + 375 > this.ClientSize.Width - padding)
            {
                cmbGPUSelect.Width = controlWidth - 85;
            }
        }

        private void InitializeComponent()
        {
            this.Text = "GPU Fan Controller - Enhanced";
            
            // AUTO-SIZE BASED ON SCREEN
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            int formWidth = Math.Min(600, workingArea.Width - 100);
            int formHeight = Math.Min(900, workingArea.Height - 100);
            this.Size = new Size(formWidth, formHeight);
            this.MinimumSize = new Size(540, 820); // Minimum size
            
            this.FormBorderStyle = FormBorderStyle.Sizable; // Allow resizing
            this.MaximizeBox = true; // Enable maximize button
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // MODERN LIGHT THEME
            this.BackColor = Color.FromArgb(240, 242, 245); // Light gray background
            this.ForeColor = Color.FromArgb(33, 37, 41); // Dark text
            
            // Enable auto-scroll for smaller screens
            this.AutoScroll = true;
            
            // Enable double buffering to reduce flicker
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | 
                         ControlStyles.AllPaintingInWmPaint | 
                         ControlStyles.UserPaint, true);
            this.UpdateStyles();

            // Title - MODERN LIGHT STYLE
            lblTitle = new Label
            {
                Text = $"GPU Fan Controller v{UpdateChecker.CurrentVersion}",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Location = new Point(20, 15),
                Size = new Size(490, 45),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(13, 110, 253), // Modern blue
                BackColor = Color.White
            };
            this.Controls.Add(lblTitle);

            // GPU Selection - MODERN LIGHT STYLE
            Label lblGPUSelect = new Label
            {
                Text = "Select GPU:",
                Location = new Point(20, 70),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblGPUSelect);

            cmbGPUSelect = new ComboBox
            {
                Location = new Point(105, 68),
                Size = new Size(385, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(33, 37, 41)
            };
            cmbGPUSelect.SelectedIndexChanged += CmbGPUSelect_SelectedIndexChanged;
            this.Controls.Add(cmbGPUSelect);

            // Status Group - MODERN LIGHT STYLE
            grpStatus = new GroupBox
            {
                Text = " GPU Status ",
                Location = new Point(20, 105),
                Size = new Size(490, 160),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(13, 110, 253), // Modern blue
                BackColor = Color.White
            };

            lblGPUName = new Label
            {
                Text = "GPU: Detecting...",
                Location = new Point(15, 30),
                Size = new Size(450, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41), // Dark gray
                BackColor = Color.Transparent
            };
            grpStatus.Controls.Add(lblGPUName);

            lblGPUType = new Label
            {
                Text = "Type: Unknown",
                Location = new Point(15, 58),
                Size = new Size(450, 22),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(108, 117, 125), // Gray
                BackColor = Color.Transparent
            };
            grpStatus.Controls.Add(lblGPUType);

            lblTemperature = new Label
            {
                Text = "Temperature: -- °C",
                Location = new Point(15, 85),
                Size = new Size(450, 24),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 135, 84), // Green
                BackColor = Color.Transparent
            };
            grpStatus.Controls.Add(lblTemperature);

            lblFanRPM = new Label
            {
                Text = "Fan Speed: -- RPM",
                Location = new Point(15, 112),
                Size = new Size(450, 22),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(108, 117, 125),
                BackColor = Color.Transparent
            };
            grpStatus.Controls.Add(lblFanRPM);

            lblCurrentSpeed = new Label
            {
                Text = "Fan Control: -- %",
                Location = new Point(15, 136),
                Size = new Size(450, 22),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(108, 117, 125),
                BackColor = Color.Transparent
            };
            grpStatus.Controls.Add(lblCurrentSpeed);

            this.Controls.Add(grpStatus);

            // Auto Control Group - MODERN LIGHT STYLE
            grpAutoControl = new GroupBox
            {
                Text = " Automatic Fan Control ",
                Location = new Point(20, 275),
                Size = new Size(490, 155),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 135, 84), // Green
                BackColor = Color.White
            };

            chkAutoMode = new CheckBox
            {
                Text = "Enable Auto Mode",
                Location = new Point(15, 30),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                BackColor = Color.Transparent
            };
            chkAutoMode.CheckedChanged += ChkAutoMode_CheckedChanged;
            grpAutoControl.Controls.Add(chkAutoMode);

            Label lblProfile = new Label
            {
                Text = "Profile:",
                Location = new Point(15, 60),
                Size = new Size(60, 25),
                Font = new Font("Segoe UI", 9)
            };
            grpAutoControl.Controls.Add(lblProfile);

            cmbFanProfile = new ComboBox
            {
                Location = new Point(80, 58),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9),
                Enabled = false
            };
            cmbFanProfile.Items.AddRange(new object[] { "Silent", "Balanced", "Performance", "Aggressive" });
            cmbFanProfile.SelectedIndex = 1;
            grpAutoControl.Controls.Add(cmbFanProfile);

            btnApplyProfile = new Button
            {
                Text = "Start Auto",
                Location = new Point(240, 55),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9),
                Enabled = false
            };
            btnApplyProfile.Click += BtnApplyProfile_Click;
            grpAutoControl.Controls.Add(btnApplyProfile);

            lblAutoStatus = new Label
            {
                Text = "Status: Inactive",
                Location = new Point(15, 95),
                Size = new Size(440, 45),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray
            };
            grpAutoControl.Controls.Add(lblAutoStatus);

            this.Controls.Add(grpAutoControl);

            // Control Group - MODERN LIGHT STYLE
            grpControl = new GroupBox
            {
                Text = " Manual Fan Control ",
                Location = new Point(20, 440),
                Size = new Size(490, 180),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69), // Red
                BackColor = Color.White
            };

            chkManualControl = new CheckBox
            {
                Text = "Enable Manual Control",
                Location = new Point(15, 30),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                BackColor = Color.Transparent
            };
            chkManualControl.CheckedChanged += ChkManualControl_CheckedChanged;
            grpControl.Controls.Add(chkManualControl);

            Label lblSlider = new Label
            {
                Text = "Fan Speed:",
                Location = new Point(15, 60),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 9)
            };
            grpControl.Controls.Add(lblSlider);

            fanSpeedSlider = new TrackBar
            {
                Location = new Point(100, 55),
                Size = new Size(250, 45),
                Minimum = 0,
                Maximum = 100,
                TickFrequency = 10,
                Value = 50,
                Enabled = false
            };
            fanSpeedSlider.ValueChanged += FanSpeedSlider_ValueChanged;
            grpControl.Controls.Add(fanSpeedSlider);

            lblSliderValue = new Label
            {
                Text = "50%",
                Location = new Point(360, 60),
                Size = new Size(60, 25),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            grpControl.Controls.Add(lblSliderValue);

            btnApply = new Button
            {
                Text = "Apply",
                Location = new Point(100, 105),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 9),
                Enabled = false
            };
            btnApply.Click += BtnApply_Click;
            grpControl.Controls.Add(btnApply);

            btnAuto = new Button
            {
                Text = "Reset to Auto",
                Location = new Point(210, 105),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 9)
            };
            btnAuto.Click += BtnAuto_Click;
            grpControl.Controls.Add(btnAuto);

            lblWarning = new Label
            {
                Text = "⚠ Warning: Manual fan control can damage your GPU if set too low!",
                Location = new Point(15, 145),
                Size = new Size(410, 30),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleCenter
            };
            grpControl.Controls.Add(lblWarning);

            this.Controls.Add(grpControl);

            // Settings Group - MODERN LIGHT STYLE
            grpSettings = new GroupBox
            {
                Text = " Settings & Presets ",
                Location = new Point(20, 630),
                Size = new Size(490, 150),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(111, 66, 193), // Purple
                BackColor = Color.White
            };

            Label lblPresets = new Label
            {
                Text = "Presets:",
                Location = new Point(135, 30),
                Size = new Size(60, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                BackColor = Color.Transparent
            };
            grpSettings.Controls.Add(lblPresets);

            btnSavePreset = new Button
            {
                Text = "Save",
                Location = new Point(195, 25),
                Size = new Size(60, 32),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(111, 66, 193), // Purple
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSavePreset.FlatAppearance.BorderSize = 1;
            btnSavePreset.FlatAppearance.BorderColor = Color.FromArgb(111, 66, 193);
            btnSavePreset.Click += BtnSavePreset_Click;
            grpSettings.Controls.Add(btnSavePreset);

            btnLoadPreset = new Button
            {
                Text = "Load",
                Location = new Point(260, 25),
                Size = new Size(60, 32),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(13, 202, 240), // Cyan
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoadPreset.FlatAppearance.BorderSize = 1;
            btnLoadPreset.FlatAppearance.BorderColor = Color.FromArgb(13, 202, 240);
            btnLoadPreset.Click += BtnLoadPreset_Click;
            grpSettings.Controls.Add(btnLoadPreset);

            btnDeletePreset = new Button
            {
                Text = "Delete",
                Location = new Point(325, 25),
                Size = new Size(50, 32),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 53, 69), // Red
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDeletePreset.FlatAppearance.BorderSize = 1;
            btnDeletePreset.FlatAppearance.BorderColor = Color.FromArgb(220, 53, 69);
            btnDeletePreset.Click += BtnDeletePreset_Click;
            grpSettings.Controls.Add(btnDeletePreset);

            btnDiagnostics = new Button
            {
                Text = "Diagnostics",
                Location = new Point(15, 25),
                Size = new Size(110, 32),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(25, 135, 84), // Green
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDiagnostics.FlatAppearance.BorderSize = 1;
            btnDiagnostics.FlatAppearance.BorderColor = Color.FromArgb(25, 135, 84);
            btnDiagnostics.Click += BtnDiagnostics_Click;
            grpSettings.Controls.Add(btnDiagnostics);

            Button btnCheckUpdate = new Button
            {
                Text = "Check Update",
                Location = new Point(380, 25),
                Size = new Size(100, 32),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(13, 110, 253), // Blue
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCheckUpdate.FlatAppearance.BorderSize = 1;
            btnCheckUpdate.FlatAppearance.BorderColor = Color.FromArgb(13, 110, 253);
            btnCheckUpdate.Click += async (s, ev) => 
            {
                await CheckForUpdates(showNoUpdateMessage: true);
            };
            grpSettings.Controls.Add(btnCheckUpdate);

            chkStartWithWindows = new CheckBox
            {
                Text = "Start with Windows",
                Location = new Point(15, 65),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(33, 37, 41),
                BackColor = Color.Transparent,
                Checked = StartupManager.IsStartupEnabled()
            };
            chkStartWithWindows.CheckedChanged += ChkStartWithWindows_CheckedChanged;
            grpSettings.Controls.Add(chkStartWithWindows);

            chkStartMinimized = new CheckBox
            {
                Text = "Start minimized",
                Location = new Point(220, 65),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(33, 37, 41),
                BackColor = Color.Transparent,
                Checked = _appConfig.StartMinimized,
                Enabled = chkStartWithWindows.Checked
            };
            grpSettings.Controls.Add(chkStartMinimized);

            Label lblPresetInfo = new Label
            {
                Text = "Save custom fan curves • Load saved profiles • Delete presets\n" +
                       "F11: Fullscreen | ESC: Exit fullscreen | Presets: %AppData%\\GPUFanController",
                Location = new Point(15, 100),
                Size = new Size(460, 42),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(108, 117, 125), // Gray
                BackColor = Color.Transparent
            };
            grpSettings.Controls.Add(lblPresetInfo);

            this.Controls.Add(grpSettings);

            this.FormClosing += MainForm_FormClosing;
            
            // Intercept close button to minimize to tray instead
            this.FormClosing += MainForm_FormClosingToTray;
        }

        private void InitializeGPUController()
        {
            try
            {
                _multiGPUController = new MultiGPUController();

                if (_multiGPUController.GetGPUCount() > 0)
                {
                    // Populate GPU selector
                    var gpus = _multiGPUController.GetAllGPUs();
                    foreach (var gpu in gpus)
                    {
                        cmbGPUSelect.Items.Add($"GPU {gpu.Index}: {gpu.Name} ({gpu.Type})");
                    }
                    cmbGPUSelect.SelectedIndex = 0;
                    UpdateSelectedGPUInfo();
                }
                else
                {
                    lblGPUName.Text = "GPU: No compatible GPU detected";
                    lblGPUType.Text = "Type: Unknown";
                    MessageBox.Show(
                        "No compatible GPU detected. Please ensure you have:\n\n" +
                        "1. A supported GPU (NVIDIA, AMD, or Intel)\n" +
                        "2. Latest GPU drivers installed\n" +
                        "3. Run this application as Administrator",
                        "GPU Not Detected",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error initializing GPU controller: {ex.Message}\n\n" +
                    "Please run this application as Administrator.",
                    "Initialization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void UpdateSelectedGPUInfo()
        {
            if (_selectedGPUIndex >= 0 && _selectedGPUIndex < _multiGPUController.GetGPUCount())
            {
                var gpu = _multiGPUController.GetGPU(_selectedGPUIndex);
                if (gpu != null)
                {
                    lblGPUName.Text = $"🎮 GPU: {gpu.Name}";
                    lblGPUType.Text = $"📌 Type: {gpu.Type}";
                }
            }
        }

        private void SetupUpdateTimer()
        {
            _updateTimer = new System.Windows.Forms.Timer();
            _updateTimer.Interval = 3000; // Update every 3 seconds for optimal performance
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        private int _updateTickCounter = 0;
        
        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            if (_multiGPUController?.GetGPUCount() > 0)
            {
                try
                {
                    // Get GPU status in background thread to avoid UI blocking
                    Task.Run(() =>
                    {
                        try
                        {
                            var status = _multiGPUController.GetGPUStatus(_selectedGPUIndex);
                            
                            // Update UI on main thread
                            this.BeginInvoke(new Action(() =>
                            {
                                // Batch update UI to reduce redraws
                                this.SuspendLayout();
                                
                                lblTemperature.Text = $"🌡 Temperature: {status.Temperature:F1} °C";
                                lblFanRPM.Text = $"💨 Fan Speed: {status.FanRPM:F0} RPM";
                                lblCurrentSpeed.Text = $"⚡ Fan Control: {status.FanSpeed:F0} %";

                                // Update temperature color warning
                                Color tempColor;
                                if (status.Temperature > 85)
                                    tempColor = Color.FromArgb(255, 69, 0);
                                else if (status.Temperature > 75)
                                    tempColor = Color.FromArgb(255, 140, 0);
                                else if (status.Temperature > 60)
                                    tempColor = Color.FromArgb(255, 215, 0);
                                else
                                    tempColor = Color.FromArgb(0, 255, 127);
                                
                                if (lblTemperature.ForeColor != tempColor)
                                    lblTemperature.ForeColor = tempColor;

                                // Update auto mode status (only if changed)
                                if (_autoControllers.ContainsKey(_selectedGPUIndex) && 
                                    _autoControllers[_selectedGPUIndex].IsRunning)
                                {
                                    var profile = _autoControllers[_selectedGPUIndex].ActiveProfile;
                                    string newStatus = $"Status: Active ({profile.Name} profile)\n" +
                                        $"Target fan speed for {status.Temperature:F1}°C: {profile.GetFanSpeedForTemperature(status.Temperature)}%";
                                    
                                    if (lblAutoStatus.Text != newStatus)
                                    {
                                        lblAutoStatus.Text = newStatus;
                                        lblAutoStatus.ForeColor = Color.Green;
                                    }
                                }

                                this.ResumeLayout(false);
                                
                                // Update system tray less frequently (every 3rd update = 9 seconds)
                                _updateTickCounter++;
                                if (_updateTickCounter % 3 == 0 && _trayManager != null && status.Temperature > 0)
                                {
                                    _trayManager.UpdateTemperature(status.Temperature);
                                }
                            }));
                        }
                        catch
                        {
                            // Silently fail background updates
                        }
                    });
                    
                    // Send analytics heartbeat (throttled internally to once per 5 minutes)
                    // Only check every 10th update to reduce overhead
                    if (_updateTickCounter % 10 == 0)
                    {
                        _ = Task.Run(async () => await AnalyticsService.TrackHeartbeatAsync());
                    }
                }
                catch (Exception ex)
                {
                    lblTemperature.Text = $"Error: {ex.Message}";
                }
            }
        }

        private void CmbGPUSelect_SelectedIndexChanged(object? sender, EventArgs e)
        {
            _selectedGPUIndex = cmbGPUSelect.SelectedIndex;
            UpdateSelectedGPUInfo();
        }

        private void ChkManualControl_CheckedChanged(object? sender, EventArgs e)
        {
            _isManualControl = chkManualControl.Checked;
            fanSpeedSlider.Enabled = _isManualControl;
            btnApply.Enabled = _isManualControl;

            if (_isManualControl)
            {
                // Disable auto mode when enabling manual
                if (chkAutoMode.Checked)
                {
                    chkAutoMode.Checked = false;
                }
            }

            if (!_isManualControl)
            {
                // Reset to auto when unchecked
                _multiGPUController?.SetAutoFanControl(_selectedGPUIndex);
            }
        }

        private void ChkAutoMode_CheckedChanged(object? sender, EventArgs e)
        {
            bool autoEnabled = chkAutoMode.Checked;
            cmbFanProfile.Enabled = autoEnabled;
            btnApplyProfile.Enabled = autoEnabled;

            if (autoEnabled)
            {
                // Disable manual mode when enabling auto
                if (chkManualControl.Checked)
                {
                    chkManualControl.Checked = false;
                }
            }
            else
            {
                // Stop auto controller
                if (_autoControllers.ContainsKey(_selectedGPUIndex))
                {
                    _autoControllers[_selectedGPUIndex].Stop();
                    _autoControllers.Remove(_selectedGPUIndex);
                    lblAutoStatus.Text = "Status: Inactive";
                    lblAutoStatus.ForeColor = Color.Gray;
                }
            }
        }

        private void BtnApplyProfile_Click(object? sender, EventArgs e)
        {
            if (_multiGPUController?.GetGPUCount() > 0 && chkAutoMode.Checked)
            {
                FanCurveProfile? selectedProfile = null;

                switch (cmbFanProfile.SelectedIndex)
                {
                    case 0: selectedProfile = FanCurveProfile.GetSilentProfile(); break;
                    case 1: selectedProfile = FanCurveProfile.GetBalancedProfile(); break;
                    case 2: selectedProfile = FanCurveProfile.GetPerformanceProfile(); break;
                    case 3: selectedProfile = FanCurveProfile.GetAggressiveProfile(); break;
                }

                if (selectedProfile != null)
                {
                    // Create a single GPU controller wrapper
                    var singleGPUController = new GPUController();

                    // Stop existing auto controller if any
                    if (_autoControllers.ContainsKey(_selectedGPUIndex))
                    {
                        _autoControllers[_selectedGPUIndex].Stop();
                        _autoControllers.Remove(_selectedGPUIndex);
                    }

                    // Create and start new auto controller
                    var autoController = new AutoFanController(singleGPUController, selectedProfile);
                    autoController.Start();
                    _autoControllers[_selectedGPUIndex] = autoController;

                    lblAutoStatus.Text = $"Status: Starting ({selectedProfile.Name} profile)...";
                    lblAutoStatus.ForeColor = Color.Green;

                    MessageBox.Show(
                        $"Auto fan control started with '{selectedProfile.Name}' profile.\n\n" +
                        "The fan speed will automatically adjust based on temperature.",
                        "Auto Mode Started",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        private void FanSpeedSlider_ValueChanged(object? sender, EventArgs e)
        {
            lblSliderValue.Text = $"{fanSpeedSlider.Value}%";
        }

        private void BtnApply_Click(object? sender, EventArgs e)
        {
            if (_multiGPUController?.GetGPUCount() > 0 && _isManualControl)
            {
                int fanSpeed = fanSpeedSlider.Value;

                // Safety check
                if (fanSpeed < 30)
                {
                    var result = MessageBox.Show(
                        $"Setting fan speed to {fanSpeed}% is potentially dangerous and may cause overheating!\n\n" +
                        "Are you sure you want to continue?",
                        "Safety Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result != DialogResult.Yes)
                        return;
                }

                bool success = _multiGPUController.SetFanSpeed(_selectedGPUIndex, fanSpeed);

                if (success)
                {
                    MessageBox.Show(
                        $"Fan speed set to {fanSpeed}% on GPU {_selectedGPUIndex}",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    // Get detailed control status
                    string controlStatus = _multiGPUController.GetFanControlStatus(_selectedGPUIndex);
                    bool isLaptop = _multiGPUController.IsLaptopGPU(_selectedGPUIndex);
                    
                    string message = "❌ Failed to set fan speed!\n\n" +
                        $"Reason: {controlStatus}\n\n";
                    
                    if (isLaptop)
                    {
                        message += "💻 Laptop GPU Detected!\n\n" +
                            "Most laptop manufacturers lock fan control for safety and warranty reasons.\n\n" +
                            "Try these alternatives:\n" +
                            "• MSI Afterburner (works on many laptops)\n" +
                            "• Your laptop manufacturer's tool:\n" +
                            "  - ASUS: Armoury Crate\n" +
                            "  - MSI: Dragon Center\n" +
                            "  - Acer: NitroSense\n" +
                            "  - Lenovo: Vantage\n" +
                            "  - HP: Omen Gaming Hub\n" +
                            "  - Dell: Alienware Command Center\n\n" +
                            "Would you like to run diagnostics for more details?";
                    }
                    else
                    {
                        message += "Common reasons:\n" +
                            "• GPU model doesn't support software control\n" +
                            "• Driver doesn't expose fan control\n" +
                            "• Manufacturer-specific control required\n\n" +
                            "Try:\n" +
                            "• Update GPU drivers\n" +
                            "• MSI Afterburner\n" +
                            "• Manufacturer's GPU tool\n\n" +
                            "Would you like to run diagnostics?";
                    }
                    
                    var result = MessageBox.Show(
                        message,
                        isLaptop ? "Laptop GPU Not Supported" : "Fan Control Not Supported",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                    
                    if (result == DialogResult.Yes)
                    {
                        BtnDiagnostics_Click(sender, e);
                    }
                }
            }
        }

        private void BtnAuto_Click(object? sender, EventArgs e)
        {
            // Stop auto controller if running
            if (_autoControllers.ContainsKey(_selectedGPUIndex))
            {
                _autoControllers[_selectedGPUIndex].Stop();
                _autoControllers.Remove(_selectedGPUIndex);
            }

            _multiGPUController?.SetAutoFanControl(_selectedGPUIndex);
            chkManualControl.Checked = false;
            chkAutoMode.Checked = false;
            
            MessageBox.Show(
                $"Fan control reset to automatic mode for GPU {_selectedGPUIndex}.",
                "Auto Mode",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void LoadSettings()
        {
            // Apply saved settings
            if (_appConfig.LastSelectedGPU >= 0 && _appConfig.LastSelectedGPU < cmbGPUSelect.Items.Count)
            {
                cmbGPUSelect.SelectedIndex = _appConfig.LastSelectedGPU;
            }
        }

        private void SaveSettings()
        {
            _appConfig.LastSelectedGPU = _selectedGPUIndex;
            _appConfig.StartMinimized = chkStartMinimized.Checked;
            _appConfig.StartWithWindows = chkStartWithWindows.Checked;
            _presetManager.SaveConfig(_appConfig);
        }

        private void BtnSavePreset_Click(object? sender, EventArgs e)
        {
            // Check if there's an active auto controller or manual setting to save
            FanCurveProfile? profileToSave = null;

            // If auto mode is active, save the current profile
            if (chkAutoMode.Checked && _autoControllers.ContainsKey(_selectedGPUIndex))
            {
                profileToSave = _autoControllers[_selectedGPUIndex].ActiveProfile;
            }
            else if (chkManualControl.Checked)
            {
                // Save manual setting as a flat curve at the selected speed
                int manualSpeed = fanSpeedSlider.Value;
                
                var result = MessageBox.Show(
                    $"Save current manual fan speed ({manualSpeed}%) as a preset?\n\n" +
                    "This will create a flat fan curve at {manualSpeed}%.\n\n" +
                    "Or click 'No' to create a custom temperature-based curve.",
                    "Save Manual Setting",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Cancel)
                    return;

                if (result == DialogResult.Yes)
                {
                    // Create flat curve at manual speed
                    string presetName = Microsoft.VisualBasic.Interaction.InputBox(
                        "Enter a name for this preset:",
                        "Save Manual Preset",
                        $"Manual {manualSpeed}%"
                    );

                    if (string.IsNullOrWhiteSpace(presetName))
                        return;

                    profileToSave = new FanCurveProfile(presetName, new List<CurvePoint>
                    {
                        new CurvePoint(0, manualSpeed),
                        new CurvePoint(100, manualSpeed)
                    });
                }
                else
                {
                    // Create custom curve
                    profileToSave = CreateCustomCurve();
                }
            }
            else
            {
                // No active mode, create custom curve
                var result = MessageBox.Show(
                    "No active fan control mode.\n\n" +
                    "Would you like to create a custom fan curve?",
                    "Create Custom Curve",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    profileToSave = CreateCustomCurve();
                }
                else
                {
                    return;
                }
            }

            if (profileToSave == null)
                return;

            // If we're saving an existing profile, ask for new name
            if (chkAutoMode.Checked && _autoControllers.ContainsKey(_selectedGPUIndex))
            {
                string currentName = profileToSave.Name;
                string presetName = Microsoft.VisualBasic.Interaction.InputBox(
                    $"Saving copy of '{currentName}' profile.\n\nEnter a name for this preset:",
                    "Save Preset",
                    $"{currentName} Copy"
                );

                if (string.IsNullOrWhiteSpace(presetName))
                    return;

                // Create new profile with new name but same points
                profileToSave = new FanCurveProfile(presetName, profileToSave.Points);
            }

            // Save the profile
            if (_presetManager.SavePreset(profileToSave.Name, profileToSave))
            {
                MessageBox.Show(
                    $"✅ Preset '{profileToSave.Name}' saved successfully!\n\n" +
                    "Load it anytime using the 'Load Preset' button.",
                    "Preset Saved",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            else
            {
                MessageBox.Show(
                    "Failed to save preset. Please try again.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private FanCurveProfile? CreateCustomCurve()
        {
            // Show custom curve creator dialog
            Form curveForm = new Form
            {
                Text = "Create Custom Fan Curve",
                Size = new Size(500, 600),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(20, 20, 30),
                ForeColor = Color.FromArgb(0, 255, 255)
            };

            Label lblInstructions = new Label
            {
                Text = "Create your custom fan curve by setting fan speeds at different temperatures.\n" +
                       "The app will automatically interpolate between points.",
                Location = new Point(20, 20),
                Size = new Size(440, 50),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(200, 200, 200)
            };
            curveForm.Controls.Add(lblInstructions);

            // Create 8 temperature/speed pairs
            List<NumericUpDown> tempInputs = new List<NumericUpDown>();
            List<NumericUpDown> speedInputs = new List<NumericUpDown>();

            int[] defaultTemps = { 0, 50, 60, 70, 75, 80, 85, 90 };
            int[] defaultSpeeds = { 30, 40, 50, 65, 75, 85, 95, 100 };

            for (int i = 0; i < 8; i++)
            {
                int yPos = 80 + (i * 50);

                Label lblTemp = new Label
                {
                    Text = $"Point {i + 1}:",
                    Location = new Point(20, yPos + 5),
                    Size = new Size(60, 20),
                    ForeColor = Color.FromArgb(0, 255, 127)
                };
                curveForm.Controls.Add(lblTemp);

                Label lblTempC = new Label
                {
                    Text = "Temp (°C):",
                    Location = new Point(85, yPos + 5),
                    Size = new Size(70, 20),
                    ForeColor = Color.FromArgb(200, 200, 200)
                };
                curveForm.Controls.Add(lblTempC);

                NumericUpDown numTemp = new NumericUpDown
                {
                    Location = new Point(160, yPos),
                    Size = new Size(70, 25),
                    Minimum = 0,
                    Maximum = 100,
                    Value = defaultTemps[i],
                    BackColor = Color.FromArgb(30, 30, 40),
                    ForeColor = Color.White
                };
                tempInputs.Add(numTemp);
                curveForm.Controls.Add(numTemp);

                Label lblSpeed = new Label
                {
                    Text = "Fan (%):",
                    Location = new Point(240, yPos + 5),
                    Size = new Size(60, 20),
                    ForeColor = Color.FromArgb(200, 200, 200)
                };
                curveForm.Controls.Add(lblSpeed);

                NumericUpDown numSpeed = new NumericUpDown
                {
                    Location = new Point(305, yPos),
                    Size = new Size(70, 25),
                    Minimum = 0,
                    Maximum = 100,
                    Value = defaultSpeeds[i],
                    BackColor = Color.FromArgb(30, 30, 40),
                    ForeColor = Color.White
                };
                speedInputs.Add(numSpeed);
                curveForm.Controls.Add(numSpeed);
            }

            TextBox txtName = new TextBox
            {
                Location = new Point(100, 490),
                Size = new Size(270, 25),
                Text = "My Custom Curve",
                BackColor = Color.FromArgb(30, 30, 40),
                ForeColor = Color.White
            };
            curveForm.Controls.Add(txtName);

            Label lblName = new Label
            {
                Text = "Name:",
                Location = new Point(20, 493),
                Size = new Size(50, 20),
                ForeColor = Color.FromArgb(200, 200, 200)
            };
            curveForm.Controls.Add(lblName);

            Button btnSave = new Button
            {
                Text = "Save Curve",
                Location = new Point(180, 525),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 150, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, ev) => curveForm.DialogResult = DialogResult.OK;
            curveForm.Controls.Add(btnSave);

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(290, 525),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, ev) => curveForm.DialogResult = DialogResult.Cancel;
            curveForm.Controls.Add(btnCancel);

            if (curveForm.ShowDialog() == DialogResult.OK)
            {
                List<CurvePoint> points = new List<CurvePoint>();
                for (int i = 0; i < 8; i++)
                {
                    points.Add(new CurvePoint((float)tempInputs[i].Value, (int)speedInputs[i].Value));
                }

                return new FanCurveProfile(txtName.Text, points);
            }

            return null;
        }

        private void BtnLoadPreset_Click(object? sender, EventArgs e)
        {
            var presets = _presetManager.GetPresetNames();

            if (presets.Count == 0)
            {
                MessageBox.Show(
                    "No custom presets found.\n\n" +
                    "Create a preset using the 'Save Preset' button first.",
                    "No Presets",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            // Create selection dialog
            Form presetDialog = new Form
            {
                Text = "Load Custom Preset",
                Size = new Size(350, 250),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            ListBox listBox = new ListBox
            {
                Location = new Point(20, 20),
                Size = new Size(290, 140),
                Font = new Font("Segoe UI", 10)
            };
            listBox.Items.AddRange(presets.ToArray());
            listBox.SelectedIndex = 0;
            presetDialog.Controls.Add(listBox);

            Button btnLoad = new Button
            {
                Text = "Load",
                Location = new Point(130, 170),
                Size = new Size(80, 30),
                DialogResult = DialogResult.OK
            };
            presetDialog.Controls.Add(btnLoad);

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(220, 170),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel
            };
            presetDialog.Controls.Add(btnCancel);

            presetDialog.AcceptButton = btnLoad;
            presetDialog.CancelButton = btnCancel;

            if (presetDialog.ShowDialog() == DialogResult.OK && listBox.SelectedItem != null)
            {
                string selectedPreset = listBox.SelectedItem.ToString() ?? "";
                var profile = _presetManager.LoadPreset(selectedPreset);

                if (profile != null)
                {
                    // Check if this is a flat curve (manual preset)
                    bool isFlatCurve = profile.Points.Count >= 2 && 
                                       profile.Points.All(p => p.FanSpeed == profile.Points[0].FanSpeed);

                    if (isFlatCurve)
                    {
                        // This is a manual/flat curve preset - ask user how to apply it
                        int flatSpeed = profile.Points[0].FanSpeed;
                        var applyChoice = MessageBox.Show(
                            $"This preset '{selectedPreset}' has a flat fan curve at {flatSpeed}%.\n\n" +
                            "How would you like to apply it?\n\n" +
                            "• Yes = Apply as Manual Control ({flatSpeed}%)\n" +
                            "• No = Apply as Auto Mode (temperature-based curve)\n" +
                            "• Cancel = Don't apply",
                            "Load Manual Preset",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question
                        );

                        if (applyChoice == DialogResult.Cancel)
                            return;

                        if (applyChoice == DialogResult.Yes)
                        {
                            // Apply as manual control
                            chkManualControl.Checked = true;
                            fanSpeedSlider.Value = flatSpeed;
                            
                            // Actually set the fan speed
                            bool success = _multiGPUController.SetFanSpeed(_selectedGPUIndex, flatSpeed);
                            
                            if (success)
                            {
                                MessageBox.Show(
                                    $"✅ Preset '{selectedPreset}' loaded successfully!\n\n" +
                                    $"Manual fan control set to {flatSpeed}%.",
                                    "Preset Loaded",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information
                                );
                            }
                            else
                            {
                                MessageBox.Show(
                                    $"⚠️ Preset loaded but failed to set fan speed.\n\n" +
                                    "Your GPU may not support manual fan control.",
                                    "Partial Success",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                );
                            }
                            return;
                        }
                        // If No, fall through to auto mode
                    }

                    // Apply as auto mode (for temperature-based curves or if user chose No)
                    // Create single GPU controller wrapper
                    var singleGPUController = new GPUController();

                    // Stop existing auto controller if any
                    if (_autoControllers.ContainsKey(_selectedGPUIndex))
                    {
                        _autoControllers[_selectedGPUIndex].Stop();
                        _autoControllers.Remove(_selectedGPUIndex);
                    }

                    // Start with loaded profile
                    var autoController = new AutoFanController(singleGPUController, profile);
                    autoController.Start();
                    _autoControllers[_selectedGPUIndex] = autoController;

                    chkAutoMode.Checked = true;
                    lblAutoStatus.Text = $"Status: Active ({profile.Name} profile - Custom)";
                    lblAutoStatus.ForeColor = Color.Green;

                    MessageBox.Show(
                        $"✅ Loaded preset '{selectedPreset}' and started auto mode.",
                        "Preset Loaded",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }

        private void BtnDeletePreset_Click(object? sender, EventArgs e)
        {
            var presets = _presetManager.GetPresetNames();

            if (presets.Count == 0)
            {
                MessageBox.Show(
                    "No custom presets to delete.",
                    "No Presets",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            // Create selection dialog
            Form deleteDialog = new Form
            {
                Text = "Delete Custom Preset",
                Size = new Size(350, 250),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            ListBox listBox = new ListBox
            {
                Location = new Point(20, 20),
                Size = new Size(290, 140),
                Font = new Font("Segoe UI", 10)
            };
            listBox.Items.AddRange(presets.ToArray());
            listBox.SelectedIndex = 0;
            deleteDialog.Controls.Add(listBox);

            Button btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(130, 170),
                Size = new Size(80, 30),
                DialogResult = DialogResult.OK
            };
            deleteDialog.Controls.Add(btnDelete);

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(220, 170),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel
            };
            deleteDialog.Controls.Add(btnCancel);

            deleteDialog.AcceptButton = btnDelete;
            deleteDialog.CancelButton = btnCancel;

            if (deleteDialog.ShowDialog() == DialogResult.OK && listBox.SelectedItem != null)
            {
                string selectedPreset = listBox.SelectedItem.ToString() ?? "";
                
                var result = MessageBox.Show(
                    $"Are you sure you want to delete preset '{selectedPreset}'?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    if (_presetManager.DeletePreset(selectedPreset))
                    {
                        MessageBox.Show(
                            $"Preset '{selectedPreset}' deleted successfully.",
                            "Preset Deleted",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to delete preset.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        private void BtnDiagnostics_Click(object? sender, EventArgs e)
        {
            try
            {
                using (var diagnostics = new GPUDiagnostics())
                {
                    string report = diagnostics.GetFullDiagnosticReport();

                    // Show in a new window
                    Form diagForm = new Form
                    {
                        Text = "GPU Diagnostics Report",
                        Size = new Size(700, 600),
                        StartPosition = FormStartPosition.CenterParent,
                        BackColor = Color.FromArgb(15, 15, 15),
                        ForeColor = Color.FromArgb(0, 255, 255)
                    };

                    TextBox txtReport = new TextBox
                    {
                        Multiline = true,
                        ScrollBars = ScrollBars.Both,
                        Dock = DockStyle.Fill,
                        Font = new Font("Consolas", 9),
                        Text = report,
                        ReadOnly = true,
                        BackColor = Color.FromArgb(20, 20, 20),
                        ForeColor = Color.FromArgb(0, 255, 127),
                        BorderStyle = BorderStyle.None
                    };
                    diagForm.Controls.Add(txtReport);

                    Button btnCopy = new Button
                    {
                        Text = "📋 Copy to Clipboard",
                        Dock = DockStyle.Bottom,
                        Height = 40,
                        BackColor = Color.FromArgb(0, 150, 200),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI", 10, FontStyle.Bold)
                    };
                    btnCopy.FlatAppearance.BorderSize = 0;
                    btnCopy.Click += (s, ev) =>
                    {
                        System.Windows.Forms.Clipboard.SetText(report);
                        MessageBox.Show("Diagnostic report copied to clipboard!", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    };
                    diagForm.Controls.Add(btnCopy);

                    diagForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error running diagnostics: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void ChkStartWithWindows_CheckedChanged(object? sender, EventArgs e)
        {
            bool enabled = chkStartWithWindows.Checked;
            chkStartMinimized.Enabled = enabled;

            if (enabled)
            {
                string exePath = StartupManager.GetExecutablePath();
                bool success = StartupManager.EnableStartup(exePath, chkStartMinimized.Checked);

                if (success)
                {
                    MessageBox.Show(
                        "GPU Fan Controller will now start automatically when Windows starts.",
                        "Auto-Start Enabled",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    chkStartWithWindows.Checked = false;
                    MessageBox.Show(
                        "Failed to enable auto-start.\n\n" +
                        "Please make sure the application is running as Administrator.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            else
            {
                bool success = StartupManager.DisableStartup();
                if (success)
                {
                    MessageBox.Show(
                        "Auto-start disabled. GPU Fan Controller will no longer start with Windows.",
                        "Auto-Start Disabled",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }

            SaveSettings();
        }

        private bool _isReallyClosing = false;

        public void ReallyClose()
        {
            _isReallyClosing = true;
            this.Close();
            Application.Exit();
        }

        private bool _hasShownMinimizeNotification = false;

        private void MainForm_FormClosingToTray(object? sender, FormClosingEventArgs e)
        {
            // If user clicks X, minimize to tray instead of closing
            if (!_isReallyClosing && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                _trayManager?.HideToTray();
                
                // Show notification only once per session
                if (!_hasShownMinimizeNotification)
                {
                    _trayManager?.ShowBalloonTip(
                        "GPU Fan Controller",
                        "Minimized to tray. Double-click to restore.",
                        ToolTipIcon.Info
                    );
                    _hasShownMinimizeNotification = true;
                }
            }
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // Only actually close if _isReallyClosing is true
            if (_isReallyClosing)
            {
                // Save settings before closing
                SaveSettings();

                _updateTimer?.Stop();
                
                // Stop all auto controllers
                foreach (var controller in _autoControllers.Values)
                {
                    controller.Stop();
                    controller.Dispose();
                }
                _autoControllers.Clear();

                _multiGPUController?.Dispose();
                _trayManager?.Dispose();
            }
        }
    }
}
