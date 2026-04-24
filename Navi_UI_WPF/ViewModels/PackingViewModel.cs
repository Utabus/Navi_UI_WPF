using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Navi.Application.DTOs;
using Navi.Application.Interfaces;
using Navi.Application.Services;
using Navi_UI_WPF.Services;
using Serilog;

namespace Navi_UI_WPF.ViewModels
{
    /// <summary>
    /// ViewModel cho Packing View - Kế thừa từ BaseAssemblyViewModel
    /// </summary>
    public class PackingViewModel : BaseAssemblyViewModel
    {
        private readonly ManufaService _manufaService;
        public ICommand SearchPoCommand { get; }

        public PackingViewModel(
            ManufaService manufaService,
            INaviHistoryService historyService,
            IDeviceService deviceService,
            INaviItemService naviItemService,
            IServiceProvider serviceProvider)
            : base(historyService, deviceService, naviItemService, serviceProvider)
        {
            _manufaService = manufaService;
            SearchPoCommand = new AsyncRelayCommand(SearchPoAsync);
        }

        #region Overrides

        protected override string GetHistoryType(AssemblyStepViewModel step)
        {
            // Trả về type riêng cho Packing (Ví dụ: PACKING)
            return "PACKING";
        }

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
                    ErrorMessage = $"Không tìm thấy dữ liệu đóng gói cho sản phẩm: {productName}";
                    return;
                }

                ProductId = 0; 
                ProductName = productName;
                Description = $"Quy trình đóng gói cho {productName}";
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
                        Images = item.Images,
                        Type = item.Type,
                        IsCompleted = statusDto?.OK ?? false,
                        IsNg = statusDto?.NG ?? false,
                        IsCurrent = false,
                        StepNumber = item.Step ?? (Steps.Count + 1),
                        PO = statusDto?.PO,
                        HistoryNote = statusDto?.HistoryNote,
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
                Log.Error(ex, "Error loading packing items for {ProductName}", productName);
                ErrorMessage = $"Lỗi khi tải dữ liệu đóng gói: {ex.Message}";
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
                Log.Error(ex, "Error recording history for packing step {Step}", CurrentStep?.StepNumber);
            }
        }

        #endregion

        #region Methods riêng

        private async Task SearchPoAsync()
        {
            if (string.IsNullOrWhiteSpace(PoNumber)) return;
             
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
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in Packing SearchPoAsync");
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
