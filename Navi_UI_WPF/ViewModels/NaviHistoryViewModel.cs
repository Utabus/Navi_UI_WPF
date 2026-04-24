using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Navi.Application.DTOs;
using Navi.Application.Interfaces;
using Navi.Application.Services;
using Navi.Core.Interfaces;
using Navi.Infrastructure.Repositories;

namespace Navi_UI_WPF.ViewModels
{
    /// <summary>
    /// ViewModel quản lý lịch sử thao tác nhân viên (NaviHistory)
    /// </summary>
    public class NaviHistoryViewModel : ObservableObject
    {
        private readonly INaviHistoryService _service;

        public NaviHistoryViewModel(INaviHistoryService service)
        {
            _service = service;

            HistoryRecords = new ObservableCollection<NaviHistoryDto>();
            AvailableTypes = new ObservableCollection<string> { "SCAN", "CHECK", "CONFIRM", "REWORK", "REJECT" };
            
            // Initial load
            LoadAll();

            LoadAllCommand         = new AsyncRelayCommand(async () => await LoadAllAsync());
            FilterByCodeNVCommand  = new AsyncRelayCommand(async () => await ExecuteFilterByCodeNVAsync());
            FilterByPOCommand      = new AsyncRelayCommand(async () => await ExecuteFilterByPOAsync());
            FilterByProductItemCommand = new AsyncRelayCommand(async () => await ExecuteFilterByProductItemAsync());
            ClearFilterCommand     = new RelayCommand(() => ClearFilters());
            AddCommand             = new RelayCommand(() => OpenAddDialog());
            EditCommand            = new RelayCommand<NaviHistoryDto>(item => OpenEditDialog(item), _ => SelectedRecord != null);
            DeleteCommand          = new AsyncRelayCommand<NaviHistoryDto>(async item => await ExecuteDeleteAsync(item), _ => SelectedRecord != null);
            SelectRecordCommand    = new RelayCommand<NaviHistoryDto>(item => SelectRecord(item));
            SaveCommand            = new AsyncRelayCommand(async () => await ExecuteSaveAsync());
            CancelEditCommand      = new RelayCommand(() => { IsDetailVisible = false; SelectedRecord = null; });
        }

        // ── Collections & Selection ──────────────────────────────────────

        private ObservableCollection<NaviHistoryDto> _historyRecords;
        public ObservableCollection<NaviHistoryDto> HistoryRecords
        {
            get => _historyRecords;
            set => SetProperty(ref _historyRecords, value);
        }

        private NaviHistoryDto _selectedRecord;
        public NaviHistoryDto SelectedRecord
        {
            get => _selectedRecord;
            set => SetProperty(ref _selectedRecord, value);
        }

        private ObservableCollection<string> _availableTypes;
        public ObservableCollection<string> AvailableTypes
        {
            get => _availableTypes;
            set => SetProperty(ref _availableTypes, value);
        }

        // ── Filter Fields ────────────────────────────────────────────────

        private string _filterCodeNV;
        public string FilterCodeNV
        {
            get => _filterCodeNV;
            set => SetProperty(ref _filterCodeNV, value);
        }

        private string _filterPO;
        public string FilterPO
        {
            get => _filterPO;
            set => SetProperty(ref _filterPO, value);
        }

        private string _filterProductItemId;
        public string FilterProductItemId
        {
            get => _filterProductItemId;
            set => SetProperty(ref _filterProductItemId, value);
        }

        // ── Summary Cards ────────────────────────────────────────────────

        private int _totalCount;
        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        private int _uniqueEmployeeCount;
        public int UniqueEmployeeCount
        {
            get => _uniqueEmployeeCount;
            set => SetProperty(ref _uniqueEmployeeCount, value);
        }

        private int _activePOCount;
        public int ActivePOCount
        {
            get => _activePOCount;
            set => SetProperty(ref _activePOCount, value);
        }

        // ── UI State ─────────────────────────────────────────────────────

