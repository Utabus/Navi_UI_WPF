# Navi API

ASP.NET Core Web API quản lý dữ liệu sản phẩm, các bước lắp ráp (items) và lịch sử thao tác nhân viên trong quy trình sản xuất.

## Kiến trúc

Dự án được tổ chức theo mô hình **Clean Architecture** kết hợp **CQRS** (Command Query Responsibility Segregation) với **MediatR**:

```
Navi_API/          → ASP.NET Core Web API (Controllers, Program.cs)
Services/          → CQRS Handlers, Commands, Queries (Application Layer)
Entities/          → EF Core Entities, DbContext, Configurations
Repository/        → Repository implementations
Repository.Contracts/ → Repository interfaces
Services.Contracts/   → Service interfaces
Shared/            → DTOs, Response models (dùng chung)
LoggerService/     → Logging service
```

**Các thư viện chính:**
- `MediatR` — CQRS pipeline
- `AutoMapper` — Entity ↔ DTO mapping
- `Entity Framework Core` — ORM chính (database ERP)
- `Dapper` — Raw SQL queries (dùng cho cross-database như MANUFA)
- `ClosedXML` — Đọc file Excel (.xlsx)
- `Swagger / Swashbuckle` — API documentation

---

## Chuỗi kết nối (Connection String)

Cấu hình trong `appsettings.json` / `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=ERP;...",
    "ManufaConnection": "Server=...;Database=MANUFA;..."
  }
}
```

| Key | Database | Dùng bởi |
|-----|----------|----------|
| `DefaultConnection` | ERP | EF Core (NaviProduct, NaviItem, NaviHistory, v.v.) |
| `ManufaConnection` | MANUFA | Dapper (ManufaController — đọc DT_ASSIST, DT_REQ_HED) |

> Nếu `DefaultConnection` rỗng → tự động dùng **InMemory Database** (development mode). `ManufaConnection` bắt buộc phải có khi dùng ManufaController.

---

## Format Response chung

