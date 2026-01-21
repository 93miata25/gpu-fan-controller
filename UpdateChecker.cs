using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;

namespace GPUFanController
{
    public class UpdateInfo
    {
        public string Version { get; set; } = "";
        public string DownloadUrl { get; set; } = "";
        public string DownloadUrlWindows { get; set; } = "";
        public string DownloadUrlLinux { get; set; } = "";
        public string ReleaseNotes { get; set; } = "";
        public string ReleaseDate { get; set; } = "";
        public bool IsCritical { get; set; } = false;
        public AnalyticsConfig? Analytics { get; set; }

        /// <summary>
        /// Get the appropriate download URL for the current platform
        /// </summary>
        public string GetPlatformDownloadUrl()
        {
            if (OperatingSystem.IsLinux() && !string.IsNullOrEmpty(DownloadUrlLinux))
            {
                return DownloadUrlLinux;
            }
            else if (OperatingSystem.IsWindows() && !string.IsNullOrEmpty(DownloadUrlWindows))
            {
                return DownloadUrlWindows;
            }
            else
            {
                // Fallback to generic DownloadUrl
                return DownloadUrl;
            }
        }
    }

    public class AnalyticsConfig
    {
        public string MeasurementId { get; set; } = "";
        public string ApiSecret { get; set; } = "";
    }

    public class UpdateChecker
    {
        // Current version of the application
        public static readonly string CurrentVersion = "2.3.2";

        // URL to your version.json file hosted on your website
        private static string _updateCheckUrl = "";

        /// <summary>
        /// Set the URL where version.json is hosted (e.g., on your Hostinger website)
        /// </summary>
        public static void SetUpdateUrl(string versionJsonUrl)
        {
            _updateCheckUrl = versionJsonUrl;
        }

        public static async Task<UpdateInfo?> CheckForUpdatesAsync()
        {
            if (string.IsNullOrEmpty(_updateCheckUrl))
                return null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    
                    string json = await client.GetStringAsync(_updateCheckUrl);
                    var updateInfo = JsonSerializer.Deserialize<UpdateInfo>(json);

                    // Configure analytics if present in version.json
                    if (updateInfo?.Analytics != null && 
                        !string.IsNullOrEmpty(updateInfo.Analytics.MeasurementId) &&
                        !string.IsNullOrEmpty(updateInfo.Analytics.ApiSecret))
                    {
                        AnalyticsService.Configure(
                            updateInfo.Analytics.MeasurementId,
                            updateInfo.Analytics.ApiSecret
                        );
                    }

                    if (updateInfo != null && IsNewerVersion(updateInfo.Version, CurrentVersion))
                    {
                        return updateInfo;
                    }
                }
            }
            catch (Exception)
            {
                // Silently fail if update check fails (no internet, etc.)
            }

            return null;
        }

        public static bool IsNewerVersion(string newVersion, string currentVersion)
        {
            try
            {
                var newVer = new Version(newVersion);
                var curVer = new Version(currentVersion);
                return newVer > curVer;
            }
            catch
            {
                return false;
            }
        }

        public static void OpenDownloadUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception)
            {
                // Ignore errors
            }
        }
    }
}
