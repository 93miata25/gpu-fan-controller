using System;

namespace GPUFanController
{
    class ProgramConsole
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            Console.Title = "GPU Fan Controller - Console Edition";
            
            try
            {
                // Initialize analytics (silently)
                await InitializeAnalyticsAsync();
                
                var app = new ConsoleApp();
                app.Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Fatal Error: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("\nPlease ensure:");
                Console.WriteLine("  1. Running as Administrator");
                Console.WriteLine("  2. GPU drivers are up to date");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }

        static async System.Threading.Tasks.Task InitializeAnalyticsAsync()
        {
            try
            {
                // Configure analytics with environment variables (secure for open source)
                var gaId = Environment.GetEnvironmentVariable("GA_MEASUREMENT_ID");
                var gaSecret = Environment.GetEnvironmentVariable("GA_API_SECRET");
                if (!string.IsNullOrEmpty(gaId) && !string.IsNullOrEmpty(gaSecret))
                {
                    AnalyticsService.Configure(gaId, gaSecret);
                }
                
                // Track install and app start
                await AnalyticsService.TrackInstallAsync();
                await AnalyticsService.TrackAppStartAsync();
            }
            catch
            {
                // Silently fail - analytics should never break the app
            }
        }
    }
}
