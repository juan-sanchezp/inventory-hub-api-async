# InventoryHub API — Agent Guide

## Build & Run

```powershell
dotnet run --project InventoryHub
# or from publish folder:
.\publish\dotnet InventoryHub.dll  # requires MySQL running
```

## Architecture

- **Controller → Service → Repository → EF Core DbContext** (layered)
- All responses wrapped in `ApiResponse<T>` via `ResponseFactory.Success/Fail`
- AutoMapper profiles in `Mapping/` registered via `AddAutoMapper(typeof(Program))`
- `Program.cs` creates DB/ tables on startup via `db.Database.EnsureCreated()` (dev only)
- Orphan file: `Mapping/ProductMapper - Copy.cs` — old manual mapper, not in use. Do not reference.

## DB & Config

- MySQL 8.4.3 (Pomelo), connection string in `appsettings.json`
- `appsettings.json` is gitignored — use `appsettings.Development.json` as template
- All EF Core table names are lowercased via `OnModelCreating`

## Key Conventions

- Repository impl files: `*RepositoryImpl.cs`, service impl: `*ServiceImpl.cs`
- Async suffix on all async methods (`GetAllAsync`, `SaveAsync`, etc.)
- Product code normalization: `Trim().ToUpper()` in service layer
- Natural string ordering for product codes via `NaturalStringComparer`
- Image publicId format: `{ProductCode}_{yyMMdds}_{counter}`

## Domains

| Domain | Controller | Entities |
|--------|-----------|----------|
| Products | `api/products` | ProductEntity, CategoryEntity, LedStripDetailsEntity, TVModelEntity, ProductImageEntity |
| Customers | `api/customers` | CustomerEntity |
| Sales | `api/sale` | SaleEntity, SaleDetailEntity — includes cart/Draft flow and DIAN e-invoice fields |

### LedStrip details
- Product has optional `LedDetails` (LedStripDetailsEntity) for TV LED strip products
- Filters on `GET api/products/led-strips/search` via `LedStripFilterDTO`
- `LedType` enum: Normal=0, Cuadrado=1, SinLente=2, Mediano=3, OjoDeVaca=4

### Sale cart flow
1. `POST api/sale/cart/add` creates Sale with Status=Draft
2. Add more items, update quantities, remove items
3. `POST api/sale/{id}/checkout` changes Status → Pending/Completed

### Media
- Images stored in Cloudinary via `CloudinaryService` (config in `CloudinarySettings` section)
- Images tracked in `ProductImageEntity` with Url, PublicId, IsMain
- Endpoints: `POST/PUT/DELETE api/products/{id}/images`

### Excel Import/Export
- Uses both EPPlus (template download) and ExcelDataReader (import)
- EPPlus license set to `NonCommercial` in Program.cs — do not remove
- `POST api/products/import-excel` (full import), `POST api/products/import-StockExcel` (stock-only)
- `GET api/products/download-excel-template`
- Max request body: 100MB (configured in Program.cs)

## Auth (DISABLED)

- JWT auth code is **commented out** in `Program.cs` and `[Authorize]` on controllers
- `/login` endpoint exists with hardcoded creds `admin:1234` — not wired into auth pipeline
- Swagger has Bearer security scheme defined but unused

## Notable Gaps

- No test project exists
- No CI workflows
- No lint/format config
- `ReplaceProductImages` throws `NotImplementedException`
