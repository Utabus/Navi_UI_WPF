using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Navi.Application.DTOs;

namespace Navi_UI_WPF.ViewModels
{
    /// <summary>
    /// ViewModel quản lý liên kết Product ↔ Item (NaviProductItem)
    /// </summary>
    public class NaviProductItemViewModel : ObservableObject
    {
        public NaviProductItemViewModel()
        {
            ProductItems = new ObservableCollection<NaviProductItemDto>();
            AllProducts = new ObservableCollection<NaviProductDto>();
            AllItems = new ObservableCollection<NaviItemDto>();
            LoadSampleData();

            LoadAllCommand          = new RelayCommand(() => LoadAll());
            FilterByProductCommand  = new RelayCommand(() => ExecuteFilterByProduct());
            FilterByItemCommand     = new RelayCommand(() => ExecuteFilterByItem());
            CheckExistsCommand      = new RelayCommand(() => CheckExists());
            AddLinkCommand          = new RelayCommand(() => OpenAddLinkDialog());
            DeleteLinkCommand       = new RelayCommand<NaviProductItemDto>(link => ExecuteDeleteLink(link));
            ClearFilterCommand      = new RelayCommand(() => ClearFilters());
        }

        // ── Collections ──────────────────────────────────────────────────

        private ObservableCollection<NaviProductItemDto> _productItems;
        public ObservableCollection<NaviProductItemDto> ProductItems
        {
            get => _productItems;
            set => SetProperty(ref _productItems, value);
        }

        private ObservableCollection<NaviProductDto> _allProducts;
        public ObservableCollection<NaviProductDto> AllProducts
        {
            get => _allProducts;
            set => SetProperty(ref _allProducts, value);
        }

        private ObservableCollection<NaviItemDto> _allItems;
        public ObservableCollection<NaviItemDto> AllItems
        {
            get => _allItems;
            set => SetProperty(ref _allItems, value);
        }

        // ── Filters ──────────────────────────────────────────────────────

        private NaviProductDto _filterByProduct;
        public NaviProductDto FilterByProduct
        {
            get => _filterByProduct;
            set => SetProperty(ref _filterByProduct, value);
        }

        private NaviItemDto _filterByItem;
        public NaviItemDto FilterByItem
        {
            get => _filterByItem;
            set => SetProperty(ref _filterByItem, value);
        }

        // ── Exists Check ─────────────────────────────────────────────────

        private int _checkProductId;
        public int CheckProductId
        {
            get => _checkProductId;
            set => SetProperty(ref _checkProductId, value);
        }

        private int _checkItemId;
        public int CheckItemId
        {
            get => _checkItemId;
            set => SetProperty(ref _checkItemId, value);
        }

        private string _existsCheckMessage;
        public string ExistsCheckMessage
        {
            get => _existsCheckMessage;
            set => SetProperty(ref _existsCheckMessage, value);
        }

        // ── Add Link Form ─────────────────────────────────────────────────

        private NaviProductDto _newLinkProduct;
        public NaviProductDto NewLinkProduct
        {
            get => _newLinkProduct;
            set { SetProperty(ref _newLinkProduct, value); RunExistsCheck(); }
        }

        private NaviItemDto _newLinkItem;
        public NaviItemDto NewLinkItem
        {
            get => _newLinkItem;
            set { SetProperty(ref _newLinkItem, value); RunExistsCheck(); }
        }

        private bool _isAddPanelVisible;
        public bool IsAddPanelVisible
        {
            get => _isAddPanelVisible;
            set => SetProperty(ref _isAddPanelVisible, value);
        }

        private bool _linkAlreadyExists;
        public bool LinkAlreadyExists
        {
            get => _linkAlreadyExists;
            set => SetProperty(ref _linkAlreadyExists, value);
        }

        // ── UI State ─────────────────────────────────────────────────────

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

        // ── Commands ─────────────────────────────────────────────────────

        public ICommand LoadAllCommand { get; }
        public ICommand FilterByProductCommand { get; }
        public ICommand FilterByItemCommand { get; }
        public ICommand CheckExistsCommand { get; }
        public ICommand AddLinkCommand { get; }
        public ICommand DeleteLinkCommand { get; }
        public ICommand ClearFilterCommand { get; }

