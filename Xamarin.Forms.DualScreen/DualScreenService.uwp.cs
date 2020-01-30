using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Xamarin.Forms;
using Xamarin.Forms.DualScreen;
using Xamarin.Forms.Platform.UWP;

[assembly: Dependency(typeof(DualScreenService))]
namespace Xamarin.Forms.DualScreen
{
    internal class DualScreenService : IDualScreenService
    {
        public event EventHandler OnScreenChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public DualScreenService()
        {
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

        double ScaledPixels(double n)
            => n / DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

        public Point? GetLocationOnScreen(VisualElement visualElement)
        {
            var view = Platform.UWP.Platform.GetRenderer(visualElement);

            if (view?.ContainerElement == null)
                return null;

            var ttv = view.ContainerElement.TransformToVisual(Window.Current.Content);
            Windows.Foundation.Point screenCoords = ttv.TransformPoint(new Windows.Foundation.Point(0, 0));

            return new Point(screenCoords.X, screenCoords.Y);
        }


        public bool HasCompactModeSupport()
        {
            return ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay);
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

            // 2. Create the pageand set the new window's content
            ElementCompositionPreview.SetAppWindowContent(appWindow, frame);
            CompactModeArgs args = null;

            // 3. Check if you can leverage the compact overlay APIs
            if (appWindow.Presenter.IsPresentationSupported(AppWindowPresentationKind.CompactOverlay))
            {
                // 4. Show the window
                bool result = await appWindow.TryShowAsync();

                if (result)
                {
                    appWindow.Presenter.RequestPresentation(AppWindowPresentationKind.CompactOverlay);
                    frame.SizeChanged += OnFrameSizeChanged;

                    void OnFrameSizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
                    {
                        contentPage.HeightRequest = frame.ActualWidth;
                        contentPage.WidthRequest = frame.ActualHeight;

                        var content = contentPage.Content as Layout;

                        Layout.LayoutChildIntoBoundingRegion(content,
                            new Rectangle(0, 0, frame.ActualWidth, frame.ActualHeight));
                    }

                    args = new CompactModeArgs(async () =>
                    {
                        frame.SizeChanged -= OnFrameSizeChanged;
                        await appWindow.CloseAsync();
                    }, true);
                }
            }


            if (args == null)
            {
                args = new CompactModeArgs(null, false);
            }

            return args;
        }
    }
}