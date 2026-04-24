# CLAUDE.md — Technical Notes for Navi UI WPF

> Tài liệu kỹ thuật dành cho AI assistant. Đọc file này trước khi viết hoặc sửa code.
> Tham khảo thêm: `CONTEXT.md` (ngữ cảnh đầy đủ), `API.md` (tài liệu REST API), `.rules/` (quy tắc bắt buộc).

---

## Project Overview

**Navi UI WPF** là ứng dụng desktop WPF (.NET Framework 4.7.2) phục vụ dây chuyền sản xuất:
- Hướng dẫn lắp ráp từng bước cho công nhân (step-by-step assembly guide)
- Quản lý sản phẩm, items, quan hệ Product ↔ Item, lịch sử thao tác
- Đo lực (force gauge) qua Serial Port (COM3, 9600 baud) với biểu đồ real-time

**Solution:** `Navi_UI_WPF.sln`

---

## Architecture

```
d:\Khang\Navi_UI_WPF\
├── Navi_UI_WPF\          → WPF App chính (ViewModels, Views, Converters, Styles)
├── Navi.Infrastructure\  → Repositories (HTTP Clients), Hardware Integration
├── Navi.Shared\          → DTOs dùng chung (legacy)
├── src\
│   ├── Navi.Core\        → Domain entities, interfaces, constants, enums
│   └── Navi.Application\ → DTOs, Service Interfaces, Business Logic
├── .rules\               → Bộ quy tắc kiến trúc (BẮT BUỘC đọc)
├── API.md                → Tài liệu REST API đầy đủ
└── CONTEXT.md            → Ngữ cảnh dự án
```

### Quy tắc phân lớp
- **UI concerns** (ViewModel, View, Converter) → `Navi_UI_WPF/`
- **Data fetching, Services, API calls** → `Navi.Infrastructure/`
- **Domain models** → `Navi.Core/`
- **DTOs & interfaces** → `Navi.Application/` (hoặc `Navi.Shared/` nếu legacy)

---

## CRITICAL: .NET 4.7.2 — Source Generators KHÔNG hoạt động

**KHÔNG BAO GIỜ dùng:**
- `[ObservableProperty]` attribute
- `[RelayCommand]` attribute

**Luôn viết thủ công:**

```csharp
// Property
private string _myProp;
public string MyProp
{
    get => _myProp;
    set => SetProperty(ref _myProp, value);
}

// Command — không tham số
public IRelayCommand SaveCommand { get; }
// Trong constructor:
SaveCommand = new RelayCommand(ExecuteSave);
SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);

// Command — có tham số
public IRelayCommand<MyDto> EditCommand { get; }
// Trong constructor:
EditCommand = new RelayCommand<MyDto>(item => DoEdit(item), _ => SelectedItem != null);

// Async command
LoadCommand = new RelayCommand<int>(async id => await LoadAsync(id));
```

Tất cả ViewModel kế thừa `CommunityToolkit.Mvvm.ComponentModel.ObservableObject`.

---

## CRITICAL: Legacy .csproj — Phải thêm file thủ công

Vì đây là **Legacy .NET Framework project**, file `.cs` mới **KHÔNG tự động được include**.

**Mỗi khi tạo file `.cs` mới, phải thêm vào `.csproj` tương ứng:**
```xml
<Compile Include="ViewModels\MyNewViewModel.cs" />
```

Không làm bước này → file sẽ không được compile → build lỗi.

---

## CRITICAL: UI Threading

Mọi cập nhật lên `ObservableCollection` hoặc property được bind từ thread khác (Serial Port, `Task.Run`) **phải** qua Dispatcher:

```csharp
System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
{
    MyCollection.Add(item);
    MyProperty = value;
}));
```

---

## ViewModels — Danh sách chức năng