        // ── Handlers ─────────────────────────────────────────────────────

        private void LoadAll()
        {
            FilterByProduct = null;
            FilterByItem = null;
            StatusMessage = $"Tải thành công {ProductItems.Count} liên kết";
        }

        private void ExecuteFilterByProduct()
        {
            if (FilterByProduct == null) return;
            StatusMessage = $"Lọc theo sản phẩm: {FilterByProduct.ProductName}";
            // TODO: gọi INaviProductItemService.GetByProductAsync(FilterByProduct.Id)
        }

        private void ExecuteFilterByItem()
        {
            if (FilterByItem == null) return;
            StatusMessage = $"Lọc theo bước: {FilterByItem.Description}";
            // TODO: gọi INaviProductItemService.GetByItemAsync(FilterByItem.Id)
        }

        private void CheckExists()
        {
            bool exists = CheckProductId > 0 && CheckItemId > 0;
            ExistsCheckMessage = exists
                ? $"✅ Liên kết ProductId={CheckProductId} ↔ ItemId={CheckItemId} đã tồn tại"
                : $"❌ Chưa có liên kết ProductId={CheckProductId} ↔ ItemId={CheckItemId}";
            // TODO: gọi INaviProductItemService.ExistsAsync
        }

        private void RunExistsCheck()
        {
            if (NewLinkProduct != null && NewLinkItem != null)
            {
                LinkAlreadyExists = false; // TODO: check từ service
                StatusMessage = LinkAlreadyExists ? "⚠️ Liên kết đã tồn tại!" : "";
            }
        }

        private void OpenAddLinkDialog()
        {
            NewLinkProduct = null;
            NewLinkItem = null;
            LinkAlreadyExists = false;
            IsAddPanelVisible = true;
            StatusMessage = "Thêm liên kết mới";
        }

        private void ExecuteDeleteLink(NaviProductItemDto link)
        {
            if (link == null) return;
            ProductItems.Remove(link);
            StatusMessage = $"Đã xóa liên kết ID={link.Id}";
            // TODO: gọi INaviProductItemService.DeleteAsync(link.Id)
        }

        private void ClearFilters()
        {
            FilterByProduct = null;
            FilterByItem = null;
            StatusMessage = "Đã xóa bộ lọc";
        }

        // ── Sample Data ──────────────────────────────────────────────────

        private void LoadSampleData()
        {
            AllProducts.Add(new NaviProductDto { Id = 1, ProductName = "SP001" });
            AllProducts.Add(new NaviProductDto { Id = 2, ProductName = "SP002" });
            AllProducts.Add(new NaviProductDto { Id = 3, ProductName = "SP003" });

            AllItems.Add(new NaviItemDto { Id = 1, Step = 1, Description = "Siết bu lông M10", Type = "Assembly" });
            AllItems.Add(new NaviItemDto { Id = 2, Step = 2, Description = "Kiểm tra lực siết", Type = "Validation" });
            AllItems.Add(new NaviItemDto { Id = 3, Step = 3, Description = "Lắp vòng đệm", Type = "Assembly" });

            ProductItems.Add(new NaviProductItemDto { Id = 1, ProductId = 1, ItemId = 1, ProductName = "SP001", ItemDescription = "Siết bu lông M10", ItemStep = 1 });
            ProductItems.Add(new NaviProductItemDto { Id = 2, ProductId = 1, ItemId = 2, ProductName = "SP001", ItemDescription = "Kiểm tra lực siết", ItemStep = 2 });
            ProductItems.Add(new NaviProductItemDto { Id = 3, ProductId = 2, ItemId = 1, ProductName = "SP002", ItemDescription = "Siết bu lông M10", ItemStep = 1 });
            ProductItems.Add(new NaviProductItemDto { Id = 4, ProductId = 3, ItemId = 3, ProductName = "SP003", ItemDescription = "Lắp vòng đệm", ItemStep = 3 });

            StatusMessage = $"Tải thành công {ProductItems.Count} liên kết";
        }
    }
}
