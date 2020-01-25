using XamarinTV.ViewModels;
using XamarinTV.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinTV.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = MainViewModel.Instance;
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