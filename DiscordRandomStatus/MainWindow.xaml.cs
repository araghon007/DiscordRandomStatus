using System.ComponentModel;
using System.Windows;

namespace DiscordRandomStatus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            App.client.Ready += Client_Ready;

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Main();
            }
        }

        private void Client_Ready(ReadyEventArgs e)
        {
            Mainframe.Navigate(new MainPage());
        }

        public void Main()
        {
            Mainframe.Navigate(new LoginPage());
        }
    }
}
