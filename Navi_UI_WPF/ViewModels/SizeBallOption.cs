using CommunityToolkit.Mvvm.ComponentModel;

namespace Navi_UI_WPF.ViewModels
{
    /// <summary>
    /// Model đại diện cho một tùy chọn size ball trong grid 3x7
    /// </summary>
    public class SizeBallOption : ObservableObject
    {
        private double _value;
        public double Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        private string _displayText;
        public string DisplayText
        {
            get => _displayText;
            set => SetProperty(ref _displayText, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
