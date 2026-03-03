# Plan: Implement CRUD API Client Services

Dự án WPF hiện dùng **in-memory data** trong Service Layer. Mục tiêu là bổ sung đầy đủ **HTTP client services** để gọi tới 4 REST API endpoints mô tả trong `API.md`:

- `NaviProduct` — `/api/naviproducts`
- `NaviItem` — `/api/naviitems`
- `NaviProductItem` — `/api/naviproductitems`
- `NaviHistory` — `/api/navihistory`

Kiến trúc giữ nguyên **Clean Architecture**: `Navi.Core` → `Navi.Application` → `Navi.Infrastructure`.

---

## Tổng hợp File cần tạo

| File | Layer | Mô tả |
|---|---|---|
| `src/Navi.Core/Common/ApiResponse.cs` | Core | Generic response wrapper |
| `src/Navi.Core/Common/ApiSettings.cs` | Core | Base URL config |
| `src/Navi.Core/Interfaces/INaviProductService.cs` | Core | Interface service sản phẩm |
| `src/Navi.Core/Interfaces/INaviItemService.cs` | Core | Interface service item/bước |
| `src/Navi.Core/Interfaces/INaviProductItemService.cs` | Core | Interface service liên kết |
| `src/Navi.Core/Interfaces/INaviHistoryService.cs` | Core | Interface service lịch sử |
| `src/Navi.Application/DTOs/NaviProductDto.cs` | Application | DTOs cho NaviProduct |
| `src/Navi.Application/DTOs/NaviItemDto.cs` | Application | DTOs cho NaviItem |
| `src/Navi.Application/DTOs/NaviProductItemDto.cs` | Application | DTOs cho NaviProductItem |
| `src/Navi.Application/DTOs/NaviHistoryDto.cs` | Application | DTOs cho NaviHistory |
| `src/Navi.Application/Services/NaviProductService.cs` | Application | HTTP service sản phẩm |
| `src/Navi.Application/Services/NaviItemService.cs` | Application | HTTP service item/bước |
| `src/Navi.Application/Services/NaviProductItemService.cs` | Application | HTTP service liên kết |
| `src/Navi.Application/Services/NaviHistoryService.cs` | Application | HTTP service lịch sử |

**Tổng: 14 file mới + 1 file chỉnh sửa (DI setup trong `App.xaml.cs`).**

---

## Phase 1 — Shared Infrastructure

### `src/Navi.Core/Common/ApiResponse.cs`
```csharp
public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}
```

### `src/Navi.Core/Common/ApiSettings.cs`
```csharp
public class ApiSettings
{
    public string BaseUrl { get; set; } = "https://localhost:5001";
}
```

---

## Phase 2 — NaviProduct

### DTOs (`NaviProductDto.cs`)
- `NaviProductDto` — Id, ProductName, Description, CDT, UDT, IsDelete
- `NaviProductWithItemsDto` — kế thừa + `List<NaviItemDto> Items`
- `CreateNaviProductDto` — ProductName, Description
- `UpdateNaviProductDto` — ProductName, Description
- `CreateNaviProductWithItemsDto` — ProductName, Description, `List<int> ItemIds`
- `ImportResultDto` — TotalRows, InsertedRows, UpdatedRows, SkippedRows, FailedRows, Errors

### Interface (`INaviProductService.cs`)
```
GetAllAsync()                                     → Task<List<NaviProductDto>>
GetByIdAsync(int id)                              → Task<NaviProductDto>
GetWithItemsAsync(int id)                         → Task<NaviProductWithItemsDto>
SearchAsync(string term)                          → Task<List<NaviProductDto>>
CreateAsync(CreateNaviProductDto dto)             → Task<NaviProductDto>
UpdateAsync(int id, UpdateNaviProductDto dto)     → Task<NaviProductDto>
DeleteAsync(int id)                               → Task<bool>
CreateWithItemsAsync(CreateNaviProductWithItemsDto) → Task<NaviProductDto>
UpdateWithItemsAsync(int id, ...)                 → Task<NaviProductDto>
DeleteWithItemsAsync(int id)                      → Task<bool>
ImportExcelAsync(Stream fileStream, string fileName) → Task<ImportResultDto>
```

