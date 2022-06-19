using System.Runtime.InteropServices;
using System.Threading;
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
            App.client.LoginErrorEvent += LoginError;
            App.client.LoginCaptchaEvent += LoginCaptcha;
            App.client.LoginMfaEvent += LoginMfa;
            App.client.LoginMfaErrorEvent += LoginMfaError;
            TryLogin();
        }

        private void LoginMfaError(LoginErrorEventArgs e)
        {
            code.Text = e.Error;
        }

        private void LoginMfa(LoginEventArgs e)
        {
            _lastPayload = e.LoginPayload;
            _loginState = LoginState.Mfa;
            LoginPanel.Visibility = Visibility.Collapsed;
            MfaPanel.Visibility = Visibility.Visible;
        }

        private LoginState _loginState = LoginState.Login;

        private LoginPayload _lastPayload;

        private void LoginCaptcha(LoginEventArgs e)
        {
            CaptchaBox.Visibility = Visibility.Visible;
            CaptchaWebBrowser.Navigate($"https://accounts.hcaptcha.com/demo?sitekey={e.LoginPayload.CaptchaSiteKey}"); // Please don't hurt me hCaptcha
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
            if(_loginState == LoginState.Mfa)
            {
                _loginState = LoginState.Login;
                LoginPanel.Visibility = Visibility.Visible;
                MfaPanel.Visibility = Visibility.Collapsed;
            }
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
            if(_loginState == LoginState.Login)
            {
                Login(null);
            }
            else if (_lastPayload != null)
            {
                App.client.LoginMfa(_lastPayload.Ticket, code.Text);
            }
        }

        private void Login(string captchaKey)
        {
            if (string.IsNullOrEmpty(token.Text))
            {
                if (string.IsNullOrEmpty(email.Text) && string.IsNullOrEmpty(password.Password)) email.Text = "Fill this and password (and code if you're using 2FA)";
                else App.client.Login(email.Text, password.Password, captchaKey);
            }
            else
            {
                App.client.Login(token.Text);
            }
        }

        public void CaptchaDone(string captchaKey)
        {
            CaptchaWebBrowser.Source = null;
            CaptchaBox.Visibility = Visibility.Collapsed;
            Login(captchaKey);
        }

        private void PageLoaded(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if(e.Uri != null)
            {
                var ae = new CaptchaTest(this);
                CaptchaWebBrowser.ObjectForScripting = ae;
                CaptchaWebBrowser.InvokeScript("eval", Properties.Resources.captchaFix);
            }
        }

        private enum LoginState
        {
            Login,
            Mfa
        }
    }

    [ComVisible(true)]
    public class CaptchaTest
    {
        private LoginPage _login;

        public CaptchaTest(LoginPage login)
        {
            _login = login;
        }

        public void SolveCaptcha(string captchaKey)
        {
            _login.CaptchaDone(captchaKey);
        }
    }
}
