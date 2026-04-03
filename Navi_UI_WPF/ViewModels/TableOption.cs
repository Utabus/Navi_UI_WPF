using CommunityToolkit.Mvvm.ComponentModel;

namespace Navi_UI_WPF.ViewModels
{
    public class TableOption : ObservableObject
    {
        private int _number;
        public int Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
