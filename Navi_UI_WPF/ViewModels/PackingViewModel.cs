using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Navi_UI_WPF.ViewModels
{
    /// <summary>
    /// ViewModel chính cho Packing View (Đóng gói sản phẩm)
    /// </summary>
    public class PackingViewModel : ObservableObject
    {
        private int _productId;
        private string _productName;
        private string _description;
        
        private ObservableCollection<AssemblyStepViewModel> _steps;
        private AssemblyStepViewModel _currentStep;
        private int _currentStepIndex;
        
        private bool _isLoading;
        private string _errorMessage;
        private bool _hasError;

        public PackingViewModel()
        {
            Steps = new ObservableCollection<AssemblyStepViewModel>();
            StepsView = System.Windows.Data.CollectionViewSource.GetDefaultView(Steps);
            StepsView.Filter = FilterSteps;
            
            // Khởi tạo Commands theo chuẩn .NET 4.7.2
            NextStepCommand             = new RelayCommand(NextStep, CanGoNext);
            PreviousStepCommand         = new RelayCommand(PreviousStep, CanGoPrevious);
            JumpToStepCommand           = new RelayCommand<AssemblyStepViewModel>(JumpToStep);
            LoadDataCommand             = new RelayCommand<int>(async id => await LoadPackingDataAsync(id));
            ToggleStepCompletionCommand = new RelayCommand(ToggleCurrentStepCompletion);
            
            // Tạm thời load dữ liệu mẫu
            LoadSampleData();
        }

        #region Properties

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    if (int.TryParse(_searchText, out int stepNumber))
                    {
                        int index = stepNumber - 1;
                        if (index >= 0 && index < TotalSteps)
                        {
                            CurrentStepIndex = index;
                        }
                        StepsView.Filter = null; 
                    }
                    else
                    {
                        StepsView.Filter = FilterSteps;
                    }
                    StepsView.Refresh();
                }
            }
        }

        public System.ComponentModel.ICollectionView StepsView { get; private set; }
        
        private string _poNumber;
        public string PoNumber
        {
            get => _poNumber;
            set => SetProperty(ref _poNumber, value);
        }

        public int ProductId
        {
            get => _productId;
            set => SetProperty(ref _productId, value);
        }

        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public ObservableCollection<AssemblyStepViewModel> Steps
        {
            get => _steps;
            set => SetProperty(ref _steps, value);
        }

        public AssemblyStepViewModel CurrentStep
        {
            get => _currentStep;
            set
            {
                if (SetProperty(ref _currentStep, value))
                {
                    if (_currentStep != null)
                    {
                        var index = Steps.IndexOf(_currentStep);
                        if (index != -1 && index != _currentStepIndex)
                        {
                            _currentStepIndex = index;
                            OnPropertyChanged(nameof(CurrentStepIndex));
                        }
                        
                        foreach (var step in Steps) step.IsCurrent = false;
                        _currentStep.IsCurrent = true;
                    }

                    OnPropertyChanged(nameof(CurrentStepNumber));
                    OnPropertyChanged(nameof(HasCurrentStep));
                    OnPropertyChanged(nameof(ProgressPercentage));
                }
            }
        }

        public int CurrentStepIndex
        {
            get => _currentStepIndex;
            set
            {
                if (SetProperty(ref _currentStepIndex, value))
                {
                    UpdateCurrentStep();
                    OnPropertyChanged(nameof(CurrentStepNumber));
                    OnPropertyChanged(nameof(ProgressPercentage));
                    
                    NextStepCommand?.NotifyCanExecuteChanged();
                    PreviousStepCommand?.NotifyCanExecuteChanged();
                }
            }
        }

        public int CurrentStepNumber => CurrentStepIndex + 1;
        
        public int TotalSteps => Steps?.Count ?? 0;

        public double ProgressPercentage
        {
            get
            {
                if (TotalSteps == 0) return 0;
                return (double)CurrentStepNumber / TotalSteps * 100;
            }
        }

        public int CompletedStepsCount => Steps?.Count(s => s.IsCompleted) ?? 0;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (SetProperty(ref _errorMessage, value))
                {
                    HasError = !string.IsNullOrEmpty(value);
                }
            }
        }

        public bool HasCurrentStep => CurrentStep != null;

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        #endregion

        #region Commands

        public IRelayCommand NextStepCommand { get; }
        public IRelayCommand PreviousStepCommand { get; }
        public ICommand JumpToStepCommand { get; }
        public ICommand LoadDataCommand { get; }
        public ICommand ToggleStepCompletionCommand { get; }

        #endregion

        #region Methods

        public async Task LoadPackingDataAsync(int productId)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;
                
                // TODO: Gọi Service lấy API thực sự ở đây
                await Task.Delay(500); 
                
                LoadSampleData();
            }
            catch (Exception ex)
            {
                LoadSampleData();
                ErrorMessage = $"Lỗi kết nối ({ex.Message}). Hiển thị dữ liệu mẫu.";
                HasError = true;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void LoadSampleData()
        {
            ProductId = 2002;
            ProductName = "Sản phẩm đóng gói Mẫu Y2";
            Description = "Quy trình đóng gói thành phẩm trước khi xuất hàng";

            Steps.Clear();
            
            Steps.Add(new AssemblyStepViewModel
            {
                Id = 1,
                Description = "Chuẩn bị hộp và vật liệu đóng gói",
                Note = "Gấp hộp carton đúng chiều, dán đáy hộp bằng băng dính.",
                Images = "https://placehold.co/800x600/png?text=B1+Chuan+Bi+Hop", 
                Type = "Preparation",
                IsCompleted = true,
                IsCurrent = false,
                StepNumber = 1
            });

            Steps.Add(new AssemblyStepViewModel
            {
                Id = 2,
                Description = "Bọc chống sốc cho sản phẩm",
                Note = "Quấn 2 lớp màng xốp nổ (Bubble wrap) quanh sản phẩm.",
                Images = "https://placehold.co/800x600/png?text=B2+Boc+Chong+Soc",
                Type = "Packing",
                IsCompleted = true,
                IsCurrent = false,
                StepNumber = 2
            });

            Steps.Add(new AssemblyStepViewModel
            {
                Id = 3,
                Description = "Đặt sản phẩm và phụ kiện vào hộp",
                Note = "Sản phẩm nằm giữa, hướng tem lên trên. Sách HDSD và phụ kiện đặt bên cạnh.",
                Images = "https://placehold.co/800x600/png?text=B3+Sap+Xep+Vao+Hop", 
                Type = "Packing",
                IsCompleted = false,
                IsCurrent = false,
                StepNumber = 3
            });

            Steps.Add(new AssemblyStepViewModel
            {
                Id = 4,
                Description = "Dán kín hộp và dán tem vận chuyển",
                Note = "Dán băng dính chữ H trên nắp hộp. Dán tem vận chuyển ở góc phải trên cùng.",
                Images = "https://placehold.co/800x600/png?text=B4+Dan+Kien+Hop",
                Type = "Finishing",
                IsCompleted = false,
                IsCurrent = false,
                StepNumber = 4
            });

            CurrentStepIndex = 2; // Step 3
            
            OnPropertyChanged(nameof(TotalSteps));
            OnPropertyChanged(nameof(CompletedStepsCount));
            OnPropertyChanged(nameof(ProgressPercentage));
            
            NextStepCommand?.NotifyCanExecuteChanged();
            PreviousStepCommand?.NotifyCanExecuteChanged();
        }

        private void UpdateCurrentStep()
        {
            foreach (var step in Steps)
            {
                step.IsCurrent = false;
            }
            
            if (CurrentStepIndex >= 0 && CurrentStepIndex < Steps.Count)
            {
                CurrentStep = Steps[CurrentStepIndex];
                CurrentStep.IsCurrent = true;
            }
        }

        private bool CanGoNext() => CurrentStepIndex < TotalSteps - 1;
        private void NextStep() { if (CanGoNext()) CurrentStepIndex++; }

        private bool CanGoPrevious() => CurrentStepIndex > 0;
        private void PreviousStep() { if (CanGoPrevious()) CurrentStepIndex--; }

        private void JumpToStep(AssemblyStepViewModel step)
        {
            if (step != null)
            {
                var index = Steps.IndexOf(step);
                if (index >= 0)
                {
                    CurrentStepIndex = index;
                }
            }
        }

        private bool FilterSteps(object item)
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return true;
            if (item is AssemblyStepViewModel step)
            {
                return (step.Description?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                       (step.Note?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                       (step.StepNumber.ToString().Contains(SearchText));
            }
            return false;
        }

        private void ToggleCurrentStepCompletion()
        {
            if (CurrentStep != null)
            {
                CurrentStep.IsCompleted = !CurrentStep.IsCompleted;
                OnPropertyChanged(nameof(CompletedStepsCount));
                OnPropertyChanged(nameof(ProgressPercentage));
            }
        }

        #endregion
    }
}
