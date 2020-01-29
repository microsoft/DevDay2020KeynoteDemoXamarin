using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.DualScreen;

[assembly: Dependency(typeof(LayoutService))]
namespace Xamarin.Forms.DualScreen
{
    public class LayoutService : ILayoutService
    {
        public Point? GetLocationOnScreen(VisualElement visualElement)
        {
            var view = Platform.Android.Platform.GetRenderer(visualElement);

            if (view?.View == null)
                return null;

            int[] location = new int[2];
            view.View.GetLocationOnScreen(location);
            return new Point(view.View.Context.FromPixels(location[0]), view.View.Context.FromPixels(location[1]));
        }
    }
}