| ViewModel | File | Chức năng chính |
|---|---|---|
| `MainViewModel` | `ViewModels/MainViewModel.cs` | Navigation toàn app qua `CurrentView` (object), bind với `ContentControl` trong `MainWindow.xaml` |
| `HomeViewModel` | `ViewModels/HomeViewModel.cs` | Trang chủ đơn giản |
| `ProductAssemblyViewModel` | `ViewModels/ProductAssemblyViewModel.cs` | Hướng dẫn lắp ráp từng bước cho công nhân. Sử dụng OK/NG để chuyển bước. Khóa các bước tương lai. |
| `AssemblyStepViewModel` | `ViewModels/AssemblyStepViewModel.cs` | Đại diện một bước lắp ráp |
| `ForceGaugeViewModel` | `ViewModels/ForceGaugeViewModel.cs` | Đo lực Serial Port + biểu đồ real-time |
| `NaviProductViewModel` | `ViewModels/NaviProductViewModel.cs` | CRUD sản phẩm |
| `NaviItemViewModel` | `ViewModels/NaviItemViewModel.cs` | CRUD các bước lắp ráp (NaviItem) |
| `NaviProductItemViewModel` | `ViewModels/NaviProductItemViewModel.cs` | Quản lý quan hệ nhiều-nhiều Product ↔ Item |
| `NaviHistoryViewModel` | `ViewModels/NaviHistoryViewModel.cs` | Xem/quản lý lịch sử thao tác nhân viên |
| `SettingsViewModel` | `ViewModels/SettingsViewModel.cs` | Cấu hình hệ thống |

### ProductAssemblyViewModel — Chi tiết
- **Service:** `ProductAssemblyService` (trong `Navi.Infrastructure`)
- **Key properties:** `Steps` (ObservableCollection), `CurrentStep`, `CurrentStepIndex`, `StepsView` (ICollectionView với filter), `SearchText`, `ProgressPercentage`, `PoNumber`, `ProductId`, `ProductName`
- **SearchText logic:** Nếu là số → nhảy đến step đó; nếu là text → filter theo description
- **Navigation:** Không dùng nút Next/Previous. Dùng `ConfirmOkCommand` và `ConfirmNgCommand` để tự động tăng `CurrentStepIndex`.
- **Locking:** Bước có index > `CurrentStepIndex` sẽ có `IsLocked = true` và bị disabled trên UI.
- **Fallback:** Nếu API lỗi → `LoadSampleData()` (6 bước mẫu lắp Rail đường sắt)
- **API:** `GET /api/naviproducts/{id}/items`

### ForceGaugeViewModel — Chi tiết
- **Serial Port:** COM3, 9600 baud, Parity.None, 8 data bits, 1 stop bit (hardcoded)
- **Frame protocol:** ASCII hex, 14 ký tự/frame
  - Header: `"4159"` (chars 0–3)
  - Mode (chars 4–5): `"02"` = dương (+), `"12"` = âm (-)
  - Padding: `"000000"` (chars 6–11)
  - Lực (chars 12–13): BCD hex, ví dụ `"13"` → 13g
- **Chart:** `SeriesCollection` (LiveCharts), max 100 điểm (`MaxPoints = 100`)
- **Khi mở port:** Reset toàn bộ chart data + `_strBuffer` + `_pointIndex`

---

## Navigation Pattern

`MainViewModel.CurrentView` (object) được bind với `ContentControl` + `DataTemplate` trong `MainWindow.xaml`.

**Quan trọng:** Mỗi lần navigate → tạo mới ViewModel instance → `ObservableCollection` rỗng. Nếu cần giữ state → dùng DI Singleton hoặc truyền ViewModel cũ vào.

---

## Backend API — Tóm tắt

Backend là **ASP.NET Core Web API** (Clean Architecture + CQRS + MediatR).
Chi tiết đầy đủ xem [API.md](./API.md).

### Base URL
```
https://localhost:{port}/api
```

### Response format chung
```json
{ "statusCode": 200, "isSuccess": true, "message": "...", "data": {} }
```

### Controllers chính

| Controller | Prefix | Mô tả |
|---|---|---|
| `NaviProductsController` | `/api/naviproducts` | CRUD sản phẩm, import Excel |
| `NaviItemsController` | `/api/naviitems` | CRUD items/bước lắp ráp |
| `NaviProductItemsController` | `/api/naviproductitems` | Quan hệ nhiều-nhiều Product↔Item |
| `NaviHistoryController` | `/api/navihistory` | Lịch sử thao tác nhân viên |

### Endpoints hay dùng nhất
```
GET  /api/naviproducts/{id}/items             → Product + danh sách items (ProductAssembly)
GET  /api/navihistory/nv/{codeNV}             → Lịch sử theo mã nhân viên
GET  /api/navihistory/po/{po}                 → Lịch sử theo Production Order
POST /api/navihistory                         → Ghi lịch sử thao tác
POST /api/naviproducts/import-excel           → Import hàng loạt từ Excel
```

