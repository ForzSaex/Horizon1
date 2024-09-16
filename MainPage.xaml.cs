using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Horizon
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
#if ANDROID
            InitializeComponent();
            MainPageViewModel.MainPageViewModel mainPageViewModel = new MainPageViewModel.MainPageViewModel();
            BindingContext = mainPageViewModel;
            CollectionView.ItemsSource = mainPageViewModel.userApps;
#endif
        }
    }
}
