using System;
using System.ComponentModel;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Xamarin.Forms;
using XamarinTV.Services;
using XamarinTV.UWP;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Hosting;
using Xamarin.Forms.Platform.UWP;
using System.Threading.Tasks;

[assembly: Dependency(typeof(HingeService))]
namespace XamarinTV.UWP
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

        public async Task<bool> PromoteToCompactMode()
        {
            var applicationView = ApplicationView.GetForCurrentView();
            if (applicationView.IsViewModeSupported(ApplicationViewMode.CompactOverlay))
            {
                if (await applicationView.TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay))
                    return true;
            }

            return false;
        }

        public async Task<bool> DemoteFromCompactMode()
        {
            var applicationView = ApplicationView.GetForCurrentView();
            if (applicationView.IsViewModeSupported(ApplicationViewMode.Default))
            {
                if (await applicationView.TryEnterViewModeAsync(ApplicationViewMode.Default))
                    return true;
            }

            return false;
        }

        public async Task<CompactModeArgs> OpenCompactMode(ContentPage contentPage)
        {
            // 1. Create a new Window
            AppWindow appWindow = await AppWindow.TryCreateAsync();

            var frameworkElement = contentPage.CreateFrameworkElement();

            Windows.UI.Xaml.Controls.Frame frame = new Windows.UI.Xaml.Controls.Frame()
            {
                Content = frameworkElement
            };

            frame.SizeChanged += (_, __) =>
            {
                contentPage.HeightRequest = ScaledPixels(frame.ActualWidth);
                contentPage.WidthRequest = ScaledPixels(frame.ActualHeight);

                var content = contentPage.Content as Layout;

                Layout.LayoutChildIntoBoundingRegion(content,
                    new Rectangle(0, 0, ScaledPixels(frame.ActualWidth), ScaledPixels(frame.ActualHeight)));
            };

            // 2. Create the pageand set the new window's content
            ElementCompositionPreview.SetAppWindowContent(appWindow, frame);
            CompactModeArgs args = new CompactModeArgs();
            args.Close = () =>
            {
                return Task.CompletedTask;
            };

            // 3. Check if you can leverage the compact overlay APIs
            if (appWindow.Presenter.IsPresentationSupported(AppWindowPresentationKind.CompactOverlay))
            {
                // 4. Show the window
                bool result = await appWindow.TryShowAsync();

                //appWindow.Changed += (a, e) =>
                //{
                //    var regions = appWindow.GetDisplayRegions()[0];
                //    contentPage.HeightRequest = ScaledPixels(regions.WorkAreaSize.Height);
                //    contentPage.WidthRequest = ScaledPixels(regions.WorkAreaSize.Width);
                //};

                if (result)
                {
                    args.Close = async () =>
                    {
                        await appWindow.CloseAsync();
                    };

                    args.Success = true;
                    appWindow.Presenter.RequestPresentation(AppWindowPresentationKind.CompactOverlay);
                }
            }

            return args;
        }
    }
}