### Soft Delete
**Không bao giờ xóa vật lý.** Luôn đặt `IsDelete = true`. Dữ liệu đã xóa không được trả về trong query thông thường.

---

## Data Models chính

### NaviHistory — `LXA_NAVI_HISTORY`
| Field | Type | Mô tả |
|---|---|---|
| `Id` | int | Primary key |
| `NameNV` | string? | Tên nhân viên |
| `CodeNV` | string? | Mã nhân viên |
| `PO` | string? | Production Order |
| `Step` | string? | Bước sản xuất |
| `PRODUCT_ITEM_Id` | int? | FK → `LXA_NAVI_PRODUCT_ITEM` |
| `Type` | string? | Loại: `SCAN`, `CHECK`, `CONFIRM`, `REWORK`, `REJECT` |
| `Count` | int? | Số lượng |
| `CDT` / `UDT` | DateTime | Thời gian tạo/cập nhật |
| `IsDelete` | bool | Soft delete flag |

**JSON property naming:** Backend dùng `PRODUCT_ITEM_Id` (có underscore). Trong C# DTO phải dùng `[JsonProperty("PRODUCT_ITEM_Id")]` để mapping đúng.

---

## Common Gotchas

1. **Source Generator không hoạt động** trên .NET 4.7.2 → luôn viết property và command thủ công.

2. **Thêm file mới vào .csproj** thủ công — Legacy project không tự include file → build sẽ không thấy class mới.

3. **Navigation reset state** — Mỗi navigate tạo ViewModel mới → mất data. Cần DI Singleton nếu muốn giữ.

4. **ForceGauge chart clear khi Connect** — `OpenPort()` luôn gọi `.Clear()` trước khi mở port (có chủ ý).

5. **LiveCharts + Serial Port → Dispatcher** — Mọi update từ Serial event phải qua `Application.Current.Dispatcher.BeginInvoke(...)`.

6. **JSON casing API** — Backend trả về camelCase nhưng một số field dùng UPPER_SNAKE_CASE (như `PRODUCT_ITEM_Id`). Phải dùng `[JsonProperty]` attribute để map đúng.

7. **Sample data fallback** — `ProductAssemblyViewModel`, `NaviProductViewModel`, `NaviHistoryViewModel` đều có `LoadSampleData()` để chạy offline khi chưa có API.

8. **HandyControls ResourceDictionary** — Nếu dùng control/style của HandyControls phải import ResourceDictionary tương ứng trong `App.xaml`.

---

## Tech Stack

| Package | Version | Dùng cho |
|---|---|---|
| `CommunityToolkit.Mvvm` | 8.2.2 | MVVM (`ObservableObject`, `RelayCommand`) |
| `LiveCharts` + `LiveCharts.Wpf` | 0.9.7 | Biểu đồ real-time (ForceGauge) |
| `HandyControls` | 3.5.3 | UI controls bổ sung |
| `Serilog` + `Sinks.File` | 2.10 / 5.0 | Logging |
| `Microsoft.Extensions.DependencyInjection` | 8.0.0 | DI Container |

---

## TODO / Chưa implement

- [ ] `ForceGaugeViewModel.Save()` — logic lưu kết quả đo
- [ ] `SettingsViewModel` — tích hợp vào UI
- [ ] ForceGauge COM port: hiện hardcode `"COM3"`, cần cho user chọn
- [ ] Chuyển đổi các ViewModel cũ sang DI hoàn toàn (constructor injection)

---

## Data Flow — ProductAssembly

```
User chọn PO + ProductId
    ↓
ProductAssemblyViewModel.LoadDataCommand
    ↓
ProductAssemblyService.GetProductWithItemsAsync()
    ↓ GET /api/naviproducts/{id}/items
    ↓
Map items → ObservableCollection<AssemblyStepViewModel>
    ↓
Worker thao tác từng bước → ToggleStepCompletionCommand
    ↓
POST /api/navihistory (ghi lịch sử: codeNV, PO, step, type, productItemId)
```

## Data Flow — History Recording

```
Worker hoàn thành bước lắp ráp
    ↓
ProductAssemblyViewModel.RecordStepHistoryAsync()
    ↓
NaviHistoryDto { NameNV, CodeNV, PO, Step, PRODUCT_ITEM_Id, Type, Count }
    ↓
POST /api/navihistory
    ↓
Backend lưu vào LXA_NAVI_HISTORY (với IsDelete = false)
```




