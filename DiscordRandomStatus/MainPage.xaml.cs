using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private Timer statusTimer;

        private static Random random = new Random();

        private CustomStatus status;

        private List<StatusEntry> Entries = new List<StatusEntry> { };

        public MainPage()
        {
            InitializeComponent();

            statusTimer = new Timer();
            statusTimer.Elapsed += (s, e) => ChangeStatus();
            statusTimer.AutoReset = true;

            Load();
        }

        public void Load()
        {
            var arrays = DataStore.Load();
            if (arrays != null)
            {
                for (int i = 0; i < arrays.Length; i++)
                {
                    for(int x = arrays[i].Statuses.Length - 1; x >= 0; x--)
                    {

                        AddEntry(arrays[i].Statuses[x].GetStatus());
                    }
                }
            }
            else
            {
                AddEntry();
            }
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;


            ChangeStatus();
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void Stop()
        {
            stopButton.IsEnabled = false;
            startButton.IsEnabled = true;

            statusTimer.Stop();
            App.client.SetStatus(null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddEntry();
        }

        private void Interrupt()
        {
            saveButton.IsEnabled = true;
            if (stopButton.IsEnabled)
            {
                Stop();
            }
        }

        private void AddEntry(CustomStatus status = null)
        {
            StatusEntry stats;
            if(status == null)
            {
                stats = new StatusEntry();
                Interrupt();
            }
            else
            {
                stats = new StatusEntry(status);
            }
            stats.Margin = new Thickness(0, 0, 0, 10);
            stats.CloseButton.Click += new RoutedEventHandler((s, x) => CloseButtnClcl(stats));
            stats.Text.TextChanged += new TextChangedEventHandler((s, x) => Interrupt());
            stats.EmojText.TextChanged += new TextChangedEventHandler((s, x) => Interrupt());
            stats.TimeSeconds.TextBox.TextChanged += new TextChangedEventHandler((s, x) => Interrupt());
            VerticalStack.Children.Insert(1, stats);
            Entries.Insert(0, stats);
            Count.Text = Entries.Count.ToString();
        }

        private void CloseButtnClcl(StatusEntry stats)
        {
            Interrupt();
            VerticalStack.Children.Remove(stats);
            Entries.Remove(stats);
            Count.Text = Entries.Count.ToString();
        }

        private void ChangeStatus()
        {
            Debug.WriteLine(Entries.Count);
            statusTimer.Stop();
            /*
            if (random.NextDouble() > 0.95)
            {
                status = StatusList.ListRare[random.Next(StatusList.ListRare.Count())];
            }
            else
            {
            */
                status = (Entries[random.Next(Entries.Count)] as StatusEntry).customStatus;
            
            //}

            statusTimer.Interval = status.Time * 1000;
            App.client.SetStatus(status);
            statusTimer.Start();
            Debug.WriteLine("Changed! " + status.Text);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            saveButton.IsEnabled = false;
            var statuses = new TransitCustomStatus[Entries.Count];
            for (int i = 0; i < Entries.Count; i++)
            {
                statuses[i] = new TransitCustomStatus(Entries[i].customStatus);
            }
            DataStore.Save(new StatusArray[] { new StatusArray(statuses) });
        }
    }
}
