# CONTEXT.md — Navi UI WPF

> Tài liệu ngữ cảnh dự án. Dùng làm context cho AI assistant trong các cuộc hội thoại tiếp theo.

---

## 1. Tổng quan dự án

**Navi UI WPF** là ứng dụng desktop WPF (.NET 4.7.2) dùng trong dây chuyền sản xuất, hỗ trợ:
- Hướng dẫn lắp ráp từng bước (step-by-step assembly guide) cho công nhân
- Quản lý sản phẩm, items, quan hệ product↔item, lịch sử thao tác
- Đo lực (force gauge) qua Serial Port (COM3, 9600 baud) và hiển thị biểu đồ real-time

**Solution:** `Navi_UI_WPF.sln`

---

## 2. Cấu trúc Solution

```
d:\Khang\Navi_UI_WPF\
├── Navi_UI_WPF\          → WPF App (main project - ViewModels/Views)
├── Navi.Infrastructure\  → Repositories (HTTP Clients), Hardware Integration
├── Navi.Shared\          → DTOs dùng chung (legacy - migrating to Navi.Application)
├── src\
│   ├── Navi.Core\        → Domain entities, interfaces, constants, enums
│   └── Navi.Application\ → DTOs, Service Interfaces, Business Logic
├── .rules\               → Architecture and coding rules (MANDATORY)
├── API.md                → Tài liệu REST API đầy đủ
└── CONTEXT.md            → File này
```

### Cấu trúc `Navi_UI_WPF/`

```
Navi_UI_WPF/
├── App.xaml / App.xaml.cs
├── MainWindow.xaml / .cs
├── ViewModels/           → Tất cả ViewModel
├── Views/                → Tất cả XAML View + code-behind
├── Commands/             → (legacy, đã migrate sang CommunityToolkit)
├── Converters/
├── Helpers/
├── Styles/               → ResourceDictionary styles toàn app
├── Utils/
└── packages.config
```

---

## 3. Tech Stack & NuGet Packages

| Package | Version | Dùng cho |
|---|---|---|
| `CommunityToolkit.Mvvm` | 8.2.2 | MVVM base (`ObservableObject`, `RelayCommand`) |
| `LiveCharts` + `LiveCharts.Wpf` | 0.9.7 | Biểu đồ real-time (ForceGauge) |
| `HandyControls` | 3.5.3 | UI controls bổ sung |
| `OpenTK` + `OpenTK.GLWpfControl` | 3.3.x | OpenGL (optional/future) |
| `Serilog` + `Sinks.File` | 2.10 / 5.0 | Logging |
| `SkiaSharp` | 3.119.2 | 2D graphics |
| `System.Memory`, `System.Buffers` | 4.x | Performance helpers |
| `Microsoft.Extensions.DependencyInjection` | 8.0.0 | DI Container / IoC |

> **Quan trọng:** .NET 4.7.2 → **không dùng Source Generator** của CommunityToolkit. Dùng `[ObservableProperty]` attribute sẽ không hoạt động. Phải viết property thủ công với `SetProperty(ref ...)`.

---

## 4. MVVM Pattern

### Base class
Tất cả ViewModel kế thừa `CommunityToolkit.Mvvm.ComponentModel.ObservableObject`.

### Property pattern (bắt buộc)
```csharp
private string _myProp;
public string MyProp
{
    get => _myProp;
    set => SetProperty(ref _myProp, value);
}
```

### Command pattern
```csharp
// Không tham số
SomeCommand = new RelayCommand(ExecuteSome);
SomeCommand = new RelayCommand(ExecuteSome, CanExecuteSome);

// Có tham số generic
EditCommand = new RelayCommand<MyDto>(item => DoEdit(item), _ => SelectedItem != null);

// Async
LoadCommand = new RelayCommand<int>(async id => await LoadAsync(id));
```

