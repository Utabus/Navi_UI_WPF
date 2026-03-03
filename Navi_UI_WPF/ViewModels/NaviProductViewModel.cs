using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Navi_UI_WPF.Commands;
using Navi.Application.DTOs;

namespace Navi_UI_WPF.ViewModels
{
    /// <summary>
    /// ViewModel quản lý Sản phẩm (NaviProduct)
    /// </summary>
    public class NaviProductViewModel : ViewModelBase
    {
        public NaviProductViewModel()
        {
            Products = new ObservableCollection<NaviProductDto>();
            LoadSampleData();

            SearchCommand = new RelayCommand(o => ExecuteSearch());
            AddCommand = new RelayCommand(o => OpenAddDialog());
            EditCommand = new RelayCommand(o => OpenEditDialog(o as NaviProductDto), o => SelectedProduct != null);
            DeleteCommand = new RelayCommand(o => ExecuteDelete(o as NaviProductDto), o => SelectedProduct != null);
            SelectProductCommand = new RelayCommand(o => SelectProduct(o as NaviProductDto));
            ClearSelectionCommand = new RelayCommand(o => { SelectedProduct = null; IsDetailVisible = false; });
        }

        // ── Collections & Selection ──────────────────────────────────────

        private ObservableCollection<NaviProductDto> _products;
        public ObservableCollection<NaviProductDto> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        private NaviProductDto _selectedProduct;
        public NaviProductDto SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        private NaviProductWithItemsDto _selectedProductWithItems;
        public NaviProductWithItemsDto SelectedProductWithItems
        {
            get => _selectedProductWithItems;
            set => SetProperty(ref _selectedProductWithItems, value);
        }

        // ── UI State ─────────────────────────────────────────────────────

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set => SetProperty(ref _searchTerm, value);
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

        // ── Form Fields (for Add/Edit inline or dialog) ──────────────────

        private string _editProductName;
        public string EditProductName
        {
            get => _editProductName;
            set => SetProperty(ref _editProductName, value);
        }

        private string _editDescription;
        public string EditDescription
        {
            get => _editDescription;
            set => SetProperty(ref _editDescription, value);
        }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        // ── Commands ─────────────────────────────────────────────────────

        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SelectProductCommand { get; }
        public ICommand ClearSelectionCommand { get; }

        // ── Handlers ─────────────────────────────────────────────────────

        private void ExecuteSearch()
        {
            StatusMessage = $"Đang tìm kiếm: \"{SearchTerm}\"...";
            // TODO: gọi INaviProductService.SearchAsync(SearchTerm)
        }

        private void OpenAddDialog()
        {
            IsEditMode = false;
            EditProductName = "";
            EditDescription = "";
            IsDetailVisible = true;
            SelectedProductWithItems = new NaviProductWithItemsDto();
            StatusMessage = "Thêm sản phẩm mới";
        }

        private void OpenEditDialog(NaviProductDto product)
        {
            if (product == null) return;
            IsEditMode = true;
            EditProductName = product.ProductName;
            EditDescription = product.Description;
            IsDetailVisible = true;
            StatusMessage = $"Chỉnh sửa: {product.ProductName}";
        }

        private void ExecuteDelete(NaviProductDto product)
        {
            if (product == null) return;
            Products.Remove(product);
            SelectedProduct = null;
            IsDetailVisible = false;
            StatusMessage = $"Đã xóa: {product.ProductName}";
            // TODO: gọi INaviProductService.DeleteWithItemsAsync(product.Id)
        }

        private void SelectProduct(NaviProductDto product)
        {
            if (product == null) return;
            SelectedProduct = product;
            IsDetailVisible = true;
            // Tạo WithItems từ selected product (demo)
            SelectedProductWithItems = new NaviProductWithItemsDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                Cdt = product.Cdt,
                Udt = product.Udt
            };
            EditProductName = product.ProductName;
            EditDescription = product.Description;
            StatusMessage = $"Đang xem: {product.ProductName}";
        }

        // ── Sample Data ──────────────────────────────────────────────────

        private void LoadSampleData()
        {
            Products.Add(new NaviProductDto { Id = 1, ProductName = "SP001", Description = "Sản phẩm lắp ráp động cơ", Cdt = DateTime.Now.AddDays(-10), Udt = DateTime.Now });
            Products.Add(new NaviProductDto { Id = 2, ProductName = "SP002", Description = "Bộ truyền động chính", Cdt = DateTime.Now.AddDays(-5), Udt = DateTime.Now });
            Products.Add(new NaviProductDto { Id = 3, ProductName = "SP003", Description = "Khung sườn phụ", Cdt = DateTime.Now.AddDays(-2), Udt = DateTime.Now });
            Products.Add(new NaviProductDto { Id = 4, ProductName = "SP004", Description = "Cụm bánh răng hộp số", Cdt = DateTime.Now.AddDays(-1), Udt = DateTime.Now });
            StatusMessage = $"Tải thành công {Products.Count} sản phẩm";
        }
    }
}
