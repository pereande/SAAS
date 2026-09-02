-- Script para inserir dados iniciais no banco ERP_MASTER
-- Executar após a criação do banco

-- Verificar se já há dados
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM public.plans LIMIT 1) THEN
        -- Inserir planos
        INSERT INTO public.plans (id, name, code, description, monthly_price, yearly_price, trial_days, max_users, max_filials, max_storage, included_modules, included_features, is_active, is_default, display_order) VALUES
        ('550e8400-e29b-41d4-a716-446655440000', 'Basic', 'basic', 'Plano básico para pequenas empresas', 99.00, 990.00, 30, 5, 1, 1073741824, '{"sales","purchases","inventory","financial"}', '{"basic_reports","email_support"}', true, true, 1),
        ('550e8400-e29b-41d4-a716-446655440001', 'Professional', 'pro', 'Plano profissional para empresas em crescimento', 299.00, 2990.00, 30, 20, 5, 10737418240, '{"sales","purchases","inventory","financial","fiscal","crm"}', '{"advanced_reports","priority_support","api_access"}', true, false, 2),
        ('550e8400-e29b-41d4-a716-446655440002', 'Enterprise', 'enterprise', 'Plano enterprise para grandes empresas', 999.00, 9990.00, 30, 100, 50, 107374182400, '{"sales","purchases","inventory","financial","fiscal","crm","hr","reports","dashboard","api"}', '{"custom_reports","24_7_support","dedicated_account_manager","custom_integrations"}', true, false, 3);

        RAISE NOTICE 'Planos inseridos com sucesso';
    ELSE
        RAISE NOTICE 'Planos já existem, pulando inserção';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM public.permissions LIMIT 1) THEN
        -- Inserir permissões
        INSERT INTO public.permissions (id, code, name, description, module, action) VALUES
        -- Sistema
        ('660e8400-e29b-41d4-a716-446655440000', 'tenants:read', 'View Tenants', 'Permite visualizar tenants', 'system', 'read'),
        ('660e8400-e29b-41d4-a716-446655440001', 'tenants:create', 'Create Tenants', 'Permite criar tenants', 'system', 'create'),
        ('660e8400-e29b-41d4-a716-446655440002', 'tenants:update', 'Update Tenants', 'Permite atualizar tenants', 'system', 'update'),
        ('660e8400-e29b-41d4-a716-446655440003', 'tenants:delete', 'Delete Tenants', 'Permite deletar tenants', 'system', 'delete'),
        ('660e8400-e29b-41d4-a716-446655440004', 'tenants:manage', 'Manage Tenants', 'Permite gerenciar todos os aspectos dos tenants', 'system', 'manage'),
        
        -- Usuários
        ('660e8400-e29b-41d4-a716-446655440010', 'users:read', 'View Users', 'Permite visualizar usuários', 'system', 'read'),
        ('660e8400-e29b-41d4-a716-446655440011', 'users:create', 'Create Users', 'Permite criar usuários', 'system', 'create'),
        ('660e8400-e29b-41d4-a716-446655440012', 'users:update', 'Update Users', 'Permite atualizar usuários', 'system', 'update'),
        ('660e8400-e29b-41d4-a716-446655440013', 'users:delete', 'Delete Users', 'Permite deletar usuários', 'system', 'delete'),
        ('660e8400-e29b-41d4-a716-446655440014', 'users:manage', 'Manage Users', 'Permite gerenciar todos os aspectos dos usuários', 'system', 'manage'),
        
        -- Perfis
        ('660e8400-e29b-41d4-a716-446655440020', 'roles:read', 'View Roles', 'Permite visualizar perfis', 'system', 'read'),
        ('660e8400-e29b-41d4-a716-446655440021', 'roles:create', 'Create Roles', 'Permite criar perfis', 'system', 'create'),
        ('660e8400-e29b-41d4-a716-446655440022', 'roles:update', 'Update Roles', 'Permite atualizar perfis', 'system', 'update'),
        ('660e8400-e29b-41d4-a716-446655440023', 'roles:delete', 'Delete Roles', 'Permite deletar perfis', 'system', 'delete'),
        ('660e8400-e29b-41d4-a716-446655440024', 'roles:manage', 'Manage Roles', 'Permite gerenciar todos os aspectos dos perfis', 'system', 'manage'),
        
        -- Estoque
        ('660e8400-e29b-41d4-a716-446655440100', 'products:read', 'View Products', 'Permite visualizar produtos', 'inventory', 'read'),
        ('660e8400-e29b-41d4-a716-446655440101', 'products:create', 'Create Products', 'Permite criar produtos', 'inventory', 'create'),
        ('660e8400-e29b-41d4-a716-446655440102', 'products:update', 'Update Products', 'Permite atualizar produtos', 'inventory', 'update'),
        ('660e8400-e29b-41d4-a716-446655440103', 'products:delete', 'Delete Products', 'Permite deletar produtos', 'inventory', 'delete'),
        ('660e8400-e29b-41d4-a716-446655440104', 'products:manage', 'Manage Products', 'Permite gerenciar todos os aspectos dos produtos', 'inventory', 'manage'),
        
        ('660e8400-e29b-41d4-a716-446655440110', 'inventory:read', 'View Inventory', 'Permite visualizar estoque', 'inventory', 'read'),
        ('660e8400-e29b-41d4-a716-446655440111', 'inventory:update', 'Update Inventory', 'Permite atualizar estoque', 'inventory', 'update'),
        ('660e8400-e29b-41d4-a716-446655440112', 'inventory:manage', 'Manage Inventory', 'Permite gerenciar estoque', 'inventory', 'manage'),
        
        -- Vendas
        ('660e8400-e29b-41d4-a716-446655440200', 'sales:read', 'View Sales', 'Permite visualizar vendas', 'sales', 'read'),
        ('660e8400-e29b-41d4-a716-446655440201', 'sales:create', 'Create Sales', 'Permite criar vendas', 'sales', 'create'),
        ('660e8400-e29b-41d4-a716-446655440202', 'sales:update', 'Update Sales', 'Permite atualizar vendas', 'sales', 'update'),
        ('660e8400-e29b-41d4-a716-446655440203', 'sales:delete', 'Delete Sales', 'Permite deletar vendas', 'sales', 'delete'),
        ('660e8400-e29b-41d4-a716-446655440204', 'sales:manage', 'Manage Sales', 'Permite gerenciar todos os aspectos das vendas', 'sales', 'manage'),
        ('660e8400-e29b-41d4-a716-446655440205', 'sales:cancel', 'Cancel Sales', 'Permite cancelar vendas', 'sales', 'cancel'),
        ('660e8400-e29b-41d4-a716-446655440206', 'sales:confirm', 'Confirm Sales', 'Permite confirmar vendas', 'sales', 'confirm'),
        
        -- Compras
        ('660e8400-e29b-41d4-a716-446655440300', 'purchases:read', 'View Purchases', 'Permite visualizar compras', 'purchases', 'read'),
        ('660e8400-e29b-41d4-a716-446655440301', 'purchases:create', 'Create Purchases', 'Permite criar compras', 'purchases', 'create'),
        ('660e8400-e29b-41d4-a716-446655440302', 'purchases:update', 'Update Purchases', 'Permite atualizar compras', 'purchases', 'update'),
        ('660e8400-e29b-41d4-a716-446655440303', 'purchases:delete', 'Delete Purchases', 'Permite deletar compras', 'purchases', 'delete'),
        ('660e8400-e29b-41d4-a716-446655440304', 'purchases:manage', 'Manage Purchases', 'Permite gerenciar todos os aspectos das compras', 'purchases', 'manage'),
        ('660e8400-e29b-41d4-a716-446655440305', 'purchases:cancel', 'Cancel Purchases', 'Permite cancelar compras', 'purchases', 'cancel'),
        ('660e8400-e29b-41d4-a716-446655440306', 'purchases:confirm', 'Confirm Purchases', 'Permite confirmar compras', 'purchases', 'confirm'),
        ('660e8400-e29b-41d4-a716-446655440307', 'purchases:receive', 'Receive Purchases', 'Permite receber compras', 'purchases', 'receive'),
        
        -- Financeiro
        ('660e8400-e29b-41d4-a716-446655440400', 'accounts_payable:read', 'View Accounts Payable', 'Permite visualizar contas a pagar', 'financial', 'read'),
        ('660e8400-e29b-41d4-a716-446655440401', 'accounts_payable:create', 'Create Accounts Payable', 'Permite criar contas a pagar', 'financial', 'create'),
        ('660e8400-e29b-41d4-a716-446655440402', 'accounts_payable:update', 'Update Accounts Payable', 'Permite atualizar contas a pagar', 'financial', 'update'),
        ('660e8400-e29b-41d4-a716-446655440403', 'accounts_payable:delete', 'Delete Accounts Payable', 'Permite deletar contas a pagar', 'financial', 'delete'),
        ('660e8400-e29b-41d4-a716-446655440404', 'accounts_payable:pay', 'Pay Accounts Payable', 'Permite pagar contas a pagar', 'financial', 'pay'),
        
        ('660e8400-e29b-41d4-a716-446655440410', 'accounts_receivable:read', 'View Accounts Receivable', 'Permite visualizar contas a receber', 'financial', 'read'),
        ('660e8400-e29b-41d4-a716-446655440411', 'accounts_receivable:create', 'Create Accounts Receivable', 'Permite criar contas a receber', 'financial', 'create'),
        ('660e8400-e29b-41d4-a716-446655440412', 'accounts_receivable:update', 'Update Accounts Receivable', 'Permite atualizar contas a receber', 'financial', 'update'),
        ('660e8400-e29b-41d4-a716-446655440413', 'accounts_receivable:delete', 'Delete Accounts Receivable', 'Permite deletar contas a receber', 'financial', 'delete'),
        ('660e8400-e29b-41d4-a716-446655440414', 'accounts_receivable:receive', 'Receive Accounts Receivable', 'Permite receber contas a receber', 'financial', 'receive'),
        
        ('660e8400-e29b-41d4-a716-446655440420', 'financial_entries:read', 'View Financial Entries', 'Permite visualizar lançamentos financeiros', 'financial', 'read'),
        ('660e8400-e29b-41d4-a716-446655440421', 'financial_entries:create', 'Create Financial Entries', 'Permite criar lançamentos financeiros', 'financial', 'create'),
        ('660e8400-e29b-41d4-a716-446655440422', 'financial_entries:update', 'Update Financial Entries', 'Permite atualizar lançamentos financeiros', 'financial', 'update'),
        ('660e8400-e29b-41d4-a716-446655440423', 'financial_entries:delete', 'Delete Financial Entries', 'Permite deletar lançamentos financeiros', 'financial', 'delete'),
        ('660e8400-e29b-41d4-a716-446655440424', 'financial_entries:reconcile', 'Reconcile Financial Entries', 'Permite conciliar lançamentos financeiros', 'financial', 'reconcile'),
        
        -- Fiscal
        ('660e8400-e29b-41d4-a716-446655440500', 'fiscal_notes:read', 'View Fiscal Notes', 'Permite visualizar notas fiscais', 'fiscal', 'read'),
        ('660e8400-e29b-41d4-a716-446655440501', 'fiscal_notes:create', 'Create Fiscal Notes', 'Permite criar notas fiscais', 'fiscal', 'create'),
        ('660e8400-e29b-41d4-a716-446655440502', 'fiscal_notes:update', 'Update Fiscal Notes', 'Permite atualizar notas fiscais', 'fiscal', 'update'),
        ('660e8400-e29b-41d4-a716-446655440503', 'fiscal_notes:delete', 'Delete Fiscal Notes', 'Permite deletar notas fiscais', 'fiscal', 'delete'),
        ('660e8400-e29b-41d4-a716-446655440504', 'fiscal_notes:authorize', 'Authorize Fiscal Notes', 'Permite autorizar notas fiscais', 'fiscal', 'authorize'),
        ('660e8400-e29b-41d4-a716-446655440505', 'fiscal_notes:cancel', 'Cancel Fiscal Notes', 'Permite cancelar notas fiscais', 'fiscal', 'cancel'),
        
        ('660e8400-e29b-41d4-a716-446655440510', 'fiscal_configurations:read', 'View Fiscal Configurations', 'Permite visualizar configurações fiscais', 'fiscal', 'read'),
        ('660e8400-e29b-41d4-a716-446655440511', 'fiscal_configurations:update', 'Update Fiscal Configurations', 'Permite atualizar configurações fiscais', 'fiscal', 'update'),
        
        -- Relatórios
        ('660e8400-e29b-41d4-a716-446655440600', 'reports:read', 'View Reports', 'Permite visualizar relatórios', 'reports', 'read'),
        ('660e8400-e29b-41d4-a716-446655440601', 'reports:custom', 'Custom Reports', 'Permite criar relatórios personalizados', 'reports', 'custom'),
        
        -- Dashboard
        ('660e8400-e29b-41d4-a716-446655440700', 'dashboard:read', 'View Dashboard', 'Permite visualizar dashboard', 'dashboard', 'read');

        RAISE NOTICE 'Permissões inseridas com sucesso';
    ELSE
        RAISE NOTICE 'Permissões já existem, pulando inserção';
    END IF;

    -- Criar perfil Admin (se não existir)
    IF NOT EXISTS (SELECT 1 FROM public.roles WHERE name = 'Admin') THEN
        INSERT INTO public.roles (id, name, normalized_name, description, is_system, is_active, concurrency_stamp) VALUES
        ('770e8400-e29b-41d4-a716-446655440000', 'Admin', 'ADMIN', 'Administrador do sistema', true, true, '00000000-0000-0000-0000-000000000000');

        RAISE NOTICE 'Perfil Admin criado';
    END IF;

    -- Criar perfil TenantAdmin (se não existir)
    IF NOT EXISTS (SELECT 1 FROM public.roles WHERE name = 'TenantAdmin') THEN
        INSERT INTO public.roles (id, name, normalized_name, description, is_system, is_active, concurrency_stamp) VALUES
        ('770e8400-e29b-41d4-a716-446655440001', 'TenantAdmin', 'TENANTADMIN', 'Administrador do tenant', true, true, '00000000-0000-0000-0000-000000000000');

        RAISE NOTICE 'Perfil TenantAdmin criado';
    END IF;

    -- Criar perfil User (se não existir)
    IF NOT EXISTS (SELECT 1 FROM public.roles WHERE name = 'User') THEN
        INSERT INTO public.roles (id, name, normalized_name, description, is_system, is_active, concurrency_stamp) VALUES
        ('770e8400-e29b-41d4-a716-446655440002', 'User', 'USER', 'Usuário comum', true, true, '00000000-0000-0000-0000-000000000000');

        RAISE NOTICE 'Perfil User criado';
    END IF;

    -- Associar permissões ao perfil Admin
    -- (Isso deve ser feito via API ou manualmente)

