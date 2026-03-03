# UI Plan: NaviProductItem Management

## Mục tiêu

Giao diện quản lý **Mối quan hệ Sản phẩm ↔ Bước (NaviProductItem)** — xem, tạo, xóa mối liên kết nhiều-nhiều giữa `NaviProduct` và `NaviItem`. Tương ứng bộ API `/api/naviproductitems`.

> **Lưu ý thiết kế:** Màn hình này chủ yếu dùng cho admin/kỹ thuật viên để xem toàn bộ liên kết, tra cứu và chỉnh sửa trực tiếp. Luồng thêm/sửa liên kết thông thường được thực hiện qua màn hình **NaviProduct** (dialog Quản lý Items).

---

## Màn hình & Layout

### Màn hình chính: `NaviProductItemView.xaml`

Layout dạng **Master/Filter + Result Grid**:

```
┌────────────────────────────────────────────────────────────────┐
│  Filter                                    [+ Thêm liên kết]   │
│  ┌──────────────────────┐  ┌──────────────────────┐            │
│  │ Sản phẩm: [SP001  ▼] │  │ Item:   [Tất cả    ▼]│            │
│  └──────────────────────┘  └──────────────────────┘            │
│  [🔍 Kiểm tra tồn tại: ProductId ___  ItemId ___  [Kiểm tra]]  │
├────────────────────────────────────────────────────────────────┤
│  DataGrid                                                       │
│  ┌────┬──────────────────┬─────────────────────────┬─────────┐  │
│  │ ID │ Sản phẩm         │ Bước lắp ráp             │ Xóa     │  │
│  ├────┼──────────────────┼─────────────────────────┼─────────┤  │
│  │  1 │ SP001            │ Step 1 - Siết bu lông    │   🗑️   │  │
│  │  2 │ SP001            │ Step 2 - Kiểm tra lực    │   🗑️   │  │
│  │  3 │ SP002            │ Step 1 - Siết bu lông    │   🗑️   │  │
│  └────┴──────────────────┴─────────────────────────┴─────────┘  │
│                                           [◀ Trang 1/2 ▶]       │
└────────────────────────────────────────────────────────────────┘
```

---

## Dialogs

### 1. Dialog Thêm liên kết — `ProductItemLinkDialog.xaml`

```
┌────────────────────────────────────────┐
│  Thêm liên kết Sản phẩm ↔ Bước         │
│────────────────────────────────────────│
│  Sản phẩm:  [Chọn sản phẩm...     ▼]  │
│  Bước:      [Chọn bước lắp ráp... ▼]  │
│────────────────────────────────────────│
│  ⚠️ Liên kết đã tồn tại: (hiện nếu có) │
│────────────────────────────────────────│
│              [Thêm]     [Hủy]          │
└────────────────────────────────────────┘
```

- Khi chọn Product và Item → tự động gọi `GET /api/naviproductitems/exists?productId=&itemId=` để kiểm tra trùng
- Hiển thị cảnh báo ngay nếu đã tồn tại (disable nút Thêm)

---

### 2. Confirm Dialog xóa liên kết

```
┌─────────────────────────────────────────────┐
│ ⚠️ Xác nhận xóa                              │
│                                             │
│  Bạn có chắc muốn xóa liên kết:           │
│  SP001  ↔  Step 1 - Siết bu lông?          │
│                                             │
│       [Xóa]           [Hủy]                │
└─────────────────────────────────────────────┘
```

---

## Các chế độ xem (View Modes)

| Mode | Cách filter | API được gọi |
|---|---|---|
| Tất cả | Không filter | `GET /api/naviproductitems` |
| Theo sản phẩm | Chọn ComboBox sản phẩm | `GET /api/naviproductitems/product/{productId}` |
| Theo item | Chọn ComboBox item | `GET /api/naviproductitems/item/{itemId}` |
| Kiểm tra tồn tại | Nhập productId + itemId | `GET /api/naviproductitems/exists?productId=&itemId=` |

---

## ViewModel: `NaviProductItemViewModel.cs`

### Properties
```
ObservableCollection<NaviProductItemDto>   ProductItems
ObservableCollection<NaviProductDto>       AllProducts    ← dùng cho ComboBox filter
ObservableCollection<NaviItemDto>          AllItems       ← dùng cho ComboBox filter
NaviProductDto                             FilterByProduct
NaviItemDto                                FilterByItem
int                                        CheckProductId
int                                        CheckItemId
bool                                       ExistsCheckResult
string                                     ExistsCheckMessage
bool                                       IsLoading
```

### Commands
```
LoadAllCommand              → GET /api/naviproductitems
FilterByProductCommand      → GET /api/naviproductitems/product/{productId}
FilterByItemCommand         → GET /api/naviproductitems/item/{itemId}
CheckExistsCommand          → GET /api/naviproductitems/exists?productId=&itemId=
OpenAddLinkDialogCommand    → mở ProductItemLinkDialog
DeleteLinkCommand           → DELETE /api/naviproductitems/{id} (confirm)
```

---

## API Calls Mapping

| Action | Method | Endpoint |
|---|---|---|
| Load tất cả | GET | `/api/naviproductitems` |
| Lấy theo Id | GET | `/api/naviproductitems/{id}` |
| Filter theo Product | GET | `/api/naviproductitems/product/{productId}` |
| Filter theo Item | GET | `/api/naviproductitems/item/{itemId}` |
| Kiểm tra tồn tại | GET | `/api/naviproductitems/exists?productId=&itemId=` |
| Tạo liên kết | POST | `/api/naviproductitems` |
| Xóa liên kết | DELETE | `/api/naviproductitems/{id}` |

---

## Files cần tạo

| File | Loại |
|---|---|
| `Navi_UI_WPF/Views/NaviProductItemView.xaml` | View |
| `Navi_UI_WPF/Views/NaviProductItemView.xaml.cs` | Code-behind |
| `Navi_UI_WPF/Views/Dialogs/ProductItemLinkDialog.xaml` | Dialog |
| `Navi_UI_WPF/ViewModels/NaviProductItemViewModel.cs` | ViewModel |
