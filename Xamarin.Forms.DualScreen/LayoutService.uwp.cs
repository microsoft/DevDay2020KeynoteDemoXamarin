
using Windows.UI.Xaml;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Xamarin.Forms.DualScreen;

[assembly: Dependency(typeof(LayoutService))]
namespace Xamarin.Forms.DualScreen
{
    public class LayoutService : LayoutServiceBase, ILayoutService
    {
        public override Point? GetLocationOnScreen(VisualElement visualElement)
        {
            var view = Platform.UWP.Platform.GetRenderer(visualElement);

            if (view?.ContainerElement == null)
                return null;

            var ttv = view.ContainerElement.TransformToVisual(Window.Current.Content);
            Windows.Foundation.Point screenCoords = ttv.TransformPoint(new Windows.Foundation.Point(0, 0));

            return new Point(screenCoords.X, screenCoords.Y);
        }
    }
}