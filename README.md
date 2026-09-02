# ERP SaaS - Sistema de Gestão Empresarial Multiempresa

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-purple?logo=dotnet" alt=".NET 8.0" />
  <img src="https://img.shields.io/badge/PostgreSQL-15-blue?logo=postgresql" alt="PostgreSQL" />
  <img src="https://img.shields.io/badge/Docker-2496ED?logo=docker" alt="Docker" />
  <img src="https://img.shields.io/badge/JWT-FF6B6B?logo=jsonwebtokens" alt="JWT" />
  <img src="https://img.shields.io/badge/Multi%20Tenant-4ECDC4" alt="Multi Tenant" />
</p>

<p align="center">
  <strong>Sistema ERP SaaS completo com suporte a multi-tenancy, autenticação JWT, 2FA e todos os módulos empresariais essenciais.</strong>
</p>

---

## 📋 **Índice**

- [🎯 Objetivo](#-objetivo)
- [✨ Características](#-características)
- [🏗️ Arquitetura](#️-arquitetura)
- [📦 Tecnologias](#-tecnologias)
- [🚀 Instalação](#-instalação)
- [⚙️ Configuração](#️-configuração)
- [🔐 Autenticação](#-autenticação)
- [🏢 Multi-Tenancy](#-multi-tenancy)
- [📂 Estrutura do Projeto](#-estrutura-do-projeto)
- [🎪 Endpoints da API](#-endpoints-da-api)
- [📊 Módulos Disponíveis](#-módulos-disponíveis)
- [🔧 Desenvolvimento](#-desenvolvimento)
- [🐳 Docker](#-docker)
- [📄 Licença](#-licença)
- [🤝 Contribuição](#-contribuição)

---

## 🎯 **Objetivo**

O **ERP SaaS** é um **Sistema de Gestão Empresarial** desenvolvido como uma solução **Software as a Service (SaaS)** com arquitetura **multi-tenancy**, projetado para atender empresas de todos os portes com uma plataforma **escalável, segura e modular**.

### **Problema que resolve:**
- 🏢 **Multi-empresas**: Gerencie múltiplas empresas em um único sistema com **isolamento total de dados**
- 💰 **Custo reduzido**: Modelos de assinatura flexíveis (Basic, Professional, Enterprise)
- 🔒 **Segurança**: Autenticação robusta com JWT, 2FA e controle de acesso granular
- 📱 **Acessibilidade**: API RESTful para integração com qualquer frontend (Web, Mobile, Desktop)
- 🌐 **Escalabilidade**: Arquitetura preparada para crescimento horizontal

### **Público Alvo:**
- **Startups e PMEs** que precisam de um ERP completo sem alto investimento inicial
- **Grandes empresas** que buscam uma solução customizável e escalável
- **Desenvolvedores** que querem uma base sólida para construir soluções personalizadas
- **Provedores de SaaS** que desejam oferecer ERP como serviço para seus clientes

---

## ✨ **Características**

### **🔐 Segurança**
- ✅ Autenticação **JWT** com **Refresh Token**
- ✅ **2FA (Two-Factor Authentication)** usando TOTP (Google Authenticator)
- ✅ **Códigos de recuperação** para 2FA
- ✅ **RBAC** (Role-Based Access Control) com permissões granulares
- ✅ **Bloqueio de conta** após tentativas falhas
- ✅ **Senhas fortes** com validação
- ✅ **CORS** configurável
- ✅ **Rate Limiting** para prevenir abusos

### **🏢 Multi-Tenancy**
- ✅ **Database Per Tenant**: Cada empresa tem seu próprio banco de dados
- ✅ **Isolamento total de dados**: Nenhum tenant acessa dados de outro
- ✅ **Resolução de tenant** via Header, Subdomínio ou Token JWT
- ✅ **Validação automática** de tenant ativo
- ✅ **Filtros globais** para garantir isolamento

### **📊 Módulos Empresariais**

#### **🏠 Cadastros**
- Empresas (Company)
- Filiais (Branch)
- Pessoas (Person) - Base para clientes, fornecedores e funcionários
- Clientes (Client)
- Fornecedores (Supplier)
- Funcionários (Employee)

#### **📦 Estoque**
- Produtos (Product) - com categorias, marcas, NCM, CST, etc.
- Estoque (Inventory) - por filial
- Movimentações de Estoque (InventoryMovement)
- Controle de estoque mínimo/máximo

#### **💰 Vendas**
- Pedidos de Venda (Sale)
- Itens de Venda (SaleItem)
- Orçamentos e Cotações
- Controle de status (Rascunho, Confirmada, Entregue, etc.)

#### **🛒 Compras**
- Pedidos de Compra (Purchase)
- Itens de Compra (PurchaseItem)
- Recebimento de mercadorias
- Controle de status

#### **💳 Financeiro**
- Contas a Pagar (AccountPayable)
- Contas a Receber (AccountReceivable)
- Lançamentos Financeiros (FinancialEntry)
- Bancos e Contas Bancárias (Bank, BankAccount)
- Formas de Pagamento (PaymentMethod)
- Condições de Pagamento (PaymentTerm)
- Centros de Custo (CostCenter)

#### **📑 Fiscal**
- Notas Fiscais (FiscalNote) - Entrada e Saída
- Itens de Nota Fiscal (FiscalNoteItem)
- CFOP, NCM, CST, CSOSN
- Configurações Fiscais (FiscalConfiguration)
- Cálculo automático de impostos (ICMS, IPI, PIS, COFINS)

#### **📈 Relatórios & Dashboard**
- Audit Log - Registro de todas as ações
- Login Attempts - Tentativas de acesso
- User Sessions - Sessões ativas

### **🔧 Infraestrutura**
- ✅ **Docker Compose** para ambiente de desenvolvimento
- ✅ **PostgreSQL** como banco de dados
- ✅ **Redis** para cache
- ✅ **Seq** para logging centralizado
- ✅ **PGAdmin** para gerenciamento do banco
- ✅ **Swagger** para documentação da API

---

## 🏗️ **Arquitetura**

```
┌─────────────────────────────────────────────────────────────────────┐
│                         ERP SaaS System                                 │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────────────────────┐  │
│  │   Client    │    │   Client    │    │        API Gateway            │  │
│  │  (Web App)  │───▶│  (Mobile)   │───▶│   (ERP.Web)                   │  │
│  └─────────────┘    └─────────────┘    └──────────────┬──────────────┘  │
│                                                      │                  │
│                                                      ▼                  │
│  ┌─────────────────────────────────────────────────────────────────┐ │
│  │                    Middleware Layer                              │ │
│  │  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐ ┌─────────┐  │ │
│  │  │  Exception   │ │  Tenant      │ │ Authorization │ │  Audit  │  │ │
│  │  │  Handler     │ │  Resolution  │ │  Middleware   │ │  Log    │  │ │
│  │  └──────────────┘ └──────────────┘ └──────────────┘ └─────────┘  │ │
│  └─────────────────────────────────────────────────────────────────┘ │
│                                                      │                  │
│                      ┌───────────────────────┬───────────────────────┐  │
│                      ▼                       ▼                       ▼  │
│              ┌─────────────┐       ┌─────────────┐       ┌─────────────┐ │
│              │ ERP_MASTER  │       │ERP_TENANT_1 │       │ERP_TENANT_N │ │
│              │  Database   │       │  Database   │       │  Database   │ │
│              │ (PostgreSQL)│       │ (PostgreSQL)│       │ (PostgreSQL)│ │
│              └─────────────┘       └─────────────┘       └─────────────┘ │
│                              │               │               │           │
│                              └───────────────┴───────────────┘           │
│                                          │                               │
│                                          ▼                               │
│                              ┌─────────────────────┐                      │
│                              │     External         │                      │
│                              │     Services        │                      │
│                              │  - Redis (Cache)    │                      │
│                              │  - Seq (Logging)    │                      │
│                              └─────────────────────┘                      │
└─────────────────────────────────────────────────────────────────────┘
```

### **Fluxo de Requisição**

```
1. Requisição HTTP chega à API
2. ExceptionMiddleware → Trata erros globais
3. TenantResolutionMiddleware → Identifica o tenant (Header/Subdomínio/Token)
4. TenantValidationMiddleware → Valida se tenant existe e está ativo
5. AuthorizationMiddleware → Verifica permissões (RBAC)
6. AuditMiddleware → Loga a requisição
7. Controller → Processa a requisição
8. Service → Lógica de negócios
9. Repository → Acesso ao banco de dados (Master ou Tenant)
10. Resposta HTTP
```

---

## 📦 **Tecnologias**

| **Categoria** | **Tecnologia** | **Versão** | **Uso** |
|---------------|----------------|------------|---------|
| **Backend** | .NET | 8.0 | API RESTful |
| **Banco de Dados** | PostgreSQL | 15 | Dados relacionais |
| **ORM** | Entity Framework Core | 8.0 | Mapeamento O/R |
| **Autenticação** | ASP.NET Core Identity | 8.0 | Gerenciamento de usuários |
| **JWT** | System.IdentityModel.Tokens.Jwt | 7.0 | Tokens de autenticação |
| **2FA** | OtpNet | - | Autenticação de dois fatores |
| **Logging** | Serilog | 3.1 | Logging estruturado |
| **Cache** | Redis | - | Cache distribuído |
| **API Docs** | Swashbuckle.AspNetCore | 6.5 | Documentação Swagger |
| **Container** | Docker | - | Containerização |
| **Validação** | FluentValidation | 11.5 | Validação de modelos |
| **Mapper** | AutoMapper | 13.0 | Mapeamento de objetos |

---

## 🚀 **Instalação**

### **Pré-requisitos**

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior
- [Docker](https://www.docker.com/get-docker) e [Docker Compose](https://docs.docker.com/compose/install/)
- [PostgreSQL Client](https://www.postgresql.org/download/) (opcional, para acesso direto)

### **1. Clonar o Repositório**

```bash
# HTTPS
git clone https://github.com/pereande/SAAS.git
cd SAAS

# SSH
git clone git@github.com:pereande/SAAS.git
cd SAAS
```

### **2. Iniciar Ambiente com Docker**

```bash
# Iniciar todos os serviços (PostgreSQL, Redis, Seq, PGAdmin)
docker-compose up -d

# Verificar se os containers estão rodando
docker-compose ps
```

### **3. Inicializar Banco de Dados**

```bash
# Acessar o container do PostgreSQL
docker exec -it erp-db psql -U postgres

# Dentro do psql, executar os scripts:
\i /docker-entrypoint-initdb.d/init-master-db.sql
\c erp_master
\i /docker-entrypoint-initdb.d/seed-master-data.sql

# Ou executar diretamente do host:
psql -h localhost -U postgres -f scripts/database/init-master-db.sql
psql -h localhost -U postgres -d erp_master -f scripts/database/seed-master-data.sql
```

> **⚠️ Nota**: Os scripts de inicialização já estão configurados para serem executados automaticamente na primeira vez que o container do PostgreSQL é iniciado (via volume mount em `docker-compose.yml`).

### **4. Executar a API**

```bash
# Navegar para o diretório da API
cd src/ERP.Web

# Restaurar dependências
dotnet restore

# Executar a aplicação
dotnet run
```

A API estará disponível em:
- **HTTP**: `http://localhost:5001`
- **HTTPS**: `https://localhost:5000`
- **Swagger**: `https://localhost:5000/swagger`

---

## ⚙️ **Configuração**

### **Arquivos de Configuração**

| **Arquivo** | **Descrição** | **Ambiente** |
|-------------|---------------|--------------|
| `appsettings.json` | Configurações de desenvolvimento | Development |
| `appsettings.Production.json` | Configurações de produção | Production |
| `launchSettings.json` | Configurações de execução local | Development |

### **Configurações Principais**

#### **Conexão com Banco de Dados**

Edite `src/ERP.Web/appsettings.json`:

```json
{
  "AppSettings": {
    "Database": {
      "MasterConnectionString": "Host=localhost;Port=5432;Database=erp_master;Username=postgres;Password=postgres",
      "TenantDatabasePrefix": "erp_tenant_"
    }
  }
}
```

#### **Configurações de JWT**

```json
{
  "AppSettings": {
    "Jwt": {
      "Secret": "Sua_Chave_Secreta_Aqui_1234567890",
      "Issuer": "ERP SaaS",
      "Audience": "ERP SaaS API",
      "ExpiryMinutes": 60,
      "RefreshTokenExpiryDays": 30
    }
  }
}
```

> **⚠️ Importante**: Em produção, sempre use **variáveis de ambiente** ou **Azure Key Vault** para armazenar segredos como connection strings e JWT secrets.

### **Variáveis de Ambiente**

Crie um arquivo `.env` na raiz do projeto:

```env
# Database
DB_HOST=localhost
DB_PORT=5432
DB_NAME=erp_master
DB_USER=postgres
DB_PASSWORD=postgres

# JWT
JWT_SECRET=Sua_Chave_Secreta_Aqui
JWT_ISSUER=ERP SaaS
JWT_AUDIENCE=ERP SaaS API

# App
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=https://+:443;http://+:80
```

---

## 🔐 **Autenticação**

### **Fluxo de Autenticação**

```
1. Login com usuário e senha
   POST /api/auth/login
   
2. Recebe token JWT e refresh token
   {
     "token": "eyJhbGciOiJIUzI1NiIs...",
     "refreshToken": "abc123...",
     "expiresIn": 3600
   }
   
3. Usa token JWT no header das requisições
   Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
   
4. Quando token expira, usa refresh token
   POST /api/auth/refresh
   
5. Recebe novo token JWT
```

### **Endpoints de Autenticação**

| **Endpoint** | **Método** | **Descrição** | **Autenticação** |
|--------------|------------|---------------|------------------|
| `/api/auth/login` | POST | Login com usuário e senha | ❌ |
| `/api/auth/login-2fa` | POST | Login com código 2FA | ❌ |
| `/api/auth/refresh` | POST | Refresh token | ❌ |
| `/api/auth/logout` | POST | Logout | ✅ |
| `/api/auth/me` | GET | Dados do usuário atual | ✅ |
| `/api/auth/enable-2fa` | POST | Habilitar 2FA | ✅ |
| `/api/auth/disable-2fa` | POST | Desabilitar 2FA | ✅ |
| `/api/auth/2fa-secret` | GET | Gerar segredo para 2FA | ✅ |

### **Exemplo: Login**

**Request:**
```bash
curl -X POST "https://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin@erpsaas.com",
    "password": "Admin@123"
  }'
```

**Response (Sucesso):**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "abc123def456...",
    "expiresIn": 3600,
    "requiresTwoFactor": false,
    "user": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "username": "admin@erpsaas.com",
      "email": "admin@erpsaas.com",
      "firstName": "Admin",
      "lastName": "User",
      "fullName": "Admin User",
      "roles": ["Admin"],
      "twoFactorEnabled": false
    }
  }
}
```

**Response (2FA Required):**
```json
{
  "success": true,
  "message": "2FA required",
  "data": {
    "requiresTwoFactor": true,
    "twoFactorToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "userId": "550e8400-e29b-41d4-a716-446655440000"
  }
}
```

**Login com 2FA:**
```bash
curl -X POST "https://localhost:5000/api/auth/login-2fa" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "code": "123456"
  }'
```

### **Habilitar 2FA**

1. **Gerar segredo:**
   ```bash
   curl -X GET "https://localhost:5000/api/auth/2fa-secret" \
     -H "Authorization: Bearer {token}"
   ```

2. **Escaneie o QR Code** no Google Authenticator

3. **Habilitar 2FA:**
   ```bash
   curl -X POST "https://localhost:5000/api/auth/enable-2fa" \
     -H "Authorization: Bearer {token}" \
     -H "Content-Type: application/json" \
     -d '{
       "secret": "JBSWY3DPEHPK3PXP",
       "code": "123456"
     }'
   ```

---

## 🏢 **Multi-Tenancy**

### **Como Funciona**

O sistema usa a estratégia **Database Per Tenant**, onde cada empresa (tenant) tem seu próprio banco de dados com todas as tabelas necessárias para seus dados.

### **Identificação do Tenant**

O tenant pode ser identificado de 3 formas:

1. **Header HTTP** (Recomendado):
   ```http
   X-Tenant-Id: 550e8400-e29b-41d4-a716-446655440000
   ```

2. **Subdomínio** (Opcional):
   ```
   empresa1.erpsaas.com.br
   empresa2.erpsaas.com.br
   ```

3. **Token JWT** (Para usuários autenticados):
   ```json
   {
     "tenant_id": "550e8400-e29b-41d4-a716-446655440000"
   }
   ```

### **Criação de Tenant**

**Request:**
```bash
curl -X POST "https://localhost:5000/api/tenants" \
  -H "Authorization: Bearer {admin_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Minha Empresa LTDA",
    "cnpj": "12345678000100",
    "email": "contato@minhaempresa.com.br",
    "phone": "(11) 1234-5678",
    "maxUsers": 10,
    "maxStorage": 10737418240,
    "trialDays": 30,
    "enabledModules": ["sales", "purchases", "inventory", "financial"],
    "adminUser": {
      "username": "admin@minhaempresa.com.br",
      "email": "admin@minhaempresa.com.br",
      "password": "Admin@123",
      "firstName": "Admin",
      "lastName": "Empresarial",
      "roles": ["TenantAdmin"]
    },
    "subscription": {
      "planId": "550e8400-e29b-41d4-a716-446655440001",
      "trialDays": 30
    }
  }'
```

---

## 📂 **Estrutura do Projeto**

```
ERP.SaaS/
├── ERP.SaaS.sln                          # Solução Visual Studio
├── .gitignore                            # Arquivos ignorados pelo Git
├── docker-compose.yml                    # Configuração Docker
├── README.md                             # Este arquivo
│
├── docker/
│   └── Dockerfile.web                    # Dockerfile para API
│
├── scripts/
│   └── database/
│       ├── init-master-db.sql           # Script de criação do banco master
│       ├── init-tenant-db.sql           # Script de criação do banco tenant
│       └── seed-master-data.sql         # Dados iniciais
│
├── src/
│   ├── ERP.Shared/                       # Projeto compartilhado
│   │   ├── Constants/ErrorMessages.cs    # Mensagens de erro padronizadas
│   │   ├── Exceptions/                   # Exceções personalizadas
│   │   │   ├── BadRequestException.cs
│   │   │   ├── EntityNotFoundException.cs
│   │   │   ├── UnauthorizedException.cs
│   │   │   └── ValidationException.cs
│   │   ├── Interfaces/                   # Interfaces genéricas
│   │   │   ├── IRepository.cs
│   │   │   └── IUnitOfWork.cs
│   │   ├── Models/BaseEntity.cs          # Entidade base com timestamps
│   │   ├── Services/                     # Serviços compartilhados
│   │   │   ├── JwtService.cs            # Serviço de JWT
│   │   │   └── TwoFactorService.cs      # Serviço de 2FA
│   │   └── Settings/                     # Configurações
│   │       ├── AppSettings.cs
│   │       └── JwtSettings.cs
│   │
│   ├── ERP.Master/                       # Projeto Master (banco central)
│   │   ├── ERP.Master.csproj
│   │   ├── Infrastructure/
│   │   │   └── Data/
│   │   │       ├── Configurations/      # Configurações EF Core
│   │   │       │   └── TenantConfiguration.cs
│   │   │       └── MasterDbContext.cs    # DbContext do master
│   │   └── Models/                       # Modelos do master
│   │       ├── Tenant.cs
│   │       ├── User.cs
│   │       ├── Role.cs
│   │       ├── Permission.cs
│   │       ├── Plan.cs
│   │       ├── Subscription.cs
│   │       ├── UserToken.cs
│   │       ├── UserSession.cs
│   │       ├── LoginAttempt.cs
│   │       └── AuditLog.cs
│   │
│   ├── ERP.Tenant/                       # Projeto Tenant (banco por empresa)
│   │   ├── ERP.Tenant.csproj
│   │   ├── Infrastructure/
│   │   │   └── Data/
│   │   │       └── TenantDbContext.cs   # DbContext do tenant
│   │   └── Models/                       # Modelos do tenant
│   │       ├── Company.cs
│   │       ├── Branch.cs
│   │       ├── Person.cs
│   │       ├── Client.cs
│   │       ├── Supplier.cs
│   │       ├── Employee.cs
│   │       ├── ProductCategory.cs
│   │       ├── Brand.cs
│   │       ├── Product.cs
│   │       ├── SupplierProduct.cs
│   │       ├── Inventory.cs
│   │       ├── InventoryMovement.cs
│   │       ├── PaymentMethod.cs
│   │       ├── PaymentTerm.cs
│   │       ├── Bank.cs
│   │       ├── BankAccount.cs
│   │       ├── CostCenter.cs
│   │       ├── Sale.cs
│   │       ├── SaleItem.cs
│   │       ├── Purchase.cs
│   │       ├── PurchaseItem.cs
│   │       ├── AccountPayable.cs
│   │       ├── AccountReceivable.cs
│   │       ├── FinancialEntry.cs
│   │       ├── FiscalNote.cs
│   │       ├── FiscalNoteItem.cs
│   │       ├── Cfop.cs
│   │       ├── Ncm.cs
│   │       ├── Cst.cs
│   │       ├── Csosn.cs
│   │       └── FiscalConfiguration.cs
│   │
│   └── ERP.Web/                          # Projeto API Web
│       ├── ERP.Web.csproj
│       ├── Properties/
│       │   └── launchSettings.json
│       ├── Controllers/
│       │   ├── BaseController.cs        # Controller base
│       │   ├── AuthController.cs        # Autenticação
│       │   └── TenantsController.cs      # Gerenciamento de tenants
│       ├── Middleware/
│       │   ├── ExceptionMiddleware.cs
│       │   ├── TenantResolutionMiddleware.cs
│       │   ├── TenantValidationMiddleware.cs
│       │   ├── AuthorizationMiddleware.cs
│       │   └── AuditMiddleware.cs
│       ├── appsettings.json              # Configurações de desenvolvimento
│       ├── appsettings.Production.json   # Configurações de produção
│       └── Program.cs                    # Configuração da API
│
└── tests/                               # Projetos de teste
    ├── ERP.Master.Tests/
    │   └── ERP.Master.Tests.csproj
    ├── ERP.Shared.Tests/
    │   └── ERP.Shared.Tests.csproj
    ├── ERP.Tenant.Tests/
    │   └── ERP.Tenant.Tests.csproj
    └── ERP.Web.Tests/
        └── ERP.Web.Tests.csproj
```

---

## 🎪 **Endpoints da API**

### **Autenticação**

| **Endpoint** | **Método** | **Descrição** | **Requisição** | **Resposta** |
|--------------|------------|---------------|----------------|--------------|
| `/api/auth/login` | POST | Login | `username`, `password` | `token`, `refreshToken`, `user` |
| `/api/auth/login-2fa` | POST | Login com 2FA | `userId`, `code` | `token`, `refreshToken`, `user` |
| `/api/auth/refresh` | POST | Refresh token | `token`, `refreshToken` | `token`, `refreshToken` |
| `/api/auth/logout` | POST | Logout | - | `success` |
| `/api/auth/me` | GET | Dados do usuário | - | `user` |
| `/api/auth/enable-2fa` | POST | Habilitar 2FA | `secret`, `code` | `enabled`, `recoveryCodes` |
| `/api/auth/disable-2fa` | POST | Desabilitar 2FA | `password` | `success` |
| `/api/auth/2fa-secret` | GET | Gerar segredo 2FA | - | `secret`, `qrCodeUri` |

### **Tenants**

| **Endpoint** | **Método** | **Descrição** | **Requisição** | **Resposta** |
|--------------|------------|---------------|----------------|--------------|
| `/api/tenants` | GET | Listar tenants | `name`, `cnpj`, `email`, `status`, `pageNumber`, `pageSize` | `pagedResponse` |
| `/api/tenants/{id}` | GET | Obter tenant por ID | - | `tenant` |
| `/api/tenants` | POST | Criar tenant | `tenantData` | `tenant` |
| `/api/tenants/{id}` | PUT | Atualizar tenant | `tenantData` | `tenant` |
| `/api/tenants/{id}` | DELETE | Deletar tenant | - | `success` |
| `/api/tenants/{id}/status` | PATCH | Atualizar status | `status` | `tenant` |

---

## 📊 **Módulos Disponíveis**

| **Módulo** | **Descrição** | **Tabelas Principais** | **Status** |
|------------|---------------|------------------------|------------|
| **Sistema** | Gerenciamento de tenants, usuários e permissões | Tenants, Users, Roles, Permissions | ✅ Completo |
| **Cadastros** | Empresas, filiais, clientes, fornecedores, funcionários | Companies, Branches, Clients, Suppliers, Employees | ✅ Modelos |
| **Estoque** | Produtos, estoque, movimentações | Products, Inventories, InventoryMovements | ✅ Modelos |
| **Vendas** | Pedidos de venda, itens | Sales, SaleItems | ✅ Modelos |
| **Compras** | Pedidos de compra, itens | Purchases, PurchaseItems | ✅ Modelos |
| **Financeiro** | Contas a pagar/receber, lançamentos, bancos | AccountsPayable, AccountsReceivable, FinancialEntries, Banks | ✅ Modelos |
| **Fiscal** | Notas fiscais, impostos, configurações | FiscalNotes, FiscalNoteItems, Cfop, Ncm, Cst, Csosn | ✅ Modelos |
| **Relatórios** | Audit log, login attempts | AuditLogs, LoginAttempts | ✅ Completo |

> **📌 Status**: Todos os modelos estão implementados. Os controllers e serviços para cada módulo podem ser implementados conforme necessário.

---

## 🔧 **Desenvolvimento**

### **Requisitos**

- [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.0+) ou [VS Code](https://code.visualstudio.com/)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Docker](https://www.docker.com/get-docker) (opcional, para ambiente containerizado)

### **Comandos Úteis**

```bash
# Restaurar dependências
dotnet restore ERP.SaaS.sln

# Build da solução
dotnet build ERP.SaaS.sln

# Executar testes
dotnet test ERP.SaaS.sln

# Executar API
dotnet run --project src/ERP.Web

# Gerar migrations (Master)
dotnet ef migrations add AddTableName --project src/ERP.Master --startup-project src/ERP.Web

# Aplicar migrations (Master)
dotnet ef database update --project src/ERP.Master --startup-project src/ERP.Web
```

### **Estrutura de um Novo Controller**

```csharp
using ERP.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Controllers;

[Route("api/[controller]")]
[Authorize] // ou [Authorize(Roles = "Admin")]
public class ProductsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Lógica para obter todos os produtos
        return Success(products, "Products retrieved successfully");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        // Lógica para obter produto por ID
        return Success(product, "Product retrieved successfully");
    }

    [HttpPost]
    [Authorize(Roles = "Admin,TenantAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        // Lógica para criar produto
        return Success(product, "Product created successfully");
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,TenantAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        // Lógica para atualizar produto
        return Success(product, "Product updated successfully");
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        // Lógica para deletar produto
        return Success("Product deleted successfully");
    }
}
```

### **Criando um Novo Serviço**

```csharp
using ERP.Shared.Interfaces;
using ERP.Tenant.Models;

namespace ERP.Tenant.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Product, Guid> _productRepository;

    public ProductService(IRepository<Product, Guid> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<Product> GetByIdAsync(Guid id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        return await _productRepository.AddAsync(product);
    }

    public async Task UpdateAsync(Product product)
    {
        await _productRepository.UpdateAsync(product);
    }

    public async Task DeleteAsync(Product product)
    {
        await _productRepository.DeleteAsync(product);
    }
}
```

---

## 🐳 **Docker**

### **Comandos Docker**

```bash
# Build e executar todos os serviços
docker-compose up -d --build

# Parar todos os serviços
docker-compose down

# Ver logs
docker-compose logs -f erp-web

# Acessar container do PostgreSQL
docker exec -it erp-db bash

# Acessar psql
docker exec -it erp-db psql -U postgres
```

### **Serviços Disponíveis no Docker**

| **Serviço** | **Container** | **Porta** | **URL** | **Credenciais** |
|-------------|---------------|-----------|---------|-----------------|
| **API Web** | erp-web | 5000 (HTTPS) / 5001 (HTTP) | `https://localhost:5000` | - |
| **PostgreSQL** | erp-db | 5432 | `postgresql://localhost:5432` | postgres/postgres |
| **Redis** | erp-redis | 6379 | `redis://localhost:6379` | - |
| **Seq** | erp-seq | 5341 | `http://localhost:5341` | - |
| **PGAdmin** | erp-pgadmin | 5050 | `http://localhost:5050` | admin@erpsaas.com/admin |

### **Dockerfile**

O Dockerfile está configurado para:
1. **Stage 1 (Build)**: Compilar a aplicação
2. **Stage 2 (Runtime)**: Executar a aplicação com ASP.NET Core Runtime
3. **Multi-stage build**: Otimizar o tamanho da imagem final

---

## 📄 **Licença**

Este projeto está licenciado sob a **MIT License** - veja o arquivo [LICENSE](LICENSE) para mais detalhes.

```
MIT License

Copyright (c) 2024 ERP SaaS

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## 🤝 **Contribuição**

Contribuições são bem-vindas! Por favor, siga estas etapas:

1. **Fork** o repositório
2. **Crie uma branch** para sua feature (`git checkout -b feature/nova-feature`)
3. **Commit** suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. **Push** para a branch (`git push origin feature/nova-feature`)
5. **Abra um Pull Request**

### **Regras de Contribuição**

- ✅ Siga o estilo de código existente
- ✅ Adicione testes para novas funcionalidades
- ✅ Atualize a documentação conforme necessário
- ✅ Mantenha os commits atômicos e descritivos
- ✅ Use **conventional commits** (feat:, fix:, docs:, etc.)

### **Conventional Commits**

| **Tipo** | **Descrição** | **Exemplo** |
|----------|---------------|-------------|
| `feat` | Nova funcionalidade | `feat: add ProductsController` |
| `fix` | Correção de bug | `fix: validate tenant on login` |
| `docs` | Documentação | `docs: update README.md` |
| `style` | Formatação de código | `style: format code with dotnet-format` |
| `refactor` | Refatoração | `refactor: improve repository pattern` |
| `perf` | Melhoria de performance | `perf: optimize database queries` |
| `test` | Testes | `test: add unit tests for AuthService` |
| `chore` | Tarefas de manutenção | `chore: update NuGet packages` |

---

## 📞 **Suporte**

| **Tipo** | **Contato** | **Descrição** |
|----------|-------------|---------------|
| **Bugs** | [GitHub Issues](https://github.com/pereande/SAAS/issues) | Reportar bugs e problemas |
| **Dúvidas** | [GitHub Discussions](https://github.com/pereande/SAAS/discussions) | Tirar dúvidas |
| **Contribuições** | [Pull Requests](https://github.com/pereande/SAAS/pulls) | Enviar melhorias |
| **E-mail** | suporte@erpsaas.com.br | Suporte direto |

---

## 🎓 **Recursos Adicionais**

- [Documentação do .NET 8](https://learn.microsoft.com/pt-br/dotnet/core/whats-new/dotnet-8)
- [Documentação do Entity Framework Core](https://learn.microsoft.com/pt-br/ef/core/)
- [Documentação do PostgreSQL](https://www.postgresql.org/docs/)
- [Documentação do Docker](https://docs.docker.com/)
- [Documentação do JWT](https://jwt.io/)

---

<p align="center">
  <strong>Desenvolvido com ❤️ usando .NET 8 e PostgreSQL</strong>
</p>

<p align="center">
  <a href="https://github.com/pereande/SAAS">
    <img src="https://img.shields.io/github/stars/pereande/SAAS?style=social" alt="GitHub Stars" />
  </a>
  <a href="https://github.com/pereande/SAAS/fork">
    <img src="https://img.shields.io/github/forks/pereande/SAAS?style=social" alt="GitHub Forks" />
  </a>
  <a href="https://github.com/pereande/SAAS/watchers">
    <img src="https://img.shields.io/github/watchers/pereande/SAAS?style=social" alt="GitHub Watchers" />
  </a>
</p>

<p align="center">
  <em>© 2024 ERP SaaS. Todos os direitos reservados.</em>
</p>
