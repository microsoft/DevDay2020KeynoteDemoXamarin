using XamarinTV.Services;
using XamarinTV.UWP;
using Windows.UI.Xaml;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: Dependency(typeof(LayoutService))]
namespace XamarinTV.UWP
{
    public class LayoutService : LayoutServiceBase, ILayoutService
    {
        public override Point? GetLocationOnScreen(VisualElement visualElement)
        {
            var view = Platform.GetRenderer(visualElement);

            if (view?.ContainerElement == null)
                return null;

            var ttv = view.ContainerElement.TransformToVisual(Window.Current.Content);
            Windows.Foundation.Point screenCoords = ttv.TransformPoint(new Windows.Foundation.Point(0, 0));

            return new Point(screenCoords.X, screenCoords.Y);
        }
    }
}