using Xamarin.Forms;

namespace XamarinTV.Views
{
    public partial class BrowseVideosView : ContentView
    {
        public BrowseVideosView()
        {
            InitializeComponent();
        }

        protected override void InvalidateLayout()
        {
            base.InvalidateLayout();
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            return base.OnMeasure(widthConstraint, heightConstraint);
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);
        }
    }
}
