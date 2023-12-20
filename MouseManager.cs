using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Autoclicker
{
    public class MouseManager
    {
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static IntPtr _hookID = IntPtr.Zero;
        private LowLevelMouseProc _proc = HookCallback;

        public delegate void MouseCallBack();

        private static bool DISABLE_INTERRUPTS = false;
        
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
        
        [DllImport("user32.dll", EntryPoint = "mouse_event", CallingConvention = CallingConvention.Winapi)]
        internal static extern void MouseEvent(MouseEventFlags dwFlags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo);

        public MouseManager()
        {
            _hookID = SetHook(_proc);
        }

        
        public void AddEvent(MouseMessages eventType, MouseCallBack callBack)
        {
            mouseEventActions.Add(eventType, callBack);
        }

        public void RemoveEvent(MouseMessages eventType)
        {
            mouseEventActions.Remove(eventType);
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
            if (DISABLE_INTERRUPTS)
            {
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }
            DISABLE_INTERRUPTS = true;
            if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            {
                if (mouseEventActions.ContainsKey(MouseMessages.WM_LBUTTONDOWN))
                {
                    MouseCallBack callback = mouseEventActions[MouseMessages.WM_LBUTTONDOWN];
                    callback();
                }
            }

            DISABLE_INTERRUPTS = false;
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static void UnHook()
        {
            UnhookWindowsHookEx(_hookID);
        }
        
        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }
        
        //has 100ms of delay between clicks so it doesnt crash your computer
        public async void clickXTimes(int clicks) 
        {
            for (int i = 0; i < clicks; i++)
            {
                MouseEvent(MouseEventFlags.LEFTDOWN, 0, 0, 0, IntPtr.Zero);
                MouseEvent(MouseEventFlags.LEFTUP, 0, 0, 0, IntPtr.Zero);
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                Console.WriteLine($"Click{i}\n");
            }
        }
        
        public void doMouseEvent(MouseEventFlags e, uint xPos=0, uint yPos=0) 
        {
            MouseEvent(e, xPos, yPos, 0, IntPtr.Zero);
        }
    }
}