        private bool _isDetailVisible;
        public bool IsDetailVisible
        {
            get => _isDetailVisible;
            set => SetProperty(ref _isDetailVisible, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // ── Form Fields ──────────────────────────────────────────────────

        private string _editNameNV;
        public string EditNameNV
        {
            get => _editNameNV;
            set => SetProperty(ref _editNameNV, value);
        }

        private string _editCodeNV;
        public string EditCodeNV
        {
            get => _editCodeNV;
            set => SetProperty(ref _editCodeNV, value);
        }

        private string _editPO;
        public string EditPO
        {
            get => _editPO;
            set => SetProperty(ref _editPO, value);
        }

        private int _editStep;
        public int EditStep
        {
            get => _editStep;
            set => SetProperty(ref _editStep, value);
        }

        private string _editType;
        public string EditType
        {
            get => _editType;
            set => SetProperty(ref _editType, value);
        }

        private int? _editCount;
        public int? EditCount
        {
            get => _editCount;
            set => SetProperty(ref _editCount, value);
        }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        // ── Commands ─────────────────────────────────────────────────────

        public ICommand LoadAllCommand { get; }
        public ICommand FilterByCodeNVCommand { get; }
        public ICommand FilterByPOCommand { get; }
        public ICommand FilterByProductItemCommand { get; }
        public ICommand ClearFilterCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SelectRecordCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelEditCommand { get; }

        // ── Handlers ─────────────────────────────────────────────────────

        private void LoadAll()
        {
            _ = LoadAllAsync();
        }

        private async Task LoadAllAsync()
        {
            try
            {
                IsLoading = true;
                FilterCodeNV = ""; FilterPO = ""; FilterProductItemId = "";
                
                var data = await _service.GetAllAsync();
                HistoryRecords.Clear();
                foreach (var item in data.OrderByDescending(x => x.Cdt))
                    HistoryRecords.Add(item);

                UpdateSummary();
                StatusMessage = $"Tải thành công {HistoryRecords.Count} bản ghi";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExecuteFilterByCodeNVAsync()
        {
            if (string.IsNullOrWhiteSpace(FilterCodeNV)) return;
            try
            {
                IsLoading = true;
                StatusMessage = $"Lọc theo mã NV: {FilterCodeNV}";
                var data = await _service.GetByCodeNVAsync(FilterCodeNV);
                HistoryRecords.Clear();
                foreach (var item in data) HistoryRecords.Add(item);
                UpdateSummary();
            }
            catch (Exception ex) { StatusMessage = $"Lỗi: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        private async Task ExecuteFilterByPOAsync()
        {
            if (string.IsNullOrWhiteSpace(FilterPO)) return;
            try
            {
                IsLoading = true;
                StatusMessage = $"Lọc theo PO: {FilterPO}";
                var data = await _service.GetByPOAsync(FilterPO);
                HistoryRecords.Clear();
                foreach (var item in data) HistoryRecords.Add(item);
                UpdateSummary();
            }
            catch (Exception ex) { StatusMessage = $"Lỗi: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        private async Task ExecuteFilterByProductItemAsync()
        {
            if (!int.TryParse(FilterProductItemId, out int id))
            {
                StatusMessage = "ID ProductItem không hợp lệ";
                return;
            }
            try
            {
                IsLoading = true;
                StatusMessage = $"Lọc theo Item: {id}";
                var data = await _service.GetByItemIdAsync(id);
                HistoryRecords.Clear();
                foreach (var item in data) HistoryRecords.Add(item);
                UpdateSummary();
            }
            catch (Exception ex) { StatusMessage = $"Lỗi: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        private void ClearFilters()
        {
            FilterCodeNV = ""; FilterPO = ""; FilterProductItemId = "";
            StatusMessage = "Đã xóa bộ lọc";
        }

        private void OpenAddDialog()
        {
            IsEditMode = false;
            EditNameNV = ""; EditCodeNV = ""; EditPO = ""; EditStep = 0; EditType = "SCAN"; EditCount = 1;
            IsDetailVisible = true;
            SelectedRecord = null;
            StatusMessage = "Ghi log thao tác mới";
        }

        private void OpenEditDialog(NaviHistoryDto record)
        {
            if (record == null) return;
            IsEditMode = true;
            EditNameNV = record.NameNV;
            EditCodeNV = record.CodeNV;
            EditPO = record.PO;
            EditStep = record.Step ?? 0;
            EditType = record.Type;
            EditCount = record.Count;
            IsDetailVisible = true;
            StatusMessage = $"Chỉnh sửa: #{record.Id}";
        }

        private async Task ExecuteSaveAsync()
        {
            try
            {
                IsLoading = true;
                if (IsEditMode && SelectedRecord != null)
                {
                    var updateDto = new UpdateNaviHistoryDto
                    {
                        NameNV = EditNameNV, CodeNV = EditCodeNV, PO = EditPO,
                        Step = EditStep, Type = EditType, Count = EditCount,
                        ItemId = SelectedRecord.ItemId
                    };
                    var updated = await _service.UpdateAsync(SelectedRecord.Id, updateDto);
                    
                    // Update local collection
                    var index = HistoryRecords.IndexOf(SelectedRecord);
                    if (index != -1) HistoryRecords[index] = updated;
                    
                    StatusMessage = $"Đã cập nhật record #{updated.Id}";
                }
                else
                {
                    var createDto = new CreateNaviHistoryDto
                    {
                        NameNV = EditNameNV, CodeNV = EditCodeNV, PO = EditPO,
                        Step = EditStep, Type = EditType, Count = EditCount,
                        ItemId = 0
                    };
                    var created = await _service.CreateAsync(createDto);
                    HistoryRecords.Insert(0, created);
                    StatusMessage = $"Đã thêm record mới #{created.Id}";
                }
                IsDetailVisible = false;
                UpdateSummary();
            }
            catch (Exception ex) { StatusMessage = $"Lỗi lưu: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        private async Task ExecuteDeleteAsync(NaviHistoryDto record)
        {
            if (record == null) return;
            try
            {
                IsLoading = true;
                bool deleted = await _service.DeleteAsync(record.Id);
                if (deleted)
                {
                    HistoryRecords.Remove(record);
                    SelectedRecord = null;
                    IsDetailVisible = false;
                    StatusMessage = $"Đã xóa record #{record.Id}";
                    UpdateSummary();
                }
            }
            catch (Exception ex) { StatusMessage = $"Lỗi xóa: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        private void SelectRecord(NaviHistoryDto record)
        {
            if (record == null) return;
            SelectedRecord = record;
            IsDetailVisible = true;
            EditNameNV = record.NameNV; EditCodeNV = record.CodeNV;
            EditPO = record.PO; EditStep = record.Step ?? 0;
            EditType = record.Type; EditCount = record.Count;
            IsEditMode = false;
            StatusMessage = $"Xem chi tiết: #{record.Id} — {record.NameNV}";
        }

        private void UpdateSummary()
        {
            TotalCount = HistoryRecords.Count;
            // Simplified counts for demo
            UniqueEmployeeCount = 3;
            ActivePOCount = 2;
        }

        // ── Sample Data ──────────────────────────────────────────────────

        private void LoadSampleData()
        {
            HistoryRecords.Add(new NaviHistoryDto { Id = 1, NameNV = "Nguyễn Văn A", CodeNV = "NV001", PO = "PO-2025-001", Step = 3, Type = "SCAN", Count = 1, Cdt = DateTime.Now.AddHours(-2), Udt = DateTime.Now.AddHours(-2) });
            HistoryRecords.Add(new NaviHistoryDto { Id = 2, NameNV = "Trần Thị B", CodeNV = "NV002", PO = "PO-2025-001", Step = 1, Type = "CHECK", Count = 2, Cdt = DateTime.Now.AddHours(-1), Udt = DateTime.Now.AddHours(-1) });
            HistoryRecords.Add(new NaviHistoryDto { Id = 3, NameNV = "Lê Văn C", CodeNV = "NV003", PO = "PO-2025-002", Step = 2, Type = "SCAN", Count = 1, Cdt = DateTime.Now.AddMinutes(-30), Udt = DateTime.Now.AddMinutes(-30) });
            HistoryRecords.Add(new NaviHistoryDto { Id = 4, NameNV = "Nguyễn Văn A", CodeNV = "NV001", PO = "PO-2025-002", Step = 4, Type = "CONFIRM", Count = 1, Cdt = DateTime.Now.AddMinutes(-10), Udt = DateTime.Now.AddMinutes(-10) });
            HistoryRecords.Add(new NaviHistoryDto { Id = 5, NameNV = "Phạm Thị D", CodeNV = "NV004", PO = "PO-2025-003", Step = 1, Type = "SCAN", Count = 3, Cdt = DateTime.Now.AddMinutes(-5), Udt = DateTime.Now.AddMinutes(-5) });
            UpdateSummary();
            StatusMessage = $"Tải thành công {HistoryRecords.Count} bản ghi";
        }
    }
}