### Service (`NaviProductService.cs`)
- Inject `IHttpClientFactory`, tạo client với base URL
- Mỗi method: gọi đúng endpoint, deserialize `ApiResponse<T>`, throw exception nếu `!IsSuccess`

---

## Phase 3 — NaviItem

### DTOs (`NaviItemDto.cs`)
- `NaviItemDto` — Id, Description, Note, Bolts, Force, Images, Type, Step, CDT, UDT
- `NaviItemWithProductsDto` — kế thừa + `List<NaviProductDto> Products`
- `CreateNaviItemDto`, `UpdateNaviItemDto`

### Interface (`INaviItemService.cs`)
```
GetAllAsync()                           → Task<List<NaviItemDto>>
GetByIdAsync(int id)                    → Task<NaviItemDto>
GetWithProductsAsync(int id)            → Task<NaviItemWithProductsDto>
GetByTypeAsync(string type)             → Task<List<NaviItemDto>>
SearchAsync(string term)                → Task<List<NaviItemDto>>
CreateAsync(CreateNaviItemDto dto)      → Task<NaviItemDto>
UpdateAsync(int id, UpdateNaviItemDto)  → Task<NaviItemDto>
DeleteAsync(int id)                     → Task<bool>
```

---

## Phase 4 — NaviProductItem

### DTOs (`NaviProductItemDto.cs`)
- `NaviProductItemDto` — Id, ProductId, ItemId, IsDelete
- `CreateNaviProductItemDto` — ProductId, ItemId

### Interface (`INaviProductItemService.cs`)
```
GetAllAsync()                                   → Task<List<NaviProductItemDto>>
GetByIdAsync(int id)                            → Task<NaviProductItemDto>
GetByProductAsync(int productId)               → Task<List<NaviProductItemDto>>
GetByItemAsync(int itemId)                      → Task<List<NaviProductItemDto>>
ExistsAsync(int productId, int itemId)          → Task<bool>
CreateAsync(CreateNaviProductItemDto dto)       → Task<NaviProductItemDto>
DeleteAsync(int id)                             → Task<bool>
```

---

## Phase 5 — NaviHistory

### DTOs (`NaviHistoryDto.cs`)
- `NaviHistoryDto` — Id, NameNV, CodeNV, PO, Step, PRODUCT_ITEM_Id, Type, Count, CDT, UDT
- `CreateNaviHistoryDto`, `UpdateNaviHistoryDto`

### Interface (`INaviHistoryService.cs`)
```
GetAllAsync()                                   → Task<List<NaviHistoryDto>>
GetByIdAsync(int id)                            → Task<NaviHistoryDto>
GetByCodeNVAsync(string codeNV)                → Task<List<NaviHistoryDto>>
GetByProductItemAsync(int productItemId)        → Task<List<NaviHistoryDto>>
GetByPOAsync(string po)                         → Task<List<NaviHistoryDto>>
CreateAsync(CreateNaviHistoryDto dto)           → Task<NaviHistoryDto>
UpdateAsync(int id, UpdateNaviHistoryDto dto)   → Task<NaviHistoryDto>
DeleteAsync(int id)                             → Task<bool>
```

---

## Phase 6 — DI Registration

Trong `App.xaml.cs` hoặc Startup:
```csharp
services.Configure<ApiSettings>(config.GetSection("ApiSettings"));
services.AddHttpClient("NaviApi", (sp, c) =>
{
    var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
    c.BaseAddress = new Uri(settings.BaseUrl);
});
services.AddTransient<INaviProductService, NaviProductService>();
services.AddTransient<INaviItemService, NaviItemService>();
services.AddTransient<INaviProductItemService, NaviProductItemService>();
services.AddTransient<INaviHistoryService, NaviHistoryService>();
```

---

## Verification

```powershell
cd d:\Khang\Navi_UI_WPF
dotnet build Navi_UI_WPF.sln
```
Kỳ vọng: **Build succeeded — 0 errors**.
