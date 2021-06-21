using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DiscordRandomStatus;
using TwemojiNet.Utilities;
using WpfAnimatedGif;

namespace DiscordRandomStatus
{
    /// <summary>
    /// Interaction logic for StatusEntry.xaml
    /// </summary>
    public partial class StatusEntry : UserControl
    {
        public CustomStatus customStatus = new CustomStatus();

        public StatusEntry()
        {
            InitializeComponent();
            imga.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>((s, b) => {
                EmojText.Text = "";
                emo.Text = "";
                emoj.Visibility = Visibility.Visible;
                imga.Source = null;
                customStatus.EmojiName = null;
                customStatus.EmojiId = null;
                customStatus.Animated = false;
            });
            TimeSeconds.TextBox.TextChanged += TimeSecondsChanged;
        }

        public StatusEntry(CustomStatus status) :this()
        {
            EmojText.Text = status.EmojiFull;
            Text.Text = status.Text;
            TimeSeconds.Output = status.Time;
            customStatus = status;
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

        private void Aaaa(object sender, TextChangedEventArgs e)
        {
            var txt = (sender as TextBox).Text;
            customStatus.Animated = false;
            customStatus.EmojiId = null;
            customStatus.EmojiName = null;
            if (string.IsNullOrEmpty(txt))
            {
                if (!string.IsNullOrEmpty(Text.Text))
                {
                    pth.Visibility = Visibility.Hidden;
                    Placeholder.Visibility = Visibility.Visible;
                }
                emoj.Visibility = Visibility.Visible;
                emo.Text = "";
                imga.Source = null;
            }
            else
            {
                var testt = UnicodeStringParser.ToCodePoints(txt);

                var a = string.Join("-", testt);

                if (!UnicodeStringParser.IsValidEmoji(a))
                {
                    a = testt.FirstOrDefault();
                }

                if (txt[0] == '<' && txt.Length > 1)
                {
                    emoj.Visibility = Visibility.Hidden;
                    emo.Text = "";
                    var aa = txt.Split(':', '>');
                    if (txt[1] == 'a')
                    {
                        var image = new BitmapImage();
                        image.DownloadCompleted += (sen, ev) =>
                        {
                            ImageBehavior.SetAnimatedSource(imga, image);
                        };
                        image.BeginInit();
                        image.UriSource = new Uri($"https://cdn.discordapp.com/emojis/{aa[2]}.gif");
                        image.EndInit();
                        imga.Source = image;
                        customStatus.EmojiName = aa[1];
                        customStatus.EmojiId = aa[2];
                        customStatus.Animated = true;
                    }
                    else
                    {
                        imga.Source = new BitmapImage(new Uri($"https://cdn.discordapp.com/emojis/{aa[2]}.png"));
                        customStatus.EmojiName = aa[1];
                        customStatus.EmojiId = aa[2];
                    }
                }
                else if (!string.IsNullOrEmpty(a))
                {
                    emoj.Visibility = Visibility.Hidden;
                    emo.Text = "";
                    imga.Source = new BitmapImage(new Uri($"https://twemoji.maxcdn.com/v/latest/72x72/{a}.png"));
                    customStatus.EmojiName = txt;
                }
                else
                {
                    emoj.Visibility = Visibility.Hidden;
                    imga.Source = null;
                    emo.Text = txt;
                    customStatus.EmojiName = txt;
                }
                if (!(pth is null)) pth.Visibility = Visibility.Visible;
                Placeholder.Visibility = Visibility.Hidden;
            }
        }

        private void TimeSecondsChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                customStatus.Time = uint.Parse((sender as TextBox).Text);
            }
            catch
            {

            }
        }

        private void TextusDeletus(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement).IsMouseOver)
            {
                Text.Text = "";
                customStatus.Text = "";
                customStatus.EmojiId = null;
                customStatus.EmojiName = null;
                emoj.Visibility = Visibility.Visible;
                emo.Text = "";
                imga.Source = null;
                pth.Visibility = Visibility.Hidden;
            }
        }

        private void Unhoover(object sender, MouseEventArgs e)
        {
            (sender as FrameworkElement).Opacity = 0.6;
        }

        private void Hoover(object sender, MouseEventArgs e)
        {
            (sender as FrameworkElement).Opacity = 1;
        }

        private void Ggggggg(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty((sender as TextBox).Text))
            {
                pth.Visibility = Visibility.Hidden;
                Placeholder.Visibility = Visibility.Visible;
                customStatus.Text = "";
            }
            else
            {
                if (!(pth is null)) pth.Visibility = Visibility.Visible;
                Placeholder.Visibility = Visibility.Hidden;
                customStatus.Text = (sender as TextBox).Text;
            }
        }

        private void Emoj_MouseEnter(object sender, MouseEventArgs e)
        {
            emoj.Opacity = 1;
            emoj.RenderTransform = new ScaleTransform(1.1, 1.1);
        }

        private void Emoj_MouseLeave(object sender, MouseEventArgs e)
        {
            emoj.Opacity = 0.35;
            emoj.RenderTransform = new ScaleTransform(1, 1);
        }
    }
}
