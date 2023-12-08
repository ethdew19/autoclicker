using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Threading;


namespace Autoclicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        public MainWindow()
        {
            InitializeComponent();
            timer.Tick += startAutoclicker;
            //_hookID = SetHook(_proc);
            MouseManager mm = new MouseManager();
            mm.AddEvent(MouseManager.MouseMessages.WM_LBUTTONDOWN, call);


        }
        
        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }
        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            SetCursorPos(1, 1);
            MouseEvent(MouseEventFlags.LEFTDOWN, 0, 0, 0, IntPtr.Zero);
            Console.WriteLine("BUTTON CLICKED");
        }
        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);
        
        [DllImport("user32.dll", EntryPoint = "mouse_event", CallingConvention = CallingConvention.Winapi)]
        internal static extern void MouseEvent(MouseEventFlags dwFlags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo);

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private void previewNumbers(object sender, TextCompositionEventArgs e)
        {
            if (!_regex.IsMatch(e.Text))
            {
                e.Handled = false;
            }
        }

        private void startAutoclicker(object sender, EventArgs e)
        {
            int X;
            int Y;
            if (int.TryParse(XPositionNumberBox.Text, out X) && int.TryParse(YPositionNumberBox.Text, out Y))
            {
                SetCursorPos(X, Y);
                MouseEvent(MouseEventFlags.LEFTDOWN, 0, 0, 0, IntPtr.Zero);
                Console.WriteLine($"Mouse at position {X}x{Y}");
            }
        }

        private void DoEveryXSeconds(object sender, RoutedEventArgs e)
        {
            int Seconds;
            if (int.TryParse(SecondsBox.Text, out Seconds)) 
            {
                if (StartButton.Content.ToString() == "Start")
                {
                    timer.Interval = TimeSpan.FromSeconds(Seconds);
                    timer.Start();
                    StartButton.Content = "Stop";
                }
                else
                {
                    StartButton.Content = "Start";
                    timer.Stop();
                    
                }
                
            }
        }
        private void MainWindowKeyDown(object sender, KeyEventArgs e)
        {
            // Stop the timer when the 'S' key is pressed
            if (e.Key == Key.S)
            {
                timer.Stop();
                StartButton.Content = "Start";
            }
        }

        private async void StartHyperClick(object sender, EventArgs e)
        {
            int clicks;
            if (int.TryParse(AmountOfClicksBox.Text, out clicks))
            {
                makeMouseClickSysCalls(clicks);
            }

        }

        private async void makeMouseClickSysCalls(int clicks) 
        {
            for (int i = 0; i < clicks; i++)
            {
                MouseEvent(MouseEventFlags.LEFTDOWN, 0, 0, 0, IntPtr.Zero);
                MouseEvent(MouseEventFlags.LEFTUP, 0, 0, 0, IntPtr.Zero);
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                Console.WriteLine($"Click{i}\n");
            }
        }
        
        
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            {
                Console.WriteLine("HOOK CALLBACK");
                // Handle mouse click event here
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
        
        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            // Other mouse messages can be added here
        }
        
        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public static void call()
        {
            Console.WriteLine("HEREEREREERER");
        }
 
        
        //Installs a hook procedure that monitors low-level mouse input events.
        //https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowshookexa
        private const int WH_MOUSE_LL = 14;
        
        
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