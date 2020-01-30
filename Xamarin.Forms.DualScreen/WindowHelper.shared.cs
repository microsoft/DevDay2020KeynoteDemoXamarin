using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.DualScreen
{
    public static class WindowHelper
    {
        public static bool HasCompactModeSupport()
        {
            return (DependencyService.Get<IDualScreenService>() ?? NoDualScreenServiceImpl.Instance).HasCompactModeSupport(); ;
        }

        public static Task<CompactModeArgs> OpenCompactMode(ContentPage contentPage)
        {
            return (DependencyService.Get<IDualScreenService>() ?? NoDualScreenServiceImpl.Instance).OpenCompactMode(contentPage);
        }

        public static Rectangle[] GetSpanningRects()
        {
            var guide = TwoPaneViewLayoutGuide.Instance;

            if (guide.Hinge == Rectangle.Zero)
                return new Rectangle[0];

            guide.UpdateLayouts();
            return new []{ guide.Pane1, guide.Pane2 };
        }

        public static Rectangle GetHingeRect()
        {
            var guide = TwoPaneViewLayoutGuide.Instance;
            guide.UpdateLayouts();
            return guide.Hinge;
        }
    }
}
