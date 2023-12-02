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
        public MainWindow()
        {
            InitializeComponent();
            timer.Tick += startAutoclicker;
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

        private void StartHyperClick(object sender, EventArgs e)
        {
            int clicks;
            if (int.TryParse(AmountOfClicksBox.Text, out clicks))
            {
                for (int i = 0; i < clicks; i++)
                {
                    MouseEvent(MouseEventFlags.LEFTDOWN, 0, 0, 0, IntPtr.Zero);
                    Console.WriteLine($"Click{i}\n");
                }
            }
            
        }
    }
    

}