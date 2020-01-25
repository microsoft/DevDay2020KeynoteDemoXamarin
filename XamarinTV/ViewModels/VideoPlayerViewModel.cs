using System.Threading.Tasks;
using System.Windows.Input;
using MediaManager;
using XamarinTV.Models;
using XamarinTV.ViewModels.Base;
using Xamarin.Forms;

namespace XamarinTV.ViewModels
{
    public class VideoPlayerViewModel : BaseViewModel
    {
        Video _video;

        public Video Video
        {
            get { return _video; }
            set { SetProperty(ref _video, value); }
        }

        public ICommand CloseCommand { get; set; }

        public async Task PlayVideoAsync()
        {
            if (Video != null)
            {
                IsBusy = true;

                // TODO: The video playback fails in UWP, until review and fix the issue, the video will not work in UWP to avoid the exception.
                if (Device.RuntimePlatform != Device.UWP)
                {
                    var mediaItem = await CrossMediaManager.Current.PlayFromAssembly("XamarinTV.Resources.video.mp4", typeof(VideoPlayerViewModel).Assembly);

                    mediaItem.DisplayTitle = Video.Title;
                    mediaItem.Year = Video.ReleaseDate.Year;
                }
                else
                {
                    var item = await CrossMediaManager.Current.Extractor.CreateMediaItemFromResource("/Assets/video.mp4");
                    var mediaItem = await CrossMediaManager.Current.PlayFromResource("ms-appx:///Assets/video.mp4");

                    mediaItem.DisplayTitle = Video.Title;
                    mediaItem.Year = Video.ReleaseDate.Year;
                }

                IsBusy = false;
            }
        }

        public async Task StopVideoAsync()
        {
            await CrossMediaManager.Current.Stop();
        }
    }
}