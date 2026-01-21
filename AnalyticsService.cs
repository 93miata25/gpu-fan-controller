using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace GPUFanController
{
    /// <summary>
    /// Analytics service for tracking installs and active users using Google Analytics 4
    /// Privacy-focused: Uses anonymous device ID, no personal information collected
    /// </summary>
    public class AnalyticsService
    {
        private static string? _measurementId;
        private static string? _apiSecret;
        private static string? _clientId;
        private static bool _isEnabled = true;
        private static readonly string _settingsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GPUFanController",
            "analytics.dat"
        );
        private static string? _sessionId;
        private static DateTime _lastHeartbeatTime = DateTime.MinValue;
        private static readonly TimeSpan _heartbeatInterval = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Configure analytics with Google Analytics 4 credentials
        /// </summary>
        public static void Configure(string measurementId, string apiSecret)
        {
            _measurementId = measurementId;
            _apiSecret = apiSecret;
            _clientId = GetOrCreateClientId();
        }

        /// <summary>
        /// Track application install (first run)
        /// </summary>
        public static async Task TrackInstallAsync()
        {
            WriteLogFile($"TrackInstallAsync called - Enabled: {_isEnabled}, MeasurementId set: {!string.IsNullOrEmpty(_measurementId)}");
            
            if (!_isEnabled || string.IsNullOrEmpty(_measurementId))
            {
                WriteLogFile($"TrackInstallAsync skipped - Enabled: {_isEnabled}, MeasurementId: {!string.IsNullOrEmpty(_measurementId)}");
                return;
            }

            try
            {
                var installFile = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "GPUFanController",
                    ".installed"
                );

                WriteLogFile($"Checking install marker file: {installFile}");
                
                // Only track if this is the first run
                if (!File.Exists(installFile))
                {
                    WriteLogFile($"First run detected - sending app_install event");
                    await SendEventAsync("app_install", new
                    {
                        app_version = UpdateChecker.CurrentVersion,
                        os_version = Environment.OSVersion.ToString()
                    });

                    // Create marker file to prevent duplicate install tracking
                    Directory.CreateDirectory(Path.GetDirectoryName(installFile)!);
                    File.WriteAllText(installFile, DateTime.UtcNow.ToString("o"));
                    WriteLogFile($"Install marker file created");
                }
                else
                {
                    WriteLogFile($"Install already tracked (marker file exists)");
                }
            }
            catch (Exception ex)
            {
                WriteLogFile($"TrackInstallAsync error: {ex.Message}");
            }
        }

        /// <summary>
        /// Track application start (active user)
        /// </summary>
        public static async Task TrackAppStartAsync()
        {
            WriteLogFile($"TrackAppStartAsync called - Enabled: {_isEnabled}, MeasurementId set: {!string.IsNullOrEmpty(_measurementId)}");
            
            if (!_isEnabled || string.IsNullOrEmpty(_measurementId))
            {
                WriteLogFile($"TrackAppStartAsync skipped - Enabled: {_isEnabled}, MeasurementId: {!string.IsNullOrEmpty(_measurementId)}");
                return;
            }

            try
            {
                _sessionId = GenerateSessionId();
                _lastHeartbeatTime = DateTime.UtcNow;
                
                WriteLogFile($"Sending app_start event - Version: {UpdateChecker.CurrentVersion}, SessionId: {_sessionId}");
                await SendEventAsync("app_start", new
                {
                    app_version = UpdateChecker.CurrentVersion,
                    session_id = _sessionId
                });
                WriteLogFile($"app_start event sent successfully");
            }
            catch (Exception ex)
            {
                WriteLogFile($"TrackAppStartAsync error: {ex.Message}");
            }
        }

        /// <summary>
        /// Send periodic heartbeat to track active users
        /// This should be called regularly (e.g., every 5 minutes) to show user is still active
        /// Works even when app is minimized to tray
        /// </summary>
        public static async Task TrackHeartbeatAsync()
        {
            // ALWAYS log that function was called
            WriteLogFile($"TrackHeartbeatAsync called - Enabled: {_isEnabled}, MeasurementId set: {!string.IsNullOrEmpty(_measurementId)}");
            
            if (!_isEnabled || string.IsNullOrEmpty(_measurementId))
            {
                System.Diagnostics.Debug.WriteLine($"[Analytics] Heartbeat skipped - Enabled: {_isEnabled}, MeasurementId: {!string.IsNullOrEmpty(_measurementId)}");
                WriteLogFile($"Heartbeat skipped - Enabled: {_isEnabled}, MeasurementId: {!string.IsNullOrEmpty(_measurementId)}");
                return;
            }

            try
            {
                var timeSinceLastHeartbeat = DateTime.UtcNow - _lastHeartbeatTime;
                System.Diagnostics.Debug.WriteLine($"[Analytics] Heartbeat check - Time since last: {timeSinceLastHeartbeat.TotalMinutes:F2} minutes");
                WriteLogFile($"Heartbeat check - Time since last: {timeSinceLastHeartbeat.TotalMinutes:F2} minutes");
                
                // Only send heartbeat if enough time has passed
                if (timeSinceLastHeartbeat < _heartbeatInterval)
                {
                    System.Diagnostics.Debug.WriteLine($"[Analytics] Heartbeat throttled - Need {(_heartbeatInterval - timeSinceLastHeartbeat).TotalSeconds:F0} more seconds");
                    WriteLogFile($"Heartbeat throttled - Need {(_heartbeatInterval - timeSinceLastHeartbeat).TotalSeconds:F0} more seconds");
                    return;
                }

                _lastHeartbeatTime = DateTime.UtcNow;
                System.Diagnostics.Debug.WriteLine($"[Analytics] Sending heartbeat at {DateTime.UtcNow:HH:mm:ss}");
                WriteLogFile($"Sending heartbeat at {DateTime.UtcNow:HH:mm:ss}");

                await SendEventAsync("app_heartbeat", new
                {
                    app_version = UpdateChecker.CurrentVersion,
                    session_id = _sessionId ?? GenerateSessionId(),
                    engagement_time_msec = (int)_heartbeatInterval.TotalMilliseconds
                });
                
                System.Diagnostics.Debug.WriteLine($"[Analytics] Heartbeat sent successfully!");
                WriteLogFile($"Heartbeat sent successfully at {DateTime.UtcNow:HH:mm:ss}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Analytics] Heartbeat error: {ex.Message}");
                WriteLogFile($"Heartbeat error: {ex.Message}");
            }
        }

        /// <summary>
        /// Track custom event with optional parameters
        /// </summary>
        public static async Task TrackEventAsync(string eventName, object? parameters = null)
        {
            if (!_isEnabled || string.IsNullOrEmpty(_measurementId)) return;

            try
            {
                await SendEventAsync(eventName, parameters);
            }
            catch (Exception)
            {
                // Silently fail
            }
        }

        /// <summary>
        /// Send event to Google Analytics 4 Measurement Protocol
        /// </summary>
        private static async Task SendEventAsync(string eventName, object? parameters)
        {
            WriteLogFile($"SendEventAsync called - Event: {eventName}");
            
            if (string.IsNullOrEmpty(_measurementId) || string.IsNullOrEmpty(_apiSecret) || string.IsNullOrEmpty(_clientId))
            {
                WriteLogFile($"SendEvent skipped - MeasurementId: {!string.IsNullOrEmpty(_measurementId)}, ApiSecret: {!string.IsNullOrEmpty(_apiSecret)}, ClientId: {!string.IsNullOrEmpty(_clientId)}");
                System.Diagnostics.Debug.WriteLine($"[Analytics] SendEvent skipped - Missing config");
                return;
            }

            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);

                var url = $"https://www.google-analytics.com/mp/collect?measurement_id={_measurementId}&api_secret={_apiSecret}";

                var payload = new
                {
                    client_id = _clientId,
                    events = new[]
                    {
                        new
                        {
                            name = eventName,
                            @params = parameters ?? new { }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                WriteLogFile($"Sending HTTP POST to GA4 - Event: {eventName}, Payload size: {json.Length} bytes");
                System.Diagnostics.Debug.WriteLine($"[Analytics] Sending {eventName} event to GA4");
                var response = await client.PostAsync(url, content);
                WriteLogFile($"GA4 Response - Event: {eventName}, Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[Analytics] Response: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                WriteLogFile($"SendEvent error - Event: {eventName}, Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[Analytics] SendEvent error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get or create a unique anonymous client ID for this installation
        /// </summary>
        private static string GetOrCreateClientId()
        {
            try
            {
                if (File.Exists(_settingsFile))
                {
                    var data = File.ReadAllText(_settingsFile);
                    if (!string.IsNullOrEmpty(data))
                        return data;
                }

                // Generate new anonymous ID
                var clientId = Guid.NewGuid().ToString();

                // Save for future use
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsFile)!);
                File.WriteAllText(_settingsFile, clientId);

                return clientId;
            }
            catch (Exception)
            {
                // If file operations fail, generate temporary ID
                return Guid.NewGuid().ToString();
            }
        }

        /// <summary>
        /// Generate a unique session ID for this app session
        /// </summary>
        private static string GenerateSessionId()
        {
            return $"{DateTime.UtcNow.Ticks}";
        }

        /// <summary>
        /// Check if analytics is enabled
        /// </summary>
        public static bool IsEnabled => _isEnabled;

        /// <summary>
        /// Enable or disable analytics
        /// </summary>
        public static void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
        }

        /// <summary>
        /// Write debug log to file for troubleshooting
        /// </summary>
        private static void WriteLogFile(string message)
        {
            try
            {
                var logFile = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "GPUFanController",
                    "analytics_debug.log"
                );
                
                Directory.CreateDirectory(Path.GetDirectoryName(logFile)!);
                File.AppendAllText(logFile, $"[{DateTime.Now:HH:mm:ss}] {message}\n");
            }
            catch
            {
                // Ignore log errors
            }
        }
    }
}
