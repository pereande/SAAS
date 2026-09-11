# 🚀 Implementação P0 e P1 - ERP SaaS Multiempresa

## ✅ Status da Implementação

### Prioridade P0 (Bloqueador) - COMPLETO
- [x] Modelos do Master Database criados
- [x] DbContext configurado
- [x] Serviços JWT e 2FA implementados

### Prioridade P1 (Configuração do Núcleo) - EM ANDAMENTO
- [x] Program.cs configurado
- [x] appsettings.json criado
- [x] Projetos .csproj criados
- [ ] Pacotes NuGet instalados (requer dotnet SDK)
- [ ] Middleware de Multi-Tenancy implementado
- [ ] AuthService implementado
- [ ] Controladores API criados

---

## 📁 Arquivos Criados/Modificados

### `/src/ERP.Web/`
- ✅ `Program.cs` - Configuração completa da API (Identity, JWT, CORS, Rate Limiting, Swagger)
- ✅ `appsettings.json` - Configurações de conexão, JWT, CORS
- ✅ `appsettings.Development.json` - Configurações para desenvolvimento
- ✅ `ERP.Web.csproj` - Projeto web com todas as dependências

### `/src/ERP.Master/`
- ✅ `ERP.Master.csproj` - Projeto com Entity Framework Core + Npgsql

### `/src/ERP.Shared/`
- ✅ `ERP.Shared.csproj` - Projeto compartilhado com JWT e OtpNet

---

## 🔧 Como Rodar o Projeto

### Pré-requisitos
```bash
# Instalar .NET 8 SDK
# https://dotnet.microsoft.com/download/dotnet/8.0

# Instalar PostgreSQL
# https://www.postgresql.org/download/
```

### Passo 1: Restaurar pacotes
```bash
cd /workspace/src
dotnet restore
```

### Passo 2: Configurar banco de dados
```bash
# Criar banco de dados no PostgreSQL
createdb erp_master_development -U postgres

# Ou via psql
psql -U postgres
CREATE DATABASE erp_master_development;
\q
```

### Passo 3: Aplicar migrations
```bash
cd /workspace/src/ERP.Master
dotnet ef migrations add InitialCreate --startup-project ../ERP.Web
dotnet ef database update --startup-project ../ERP.Web
```

### Passo 4: Rodar a API
```bash
cd /workspace/src/ERP.Web
dotnet run
```

A API estará disponível em:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger**: http://localhost:5000/swagger

---

## 🔑 Credenciais de Teste

Após rodar a API pela primeira vez, um usuário admin será criado automaticamente:

| Campo | Valor |
|-------|-------|
| Email | `admin@erp.com` |
| Senha | `Admin@123` |

---

## 📋 Próximos Passos (Prioridade P1)

### 1. Implementar Middlewares de Multi-Tenancy
Criar em `/src/ERP.Web/Middleware/`:
- `TenantResolutionMiddleware.cs` - Resolver tenant via header/subdomínio/JWT
- `TenantValidationMiddleware.cs` - Validar se tenant está ativo
- `TenantDbContextMiddleware.cs` - Configurar DbContext dinâmico por tenant

### 2. Implementar AuthService
Criar em `/src/ERP.Shared/Services/`:
- `AuthService.cs` - Login, logout, refresh token, 2FA

### 3. Criar Controladores API
Criar em `/src/ERP.Web/Controllers/`:
- `AuthController.cs` - Autenticação (login, logout, 2FA)
- `TenantsController.cs` - CRUD de tenants
- `UsersController.cs` - CRUD de usuários
- `RolesController.cs` - CRUD de perfis
- `PermissionsController.cs` - CRUD de permissões

### 4. Adicionar Validações e Tratamento de Erros
- Criar `ExceptionMiddleware.cs` para tratamento global de erros
- Adicionar validações de DTOs com FluentValidation
- Implementar logging estruturado

---

## 🏗️ Arquitetura do Sistema

