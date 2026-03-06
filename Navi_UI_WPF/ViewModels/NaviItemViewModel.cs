using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Navi_UI_WPF.Commands;
using Navi.Application.DTOs;
using Serilog;
using Navi_UI_WPF.Helpers;

namespace Navi_UI_WPF.ViewModels
{
    /// <summary>
    /// ViewModel quản lý Bước lắp ráp (NaviItem)
    /// </summary>
    public class NaviItemViewModel : ViewModelBase
    {
        public NaviItemViewModel()
        {
            Items = new ObservableCollection<NaviItemDto>();
            AvailableTypes = new ObservableCollection<string> { "Tất cả", "Assembly", "Validation", "Check", "Manual" };
            SelectedType = "Tất cả";
            LoadSampleData();

            SearchCommand = new RelayCommand(o => ExecuteSearch());
            FilterByTypeCommand = new RelayCommand(o => ExecuteFilterByType());
            AddCommand = new RelayCommand(o => OpenAddDialog());
            EditCommand = new RelayCommand(o => OpenEditDialog(o as NaviItemDto), o => SelectedItem != null);
            DeleteCommand = new RelayCommand(o => ExecuteDelete(o as NaviItemDto), o => SelectedItem != null);
            SelectItemCommand = new RelayCommand(o => SelectItem(o as NaviItemDto));
            ClearSelectionCommand = new RelayCommand(o => { SelectedItem = null; IsDetailVisible = false; });
        }

        // ── Collections & Selection ──────────────────────────────────────

        private ObservableCollection<NaviItemDto> _items;
        public ObservableCollection<NaviItemDto> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private NaviItemDto _selectedItem;
        public NaviItemDto SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        private ObservableCollection<string> _availableTypes;
        public ObservableCollection<string> AvailableTypes
        {
            get => _availableTypes;
            set => SetProperty(ref _availableTypes, value);
        }

        // ── UI State ─────────────────────────────────────────────────────

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set => SetProperty(ref _searchTerm, value);
        }

        private string _selectedType;
        public string SelectedType
        {
            get => _selectedType;
            set => SetProperty(ref _selectedType, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _isDetailVisible;
        public bool IsDetailVisible
        {
            get => _isDetailVisible;
            set => SetProperty(ref _isDetailVisible, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // ── Form Fields ──────────────────────────────────────────────────

        private string _editDescription;
        public string EditDescription
        {
            get => _editDescription;
            set => SetProperty(ref _editDescription, value);
        }

        private string _editNote;
        public string EditNote
        {
            get => _editNote;
            set => SetProperty(ref _editNote, value);
        }

        private string _editBolts;
        public string EditBolts
        {
            get => _editBolts;
            set => SetProperty(ref _editBolts, value);
        }

        private string _editForce;
        public string EditForce
        {
            get => _editForce;
            set => SetProperty(ref _editForce, value);
        }

        private string _editType;
        public string EditType
        {
            get => _editType;
            set => SetProperty(ref _editType, value);
        }

        private int? _editStep;
        public int? EditStep
        {
            get => _editStep;
            set => SetProperty(ref _editStep, value);
        }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        // ── Commands ─────────────────────────────────────────────────────

        public ICommand SearchCommand { get; }
        public ICommand FilterByTypeCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SelectItemCommand { get; }
        public ICommand ClearSelectionCommand { get; }

        // ── Handlers ─────────────────────────────────────────────────────

        private void ExecuteSearch()
        {
            Log.Information("Người dùng thực hiện tìm kiếm với từ khóa: {SearchTerm}", SearchTerm);
            StatusMessage = $"Đang tìm kiếm: \"{SearchTerm}\"...";
            // TODO: gọi INaviItemService.SearchAsync(SearchTerm)
        }

        private void ExecuteFilterByType()
        {
            StatusMessage = $"Lọc theo loại: {SelectedType}";
            // TODO: gọi INaviItemService.GetByTypeAsync(SelectedType)
        }

        private void OpenAddDialog()
        {
            IsEditMode = false;
            EditDescription = ""; EditNote = ""; EditBolts = ""; EditForce = ""; EditType = ""; EditStep = null;
            IsDetailVisible = true;
            StatusMessage = "Thêm bước lắp ráp mới";
        }

        private void OpenEditDialog(NaviItemDto item)
        {
            if (item == null) return;
            IsEditMode = true;
            EditDescription = item.Description;
            EditNote = item.Note;
            EditBolts = item.Bolts;
            EditForce = item.Force;
            EditType = item.Type;
            EditStep = item.Step;
            IsDetailVisible = true;
            StatusMessage = $"Chỉnh sửa: Step {item.Step} - {item.Description}";
        }

        private void ExecuteDelete(NaviItemDto item)
        {
            if (item == null) return;

            using (LoggerHelper.WithSerialNumber(item.Id.ToString()))
            {
                try
                {
                    Log.Information("Bắt đầu xóa bước lắp ráp: {Description}", item.Description);
                    Items.Remove(item);
                    SelectedItem = null;
                    IsDetailVisible = false;
                    StatusMessage = $"Đã xóa bước: {item.Description}";
                    
                    Log.Information("Đã xóa thành công bước lắp ráp ID: {Id}", item.Id);
                    // TODO: gọi INaviItemService.DeleteAsync(item.Id)
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Lỗi xảy ra khi xóa bước lắp ráp ID: {Id}", item.Id);
                    StatusMessage = "Có lỗi xảy ra khi xóa.";
                }
            }
        }

        private void SelectItem(NaviItemDto item)
        {
            if (item == null) return;
            SelectedItem = item;
            IsDetailVisible = true;
            EditDescription = item.Description;
            EditNote = item.Note;
            EditBolts = item.Bolts;
            EditForce = item.Force;
            EditType = item.Type;
            EditStep = item.Step;
            StatusMessage = $"Đang xem: Step {item.Step} - {item.Description}";
        }

        // ── Sample Data ──────────────────────────────────────────────────

        private void LoadSampleData()
        {
            Items.Add(new NaviItemDto { Id = 1, Step = 1, Description = "Siết bu lông M10", Note = "Dùng cờ lê 17mm", Bolts = "M10x1.5", Force = "50Nm", Type = "Assembly", Cdt = DateTime.Now.AddDays(-5) });
            Items.Add(new NaviItemDto { Id = 2, Step = 2, Description = "Kiểm tra lực siết", Note = "Dùng cờ lê lực", Bolts = "M10x1.5", Force = "50Nm", Type = "Validation", Cdt = DateTime.Now.AddDays(-5) });
            Items.Add(new NaviItemDto { Id = 3, Step = 3, Description = "Lắp vòng đệm cao su", Note = "Không siết quá lực", Bolts = "M8x1.0", Force = "25Nm", Type = "Assembly", Cdt = DateTime.Now.AddDays(-3) });
            Items.Add(new NaviItemDto { Id = 4, Step = 4, Description = "Kiểm tra độ kín khí", Note = "Áp suất test: 2 bar", Type = "Check", Cdt = DateTime.Now.AddDays(-2) });
            Items.Add(new NaviItemDto { Id = 5, Step = 5, Description = "Lắp ốp ngoài", Note = "Canh chỉnh đối xứng", Type = "Manual", Cdt = DateTime.Now.AddDays(-1) });
            StatusMessage = $"Tải thành công {Items.Count} bước lắp ráp";
        }
    }
}
