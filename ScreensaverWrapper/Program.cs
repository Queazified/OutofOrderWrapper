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
        
        // Path to the screensaver file relative to the EXE
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "screensaver", "Screensaver.scr");

        if (!File.Exists(path))
        {
            Console.WriteLine($"Screensaver not found: {path}");
            return;
        }

        Console.WriteLine($"Found screensaver at: {path}");

        while (true)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = path;
                p.StartInfo.Arguments = "/s";  // Run screensaver in full-screen mode
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.Verb = "runas"; // Run as administrator
                p.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                
                Console.WriteLine("Launching screensaver...");
                p.Start();

                // Hide console window
                ShowWindow(GetConsoleWindow(), SW_HIDE);

                // Wait until the screensaver exits
                p.WaitForExit();
                Console.WriteLine("Screensaver exited, restarting...");

                // Short delay before restarting
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Thread.Sleep(5000); // Wait longer if there's an error
            }
        }
    }

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    
    const int SW_HIDE = 0;
}
