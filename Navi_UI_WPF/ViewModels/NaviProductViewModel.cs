using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Navi.Application.DTOs;
using Navi.Application.Interfaces;
using Navi.Application.Services;

namespace Navi_UI_WPF.ViewModels
{
    public class NaviProductViewModel : ObservableObject
    {
        private readonly System.IServiceProvider _serviceProvider;
        private readonly INaviProductService _productService;

        public NaviProductViewModel(System.IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            // TODO: Inject INaviProductService if needed
            
            NaviProducts = new ObservableCollection<NaviProductDto>();
            
            SearchCommand = new RelayCommand(ExecuteSearch);
            AddCommand = new RelayCommand(OpenAddDialog);
            EditCommand = new RelayCommand<NaviProductDto>(OpenEditDialog);
            DeleteCommand = new RelayCommand<NaviProductDto>(ExecuteDelete);
            
            LoadData();
        }

        // ─── Properties ──────────────────────────────────────────────────────

        public ObservableCollection<NaviProductDto> NaviProducts { get; }

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set => SetProperty(ref _searchTerm, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // ─── Edit Form data ───
        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

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

        private NaviProductWithItemsDto _selectedProductWithItems;
        public NaviProductWithItemsDto SelectedProductWithItems
        {
            get => _selectedProductWithItems;
            set => SetProperty(ref _selectedProductWithItems, value);
        }

        // ─── Commands ────────────────────────────────────────────────────────

        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        // ─── Methods ─────────────────────────────────────────────────────────

        private void LoadData()
        {
            StatusMessage = "Đang tải dữ liệu sản phẩm...";
            // Mock data
            NaviProducts.Clear();
            NaviProducts.Add(new NaviProductDto { Id = 1, ProductName = "Sản phẩm A", Description = "Mô tả A" });
            NaviProducts.Add(new NaviProductDto { Id = 2, ProductName = "Sản phẩm B", Description = "Mô tả B" });
            StatusMessage = "Tải dữ liệu hoàn tất.";
        }

        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                LoadData();
                return;
            }

            StatusMessage = $"Đang tìm kiếm: \"{SearchTerm}\"...";
            // TODO: gọi INaviProductService.SearchAsync(SearchTerm)
        }

        private void OpenAddDialog()
        {
            IsEditMode = false;
            EditProductName = "";
            EditDescription = "";
            SelectedProductWithItems = new NaviProductWithItemsDto();
            StatusMessage = "Thêm sản phẩm mới";

            var dialog = _serviceProvider.GetRequiredService<Views.NaviProductFormWindow>();
            dialog.ShowDialog();
        }

        private void OpenEditDialog(NaviProductDto product)
        {
            if (product == null) return;
            IsEditMode = true;
            EditProductName = product.ProductName;
            EditDescription = product.Description;
            StatusMessage = $"Chỉnh sửa: {product.ProductName}";

            var dialog = _serviceProvider.GetRequiredService<Views.NaviProductFormWindow>();
            dialog.ShowDialog();
        }

        private void ExecuteDelete(NaviProductDto product)
        {
            if (product == null) return;
            var result = MessageBox.Show($"Xác nhận xóa sản phẩm: {product.ProductName}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                NaviProducts.Remove(product);
                StatusMessage = $"Đã xóa {product.ProductName}";
            }
        }
    }
}
