using FFImageLoading.Forms.Platform;
using MediaManager;
using XamarinTV.ViewModels;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace XamarinTV.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            CachedImageRenderer.Init();
            CrossMediaManager.Current.Init();

            LoadApplication(new XamarinTV.App());

            NativeCustomize();

            Task.Run(async () =>
            {
                try
                {
                    await CrossMediaManager.Windows.Extractor.CreateMediaItemFromAssembly("XamarinTV.Resources.video.mp4", typeof(VideoPlayerViewModel).Assembly);
                }
                catch
                {

                }
                finally { }

            });
        }

        void NativeCustomize()
        {
            // PC Customization
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.BackgroundColor = (Color)App.Current.Resources["NativeAccentColor"];
                    titleBar.ButtonBackgroundColor = (Color)App.Current.Resources["NativeAccentColor"];
                }
            }
        }
    }
}