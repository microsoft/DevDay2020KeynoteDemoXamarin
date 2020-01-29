using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin.Forms.DualScreen
{
    public interface IHingeService : IDisposable
	{
		event EventHandler<HingeEventArgs> OnHingeUpdated;

		bool IsSpanned { get; }

		bool IsLandscape { get; }

		Rectangle GetHinge();
	}

	public class HingeEventArgs : EventArgs
	{
		public HingeEventArgs(int angle)
			: base()
		{
			Angle = angle;
		}

		public int Angle { get; private set; }
	}
}
