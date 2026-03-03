# UI Plan: NaviProduct Management

## Mục tiêu

Giao diện quản lý **Sản phẩm (NaviProduct)** — xem, tạo, sửa, xóa sản phẩm và quản lý danh sách bước lắp ráp liên kết. Tương ứng bộ API `/api/naviproducts`.

---

## Màn hình & Layout

### Màn hình chính: `NaviProductView.xaml`

```
┌────────────────────────────────────────────────────────────────┐
│ [🔍 Tìm kiếm...              ]  [+ Thêm sản phẩm]  [📥 Import Excel] │
├──────────────────────────────────────────────────────────────────┤
│  DataGrid — Danh sách sản phẩm                                   │
│  ┌────┬──────────────────┬────────────────────┬──────┬────────┐  │
│  │ ID │ Tên sản phẩm     │ Mô tả              │ Ngày │ Thao tác│  │
│  ├────┼──────────────────┼────────────────────┼──────┼────────┤  │
│  │  1 │ SP001            │ Mô tả...           │ ...  │ ✏️ 🗑️  │  │
│  │  2 │ SP002            │ Mô tả...           │ ...  │ ✏️ 🗑️  │  │
│  └────┴──────────────────┴────────────────────┴──────┴────────┘  │
│                                            [◀ Trang 1/5 ▶]       │
└────────────────────────────────────────────────────────────────┘
```

### Detail Panel (bên phải, hiện khi click vào dòng)

```
┌─────────────────────────────────────┐
│ Chi tiết sản phẩm: SP001            │
│─────────────────────────────────────│
│ Tên:  [SP001                      ] │
│ Mô tả:[...                        ] │
│─────────────────────────────────────│
│ Danh sách bước lắp ráp (Items):     │
│ ┌──────┬──────────────────┬────────┐ │
│ │ Step │ Mô tả            │ Loại   │ │
│ │  1   │ Siết bu lông     │ Asm    │ │
│ │  2   │ Kiểm tra lực     │ Val    │ │
│ └──────┴──────────────────┴────────┘ │
│ [Quản lý items...]                   │
│─────────────────────────────────────│
│          [Lưu]    [Hủy]             │
└─────────────────────────────────────┘
```

---

## Dialogs / Popups

### 1. Dialog Thêm / Sửa Sản phẩm — `ProductFormDialog.xaml`

| Field | Control | Validation |
|---|---|---|
| Tên sản phẩm | TextBox | Required, max 200 ký tự |
| Mô tả | TextBox multiline | Optional |

Buttons: **[Lưu]** / **[Hủy]**

---

### 2. Dialog Quản lý Items — `ProductItemsDialog.xaml`

- Hiển thị 2 cột:
  - **Trái:** Danh sách tất cả Items (có search) — chưa liên kết
  - **Phải:** Danh sách Items đã liên kết với sản phẩm này
- Nút `→` thêm Item vào sản phẩm, nút `←` gỡ Item
- Lưu toàn bộ thay đổi qua `PUT /api/naviproducts/{id}/with-items`

```
┌───────────────────────────────────────────────────────┐
│           Quản lý Items cho SP001                      │
├───────────────────┬───────┬───────────────────────────┤
│ Tất cả Items      │       │ Items đã liên kết          │
│ [🔍 Tìm...]       │  →   │                            │
│ ─────────────     │  ←   │ Step 1 - Siết bu lông      │
│ Step 1 - ...      │       │ Step 2 - Kiểm tra lực      │
│ Step 3 - ...      │       │                            │
├───────────────────┴───────┴───────────────────────────┤
│                           [Lưu thay đổi]    [Hủy]     │
└───────────────────────────────────────────────────────┘
```

---

### 3. Dialog Import Excel — `ImportExcelDialog.xaml`

| Bước | UI |
|---|---|
| 1. Chọn file | Button `[Chọn file .xlsx]` + label hiển thị tên file đã chọn |
| 2. Preview | DataGrid hiển thị 5 dòng đầu tiên của file |
| 3. Import | Button `[Bắt đầu Import]` + ProgressBar |
| 4. Kết quả | TextBlock: "✅ 80 thêm mới · 🔄 15 cập nhật · ⏭️ 3 bỏ qua · ❌ 2 lỗi" |
| 5. Chi tiết lỗi | DataGrid hiển thị `errors[]` (rowNumber, productName, reason) |

---

## ViewModel: `NaviProductViewModel.cs`

### Properties
```
ObservableCollection<NaviProductDto>     Products
NaviProductDto                           SelectedProduct
NaviProductWithItemsDto                  SelectedProductWithItems
string                                   SearchTerm
bool                                     IsLoading
bool                                     IsDetailPanelVisible
string                                   StatusMessage
```

### Commands
```
LoadProductsCommand          → GET /api/naviproducts
SearchCommand                → GET /api/naviproducts/search?term=
OpenAddDialogCommand         → mở ProductFormDialog (Create mode)
OpenEditDialogCommand        → mở ProductFormDialog (Edit mode)
DeleteProductCommand         → DELETE /api/naviproducts/{id}/with-items (có confirm dialog)
OpenItemsDialogCommand       → mở ProductItemsDialog
OpenImportExcelCommand       → mở ImportExcelDialog
SelectProductCommand         → GET /api/naviproducts/{id}/items → hiện detail panel
```

---

## API Calls Mapping

| Action | Method | Endpoint |
|---|---|---|
| Load list | GET | `/api/naviproducts` |
| Tìm kiếm | GET | `/api/naviproducts/search?term=` |
| Xem detail + items | GET | `/api/naviproducts/{id}/items` |
| Thêm mới | POST | `/api/naviproducts` |
| Cập nhật | PUT | `/api/naviproducts/{id}` |
| Xóa | DELETE | `/api/naviproducts/{id}/with-items` |
| Cập nhật items | PUT | `/api/naviproducts/{id}/with-items` |
| Import Excel | POST | `/api/naviproducts/import-excel` |

---

## Files cần tạo

| File | Loại |
|---|---|
| `Navi_UI_WPF/Views/NaviProductView.xaml` | View |
| `Navi_UI_WPF/Views/NaviProductView.xaml.cs` | Code-behind |
| `Navi_UI_WPF/Views/Dialogs/ProductFormDialog.xaml` | Dialog |
| `Navi_UI_WPF/Views/Dialogs/ProductItemsDialog.xaml` | Dialog |
| `Navi_UI_WPF/Views/Dialogs/ImportExcelDialog.xaml` | Dialog |
| `Navi_UI_WPF/ViewModels/NaviProductViewModel.cs` | ViewModel |
