using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

class Program
{
    static void Main()
    {
        // Get path to screensaver relative to executable
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "screensaver", "MyScreensaver.scr");

        if (!File.Exists(path))
        {
            Console.WriteLine($"Screensaver not found: {path}");
            return;
        }

        while (true)
        {
            Process p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.UseShellExecute = true;
            p.Start();

            // Wait for the screensaver process to exit
            p.WaitForExit();

            // Short delay before restarting
            Thread.Sleep(500);
        }
    }
}