CLAUDE.md
Các hướng dẫn về hành vi nhằm giảm thiểu các lỗi lập trình LLM thường gặp. Kết hợp với các hướng dẫn cụ thể của dự án khi cần thiết.

Sự đánh đổi: Những hướng dẫn này ưu tiên sự thận trọng hơn tốc độ. Đối với những nhiệm vụ đơn giản, hãy sử dụng khả năng phán đoán của mình.

1. Hãy suy nghĩ kỹ trước khi lập trình
Đừng vội phán đoán. Đừng che giấu sự bối rối. Hãy công khai những sự đánh đổi.

Trước khi thực hiện:

Hãy nêu rõ các giả định của bạn. Nếu không chắc chắn, hãy hỏi.
Nếu có nhiều cách hiểu khác nhau, hãy trình bày chúng - đừng im lặng lựa chọn.
Nếu có cách tiếp cận đơn giản hơn, hãy nói ra. Hãy phản đối khi cần thiết.
Nếu có điều gì không rõ ràng, hãy dừng lại. Nêu rõ điều gì gây khó hiểu. Hỏi.
2. Ưu tiên sự đơn giản
Đoạn mã tối thiểu giải quyết được vấn đề. Không mang tính suy đoán.

Không có tính năng nào khác ngoài những gì đã được yêu cầu.
Không có sự trừu tượng nào cho mã chỉ sử dụng một lần.
Không có sự "linh hoạt" hay "khả năng cấu hình" nào mà không được yêu cầu.
Không có cơ chế xử lý lỗi cho các tình huống bất khả thi.
Nếu bạn viết 200 dòng mà có thể rút gọn xuống 50 dòng, hãy viết lại.
Hãy tự hỏi: "Một kỹ sư cấp cao có cho rằng điều này quá phức tạp không?" Nếu có, hãy đơn giản hóa.

3. Những thay đổi do phẫu thuật
Chỉ chạm vào những thứ cần thiết. Chỉ dọn dẹp mớ hỗn độn do chính mình gây ra.

Khi chỉnh sửa mã hiện có:

Không nên "cải thiện" mã, chú thích hoặc định dạng liền kề.
Đừng chỉnh sửa lại những thứ vốn dĩ không bị lỗi.
Hãy tuân theo phong cách hiện có, ngay cả khi bạn muốn làm theo cách khác.
Nếu bạn phát hiện thấy đoạn mã chết không liên quan, hãy đề cập đến nó - đừng xóa nó.
Khi các thay đổi của bạn tạo ra các đối tượng mồ côi:

Xóa bỏ các phần nhập khẩu/biến/hàm mà những thay đổi CỦA BẠN đã khiến chúng không được sử dụng.
Không nên xóa mã nguồn cũ đã lỗi thời trừ khi được yêu cầu.
Bài kiểm tra: Mỗi dòng mã được thay đổi phải liên kết trực tiếp với yêu cầu của người dùng.

4. Thực thi theo định hướng mục tiêu
Xác định tiêu chí thành công. Lặp lại cho đến khi được xác minh.

Chuyển đổi nhiệm vụ thành mục tiêu có thể kiểm chứng:

"Thêm kiểm tra hợp lệ" → "Viết các bài kiểm tra cho các đầu vào không hợp lệ, sau đó làm cho chúng vượt qua"
"Sửa lỗi" → "Viết một bài kiểm tra để tái hiện lỗi đó, sau đó làm cho bài kiểm tra đó vượt qua"
"Tái cấu trúc X" → "Đảm bảo các bài kiểm tra đều đạt trước và sau khi tái cấu trúc"
Đối với các nhiệm vụ nhiều bước, hãy nêu kế hoạch ngắn gọn:

1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
Tiêu chí thành công rõ ràng cho phép bạn thực hiện vòng lặp một cách độc lập. Tiêu chí yếu ("cứ làm cho nó hoạt động") đòi hỏi phải liên tục làm rõ.

Các hướng dẫn này sẽ hiệu quả nếu: có ít thay đổi không cần thiết trong các bản so sánh, ít phải viết lại do quá phức tạp và các câu hỏi làm rõ được đặt ra trước khi triển khai chứ không phải sau khi xảy ra lỗi.