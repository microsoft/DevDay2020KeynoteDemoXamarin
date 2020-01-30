using System.Threading.Tasks;
using System.Windows.Input;
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

        public MediaSource VideoSource
        {
            get
            {
                return MediaSource.FromFile("XamarinTV.Resources.video.mp4");
                
            }
        }

        public ICommand CloseCommand { get; set; }

        public async Task PlayVideoAsync()
        {

            if (Video != null)
            {

            }
                //    IsBusy = true;

                //    // TODO: The video playback fails in UWP, until review and fix the issue, the video will not work in UWP to avoid the exception.
                //    if (Device.RuntimePlatform != Device.UWP)
                //    {
                //        var mediaItem = await CrossMediaManager.Current.PlayFromAssembly("XamarinTV.Resources.video.mp4", typeof(VideoPlayerViewModel).Assembly);

                //        mediaItem.DisplayTitle = Video.Title;
                //        mediaItem.Year = Video.ReleaseDate.Year;
                //    }

                //    IsBusy = false;
                //}
            }

        public async Task StopVideoAsync()
        {
            //await CrossMediaManager.Current.Stop();
        }
    }
}