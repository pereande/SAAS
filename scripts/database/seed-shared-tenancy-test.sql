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
