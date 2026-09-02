-- Script para inicializar o banco ERP_MASTER
-- PostgreSQL

-- Criar banco de dados
CREATE DATABASE erp_master
    WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.utf8'
    LC_CTYPE = 'en_US.utf8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

-- Conectar ao banco
\c erp_master

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

-- Tabela: role_permissions
CREATE TABLE IF NOT EXISTS public.role_permissions (
    id UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),
    role_id UUID NOT NULL,
    permission_id UUID NOT NULL,
    granted_by UUID,
    granted_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (role_id) REFERENCES public.roles(id) ON DELETE CASCADE,
    FOREIGN KEY (permission_id) REFERENCES public.permissions(id) ON DELETE CASCADE
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
COMMENT ON TABLE public.role_permissions IS 'Tabela de relacionamento entre perfis e permissões';
COMMENT ON TABLE public.audit_logs IS 'Tabela de logs de auditoria';
COMMENT ON TABLE public.login_attempts IS 'Tabela de tentativas de login';
COMMENT ON TABLE public.user_sessions IS 'Tabela de sessões de usuário';
COMMENT ON TABLE public.user_tokens IS 'Tabela de tokens de usuário (refresh tokens, etc.)';

-- Grant permissões
GRANT ALL PRIVILEGES ON DATABASE erp_master TO postgres;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO postgres;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO postgres;
