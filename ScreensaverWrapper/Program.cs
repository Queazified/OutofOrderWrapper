using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

class Program
{
    static void Main()
    {
        // Path to the screensaver file relative to the EXE
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "screensaver", "Screensaver.scr");

        if (!File.Exists(path))
        {
            Console.WriteLine($"Screensaver not found: {path}");
            return;
        }

        while (true)
        {
            Process p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.Arguments = "/s";  // Run screensaver in full-screen mode
            p.StartInfo.UseShellExecute = true;
            p.Start();

            // Wait until the screensaver exits
            p.WaitForExit();

            // Short delay before restarting
            Thread.Sleep(500);
        }
    }
}
