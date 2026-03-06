using System.Windows.Input;
using Navi_UI_WPF.Commands;
using System.Windows;
using Navi_UI_WPF.Views;

namespace Navi_UI_WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            // Default view
            CurrentView = new HomeViewModel();
            IsMenuOpen = true; // Default open
            
            // Commands
            NavigateToHomeCommand = new RelayCommand(o => CurrentView = new HomeViewModel());
            NavigateToAssemblyCommand = new RelayCommand(o => CurrentView = new ProductAssemblyViewModel());
            NavigateToProductCommand = new RelayCommand(o => CurrentView = new NaviProductViewModel());
            NavigateToItemCommand = new RelayCommand(o => CurrentView = new NaviItemViewModel());
            NavigateToProductItemCommand = new RelayCommand(o => CurrentView = new NaviProductItemViewModel());
            NavigateToHistoryCommand = new RelayCommand(o => CurrentView = new TestUC());
            ExitCommand = new RelayCommand(o => Application.Current.Shutdown());
            ToggleMenuCommand = new RelayCommand(o => IsMenuOpen = !IsMenuOpen);
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
        public ICommand ExitCommand { get; }
    }
}
