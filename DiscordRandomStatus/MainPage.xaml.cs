using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Windows;

namespace DiscordRandomStatus
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage
    {
        private Timer statusTimer;

        private static Random random = new Random();

        private CustomStatus currStatus;

        private List<StatusEntry> Entries = new List<StatusEntry>();

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
            StatusEntry statusEntry;
            if(status == null)
            {
                statusEntry = new StatusEntry();
                Interrupt();
            }
            else
            {
                statusEntry = new StatusEntry(status);
            }
            statusEntry.Margin = new Thickness(0, 0, 0, 10);
            statusEntry.CloseButton.Click += (s, x) => CloseButtnClcl(statusEntry);
            statusEntry.Text.TextChanged += (s, x) => Interrupt();
            statusEntry.EmojText.TextChanged += (s, x) => Interrupt();
            statusEntry.TimeSeconds.TextBox.TextChanged += (s, x) => Interrupt();
            VerticalStack.Children.Insert(1, statusEntry);
            Entries.Insert(0, statusEntry);
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
            
            currStatus = Entries[random.Next(Entries.Count)].customStatus;

            statusTimer.Interval = currStatus.Time * 1000;
            App.client.SetStatus(currStatus);
            statusTimer.Start();
            Debug.WriteLine("Changed! " + currStatus.Text);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            saveButton.IsEnabled = false;
            var statuses = new TransitCustomStatus[Entries.Count];
            for (int i = 0; i < Entries.Count; i++)
            {
                statuses[i] = new TransitCustomStatus(Entries[i].customStatus);
            }
            DataStore.Save(new [] { new StatusArray(statuses) });
        }
    }
}
