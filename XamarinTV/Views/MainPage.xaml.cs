using XamarinTV.ViewModels;
using XamarinTV.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinTV.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        double layoutWidth = 0;
        double layoutHeight = 0;
        public MainPage()
        {
            InitializeComponent();
            BindingContext = MainViewModel.Instance;
            twoPaneView.LayoutChanged += OnTwoPaneViewLayoutChanged;
        }

        void OnTwoPaneViewLayoutChanged(object sender, System.EventArgs e)
        {
            if(layoutWidth != twoPaneView.Width ||
                layoutHeight != twoPaneView.Height)
            {
                layoutWidth = twoPaneView.Width;
                layoutHeight = twoPaneView.Height;
                MainViewModel.Instance.UpdateLayouts();
            }
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            if (layoutWidth != width ||
                layoutHeight != height)
            {
                layoutWidth = width;
                layoutHeight = height;
                MainViewModel.Instance.UpdateLayouts();
            }

            base.LayoutChildren(x, y, width, height);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ((BaseViewModel)BindingContext).OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ((BaseViewModel)BindingContext).OnDisappearing();
        }

        
    }
}