### Navigation (trong `MainViewModel`)
`CurrentView` là `object`, được bind với `ContentControl` + `DataTemplate` trong `MainWindow.xaml`.  
Mỗi khi navigate → tạo mới ViewModel instance → **data bị reset**.

---

## 5. ViewModels — Danh sách & Chức năng

### `MainViewModel`
- **File:** `ViewModels/MainViewModel.cs`
- Quản lý navigation toàn app qua `CurrentView` (object)
- Property `IsMenuOpen` (bool) — toggle sidebar menu
- Commands: `NavigateToHomeCommand`, `NavigateToAssemblyCommand`, `NavigateToProductCommand`, `NavigateToItemCommand`, `NavigateToProductItemCommand`, `NavigateToHistoryCommand`, `NavigateToForceGaugeCommand`, `ExitCommand`, `ToggleMenuCommand`

### `HomeViewModel`
- **File:** `ViewModels/HomeViewModel.cs`
- Đơn giản, chỉ là trang chủ

### `ProductAssemblyViewModel`
- **File:** `ViewModels/ProductAssemblyViewModel.cs`
- **Chức năng:** Hướng dẫn lắp ráp từng bước cho công nhân
- **Service:** `ProductAssemblyService` (trong `Navi.Infrastructure`)
- **Repository:** `ProductAssemblyRepository`
- **Key properties:**
  - `Steps` — `ObservableCollection<AssemblyStepViewModel>` — danh sách bước
  - `CurrentStep` / `CurrentStepIndex` — bước hiện tại
  - `StepsView` — `ICollectionView` với filter
  - `SearchText` — nếu là số → nhảy đến step đó; nếu là text → filter
  - `ProgressPercentage`, `TotalSteps`, `CompletedStepsCount`
  - `PoNumber`, `ProductId`, `ProductName`, `IsLoading`, `ErrorMessage`
- **Commands:** `ConfirmOkCommand`, `ConfirmNgCommand`, `JumpToStepCommand`, `LoadDataCommand`, `ToggleStepCompletionCommand`
- **Navigation:** Sử dụng nút OK/NG để tự động chuyển bước. Các bước chưa tới sẽ bị khóa (`IsLocked`).
- **Fallback:** Nếu API lỗi → load `LoadSampleData()` (6 bước mẫu lắp Rail đường sắt)
- **API call:** `GET /api/naviproducts/{id}/items`

### `AssemblyStepViewModel`
- **File:** `ViewModels/AssemblyStepViewModel.cs`
- Đại diện một bước lắp ráp
- Properties: `Id`, `StepNumber`, `Description`, `Note`, `Bolts`, `Force`, `Images`, `Type`, `IsCompleted`, `IsCurrent`

### `ForceGaugeViewModel`
- **File:** `ViewModels/ForceGaugeViewModel.cs`
- **Chức năng:** Đo lực qua Serial Port, hiển thị biểu đồ real-time
- **Serial Port:** COM3, 9600 baud, Parity.None, 8 data bits, 1 stop bit
- **Frame protocol:** ASCII hex text, 14 ký tự/frame
  - Header: `"4159"` (chars 0–3)
  - Mode (chars 4–5): `"02"` = dương (+), `"12"` = âm (-)
  - Padding: `"000000"` (chars 6–11)
  - Giá trị lực (chars 12–13): BCD hex, ví dụ `"13"` → 13g, `"48"` → 48g
- **Chart:** `SeriesCollection` (LiveCharts), `LineSeries`, max 100 điểm (`MaxPoints = 100`)
- **Khi mở port:** Xóa toàn bộ chart data + reset `_strBuffer` + reset `_pointIndex`
- **Key properties:** `IsConnected`, `ConnectionStatus`, `ConnectionButtonColor`, `SeriesCollection`, `XFormatter`
- **Info fields:** `PoNumber`, `ProductName`, `LotNumber`, `Quantity`
- **Block 1/2 fields:** `Block1SizeBall`, `Block1LucMin`, `Block1LucMax`, `Block2SizeBall`, `Block2LucMin`, `Block2LucMax`
- **Result fields:** `Result1`, `Result2`
- **Commands:** `ConnectCommand` (toggle open/close), `SaveCommand` (TODO)

