using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DiscordRandomStatus
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage
    {
        public LoginPage()
        {
            InitializeComponent();
            App.client.LoginError += LoginError;
            TryLogin();
        }

        private void TryLogin()
        {
            var token = DataStore.LoadToken();
            if(token != null)
            {
                App.client.Login(token);
            }
        }

        private void LoginError(LoginErrorEventArgs e)
        {
            email.Text = e.Error;
        }

        private void LoginBorderAnim(object sender, MouseEventArgs e)
        {
            var bord = sender as Border;
            if (!bord.Child.IsKeyboardFocused)
            {
                (sender as Border).BorderBrush = new SolidColorBrush(Color.FromRgb(4, 4, 5));
            }
        }

        private void LoginBorderAnimLeave(object sender, MouseEventArgs e)
        {
            var bord = sender as Border;
            if (!bord.Child.IsKeyboardFocused)
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
        }

        private void FocusKeyStuff(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus((sender as Border).Child);
            e.Handled = true;
        }

        private void LoginButton(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void TextKeyHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

        private void Login()
        {
            if (string.IsNullOrEmpty(token.Text))
            {
                if (string.IsNullOrEmpty(email.Text) && string.IsNullOrEmpty(password.Password)) email.Text = "Fill this and password (and code if you're using 2FA)";
                else App.client.Login(email.Text, password.Password, code.Text);
            }
            else
            {
                App.client.Login(token.Text);
            }
        }
    }
}
