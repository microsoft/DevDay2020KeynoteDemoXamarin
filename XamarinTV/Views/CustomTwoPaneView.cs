using Xamarin.Forms;

namespace XamarinTV.Views
{
    public class CustomTwoPaneView : TwoPaneView
    {
        public override Rectangle[] TransformBoundsBeforeLayout(Rectangle pane1, Rectangle pane2)
        {
            /*
            if (!IsSpanned)
            {
                var newWidth = pane1.Width / 3;
                var newHeight = pane1.Width / 3;

                pane2 = new Rectangle(pane1.Width - newWidth - 20, pane1.Height - newHeight - 20, newWidth, newHeight);
            }
            */

            return new[] { pane1, pane2 };
        }
    }
}
