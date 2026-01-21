using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;
using System.Linq;

namespace GPUFanController
{
    public class GitHubRelease
    {
        public string tag_name { get; set; } = "";
        public string name { get; set; } = "";
        public string body { get; set; } = "";
        public string published_at { get; set; } = "";
        public bool prerelease { get; set; }
        public GitHubAsset[] assets { get; set; } = Array.Empty<GitHubAsset>();
    }

    public class GitHubAsset
    {
        public string name { get; set; } = "";
        public string browser_download_url { get; set; } = "";
        public long size { get; set; }
        public int download_count { get; set; }
    }

    public class UpdateInfo
    {
        public string Version { get; set; } = "";
        public string DownloadUrl { get; set; } = "";
        public string DownloadUrlWindows { get; set; } = "";
        public string DownloadUrlLinux { get; set; } = "";
        public string ReleaseNotes { get; set; } = "";
        public string ReleaseDate { get; set; } = "";
        public bool IsCritical { get; set; } = false;

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

    public class UpdateChecker
    {
        // Current version of the application
        public static readonly string CurrentVersion = "2.3.2";

        // GitHub repository for releases
        private const string GitHubApiUrl = "https://api.github.com/repos/93miata25/gpu-fan-controller/releases/latest";

        public static async Task<UpdateInfo?> CheckForUpdatesAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    
                    // GitHub requires User-Agent header
                    client.DefaultRequestHeaders.Add("User-Agent", "GPUFanController");
                    
                    string json = await client.GetStringAsync(GitHubApiUrl);
                    var release = JsonSerializer.Deserialize<GitHubRelease>(json);

                    if (release != null && !release.prerelease)
                    {
                        // Extract version from tag (e.g., "v2.3.3" -> "2.3.3")
                        string version = release.tag_name.TrimStart('v');

                        if (IsNewerVersion(version, CurrentVersion))
                        {
                            // Find Windows and Linux download URLs from assets
                            string windowsUrl = "";
                            string linuxUrl = "";

                            foreach (var asset in release.assets)
                            {
                                if (asset.name.Contains("Setup", StringComparison.OrdinalIgnoreCase) && 
                                    asset.name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                                {
                                    windowsUrl = asset.browser_download_url;
                                }
                                else if (asset.name.Contains("linux", StringComparison.OrdinalIgnoreCase) &&
                                        (asset.name.EndsWith(".tar.gz") || asset.name.EndsWith(".zip")))
                                {
                                    linuxUrl = asset.browser_download_url;
                                }
                            }

                            // Check if release notes indicate critical update
                            bool isCritical = release.body?.Contains("[CRITICAL]", StringComparison.OrdinalIgnoreCase) ?? false;

                            return new UpdateInfo
                            {
                                Version = version,
                                DownloadUrl = windowsUrl,
                                DownloadUrlWindows = windowsUrl,
                                DownloadUrlLinux = linuxUrl,
                                ReleaseNotes = release.body ?? "",
                                ReleaseDate = DateTime.Parse(release.published_at).ToString("yyyy-MM-dd"),
                                IsCritical = isCritical
                            };
                        }
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
