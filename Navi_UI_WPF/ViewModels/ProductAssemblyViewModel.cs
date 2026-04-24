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
using Navi.Application.Services;
using Navi_UI_WPF.Services;
using Navi_UI_WPF.Views;
using Serilog;
using HandyControl.Controls;
using HandyControl.Data;
using MessageBox = System.Windows.MessageBox;

namespace Navi_UI_WPF.ViewModels
{
    /// <summary>
    /// ViewModel chính cho Product Assembly View cụ thể
    /// </summary>
    public class ProductAssemblyViewModel : BaseAssemblyViewModel
    {
        private readonly ProductAssemblyService _service;
        private readonly ManufaService _manufaService;
        
        public ICommand SearchPoCommand { get; }

        public ProductAssemblyViewModel(
            ProductAssemblyService service, 
            ManufaService manufaService,
            INaviHistoryService historyService,
            IDeviceService deviceService,
            INaviItemService naviItemService,
            IServiceProvider serviceProvider)
            : base(historyService, deviceService, naviItemService, serviceProvider)
        {
            _service = service;
            _manufaService = manufaService;
            
            SearchPoCommand = new AsyncRelayCommand(SearchPoAsync);
        }

        #region Properties riêng

        private string _manufaComments;
        public string ManufaComments
        {
            get => _manufaComments;
            set => SetProperty(ref _manufaComments, value);
        }

        private string _phtx;
        public string Phtx
        {
            get => _phtx;
            set => SetProperty(ref _phtx, value);
        }

        private string _phcd;
        public string Phcd
        {
            get => _phcd;
            set => SetProperty(ref _phcd, value);
        }

        #endregion

        #region Overrides

        public override async Task LoadProductAssemblyAsync(string productName, string po = null)
        {
            if (string.IsNullOrEmpty(productName)) return;
             
            try
            {
                IsLoading = true;
                ErrorMessage = null;
                
                CurrentStep = null;
                CurrentStepIndex = -1;
                
                List<NaviItemDto> items;
                if (!string.IsNullOrEmpty(po))
                {
                    var itemsWithStatus = await _naviItemService.GetWithHistoryStatusAsync(productName, po);
                    items = itemsWithStatus.Cast<NaviItemDto>().ToList();
                }
                else
                {
                    items = await _naviItemService.GetByProductMasterNameAsync(productName);
                }
                
                if (items == null || items.Count == 0)
                {
                    ErrorMessage = $"Không tìm thấy dữ liệu lắp ráp cho sản phẩm: {productName}";
                    return;
                }

                ProductId = 0; 
                ProductName = productName;
                Description = $"Danh sách bước lắp ráp cho {productName}";
                CreatedDate = DateTime.Now;
                UpdatedDate = DateTime.Now;
                
                Steps.Clear();
                foreach (var item in items)
                {
                    var statusDto = item as NaviItemStatusDto;
                    var step = new AssemblyStepViewModel
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Note = item.Note,
                        Bolts = item.Bolts,
                        Force = item.Force,
                        Images = item.Images,
                        Type = item.Type,
                        IsCompleted = statusDto?.OK ?? false,
                        IsNg = statusDto?.NG ?? false,
                        IsCurrent = false,
                        StepNumber = item.Step ?? (Steps.Count + 1),
                        Grease = item.Grease,
                        ForceBit = item.ForceBit,
                        Timer = item.Timer,
                        PO = statusDto?.PO,
                        HistoryNote = statusDto?.HistoryNote,
                        Count = statusDto?.Count,
                        OK = statusDto?.OK,
                        NG = statusDto?.NG,
                        ItemAuditId = statusDto?.ItemAuditId,
                        ProductId = statusDto?.ProductId ?? ProductId
                    };

                    if (ProductId == 0 && statusDto?.ProductId != null)
                    {
                        ProductId = statusDto.ProductId.Value;
                    }

                    ProcessStepDescription(step);
                    Steps.Add(step);
                }
                
                var sortedSteps = Steps.OrderBy(s => s.StepNumber).ToList();
                Steps.Clear();
                foreach (var s in sortedSteps) Steps.Add(s);

                if (Steps.Count > 0)
                {
                    _isPoAlreadyFinished = Steps.All(s => !string.IsNullOrEmpty(s.PO));
                    var firstIncompleteIndex = Steps.ToList().FindIndex(s => !s.IsCompleted);
                    CurrentStepIndex = firstIncompleteIndex != -1 ? firstIncompleteIndex : 0;
                }
                
                UpdateStepsLockStatus();
                OnPropertyChanged(nameof(TotalSteps));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error loading assembly items for {ProductName}", productName);
                ErrorMessage = $"Lỗi khi tải dữ liệu: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected override async Task RecordStepHistoryAsync(bool isOk = false, bool isNg = false, string note = null)
        {
            if (CurrentStep == null || string.IsNullOrEmpty(PoNumber)) return;
            if (_isPoAlreadyFinished) return;

            try
            {
                var user = SessionManager.Instance.CurrentUser;
                var dto = new CreateNaviHistoryDto
                {
                    PO = PoNumber,
                    Step = CurrentStep.StepNumber,
                    ProductName = ProductName,
                    CodeNV = user?.UserId ?? "Unknown",
                    NameNV = user?.UserName ?? "Unknown",
                    Device = _deviceService.GetDeviceInfo(),
                    ItemId = CurrentStep.Id,
                    ItemAuditId = CurrentStep.ItemAuditId,
                    ProductId = (CurrentStep.ProductId != null && CurrentStep.ProductId != 0) 
                                ? CurrentStep.ProductId 
                                : (ProductId != 0 ? (int?)ProductId : null),
                    Type = GetHistoryType(CurrentStep),
                    OK = isOk || (CurrentStep.IsCompleted),
                    NG = isNg || (CurrentStep.IsNg),
                    Note = note ?? CurrentStep.HistoryNote,
                    Count = 1
                };

                await _historyService.CreateHistoryNaviAsync(dto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error recording history for step {Step}", CurrentStep?.StepNumber);
            }
        }

        protected override void ClearAssemblyData()
        {
            base.ClearAssemblyData();
            Phcd = string.Empty;
            Phtx = string.Empty;
            ManufaComments = string.Empty;
        }

        #endregion

        #region Methods riêng

        private async Task SearchPoAsync()
        {
            if (string.IsNullOrWhiteSpace(PoNumber)) return;
             
            ManufaComments = string.Empty;
            Phtx = string.Empty;
            Phcd = string.Empty;

            try 
            {
                IsLoading = true;

                var manufaData = await _manufaService.GetAssistByPOAsync(PoNumber);
                if (manufaData == null)
                {
                    System.Windows.MessageBox.Show($"PO không tồn tại.", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    PoNumber = null;
                    return;
                }

                if (manufaData != null)
                {
                    await LoadProductAssemblyAsync(manufaData.Phtx, PoNumber);
                    ManufaComments = _manufaService.GetConcatenatedComments(manufaData);
                    Phtx = manufaData.Phtx;
                    Phcd = manufaData.Phcd;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in SearchPoAsync");
                ErrorMessage = $"Lỗi khi tìm kiếm PO: {ex.Message}";
                System.Windows.MessageBox.Show($"PO không tồn tại.", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion
    }
}

