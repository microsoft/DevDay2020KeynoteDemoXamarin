using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;

#if UWP_18362
using Windows.UI.WindowManagement;
#endif

using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Xamarin.Forms;
using Xamarin.Forms.DualScreen;
using Xamarin.Forms.Platform.UWP;

[assembly: Dependency(typeof(DualScreenService))]
namespace Xamarin.Forms.DualScreen
{
    internal partial class DualScreenService : IDualScreenService
	{
#if UWP_18362

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
#endif
    }
}