Tất cả API trả về cấu trúc `ApiResponse<T>` thống nhất:

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "message": "Thông báo",
  "data": { }
}
```

| Field        | Type    | Mô tả                                |
|--------------|---------|---------------------------------------|
| `statusCode` | int     | HTTP status code                      |
| `isSuccess`  | bool    | Kết quả thành công hay thất bại       |
| `message`    | string  | Thông báo tóm tắt                     |
| `data`       | object  | Dữ liệu trả về (null nếu không có)    |

---

## Data Models

### NaviItem — `LXA_NAVI_ITEM`

| Field         | Type      | Mô tả                              |
|---------------|-----------|------------------------------------|
| `Id`          | int       | Primary key                        |
| `Description` | string?   | Mô tả bước lắp ráp                 |
| `Note`        | string?   | Ghi chú                            |
| `Bolts`       | string?   | Thông tin bu lông                  |
| `Force`       | string?   | Thông tin lực siết                 |
| `Images`      | string?   | Danh sách hình ảnh (JSON/chuỗi)   |
| `Type`        | string?   | Loại item                          |
| `Step`        | int?      | Số thứ tự bước                     |
| `CDT`         | DateTime  | Thời gian tạo                      |
| `UDT`         | DateTime  | Thời gian cập nhật                 |
| `IsDelete`    | bool      | Soft delete flag                   |

### NaviItem — `LXA_NAVI_ITEM`

### NaviHistory — `LXA_NAVI_HISTORY`

Lưu lịch sử thao tác của nhân viên trên từng bước sản xuất.

| Field         | Type      | Mô tả                            |
|---------------|-----------|----------------------------------|
| `Id`          | int       | Primary key                      |
| `NameNV`      | string?   | Tên nhân viên                    |
| `CodeNV`      | string?   | Mã nhân viên                     |
| `PO`          | string?   | Production Order                 |
| `Step`        | int?      | Bước sản xuất                    |
| `ItemId`      | int?      | FK → `LXA_NAVI_ITEM`             |
| `ItemAuditId` | int?      | FK → `LXA_NAVI_ITEM_AUDIT`       |
| `ProductId`   | int?      | FK → `LXA_NAVI_PRODUCT`          |
| `Type`        | string?   | Loại hành động                   |
| `Count`       | int?      | Số lượng                         |
| `OK`          | bool?     | Trạng thái OK                    |
| `NG`          | bool?     | Trạng thái NG                    |
| `Note`        | string?   | Ghi chú                          |
| `CDT`         | DateTime  | Thời gian tạo                    |
| `UDT`         | DateTime  | Thời gian cập nhật               |
| `IsDelete`    | bool      | Soft delete flag                 |

### NaviItemAudit — `LXA_NAVI_ITEM_AUDIT`

Bảng lưu snapshot của `NaviItem` mỗi khi có thay đổi (Versioning/Audit Trail).

| Field         | Type      | Mô tả                              |
|---------------|-----------|------------------------------------|
| `Id`          | int       | Primary key                        |
| `ItemId`      | int       | FK → `LXA_NAVI_ITEM`               |
| `Version`     | int       | Số phiên bản (tăng dần)            |
| `Description` | string?   | Snapshot Mô tả bước lắp ráp        |
| `Note`        | string?   | Snapshot Ghi chú                   |
| `Bolts`       | string?   | Snapshot Thông tin bu lông         |
| `Force`       | string?   | Snapshot Thông tin lực siết        |
| `Images`      | string?   | Snapshot Danh sách hình ảnh        |
| `Type`        | string?   | Snapshot Loại item                 |
| `Step`        | int?      | Snapshot Số thứ tự bước            |
| `Grease`      | byte?     | Snapshot Trạng thái mỡ             |
| `ForceBit`    | byte?     | Snapshot Trạng thái lực            |
| `Timer`       | int?      | Snapshot Thời gian                 |
| `ChangedBy`   | string?   | Người thực hiện thay đổi           |
| `CDT`         | DateTime  | Thời gian tạo snapshot             |

> **Cơ chế tự động hóa (Audit Logic):**
> - **Khi tạo Item:** Hệ thống tự tạo Audit `Version 1` (Snapshot đầu tiên).
> - **Khi Add History:** Nếu Item cũ chưa có Audit, hệ thống tự khởi tạo `Version 1`. History luôn được gán vào bản audit mới nhất để làm mốc so sánh.
> - **Khi Update Item:** Hệ thống kiểm tra nếu chưa có audit thì tạo `Version 1` (cũ), sau đó tạo `Version N` (mới).

### NaviFg — `LXA_NAVI_FG`

Lưu dữ liệu FG (Finished Goods) theo từng PO và nhân viên thực hiện.

| Field      | SQL Type          | C# Type   | Mô tả                             |
|------------|-------------------|-----------|-----------------------------------|
| `Id`       | int IDENTITY      | int       | Primary key                       |
| `PO`       | nvarchar(50)      | string?   | Mã Purchase Order                 |
| `MinFG`    | float             | double?   | Giá trị FG tối thiểu              |
| `MaxFG`    | float             | double?   | Giá trị FG tối đa                 |
| `SizeBall` | float             | double?   | Kích thước bóng (số thực)         |
| `Data`     | nvarchar(max)     | string?   | Dữ liệu JSON/text                 |
| `Type`     | nvarchar(50)      | string?   | Loại                              |
| `CodeNV`   | nvarchar(50)      | string?   | Mã nhân viên                      |
| `CDT`      | datetime2(7)      | DateTime? | Thời gian tạo (default: now)      |
| `UDT`      | datetime2(7)      | DateTime? | Thời gian cập nhật (default: now) |
| `IsDelete` | bit               | bool      | Soft delete flag (default: 0)     |

### NaviProductMaster — `LXA_NAVI_PRODUCTMASTER`

| Field      | Type      | Mô tả                        |
|------------|-----------|------------------------------|
| `Id`       | int       | Primary key                  |
| `ProductP` | string?   | Mã Product P                 |
| `ProductName`| string? | Tên sản phẩm chính           |
| `ProductH` | string?   | Mã Product H                 |
| `Type`      | string?   | Loại                         |
| `CDT`      | DateTime  | Thời gian tạo                |
| `UDT`      | DateTime  | Thời gian cập nhật           |
| `IsDelete` | bool      | Soft delete flag             |

### NaviProductMasterItem — `LXA_NAVI_PRODUCTMASTER_ITEM`

Bảng liên kết nhiều-nhiều giữa `NaviProductMaster` và `NaviItem`.

| Field             | Type  | Mô tả                             |
|-------------------|-------|-----------------------------------|
| `Id`              | int   | Primary key                       |
| `ProductMasterId` | int   | FK → `LXA_NAVI_PRODUCTMASTER`     |
| `ItemId`          | int   | FK → `LXA_NAVI_ITEM`              |
| `CDT`             | DateTime | Thời gian tạo                  |
| `UDT`             | DateTime | Thời gian cập nhật             |
| `IsDelete`        | bool  | Soft delete flag (default: 0)     |

### ManufaAssist — `MANUFA.MANUFA_61.DT_ASSIST` ⨝ `DT_REQ_HED`

> **Read-only** — Không phải EF Core entity. Đây là view kết hợp 2 bảng từ database MANUFA qua Dapper raw SQL.

| Field      | SQL Table  | Mô tả                        |
|------------|------------|------------------------------|
| `AUFNR`    | DT_ASSIST  | Mã lệnh sản xuất (PO)        |
| `COMMENT1` | DT_ASSIST  | Ghi chú 1                    |
| `COMMENT2` | DT_ASSIST  | Ghi chú 2                    |
| `COMMENT3` | DT_ASSIST  | Ghi chú 3                    |
| `COMMENT4` | DT_ASSIST  | Ghi chú 4                    |
| `COMMENT5` | DT_ASSIST  | Ghi chú 5                    |
| `PHTX`     | DT_REQ_HED | Mô tả yêu cầu sản xuất       |
| `PHCD`     | DT_REQ_HED | Mã yêu cầu sản xuất          |

---

## API Endpoints

### 2. NaviItems — `/api/naviitems`

Quản lý các bước lắp ráp (NaviItem).

#### `GET /api/naviitems`
Lấy danh sách tất cả items chưa bị xóa.

**Response `200 OK`** — Danh sách `NaviItemDto[]`

---

#### `GET /api/naviitems/{id}`
Lấy một item theo Id.

**Response `200 OK`** — Item tìm thấy  
**Response `404 Not Found`**

---

#### `GET /api/naviitems/{id}/products`
Lấy item kèm danh sách sản phẩm liên quan.

**Response `200 OK`** — Item với danh sách products  
**Response `404 Not Found`**

---

#### `GET /api/naviitems/type/{type}`
Lấy các items theo loại (Type).

**Path params:** `type` (string)

**Response `200 OK`** — Danh sách items có `Type` tương ứng

---

---

#### `GET /api/naviitems/by-productmaster-name?productName={productName}`
Lấy danh sách các items thuộc về một `ProductMaster` dựa trên tên sản phẩm.

**Query params:** `productName` (string, bắt buộc)

**Response `200 OK`** — Danh sách items thuộc product master tương ứng

---

#### `GET /api/naviitems/with-history-status?productName={productName}&po={po}`
Lấy danh sách items kèm trạng thái lịch sử (đã làm hay chưa) dựa trên PO và ProductName.

**Query params:** `productName` (string, bắt buộc), `po` (string, bắt buộc)

**Response `200 OK`:**
```json
{
  "data": [
    {
      "id": 1,
      "description": "Bước 1",
      "step": 1,
      "status": "COMPLETED",
      "history": { ... }
    }
  ]
}
```

---

#### `GET /api/naviitems/search?term={term}`
Tìm kiếm items theo từ khóa.

**Query params:** `term` (string, bắt buộc)

**Response `200 OK`** — Danh sách items phù hợp  
**Response `400 Bad Request`** — `term` rỗng

---

#### `POST /api/naviitems`
Tạo item mới.

**Request body:**
```json
{
  "description": "Bước siết bu lông",
  "note": "Dùng cờ lê 13mm",
  "bolts": "M10x1.5",
  "force": "50Nm",
  "type": "Assembly",
  "step": 3
}
```

**Response `201 Created`** — Item vừa tạo  
**Response `400 Bad Request`**

---

#### `PUT /api/naviitems/{id}`
Cập nhật item.

**Path params:** `id` (int)  
**Request body:** Tương tự POST

**Response `200 OK`**  
**Response `400 Bad Request`**  
**Response `404 Not Found`**

---

#### `DELETE /api/naviitems/{id}`
Xóa mềm item.

**Response `200 OK`**  
**Response `404 Not Found`**

---

### 3. NaviItemAudit — `/api/naviitems` (Audit sub-routes)

Truy xuất lịch sử các phiên bản của NaviItem.

#### `GET /api/naviitems/{id}/audits`
Lấy toàn bộ danh sách các bản snapshot (audit trail) của một item.

**Path params:** `id` (int) — Item Id

**Response `200 OK`** — Danh sách `NaviItemAuditDto[]` sắp xếp theo Version giảm dần.

---

#### `GET /api/naviitems/audits/{auditId}`
Lấy thông tin chi tiết một bản snapshot audit cụ thể.

**Path params:** `auditId` (int)

**Response `200 OK`** — Thông tin snapshot  
**Response `404 Not Found`**

---

---

### 5. NaviHistory — `/api/navihistory`

---

### 5. NaviHistory — `/api/navihistory`

Quản lý lịch sử thao tác của nhân viên trong quá trình sản xuất.

#### `GET /api/navihistory`
Lấy tất cả history records chưa bị xóa.

**Response `200 OK`** — Danh sách `NaviHistoryDto[]`

---

#### `GET /api/navihistory/{id}`
Lấy một history record theo Id.

**Response `200 OK`**  
**Response `404 Not Found`**

---

#### `GET /api/navihistory/nv/{codeNV}`
Lấy lịch sử thao tác theo mã nhân viên.

**Path params:** `codeNV` (string) — Mã nhân viên

**Response `200 OK`** — Danh sách history của nhân viên đó

---

#### `GET /api/navihistory/productitem/{productItemId}`
Lấy lịch sử thao tác theo ProductItem Id.

**Path params:** `productItemId` (int)

**Response `200 OK`** — Danh sách history của ProductItem đó

---

#### `GET /api/navihistory/po/{po}`
Lấy lịch sử thao tác theo mã Production Order.

**Path params:** `po` (string)

**Response `200 OK`** — Danh sách history của PO đó

---

#### `GET /api/navihistory/item/{itemId}/audit-comparison`
Lấy toàn bộ history records của một Item kèm theo thông tin so sánh: Snapshot (tại thời điểm assembly) vs Trạng thái hiện tại của Item. 

- **Dùng để:** Phát hiện xem Item có bị sửa đổi thông số (Drift) sau khi nhân viên đã thực hiện xong bước đó hay không.
- **HasDrift:** Nếu các trường (`Force`, `Bolts`, `Description`, `Grease`, v.v.) trong snapshot khác với hiện tại, cờ hiệu `hasDrift` sẽ trả về `true`.

**Response `200 OK`:**
```json
{
  "data": [
    {
      "historyId": 42,
      "po": "PO-2026-001",
      "assemblyTime": "2026-04-20T10:30:00",
      "snapshotAtAssembly": { "force": "50N", "version": 1 },
      "currentItem": { "force": "70N" },
      "hasDrift": true
    }
  ]
}
```

---

#### `POST /api/navihistory`
Tạo history record mới (log một thao tác của nhân viên).

**Request body:**
```json
{
  "nameNV": "Nguyễn Văn A",
  "codeNV": "NV001",
  "po": "PO-2025-001",
  "step": "3",
  "productItemId": 12,
  "type": "SCAN",
  "count": 1
}
```

**Response `201 Created`**  
**Response `400 Bad Request`**

---

#### `PUT /api/navihistory/{id}`
Cập nhật history record.

**Path params:** `id` (int)  
**Request body:** Tương tự POST

**Response `200 OK`**  
**Response `400 Bad Request`**  
**Response `404 Not Found`**

---

#### `DELETE /api/navihistory/{id}`
Xóa mềm history record (soft delete).

**Response `200 OK`**  
**Response `404 Not Found`**

---

### 5. NaviFg — `/api/navifg`

Quản lý dữ liệu FG (Finished Goods) theo từng PO và nhân viên.

#### `GET /api/navifg`
Lấy danh sách tất cả NaviFg chưa bị xóa.

**Response `200 OK`:**
```json
{
  "statusCode": 200,
  "isSuccess": true,
  "message": "Lấy danh sách NaviFg thành công",
  "data": [
    {
      "id": 1,
      "po": "PO-2025-001",
      "minFG": 1.5,
      "maxFG": 2.8,
      "sizeBall": 10.5,
      "data": "{}",
      "type": "TypeA",
      "codeNV": "NV001",
      "cdt": "2025-03-10T09:00:00",
      "udt": "2025-03-10T09:00:00"
    }
  ]
}
```

---

#### `GET /api/navifg/{id}`
Lấy một NaviFg theo Id.

**Path params:** `id` (int)

**Response `200 OK`** — NaviFg tìm thấy  
**Response `404 Not Found`** — Không tồn tại NaviFg với Id này

---

#### `GET /api/navifg/by-po?po={po}`
Lọc danh sách NaviFg theo mã PO.

**Query params:** `po` (string, bắt buộc)

**Response `200 OK`** — Danh sách NaviFg thuộc PO đó  
**Response `400 Bad Request`** — `po` rỗng

---

#### `GET /api/navifg/by-codenv?codeNv={codeNv}`
Lọc danh sách NaviFg theo mã nhân viên.

**Query params:** `codeNv` (string, bắt buộc)

**Response `200 OK`** — Danh sách NaviFg của nhân viên đó  
**Response `400 Bad Request`** — `codeNv` rỗng

---

#### `POST /api/navifg`
Tạo NaviFg mới.

**Request body:**
```json
{
  "po": "PO-2025-001",
  "minFG": 1.5,
  "maxFG": 2.8,
  "sizeBall": 10.5,
  "data": "{\"key\": \"value\"}",
  "type": "TypeA",
  "codeNV": "NV001"
}
```

**Response `201 Created`** — NaviFg vừa tạo  
**Response `400 Bad Request`** — Body null hoặc không hợp lệ

---

#### `PUT /api/navifg/{id}`
Cập nhật NaviFg.

**Path params:** `id` (int)  
**Request body:** Tương tự POST

**Response `200 OK`** — Cập nhật thành công  
**Response `400 Bad Request`** — Body null  
**Response `404 Not Found`** — Không tìm thấy NaviFg

---

#### `DELETE /api/navifg/{id}`
Xóa mềm NaviFg (đặt `IsDelete = true`).

**Path params:** `id` (int)

**Response `200 OK`** — Xóa thành công  
**Response `404 Not Found`** — Không tìm thấy NaviFg

---

---

### 6. NaviProductMaster — `/api/naviproductmaster`

Quản lý danh mục sản phẩm chính (Master Product).

#### `GET /api/naviproductmaster`
Lấy danh sách tất cả product masters chưa bị xóa.

**Response `200 OK`** — Danh sách `NaviProductMasterDto[]`

---

#### `GET /api/naviproductmaster/{id}`
Lấy một product master theo Id.

**Response `200 OK`** — Tìm thấy  
**Response `404 Not Found`**

---

#### `GET /api/naviproductmaster/by-productp?productP={productP}`
Tìm kiếm product master theo mã ProductP.

---

#### `GET /api/naviproductmaster/by-producth?productH={productH}`
Tìm kiếm product master theo mã ProductH.

---

#### `GET /api/naviproductmaster/by-productname?productName={productName}`
Tìm kiếm chính xác product master theo ProductName.

---

#### `POST /api/naviproductmaster`
Tạo product master mới.

**Request body:**
```json
{
  "productP": "P001",
  "productName": "Sản phẩm A",
  "productH": "H001",
  "type": "Type1"
}
```

---

#### `PUT /api/naviproductmaster/{id}`
Cập nhật product master.

---

#### `DELETE /api/naviproductmaster/{id}`
Xóa mềm product master.

---

### 7. NaviProductMasterItems — `/api/naviproductmasteritems`

Quản lý mối quan hệ nhiều-nhiều giữa ProductMaster và Item.

#### `GET /api/naviproductmasteritems`
Lấy tất cả relationships.

**Response `200 OK`** — Danh sách `NaviProductMasterItemDto[]`

---

#### `GET /api/naviproductmasteritems/{id}`
Lấy một relationship theo Id.

**Response `200 OK`**  
**Response `404 Not Found`**

---

#### `GET /api/naviproductmasteritems/productmaster/{productMasterId}`
Lấy tất cả items thuộc một product master.

**Path params:** `productMasterId` (int)

**Response `200 OK`** — Danh sách ProductMasterItems của product master đó

---

#### `GET /api/naviproductmasteritems/item/{itemId}`
Lấy tất cả product masters chứa một item.

**Path params:** `itemId` (int)

**Response `200 OK`** — Danh sách ProductMasterItems của item đó

---

#### `POST /api/naviproductmasteritems`
Tạo relationship mới giữa một product master và một item.

**Request body:**
```json
{
  "productMasterId": 1,
  "itemId": 5
}
```

**Response `201 Created`** — Relationship vừa tạo  
**Response `400 Bad Request`** — ProductMaster/Item không tồn tại

---

#### `DELETE /api/naviproductmasteritems/{id}`
Xóa mềm một relationship.

**Response `200 OK`**  
**Response `404 Not Found`**

---

### 8. Manufa — `/api/manufa`

Truy vấn dữ liệu **read-only** từ database `MANUFA` (schema `MANUFA_61`). Dùng **Dapper raw SQL** thay vì EF Core vì khác database với ERP.

> **Yêu cầu:** `ManufaConnection` phải được cấu hình trong `appsettings.json`.

#### `GET /api/manufa/assist?po={po}`
Lấy thông tin Assist + Req Header từ MANUFA database theo mã AUFNR (PO).

**Query params:** `po` (string, bắt buộc) — Mã AUFNR / Purchase Order

**SQL thực thi:**
```sql
SELECT a.[AUFNR], [COMMENT1], [COMMENT2], [COMMENT3], [COMMENT4], [COMMENT5],
       h.PHTX, h.PHCD
