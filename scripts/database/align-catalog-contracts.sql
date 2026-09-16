-- ERP SaaS - alinhamento dos contratos de clientes e produtos
-- Seguro para executar em um banco já instalado.

ALTER TABLE public.clients
    ADD COLUMN IF NOT EXISTS company_name VARCHAR(200);

-- category e stock já são persistidos por relacionamento:
-- products.category_id -> product_categories.id
-- inventories.product_id -> products.id
-- inventories.quantity -> estoque atual

SELECT column_name, data_type
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name = 'clients'
  AND column_name = 'company_name';
