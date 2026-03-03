# UI Plan: NaviItem Management

## Mục tiêu

Giao diện quản lý **Bước lắp ráp (NaviItem)** — xem, tạo, sửa, xóa từng bước thao tác. Tương ứng bộ API `/api/naviitems`.

---

## Màn hình & Layout

### Màn hình chính: `NaviItemView.xaml`

```
┌────────────────────────────────────────────────────────────────┐
│ [🔍 Tìm kiếm...       ]  [Lọc Type ▼]  [+ Thêm bước mới]      │
├────────────────────────────────────────────────────────────────┤
│  DataGrid — Danh sách bước lắp ráp                              │
│  ┌──────┬───────────────────┬──────────┬──────────┬──────────┐  │
│  │ Step │ Mô tả             │ Bu lông  │ Lực siết │ Thao tác │  │
│  ├──────┼───────────────────┼──────────┼──────────┼──────────┤  │
│  │  1   │ Siết bu lông M10  │ M10x1.5  │ 50Nm     │ ✏️ 🗑️    │  │
│  │  2   │ Kiểm tra lực siết │ -        │ -        │ ✏️ 🗑️    │  │
│  └──────┴───────────────────┴──────────┴──────────┴──────────┘  │
│                                            [◀ Trang 1/3 ▶]      │
└────────────────────────────────────────────────────────────────┘
```

### Detail Panel (hiện khi click vào dòng)

```
┌─────────────────────────────────────────────┐
│ Chi tiết bước: Step 1                        │
│─────────────────────────────────────────────│
│ Số thứ tự: [1    ]   Loại: [Assembly ▼]     │
│ Mô tả:     [Siết bu lông                  ] │
│ Bu lông:   [M10x1.5  ]  Lực: [50Nm       ] │
│ Ghi chú:   [Dùng cờ lê 13mm              ] │
│─────────────────────────────────────────────│
│ Hình ảnh minh họa:                          │
│  ┌──────┐ ┌──────┐ ┌──────┐                │
│  │img 1 │ │img 2 │ │  +   │  ← thêm ảnh   │
│  └──────┘ └──────┘ └──────┘                │
│─────────────────────────────────────────────│
│ Sản phẩm liên kết:                          │
│  SP001, SP002, SP003                        │
│─────────────────────────────────────────────│
│        [Lưu]    [Hủy]                       │
└─────────────────────────────────────────────┘
```

---

## Dialogs

### 1. Dialog Thêm / Sửa Bước — `ItemFormDialog.xaml`

| Field | Control | Validation |
|---|---|---|
| Số thứ tự (Step) | NumericUpDown / TextBox kiểu int | Optional, ≥ 1 |
| Mô tả | TextBox | Optional |
| Loại (Type) | ComboBox (Assembly, Validation, v.v.) | Optional |
| Bu lông (Bolts) | TextBox | Optional |
| Lực siết (Force) | TextBox | Optional |
| Ghi chú (Note) | TextBox multiline | Optional |
| Hình ảnh | WrapPanel + button `+ Thêm ảnh` | Optional, hiển thị thumbnail |

Buttons: **[Lưu]** / **[Hủy]**

---

### 2. Panel xem sản phẩm liên kết — trong Detail Panel

- Gọi `GET /api/naviitems/{id}/products`
- Hiển thị danh sách tên sản phẩm dạng Tag/Chip
- Read-only (quản lý liên kết thực hiện từ màn hình NaviProduct)

---

## Filter / Search

| Filter | Cơ chế |
|---|---|
| Tìm theo từ khóa | `GET /api/naviitems/search?term=` |
| Lọc theo Type | ComboBox → `GET /api/naviitems/type/{type}` |
| Hiển thị tất cả | `GET /api/naviitems` |

---

## ViewModel: `NaviItemViewModel.cs`

### Properties
```
ObservableCollection<NaviItemDto>     Items
NaviItemDto                           SelectedItem
NaviItemWithProductsDto               SelectedItemDetail
string                                SearchTerm
string                                SelectedType
List<string>                          AvailableTypes   ← lấy từ distinct Type trong list
bool                                  IsLoading
bool                                  IsDetailPanelVisible
```

### Commands
```
LoadItemsCommand          → GET /api/naviitems
SearchCommand             → GET /api/naviitems/search?term=
FilterByTypeCommand       → GET /api/naviitems/type/{type}
OpenAddDialogCommand      → mở ItemFormDialog (Create)
OpenEditDialogCommand     → mở ItemFormDialog (Edit)
DeleteItemCommand         → DELETE /api/naviitems/{id} (confirm)
SelectItemCommand         → GET /api/naviitems/{id}/products → hiện detail
```

---

## API Calls Mapping

| Action | Method | Endpoint |
|---|---|---|
| Load list | GET | `/api/naviitems` |
| Tìm kiếm | GET | `/api/naviitems/search?term=` |
| Lọc theo Type | GET | `/api/naviitems/type/{type}` |
| Xem detail + products | GET | `/api/naviitems/{id}/products` |
| Thêm mới | POST | `/api/naviitems` |
| Cập nhật | PUT | `/api/naviitems/{id}` |
| Xóa | DELETE | `/api/naviitems/{id}` |

---

## Files cần tạo

| File | Loại |
|---|---|
| `Navi_UI_WPF/Views/NaviItemView.xaml` | View |
| `Navi_UI_WPF/Views/NaviItemView.xaml.cs` | Code-behind |
| `Navi_UI_WPF/Views/Dialogs/ItemFormDialog.xaml` | Dialog |
| `Navi_UI_WPF/ViewModels/NaviItemViewModel.cs` | ViewModel |
