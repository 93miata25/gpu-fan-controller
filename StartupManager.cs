using System;
using Microsoft.Win32;

namespace GPUFanController
{
    public class StartupManager
    {
        private const string AppName = "GPUFanController";
        private const string RegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public static bool IsStartupEnabled()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKey, false))
                {
                    if (key == null)
                        return false;

                    object value = key.GetValue(AppName);
                    return value != null;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool EnableStartup(string executablePath, bool startMinimized = false)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKey, true))
                {
                    if (key == null)
                        return false;

                    string value = startMinimized 
                        ? $"\"{executablePath}\" -minimized" 
                        : $"\"{executablePath}\"";

                    key.SetValue(AppName, value, RegistryValueKind.String);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DisableStartup()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKey, true))
                {
                    if (key == null)
                        return false;

                    key.DeleteValue(AppName, false);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetExecutablePath()
        {
            return System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "";
        }
    }
}