EXCEPTION WHEN OTHERS THEN
    RAISE NOTICE 'Erro ao inserir dados iniciais: %', SQLERRM;
END $$;

-- Criar função para verificar se um usuário tem uma permissão
CREATE OR REPLACE FUNCTION public.has_permission(
    p_user_id UUID,
    p_permission_code VARCHAR(100)
) RETURNS BOOLEAN AS $$
DECLARE
    v_has_permission BOOLEAN DEFAULT FALSE;
BEGIN
    SELECT EXISTS (
        SELECT 1 FROM public.role_permissions rp
        JOIN public.user_roles ur ON rp.role_id = ur.role_id
        JOIN public.permissions p ON rp.permission_id = p.id
        WHERE ur.user_id = p_user_id AND p.code = p_permission_code
    ) INTO v_has_permission;

    RETURN v_has_permission;
END;
$$ LANGUAGE plpgsql;

-- Criar função para verificar se um usuário tem um role
CREATE OR REPLACE FUNCTION public.has_role(
    p_user_id UUID,
    p_role_name VARCHAR(255)
) RETURNS BOOLEAN AS $$
DECLARE
    v_has_role BOOLEAN DEFAULT FALSE;
BEGIN
    SELECT EXISTS (
        SELECT 1 FROM public.user_roles ur
        JOIN public.roles r ON ur.role_id = r.id
        WHERE ur.user_id = p_user_id AND r.name = p_role_name
    ) INTO v_has_role;

    RETURN v_has_role;
END;
$$ LANGUAGE plpgsql;
