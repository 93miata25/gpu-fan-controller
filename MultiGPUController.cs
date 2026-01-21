using System;
using System.Collections.Generic;
using System.Linq;
using LibreHardwareMonitor.Hardware;

namespace GPUFanController
{
    public class MultiGPUController : IDisposable
    {
        private Computer _computer;
        private List<GPUInfo> _gpus;
        private UpdateVisitor _updateVisitor;

        public MultiGPUController()
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

            _gpus = new List<GPUInfo>();
            DiscoverGPUs();
        }

        private void DiscoverGPUs()
        {
            int index = 0;
            foreach (var hardware in _computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.GpuNvidia ||
                    hardware.HardwareType == HardwareType.GpuAmd ||
                    hardware.HardwareType == HardwareType.GpuIntel)
                {
                    _gpus.Add(new GPUInfo
                    {
                        Index = index++,
                        Hardware = hardware,
                        Name = hardware.Name,
                        Type = GetGPUTypeName(hardware.HardwareType)
                    });
                }
            }
        }

        private string GetGPUTypeName(HardwareType type)
        {
            return type switch
            {
                HardwareType.GpuNvidia => "NVIDIA",
                HardwareType.GpuAmd => "AMD",
                HardwareType.GpuIntel => "Intel",
                _ => "Unknown"
            };
        }

        public int GetGPUCount() => _gpus.Count;

        public List<GPUInfo> GetAllGPUs() => new List<GPUInfo>(_gpus);

        public GPUInfo? GetGPU(int index)
        {
            return index >= 0 && index < _gpus.Count ? _gpus[index] : null;
        }

        public float GetTemperature(int gpuIndex)
        {
            var gpu = GetGPU(gpuIndex);
            if (gpu == null) return 0;

            gpu.Hardware.Update();

            var tempSensor = gpu.Hardware.Sensors.FirstOrDefault(s =>
                s.SensorType == SensorType.Temperature &&
                (s.Name.Contains("Core", StringComparison.OrdinalIgnoreCase) ||
                 s.Name.Contains("GPU", StringComparison.OrdinalIgnoreCase)));

            return tempSensor?.Value ?? 0;
        }

        public float GetFanSpeed(int gpuIndex)
        {
            var gpu = GetGPU(gpuIndex);
            if (gpu == null) return 0;

            gpu.Hardware.Update();

            var fanSensor = gpu.Hardware.Sensors.FirstOrDefault(s =>
                s.SensorType == SensorType.Control &&
                s.Name.Contains("Fan", StringComparison.OrdinalIgnoreCase));

            return fanSensor?.Value ?? 0;
        }

        public float GetFanRPM(int gpuIndex)
        {
            var gpu = GetGPU(gpuIndex);
            if (gpu == null) return 0;

            gpu.Hardware.Update();

            var fanRPMSensor = gpu.Hardware.Sensors.FirstOrDefault(s =>
                s.SensorType == SensorType.Fan);

            return fanRPMSensor?.Value ?? 0;
        }

        public bool SetFanSpeed(int gpuIndex, int percentage)
        {
            var gpu = GetGPU(gpuIndex);
            if (gpu == null) return false;
            if (percentage < 0 || percentage > 100) return false;

            try
            {
                var fanControl = gpu.Hardware.Sensors.FirstOrDefault(s =>
                    s.SensorType == SensorType.Control &&
                    s.Name.Contains("Fan", StringComparison.OrdinalIgnoreCase));

                if (fanControl != null)
                {
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

        public void SetAutoFanControl(int gpuIndex)
        {
            var gpu = GetGPU(gpuIndex);
            if (gpu == null) return;

            try
            {
                var fanControl = gpu.Hardware.Sensors.FirstOrDefault(s =>
                    s.SensorType == SensorType.Control &&
                    s.Name.Contains("Fan", StringComparison.OrdinalIgnoreCase));

                fanControl?.Control?.SetDefault();
            }
            catch (Exception)
            {
                // Ignore errors when resetting to auto
            }
        }

        public void SetAutoFanControlAll()
        {
            for (int i = 0; i < _gpus.Count; i++)
            {
                SetAutoFanControl(i);
            }
        }

        public GPUStatus GetGPUStatus(int gpuIndex)
        {
            var gpu = GetGPU(gpuIndex);
            if (gpu == null)
            {
                return new GPUStatus { IsAvailable = false };
            }

            return new GPUStatus
            {
                IsAvailable = true,
                Index = gpuIndex,
                Name = gpu.Name,
                Type = gpu.Type,
                Temperature = GetTemperature(gpuIndex),
                FanSpeed = GetFanSpeed(gpuIndex),
                FanRPM = GetFanRPM(gpuIndex)
            };
        }

        public List<GPUStatus> GetAllGPUStatus()
        {
            var statusList = new List<GPUStatus>();
            for (int i = 0; i < _gpus.Count; i++)
            {
                statusList.Add(GetGPUStatus(i));
            }
            return statusList;
        }

        public bool IsLaptopGPU(int gpuIndex)
        {
            var gpu = GetGPU(gpuIndex);
            if (gpu == null) return false;
            
            string name = gpu.Name.ToLower();
            return name.Contains("mobile") || 
                   name.Contains("laptop") || 
                   name.Contains("max-q") ||
                   name.Contains("max-p") ||
                   name.Contains("notebook") ||
                   name.Contains("m ") ||
                   name.Contains(" m)");
        }

        public string GetFanControlStatus(int gpuIndex)
        {
            var gpu = GetGPU(gpuIndex);
            if (gpu == null) return "Invalid GPU index";
            
            gpu.Hardware.Update();
            
            var fanControl = gpu.Hardware.Sensors.FirstOrDefault(s => 
                s.SensorType == SensorType.Control && 
                s.Name.Contains("Fan", StringComparison.OrdinalIgnoreCase));
            
            if (fanControl == null)
            {
                if (IsLaptopGPU(gpuIndex))
                    return "Laptop GPU - Fan control locked by manufacturer";
                else
                    return "GPU does not expose fan control sensor";
            }
            
            if (fanControl.Control == null)
                return "Fan sensor detected but control not available";
            
            return "Full fan control available";
        }

        public void Dispose()
        {
            SetAutoFanControlAll();
            _computer?.Close();
        }
    }

    public class GPUInfo
    {
        public int Index { get; set; }
        public IHardware Hardware { get; set; } = null!;
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
    }

    public class GPUStatus
    {
        public bool IsAvailable { get; set; }
        public int Index { get; set; }
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public float Temperature { get; set; }
        public float FanSpeed { get; set; }
        public float FanRPM { get; set; }

        public override string ToString()
        {
            if (!IsAvailable) return "GPU Not Available";
            return $"GPU {Index}: {Name} ({Type}) - {Temperature:F1}°C, {FanSpeed:F0}% ({FanRPM:F0} RPM)";
        }
    }
}
