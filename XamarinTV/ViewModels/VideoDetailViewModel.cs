using XamarinTV.Models;
using XamarinTV.ViewModels.Base;

namespace XamarinTV.ViewModels
{
    public class VideoDetailViewModel : BaseViewModel
    {
        Video _selectedVideo;
        BaseViewModel _topViewModel;
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

        public BaseViewModel TopViewModel
        {
            get => _topViewModel;
            set { SetProperty(ref _topViewModel, value); }
        }

        public int SelectedViewModelIndex
        {
            get { return _selectedViewModelIndex; }
            set { SetProperty(ref _selectedViewModelIndex, value); }
        }
    }
}