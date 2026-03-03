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
- `Entity Framework Core` — ORM
- `ClosedXML` — Đọc file Excel (.xlsx)
- `Swagger / Swashbuckle` — API documentation

---

## Chuỗi kết nối (Connection String)

Cấu hình trong `appsettings.json` / `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;..."
  }
}
```

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

### NaviProduct — `LXA_NAVI_PRODUCT`

| Field         | Type      | Mô tả                  |
|---------------|-----------|------------------------|
| `Id`          | int       | Primary key            |
| `ProductName` | string?   | Tên sản phẩm           |
| `Description` | string?   | Mô tả sản phẩm         |
| `CDT`         | DateTime  | Thời gian tạo          |
| `UDT`         | DateTime  | Thời gian cập nhật     |
| `IsDelete`    | bool      | Soft delete flag       |

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

### NaviProductItem — `LXA_NAVI_PRODUCT_ITEM`

Bảng liên kết nhiều-nhiều giữa `NaviProduct` và `NaviItem`.

| Field       | Type  | Mô tả                        |
|-------------|-------|-------------------------------|
| `Id`        | int   | Primary key                   |
| `ProductId` | int   | FK → `LXA_NAVI_PRODUCT`      |
| `ItemId`    | int   | FK → `LXA_NAVI_ITEM`         |
| `IsDelete`  | bool  | Soft delete flag              |

### NaviHistory — `LXA_NAVI_HISTORY`

Lưu lịch sử thao tác của nhân viên trên từng bước sản xuất.

| Field             | Type      | Mô tả                            |
|-------------------|-----------|----------------------------------|
| `Id`              | int       | Primary key                      |
| `NameNV`          | string?   | Tên nhân viên                    |
| `CodeNV`          | string?   | Mã nhân viên                     |
| `PO`              | string?   | Production Order                 |
| `Step`            | string?   | Bước sản xuất                    |
| `PRODUCT_ITEM_Id` | int?      | FK → `LXA_NAVI_PRODUCT_ITEM`     |
| `Type`            | string?   | Loại hành động                   |
| `Count`           | int?      | Số lượng                         |
| `CDT`             | DateTime  | Thời gian tạo                    |
| `UDT`             | DateTime  | Thời gian cập nhật               |
| `IsDelete`        | bool      | Soft delete flag                 |

---

## API Endpoints

### 1. NaviProducts — `/api/naviproducts`

Quản lý sản phẩm (NaviProduct).

#### `GET /api/naviproducts`
Lấy danh sách tất cả sản phẩm chưa bị xóa.

**Response `200 OK`:**
```json
{
  "statusCode": 200,
  "isSuccess": true,
  "message": "Lấy danh sách products thành công",
  "data": [
    { "id": 1, "productName": "SP001", "description": "Mô tả sản phẩm" }
  ]
}
```

---

#### `GET /api/naviproducts/{id}`
Lấy một sản phẩm theo Id.

**Path params:** `id` (int)

**Response `200 OK`** — Product tìm thấy  
**Response `404 Not Found`** — Không tồn tại product với Id này

---

#### `GET /api/naviproducts/{id}/items`
Lấy sản phẩm kèm danh sách các items (bước lắp ráp) liên quan.

**Path params:** `id` (int)

**Response `200 OK`:**
```json
{
  "data": {
    "id": 1,
    "productName": "SP001",
    "items": [
      { "id": 10, "description": "Bước 1", "step": 1 }
    ]
  }
}
```
**Response `404 Not Found`** — Không tồn tại product với Id này

---

#### `GET /api/naviproducts/search?term={term}`
Tìm kiếm sản phẩm theo từ khóa (tên hoặc mô tả).

**Query params:** `term` (string, bắt buộc)

**Response `200 OK`** — Danh sách products phù hợp  
**Response `400 Bad Request`** — `term` rỗng

---

#### `POST /api/naviproducts`
Tạo sản phẩm mới.

**Request body:**
```json
{
  "productName": "SP002",
  "description": "Mô tả sản phẩm mới"
}
```

**Response `201 Created`** — Product vừa tạo  
**Response `400 Bad Request`** — Body null hoặc không hợp lệ

---

#### `PUT /api/naviproducts/{id}`
Cập nhật thông tin sản phẩm.

