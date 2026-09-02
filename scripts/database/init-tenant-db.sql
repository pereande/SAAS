-- Script para inicializar um banco de dados de tenant
-- PostgreSQL
-- Substituir {TENANT_DB_NAME} pelo nome do banco do tenant

-- Criar banco de dados (executar uma vez por tenant)
-- CREATE DATABASE erp_tenant_{tenant_id}
--     WITH
--     OWNER = postgres
--     ENCODING = 'UTF8'
--     LC_COLLATE = 'en_US.utf8'
--     LC_CTYPE = 'en_US.utf8'
--     TABLESPACE = pg_default
--     CONNECTION LIMIT = -1;

-- Conectar ao banco do tenant
-- \c erp_tenant_{tenant_id}

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
    sale_id UUID REFERENCES public.sales(id) ON DELETE SET NULL,
    purchase_id UUID REFERENCES public.purchases(id) ON DELETE SET NULL,
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
    fiscal_note_id UUID REFERENCES public.fiscal_notes(id) ON DELETE SET NULL,
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
    fiscal_note_id UUID REFERENCES public.fiscal_notes(id) ON DELETE SET NULL,
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
    purchase_id UUID REFERENCES public.purchases(id) ON DELETE SET NULL,
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
    sale_id UUID REFERENCES public.sales(id) ON DELETE SET NULL,
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
    sale_id UUID REFERENCES public.sales(id) ON DELETE SET NULL,
    purchase_id UUID REFERENCES public.purchases(id) ON DELETE SET NULL,
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

-- Grant permissões
GRANT ALL PRIVILEGES ON DATABASE erp_tenant_{tenant_id} TO postgres;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO postgres;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO postgres;
