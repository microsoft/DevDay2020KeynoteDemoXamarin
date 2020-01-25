using System.Collections.ObjectModel;
using System.Threading.Tasks;
using XamarinTV.Models;
using XamarinTV.Services;
using XamarinTV.ViewModels.Base;

namespace XamarinTV.ViewModels
{
    public class FeaturedVideosViewModel : BaseViewModel
    {
        ObservableCollection<VideoGroup> _videos;

        public FeaturedVideosViewModel()
        {
            LoadVideoGroupsAsync();
        }

        public ObservableCollection<VideoGroup> Videos
        {
            get { return _videos; }
            set { SetProperty(ref _videos, value); }
        }

        async void LoadVideoGroupsAsync()
        {
            IsBusy = true;

            var videoGroups = await XamarinTvService.Instance.GetVideoGroupsAsync();

            Videos = new ObservableCollection<VideoGroup>();

            foreach (var videoGroup in videoGroups)
            {
                Videos.Add(videoGroup);
            }

            await Task.Delay(3000);

            IsBusy = false;
        }
    }
}