using System;
using System.Diagnostics;
using System.Drawing;
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

        // Create input blocking window
        using var blocker = new BlockingForm();
        blocker.Show();

        while (true)
        {
            try
            {
                using var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c \"{screensaverPath}\" /s",
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    }
                };

                // Hide our window
                ShowWindow(GetConsoleWindow(), SW_HIDE);
                blocker.BringToFront();
                
                p.Start();
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

class BlockingForm : Form
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WH_MOUSE_LL = 14;
    private static IntPtr hookId = IntPtr.Zero;
    private static LowLevelHookProc? hookProc; // Keep delegate alive
    
    public BlockingForm()
    {
        // Cover all screens
        this.Bounds = SystemInformation.VirtualScreen;
        this.FormBorderStyle = FormBorderStyle.None;
        this.BackColor = Color.Black;
        this.Opacity = 0.1;  // Slightly visible to ensure it's working
        
        // Make it block input but allow click-through
        this.TopMost = true;
        this.ShowInTaskbar = false;
        this.ShowIcon = false;
        
        // Block all input
        this.KeyPreview = true;
        this.KeyDown += (s,e) => e.Handled = true;
        this.KeyUp += (s,e) => e.Handled = true;
        this.KeyPress += (s,e) => e.Handled = true;
        
        // Block mouse
        this.MouseDown += (s,e) => BlockMouse();
        this.MouseUp += (s,e) => BlockMouse();
        this.MouseMove += (s,e) => BlockMouse();
        this.MouseClick += (s,e) => BlockMouse();
        this.MouseDoubleClick += (s,e) => BlockMouse();

        // Set hooks
        hookProc = LowLevelProc; // Store delegate reference
        hookId = SetHook(hookProc);
    }

    private void BlockMouse()
    {
        Cursor.Position = new Point(0, 0);
        this.Capture = true;
        this.BringToFront();
    }

    protected override void WndProc(ref Message m)
    {
        const int WM_SYSCOMMAND = 0x0112;
        const int SC_CLOSE = 0xF060;

        if (m.Msg == WM_SYSCOMMAND && ((int)m.WParam == SC_CLOSE))
        {
            return; // Prevent closing
        }
        base.WndProc(ref m);
    }

    protected override CreateParams CreateParams
    {
        get {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x80000 /* WS_EX_LAYERED */ |
                         0x20 /* WS_EX_TRANSPARENT */ |
                         0x80 /* WS_EX_TOOLWINDOW */ |
                         0x08000000 /* WS_EX_NOACTIVATE */;
            cp.Style |= 0x80000000 /* WS_POPUP */;
            return cp;
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (hookId != IntPtr.Zero)
        {
            UnhookWindowsHookEx(hookId);
        }
        base.OnFormClosing(e);
    }

    private delegate IntPtr LowLevelHookProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr SetHook(LowLevelHookProc proc)
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule ?? throw new InvalidOperationException("Failed to get main module");
        
        return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
            GetModuleHandle(curModule.ModuleName ?? string.Empty), 0u); // Added 'u' suffix for uint
    }

    private static IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            return (IntPtr)1; // Block input
        }
        return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
    }

    // Add missing P/Invoke
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelHookProc lpfn, IntPtr hMod, uint dwThreadId);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}
