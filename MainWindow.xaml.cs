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
        private static IntPtr _hookID = IntPtr.Zero;
        MouseManager mm = new MouseManager();
        Key clickerToggleKey;
        public MainWindow()
        {
            InitializeComponent();
            timer.Tick += clickXAndYCoordinates;
            //_hookID = SetHook(_proc);
            mm.AddEvent(MouseManager.MouseMessages.WM_LBUTTONDOWN, call);
        }
        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private void previewNumbers(object sender, TextCompositionEventArgs e)
        {
            if (!_regex.IsMatch(e.Text))
            {
                e.Handled = false;
            }
        }

        private void clickXAndYCoordinates(object sender, EventArgs e)
        {
            int X;
            int Y;
            if (int.TryParse(XPositionNumberBox.Text, out X) && int.TryParse(YPositionNumberBox.Text, out Y))
            {
                SetCursorPos(X, Y);
                mm.doMouseEvent(MouseManager.MouseEventFlags.LEFTDOWN);
                mm.doMouseEvent(MouseManager.MouseEventFlags.LEFTUP);
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

            if (e.Key == clickerToggleKey)
            {
                clickerToggleKeyPressed(sender, e);
            }
        }

        private async void StartHyperClick(object sender, EventArgs e)
        {
            int clicks;
            if (int.TryParse(AmountOfClicksBox.Text, out clicks))
            {
                mm.clickXTimes(clicks);
            }

        }
        
        public static void call()
        {
            Console.WriteLine("HEREEREREERER");
        }
        
        public void setClickerToggleKey(object sender, RoutedEventArgs e)
        {
            Key key;
            if (Enum.TryParse(ToggleClicksKey.Text, out key))
            {
                clickerToggleKey = key;
            }
        }

        public void clickerToggleKeyPressed(object sender, KeyEventArgs e)
        {
            
        }

    }
}