-- ERP SaaS - instalação limpa para Supabase
-- Execute este arquivo inteiro no SQL Editor de um projeto/banco vazio.
-- Não contém senhas, tokens ou connection strings.
-- Estratégia: PostgreSQL compartilhado com tenant_id.

BEGIN;

-- 1) Banco master
-- Criar extensões
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Criar schema
CREATE SCHEMA IF NOT EXISTS public;

-- Criar tabelas

-- Tabela: tenants
CREATE TABLE IF NOT EXISTS public.tenants (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(200) NOT NULL,
    cnpj VARCHAR(18) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    status VARCHAR(20) NOT NULL DEFAULT 'Active',
    db_name VARCHAR(100) NOT NULL UNIQUE,
    conn_string TEXT NOT NULL,
    max_users INTEGER,
    max_storage BIGINT,
    current_storage BIGINT NOT NULL DEFAULT 0,
    trial_end TIMESTAMP,
    subscription_id UUID,
    plan_id UUID,
    modules TEXT,
    settings JSONB,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabela: plans
CREATE TABLE IF NOT EXISTS public.plans (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(100) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    description TEXT,
    monthly_price DECIMAL(18,2) NOT NULL,
    yearly_price DECIMAL(18,2),
    trial_days INTEGER DEFAULT 30,
    max_users INTEGER,
    max_filials INTEGER,
    max_storage BIGINT,
    included_modules TEXT,
    included_features TEXT,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    is_default BOOLEAN NOT NULL DEFAULT FALSE,
    display_order INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabela: subscriptions
CREATE TABLE IF NOT EXISTS public.subscriptions (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL REFERENCES public.tenants(id) ON DELETE CASCADE,
    plan_id UUID NOT NULL REFERENCES public.plans(id) ON DELETE RESTRICT,
    status VARCHAR(20) NOT NULL DEFAULT 'Active',
    start_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    end_date TIMESTAMP,
    trial_start TIMESTAMP,
    trial_end TIMESTAMP,
    payment_method VARCHAR(50),
    payment_status VARCHAR(20),
    last_payment TIMESTAMP,
    next_payment TIMESTAMP,
    modules JSONB,
    max_users INTEGER,
    max_filials INTEGER,
    max_storage BIGINT,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabela: permissions
CREATE TABLE IF NOT EXISTS public.permissions (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    code VARCHAR(100) NOT NULL UNIQUE,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    module VARCHAR(50),
    action VARCHAR(50),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);


-- Tabela: audit_logs
CREATE TABLE IF NOT EXISTS public.audit_logs (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID,
    user_id UUID,
    action VARCHAR(100) NOT NULL,
    entity_type VARCHAR(100),
    entity_id VARCHAR(100),
    old_values JSONB,
    new_values JSONB,
    client_ip VARCHAR(45),
    user_agent VARCHAR(500),
    request_url VARCHAR(1000),
    http_method VARCHAR(10),
    status_code INTEGER,
    error_message VARCHAR(1000),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabela: login_attempts
CREATE TABLE IF NOT EXISTS public.login_attempts (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID,
    username VARCHAR(255) NOT NULL,
    client_ip VARCHAR(45),
    user_agent VARCHAR(500),
    is_success BOOLEAN NOT NULL DEFAULT FALSE,
    wrong_password BOOLEAN NOT NULL DEFAULT FALSE,
    user_not_found BOOLEAN NOT NULL DEFAULT FALSE,
    account_locked BOOLEAN NOT NULL DEFAULT FALSE,
    two_factor_failed BOOLEAN NOT NULL DEFAULT FALSE,
    error_message VARCHAR(500),
    attempt_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    tenant_id UUID
);

-- Tabela: user_sessions
CREATE TABLE IF NOT EXISTS public.user_sessions (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL,
    session_id VARCHAR(100) NOT NULL,
    access_token TEXT,
    refresh_token TEXT,
    client_ip VARCHAR(45),
    user_agent VARCHAR(500),
    expires_at TIMESTAMP,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    tenant_id UUID
);

-- Tabela: user_tokens (para Identity e refresh tokens)
CREATE TABLE IF NOT EXISTS public.user_tokens (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL,
    login_provider VARCHAR(255),
    name VARCHAR(255),
    value TEXT,
    token_hash VARCHAR(500),
    token_original TEXT,
    token_type VARCHAR(50),
    expires_at TIMESTAMP NOT NULL,
    is_revoked BOOLEAN NOT NULL DEFAULT FALSE,
    revoked_at TIMESTAMP,
    revoked_reason VARCHAR(500),
    created_ip VARCHAR(45),
    user_agent VARCHAR(500),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Criar índices

-- Índices para tenants
CREATE INDEX IF NOT EXISTS ix_tenants_cnpj ON public.tenants(cnpj);
CREATE INDEX IF NOT EXISTS ix_tenants_email ON public.tenants(email);
CREATE INDEX IF NOT EXISTS ix_tenants_db_name ON public.tenants(db_name);
CREATE INDEX IF NOT EXISTS ix_tenants_status ON public.tenants(status);
CREATE INDEX IF NOT EXISTS ix_tenants_created_at ON public.tenants(created_at);

-- Índices para audit_logs
CREATE INDEX IF NOT EXISTS ix_audit_logs_tenant_id ON public.audit_logs(tenant_id);
CREATE INDEX IF NOT EXISTS ix_audit_logs_user_id ON public.audit_logs(user_id);
CREATE INDEX IF NOT EXISTS ix_audit_logs_created_at ON public.audit_logs(created_at);

-- Índices para login_attempts
CREATE INDEX IF NOT EXISTS ix_login_attempts_user_id ON public.login_attempts(user_id);
CREATE INDEX IF NOT EXISTS ix_login_attempts_username ON public.login_attempts(username);
CREATE INDEX IF NOT EXISTS ix_login_attempts_attempt_time ON public.login_attempts(attempt_time);

-- Índices para user_sessions
CREATE INDEX IF NOT EXISTS ix_user_sessions_user_id ON public.user_sessions(user_id);
CREATE INDEX IF NOT EXISTS ix_user_sessions_session_id ON public.user_sessions(session_id);

-- Inserir dados iniciais

-- Planos padrão
INSERT INTO public.plans (id, name, code, description, monthly_price, yearly_price, trial_days, max_users, max_filials, max_storage, included_modules, included_features, is_active, is_default, display_order) VALUES
('550e8400-e29b-41d4-a716-446655440000', 'Basic', 'basic', 'Plano básico para pequenas empresas', 99.00, 990.00, 30, 5, 1, 1073741824, '{"sales","purchases","inventory","financial"}', '{"basic_reports","email_support"}', true, true, 1),
('550e8400-e29b-41d4-a716-446655440001', 'Professional', 'pro', 'Plano profissional para empresas em crescimento', 299.00, 2990.00, 30, 20, 5, 10737418240, '{"sales","purchases","inventory","financial","fiscal","crm"}', '{"advanced_reports","priority_support","api_access"}', true, false, 2),
('550e8400-e29b-41d4-a716-446655440002', 'Enterprise', 'enterprise', 'Plano enterprise para grandes empresas', 999.00, 9990.00, 30, 100, 50, 107374182400, '{"sales","purchases","inventory","financial","fiscal","crm","hr","reports","dashboard","api"}', '{"custom_reports","24_7_support","dedicated_account_manager","custom_integrations"}', true, false, 3);

-- Permissões padrão
INSERT INTO public.permissions (id, code, name, description, module, action) VALUES
('660e8400-e29b-41d4-a716-446655440000', 'tenants:read', 'View Tenants', 'Permite visualizar tenants', 'system', 'read'),
('660e8400-e29b-41d4-a716-446655440001', 'tenants:create', 'Create Tenants', 'Permite criar tenants', 'system', 'create'),
('660e8400-e29b-41d4-a716-446655440002', 'tenants:update', 'Update Tenants', 'Permite atualizar tenants', 'system', 'update'),
('660e8400-e29b-41d4-a716-446655440003', 'tenants:delete', 'Delete Tenants', 'Permite deletar tenants', 'system', 'delete'),
('660e8400-e29b-41d4-a716-446655440010', 'users:read', 'View Users', 'Permite visualizar usuários', 'system', 'read'),
('660e8400-e29b-41d4-a716-446655440011', 'users:create', 'Create Users', 'Permite criar usuários', 'system', 'create'),
('660e8400-e29b-41d4-a716-446655440012', 'users:update', 'Update Users', 'Permite atualizar usuários', 'system', 'update'),
('660e8400-e29b-41d4-a716-446655440013', 'users:delete', 'Delete Users', 'Permite deletar usuários', 'system', 'delete'),
('660e8400-e29b-41d4-a716-446655440020', 'roles:read', 'View Roles', 'Permite visualizar perfis', 'system', 'read'),
('660e8400-e29b-41d4-a716-446655440021', 'roles:create', 'Create Roles', 'Permite criar perfis', 'system', 'create'),
('660e8400-e29b-41d4-a716-446655440022', 'roles:update', 'Update Roles', 'Permite atualizar perfis', 'system', 'update'),
('660e8400-e29b-41d4-a716-446655440023', 'roles:delete', 'Delete Roles', 'Permite deletar perfis', 'system', 'delete'),
('660e8400-e29b-41d4-a716-446655440100', 'products:read', 'View Products', 'Permite visualizar produtos', 'inventory', 'read'),
('660e8400-e29b-41d4-a716-446655440101', 'products:create', 'Create Products', 'Permite criar produtos', 'inventory', 'create'),
('660e8400-e29b-41d4-a716-446655440102', 'products:update', 'Update Products', 'Permite atualizar produtos', 'inventory', 'update'),
('660e8400-e29b-41d4-a716-446655440103', 'products:delete', 'Delete Products', 'Permite deletar produtos', 'inventory', 'delete'),
('660e8400-e29b-41d4-a716-446655440200', 'sales:read', 'View Sales', 'Permite visualizar vendas', 'sales', 'read'),
('660e8400-e29b-41d4-a716-446655440201', 'sales:create', 'Create Sales', 'Permite criar vendas', 'sales', 'create'),
('660e8400-e29b-41d4-a716-446655440202', 'sales:update', 'Update Sales', 'Permite atualizar vendas', 'sales', 'update'),
('660e8400-e29b-41d4-a716-446655440203', 'sales:delete', 'Delete Sales', 'Permite deletar vendas', 'sales', 'delete');

-- Comentários
COMMENT ON TABLE public.tenants IS 'Tabela de empresas contratantes (tenants) do sistema ERP SaaS';
COMMENT ON TABLE public.plans IS 'Tabela de planos de assinatura';
COMMENT ON TABLE public.subscriptions IS 'Tabela de assinaturas dos tenants';
COMMENT ON TABLE public.permissions IS 'Tabela de permissões do sistema';
COMMENT ON TABLE public.audit_logs IS 'Tabela de logs de auditoria';
COMMENT ON TABLE public.login_attempts IS 'Tabela de tentativas de login';
COMMENT ON TABLE public.user_sessions IS 'Tabela de sessões de usuário';
COMMENT ON TABLE public.user_tokens IS 'Tabela de tokens de usuário (refresh tokens, etc.)';


-- 2) Tabelas ASP.NET Identity
-- Tabelas ASP.NET Identity usadas pelo ERP SaaS no Supabase.
-- Execute depois de init-master-db-supabase.sql.
-- O projeto usa nomes de tabelas minúsculos, mas mantém os nomes padrão
-- das propriedades EF Core nas colunas.

CREATE TABLE IF NOT EXISTS public.users (
    "Id" UUID NOT NULL PRIMARY KEY,
    "UserName" VARCHAR(256),
    "NormalizedUserName" VARCHAR(256),
    "Email" VARCHAR(256),
    "NormalizedEmail" VARCHAR(256),
    "EmailConfirmed" BOOLEAN NOT NULL DEFAULT FALSE,
    "PasswordHash" TEXT,
    "SecurityStamp" TEXT,
    "ConcurrencyStamp" TEXT,
    "PhoneNumber" TEXT,
    "PhoneNumberConfirmed" BOOLEAN NOT NULL DEFAULT FALSE,
    "TwoFactorEnabled" BOOLEAN NOT NULL DEFAULT FALSE,
    "LockoutEnd" TIMESTAMPTZ,
    "LockoutEnabled" BOOLEAN NOT NULL DEFAULT FALSE,
    "AccessFailedCount" INTEGER NOT NULL DEFAULT 0,
    "TenantId" UUID,
    "FirstName" VARCHAR(100) NOT NULL DEFAULT '',
    "LastName" VARCHAR(100) NOT NULL DEFAULT '',
    "Locale" VARCHAR(20) NOT NULL DEFAULT 'pt-BR',
    "Timezone" VARCHAR(100) NOT NULL DEFAULT 'America/Sao_Paulo',
    "AvatarUrl" TEXT,
    "Status" INTEGER NOT NULL DEFAULT 0,
    "EmailVerified" BOOLEAN NOT NULL DEFAULT FALSE,
    "LastLogin" TIMESTAMP,
    "LastLoginIp" VARCHAR(45),
    "FailedLoginAttempts" INTEGER NOT NULL DEFAULT 0,
    "TwoFactorSecret" TEXT,
    "TwoFactorRecoveryCodes" TEXT,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS public.roles (
    "Id" UUID NOT NULL PRIMARY KEY,
    "Name" VARCHAR(256),
    "NormalizedName" VARCHAR(256),
    "ConcurrencyStamp" TEXT,
    "Description" TEXT,
    "IsSystem" BOOLEAN NOT NULL DEFAULT FALSE,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS public.user_roles (
    "Id" UUID NOT NULL PRIMARY KEY,
    "UserId" UUID NOT NULL REFERENCES public.users("Id") ON DELETE CASCADE,
    "RoleId" UUID NOT NULL REFERENCES public.roles("Id") ON DELETE CASCADE,
    CONSTRAINT uq_user_roles_user_id_role_id UNIQUE ("UserId", "RoleId")
);

CREATE TABLE IF NOT EXISTS public.user_claims (
    "Id" INTEGER GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    "UserId" UUID NOT NULL REFERENCES public.users("Id") ON DELETE CASCADE,
    "ClaimType" TEXT,
    "ClaimValue" TEXT
);

CREATE TABLE IF NOT EXISTS public.user_logins (
    "Id" UUID NOT NULL PRIMARY KEY,
    "LoginProvider" VARCHAR(128) NOT NULL,
    "ProviderKey" VARCHAR(128) NOT NULL,
    "ProviderDisplayName" TEXT,
    "UserId" UUID NOT NULL REFERENCES public.users("Id") ON DELETE CASCADE,
    CONSTRAINT uq_user_logins_provider_key UNIQUE ("LoginProvider", "ProviderKey")
);

CREATE TABLE IF NOT EXISTS public.role_claims (
    "Id" INTEGER GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    "RoleId" UUID NOT NULL REFERENCES public.roles("Id") ON DELETE CASCADE,
    "ClaimType" TEXT,
    "ClaimValue" TEXT
);

CREATE TABLE IF NOT EXISTS public.identity_user_tokens (
    "UserId" UUID NOT NULL REFERENCES public.users("Id") ON DELETE CASCADE,
    "LoginProvider" VARCHAR(128) NOT NULL,
    "Name" VARCHAR(128) NOT NULL,
    "Value" TEXT,
    CONSTRAINT pk_identity_user_tokens PRIMARY KEY ("UserId", "LoginProvider", "Name")
);

CREATE TABLE IF NOT EXISTS public.role_permissions (
    "Id" UUID NOT NULL PRIMARY KEY,
    "RoleId" UUID NOT NULL REFERENCES public.roles("Id") ON DELETE CASCADE,
    "PermissionId" UUID NOT NULL REFERENCES public.permissions(id) ON DELETE CASCADE,
    CONSTRAINT uq_role_permissions_role_id_permission_id UNIQUE ("RoleId", "PermissionId")
);

CREATE UNIQUE INDEX IF NOT EXISTS ix_users_normalized_user_name
    ON public.users("NormalizedUserName")
    WHERE "NormalizedUserName" IS NOT NULL;

CREATE UNIQUE INDEX IF NOT EXISTS ix_users_normalized_email
    ON public.users("NormalizedEmail")
    WHERE "NormalizedEmail" IS NOT NULL;

CREATE UNIQUE INDEX IF NOT EXISTS ix_users_email
    ON public.users("Email")
    WHERE "Email" IS NOT NULL;

CREATE UNIQUE INDEX IF NOT EXISTS ix_users_username
    ON public.users("UserName")
    WHERE "UserName" IS NOT NULL;

CREATE INDEX IF NOT EXISTS ix_user_claims_user_id
    ON public.user_claims("UserId");

CREATE INDEX IF NOT EXISTS ix_role_claims_role_id
    ON public.role_claims("RoleId");

CREATE INDEX IF NOT EXISTS ix_user_roles_role_id
    ON public.user_roles("RoleId");

CREATE INDEX IF NOT EXISTS ix_role_permissions_role_id
    ON public.role_permissions("RoleId");

CREATE INDEX IF NOT EXISTS ix_role_permissions_permission_id
    ON public.role_permissions("PermissionId");

INSERT INTO public.roles ("Id", "Name", "NormalizedName", "Description", "IsSystem", "IsActive")
VALUES (
    '770e8400-e29b-41d4-a716-446655440000',
    'Admin',
    'ADMIN',
    'Administrador do sistema',
    TRUE,
    TRUE
)
ON CONFLICT ("Id") DO NOTHING;

-- 3) Tabelas do ERP tenant no banco compartilhado
-- Criar extensões
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Criar schema
CREATE SCHEMA IF NOT EXISTS public;

-- Criar tabelas

-- Tabela: companies
CREATE TABLE IF NOT EXISTS public.companies (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    corporate_name VARCHAR(200) NOT NULL,
    trade_name VARCHAR(200),
    cnpj VARCHAR(18) NOT NULL,
    state_registration VARCHAR(20),
    municipal_registration VARCHAR(20),
    tax_regime VARCHAR(20) NOT NULL DEFAULT 'SimpleNational',
    opening_date DATE,
    email VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    website VARCHAR(500),
    zip_code VARCHAR(10),
    address VARCHAR(200),
    address_number VARCHAR(20),
    address_complement VARCHAR(100),
    neighborhood VARCHAR(100),
    city VARCHAR(100),
    state VARCHAR(2),
    country VARCHAR(100) NOT NULL DEFAULT 'Brasil',
    logo_url VARCHAR(500),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: branches
CREATE TABLE IF NOT EXISTS public.branches (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    company_id UUID NOT NULL REFERENCES public.companies(id) ON DELETE CASCADE,
    code VARCHAR(20) NOT NULL,
    name VARCHAR(200) NOT NULL,
    cnpj VARCHAR(18),
    state_registration VARCHAR(20),
    municipal_registration VARCHAR(20),
    email VARCHAR(255),
    phone VARCHAR(20),
    zip_code VARCHAR(10),
    address VARCHAR(200),
    address_number VARCHAR(20),
    address_complement VARCHAR(100),
    neighborhood VARCHAR(100),
    city VARCHAR(100),
    state VARCHAR(2),
    country VARCHAR(100) NOT NULL DEFAULT 'Brasil',
    is_headquarters BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    invoice_sequence INTEGER NOT NULL DEFAULT 0,
    sale_sequence INTEGER NOT NULL DEFAULT 0,
    purchase_sequence INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: people
CREATE TABLE IF NOT EXISTS public.people (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    person_type VARCHAR(20) NOT NULL DEFAULT 'Physical',
    name VARCHAR(200) NOT NULL,
    document VARCHAR(18),
    rg_ie VARCHAR(20),
    birth_date DATE,
    email VARCHAR(255),
    phone VARCHAR(20),
    secondary_phone VARCHAR(20),
    zip_code VARCHAR(10),
    address VARCHAR(200),
    address_number VARCHAR(20),
    address_complement VARCHAR(100),
    neighborhood VARCHAR(100),
    city VARCHAR(100),
    state VARCHAR(2),
    country VARCHAR(100) NOT NULL DEFAULT 'Brasil',
    notes TEXT,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: clients
CREATE TABLE IF NOT EXISTS public.clients (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    person_id UUID NOT NULL REFERENCES public.people(id) ON DELETE CASCADE,
    code VARCHAR(20) NOT NULL,
    credit_limit DECIMAL(18,2),
    payment_days INTEGER,
    payment_term_id UUID,
    notes TEXT,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: suppliers
CREATE TABLE IF NOT EXISTS public.suppliers (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    person_id UUID NOT NULL REFERENCES public.people(id) ON DELETE CASCADE,
    code VARCHAR(20) NOT NULL,
    contact VARCHAR(200),
    contact_phone VARCHAR(20),
    contact_email VARCHAR(255),
    notes TEXT,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: employees
CREATE TABLE IF NOT EXISTS public.employees (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    person_id UUID NOT NULL REFERENCES public.people(id) ON DELETE CASCADE,
    branch_id UUID REFERENCES public.branches(id) ON DELETE SET NULL,
    registration VARCHAR(20) NOT NULL,
    position VARCHAR(100),
    department VARCHAR(100),
    salary DECIMAL(18,2),
    hire_date DATE,
    termination_date DATE,
    employment_type VARCHAR(50),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    notes TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: product_categories
CREATE TABLE IF NOT EXISTS public.product_categories (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    parent_id UUID REFERENCES public.product_categories(id) ON DELETE SET NULL,
    code VARCHAR(20),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    display_order INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: brands
CREATE TABLE IF NOT EXISTS public.brands (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    website VARCHAR(500),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: products
CREATE TABLE IF NOT EXISTS public.products (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    code VARCHAR(50) NOT NULL,
    barcode VARCHAR(100),
    name VARCHAR(200) NOT NULL,
    description TEXT,
    category_id UUID REFERENCES public.product_categories(id) ON DELETE SET NULL,
    brand_id UUID REFERENCES public.brands(id) ON DELETE SET NULL,
    product_type VARCHAR(20) NOT NULL DEFAULT 'Product',
    unit_of_measure VARCHAR(10) NOT NULL DEFAULT 'UN',
    cost_price DECIMAL(18,2),
    sale_price DECIMAL(18,2),
    min_sale_price DECIMAL(18,2),
    profit_margin DECIMAL(18,2),
    icms_rate DECIMAL(18,2),
    ipi_rate DECIMAL(18,2),
    pis_rate DECIMAL(18,2),
    cofins_rate DECIMAL(18,2),
    ncm_id UUID,
    cst_id UUID,
    csosn_id UUID,
    min_stock DECIMAL(18,4),
    max_stock DECIMAL(18,4),
    reorder_point DECIMAL(18,4),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    manage_stock BOOLEAN NOT NULL DEFAULT TRUE,
    allow_sale_without_stock BOOLEAN NOT NULL DEFAULT FALSE,
    notes TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: ncms
CREATE TABLE IF NOT EXISTS public.ncms (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    code VARCHAR(20) NOT NULL,
    description VARCHAR(500) NOT NULL,
    ipi_rate DECIMAL(18,2),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabela: csts
CREATE TABLE IF NOT EXISTS public.csts (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    code VARCHAR(10) NOT NULL,
    description VARCHAR(500) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabela: csosns
CREATE TABLE IF NOT EXISTS public.csosns (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    code VARCHAR(10) NOT NULL,
    description VARCHAR(500) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabela: cfops
CREATE TABLE IF NOT EXISTS public.cfops (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    code VARCHAR(10) NOT NULL,
    description VARCHAR(500) NOT NULL,
    application VARCHAR(100),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabela: inventories
CREATE TABLE IF NOT EXISTS public.inventories (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    branch_id UUID NOT NULL REFERENCES public.branches(id) ON DELETE CASCADE,
    product_id UUID NOT NULL REFERENCES public.products(id) ON DELETE CASCADE,
    quantity DECIMAL(18,4) NOT NULL DEFAULT 0,
    reserved_quantity DECIMAL(18,4) NOT NULL DEFAULT 0,
    average_cost DECIMAL(18,2),
    location VARCHAR(100),
    last_entry_date TIMESTAMP,
    last_exit_date TIMESTAMP,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: inventory_movements
CREATE TABLE IF NOT EXISTS public.inventory_movements (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    inventory_id UUID REFERENCES public.inventories(id) ON DELETE SET NULL,
    branch_id UUID NOT NULL REFERENCES public.branches(id) ON DELETE CASCADE,
    product_id UUID NOT NULL REFERENCES public.products(id) ON DELETE CASCADE,
    movement_type VARCHAR(20) NOT NULL DEFAULT 'Entry',
    quantity DECIMAL(18,4) NOT NULL,
    previous_quantity DECIMAL(18,4),
    new_quantity DECIMAL(18,4),
    unit_cost DECIMAL(18,2),
    total_cost DECIMAL(18,2),
    sale_id UUID,
    purchase_id UUID,
    sale_item_id UUID,
    purchase_item_id UUID,
    description VARCHAR(500),
    notes TEXT,
    user_id VARCHAR(450),
    user_ip VARCHAR(45),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: payment_methods
CREATE TABLE IF NOT EXISTS public.payment_methods (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    name VARCHAR(100) NOT NULL,
    code VARCHAR(10) NOT NULL,
    description VARCHAR(500),
    payment_method_type VARCHAR(20) NOT NULL DEFAULT 'Cash',
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: payment_terms
CREATE TABLE IF NOT EXISTS public.payment_terms (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(500),
    days INTEGER NOT NULL DEFAULT 0,
    cash_discount_percent DECIMAL(18,2),
    cash_discount_value DECIMAL(18,2),
    late_fee_percent DECIMAL(18,2),
    daily_interest_percent DECIMAL(18,2),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: banks
CREATE TABLE IF NOT EXISTS public.banks (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    code VARCHAR(10) NOT NULL,
    name VARCHAR(200) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: bank_accounts
CREATE TABLE IF NOT EXISTS public.bank_accounts (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    bank_id UUID NOT NULL REFERENCES public.banks(id) ON DELETE CASCADE,
    branch_id UUID REFERENCES public.branches(id) ON DELETE SET NULL,
    agency VARCHAR(20),
    agency_digit VARCHAR(5),
    account VARCHAR(20) NOT NULL,
    account_digit VARCHAR(5),
    account_type VARCHAR(20) NOT NULL DEFAULT 'Checking',
    account_name VARCHAR(100),
    current_balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    initial_balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    initial_balance_date DATE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    is_default BOOLEAN NOT NULL DEFAULT FALSE,
    notes TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: cost_centers
CREATE TABLE IF NOT EXISTS public.cost_centers (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    code VARCHAR(20) NOT NULL,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(500),
    cost_center_type VARCHAR(20) NOT NULL DEFAULT 'Expense',
    parent_id UUID REFERENCES public.cost_centers(id) ON DELETE SET NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: sales
CREATE TABLE IF NOT EXISTS public.sales (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    branch_id UUID NOT NULL REFERENCES public.branches(id) ON DELETE CASCADE,
    number VARCHAR(20) NOT NULL,
    client_id UUID REFERENCES public.clients(id) ON DELETE SET NULL,
    client_name VARCHAR(200),
    client_document VARCHAR(18),
    sale_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    status VARCHAR(20) NOT NULL DEFAULT 'Draft',
    sale_type VARCHAR(20) NOT NULL DEFAULT 'Sale',
    payment_term_id UUID REFERENCES public.payment_terms(id) ON DELETE SET NULL,
    seller_id UUID REFERENCES public.employees(id) ON DELETE SET NULL,
    subtotal DECIMAL(18,2) NOT NULL DEFAULT 0,
    discount DECIMAL(18,2) NOT NULL DEFAULT 0,
    discount_percent DECIMAL(18,2),
    icms_total DECIMAL(18,2),
    ipi_total DECIMAL(18,2),
    pis_total DECIMAL(18,2),
    cofins_total DECIMAL(18,2),
    freight DECIMAL(18,2),
    other_expenses DECIMAL(18,2),
    total DECIMAL(18,2) NOT NULL DEFAULT 0,
    notes TEXT,
    fiscal_note_id UUID,
    cancelled_date TIMESTAMP,
    cancelled_reason VARCHAR(500),
    cancelled_by VARCHAR(450),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: sale_items
CREATE TABLE IF NOT EXISTS public.sale_items (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    sale_id UUID NOT NULL REFERENCES public.sales(id) ON DELETE CASCADE,
    product_id UUID NOT NULL REFERENCES public.products(id) ON DELETE CASCADE,
    product_code VARCHAR(50),
    product_name VARCHAR(200),
    quantity DECIMAL(18,4) NOT NULL DEFAULT 0,
    unit_of_measure VARCHAR(10),
    unit_price DECIMAL(18,2) NOT NULL DEFAULT 0,
    discount DECIMAL(18,2),
    discount_percent DECIMAL(18,2),
    icms_rate DECIMAL(18,2),
    icms_value DECIMAL(18,2),
    ipi_rate DECIMAL(18,2),
    ipi_value DECIMAL(18,2),
    pis_rate DECIMAL(18,2),
    pis_value DECIMAL(18,2),
    cofins_rate DECIMAL(18,2),
    cofins_value DECIMAL(18,2),
    subtotal DECIMAL(18,2) NOT NULL DEFAULT 0,
    total DECIMAL(18,2) NOT NULL DEFAULT 0,
    notes VARCHAR(500),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: purchases
CREATE TABLE IF NOT EXISTS public.purchases (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    branch_id UUID NOT NULL REFERENCES public.branches(id) ON DELETE CASCADE,
    number VARCHAR(20) NOT NULL,
    supplier_id UUID NOT NULL REFERENCES public.suppliers(id) ON DELETE CASCADE,
    purchase_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    entry_date TIMESTAMP,
    status VARCHAR(20) NOT NULL DEFAULT 'Draft',
    purchase_type VARCHAR(20) NOT NULL DEFAULT 'Purchase',
    payment_term_id UUID REFERENCES public.payment_terms(id) ON DELETE SET NULL,
    buyer_id UUID REFERENCES public.employees(id) ON DELETE SET NULL,
    subtotal DECIMAL(18,2) NOT NULL DEFAULT 0,
    discount DECIMAL(18,2) NOT NULL DEFAULT 0,
    discount_percent DECIMAL(18,2),
    freight DECIMAL(18,2),
    other_expenses DECIMAL(18,2),
    icms_total DECIMAL(18,2),
    ipi_total DECIMAL(18,2),
    pis_total DECIMAL(18,2),
    cofins_total DECIMAL(18,2),
    total DECIMAL(18,2) NOT NULL DEFAULT 0,
    notes TEXT,
    fiscal_note_id UUID,
    cancelled_date TIMESTAMP,
    cancelled_reason VARCHAR(500),
    cancelled_by VARCHAR(450),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: purchase_items
CREATE TABLE IF NOT EXISTS public.purchase_items (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    purchase_id UUID NOT NULL REFERENCES public.purchases(id) ON DELETE CASCADE,
    product_id UUID REFERENCES public.products(id) ON DELETE SET NULL,
    product_code VARCHAR(50),
    product_name VARCHAR(200),
    quantity DECIMAL(18,4) NOT NULL DEFAULT 0,
    received_quantity DECIMAL(18,4),
    unit_of_measure VARCHAR(10),
    unit_price DECIMAL(18,2) NOT NULL DEFAULT 0,
    discount DECIMAL(18,2),
    discount_percent DECIMAL(18,2),
    icms_rate DECIMAL(18,2),
    icms_value DECIMAL(18,2),
    ipi_rate DECIMAL(18,2),
    ipi_value DECIMAL(18,2),
    pis_rate DECIMAL(18,2),
    pis_value DECIMAL(18,2),
    cofins_rate DECIMAL(18,2),
    cofins_value DECIMAL(18,2),
    subtotal DECIMAL(18,2) NOT NULL DEFAULT 0,
    total DECIMAL(18,2) NOT NULL DEFAULT 0,
    notes VARCHAR(500),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: accounts_payable
CREATE TABLE IF NOT EXISTS public.accounts_payable (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    number VARCHAR(20) NOT NULL,
    supplier_id UUID REFERENCES public.suppliers(id) ON DELETE SET NULL,
    supplier_name VARCHAR(200),
    purchase_id UUID,
    document_type VARCHAR(20) NOT NULL DEFAULT 'Invoice',
    document_number VARCHAR(50),
    issue_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    due_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    value DECIMAL(18,2) NOT NULL DEFAULT 0,
    paid_value DECIMAL(18,2),
    payment_method_id UUID REFERENCES public.payment_methods(id) ON DELETE SET NULL,
    cost_center_id UUID REFERENCES public.cost_centers(id) ON DELETE SET NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'Pending',
    notes VARCHAR(500),
    payment_date TIMESTAMP,
    financial_entry_id UUID,
    fiscal_note_id UUID,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: accounts_receivable
CREATE TABLE IF NOT EXISTS public.accounts_receivable (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    number VARCHAR(20) NOT NULL,
    client_id UUID REFERENCES public.clients(id) ON DELETE SET NULL,
    client_name VARCHAR(200),
    sale_id UUID,
    document_type VARCHAR(20) NOT NULL DEFAULT 'Invoice',
    document_number VARCHAR(50),
    issue_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    due_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    value DECIMAL(18,2) NOT NULL DEFAULT 0,
    received_value DECIMAL(18,2),
    payment_method_id UUID REFERENCES public.payment_methods(id) ON DELETE SET NULL,
    cost_center_id UUID REFERENCES public.cost_centers(id) ON DELETE SET NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'Pending',
    notes VARCHAR(500),
    receipt_date TIMESTAMP,
    financial_entry_id UUID,
    fiscal_note_id UUID,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: financial_entries
CREATE TABLE IF NOT EXISTS public.financial_entries (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    number VARCHAR(20) NOT NULL,
    entry_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    entry_type VARCHAR(20) NOT NULL DEFAULT 'Expense',
    description VARCHAR(500) NOT NULL,
    value DECIMAL(18,2) NOT NULL DEFAULT 0,
    payment_method_id UUID REFERENCES public.payment_methods(id) ON DELETE SET NULL,
    cost_center_id UUID REFERENCES public.cost_centers(id) ON DELETE SET NULL,
    bank_account_id UUID REFERENCES public.bank_accounts(id) ON DELETE SET NULL,
    supplier_id UUID REFERENCES public.suppliers(id) ON DELETE SET NULL,
    client_id UUID REFERENCES public.clients(id) ON DELETE SET NULL,
    document_number VARCHAR(50),
    document_date DATE,
    status VARCHAR(20) NOT NULL DEFAULT 'Pending',
    is_reconciled BOOLEAN NOT NULL DEFAULT FALSE,
    reconciliation_date TIMESTAMP,
    notes TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: fiscal_notes
CREATE TABLE IF NOT EXISTS public.fiscal_notes (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    branch_id UUID NOT NULL REFERENCES public.branches(id) ON DELETE CASCADE,
    note_type VARCHAR(20) NOT NULL DEFAULT 'Output',
    series VARCHAR(10) NOT NULL DEFAULT '1',
    number VARCHAR(20) NOT NULL,
    issue_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    entry_exit_date TIMESTAMP,
    client_id UUID REFERENCES public.clients(id) ON DELETE SET NULL,
    client_name VARCHAR(200),
    client_document VARCHAR(18),
    client_ie VARCHAR(20),
    supplier_id UUID REFERENCES public.suppliers(id) ON DELETE SET NULL,
    supplier_name VARCHAR(200),
    supplier_document VARCHAR(18),
    supplier_ie VARCHAR(20),
    sale_id UUID,
    purchase_id UUID,
    cfop VARCHAR(10),
    icms_basis DECIMAL(18,2),
    icms_value DECIMAL(18,2),
    icms_rate DECIMAL(18,2),
    ipi_basis DECIMAL(18,2),
    ipi_value DECIMAL(18,2),
    ipi_rate DECIMAL(18,2),
    pis_basis DECIMAL(18,2),
    pis_value DECIMAL(18,2),
    pis_rate DECIMAL(18,2),
    cofins_basis DECIMAL(18,2),
    cofins_value DECIMAL(18,2),
    cofins_rate DECIMAL(18,2),
    subtotal DECIMAL(18,2) NOT NULL DEFAULT 0,
    discount DECIMAL(18,2),
    freight DECIMAL(18,2),
    insurance DECIMAL(18,2),
    other_expenses DECIMAL(18,2),
    total DECIMAL(18,2) NOT NULL DEFAULT 0,
    status VARCHAR(20) NOT NULL DEFAULT 'Pending',
    is_authorized BOOLEAN NOT NULL DEFAULT FALSE,
    authorization_date TIMESTAMP,
    authorization_protocol VARCHAR(50),
    xml TEXT,
    pdf BYTEA,
    access_key VARCHAR(50),
    notes TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: fiscal_note_items
CREATE TABLE IF NOT EXISTS public.fiscal_note_items (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    fiscal_note_id UUID NOT NULL REFERENCES public.fiscal_notes(id) ON DELETE CASCADE,
    product_id UUID REFERENCES public.products(id) ON DELETE SET NULL,
    product_code VARCHAR(50),
    product_name VARCHAR(200),
    ncm VARCHAR(20),
    cfop VARCHAR(10),
    cst VARCHAR(10),
    csosn VARCHAR(10),
    unit_of_measure VARCHAR(10),
    quantity DECIMAL(18,4) NOT NULL DEFAULT 0,
    unit_price DECIMAL(18,2) NOT NULL DEFAULT 0,
    discount DECIMAL(18,2),
    total DECIMAL(18,2) NOT NULL DEFAULT 0,
    icms_basis DECIMAL(18,2),
    icms_value DECIMAL(18,2),
    icms_rate DECIMAL(18,2),
    ipi_basis DECIMAL(18,2),
    ipi_value DECIMAL(18,2),
    ipi_rate DECIMAL(18,2),
    pis_basis DECIMAL(18,2),
    pis_value DECIMAL(18,2),
    pis_rate DECIMAL(18,2),
    cofins_basis DECIMAL(18,2),
    cofins_value DECIMAL(18,2),
    cofins_rate DECIMAL(18,2),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: supplier_products
CREATE TABLE IF NOT EXISTS public.supplier_products (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    supplier_id UUID NOT NULL REFERENCES public.suppliers(id) ON DELETE CASCADE,
    product_id UUID NOT NULL REFERENCES public.products(id) ON DELETE CASCADE,
    supplier_code VARCHAR(50),
    purchase_price DECIMAL(18,2),
    delivery_days INTEGER,
    min_purchase_quantity DECIMAL(18,4),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Tabela: fiscal_configurations
CREATE TABLE IF NOT EXISTS public.fiscal_configurations (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    company_id UUID NOT NULL REFERENCES public.companies(id) ON DELETE CASCADE,
    default_output_cfop VARCHAR(10),
    default_input_cfop VARCHAR(10),
    default_cst VARCHAR(10),
    default_csosn VARCHAR(10),
    default_icms_rate DECIMAL(18,2),
    auto_issue_fiscal_note BOOLEAN NOT NULL DEFAULT FALSE,
    default_series VARCHAR(10) NOT NULL DEFAULT '1',
    next_fiscal_note_number INTEGER NOT NULL DEFAULT 1,
    environment VARCHAR(20) NOT NULL DEFAULT 'Production',
    digital_certificate BYTEA,
    certificate_password VARCHAR(100),
    certificate_expiry_date DATE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(450),
    updated_by VARCHAR(450)
);

-- Criar índices

-- Índices para companies
CREATE INDEX IF NOT EXISTS ix_companies_tenant_id ON public.companies(tenant_id);
CREATE INDEX IF NOT EXISTS ix_companies_cnpj ON public.companies(cnpj);
CREATE INDEX IF NOT EXISTS ix_companies_name ON public.companies(corporate_name);

-- Índices para branches
CREATE INDEX IF NOT EXISTS ix_branches_tenant_id ON public.branches(tenant_id);
CREATE INDEX IF NOT EXISTS ix_branches_company_id ON public.branches(company_id);
CREATE INDEX IF NOT EXISTS ix_branches_code ON public.branches(code);

-- Índices para people
CREATE INDEX IF NOT EXISTS ix_people_tenant_id ON public.people(tenant_id);
CREATE INDEX IF NOT EXISTS ix_people_name ON public.people(name);
CREATE INDEX IF NOT EXISTS ix_people_document ON public.people(document);

-- Índices para clients
CREATE INDEX IF NOT EXISTS ix_clients_tenant_id ON public.clients(tenant_id);
CREATE INDEX IF NOT EXISTS ix_clients_code ON public.clients(code);
CREATE INDEX IF NOT EXISTS ix_clients_person_id ON public.clients(person_id);

-- Índices para suppliers
CREATE INDEX IF NOT EXISTS ix_suppliers_tenant_id ON public.suppliers(tenant_id);
CREATE INDEX IF NOT EXISTS ix_suppliers_code ON public.suppliers(code);
CREATE INDEX IF NOT EXISTS ix_suppliers_person_id ON public.suppliers(person_id);

-- Índices para employees
CREATE INDEX IF NOT EXISTS ix_employees_tenant_id ON public.employees(tenant_id);
CREATE INDEX IF NOT EXISTS ix_employees_registration ON public.employees(registration);
CREATE INDEX IF NOT EXISTS ix_employees_branch_id ON public.employees(branch_id);

-- Índices para products
CREATE INDEX IF NOT EXISTS ix_products_tenant_id ON public.products(tenant_id);
CREATE INDEX IF NOT EXISTS ix_products_code ON public.products(code);
CREATE INDEX IF NOT EXISTS ix_products_barcode ON public.products(barcode);
CREATE INDEX IF NOT EXISTS ix_products_category_id ON public.products(category_id);

-- Índices para inventories
CREATE INDEX IF NOT EXISTS ix_inventories_tenant_id ON public.inventories(tenant_id);
CREATE INDEX IF NOT EXISTS ix_inventories_branch_id ON public.inventories(branch_id);
CREATE INDEX IF NOT EXISTS ix_inventories_product_id ON public.inventories(product_id);

-- Índices para sales
CREATE INDEX IF NOT EXISTS ix_sales_tenant_id ON public.sales(tenant_id);
CREATE INDEX IF NOT EXISTS ix_sales_branch_id ON public.sales(branch_id);
CREATE INDEX IF NOT EXISTS ix_sales_number ON public.sales(number);
CREATE INDEX IF NOT EXISTS ix_sales_client_id ON public.sales(client_id);
CREATE INDEX IF NOT EXISTS ix_sales_date ON public.sales(sale_date);
CREATE INDEX IF NOT EXISTS ix_sales_status ON public.sales(status);

-- Índices para purchases
CREATE INDEX IF NOT EXISTS ix_purchases_tenant_id ON public.purchases(tenant_id);
CREATE INDEX IF NOT EXISTS ix_purchases_branch_id ON public.purchases(branch_id);
CREATE INDEX IF NOT EXISTS ix_purchases_number ON public.purchases(number);
CREATE INDEX IF NOT EXISTS ix_purchases_supplier_id ON public.purchases(supplier_id);
CREATE INDEX IF NOT EXISTS ix_purchases_date ON public.purchases(purchase_date);
CREATE INDEX IF NOT EXISTS ix_purchases_status ON public.purchases(status);

-- Índices para accounts_payable
CREATE INDEX IF NOT EXISTS ix_accounts_payable_tenant_id ON public.accounts_payable(tenant_id);
CREATE INDEX IF NOT EXISTS ix_accounts_payable_supplier_id ON public.accounts_payable(supplier_id);
CREATE INDEX IF NOT EXISTS ix_accounts_payable_due_date ON public.accounts_payable(due_date);
CREATE INDEX IF NOT EXISTS ix_accounts_payable_status ON public.accounts_payable(status);

-- Índices para accounts_receivable
CREATE INDEX IF NOT EXISTS ix_accounts_receivable_tenant_id ON public.accounts_receivable(tenant_id);
CREATE INDEX IF NOT EXISTS ix_accounts_receivable_client_id ON public.accounts_receivable(client_id);
CREATE INDEX IF NOT EXISTS ix_accounts_receivable_due_date ON public.accounts_receivable(due_date);
CREATE INDEX IF NOT EXISTS ix_accounts_receivable_status ON public.accounts_receivable(status);

-- Índices para financial_entries
CREATE INDEX IF NOT EXISTS ix_financial_entries_tenant_id ON public.financial_entries(tenant_id);
CREATE INDEX IF NOT EXISTS ix_financial_entries_entry_date ON public.financial_entries(entry_date);
CREATE INDEX IF NOT EXISTS ix_financial_entries_entry_type ON public.financial_entries(entry_type);
CREATE INDEX IF NOT EXISTS ix_financial_entries_status ON public.financial_entries(status);

-- Índices para fiscal_notes
CREATE INDEX IF NOT EXISTS ix_fiscal_notes_tenant_id ON public.fiscal_notes(tenant_id);
CREATE INDEX IF NOT EXISTS ix_fiscal_notes_branch_id ON public.fiscal_notes(branch_id);
CREATE INDEX IF NOT EXISTS ix_fiscal_notes_number ON public.fiscal_notes(number);
CREATE INDEX IF NOT EXISTS ix_fiscal_notes_issue_date ON public.fiscal_notes(issue_date);


-- 4) Plano inicial e dois tenants de teste
-- Execute depois de criar as tabelas master e tenant.
-- Este script não contém senhas nem connection strings.

INSERT INTO public.plans (
    id, name, code, description, monthly_price, yearly_price, trial_days,
    max_users, max_filials, max_storage, included_modules, included_features,
    is_active, is_default, display_order
) VALUES (
    '550e8400-e29b-41d4-a716-446655440000',
    'Basic',
    'basic',
    'Plano inicial para testes do ERP SaaS',
    0.00,
    0.00,
    30,
    5,
    1,
    1073741824,
    '{"sales","purchases","inventory","financial"}',
    '{"basic_reports"}',
    TRUE,
    TRUE,
    1
)
ON CONFLICT (code) DO UPDATE SET
    name = EXCLUDED.name,
    description = EXCLUDED.description,
    is_active = TRUE,
    is_default = TRUE,
    updated_at = CURRENT_TIMESTAMP;

INSERT INTO public.tenants (
    id, name, cnpj, email, phone, status, db_name, conn_string,
    max_users, max_storage, plan_id, modules, settings
) VALUES
(
    '11111111-1111-1111-1111-111111111111',
    'Empresa A - Teste',
    '11.111.111/0001-11',
    'empresa-a@example.com',
    '(11) 1111-1111',
    'Active',
    'shared_database_empresa_a',
    '',
    5,
    1073741824,
    '550e8400-e29b-41d4-a716-446655440000',
    '{"sales","inventory"}',
    '{}'::jsonb
),
(
    '22222222-2222-2222-2222-222222222222',
    'Empresa B - Teste',
    '22.222.222/0001-22',
    'empresa-b@example.com',
    '(22) 2222-2222',
    'Active',
    'shared_database_empresa_b',
    '',
    5,
    1073741824,
    '550e8400-e29b-41d4-a716-446655440000',
    '{"sales","inventory"}',
    '{}'::jsonb
)
ON CONFLICT (id) DO UPDATE SET
    name = EXCLUDED.name,
    email = EXCLUDED.email,
    phone = EXCLUDED.phone,
    status = 'Active',
    plan_id = EXCLUDED.plan_id,
    updated_at = CURRENT_TIMESTAMP;

SELECT id, name, code, is_active FROM public.plans WHERE code = 'basic';
SELECT id, name, cnpj, status, plan_id FROM public.tenants
WHERE id IN ('11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222')
ORDER BY name;

COMMIT;

-- 5) Conferência final
SELECT id, name, code, is_active
FROM public.plans
ORDER BY display_order;

SELECT id, name, email, db_name, status, plan_id
FROM public.tenants
ORDER BY name;

SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'public'
  AND table_name IN ('users', 'roles', 'tenants', 'plans', 'people', 'clients', 'products', 'sales')
ORDER BY table_name;
