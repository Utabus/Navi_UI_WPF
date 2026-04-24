using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Navi.Application.DTOs;
using Navi.Application.Interfaces;
using Navi_UI_WPF.Services;
using Navi_UI_WPF.Views;
using Serilog;
using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Navi_UI_WPF.ViewModels
{
    /// <summary>
    /// Base ViewModel for Assembly-like views
    /// </summary>
    public abstract class BaseAssemblyViewModel : ObservableObject
    {
        protected readonly INaviHistoryService _historyService;
        protected readonly IDeviceService _deviceService;
        protected readonly INaviItemService _naviItemService;
        protected readonly IServiceProvider _serviceProvider;

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

        public Action RequestFocusAction { get; set; }
        protected bool _isPoAlreadyFinished;

        protected BaseAssemblyViewModel(
            INaviHistoryService historyService,
            IDeviceService deviceService,
            INaviItemService naviItemService,
            IServiceProvider serviceProvider)
        {
            _historyService = historyService;
            _deviceService = deviceService;
            _naviItemService = naviItemService;
            _serviceProvider = serviceProvider;

            Steps = new ObservableCollection<AssemblyStepViewModel>();
            StepsView = System.Windows.Data.CollectionViewSource.GetDefaultView(Steps);
            StepsView.Filter = FilterSteps;

            JumpToStepCommand = new RelayCommand<AssemblyStepViewModel>(JumpToStep);
            LoadDataCommand = new RelayCommand<string>(async name => await LoadProductAssemblyAsync(name));
            ToggleStepCompletionCommand = new RelayCommand(ToggleCurrentStepCompletion);
            ConfirmOkCommand = new RelayCommand(ConfirmOk, CanConfirm);
            ConfirmNgCommand = new RelayCommand(ConfirmNg, CanConfirm);
            NextStepCommand = new RelayCommand(NextStep, CanGoNext);
            PreviousStepCommand = new RelayCommand(PreviousStep, CanGoPrevious);
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

        public ICollectionView StepsView { get; private set; }

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
                if (_currentStep != null && !_currentStep.IsCompleted && _currentStep.RequiresConfirmation && value != null)
                {
                    var oldIndex = Steps.IndexOf(_currentStep);
                    var newIndex = Steps.IndexOf(value);

                    if (newIndex > oldIndex)
                    {
                        OnPropertyChanged(nameof(CurrentStep));
                        return;
                    }
                }

                var oldStep = _currentStep;
                if (SetProperty(ref _currentStep, value))
                {
                    if (oldStep != null)
                    {
                        oldStep.PropertyChanged -= OnStepPropertyChanged;
                    }

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

                        _currentStep.PropertyChanged += OnStepPropertyChanged;
                    }

                    OnPropertyChanged(nameof(CurrentStepNumber));
                    OnPropertyChanged(nameof(HasCurrentStep));
                    OnPropertyChanged(nameof(ProgressPercentage));

                    UpdateStepsLockStatus();
                    RefreshCommandStates();
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
                    UpdateStepsLockStatus();
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

        public ICommand JumpToStepCommand { get; }
        public ICommand LoadDataCommand { get; }
        public ICommand ToggleStepCompletionCommand { get; }
        public IRelayCommand ConfirmOkCommand { get; }
        public IRelayCommand ConfirmNgCommand { get; }
        public IRelayCommand NextStepCommand { get; }
        public IRelayCommand PreviousStepCommand { get; }

        #endregion

        #region Methods

        public abstract Task LoadProductAssemblyAsync(string productName, string po = null);

        protected virtual void OnStepPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AssemblyStepViewModel.IsCompleted) ||
                e.PropertyName == nameof(AssemblyStepViewModel.IsNg))
            {
                RefreshCommandStates();
                OnPropertyChanged(nameof(CompletedStepsCount));
                OnPropertyChanged(nameof(ProgressPercentage));
            }
        }

        protected void RefreshCommandStates()
        {
            ConfirmOkCommand?.NotifyCanExecuteChanged();
            ConfirmNgCommand?.NotifyCanExecuteChanged();
            NextStepCommand?.NotifyCanExecuteChanged();
            PreviousStepCommand?.NotifyCanExecuteChanged();
        }

        protected void UpdateCurrentStep()
        {
            if (CurrentStepIndex >= 0 && CurrentStepIndex < Steps.Count)
            {
                var stepToSelect = Steps[CurrentStepIndex];
                if (CurrentStep != stepToSelect)
                {
                    CurrentStep = stepToSelect;
                }
            }
            RefreshCommandStates();
        }

        protected bool CanGoNext() => CurrentStepIndex < TotalSteps - 1;
        protected void NextStep() { if (CanGoNext()) CurrentStepIndex++; }

        protected bool CanGoPrevious() => CurrentStepIndex > 0;
        protected void PreviousStep() { if (CanGoPrevious()) CurrentStepIndex--; }

        protected void UpdateStepsLockStatus()
        {
            if (Steps == null) return;
            var firstIncompleteIndex = Steps.ToList().FindIndex(s => string.IsNullOrEmpty(s.PO));
            for (int i = 0; i < Steps.Count; i++)
            {
                bool hasHistory = !string.IsNullOrEmpty(Steps[i].PO);
                bool isNextAvailable = (i == firstIncompleteIndex);
                Steps[i].IsLocked = !hasHistory && !isNextAvailable;
            }
        }

        protected void JumpToStep(AssemblyStepViewModel step)
        {
            if (step != null && !step.IsLocked)
            {
                var index = Steps.IndexOf(step);
                if (index >= 0)
                {
                    CurrentStepIndex = index;
                }
            }
        }

        protected bool FilterSteps(object item)
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

        protected void ToggleCurrentStepCompletion()
        {
            if (CurrentStep != null)
            {
                CurrentStep.IsCompleted = !CurrentStep.IsCompleted;
                if (CurrentStep.IsCompleted) CurrentStep.IsNg = false;
                OnPropertyChanged(nameof(CompletedStepsCount));
                OnPropertyChanged(nameof(ProgressPercentage));
            }
        }

        protected bool CanConfirm() => CurrentStep != null;

        protected virtual void ConfirmOk()
        {
            if (CurrentStep != null)
            {
                CurrentStep.IsCompleted = true;
                CurrentStep.IsNg = false;
                CurrentStep.PO = PoNumber;

                OnPropertyChanged(nameof(CompletedStepsCount));
                OnPropertyChanged(nameof(ProgressPercentage));

                _ = RecordStepHistoryAsync(isOk: true);

                RefreshCommandStates();

                if (CurrentStepIndex < TotalSteps - 1)
                {
                    CurrentStepIndex++;
                }
                else
                {
                    CompleteAssembly();
                }
            }
        }

        protected virtual void CompleteAssembly()
        {
            Growl.Success(new GrowlInfo
            {
                Message = "Sản phẩm đã được lắp ráp hoàn tất!",
                WaitTime = 3,
                Token = "MainGrowl"
            });

            ClearAssemblyData();
            RequestFocusAction?.Invoke();
        }

        protected virtual void ClearAssemblyData()
        {
            PoNumber = string.Empty;
            ProductName = string.Empty;
            _isPoAlreadyFinished = false;

            if (Steps != null)
            {
                Steps.Clear();
            }

            CurrentStep = null;
            CurrentStepIndex = -1;

            OnPropertyChanged(nameof(TotalSteps));
            OnPropertyChanged(nameof(CompletedStepsCount));
            OnPropertyChanged(nameof(ProgressPercentage));
            OnPropertyChanged(nameof(HasCurrentStep));
            OnPropertyChanged(nameof(CurrentStepNumber));
        }

        protected virtual void ConfirmNg()
        {
            if (CurrentStep != null)
            {
                var dialog = _serviceProvider.GetRequiredService<NgReasonDialog>();
                dialog.Owner = Application.Current.MainWindow;

                if (dialog.ShowDialog() == true)
                {
                    var vm = dialog.DataContext as NgReasonViewModel;
                    var selectedReason = vm?.FinalReason ?? "Lỗi không xác định";

                    CurrentStep.IsNg = true;
                    CurrentStep.IsCompleted = false;
                    CurrentStep.OK = false;
                    CurrentStep.NG = true;
                    CurrentStep.PO = PoNumber;
                    CurrentStep.HistoryNote = selectedReason;

                    OnPropertyChanged(nameof(CompletedStepsCount));
                    OnPropertyChanged(nameof(ProgressPercentage));

                    _ = RecordStepHistoryAsync(isNg: true, note: selectedReason);

                    if (CurrentStepIndex < TotalSteps - 1)
                    {
                        CurrentStepIndex++;
                    }
                }
            }
        }

        protected abstract Task RecordStepHistoryAsync(bool isOk = false, bool isNg = false, string note = null);

        protected virtual string GetHistoryType(AssemblyStepViewModel step)
        {
            return step.Type;
        }

        protected void ProcessStepDescription(AssemblyStepViewModel step)
        {
            if (string.IsNullOrEmpty(step.Description)) return;
            string delimiter = "※Chú ý:";
            if (step.Description.Contains(delimiter))
            {
                var parts = step.Description.Split(new[] { delimiter }, StringSplitOptions.None);
                step.Description = parts[0].Trim();
                if (parts.Length > 1)
                {
                    string extractedNote = parts[1].Trim();
                    if (string.IsNullOrEmpty(extractedNote)) return;
                    if (string.IsNullOrEmpty(step.Note))
                    {
                        step.Note = extractedNote;
                    }
                    else if (!step.Note.Contains(extractedNote))
                    {
                        step.Note = $"{step.Note}\n{extractedNote}";
                    }
                }
            }
        }

        #endregion
    }
}
