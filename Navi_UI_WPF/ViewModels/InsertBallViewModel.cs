using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Navi_UI_WPF.ViewModels
{
    public class InsertBallViewModel : ObservableObject
    {
        private string _orderNumber = "ORDER-2026-001";
        private string _productName = "SẢN PHẨM MẪU CHÈN BI";
        private int _count = 0;
        private string _sizeBallLabel = "SIZE BALL 26";
        private string _status = "Free";
        private bool _isRunning = false;
        private TableOption _selectedTable;
        private SizeBallOption _selectedSizeBall;
        private ObservableCollection<SizeBallOption> _sizeBallValues;
        private ObservableCollection<TableOption> _tableValues;

        public InsertBallViewModel()
        {
            InitializeSizeBallValues();
            InitializeTableValues();

            SelectSizeBallCommand = new RelayCommand<SizeBallOption>(SelectSizeBall);
            SelectTableCommand = new RelayCommand<TableOption>(SelectTable);
            StartCommand = new RelayCommand(Start, () => !IsRunning);
            StopCommand = new RelayCommand(Stop, () => IsRunning);
        }

        #region Properties

        public string OrderNumber
        {
            get => _orderNumber;
            set => SetProperty(ref _orderNumber, value);
        }

        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        public string SizeBallLabel
        {
            get => _sizeBallLabel;
            set => SetProperty(ref _sizeBallLabel, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public Brush StatusColor => Status == "Busy" ? Brushes.Red : (SolidColorBrush)(new BrushConverter().ConvertFrom("#2E7D32"));

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (SetProperty(ref _isRunning, value))
                {
                    Status = _isRunning ? "Busy" : "Free";
                    OnPropertyChanged(nameof(StatusColor));
                    ((RelayCommand)StartCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)StopCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public TableOption SelectedTable
        {
            get => _selectedTable;
            set => SetProperty(ref _selectedTable, value);
        }

        public ObservableCollection<SizeBallOption> SizeBallValues
        {
            get => _sizeBallValues;
            set => SetProperty(ref _sizeBallValues, value);
        }

        public ObservableCollection<TableOption> TableValues
        {
            get => _tableValues;
            set => SetProperty(ref _tableValues, value);
        }

        public SizeBallOption SelectedSizeBall
        {
            get => _selectedSizeBall;
            set => SetProperty(ref _selectedSizeBall, value);
        }

        #endregion

        #region Commands

        public ICommand SelectSizeBallCommand { get; }
        public ICommand SelectTableCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        #endregion

        #region Methods

        private void InitializeSizeBallValues()
        {
            SizeBallValues = new ObservableCollection<SizeBallOption>();

            for (double v = -5.0; v <= 5.0; v += 0.5)
            {
                SizeBallValues.Add(new SizeBallOption
                {
                    Value = v,
                    DisplayText = v > 0 ? $"+{v}" : v.ToString()
                });
            }

            var defaultOption = SizeBallValues.FirstOrDefault(x => Math.Abs(x.Value - 0) < 0.001);
            if (defaultOption != null)
            {
                SelectSizeBall(defaultOption);
            }
        }

        private void InitializeTableValues()
        {
            TableValues = new ObservableCollection<TableOption>();
            for (int i = 1; i <= 5; i++)
            {
                TableValues.Add(new TableOption { Number = i });
            }
            
            var firstTable = TableValues.FirstOrDefault();
            if (firstTable != null) SelectTable(firstTable);
        }

        private void SelectSizeBall(SizeBallOption option)
        {
            if (option == null) return;

            foreach (var item in SizeBallValues)
            {
                item.IsSelected = false;
            }

            option.IsSelected = true;
            SelectedSizeBall = option;
        }

        private void SelectTable(TableOption option)
        {
            if (option == null) return;

            foreach (var item in TableValues)
            {
                item.IsSelected = false;
            }

            option.IsSelected = true;
            SelectedTable = option;
        }

        private void Start()
        {
            IsRunning = true;
            Count++;
        }

        private void Stop()
        {
            IsRunning = false;
        }

        #endregion
    }
}
