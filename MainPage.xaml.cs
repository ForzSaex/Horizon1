using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;

namespace Horizon
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            MainPageViewModel.MainPageViewModel mainPageViewModel = new MainPageViewModel.MainPageViewModel();
            BindingContext = mainPageViewModel;
            CollectionView.ItemsSource = mainPageViewModel.Apps;
        }
    }
}
