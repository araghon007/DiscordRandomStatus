using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiscordCustomStatus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
