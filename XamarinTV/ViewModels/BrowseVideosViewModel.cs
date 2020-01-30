using System.Windows.Input;
using XamarinTV.ViewModels.Base;
using Xamarin.Forms;

namespace XamarinTV.ViewModels
{
    public class BrowseVideosViewModel : BaseViewModel
    {
        int _selectedViewModelIndex;

        public BrowseVideosViewModel()
        {
        }

        public int SelectedViewModelIndex
        {
            get { return _selectedViewModelIndex; }
            set { SetProperty(ref _selectedViewModelIndex, value); }
        }

        public ICommand SettingsCommand => new Command(OpenSettings);

        void OpenSettings()
        {
            MainViewModel.Instance.OpenSettingWindow();
        }
    }
}