using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        
        // Look for screensaver in same directory as exe
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screensaver.scr");
        string altPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "screensaver", "Screensaver.scr");

        if (!File.Exists(path) && !File.Exists(altPath))
        {
            MessageBox.Show($"Screensaver not found at:\n{path}\nor\n{altPath}", 
                "Screensaver Not Found", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error);
            return;
        }

        // Use whichever path exists
        string screensaverPath = File.Exists(path) ? path : altPath;

        while (true)
        {
            try
            {
                using var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = screensaverPath,
                        Arguments = "/s /p 0", // Run in full screen mode
                        UseShellExecute = true,
                        Verb = "runas", // Request admin rights
                        WindowStyle = ProcessWindowStyle.Maximized,
                        CreateNoWindow = false,
                        RedirectStandardError = false,
                        RedirectStandardOutput = false
                    }
                };

                // Hide console before starting screensaver
                ShowWindow(GetConsoleWindow(), SW_HIDE);
                
                // Set current process to background
                SetPriorityClass(GetCurrentProcess(), IDLE_PRIORITY_CLASS);
                
                p.Start();
                p.PriorityClass = ProcessPriorityClass.AboveNormal;
                p.WaitForExit();
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error running screensaver:\n{ex.Message}\n\nPath: {screensaverPath}", 
                    "Screensaver Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Thread.Sleep(5000);
            }
        }
    }

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern bool SetPriorityClass(IntPtr handle, int priorityClass);
    
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern IntPtr GetCurrentProcess();
    
    const int SW_HIDE = 0;
    const int IDLE_PRIORITY_CLASS = 0x00000040;
}
