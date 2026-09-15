-- ERP SaaS - tenant inicial para o banco compartilhado
-- Execute no SQL Editor do Supabase depois de substituir o valor abaixo.
-- Use exatamente o mesmo UUID configurado em VITE_TENANT_ID no frontend e em Railway.

BEGIN;

INSERT INTO public.tenants (
    id,
    name,
    cnpj,
    email,
    phone,
    status,
    db_name,
    conn_string,
    current_storage
)
VALUES (
    '11111111-1111-1111-1111-111111111111',
    'Casa Nativa',
    '00.000.000/0001-91',
    'admin@erpsaas.com',
    '',
    'Active',
    'shared',
    'managed-shared-database',
    0
)
ON CONFLICT (id) DO UPDATE SET
    name = EXCLUDED.name,
    email = EXCLUDED.email,
    phone = EXCLUDED.phone,
    status = 'Active',
    updated_at = CURRENT_TIMESTAMP;

COMMIT;

-- Confirmação: deve retornar uma linha com status Active.
SELECT id, name, status
FROM public.tenants
WHERE id = '11111111-1111-1111-1111-111111111111';
