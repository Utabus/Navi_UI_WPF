# UI Plan: NaviHistory Management

## Mục tiêu

Giao diện quản lý **Lịch sử thao tác (NaviHistory)** — xem, tìm kiếm, và ghi nhận lịch sử thao tác của nhân viên trong quá trình sản xuất. Tương ứng bộ API `/api/navihistory`.

> **Đặc thù:** Màn hình này chủ yếu là **read-heavy** (xem & filter lịch sử). Tạo mới thường được tự động khi nhân viên scan/thao tác. Sửa và xóa chỉ dành cho admin/supervisor.

---

## Màn hình & Layout

### Màn hình chính: `NaviHistoryView.xaml`

Layout dạng **Filter Bar + DataGrid + Summary Cards**:

```
┌──────────────────────────────────────────────────────────────────┐
│  ┌────────────┐  ┌──────────────┐  ┌─────────────┐  [Tìm kiếm] │
│  │ Mã NV: ___ │  │ PO: ________│  │ Từ ngày:___ │  [Xóa filter]│
│  └────────────┘  └──────────────┘  └─────────────┘  [+ Ghi log] │
├──────────────────────────────────────────────────────────────────┤
│  Cards tóm tắt:                                                   │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐            │
│  │ Tổng records │  │ Nhân viên    │  │ PO đang chạy │            │
│  │     1,240    │  │     12       │  │      5       │            │
│  └──────────────┘  └──────────────┘  └──────────────┘            │
├──────────────────────────────────────────────────────────────────┤
│  DataGrid                                                         │
│  ┌────┬────────┬───────┬─────────────┬───────┬───────┬─────────┐ │
│  │ ID │ Mã NV  │ Tên NV│ PO          │ Bước  │ Loại  │ Thao tác│ │
│  ├────┼────────┼───────┼─────────────┼───────┼───────┼─────────┤ │
│  │  1 │ NV001  │ A.Văn │ PO-2025-001 │   3   │ SCAN  │ ✏️ 🗑️   │ │
│  │  2 │ NV002  │ B.Thị │ PO-2025-002 │   1   │ CHECK │ ✏️ 🗑️   │ │
│  └────┴────────┴───────┴─────────────┴───────┴───────┴─────────┘ │
│                                             [◀ Trang 1/50 ▶]     │
└──────────────────────────────────────────────────────────────────┘
```

---

## Side Panel (xem detail khi click dòng)

```
┌────────────────────────────────────────┐
│ Chi tiết thao tác #1                   │
│────────────────────────────────────────│
│ Nhân viên:  Nguyễn Văn A (NV001)      │
│ PO:         PO-2025-001               │
│ Bước:       3                         │
│ Loại:       SCAN                      │
│ Số lượng:   1                         │
│ ProductItem: #12                      │
│────────────────────────────────────────│
│ Thời gian tạo:    27/02/2025 08:30    │
│ Thời gian cập nhật: 27/02/2025 08:35  │
│────────────────────────────────────────│
│  [Sửa]          [Xóa]                 │
└────────────────────────────────────────┘
```

---

## Dialogs

### 1. Dialog Ghi log thủ công — `HistoryFormDialog.xaml`

| Field | Control | Validation |
|---|---|---|
| Mã nhân viên (CodeNV) | TextBox | Optional |
| Tên nhân viên (NameNV) | TextBox | Optional |
| Production Order (PO) | TextBox | Optional |
| Bước sản xuất (Step) | TextBox | Optional |
| ProductItem | ComboBox (chọn từ list) | Optional |
| Loại hành động (Type) | ComboBox (SCAN, CHECK, v.v.) | Optional |
| Số lượng (Count) | NumericUpDown | Optional, ≥ 0 |

Buttons: **[Lưu]** / **[Hủy]**  
Mode: **Thêm mới** (`POST /api/navihistory`) hoặc **Sửa** (`PUT /api/navihistory/{id}`)

---

### 2. Confirm Dialog xóa

```
┌───────────────────────────────────────┐
│ ⚠️ Xác nhận xóa history record #1     │
│  Hành động này không thể hoàn tác.   │
│          [Xóa]      [Hủy]            │
└───────────────────────────────────────┘
```

---

## Các chế độ Filter

| Filter | Control | API |
|---|---|---|
| Tất cả | Mặc định | `GET /api/navihistory` |
| Theo mã NV | TextBox CodeNV | `GET /api/navihistory/nv/{codeNV}` |
| Theo PO | TextBox PO | `GET /api/navihistory/po/{po}` |
| Theo ProductItem | TextBox/ComboBox | `GET /api/navihistory/productitem/{productItemId}` |
| Theo Id | TextBox Id | `GET /api/navihistory/{id}` |

---

## ViewModel: `NaviHistoryViewModel.cs`

### Properties
```
ObservableCollection<NaviHistoryDto>   HistoryRecords
NaviHistoryDto                         SelectedRecord
string                                 FilterCodeNV
string                                 FilterPO
int?                                   FilterProductItemId
bool                                   IsLoading
bool                                   IsDetailVisible
bool                                   IsAdminMode         ← ẩn/hiện nút Sửa/Xóa
int                                    TotalCount
int                                    UniqueEmployeeCount
int                                    ActivePOCount
```

### Commands
```
LoadAllCommand               → GET /api/navihistory
FilterByCodeNVCommand        → GET /api/navihistory/nv/{codeNV}
FilterByPOCommand            → GET /api/navihistory/po/{po}
FilterByProductItemCommand   → GET /api/navihistory/productitem/{productItemId}
ClearFilterCommand           → reset về GET /api/navihistory
OpenAddDialogCommand         → mở HistoryFormDialog (Create)
OpenEditDialogCommand        → mở HistoryFormDialog (Edit)
DeleteCommand                → DELETE /api/navihistory/{id} (confirm)
SelectRecordCommand          → hiện side panel detail
ExportCommand                → (optional) xuất CSV/Excel từ kết quả hiện tại
```

---

## API Calls Mapping

| Action | Method | Endpoint |
|---|---|---|
| Load tất cả | GET | `/api/navihistory` |
| Xem theo Id | GET | `/api/navihistory/{id}` |
| Filter theo mã NV | GET | `/api/navihistory/nv/{codeNV}` |
| Filter theo ProductItem | GET | `/api/navihistory/productitem/{productItemId}` |
| Filter theo PO | GET | `/api/navihistory/po/{po}` |
| Tạo log thủ công | POST | `/api/navihistory` |
| Sửa log | PUT | `/api/navihistory/{id}` |
| Xóa mềm | DELETE | `/api/navihistory/{id}` |

---

## Notes về UX

- **Phân trang:** Bảng này có thể có hàng nghìn records → cần server-side pagination hoặc client-side virtual scrolling
- **Auto-refresh:** Option tự refresh mỗi N giây (cho màn hình production floor)
- **Read-only mode mặc định:** Các nút Sửa/Xóa chỉ hiện khi user có quyền admin
- **Export:** Nút xuất Excel từ kết quả filter hiện tại (optional)

---

## Files cần tạo

| File | Loại |
|---|---|
| `Navi_UI_WPF/Views/NaviHistoryView.xaml` | View |
| `Navi_UI_WPF/Views/NaviHistoryView.xaml.cs` | Code-behind |
| `Navi_UI_WPF/Views/Dialogs/HistoryFormDialog.xaml` | Dialog |
| `Navi_UI_WPF/ViewModels/NaviHistoryViewModel.cs` | ViewModel |
