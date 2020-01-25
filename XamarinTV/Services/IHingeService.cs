using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinTV.Services
{

	public class CompactModeArgs : EventArgs
	{
		public Func<Task> Close { get; set; }
		public bool Success { get; set; }
	}

    public interface IHingeService : INotifyPropertyChanged, IDisposable
	{
		event EventHandler<HingeEventArgs> OnHingeUpdated;

		bool IsSpanned { get; }

		bool IsLandscape { get; }

		Rectangle GetHinge();

		Task<CompactModeArgs> OpenCompactMode(ContentPage contentPage);
		Task<bool> PromoteToCompactMode();
		Task<bool> DemoteFromCompactMode();
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
