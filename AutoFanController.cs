using System;
using System.Threading;

namespace GPUFanController
{
    public class AutoFanController : IDisposable
    {
        private GPUController _gpuController;
        private FanCurveProfile _activeProfile;
        private System.Threading.Timer? _autoTimer;
        private bool _isRunning;
        private int _updateInterval = 2000; // 2 seconds
        private float _lastTemperature;
        private int _lastFanSpeed;

        public event EventHandler<AutoAdjustmentEventArgs>? FanSpeedAdjusted;

        public AutoFanController(GPUController gpuController, FanCurveProfile profile)
        {
            _gpuController = gpuController;
            _activeProfile = profile;
        }

        public bool IsRunning => _isRunning;
        public FanCurveProfile ActiveProfile => _activeProfile;

        public void SetProfile(FanCurveProfile profile)
        {
            _activeProfile = profile;
        }

        public void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            _autoTimer = new System.Threading.Timer(AutoAdjustCallback, null, 0, _updateInterval);
        }

        public void Stop()
        {
            _isRunning = false;
            _autoTimer?.Dispose();
            _autoTimer = null;
        }

        private void AutoAdjustCallback(object? state)
        {
            if (!_isRunning || !_gpuController.IsGPUAvailable) return;

            try
            {
                float currentTemp = _gpuController.GetCurrentTemperature();
                
                if (currentTemp == 0) return; // No valid reading

                int targetFanSpeed = _activeProfile.GetFanSpeedForTemperature(currentTemp);

                // Apply hysteresis to prevent rapid changes
                if (Math.Abs(targetFanSpeed - _lastFanSpeed) >= 5 || 
                    Math.Abs(currentTemp - _lastTemperature) >= 2)
                {
                    bool success = _gpuController.SetFanSpeed(targetFanSpeed);

                    if (success)
                    {
                        _lastTemperature = currentTemp;
                        _lastFanSpeed = targetFanSpeed;

                        FanSpeedAdjusted?.Invoke(this, new AutoAdjustmentEventArgs
                        {
                            Temperature = currentTemp,
                            FanSpeed = targetFanSpeed,
                            ProfileName = _activeProfile.Name,
                            Timestamp = DateTime.Now
                        });
                    }
                }
            }
            catch (Exception)
            {
                // Silently handle errors in auto mode
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }

    public class AutoAdjustmentEventArgs : EventArgs
    {
        public float Temperature { get; set; }
        public int FanSpeed { get; set; }
        public string ProfileName { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }
}
