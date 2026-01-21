using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace GPUFanController
{
    public class PresetManager
    {
        private static readonly string PresetFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GPUFanController",
            "Presets"
        );

        private static readonly string ConfigFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GPUFanController",
            "config.json"
        );

        public PresetManager()
        {
            // Ensure preset folder exists
            if (!Directory.Exists(PresetFolder))
            {
                Directory.CreateDirectory(PresetFolder);
            }

            // Ensure config folder exists
            string configDir = Path.GetDirectoryName(ConfigFile);
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }
        }

        public bool SavePreset(string name, FanCurveProfile profile)
        {
            try
            {
                string filePath = Path.Combine(PresetFolder, $"{SanitizeFileName(name)}.json");
                string json = JsonSerializer.Serialize(profile, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public FanCurveProfile? LoadPreset(string name)
        {
            try
            {
                string filePath = Path.Combine(PresetFolder, $"{SanitizeFileName(name)}.json");
                if (!File.Exists(filePath))
                    return null;

                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<FanCurveProfile>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<string> GetPresetNames()
        {
            try
            {
                if (!Directory.Exists(PresetFolder))
                    return new List<string>();

                return Directory.GetFiles(PresetFolder, "*.json")
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public bool DeletePreset(string name)
        {
            try
            {
                string filePath = Path.Combine(PresetFolder, $"{SanitizeFileName(name)}.json");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SaveConfig(AppConfig config)
        {
            try
            {
                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                File.WriteAllText(ConfigFile, json);
            }
            catch (Exception)
            {
                // Silently fail
            }
        }

        public AppConfig LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigFile))
                {
                    string json = File.ReadAllText(ConfigFile);
                    return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                }
            }
            catch (Exception)
            {
                // Silently fail
            }
            return new AppConfig();
        }

        private string SanitizeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name;
        }
    }

    public class AppConfig
    {
        public bool StartWithWindows { get; set; } = false;
        public bool StartMinimized { get; set; } = false;
        public bool AutoStartLastProfile { get; set; } = false;
        public int LastSelectedGPU { get; set; } = 0;
        public string LastProfile { get; set; } = "Balanced";
        public bool LastAutoModeEnabled { get; set; } = false;
    }
}
