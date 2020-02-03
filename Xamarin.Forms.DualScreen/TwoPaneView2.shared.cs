using System;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms.DualScreen
{
    public partial class TwoPaneView
	{

		static TwoPaneView()
		{
#if UWP
			// Without this the service gets linked out when compiling with the native tool chain on UWP
			DependencyService.Register<DualScreenService>();
#elif !ANDROID
			DependencyService.Register<NoDualScreenServiceImpl>();
#endif
		}


        //TODO replace with OnIsPlatformEnabledChanged once we have NUGET and IVT is turned on
        bool _isconnected = false;
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (IsPlatformEnabled)
            {
                if (!_isconnected)
                {
                    _isconnected = true;
                    TwoPaneViewLayoutGuide.WatchForChanges();
                    TwoPaneViewLayoutGuide.PropertyChanged += OnTwoPaneViewLayoutGuide;
                }
            }
            else
            {
                if (_isconnected)
                {
                    _isconnected = false;
                    TwoPaneViewLayoutGuide.StopWatchingForChanges();
                    TwoPaneViewLayoutGuide.PropertyChanged -= OnTwoPaneViewLayoutGuide;
                }
            }
        }
    }
}
