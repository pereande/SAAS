# AGENTS.md — ERP SaaS

## Project Overview
Multi-tenant ERP SaaS platform built on .NET 8.0, PostgreSQL, and React (frontend em `frontend/`).

## Architecture
- **ERP.Shared** — Interfaces, base models, settings, exceptions (no business logic)
- **ERP.Master** — Identity models (User, Role, UserClaim, UserRole, UserLogin, RoleClaim, UserToken), business models (Tenant, Subscription, Plan, Permission, RolePermission, AuditLog, LoginAttempt, UserSession), MasterDbContext, services (JwtService, TwoFactorService)
- **ERP.Tenant** — Tenant-specific models (Company, Branch, Product, Sale, Purchase, Person, Financial, Fiscal, Inventory), TenantDbContext
- **ERP.Web** — ASP.NET 8 Web API, controllers (Auth, Tenants, Products, Persons, Sales, Purchases, Inventory), middleware (Audit, Authorization, Exception, TenantResolution, TenantValidation), Program.cs
- **frontend/** — SPA React 19 + Vite 7 + Tailwind 4 (páginas: Login, Dashboard, Tenants, Pessoas, Produtos, Estoque, Vendas, Compras)

## Key Decisions
- JwtService and TwoFactorService live in ERP.Master (not ERP.Shared) because they depend on ERP.Master.Models and Microsoft.AspNetCore.Identity
- ERP.Shared/Services/ contains only stub files pointing to ERP.Master
- Identity classes UserClaim, UserRole, UserLogin, RoleClaim are defined in Role.cs (not separate files)
- RolePermission is defined in Permission.cs
- Database initialized via EnsureCreated() in Development mode (no EF migrations), seguido por DbSeeder (idempotente)
- Connection string is in AppSettings:Database:MasterConnectionString (not ConnectionStrings:MasterConnection)
- **UserRole tem PK única (Id)** — `UserManager.AddToRoleAsync` QUEBRA (UserStore espera chave composta). Sempre vincule roles via `db.UserRoles.AddAsync(new UserRole { UserId, RoleId })` — ver DbSeeder.AddUserToRoleAsync e TenantsController.AddUserToRoleAsync
- **NotFound/ForbiddenException** já existem dentro de EntityNotFoundException.cs e UnauthorizedException.cs (não criar arquivos duplicados)
- Middlewares do pipeline: Exception → Authentication → TenantResolution → TenantValidation → Authorization → Audit (UseHttpsRedirection removido — proxy não termina TLS)
- TenantDbContext é scoped e resolvido por request a partir do tenant (header X-Tenant-Id ou claim tenant_id do JWT); controllers de módulo (Products/Sales/Purchases/Inventory/Persons) só funcionam para usuários com tenant
- Tenant DB criado em TenantDatabaseInitializer (CREATE DATABASE + EnsureCreated + seed de Company/Branch Matriz/pessoas/produtos demo)
- Confirmação de venda baixa estoque (InventoryMovement Exit); recebimento de compra dá entrada (Entry); cancelamento de venda confirmada devolve estoque (SalesReturn)
- Frontend conversa com a API pelo proxy do Vite (`/api` → http://web:80), mesma origem; token JWT em localStorage (`erp_token`)

## Contas de demonstração (senha: Admin@123)
- admin@erpsaas.com — Admin da plataforma (gerencia tenants)
- admin@demo.com.br — TenantAdmin da Empresa Demo LTDA (usa módulos)

## Running in Base44
- `docker compose -f docker-compose.base44.yml up -d`
- Frontend na porta 3000 (React+Vite, hot reload), API interna no service `web` (Swagger em http://localhost:8000/swagger)
- PostgreSQL com POSTGRES_DB=erp_master
- API roda do código via `dotnet watch`; frontend via `vite` (node_modules em volume nomeado)

## Dockerfile Note
- docker/Dockerfile.web restores only src/ERP.Web/ERP.Web.csproj (not the .sln) because the .sln references test projects whose csproj files may not be copied in the Docker build context
