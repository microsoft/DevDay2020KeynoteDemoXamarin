using XamarinTV.ViewModels;
using Xamarin.Forms;

namespace XamarinTV.Views
{
    public partial class VideoDetailView : ContentView
    {
        public VideoDetailView()
        {
            InitializeComponent();
        }

        void OnContentViewBindingContextChanged(object sender, System.EventArgs e)
        {
            //if(MainViewModel.Instance.TwoPaneViewMode != TwoPaneViewMode.SinglePane)
            //{
            //    ContentRow.Height = new GridLength(0, GridUnitType.Star);
            //}
            //else
            //{
            //    ContentRow.Height = new GridLength(2, GridUnitType.Star);
            //}
        }
    }
}