### `SettingsViewModel`
- **File:** `ViewModels/SettingsViewModel.cs`
- **Chức năng:** Quản lý cấu hình hệ thống (theme, auto-save, interval).
- **Service:** `ISettingsService`.

### `NaviProductViewModel`
- **File:** `ViewModels/NaviProductViewModel.cs`
- **Chức năng:** Quản lý sản phẩm (CRUD)
- **Key properties:** `Products` (`ObservableCollection<NaviProductDto>`), `SelectedProduct`, `SelectedProductWithItems`, `SearchTerm`, `IsDetailVisible`, `IsEditMode`, `EditProductName`, `EditDescription`
- **Commands:** `SearchCommand`, `AddCommand`, `EditCommand`, `DeleteCommand`, `SelectProductCommand`, `ClearSelectionCommand`
- Add/Edit mở `NaviProductFormWindow` (dialog riêng)
- Hiện tại dùng sample data + TODO gọi API

### `NaviItemViewModel`
- **File:** `ViewModels/NaviItemViewModel.cs`
- **Chức năng:** Quản lý các bước lắp ráp (NaviItem) — CRUD

### `NaviProductItemViewModel`
- **File:** `ViewModels/NaviProductItemViewModel.cs`
- **Chức năng:** Quản lý quan hệ nhiều-nhiều Product ↔ Item

### `NaviHistoryViewModel`
- **File:** `ViewModels/NaviHistoryViewModel.cs`
- **Chức năng:** Xem/quản lý lịch sử thao tác nhân viên
- **Key properties:** `HistoryRecords` (`ObservableCollection<NaviHistoryDto>`), `SelectedRecord`, `AvailableTypes` (`SCAN`, `CHECK`, `CONFIRM`, `REWORK`, `REJECT`), filter fields (`FilterCodeNV`, `FilterPO`, `FilterProductItemId`), summary cards (`TotalCount`, `UniqueEmployeeCount`, `ActivePOCount`), `IsDetailVisible`, `IsEditMode`
- **Commands:** `LoadAllCommand`, `FilterByCodeNVCommand`, `FilterByPOCommand`, `FilterByProductItemCommand`, `ClearFilterCommand`, `AddCommand`, `EditCommand`, `DeleteCommand`, `SelectRecordCommand`, `SaveCommand`, `CancelEditCommand`
- Hiện tại dùng sample data + TODO gọi API

---

## 6. Views — Danh sách

| View | Tương ứng ViewModel |
|---|---|
| `HomeView.xaml` | `HomeViewModel` |
| `ProductAssemblyView.xaml` | `ProductAssemblyViewModel` |
| `ForceGaugeView.xaml` | `ForceGaugeViewModel` |
| `NaviProductView.xaml` | `NaviProductViewModel` |
| `NaviProductFormWindow.xaml` | Dialog cho add/edit product |
| `NaviItemView.xaml` | `NaviItemViewModel` |
| `NaviProductItemView.xaml` | `NaviProductItemViewModel` |
| `NaviHistoryView.xaml` | `NaviHistoryViewModel` |
| `TestUC.xaml` | Test UserControl |

---

## 7. Infrastructure Layer

### `Navi.Application`
- **DTOs:** Chứa các data models cho API communication.
- **Interfaces:** Định nghĩa service contracts.
- **Services:** Logic gọi API via `HttpClient` (pattern `SendAsync<T>`).

### `Navi.Core`
- **Entities:** Domain entities (Mirror backend database names).
- **Common/Constants:** Exception types, API Endpoints, Enums.

### `Navi.Infrastructure`
- `ProductAssemblyRepository` — Repository gọi API lấy dữ liệu lắp ráp
- `ProductAssemblyService` — Service wrapper dùng trong `ProductAssemblyViewModel`