**Path params:** `id` (int)  
**Request body:**
```json
{
  "productName": "SP002 Updated",
  "description": "Mô tả mới"
}
```

**Response `200 OK`** — Thành công  
**Response `400 Bad Request`** — Body null  
**Response `404 Not Found`** — Không tìm thấy product

---

#### `DELETE /api/naviproducts/{id}`
Xóa mềm sản phẩm (đặt `IsDelete = true`).

**Path params:** `id` (int)

**Response `200 OK`** — Xóa thành công  
**Response `404 Not Found`** — Không tìm thấy product

---

#### `POST /api/naviproducts/with-items`
Tạo sản phẩm kèm danh sách items trong **một transaction**.

**Request body:**
```json
{
  "productName": "SP003",
  "description": "Mô tả",
  "itemIds": [1, 2, 3]
}
```

**Response `201 Created`** — Product với items vừa tạo  
**Response `400 Bad Request`** — Item không tồn tại hoặc body null

---

#### `PUT /api/naviproducts/{id}/with-items`
Cập nhật sản phẩm và quản lý danh sách items trong **một transaction**.

**Path params:** `id` (int)  
**Request body:**
```json
{
  "productName": "SP003 Updated",
  "description": "Mô tả mới",
  "itemIds": [1, 4]
}
```

**Response `200 OK`** — Product với items sau cập nhật  
**Response `400 Bad Request`** — Body null  
**Response `404 Not Found`** — Không tìm thấy product

---

#### `DELETE /api/naviproducts/{id}/with-items`
Xóa mềm sản phẩm và **toàn bộ** ProductItems liên quan.

**Path params:** `id` (int)

**Response `200 OK`** — Xóa thành công  
**Response `404 Not Found`** — Không tìm thấy product

---

#### `POST /api/naviproducts/import-excel`
Import hàng loạt dữ liệu từ file Excel (`.xlsx`) vào `NaviProduct`, `NaviItem`, `NaviProductItem`.

**Content-Type:** `multipart/form-data`  
**Form field:** `file` — File `.xlsx`, tối đa **10 MB**

**Cấu trúc file Excel (có header row):**

| ProductName | ProductDescription | ItemDescription | ItemNote | ItemBolts | ItemForce | ItemType | Step |
|-------------|-------------------|-----------------|----------|-----------|-----------|----------|------|
| SP001       | Mô tả SP          | Bước lắp bu lông | Chú ý  | M10x1.5   | 50Nm      | Assembly | 1    |

**Logic xử lý:**
- `ProductName` **chưa tồn tại** → tạo mới Product + Item + ProductItem
- `ProductName` **đã tồn tại** + Item **đã được liên kết** → **UPDATE** Item đó
- `ProductName` **đã tồn tại** + Item **chưa liên kết** → tạo mới Item + ProductItem

**Response `200 OK`:**
```json
{
  "data": {
    "totalRows": 100,
    "insertedRows": 80,
    "updatedRows": 15,
    "skippedRows": 3,
    "failedRows": 2,
    "errors": [
      { "rowNumber": 45, "productName": "SP999", "reason": "Lỗi database" }
    ]
  }
}
```

**Response `400 Bad Request`** — File null, không phải `.xlsx`, hoặc vượt quá 10 MB

---

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

### 3. NaviProductItems — `/api/naviproductitems`

Quản lý mối quan hệ nhiều-nhiều giữa Product và Item.

#### `GET /api/naviproductitems`
Lấy tất cả relationships.

**Response `200 OK`** — Danh sách `NaviProductItemDto[]`

---

#### `GET /api/naviproductitems/{id}`
Lấy một relationship theo Id.

**Response `200 OK`**  
**Response `404 Not Found`**

---

#### `GET /api/naviproductitems/product/{productId}`
Lấy tất cả items thuộc một product.

**Path params:** `productId` (int)

**Response `200 OK`** — Danh sách ProductItems của product đó

---

#### `GET /api/naviproductitems/item/{itemId}`
Lấy tất cả products chứa một item.

**Path params:** `itemId` (int)

**Response `200 OK`** — Danh sách ProductItems của item đó

---

#### `GET /api/naviproductitems/exists?productId={productId}&itemId={itemId}`
Kiểm tra xem một relationship có tồn tại không.

