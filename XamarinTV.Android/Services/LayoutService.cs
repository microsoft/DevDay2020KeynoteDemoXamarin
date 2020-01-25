using XamarinTV.Android.Services;
using XamarinTV.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(LayoutService))]
namespace XamarinTV.Android.Services
{
    public class LayoutService : LayoutServiceBase, ILayoutService
    {
        public override Point? GetLocationOnScreen(VisualElement visualElement)
        {
            var view = Platform.GetRenderer(visualElement);

            if (view?.View == null)
                return null;

            int[] location = new int[2];
            view.View.GetLocationOnScreen(location);
            return new Point(view.View.Context.FromPixels(location[0]), view.View.Context.FromPixels(location[1]));
        }
    }
}