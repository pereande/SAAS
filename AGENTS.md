# AGENTS.md — ERP SaaS

## Project Overview
Multi-tenant ERP SaaS platform built on .NET 8.0, PostgreSQL, and Redis.

## Architecture
- **ERP.Shared** — Interfaces, base models, settings, exceptions (no business logic)
- **ERP.Master** — Identity models (User, Role, UserClaim, UserRole, UserLogin, RoleClaim, UserToken), business models (Tenant, Subscription, Plan, Permission, RolePermission, AuditLog, LoginAttempt, UserSession), MasterDbContext, services (JwtService, TwoFactorService)
- **ERP.Tenant** — Tenant-specific models (Company, Branch, Product, Sale, Purchase, Person, Financial, Fiscal, Inventory), TenantDbContext
- **ERP.Web** — ASP.NET 8 Web API, controllers (Auth, Tenants), middleware (Audit, Authorization, Exception, TenantResolution, TenantValidation), Program.cs

## Key Decisions
- JwtService and TwoFactorService live in ERP.Master (not ERP.Shared) because they depend on ERP.Master.Models and Microsoft.AspNetCore.Identity
- ERP.Shared/Services/ contains only stub files pointing to ERP.Master
- Identity classes UserClaim, UserRole, UserLogin, RoleClaim are defined in Role.cs (not separate files)
- RolePermission is defined in Permission.cs
- Database initialized via EnsureCreated() in Development mode (no EF migrations)
- Connection string is in AppSettings:Database:MasterConnectionString (not ConnectionStrings:MasterConnection)

## Running in Base44
- `docker compose -f docker-compose.base44.yml up -d --build`
- Web API on port 3000, Swagger at /swagger
- PostgreSQL with POSTGRES_DB=erp_master
- App runs from source via `dotnet watch` (no image rebuild needed for edits)

## Dockerfile Note
- `docker/Dockerfile.web` restores only `src/ERP.Web/ERP.Web.csproj` (not the .sln) because the .sln references test projects whose csproj files may not be copied in the Docker build context
