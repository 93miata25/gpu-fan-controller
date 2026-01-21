using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace GPUFanController
{
    public class SystemTrayManager : IDisposable
    {
        private NotifyIcon _trayIcon;
        private ContextMenuStrip _contextMenu;
        private Form _mainForm;
        private Font _temperatureFont;
        private Brush _textBrush;
        private Brush _backgroundBrush;

        public SystemTrayManager(Form mainForm)
        {
            _mainForm = mainForm;
            _temperatureFont = new Font("Segoe UI", 7, FontStyle.Bold);
            _textBrush = new SolidBrush(Color.White);
            _backgroundBrush = new SolidBrush(Color.FromArgb(0, 191, 255)); // Cyan

            CreateTrayIcon();
            CreateContextMenu();
        }

        private void CreateTrayIcon()
        {
            _trayIcon = new NotifyIcon
            {
                Visible = true,
                Text = "GPU Fan Controller",
                Icon = CreateTemperatureIcon(0) // Initial icon
            };

            _trayIcon.DoubleClick += (s, e) => ShowMainForm();
        }

        private void CreateContextMenu()
        {
            _contextMenu = new ContextMenuStrip();

            var showItem = new ToolStripMenuItem("Show Window");
            showItem.Click += (s, e) => ShowMainForm();
            _contextMenu.Items.Add(showItem);

            _contextMenu.Items.Add(new ToolStripSeparator());

            var exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => 
            {
                // Set flag to actually close the app
                var mainForm = _mainForm as MainForm;
                if (mainForm != null)
                {
                    mainForm.ReallyClose();
                }
                else
                {
                    _mainForm.Close();
                    Application.Exit();
                }
            };
            _contextMenu.Items.Add(exitItem);

            _trayIcon.ContextMenuStrip = _contextMenu;
        }

        public void UpdateTemperature(float temperature)
        {
            try
            {
                // Update icon with current temperature
                var oldIcon = _trayIcon.Icon;
                _trayIcon.Icon = CreateTemperatureIcon(temperature);
                oldIcon?.Dispose();

                // Update tooltip
                string tempColor = temperature >= 85 ? "🔴" : temperature >= 75 ? "🟠" : temperature >= 60 ? "🟡" : "🟢";
                _trayIcon.Text = $"GPU: {temperature:F1}°C {tempColor}";
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                System.Diagnostics.Debug.WriteLine($"Error updating tray icon: {ex.Message}");
            }
        }

        private Icon CreateTemperatureIcon(float temperature)
        {
            // Create a bitmap for the icon
            Bitmap bitmap = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Determine color based on temperature
                Color bgColor = GetTemperatureColor(temperature);
                using (Brush bgBrush = new SolidBrush(bgColor))
                {
                    g.FillEllipse(bgBrush, 2, 2, 28, 28);
                }

                // Draw temperature text
                string tempText = temperature > 0 ? $"{temperature:F0}" : "--";
                using (Font font = new Font("Arial", 9, FontStyle.Bold))
                {
                    SizeF textSize = g.MeasureString(tempText, font);
                    float x = (32 - textSize.Width) / 2;
                    float y = (32 - textSize.Height) / 2;
                    g.DrawString(tempText, font, Brushes.White, x, y);
                }
            }

            // Convert bitmap to icon
            IntPtr hIcon = bitmap.GetHicon();
            Icon icon = Icon.FromHandle(hIcon);
            
            // Clean up
            bitmap.Dispose();
            
            return icon;
        }

        private Color GetTemperatureColor(float temperature)
        {
            if (temperature < 50) return Color.FromArgb(0, 191, 255); // Cyan - Cool
            if (temperature < 60) return Color.FromArgb(0, 255, 127); // Green - Good
            if (temperature < 70) return Color.FromArgb(255, 215, 0); // Gold - Warm
            if (temperature < 80) return Color.FromArgb(255, 140, 0); // Orange - Hot
            return Color.FromArgb(255, 69, 0); // Red - Very Hot
        }

        private void ShowMainForm()
        {
            _mainForm.Show();
            _mainForm.WindowState = FormWindowState.Normal;
            _mainForm.Activate();
        }

        public void HideToTray()
        {
            _mainForm.Hide();
        }

        public void ShowBalloonTip(string title, string text, ToolTipIcon icon = ToolTipIcon.Info)
        {
            _trayIcon.ShowBalloonTip(3000, title, text, icon);
        }

        public void Dispose()
        {
            _trayIcon?.Dispose();
            _contextMenu?.Dispose();
            _temperatureFont?.Dispose();
            _textBrush?.Dispose();
            _backgroundBrush?.Dispose();
        }
    }
}