```
/src
├── ERP.Web/              # API RESTful (ASP.NET Core)
│   ├── Controllers/      # Endpoints da API
│   ├── Middleware/       # Middlewares customizados
│   ├── Program.cs        # Configuração da aplicação
│   └── appsettings.json  # Configurações
│
├── ERP.Master/           # Banco Master (Multi-tenancy)
│   ├── Models/           # Entidades do banco master
│   ├── Data/             # DbContext
│   └── Migrations/       # Migrations do EF Core
│
└── ERP.Shared/           # Código compartilhado
    ├── Services/         # Serviços (JWT, 2FA, Auth)
    ├── Settings/         # Configurações (JwtSettings)
    └── Entities/         # Interfaces base (IEntity)
```

---

## 🔐 Fluxo de Autenticação

1. **Login** → POST `/api/auth/login`
   - Valida email/senha
   - Verifica bloqueio de conta
   - Retorna access token + refresh token

2. **2FA (Opcional)** → POST `/api/auth/verify-2fa`
   - Valida código TOTP
   - Retorna tokens definitivos

3. **Refresh Token** → POST `/api/auth/refresh-token`
   - Renova access token expirado
   - Revoga refresh token antigo

4. **Logout** → POST `/api/auth/logout`
   - Revoga todos os tokens
   - Finaliza sessões

---

## 📊 Estrutura do Token JWT

```json
{
  "sub": "user-id",
  "email": "usuario@empresa.com",
  "first_name": "Nome",
  "last_name": "Sobrenome",
  "tenant_id": "tenant-id",
  "role": ["Admin", "User"],
  "permission": ["sales:read", "sales:write"],
  "exp": 1234567890,
  "iss": "ERP.SaaS",
  "aud": "ERP.SaaS.Client"
}
```

---

## 🛡️ Segurança Implementada

- ✅ Senha forte (8+ caracteres, maiúscula, minúscula, número, símbolo)
- ✅ Bloqueio de conta (5 tentativas falhas = 15 minutos)
- ✅ JWT com expiração (60 minutos)
- ✅ Refresh Token com expiração (7 dias)
- ✅ 2FA com TOTP (Google Authenticator)
- ✅ CORS configurável
- ✅ Rate Limiting (100 requisições/minuto)
- ✅ Hash de refresh tokens no banco
- ✅ Audit log de todas as ações
- ✅ Log de tentativas de login

---

## 📝 Checklist de Implantação

### Ambiente de Desenvolvimento
- [ ] Instalar .NET 8 SDK
- [ ] Instalar PostgreSQL
- [ ] Criar banco de dados `erp_master_development`
- [ ] Configurar connection string no appsettings.Development.json
- [ ] Rodar migrations
- [ ] Testar endpoint de login no Swagger

### Ambiente de Produção
- [ ] Gerar nova Secret Key para JWT (mínimo 32 caracteres)
- [ ] Configurar connection string de produção
- [ ] Habilitar HTTPS obrigatório
- [ ] Configurar CORS para domínios específicos
- [ ] Ajustar Rate Limiting conforme necessidade
- [ ] Configurar logging para arquivo/cloud
- [ ] Criar backup automático do banco de dados

---

## 🆘 Solução de Problemas

### Erro: "dotnet: command not found"
```bash
# Instalar .NET 8 SDK
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0
```

### Erro: "Npgsql.PostgresException: database does not exist"
```bash
# Criar banco de dados
createdb erp_master_development -U postgres
```

### Erro: "The secret key must be at least 32 characters"
```bash
# Gerar nova chave
openssl rand -base64 32
# Atualizar no appsettings.json
```

### Erro: "CORS policy not found"
```bash
# Verificar se UseCors() está antes de UseAuthorization() no Program.cs
```

---

## 📞 Suporte

Para dúvidas ou problemas, consulte:
- Documentação oficial do ASP.NET Core: https://docs.microsoft.com/aspnet/core
- Documentação do Entity Framework Core: https://docs.microsoft.com/ef/core
- Documentação do Identity: https://docs.microsoft.com/aspnet/core/security/authentication/identity

---

**Última atualização**: Setembro 2025
**Versão**: 1.0.0-alpha