**Query params:** `productId` (int), `itemId` (int)

**Response `200 OK`:**
```json
{ "data": true }
```

---

#### `POST /api/naviproductitems`
Tạo relationship mới giữa một product và một item.

**Request body:**
```json
{
  "productId": 1,
  "itemId": 5
}
```

**Response `201 Created`** — Relationship vừa tạo  
**Response `400 Bad Request`** — Product/Item không tồn tại

---

#### `DELETE /api/naviproductitems/{id}`
Xóa mềm một relationship.

**Response `200 OK`**  
**Response `404 Not Found`**

---

### 4. NaviHistory — `/api/navihistory`

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

## Tổng hợp Endpoints

| Controller             | Method | Route                                          | Mô tả                                       |
|------------------------|--------|------------------------------------------------|---------------------------------------------|
| **NaviProducts**       | GET    | `/api/naviproducts`                            | Lấy tất cả products                         |
|                        | GET    | `/api/naviproducts/{id}`                       | Lấy product theo Id                         |
|                        | GET    | `/api/naviproducts/{id}/items`                 | Lấy product kèm items                       |
|                        | GET    | `/api/naviproducts/search?term=`               | Tìm kiếm products                           |
|                        | POST   | `/api/naviproducts`                            | Tạo product mới                             |
|                        | PUT    | `/api/naviproducts/{id}`                       | Cập nhật product                            |
|                        | DELETE | `/api/naviproducts/{id}`                       | Xóa mềm product                             |
|                        | POST   | `/api/naviproducts/with-items`                 | Tạo product kèm items (transaction)         |
|                        | PUT    | `/api/naviproducts/{id}/with-items`            | Cập nhật product + items (transaction)      |
|                        | DELETE | `/api/naviproducts/{id}/with-items`            | Xóa product + ProductItems (transaction)    |
|                        | POST   | `/api/naviproducts/import-excel`               | Import từ file Excel                        |
| **NaviItems**          | GET    | `/api/naviitems`                               | Lấy tất cả items                            |
|                        | GET    | `/api/naviitems/{id}`                          | Lấy item theo Id                            |
|                        | GET    | `/api/naviitems/{id}/products`                 | Lấy item kèm products                       |
|                        | GET    | `/api/naviitems/type/{type}`                   | Lấy items theo type                         |
|                        | GET    | `/api/naviitems/search?term=`                  | Tìm kiếm items                              |
|                        | POST   | `/api/naviitems`                               | Tạo item mới                                |
|                        | PUT    | `/api/naviitems/{id}`                          | Cập nhật item                               |
|                        | DELETE | `/api/naviitems/{id}`                          | Xóa mềm item                                |
| **NaviProductItems**   | GET    | `/api/naviproductitems`                        | Lấy tất cả relationships                    |
|                        | GET    | `/api/naviproductitems/{id}`                   | Lấy relationship theo Id                    |
|                        | GET    | `/api/naviproductitems/product/{productId}`    | Lấy items theo productId                    |
|                        | GET    | `/api/naviproductitems/item/{itemId}`          | Lấy products theo itemId                    |
|                        | GET    | `/api/naviproductitems/exists?productId=&itemId=` | Kiểm tra relationship tồn tại           |
|                        | POST   | `/api/naviproductitems`                        | Tạo relationship mới                        |
|                        | DELETE | `/api/naviproductitems/{id}`                   | Xóa mềm relationship                        |
| **NaviHistory**        | GET    | `/api/navihistory`                             | Lấy tất cả history records                  |
|                        | GET    | `/api/navihistory/{id}`                        | Lấy history theo Id                         |
|                        | GET    | `/api/navihistory/nv/{codeNV}`                 | Lấy history theo mã NV                      |
|                        | GET    | `/api/navihistory/productitem/{productItemId}` | Lấy history theo ProductItem                |
|                        | GET    | `/api/navihistory/po/{po}`                     | Lấy history theo Production Order           |
|                        | POST   | `/api/navihistory`                             | Tạo history record mới                      |
|                        | PUT    | `/api/navihistory/{id}`                        | Cập nhật history record                     |
|                        | DELETE | `/api/navihistory/{id}`                        | Xóa mềm history record                      |

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