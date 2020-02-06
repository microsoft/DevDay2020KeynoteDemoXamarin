using System.Windows.Input;
using XamarinTV.Models;
using XamarinTV.ViewModels.Base;

namespace XamarinTV.ViewModels
{
    public class VideoPlayerViewModel : BaseViewModel
    {
        Video _video;

        public Video Video
        {
            get { return _video; }
            set
            {
                SetProperty(ref _video, value);
            }
        }

        public string VideoSource
        {
            get
            {
                return "video.mp4";
            }
        }

        public ICommand CloseCommand { get; set; }
    }
}