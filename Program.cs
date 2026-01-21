using System;
using System.Linq;
using System.Windows.Forms;

namespace GPUFanController
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Check for command-line arguments
            bool startMinimized = args.Contains("-minimized") || args.Contains("--minimized");
            
            Application.Run(new MainForm(startMinimized));
        }
    }
}
