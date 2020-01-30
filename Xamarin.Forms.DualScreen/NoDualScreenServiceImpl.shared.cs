using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.DualScreen
{
    internal class NoDualScreenServiceImpl : IDualScreenService
    {
        static Lazy<NoDualScreenServiceImpl> _Instance = new Lazy<NoDualScreenServiceImpl>(() => new NoDualScreenServiceImpl());
        public static NoDualScreenServiceImpl Instance => _Instance.Value;

        private NoDualScreenServiceImpl()
        {
        }

        public bool IsSpanned => false;

        public bool IsLandscape => Device.info.CurrentOrientation.IsLandscape();

        public event EventHandler OnScreenChanged
        {
            add
            {

            }
            remove
            {

            }
        }

        public void Dispose()
        {
        }

        public Rectangle GetHinge()
        {
            return Rectangle.Zero;
        }

        public Point? GetLocationOnScreen(VisualElement visualElement)
        {
            return null;
        }

        public bool HasCompactModeSupport()
        {
            return false;
        }

        public Task<CompactModeArgs> OpenCompactMode(ContentPage contentPage)
        {
            return Task.FromResult(new CompactModeArgs(null, false));
        }
    }
}
