using System;
using System.Collections.Generic;
using System.Linq;
using LibreHardwareMonitor.Hardware;

namespace GPUFanController
{
    public class GPUController : IDisposable
    {
        private Computer _computer;
        private IHardware? _gpuHardware;
        private UpdateVisitor _updateVisitor;

        public GPUController()
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

            // Find the first GPU
            _gpuHardware = _computer.Hardware.FirstOrDefault(h => 
                h.HardwareType == HardwareType.GpuNvidia || 
                h.HardwareType == HardwareType.GpuAmd ||
                h.HardwareType == HardwareType.GpuIntel);
        }

        public bool IsGPUAvailable => _gpuHardware != null;

        public string GetGPUName()
        {
            return _gpuHardware?.Name ?? "No GPU detected";
        }

        public string GetGPUType()
        {
            if (_gpuHardware == null) return "Unknown";
            
            return _gpuHardware.HardwareType switch
            {
                HardwareType.GpuNvidia => "NVIDIA",
                HardwareType.GpuAmd => "AMD",
                HardwareType.GpuIntel => "Intel",
                _ => "Unknown"
            };
        }

        public float GetCurrentFanSpeed()
        {
            if (_gpuHardware == null) return 0;

            _gpuHardware.Update();

            // Look for fan control sensor
            var fanSensor = _gpuHardware.Sensors.FirstOrDefault(s => 
                s.SensorType == SensorType.Control && 
                s.Name.Contains("Fan", StringComparison.OrdinalIgnoreCase));

            return fanSensor?.Value ?? 0;
        }

        public float GetCurrentTemperature()
        {
            if (_gpuHardware == null) return 0;

            _gpuHardware.Update();

            // Look for GPU core temperature
            var tempSensor = _gpuHardware.Sensors.FirstOrDefault(s => 
                s.SensorType == SensorType.Temperature && 
                (s.Name.Contains("Core", StringComparison.OrdinalIgnoreCase) ||
                 s.Name.Contains("GPU", StringComparison.OrdinalIgnoreCase)));

            return tempSensor?.Value ?? 0;
        }

        public float GetCurrentFanRPM()
        {
            if (_gpuHardware == null) return 0;

            _gpuHardware.Update();

            // Look for fan RPM sensor
            var fanRPMSensor = _gpuHardware.Sensors.FirstOrDefault(s => 
                s.SensorType == SensorType.Fan);

            return fanRPMSensor?.Value ?? 0;
        }

        public bool SetFanSpeed(int percentage)
        {
            if (_gpuHardware == null) return false;
            if (percentage < 0 || percentage > 100) return false;

            try
            {
                // Look for fan control
                var fanControl = _gpuHardware.Sensors.FirstOrDefault(s => 
                    s.SensorType == SensorType.Control && 
                    s.Name.Contains("Fan", StringComparison.OrdinalIgnoreCase));

                if (fanControl != null)
                {
                    // Try to set the control value
                    fanControl.Control?.SetSoftware(percentage);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SetAutoFanControl()
        {
            if (_gpuHardware == null) return;

            try
            {
                var fanControl = _gpuHardware.Sensors.FirstOrDefault(s => 
                    s.SensorType == SensorType.Control && 
                    s.Name.Contains("Fan", StringComparison.OrdinalIgnoreCase));

                fanControl?.Control?.SetDefault();
            }
            catch (Exception)
            {
                // Ignore errors when resetting to auto
            }
        }

        public bool IsLaptopGPU()
        {
            if (_gpuHardware == null) return false;
            
            // Check if GPU name contains laptop/mobile indicators
            string name = _gpuHardware.Name.ToLower();
            return name.Contains("mobile") || 
                   name.Contains("laptop") || 
                   name.Contains("max-q") ||
                   name.Contains("max-p") ||
                   name.Contains("notebook") ||
                   name.Contains("m ") || // e.g., "GTX 1060 M"
                   name.Contains(" m)"); // e.g., "GTX 1060 (M)"
        }

        public string GetFanControlStatus()
        {
            if (_gpuHardware == null) return "No GPU detected";
            
            _gpuHardware.Update();
            
            // Check if fan control sensor exists
            var fanControl = _gpuHardware.Sensors.FirstOrDefault(s => 
                s.SensorType == SensorType.Control && 
                s.Name.Contains("Fan", StringComparison.OrdinalIgnoreCase));
            
            if (fanControl == null)
            {
                if (IsLaptopGPU())
                    return "Laptop GPU - Fan control typically locked by manufacturer";
                else
                    return "GPU does not expose fan control sensor";
            }
            
            if (fanControl.Control == null)
                return "Fan sensor detected but control interface not available";
            
            return "Full fan control available";
        }

        public List<string> GetSensorInfo()
        {
            var info = new List<string>();
            
            if (_gpuHardware == null)
            {
                info.Add("No GPU detected");
                return info;
            }

            _gpuHardware.Update();

            foreach (var sensor in _gpuHardware.Sensors)
            {
                info.Add($"{sensor.Name} ({sensor.SensorType}): {sensor.Value}");
            }

            return info;
        }

        public void Dispose()
        {
            SetAutoFanControl();
            _computer?.Close();
        }
    }

    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }

        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware)
                subHardware.Accept(this);
        }

        public void VisitSensor(ISensor sensor) { }

        public void VisitParameter(IParameter parameter) { }
    }
}
