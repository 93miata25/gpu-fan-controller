using System;
using System.Collections.Generic;
using System.Linq;

namespace GPUFanController
{
    public class FanCurveProfile
    {
        public string Name { get; set; } = "";
        public List<CurvePoint> Points { get; set; } = new();

        public FanCurveProfile() { }

        public FanCurveProfile(string name, List<CurvePoint> points)
        {
            Name = name;
            Points = points.OrderBy(p => p.Temperature).ToList();
        }

        public int GetFanSpeedForTemperature(float temperature)
        {
            if (Points.Count == 0) return 50; // Default
            if (temperature <= Points[0].Temperature) return Points[0].FanSpeed;
            if (temperature >= Points[^1].Temperature) return Points[^1].FanSpeed;

            // Linear interpolation between points
            for (int i = 0; i < Points.Count - 1; i++)
            {
                var p1 = Points[i];
                var p2 = Points[i + 1];

                if (temperature >= p1.Temperature && temperature <= p2.Temperature)
                {
                    float ratio = (temperature - p1.Temperature) / (p2.Temperature - p1.Temperature);
                    return (int)(p1.FanSpeed + ratio * (p2.FanSpeed - p1.FanSpeed));
                }
            }

            return 50; // Fallback
        }

        public static FanCurveProfile GetSilentProfile()
        {
            return new FanCurveProfile("Silent", new List<CurvePoint>
            {
                new CurvePoint(0, 30),
                new CurvePoint(50, 35),
                new CurvePoint(60, 40),
                new CurvePoint(70, 50),
                new CurvePoint(75, 60),
                new CurvePoint(80, 75),
                new CurvePoint(85, 90),
                new CurvePoint(90, 100)
            });
        }

        public static FanCurveProfile GetBalancedProfile()
        {
            return new FanCurveProfile("Balanced", new List<CurvePoint>
            {
                new CurvePoint(0, 35),
                new CurvePoint(50, 40),
                new CurvePoint(60, 50),
                new CurvePoint(70, 65),
                new CurvePoint(75, 75),
                new CurvePoint(80, 85),
                new CurvePoint(85, 95),
                new CurvePoint(90, 100)
            });
        }

        public static FanCurveProfile GetPerformanceProfile()
        {
            return new FanCurveProfile("Performance", new List<CurvePoint>
            {
                new CurvePoint(0, 40),
                new CurvePoint(50, 50),
                new CurvePoint(60, 60),
                new CurvePoint(70, 75),
                new CurvePoint(75, 85),
                new CurvePoint(80, 95),
                new CurvePoint(85, 100),
                new CurvePoint(90, 100)
            });
        }

        public static FanCurveProfile GetAggressiveProfile()
        {
            return new FanCurveProfile("Aggressive", new List<CurvePoint>
            {
                new CurvePoint(0, 50),
                new CurvePoint(50, 60),
                new CurvePoint(60, 70),
                new CurvePoint(70, 85),
                new CurvePoint(75, 95),
                new CurvePoint(80, 100),
                new CurvePoint(85, 100),
                new CurvePoint(90, 100)
            });
        }

        public static List<FanCurveProfile> GetAllPresets()
        {
            return new List<FanCurveProfile>
            {
                GetSilentProfile(),
                GetBalancedProfile(),
                GetPerformanceProfile(),
                GetAggressiveProfile()
            };
        }
    }

    public class CurvePoint
    {
        public float Temperature { get; set; }
        public int FanSpeed { get; set; }

        public CurvePoint() { }

        public CurvePoint(float temperature, int fanSpeed)
        {
            Temperature = temperature;
            FanSpeed = fanSpeed;
        }

        public override string ToString()
        {
            return $"{Temperature}°C → {FanSpeed}%";
        }
    }
}
