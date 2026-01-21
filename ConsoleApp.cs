using System;
using System.Collections.Generic;
using System.Threading;

namespace GPUFanController
{
    public class ConsoleApp
    {
        private MultiGPUController _multiGPUController;
        private Dictionary<int, AutoFanController> _autoControllers;
        private bool _running = true;

        public ConsoleApp()
        {
            _multiGPUController = new MultiGPUController();
            _autoControllers = new Dictionary<int, AutoFanController>();
        }

        public void Run()
        {
            Console.Clear();
            PrintHeader();

            if (_multiGPUController.GetGPUCount() == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n❌ No compatible GPU detected!");
                Console.ResetColor();
                Console.WriteLine("\nPlease ensure:");
                Console.WriteLine("  1. You have a supported GPU (NVIDIA, AMD, or Intel)");
                Console.WriteLine("  2. Latest GPU drivers are installed");
                Console.WriteLine("  3. Running as Administrator");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            ShowMainMenu();
        }

        private void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║           GPU Fan Controller - Console Edition            ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        private void ShowMainMenu()
        {
            while (_running)
            {
                Console.Clear();
                PrintHeader();
                
                Console.WriteLine("\n📊 Detected GPUs:");
                var gpus = _multiGPUController.GetAllGPUs();
                foreach (var gpu in gpus)
                {
                    var status = _multiGPUController.GetGPUStatus(gpu.Index);
                    Console.ForegroundColor = GetTemperatureColor(status.Temperature);
                    Console.WriteLine($"  [{gpu.Index}] {gpu.Name} ({gpu.Type})");
                    Console.WriteLine($"      Temp: {status.Temperature:F1}°C | Fan: {status.FanSpeed:F0}% ({status.FanRPM:F0} RPM)");
                    Console.ResetColor();
                }

                Console.WriteLine("\n═══════════════════════════════════════════════════════════");
                Console.WriteLine("Main Menu:");
                Console.WriteLine("  [1] Monitor All GPUs");
                Console.WriteLine("  [2] Manual Fan Control");
                Console.WriteLine("  [3] Auto Fan Control (Fan Curves)");
                Console.WriteLine("  [4] Reset All to Auto");
                Console.WriteLine("  [5] View GPU Details");
                Console.WriteLine("  [0] Exit");
                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.Write("\nSelect option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        MonitorAllGPUs();
                        break;
                    case "2":
                        ManualFanControl();
                        break;
                    case "3":
                        AutoFanControl();
                        break;
                    case "4":
                        ResetAllToAuto();
                        break;
                    case "5":
                        ViewGPUDetails();
                        break;
                    case "0":
                        _running = false;
                        break;
                    default:
                        Console.WriteLine("\nInvalid option. Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }

            Cleanup();
        }

        private void MonitorAllGPUs()
        {
            Console.Clear();
            PrintHeader();
            Console.WriteLine("\n📊 Real-time GPU Monitoring (Press ESC to return)\n");

            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                    break;

                Console.SetCursorPosition(0, 5);
                
                var statuses = _multiGPUController.GetAllGPUStatus();
                foreach (var status in statuses)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"GPU {status.Index}: ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{status.Name,-40}");
                    
                    Console.ForegroundColor = GetTemperatureColor(status.Temperature);
                    Console.WriteLine($"  Temperature: {status.Temperature:F1}°C      ");
                    
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"  Fan Speed:   {status.FanSpeed:F0}%       ");
                    Console.WriteLine($"  Fan RPM:     {status.FanRPM:F0} RPM     ");
                    Console.ResetColor();
                    Console.WriteLine();
                }

                Console.WriteLine($"\nLast Update: {DateTime.Now:HH:mm:ss}     ");
                Thread.Sleep(1000);
            }
        }

        private void ManualFanControl()
        {
            Console.Clear();
            PrintHeader();
            Console.WriteLine("\n🎛️  Manual Fan Control\n");

            // Select GPU
            int gpuIndex = SelectGPU();
            if (gpuIndex == -1) return;

            // Stop auto controller if running
            if (_autoControllers.ContainsKey(gpuIndex))
            {
                _autoControllers[gpuIndex].Stop();
                _autoControllers.Remove(gpuIndex);
            }

            Console.Write("\nEnter fan speed (0-100%): ");
            if (int.TryParse(Console.ReadLine(), out int fanSpeed))
            {
                if (fanSpeed < 0 || fanSpeed > 100)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Fan speed must be between 0 and 100%");
                    Console.ResetColor();
                }
                else if (fanSpeed < 30)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n⚠️  WARNING: Setting fan speed to {fanSpeed}% is dangerous!");
                    Console.WriteLine("This may cause overheating and hardware damage.");
                    Console.Write("Are you sure? (yes/no): ");
                    if (Console.ReadLine()?.ToLower() != "yes")
                    {
                        Console.WriteLine("Operation cancelled.");
                        Console.ReadKey();
                        return;
                    }
                    Console.ResetColor();
                }

