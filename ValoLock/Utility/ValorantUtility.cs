using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ValoLock.Utility
{
    public class ValorantProcessChecker
    {
        public void PrintProcessList()
        {
            foreach (var process in Process.GetProcesses())
            {
                Console.WriteLine(process.ProcessName);
            }
        }
        
        public bool IsValorantRunning()
        {
            return Process.GetProcesses().Any(p => p.ProcessName.Contains("VALORANT"));
        }
        
        public void MaximizeValorant()
        {
            Process[] processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if (process.ProcessName.Contains("VALORANT"))
                {
                    IntPtr hWnd = process.MainWindowHandle;
                    ShowWindow(hWnd, 3);
                    
                }
            }
        }
        
        public void StartValorant()
        {
            try
            {
               // Process.Start("runas /user:Administrator \"C:\\Riot Games\\VALORANT\\live\\VALORANT.exe\"");
               Process.Start("CMD.exe", "/C \"C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs\\Riot Games\\VALORANT.lnk\"");
            }
            
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    }
    
    public class MouseHook
    {
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelMouseProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public event EventHandler<Point> MouseAction = delegate { };

        public MouseHook()
        {
            _proc = HookCallback;
        }

        public void Start()
        {
            _hookID = SetHook(_proc);
        }

        public void Stop()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseAction(this, new Point(hookStruct.pt.x, hookStruct.pt.y));
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public struct POINT
        {
            public int x;
            public int y;
        }

        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}