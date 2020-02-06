using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using XamarinTV.ViewModels;

namespace XamarinTV.Views
{
    public partial class VideoPlayerView : ContentView
    {
        Timer _inactivityTimer;
        Timer _playbackTimer;
        Rectangle _previousBounds;
        bool _playing = true;
        TimeSpan _currentPosition;
        bool _layoutChanged;

        public VideoPlayerView()
        {
            InitializeComponent();
            _inactivityTimer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds);
            _playbackTimer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
            VideoPlayer.PropertyChanged += OnVideoPlayerPropertyChanged;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (Parent == null)
            {
                _playbackTimer.Elapsed -= _playbackTimer_Elapsed;
                _playbackTimer.Stop();

                _inactivityTimer.Elapsed -= _inactivityTimer_Elapsed;
                _inactivityTimer.Stop();
            }
            else
            {
                _playbackTimer.Elapsed += _playbackTimer_Elapsed;
                _playbackTimer.Start();

                _inactivityTimer.Elapsed += _inactivityTimer_Elapsed;
                _inactivityTimer.Start();

            }
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);
            if (_previousBounds != this.Bounds)
            {
                _layoutChanged = true;
                PlayPause();
            }
        }

        void OnVideoPlayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(VideoPlayer.CurrentState) && 
                Parent != null &&
                _playing &&
                VideoPlayer.CurrentState == MediaElementState.Stopped && 
                _layoutChanged &&
                VideoPlayer.Source != null &&
                BindingContext is VideoPlayerViewModel vpm &&
                vpm.Video != null &&
                _currentPosition > VideoPlayer.Position)
            {
                _layoutChanged = false;
                VideoPlayer.Position = _currentPosition;
                VideoPlayer.Play();
            }
        }

        async void PlayPause()
        {
            _previousBounds = this.Bounds;
            if (VideoPlayer.CurrentState == MediaElementState.Playing)
            {
                VideoPlayer.Stop();
                await Task.Delay(1);
                VideoPlayer.Play();
            }
        }


        private void _playbackTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.UpdateTimeDisplay();
        }

        private async void _inactivityTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await Task.WhenAny<bool>
            (
                PlayerHUD.FadeTo(0)
            );

            _inactivityTimer.Stop();

            if (Parent != null)
                _inactivityTimer.Start();

        }

        private void MediaElement_StateRequested(object sender, StateRequested e)
        {
            VisualStateManager.GoToState(PlayPauseToggle,
                (e.State == MediaElementState.Playing)
                ? "playing"
                : "paused");

            if (e.State == MediaElementState.Playing)
            {
                _playbackTimer.Stop();

                if (Parent != null)
                    _playbackTimer.Start();

            }
            else if (e.State == MediaElementState.Paused || e.State == MediaElementState.Stopped)
            {
                _playbackTimer.Stop();
            }
        }

        void PlayPauseToggle_Clicked(object sender, EventArgs e)
        {
            if (VideoPlayer.CurrentState == MediaElementState.Playing)
            {
                _playing = false;
                VideoPlayer.Pause();
            }
            else
            {
                _playing = true;
                VideoPlayer.Play();
            }
        }

        async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {           
            if (PlayerHUD.Opacity == 1)
            {
                await Task.WhenAny<bool>
                (
                    PlayerHUD.FadeTo(0)
                );
            }
            else
            {
                await Task.WhenAny<bool>
                (
                    PlayerHUD.FadeTo(1, 100)
                );
            }

            _inactivityTimer.Stop();
            if (Parent != null)
                _inactivityTimer.Start();
        }

        void VideoPlayer_MediaOpened(object sender, EventArgs e)
        {
            UpdateTimeDisplay();
        }

        void UpdateTimeDisplay()
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
            {
                if(VideoPlayer.Position > _currentPosition)
                    _currentPosition = VideoPlayer.Position;

                TimeAndDuration.Text = $"{VideoPlayer.Position.ToString(@"hh\:mm\:ss")} / {VideoPlayer.Duration?.ToString(@"hh\:mm\:ss")}";
            });
        }
    }
}