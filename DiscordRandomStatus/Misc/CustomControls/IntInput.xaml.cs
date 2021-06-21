using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DiscordRandomStatus
{
    /// <summary>
    /// Interaction logic for IntInput.xaml
    /// </summary>
    public partial class IntInput : Border
    {
        public double FontSize
        {
            get => TextBox.FontSize;
            set => TextBox.FontSize = value;
        }

        internal uint _output = 0;

        public uint Output { get => _output; set => TextBox.Text = value.ToString(); }

        public IntInput()
        {
            InitializeComponent();
        }


        private void LoginBorderAnim(object sender, MouseEventArgs e)
        {
            if (!TextBox.IsKeyboardFocused)
            {
                (sender as Border).BorderBrush = new SolidColorBrush(Color.FromRgb(4, 4, 5));
            }
        }

        private void LoginBorderAnimLeave(object sender, MouseEventArgs e)
        {
            if (!TextBox.IsKeyboardFocused)
            {
                (sender as Border).BorderBrush = new SolidColorBrush(Color.FromArgb(77, 0, 0, 0));
            }
        }

        private void LoginBlurpleAnim(object sender, KeyboardFocusChangedEventArgs e)
        {
            (sender as Border).BorderBrush = new SolidColorBrush(Color.FromRgb(114, 137, 218));
        }

        private void LoginBlurpleAnimLeave(object sender, KeyboardFocusChangedEventArgs e)
        {
            (sender as Border).BorderBrush = new SolidColorBrush(Color.FromArgb(77, 0, 0, 0));
            try
            {
                _output = uint.Parse(TextBox.Text);
            }
            catch
            {
                TextBox.Text = _output.ToString();
            }
        }

        private void FocusKeyStuff(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(TextBox);
            e.Handled = true;
        }

        private void KeyFilter(object sender, KeyEventArgs e)
        {
            if (!int.TryParse(GetCharFromKey(e.Key).ToString(), out _) && e.Key != Key.Back && e.Key != Key.Delete && e.Key != Key.Right && e.Key != Key.Left)
            {
                e.Handled = true;
            }
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                Keyboard.ClearFocus();
            }
        }

        private void Whell(object sender, MouseWheelEventArgs e)
        {
            Up(e.Delta / 100);
        }

        private void UpButt(object sender, RoutedEventArgs e)
        {
            Up(1);
        }

        private void DownButt(object sender, RoutedEventArgs e)
        {
            Up(-1);
        }

        private void Up(int n)
        {
            if (IsUint(_output + n))
            {
                _output += (uint)n;
                TextBox.Text = _output.ToString();
            }
        }

        private bool IsUint(long input)
        {
            if (input <= uint.MaxValue && input >= uint.MinValue)
            {
                return true;
            }

            return false;
        }

        /* https://stackoverflow.com/questions/5825820/how-to-capture-the-character-on-different-locale-keyboards-in-wpf-c
         * Why the hell did I think using WPF would be a good idea */
        public enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
        }

        [DllImport("user32.dll")]
        public static extern int ToUnicode(
            uint wVirtKey,
            uint wScanCode,
            byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
            StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);

        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, MapType uMapType);

        public static char GetCharFromKey(Key key)
        {
            char ch = ' ';

            int virtualKey = KeyInterop.VirtualKeyFromKey(key);
            byte[] keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            uint scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC);
            StringBuilder stringBuilder = new StringBuilder(2);

            int result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch (result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
                default:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
            }
            return ch;
        }
    }
}