### `Navi.Shared`
- DTOs dùng chung: `NaviProductDto`, `NaviItemDto`, `NaviProductWithItemsDto`, `NaviProductItemDto`, `NaviHistoryDto`

---

## 8. Backend API (tóm tắt)

API backend là **ASP.NET Core Web API** (Clean Architecture + CQRS + MediatR).  
Chi tiết đầy đủ xem [API.md](./API.md).

### Base URL
```
https://localhost:{port}/api
```

### Response format chung
```json
{ "statusCode": 200, "isSuccess": true, "message": "...", "data": {} }
```

### Các controller chính
| Controller | Prefix | Mô tả |
|---|---|---|
| `NaviProductsController` | `/api/naviproducts` | CRUD sản phẩm, import Excel |
| `NaviItemsController` | `/api/naviitems` | CRUD items/bước lắp ráp |
| `NaviProductItemsController` | `/api/naviproductitems` | Quan hệ product↔item |
| `NaviHistoryController` | `/api/navihistory` | Lịch sử thao tác nhân viên |

### Endpoints hay dùng nhất
```
GET  /api/naviproducts/{id}/items    → Product + danh sách items (dùng trong ProductAssembly)
GET  /api/navihistory/nv/{codeNV}    → Lịch sử theo mã NV
GET  /api/navihistory/po/{po}        → Lịch sử theo PO
POST /api/naviproducts/import-excel  → Import hàng loạt từ Excel
```

**Soft delete:** Tất cả entity dùng `IsDelete = true`, không xóa vật lý.

---

## 9. Các điểm đặc biệt / Gotchas

1. **.NET 4.7.2 → không dùng Source Generator** (`[ObservableProperty]`, `[RelayCommand]` sẽ không sinh code). Luôn viết property + command thủ công.

2. **Navigation reset data:** Mỗi navigate → tạo mới ViewModel instance → `ObservableCollection` rỗng. Nếu cần giữ state phải dùng DI/singleton hoặc truyền ViewModel cũ vào.

3. **ForceGauge — chart clear khi Connect:** `OpenPort()` luôn gọi `.Clear()` trên `ChartValues<double>` trước khi mở port. Đây là thiết kế có chủ ý.

4. **ForceGauge — Dispatcher:** Mọi cập nhật chart từ Serial Port event phải qua `Application.Current.Dispatcher.BeginInvoke(...)` vì chạy trên thread khác.

5. **Sample Data fallback:** `ProductAssemblyViewModel`, `NaviProductViewModel`, `NaviHistoryViewModel` đều có `LoadSampleData()` để chạy offline khi chưa có API.

6. **HandyControls:** Đang cài nhưng cần kiểm tra nếu dùng style/control nào từ thư viện này thì phải import ResourceDictionary tương ứng trong `App.xaml`.

7. **LiveCharts thread-safety:** `ChartValues<double>` phải được modify trên UI thread (dùng Dispatcher).

---

## 10. TODO / Chưa implement

- [ ] `ForceGaugeViewModel.Save()` — logic lưu kết quả đo
- [ ] Thực sự tích hợp `SettingsViewModel` vào UI (Menu/View)
- [ ] Chuyển đổi nốt các ViewModel cũ sang dùng DI hoàn toàn (constructor injection)
- [ ] `ForceGaugeViewModel` — COM port hiện hardcode `"COM3"`, cần cho phép user chọn

---

## 11. Coding Rules ([.rules/](./.rules/))

Dự án áp dụng bộ quy tắc nghiêm ngặt:
1. **00-Senior-Architect-Manifesto**: Quy tắc kiến trúc tổng thể.
2. **02-MVVM-Pattern**: Cấm Source Generator, bắt buộc manual boilerplate.
3. **04-Build-Testing**: Bắt buộc cập nhật `.csproj` thủ công khi thêm file mới.
4. **05-Data-Access**: Pattern Error handling và API communication.
