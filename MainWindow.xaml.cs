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

namespace Autoclicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
    }
    

}