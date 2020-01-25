using XamarinTV.Models;
using XamarinTV.Services;
using XamarinTV.ViewModels.Base;
using XamarinTV.Views;
using System;
using Xamarin.Forms;

namespace XamarinTV.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        BaseViewModel _pane1;
        BaseViewModel _pane2;

        readonly Lazy<BrowseVideosViewModel> _browseVideosViewModel;
        readonly Lazy<SearchVideosViewModel> _searchVideosViewModel;
        readonly Lazy<TopVideosViewModel> _topVideosViewModel;
        readonly Lazy<VideoPlayerViewModel> _videoPlayerViewModel;
        readonly Lazy<SettingsViewModel> _settingsViewModel;
        readonly Lazy<VideoDetailViewModel> _videoDetailViewModel = new Lazy<VideoDetailViewModel>(() => new VideoDetailViewModel());
        static readonly Lazy<MainViewModel> _mainViewModel = new Lazy<MainViewModel>(() => new MainViewModel());

        public static MainViewModel Instance => _mainViewModel.Value;
        public BrowseVideosViewModel BrowseVideosViewModel => _browseVideosViewModel.Value;
        public SearchVideosViewModel SearchVideosViewModel => _searchVideosViewModel.Value;
        public TopVideosViewModel TopVideosViewModel => _topVideosViewModel.Value;
        public VideoPlayerViewModel VideoPlayerViewModel => _videoPlayerViewModel.Value;
        public SettingsViewModel SettingsViewModel => _settingsViewModel.Value;
        public VideoDetailViewModel VideoDetailViewModel => _videoDetailViewModel.Value;

        TwoPaneViewTallModeConfiguration _tallModeConfiguration;
        TwoPaneViewWideModeConfiguration _wideModeConfiguration;
        TwoPaneViewMode _twoPaneViewMode;
        bool _isLandscape;



        public Command<Video> PlayVideoCommand { get; }
        public Command OpenSettingWindowCommand { get; }

        private MainViewModel()
        {
            _browseVideosViewModel = new Lazy<BrowseVideosViewModel>(OnCreateBrowseVideosViewModel);
            _searchVideosViewModel = new Lazy<SearchVideosViewModel>(OnCreateSearchVideosViewModel);
            _topVideosViewModel = new Lazy<TopVideosViewModel>(OnCreateTopVideosViewModel);
            _videoPlayerViewModel = new Lazy<VideoPlayerViewModel>(OnCreateVideoPlayerViewModel);
            _settingsViewModel = new Lazy<SettingsViewModel>(OnCreateSettingsViewModel);

            PlayVideoCommand = new Command<Video>(OnPlayVideo);
            OpenSettingWindowCommand = new Command(OpenSettingWindow);
            UpdateLayouts();
        }

        public BaseViewModel Pane1
        {
            get => _pane1;
            set
            {
                if (SetProperty(ref _pane1, value))
                {
                    _pane1?.OnDisappearing();
                    value?.OnAppearing();
                }
            }
        }

        public BaseViewModel Pane2
        {
            get => _pane2;
            set
            {
                if (SetProperty(ref _pane2, value))
                {
                    _pane2?.OnDisappearing();
                    value?.OnAppearing();
                    UpdateLayouts();
                }
            }
        }

        public TwoPaneViewTallModeConfiguration TallModeConfiguration
        {
            get => _tallModeConfiguration;
            set => SetProperty(ref _tallModeConfiguration, value);
        }

        public TwoPaneViewWideModeConfiguration WideModeConfiguration
        {
            get => _wideModeConfiguration;
            set => SetProperty(ref _wideModeConfiguration, value);
        }
        
        public TwoPaneViewMode TwoPaneViewMode
        {
            get => _twoPaneViewMode;
            set
            {
                if(SetProperty(ref _twoPaneViewMode, value))
                {
                    UpdateLayouts();
                }
            }
        }

        public bool IsLandscape
        {
            private get => _isLandscape;
            set
            {
                if (SetProperty(ref _isLandscape, value))
                {
                    UpdateLayouts();
                }
            }
        }

        void UpdateLayouts()
        {
            if (VideoPlayerViewModel.Video != null)
            {
                if (TwoPaneViewMode == TwoPaneViewMode.SinglePane)
                    VideoDetailViewModel.TopViewModel = VideoPlayerViewModel;
                else
                    VideoDetailViewModel.TopViewModel = null;

                BrowseVideosViewModel.TopViewModel = null;
            }
            else
            {
                BrowseVideosViewModel.TopViewModel = TopVideosViewModel;
                VideoDetailViewModel.TopViewModel = null;
            }

            if (TwoPaneViewMode == TwoPaneViewMode.SinglePane)
            {
                Pane2 = null;
                if (VideoPlayerViewModel.Video != null)
                {
                    if (!IsLandscape)
                        Pane1 = VideoDetailViewModel;
                    else
                        Pane1 = VideoPlayerViewModel;
                }
                else
                {
                    Pane1 = BrowseVideosViewModel;
                }
            }
            else
            {
                if (VideoPlayerViewModel.Video != null)
                {
                    Pane1 = VideoDetailViewModel;
                    Pane2 = VideoPlayerViewModel;
                }
                else
                {
                    Pane1 = SearchVideosViewModel;
                    Pane2 = BrowseVideosViewModel;
                }
            }

            if (Pane2 == null)
            {
                TallModeConfiguration = TwoPaneViewTallModeConfiguration.TopBottom;
                WideModeConfiguration = TwoPaneViewWideModeConfiguration.LeftRight;
            }
            else
            {
                TallModeConfiguration = TwoPaneViewTallModeConfiguration.BottomTop;
                WideModeConfiguration = TwoPaneViewWideModeConfiguration.RightLeft;
            }
        }

        public override void OnFirstAppearing()
        {
            base.OnFirstAppearing();
            Pane1 = BrowseVideosViewModel;
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            Pane1?.OnAppearing();
            Pane2?.OnAppearing();
        }

        BrowseVideosViewModel OnCreateBrowseVideosViewModel()
        {
            BrowseVideosViewModel viewModel = new BrowseVideosViewModel();
            return viewModel;
        }

        SearchVideosViewModel OnCreateSearchVideosViewModel()
        {
            SearchVideosViewModel viewModel = new SearchVideosViewModel();
            return viewModel;
        }

        TopVideosViewModel OnCreateTopVideosViewModel()
        {
            TopVideosViewModel viewModel = new TopVideosViewModel();
            return viewModel;
        }

        VideoPlayerViewModel OnCreateVideoPlayerViewModel()
        {
            VideoPlayerViewModel viewModel = new VideoPlayerViewModel
            {
                CloseCommand = new Command(OnClosePlayingVideo)
            };
            return viewModel;
        }

        async void OnClosePlayingVideo(object obj)
        {
            // Stop the videos
            await VideoPlayerViewModel.StopVideoAsync();

            VideoPlayerViewModel.Video = null;
            VideoDetailViewModel.SelectedVideo = null;

            UpdateLayouts();
        }
        
        async void OnPlayVideo(Video video)
        {
            VideoPlayerViewModel.Video = video;
            VideoDetailViewModel.SelectedVideo = video;
            UpdateLayouts();

            await VideoPlayerViewModel.PlayVideoAsync();
        }

        SettingsViewModel OnCreateSettingsViewModel()
        {
            var viewModel = new SettingsViewModel
            {
                CloseCommand = new Command(() =>
                {
                    UpdateLayouts();
                })
            };

            return viewModel;
        }

        public void OpenSettingWindow()
        {
            Pane1 = SettingsViewModel;
        }

        public void OpenVideoPlayerWindow(Video video)
        {
            OnPlayVideo(video);
        }
    }
}