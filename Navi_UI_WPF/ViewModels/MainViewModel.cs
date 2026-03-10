using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using Navi_UI_WPF.Views;

namespace Navi_UI_WPF.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            // Default view
            CurrentView = new HomeViewModel();
            IsMenuOpen = true; // Default open
            
            // Commands
            NavigateToHomeCommand        = new RelayCommand(() => CurrentView = new HomeViewModel());
            NavigateToAssemblyCommand    = new RelayCommand(() => CurrentView = new ProductAssemblyViewModel());
            NavigateToProductCommand     = new RelayCommand(() => CurrentView = new NaviProductViewModel());
            NavigateToItemCommand        = new RelayCommand(() => CurrentView = new NaviItemViewModel());
            NavigateToProductItemCommand = new RelayCommand(() => CurrentView = new NaviProductItemViewModel());
            NavigateToHistoryCommand     = new RelayCommand(() => CurrentView = new NaviHistoryViewModel());
            NavigateToForceGaugeCommand  = new RelayCommand(() => CurrentView = new ForceGaugeViewModel());
            ExitCommand                  = new RelayCommand(() => Application.Current.Shutdown());
            ToggleMenuCommand            = new RelayCommand(() => IsMenuOpen = !IsMenuOpen);
        }

        private bool _isMenuOpen;
        public bool IsMenuOpen
        {
            get => _isMenuOpen;
            set => SetProperty(ref _isMenuOpen, value);
        }

        public ICommand ToggleMenuCommand { get; }

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public ICommand NavigateToHomeCommand { get; }
        public ICommand NavigateToAssemblyCommand { get; }
        public ICommand NavigateToProductCommand { get; }
        public ICommand NavigateToItemCommand { get; }
        public ICommand NavigateToProductItemCommand { get; }
        public ICommand NavigateToHistoryCommand { get; }
        public ICommand NavigateToForceGaugeCommand { get; }
        public ICommand ExitCommand { get; }
    }
}