FROM [MANUFA].[MANUFA_61].[DT_ASSIST] a
LEFT JOIN MANUFA.MANUFA_61.DT_REQ_HED h ON h.AUFNR = a.AUFNR
WHERE a.AUFNR = @PO
```

**Response `200 OK`:**
```json
{
  "statusCode": 200,
  "isSuccess": true,
  "message": "Lấy dữ liệu Assist thành công cho AUFNR: PO12345",
  "data": {
    "aufnr": "PO12345",
    "comment1": "Ghi chú 1",
    "comment2": "Ghi chú 2",
    "comment3": null,
    "comment4": null,
    "comment5": null,
    "phtx": "Mô tả yêu cầu sản xuất",
    "phcd": "REQ001"
  }
}
```

**Response `400 Bad Request`** — `po` rỗng  
**Response `404 Not Found`** — Không tìm thấy record với AUFNR này

---

## Tổng hợp Endpoints

| Controller             | Method | Route                                          | Mô tả                                       |
|------------------------|--------|------------------------------------------------|---------------------------------------------|
| **NaviItems**          | GET    | `/api/naviitems`                               | Lấy tất cả items                            |
|                        | GET    | `/api/naviitems/{id}`                          | Lấy item theo Id                            |
|                        | GET    | `/api/naviitems/{id}/products`                 | Lấy item kèm products                       |
|                        | GET    | `/api/naviitems/type/{type}`                   | Lấy items theo type                         |
|                        | GET    | `/api/naviitems/by-productmaster-name?productName=` | Lấy items theo tên ProductMaster        |
|                        | GET    | `/api/naviitems/with-history-status?productName=&po=` | Lấy items kèm trạng thái History        |
|                        | GET    | `/api/naviitems/search?term=`                  | Tìm kiếm items                              |
|                        | POST   | `/api/naviitems`                               | Tạo item mới                                |
|                        | PUT    | `/api/naviitems/{id}`                          | Cập nhật item (tạo snapshot audit)          |
|                        | DELETE | `/api/naviitems/{id}`                          | Xóa mềm item                                |
|                        | GET    | `/api/naviitems/{id}/audits`                   | Lấy toàn bộ audit snapshot của item         |
|                        | GET    | `/api/naviitems/audits/{auditId}`              | Lấy thông tin snapshot audit chi tiết       |
| **NaviHistory**        | GET    | `/api/navihistory`                             | Lấy tất cả history records                  |
|                        | GET    | `/api/navihistory/{id}`                        | Lấy history theo Id                         |
|                        | GET    | `/api/navihistory/nv/{codeNV}`                 | Lấy history theo mã NV                      |
|                        | GET    | `/api/navihistory/item/{itemId}`               | Lấy history theo Item Id (Snapshot-linked)  |
|                        | GET    | `/api/navihistory/po/{po}`                     | Lấy history theo Production Order           |
|                        | GET    | `/api/navihistory/item/{itemId}/audit-comparison` | So sánh Snapshot vs Hiện tại (Drift detection) |
|                        | POST   | `/api/navihistory`                             | Tạo history record mới                      |
|                        | PUT    | `/api/navihistory/{id}`                        | Cập nhật history record                     |
|                        | DELETE | `/api/navihistory/{id}`                        | Xóa mềm history record                      |
| **NaviFg**             | GET    | `/api/navifg`                                  | Lấy tất cả NaviFg                           |
|                        | GET    | `/api/navifg/{id}`                             | Lấy NaviFg theo Id                          |
|                        | GET    | `/api/navifg/by-po?po=`                        | Lọc theo Purchase Order                     |
|                        | GET    | `/api/navifg/by-codenv?codeNv=`                | Lọc theo mã nhân viên                       |
|                        | POST   | `/api/navifg`                                  | Tạo NaviFg mới                              |
|                        | PUT    | `/api/navifg/{id}`                             | Cập nhật NaviFg                             |
|                        | DELETE | `/api/navifg/{id}`                             | Xóa mềm NaviFg                              |
| **NaviProductMaster**  | GET    | `/api/naviproductmaster`                       | Lấy tất cả product masters                  |
|                        | GET    | `/api/naviproductmaster/{id}`                  | Lấy product master theo Id                  |
|                        | GET    | `/api/naviproductmaster/by-productp?productP=` | Tìm theo ProductP                           |
|                        | GET    | `/api/naviproductmaster/by-producth?productH=` | Tìm theo ProductH                           |
|                        | GET    | `/api/naviproductmaster/by-productname?productName=` | Tìm theo ProductName                  |
|                        | POST   | `/api/naviproductmaster`                       | Tạo product master mới                      |
|                        | PUT    | `/api/naviproductmaster/{id}`                  | Cập nhật product master                     |
|                        | DELETE | `/api/naviproductmaster/{id}`                  | Xóa mềm product master                      |
| **NaviProductMasterItems** | GET | `/api/naviproductmasteritems`                 | Lấy tất cả relationships                    |
|                        | GET    | `/api/naviproductmasteritems/{id}`             | Lấy relationship theo Id                    |
|                        | GET    | `/api/naviproductmasteritems/productmaster/{productMasterId}` | Lấy items theo productMasterId            |
|                        | GET    | `/api/naviproductmasteritems/item/{itemId}`    | Lấy product masters theo itemId             |
|                        | POST   | `/api/naviproductmasteritems`                  | Tạo relationship mới                        |
|                        | DELETE | `/api/naviproductmasteritems/{id}`             | Xóa mềm relationship                        |
| **Manufa** ⚡ Dapper   | GET    | `/api/manufa/assist?po=`                       | Lấy Assist + Req Header theo AUFNR (MANUFA DB) |

---

## Soft Delete

Tất cả entity đều hỗ trợ **soft delete** — thay vì xóa vật lý khỏi database, chỉ cần đặt `IsDelete = true`. Dữ liệu bị xóa sẽ không được trả về trong các câu query thông thường.

---

## Swagger UI

Sau khi chạy ứng dụng, truy cập Swagger tại:

```
https://localhost:{port}/swagger
```

Tất cả endpoints đều được document đầy đủ với request/response schema.