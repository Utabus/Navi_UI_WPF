using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Navi.Application.Services;
using Navi.Infrastructure.Repositories;

namespace Navi_UI_WPF.ViewModels
{
    /// <summary>
    /// ViewModel chính cho Product Assembly View
    /// Main ViewModel for Product Assembly View
    /// </summary>
    public class ProductAssemblyViewModel : ObservableObject
    {
        private readonly ProductAssemblyService _service;
        
        private int _productId;
        private string _productName;
        private string _description;
        private DateTime _createdDate;
        private DateTime _updatedDate;
        
        private ObservableCollection<AssemblyStepViewModel> _steps;
        private AssemblyStepViewModel _currentStep;
        private int _currentStepIndex;
        
        private bool _isLoading;
        private string _errorMessage;
        private bool _hasError;

        public ProductAssemblyViewModel()
        {
            // Initialize service (in production, use DI)
            var repository = new ProductAssemblyRepository();
            _service = new ProductAssemblyService(repository);
            
            Steps = new ObservableCollection<AssemblyStepViewModel>();
            StepsView = System.Windows.Data.CollectionViewSource.GetDefaultView(Steps);
            StepsView.Filter = FilterSteps;
            
            // Initialize commands
            NextStepCommand             = new RelayCommand(NextStep, CanGoNext);
            PreviousStepCommand         = new RelayCommand(PreviousStep, CanGoPrevious);
            JumpToStepCommand           = new RelayCommand<AssemblyStepViewModel>(JumpToStep);
            LoadDataCommand             = new RelayCommand<int>(async id => await LoadProductAssemblyAsync(id));
            ToggleStepCompletionCommand = new RelayCommand(ToggleCurrentStepCompletion);
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
                     // If input is a number, jump to that step and DO NOT filter
                     if (int.TryParse(_searchText, out int stepNumber))
                     {
                         // Jump to step (1-based index to 0-based)
                         int index = stepNumber - 1;
                         if (index >= 0 && index < TotalSteps)
                         {
                             CurrentStepIndex = index;
                         }
                         // Disable filter so user can see context
                         StepsView.Filter = null; 
                     }
                     else
                     {
                         // If input is text, apply filter
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

        public DateTime CreatedDate
        {
            get => _createdDate;
            set => SetProperty(ref _createdDate, value);
        }

        public DateTime UpdatedDate
        {
            get => _updatedDate;
            set => SetProperty(ref _updatedDate, value);
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
                    // Sync Index if needed
                    if (_currentStep != null)
                    {
                        var index = Steps.IndexOf(_currentStep);
                        if (index != -1 && index != _currentStepIndex)
                        {
                            _currentStepIndex = index;
                            OnPropertyChanged(nameof(CurrentStepIndex));
                        }
                        
                        // Update visual state (IsCurrent)
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

        public ICommand NextStepCommand { get; }
        public ICommand PreviousStepCommand { get; }
        public ICommand JumpToStepCommand { get; }
        public ICommand LoadDataCommand { get; }
        public ICommand ToggleStepCompletionCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Load product assembly data from API
        /// </summary>
        public async Task LoadProductAssemblyAsync(int productId)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;
                
                var dto = await _service.GetProductAssemblyByIdAsync(productId);
                
                ProductId = dto.Id;
                ProductName = dto.ProductName;
                Description = dto.Description;
                CreatedDate = dto.Cdt;
                UpdatedDate = dto.Udt;
                
                // Map items to ViewModels
                Steps.Clear();
                foreach (var item in dto.Items)
                {
                    Steps.Add(new AssemblyStepViewModel
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Note = item.Note,
                        Bolts = item.Bolts,
                        Force = item.Force,
                        Images = item.Images,
                        Type = item.Type,
                        IsCompleted = false,
                        IsCurrent = false,
                        StepNumber = Steps.Count + 1 // Ensure StepNumber is populated
                    });
                }
                
                // Set first step as current
                if (Steps.Count > 0)
                {
                    CurrentStepIndex = 0;
                }
                
                OnPropertyChanged(nameof(TotalSteps));
            }
            catch (Exception ex)
            {
                // FALLBACK: Load Sample Data if API fails (for testing/demo)
                LoadSampleData();
                ErrorMessage = $"Không thể kết nối API ({ex.Message}). Đang hiển thị dữ liệu mẫu.";
                HasError = false; 
                MessageBox.Show($"Không thể kết nối API: {ex.Message}.\nĐang hiển thị dữ liệu mẫu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void LoadSampleData()
        {
            ProductId = 999;
            ProductName = "LX15(all) - Cụm Lắp Ráp Mẫu";
            Description = "Hướng dẫn lắp ráp bộ phận đường sắt mẫu (Sample Data)";
            CreatedDate = DateTime.Now;
            UpdatedDate = DateTime.Now;

            Steps.Clear();
            
            // Using placeholder images for demonstration
            Steps.Add(new AssemblyStepViewModel
            {
                Id = 1,
                Description = "Chuẩn bị Jig và các linh kiện cần thiết. Kiểm tra sạch sẽ bề mặt làm việc.",
                Note = "Đảm bảo Jig không bị dính dầu mỡ.",
                Images = "https://placehold.co/800x600/png?text=B1+Chuan+Bi", 
                Type = "Preparation",
                IsCompleted = true,
                IsCurrent = false,
                StepNumber = 1
            });

            Steps.Add(new AssemblyStepViewModel
            {
                Id = 2,
                Description = "Gá Rail lên Jig. Chú ý hướng lắp ráp theo chiều mũi tên trên Jig.",
                Images = "https://placehold.co/800x600/png?text=B2+Ga+Rail",
                Type = "Assembly",
                IsCompleted = true,
                IsCurrent = false,
                StepNumber = 2
            });

            Steps.Add(new AssemblyStepViewModel
            {
                Id = 3,
                Description = "Gắn Retuncap vào hai đầu Rail. Đảm bảo khớp nối khít, không bị hở.",
                Note = "Kiểm tra kỹ lưỡng khớp nối.",
                Images = "https://placehold.co/800x600/png?text=B3+Gan+Retuncap", 
                Type = "Assembly",
                IsCompleted = false,
                IsCurrent = false,
                StepNumber = 3
            });

            Steps.Add(new AssemblyStepViewModel
            {
                Id = 4,
                Description = "Siết ốc cố định Retuncap. Sử dụng ốc M1.7x6. Siết lực vừa đủ.",
                Bolts = "M1.7x6 (4 cái)",
                Force = "100 cN.m",
                Images = "https://placehold.co/800x600/png?text=B4+Siet+Oc",
                Type = "Fastening",
                IsCompleted = false,
                IsCurrent = false,
                StepNumber = 4
            });

            Steps.Add(new AssemblyStepViewModel
            {
                Id = 5,
                Description = "Kiểm tra hoạt động trượt của Block trên Rail. Phải trượt êm, không bị kẹt.",
                Note = "Nếu bị kẹt, tháo ra kiểm tra lại bước 3.",
                Images = "https://placehold.co/800x600/png?text=B5+Kiem+Tra",
                Type = "Inspection",
                IsCompleted = false,
                IsCurrent = false,
                StepNumber = 5
            });
            
            Steps.Add(new AssemblyStepViewModel
            {
                Id = 6,
                Description = "Dán tem bảo hành và mã sản phẩm lên mặt dưới của Block.",
                Images = "https://placehold.co/800x600/png?text=B6+Dan+Tem",
                Type = "Finishing",
                IsCompleted = false,
                IsCurrent = false,
                StepNumber = 6
            });

            // Set current step to the first incomplete step
            CurrentStepIndex = 2; // Step 3
            
            // Notify UI of changes
            OnPropertyChanged(nameof(TotalSteps));
            OnPropertyChanged(nameof(CompletedStepsCount));
            OnPropertyChanged(nameof(ProgressPercentage));
        }

        private void UpdateCurrentStep()
        {
            // Reset all steps
            foreach (var step in Steps)
            {
                step.IsCurrent = false;
            }
            
            // Set current step
            if (CurrentStepIndex >= 0 && CurrentStepIndex < Steps.Count)
            {
                CurrentStep = Steps[CurrentStepIndex];
                CurrentStep.IsCurrent = true;
            }
        }

        private bool CanGoNext()
        {
            return CurrentStepIndex < TotalSteps - 1;
        }

        private void NextStep()
        {
            if (CanGoNext())
            {
                CurrentStepIndex++;
            }
        }

        private bool CanGoPrevious()
        {
            return CurrentStepIndex > 0;
        }

        private void PreviousStep()
        {
            if (CanGoPrevious())
            {
                CurrentStepIndex--;
            }
        }

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
