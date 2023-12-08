using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Autoclicker
{
    public class MouseManager
    {
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static IntPtr _hookID = IntPtr.Zero;
        private LowLevelMouseProc _proc = HookCallback;
        public delegate void MouseCallBack();
        public enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            // Other mouse messages can be added here
        }
        
        //Installs a hook procedure that monitors low-level mouse input events.
        //https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowshookexa
        private const int WH_MOUSE_LL = 14;

        private static Dictionary<MouseMessages, MouseCallBack> mouseEventActions =
            new Dictionary<MouseMessages, MouseCallBack>();
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public MouseManager()
        {
            _hookID = SetHook(_proc);
        }

        
        public void AddEvent(MouseMessages eventType, MouseCallBack callBack)
        {
            mouseEventActions.Add(eventType, callBack);
        }
        
        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        
        
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            {
                MouseCallBack callback = mouseEventActions[MouseMessages.WM_LBUTTONDOWN];
                callback();
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}
