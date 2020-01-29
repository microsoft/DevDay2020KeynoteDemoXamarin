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
            return false;
        }

        public static Task<CompactModeArgs> OpenCompactMode(ContentPage contentPage)
        {
            return Task.FromResult(new CompactModeArgs(null, false));
        }
    }
}