                bool success = _multiGPUController.SetFanSpeed(gpuIndex, fanSpeed);
                if (success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n✅ Fan speed set to {fanSpeed}% on GPU {gpuIndex}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n❌ Failed to set fan speed. GPU may not support manual control.");
                }
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("\n❌ Invalid input.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void AutoFanControl()
        {
            Console.Clear();
            PrintHeader();
            Console.WriteLine("\n🤖 Auto Fan Control (Fan Curves)\n");

            // Select GPU
            int gpuIndex = SelectGPU();
            if (gpuIndex == -1) return;

            // Select profile
            Console.WriteLine("\nAvailable Fan Curve Profiles:");
            var profiles = FanCurveProfile.GetAllPresets();
            for (int i = 0; i < profiles.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {profiles[i].Name}");
                foreach (var point in profiles[i].Points)
                {
                    Console.WriteLine($"      {point}");
                }
                Console.WriteLine();
            }

            Console.Write("Select profile (1-4): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 4)
            {
                var selectedProfile = profiles[choice - 1];

                // Create single GPU controller for this GPU
                var singleGPUController = new GPUController();
                
                // Stop existing auto controller if any
                if (_autoControllers.ContainsKey(gpuIndex))
                {
                    _autoControllers[gpuIndex].Stop();
                    _autoControllers.Remove(gpuIndex);
                }

                // Create and start new auto controller
                var autoController = new AutoFanController(singleGPUController, selectedProfile);
                autoController.FanSpeedAdjusted += (s, e) =>
                {
                    // Event handler for logging
                };

                autoController.Start();
                _autoControllers[gpuIndex] = autoController;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✅ Auto fan control started with '{selectedProfile.Name}' profile on GPU {gpuIndex}");
                Console.ResetColor();
                Console.WriteLine("\nMonitoring... (Press ESC to stop)");

                // Monitor until ESC
                while (true)
                {
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        autoController.Stop();
                        break;
                    }

                    var status = _multiGPUController.GetGPUStatus(gpuIndex);
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.ForegroundColor = GetTemperatureColor(status.Temperature);
                    Console.Write($"Temp: {status.Temperature:F1}°C | Fan: {status.FanSpeed:F0}% | RPM: {status.FanRPM:F0}      ");
                    Console.ResetColor();

                    Thread.Sleep(1000);
                }

                Console.WriteLine("\n\nAuto control stopped.");
            }
            else
            {
                Console.WriteLine("\n❌ Invalid selection.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ResetAllToAuto()
        {
            // Stop all auto controllers
            foreach (var controller in _autoControllers.Values)
            {
                controller.Stop();
            }
            _autoControllers.Clear();

            _multiGPUController.SetAutoFanControlAll();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n✅ All GPUs reset to automatic fan control.");
            Console.ResetColor();
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ViewGPUDetails()
        {
            Console.Clear();
            PrintHeader();
            Console.WriteLine("\n📋 GPU Details\n");

            var gpus = _multiGPUController.GetAllGPUs();
            foreach (var gpu in gpus)
            {
                var status = _multiGPUController.GetGPUStatus(gpu.Index);
                
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"═══ GPU {gpu.Index}: {gpu.Name} ═══");
                Console.ResetColor();
                Console.WriteLine($"Type:        {gpu.Type}");
                Console.ForegroundColor = GetTemperatureColor(status.Temperature);
                Console.WriteLine($"Temperature: {status.Temperature:F1}°C");
                Console.ResetColor();
                Console.WriteLine($"Fan Speed:   {status.FanSpeed:F0}%");
                Console.WriteLine($"Fan RPM:     {status.FanRPM:F0}");
                Console.WriteLine();
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private int SelectGPU()
        {
            var gpus = _multiGPUController.GetAllGPUs();
            
            if (gpus.Count == 1)
            {
                Console.WriteLine($"Using GPU 0: {gpus[0].Name}");
                return 0;
            }

            Console.WriteLine("Select GPU:");
            foreach (var gpu in gpus)
            {
                Console.WriteLine($"  [{gpu.Index}] {gpu.Name} ({gpu.Type})");
            }

            Console.Write($"\nEnter GPU index (0-{gpus.Count - 1}): ");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < gpus.Count)
            {
                return index;
            }

            Console.WriteLine("\n❌ Invalid GPU index.");
            Console.ReadKey();
            return -1;
        }

        private ConsoleColor GetTemperatureColor(float temperature)
        {
            if (temperature >= 85) return ConsoleColor.Red;
            if (temperature >= 75) return ConsoleColor.Yellow;
            if (temperature >= 60) return ConsoleColor.Green;
            return ConsoleColor.Cyan;
        }

        private void Cleanup()
        {
            Console.WriteLine("\nCleaning up and resetting to auto control...");
            
            foreach (var controller in _autoControllers.Values)
            {
                controller.Stop();
            }
            _autoControllers.Clear();

            _multiGPUController.Dispose();
            
            Console.WriteLine("Done. Goodbye!");
        }
    }
}
