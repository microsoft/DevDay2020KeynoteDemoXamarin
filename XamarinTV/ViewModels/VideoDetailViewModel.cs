using XamarinTV.Models;
using XamarinTV.ViewModels.Base;

namespace XamarinTV.ViewModels
{
    public class VideoDetailViewModel : BaseViewModel
    {
        Video _selectedVideo;
        int _selectedViewModelIndex;

        public Video SelectedVideo
        {
            get => _selectedVideo;
            set => SetProperty(ref _selectedVideo, value);
        }

        public string Title
        {
            get { return _selectedVideo.Title; }
        }

        public string ViewCount
        {
            get { return $"{_selectedVideo.ViewCount} views"; }
        }

        public string Description
        {
            get { return _selectedVideo.Description; }
        }

        public int SelectedViewModelIndex
        {
            get { return _selectedViewModelIndex; }
            set { SetProperty(ref _selectedViewModelIndex, value); }
        }
    }
}