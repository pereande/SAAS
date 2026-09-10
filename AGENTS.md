# ERP SaaS — Base44 Dev Environment

## Stack
- .NET 8 API (`ERP.Web`) with `dotnet watch` (hot reload) on port 3000
- PostgreSQL 15 (`erp_master` database) in compose
- Solution: `ERP.SaaS.sln` — projects: `ERP.Shared`, `ERP.Master`, `ERP.Tenant`, `ERP.Web`

## Running the app
```bash
docker compose -f docker-compose.base44.yml up -d --build
```
- DB container `erp-db` starts first (healthcheck gated)
- Web container `erp-web` runs `dotnet restore && dotnet watch run`
- EF Core migrations apply automatically on startup (Development only)
- Role seeder creates `Admin` (level 100), `TenantAdmin` (50), `User` (10)

## Architecture
- **MasterDbContext** (ERP.Master): global tables — tenants, users, roles, plans, audit
- **TenantDbContext** (ERP.Tenant): per-tenant tables — companies, products, sales, etc.
  - Resolved per-request via `TenantDbContextFactory` (reads connection string from the
    tenant middleware's resolved `TenantContext`)
  - Global query filter on `TenantId` for all `ITenantEntity` types
- **TenantCrudController<TEntity>**: generic CRUD base for all tenant-scoped entities
  - Provides GET (paginated + search), GET/{id}, POST, PUT/{id}, DELETE/{id}
  - 21 module controllers inherit from it (Companies, Branches, Products, Sales, etc.)

## Known limitations
- No seed data for tenants or admin user — login is not possible without manual seeding
- CORS in `appsettings.json` is hardcoded to `localhost` — needs preview origin for the panel
- `TenantDbContext` has no migrations yet — tenant databases must be created manually
- Rate limiting settings exist but no middleware enforces them
- Subdomain tenant resolution has a TODO (returns name without DB lookup)
