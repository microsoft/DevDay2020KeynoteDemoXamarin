using XamarinTV.Models;
using XamarinTV.Services;
using XamarinTV.ViewModels.Base;
using XamarinTV.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.DualScreen;

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
        double minWideModeWidth;
        double minTallModeHeight;
        private GridLength pane1Length;
        private GridLength pane2Length;

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
                if (SetProperty(ref _twoPaneViewMode, value))
                {
                    UpdateLayouts();
                }
            }
        }

        public double MinWideModeWidth
        {
            get => minWideModeWidth;
            set => SetProperty(ref minWideModeWidth, value);
        }

        public double MinTallModeHeight
        {
            get => minTallModeHeight;
            set => SetProperty(ref minTallModeHeight, value);
        }

        public GridLength Pane1Length
        {
            get => pane1Length;
            set => SetProperty(ref pane1Length, value);
        }

        public GridLength Pane2Length
        {
            get => pane2Length;
            set => SetProperty(ref pane2Length, value);
        }

        void UpdateLayouts()
        {
            Pane2Length = GridLength.Star;
            if (VideoPlayerViewModel.Video != null)
            {
                MinTallModeHeight = 600;
                MinWideModeWidth = 4000;
                Pane1Length = GridLength.Auto;
                Pane1 = VideoPlayerViewModel;
                Pane2 = VideoDetailViewModel;
                TallModeConfiguration = TwoPaneViewTallModeConfiguration.TopBottom;
                WideModeConfiguration = TwoPaneViewWideModeConfiguration.SinglePane;
            }
            else
            {
                Pane1Length = new GridLength(2, GridUnitType.Star);
                MinTallModeHeight = 0;
                MinWideModeWidth = 4000;
                Pane1 = TopVideosViewModel;
                Pane2 = BrowseVideosViewModel;
                TallModeConfiguration = TwoPaneViewTallModeConfiguration.TopBottom;
                WideModeConfiguration = TwoPaneViewWideModeConfiguration.LeftRight;
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
            var viewModel = new SettingsViewModel();

            return viewModel;
        }

        public async void OpenSettingWindow()
        {
            if (!WindowHelper.HasCompactModeSupport())
            {
                if (SettingsViewModel.CloseCommand == null)
                    SettingsViewModel.CloseCommand = new Command(() => UpdateLayouts());

                Pane1 = SettingsViewModel;
                return;
            }

            if (SettingsViewModel.CloseCommand != null)
                return;

            Views.SettingsView settingsView = new SettingsView()
            {
            };

            var closeMeArgs = await WindowHelper.OpenCompactMode(new ContentPage()
            {
                Content = settingsView
            });

            SettingsViewModel.CloseCommand = new Command(async () =>
            {
                SettingsViewModel.CloseCommand = null;
                await closeMeArgs.Close();
            });

            settingsView.BindingContext = SettingsViewModel;
        }

        public void OpenVideoPlayerWindow(Video video)
        {
            OnPlayVideo(video);
        }
    }
}