using System;
using System.Collections.Generic;
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

namespace DiscordCustomStatus
{
    /// <summary>
    /// Huge failure, please ignore
    /// </summary>
    public partial class AnimatedImage : Image
    {
        public Uri SourceUri;
        public AnimatedImage() :base()
        {
        }

        protected override void OnRender(DrawingContext dc)
        {
            ImageSource imageSource = Source;
            if (imageSource == null)
            {
                return;
            }
            MediaPlayer a = new MediaPlayer();
            a.ScrubbingEnabled = true;
            a.Open(SourceUri);
            //computed from the ArrangeOverride return size 
            dc.DrawVideo(a, new Rect(new Point(), RenderSize));
            a.MediaOpened += new EventHandler((s, e) => {
                a.Play();
                Console.WriteLine(a.NaturalDuration);
            });
            a.MediaEnded += new EventHandler((s, e) => {
                a.Open(SourceUri);
            });
        }

        private void OnSourceDownloaded(object sender, EventArgs e)
        {
            InvalidateMeasure();
            InvalidateVisual(); //ensure re-rendering
        }

        private void OnSourceFailed(object sender, ExceptionEventArgs e)
        {
            SetCurrentValue(SourceProperty, null);
            //RaiseEvent(new ExceptionRoutedEventArgs(ImageFailedEvent, this, e.ErrorException));
        }

        #region WeakBitmapSourceEvents 

        /// <summary> 
        ///     WeakBitmapSourceEvents acts as a proxy between events on BitmapSource 
        ///     and handlers on Image.  It is used to respond to events on BitmapSource
        ///     without preventing Image's to be collected. 
        /// </summary>
        private class WeakBitmapSourceEvents : WeakReference
        {
            public WeakBitmapSourceEvents(Image image, BitmapSource bitmapSource) : base(image)
            {
                _bitmapSource = bitmapSource;
                _bitmapSource.DownloadCompleted += this.OnSourceDownloaded;
                _bitmapSource.DownloadFailed += this.OnSourceFailed;
                _bitmapSource.DecodeFailed += this.OnSourceFailed;
            }

            public void OnSourceDownloaded(object sender, EventArgs e)
            {
                if (this.Target is AnimatedImage image)
                {
                    image.OnSourceDownloaded(image, e);
                }
                else
                {
                    Dispose();
                }
            }

            public void OnSourceFailed(object sender, ExceptionEventArgs e)
            {
                if (this.Target is AnimatedImage image)
                {
                    image.OnSourceFailed(image, e);
                }
                else
                {
                    Dispose();
                }
            }

            public void Dispose()
            {
                // We can't remove hanlders from frozen sources. 
                if (!_bitmapSource.IsFrozen)
                {
                    _bitmapSource.DownloadCompleted -= this.OnSourceDownloaded;
                    _bitmapSource.DownloadFailed -= this.OnSourceFailed;
                    _bitmapSource.DecodeFailed -= this.OnSourceFailed;
                }
            }

            private BitmapSource _bitmapSource;
        }

        #endregion WeakBitmapSourceEvents 

    }
}
