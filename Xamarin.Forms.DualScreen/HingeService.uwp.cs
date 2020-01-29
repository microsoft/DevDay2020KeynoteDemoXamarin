using System;
using System.ComponentModel;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Xamarin.Forms;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Hosting;
using Xamarin.Forms.Platform.UWP;
using System.Threading.Tasks;
using Xamarin.Forms.DualScreen;

[assembly: Dependency(typeof(HingeService))]
namespace Xamarin.Forms.DualScreen
{
    public class HingeService : IHingeService
    {
        ILayoutService LayoutService => DependencyService.Get<ILayoutService>();
        public HingeService()
        {
        }

        void FireOnHingeUpdate()
        {
            LayoutService.AddLayoutGuide("Hinge", GetHinge());
            OnHingeUpdated?.Invoke(this, new HingeEventArgs(-1));
        }

        public bool IsSpanned
        {
            get
            {
                var visibleBounds = ApplicationView.GetForCurrentView().VisibleBounds;

                if (visibleBounds.Height > 1200 || visibleBounds.Width > 1200)
                    return true;

                return false;
            }
        }

        public bool IsLandscape
        {
            get
            {
                //when you have it spanned in double landscape it thinks that it's portrait
                // and visa versa so here's my hack for now to get the correct values for this
                if (IsSpanned)
                    return ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Portrait;
                else
                    return ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Landscape;
            }
        }

        public event EventHandler<HingeEventArgs> OnHingeUpdated;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
        }

        public Rectangle GetHinge()
        {
            var screen = DisplayInformation.GetForCurrentView();

            if (IsLandscape)
            {
                if (IsSpanned)
                    return new Rectangle(0, 664 + 24, ScaledPixels(screen.ScreenWidthInRawPixels), 0);
                else
                    return new Rectangle(0, 664, ScaledPixels(screen.ScreenWidthInRawPixels), 0);
            }
            else
                return new Rectangle(720, 0, 0, ScaledPixels(screen.ScreenHeightInRawPixels));
        }

        double ActualPixels(double n)
            => n * DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

        double ScaledPixels(double n)
            => n / DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
    }
}