using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Hosting;
using Xamarin.Forms.Platform.UWP;

namespace Xamarin.Forms.DualScreen
{
    public static class WindowHelper
    {
        static double ActualPixels(double n)
            => n * DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

        static double ScaledPixels(double n)
            => n / DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

        public static bool IsCompactModeSupport()
        {
            return ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay);
        }

        public static async Task<CompactModeArgs> OpenCompactMode(ContentPage contentPage)
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
            CompactModeArgs args = null;

            // 3. Check if you can leverage the compact overlay APIs
            if (appWindow.Presenter.IsPresentationSupported(AppWindowPresentationKind.CompactOverlay))
            {
                // 4. Show the window
                bool result = await appWindow.TryShowAsync();

                if (result)
                {
                    appWindow.Presenter.RequestPresentation(AppWindowPresentationKind.CompactOverlay);
                    args = new CompactModeArgs(async () =>
                    {
                        await appWindow.CloseAsync();
                    }, true);

                }
            }
            

            if(args == null)
            {
                args = new CompactModeArgs(null, false);
            }

            return args;
        }
    }
}
