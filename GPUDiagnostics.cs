using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibreHardwareMonitor.Hardware;

namespace GPUFanController
{
    public class GPUDiagnostics : IDisposable
    {
        private Computer _computer;
        private UpdateVisitor _updateVisitor;

        public GPUDiagnostics()
        {
            _updateVisitor = new UpdateVisitor();
            _computer = new Computer
            {
                IsGpuEnabled = true,
                IsCpuEnabled = false,
                IsMotherboardEnabled = false,
                IsMemoryEnabled = false,
                IsNetworkEnabled = false,
                IsStorageEnabled = false
            };
            _computer.Open();
            _computer.Accept(_updateVisitor);
        }

        public string GetFullDiagnosticReport()
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine("=== GPU FAN CONTROLLER DIAGNOSTIC REPORT ===");
            report.AppendLine($"Generated: {DateTime.Now}");
            report.AppendLine();

            var gpus = _computer.Hardware.Where(h =>
                h.HardwareType == HardwareType.GpuNvidia ||
                h.HardwareType == HardwareType.GpuAmd ||
                h.HardwareType == HardwareType.GpuIntel).ToList();

            if (gpus.Count == 0)
            {
                report.AppendLine("❌ NO GPUs DETECTED");
                report.AppendLine();
                report.AppendLine("Possible reasons:");
                report.AppendLine("1. No compatible GPU installed");
                report.AppendLine("2. GPU drivers not installed or outdated");
                report.AppendLine("3. Application not running as Administrator");
                report.AppendLine();
                return report.ToString();
            }

            report.AppendLine($"✅ Found {gpus.Count} GPU(s)");
            report.AppendLine();

            for (int i = 0; i < gpus.Count; i++)
            {
                var gpu = gpus[i];
                report.AppendLine($"--- GPU {i}: {gpu.Name} ---");
                report.AppendLine($"Type: {gpu.HardwareType}");
                report.AppendLine();

                gpu.Update();

                // Check for temperature sensors
                var tempSensors = gpu.Sensors.Where(s => s.SensorType == SensorType.Temperature).ToList();
                report.AppendLine($"Temperature Sensors: {tempSensors.Count}");
                foreach (var sensor in tempSensors)
                {
                    report.AppendLine($"  • {sensor.Name}: {sensor.Value}°C");
                }
                report.AppendLine();

                // Check for fan sensors
                var fanSensors = gpu.Sensors.Where(s => s.SensorType == SensorType.Fan).ToList();
                report.AppendLine($"Fan RPM Sensors: {fanSensors.Count}");
                foreach (var sensor in fanSensors)
                {
                    report.AppendLine($"  • {sensor.Name}: {sensor.Value} RPM");
                }
                report.AppendLine();

                // Check for fan CONTROL (most important)
                var controlSensors = gpu.Sensors.Where(s => s.SensorType == SensorType.Control).ToList();
                report.AppendLine($"Fan Control Sensors: {controlSensors.Count}");
                
                bool hasFanControl = false;
                foreach (var sensor in controlSensors)
                {
                    bool isFanControl = sensor.Name.Contains("Fan", StringComparison.OrdinalIgnoreCase);
                    report.AppendLine($"  • {sensor.Name}: {sensor.Value}% {(isFanControl ? "✅ FAN CONTROL" : "")}");
                    
                    if (isFanControl && sensor.Control != null)
                    {
                        hasFanControl = true;
                        report.AppendLine($"    - Control available: YES");
                        report.AppendLine($"    - Min Value: {sensor.Control.MinSoftwareValue}");
                        report.AppendLine($"    - Max Value: {sensor.Control.MaxSoftwareValue}");
                    }
                }
                report.AppendLine();

                // Overall assessment
                report.AppendLine("ASSESSMENT:");
                if (!hasFanControl)
                {
                    report.AppendLine("❌ FAN CONTROL NOT SUPPORTED");
                    report.AppendLine();
                    report.AppendLine("Possible reasons:");
                    report.AppendLine("1. This GPU model doesn't support software fan control");
                    report.AppendLine("2. Fan control is locked by BIOS/manufacturer");
                    report.AppendLine("3. Laptop GPU with vendor-locked fan control");
                    report.AppendLine("4. Need manufacturer-specific software (MSI Afterburner, etc.)");
                    report.AppendLine();
                    report.AppendLine("ALTERNATIVES:");
                    report.AppendLine("• MSI Afterburner (works with most GPUs)");
                    report.AppendLine("• EVGA Precision X1 (EVGA cards)");
                    report.AppendLine("• ASUS GPU Tweak (ASUS cards)");
                    report.AppendLine("• Gigabyte AORUS Engine (Gigabyte cards)");
                }
                else
                {
                    report.AppendLine("✅ FAN CONTROL SUPPORTED");
                    report.AppendLine("This GPU should work with manual fan control.");
                }
                report.AppendLine();

                // All sensors for reference
                report.AppendLine("ALL AVAILABLE SENSORS:");
                foreach (var sensor in gpu.Sensors)
                {
                    report.AppendLine($"  • [{sensor.SensorType}] {sensor.Name}: {sensor.Value}");
                }
                report.AppendLine();
                report.AppendLine("===========================================");
                report.AppendLine();
            }

            return report.ToString();
        }

        public void Dispose()
        {
            _computer?.Close();
        }
    }
}
