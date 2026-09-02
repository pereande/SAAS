/* =============================================================
   MATRIZ · camada de domínio do protótipo (Fase 1)
   Dados simulados que espelham a arquitetura real descrita no
   Blueprint §47 — nada aqui substitui a validação de backend.
   ============================================================= */

export type ViewId =
  | "dashboard"
  | "tenants"
  | "users"
  | "permissions"
  | "audit"
  | "logs"
  | "blueprint"
  | "roadmap";

/* ---------------- formatação ---------------- */
export const fmtBRL = (n: number) =>
  n.toLocaleString("pt-BR", { style: "currency", currency: "BRL", maximumFractionDigits: 0 });
export const fmtBRL2 = (n: number) =>
  n.toLocaleString("pt-BR", { style: "currency", currency: "BRL", minimumFractionDigits: 2 });
export const fmtInt = (n: number) => n.toLocaleString("pt-BR");
export const fmtCompact = (n: number) =>
  n >= 1_000_000 ? `${(n / 1_000_000).toLocaleString("pt-BR", { maximumFractionDigits: 1 })} mi` :
  n >= 1_000 ? `${(n / 1_000).toLocaleString("pt-BR", { maximumFractionDigits: 1 })} mil` :
  fmtInt(n);

/* ---------------- navegação ---------------- */
export interface NavItem { label: string; view?: ViewId; phase?: number; icon: string; modulo?: string }
export interface NavGroup { title: string; items: NavItem[] }

export const NAV: NavGroup[] = [
  { title: "Operação", items: [{ label: "Dashboard", view: "dashboard", icon: "dash" }] },
  {
    title: "Cadastros · Fase 2",
    items: [
      { label: "Pessoas", phase: 2, icon: "users" },
      { label: "Clientes", phase: 2, icon: "cart", modulo: "Cadastros" },
      { label: "Fornecedores", phase: 2, icon: "truck" },
      { label: "Funcionários", phase: 2, icon: "build" },
      { label: "Produtos", phase: 2, icon: "box" },
    ],
  },
  {
    title: "Comercial · Fase 3",
    items: [
      { label: "Orçamentos", phase: 3, icon: "doc" },
      { label: "Pedidos", phase: 3, icon: "cart" },
      { label: "Vendas", phase: 3, icon: "coin", modulo: "Vendas" },
      { label: "Compras", phase: 3, icon: "truck" },
      { label: "Estoque", phase: 3, icon: "box", modulo: "Estoque" },
    ],
  },
  {
    title: "Financeiro · Fase 4",
    items: [
      { label: "Contas a receber", phase: 4, icon: "coin", modulo: "Financeiro" },
      { label: "Contas a pagar", phase: 4, icon: "coin" },
      { label: "Caixa", phase: 4, icon: "coin" },
      { label: "Bancos", phase: 4, icon: "db" },
    ],
  },
  {
    title: "Fiscal · Fase 5",
    items: [
      { label: "Documentos fiscais", phase: 5, icon: "seal", modulo: "Fiscal" },
      { label: "Certificados digitais", phase: 5, icon: "key" },
    ],
  },
  {
    title: "Inteligência · Fase 6",
    items: [
      { label: "Relatórios", phase: 6, icon: "doc" },
      { label: "BI", phase: 6, icon: "layers" },
    ],
  },
  {
    title: "Administração",
    items: [
      { label: "Empresas & Bancos", view: "tenants", icon: "db" },
      { label: "Usuários", view: "users", icon: "users" },
      { label: "Permissões", view: "permissions", icon: "shield" },
      { label: "Auditoria", view: "audit", icon: "audit" },
      { label: "Logs", view: "logs", icon: "terminal" },
    ],
  },
  {
    title: "Arquitetura",
    items: [
      { label: "Blueprint §47", view: "blueprint", icon: "compass" },
      { label: "Roadmap", view: "roadmap", icon: "route" },
    ],
  },
];

/* ---------------- perfis & permissões (RBAC) ---------------- */
export const PROFILES = ["Administrador", "Gerente", "Vendedor", "Financeiro", "Fiscal"] as const;

export interface ModuloPerms { mod: string; actions: string[] }
export const MODULES_PERMS: ModuloPerms[] = [
  { mod: "Clientes", actions: ["Visualizar", "Criar", "Editar", "Excluir", "Exportar"] },
  { mod: "Vendas", actions: ["Visualizar", "Criar", "Editar", "Cancelar", "Aprovar", "Conceder desconto", "Alterar preço", "Reabrir"] },
  { mod: "Financeiro", actions: ["Visualizar", "Lançar", "Alterar", "Baixar", "Estornar"] },
  { mod: "Fiscal", actions: ["Emitir", "Cancelar", "Inutilizar", "Consultar"] },
  { mod: "Estoque", actions: ["Visualizar", "Movimentar", "Ajustar", "Transferir", "Inventariar"] },
  { mod: "Compras", actions: ["Visualizar", "Criar", "Receber", "Aprovar"] },
  { mod: "Relatórios", actions: ["Visualizar", "Exportar"] },
];

/* bits na ordem: Administrador | Gerente | Vendedor | Financeiro | Fiscal */
export const ACCESS_DEFAULT: Record<string, string> = {
  "Clientes.Visualizar": "11111", "Clientes.Criar": "11100", "Clientes.Editar": "11100", "Clientes.Excluir": "11000", "Clientes.Exportar": "11111",
  "Vendas.Visualizar": "11111", "Vendas.Criar": "11100", "Vendas.Editar": "11100", "Vendas.Cancelar": "11000", "Vendas.Aprovar": "11000",
  "Vendas.Conceder desconto": "11100", "Vendas.Alterar preço": "11000", "Vendas.Reabrir": "10000",
  "Financeiro.Visualizar": "11011", "Financeiro.Lançar": "10010", "Financeiro.Alterar": "10010", "Financeiro.Baixar": "10010", "Financeiro.Estornar": "10000",
  "Fiscal.Emitir": "10001", "Fiscal.Cancelar": "10001", "Fiscal.Inutilizar": "10001", "Fiscal.Consultar": "11011",
  "Estoque.Visualizar": "11110", "Estoque.Movimentar": "11000", "Estoque.Ajustar": "11000", "Estoque.Transferir": "11000", "Estoque.Inventariar": "11000",
  "Compras.Visualizar": "11010", "Compras.Criar": "11000", "Compras.Receber": "11000", "Compras.Aprovar": "11000",
  "Relatórios.Visualizar": "11111", "Relatórios.Exportar": "11110",
};

/* ---------------- entidades ---------------- */
export interface Filial { id: string; codigo: string; nome: string; cnpj: string; cidade: string; uf: string }
export interface Sessao { disp: string; local: string; ip: string; ativa: boolean }
export interface UserRec {
  id: string; nome: string; email: string; perfil: string;
  status: "ativo" | "suspenso" | "convidado";
  filiais: string[]; mfa: boolean; lastAccess: string; sessoes: Sessao[];
}
export interface BackupRec { ts: string; tipo: string; size: string; ok: boolean }
export interface QueueJob { job: string; status: "rodando" | "fila" | "ok"; ts: string }
export interface TenantData {
  daily: number[];
  kpi: { today: number; month: number; ticket: number; overdue: number; overdueCount: number; lowStock: number };
  topProducts: { nome: string; qtd: number; valor: number }[];
  topClients: { nome: string; valor: number }[];
  payMix: { label: string; valor: number; cor: string }[];
  branchPerf: { filial: string; vendas: number; ticket: number; margem: number }[];
  queue: QueueJob[];
}
export interface Tenant {
  id: string; db: string; razao: string; fantasia: string; cnpj: string;
  plan: "BÁSICO" | "PROFISSIONAL" | "EMPRESARIAL" | "ENTERPRISE";
  status: "ativa" | "trial" | "suspensa";
  since: string; modules: string[]; filiais: Filial[]; users: UserRec[];
  storageMb: number; schemaV: number; server: string; backups: BackupRec[]; data: TenantData;
}

const PAY = { pix: "#16a699", cartao: "#f2a516", boleto: "#7290a9", credito: "#2b5580" };

export const TENANTS: Tenant[] = [
  {
    id: "T-0001", db: "ERP_EMPRESA_000001", razao: "Comercial Aurora Ltda", fantasia: "Aurora Distribuidora",
    cnpj: "12.345.678/0001-90", plan: "EMPRESARIAL", status: "ativa", since: "12/03/2025",
    modules: ["Cadastros", "Vendas", "Estoque", "Financeiro"],
    server: "sql-cluster-01.interno", storageMb: 4820, schemaV: 14,
    filiais: [
      { id: "F-01", codigo: "MATRIZ", nome: "Matriz — São Paulo", cnpj: "12.345.678/0001-90", cidade: "São Paulo", uf: "SP" },
      { id: "F-02", codigo: "FIL-01", nome: "Filial Campinas", cnpj: "12.345.678/0002-71", cidade: "Campinas", uf: "SP" },
    ],
    users: [
      { id: "U-01", nome: "Anderson Souza", email: "anderson@aurora.com.br", perfil: "Administrador", status: "ativo", filiais: ["F-01", "F-02"], mfa: true, lastAccess: "hoje · 09:12", sessoes: [{ disp: "Chrome · Windows 11", local: "São Paulo, BR", ip: "200.147.35.18", ativa: true }, { disp: "Safari · iPhone 15", local: "São Paulo, BR", ip: "189.40.72.5", ativa: true }] },
      { id: "U-02", nome: "Mariana Lopes", email: "mariana@aurora.com.br", perfil: "Gerente", status: "ativo", filiais: ["F-01", "F-02"], mfa: true, lastAccess: "hoje · 08:47", sessoes: [{ disp: "Edge · Windows 11", local: "Campinas, BR", ip: "177.12.90.41", ativa: true }] },
      { id: "U-03", nome: "Carlos Mendes", email: "carlos@aurora.com.br", perfil: "Vendedor", status: "ativo", filiais: ["F-01"], mfa: false, lastAccess: "ontem · 18:22", sessoes: [{ disp: "Chrome · Android", local: "São Paulo, BR", ip: "200.147.35.90", ativa: false }] },
      { id: "U-04", nome: "Paula Ferraz", email: "paula@aurora.com.br", perfil: "Financeiro", status: "ativo", filiais: ["F-01", "F-02"], mfa: true, lastAccess: "hoje · 07:58", sessoes: [{ disp: "Firefox · Ubuntu", local: "São Paulo, BR", ip: "187.55.10.233", ativa: true }] },
      { id: "U-05", nome: "Ricardo Alves", email: "ricardo@aurora.com.br", perfil: "Vendedor", status: "suspenso", filiais: ["F-02"], mfa: false, lastAccess: "há 34 dias", sessoes: [] },
    ],
    backups: [
      { ts: "hoje · 02:00", tipo: "Incremental", size: "412 MB", ok: true },
      { ts: "hoje · 00:30", tipo: "Full diário", size: "4,8 GB", ok: true },
      { ts: "ontem · 00:30", tipo: "Full diário", size: "4,7 GB", ok: true },
    ],
    data: {
      daily: [18200, 21400, 19800, 24600, 22100, 26800, 29500, 23400, 25200, 27900, 31200, 28700, 33400, 35900],
      kpi: { today: 35900, month: 368100, ticket: 486, overdue: 27840, overdueCount: 9, lowStock: 8 },
      topProducts: [
        { nome: "SSD NVMe 1TB Kingston", qtd: 312, valor: 58900 },
        { nome: "Cabo HDMI 2.1 8K 2m", qtd: 1240, valor: 43400 },
        { nome: "Monitor 24\" Full HD", qtd: 168, valor: 41800 },
        { nome: "Teclado mecânico K8", qtd: 294, valor: 32100 },
        { nome: "Fonte 650W 80+ Bronze", qtd: 231, valor: 28700 },
      ],
      topClients: [
        { nome: "TechMais Varejo", valor: 61200 },
        { nome: "InfoShop Distribuição", valor: 48900 },
        { nome: "MegaByte Store", valor: 39700 },
        { nome: "Rede Connect", valor: 31400 },
      ],
      payMix: [
        { label: "PIX", valor: 169300, cor: PAY.pix },
        { label: "Cartão", valor: 103100, cor: PAY.cartao },
        { label: "Boleto", valor: 66300, cor: PAY.boleto },
        { label: "Crédito cliente", valor: 29400, cor: PAY.credito },
      ],
      branchPerf: [
        { filial: "Matriz — São Paulo", vendas: 221400, ticket: 512, margem: 31.2 },
        { filial: "Filial Campinas", vendas: 146700, ticket: 449, margem: 28.7 },
      ],
      queue: [
        { job: "Emissão NF-e 45.211 → SEFAZ", status: "rodando", ts: "há 12s" },
        { job: "Remessa de boletos → Bradesco", status: "fila", ts: "há 2min" },
        { job: "Backup incremental do banco", status: "ok", ts: "02:00" },
        { job: "Relatório mensal de vendas", status: "fila", ts: "há 9min" },
      ],
    },
  },
  {
    id: "T-0002", db: "ERP_EMPRESA_000002", razao: "Metalúrgica Vetor S.A.", fantasia: "Vetor Metais",
    cnpj: "98.765.432/0001-10", plan: "ENTERPRISE", status: "ativa", since: "04/11/2024",
    modules: ["Cadastros", "Vendas", "Estoque", "Financeiro", "Fiscal", "Compras"],
    server: "sql-cluster-01.interno", storageMb: 18930, schemaV: 14,
    filiais: [
      { id: "F-01", codigo: "MATRIZ", nome: "Matriz — Contagem", cnpj: "98.765.432/0001-10", cidade: "Contagem", uf: "MG" },
      { id: "F-02", codigo: "FIL-01", nome: "Filial Betim", cnpj: "98.765.432/0002-01", cidade: "Betim", uf: "MG" },
      { id: "F-03", codigo: "FIL-02", nome: "Filial Sorocaba", cnpj: "98.765.432/0003-84", cidade: "Sorocaba", uf: "SP" },
    ],
    users: [
      { id: "U-01", nome: "Beatriz Nunes", email: "beatriz@vetormetais.com.br", perfil: "Administrador", status: "ativo", filiais: ["F-01", "F-02", "F-03"], mfa: true, lastAccess: "hoje · 06:41", sessoes: [{ disp: "Chrome · Windows 11", local: "Belo Horizonte, BR", ip: "189.90.12.77", ativa: true }] },
      { id: "U-02", nome: "Otávio Ramos", email: "otavio@vetormetais.com.br", perfil: "Gerente", status: "ativo", filiais: ["F-01"], mfa: true, lastAccess: "hoje · 09:03", sessoes: [{ disp: "Chrome · macOS", local: "Contagem, BR", ip: "177.44.8.19", ativa: true }] },
      { id: "U-03", nome: "Sílvia Prado", email: "silvia@vetormetais.com.br", perfil: "Fiscal", status: "ativo", filiais: ["F-01", "F-02", "F-03"], mfa: true, lastAccess: "hoje · 08:15", sessoes: [{ disp: "Edge · Windows 11", local: "Contagem, BR", ip: "177.44.8.30", ativa: true }] },
      { id: "U-04", nome: "Henrique Sá", email: "henrique@vetormetais.com.br", perfil: "Financeiro", status: "ativo", filiais: ["F-01", "F-03"], mfa: false, lastAccess: "ontem · 19:40", sessoes: [{ disp: "Chrome · Windows 10", local: "Sorocaba, BR", ip: "201.20.55.8", ativa: false }] },
      { id: "U-05", nome: "Luana Castro", email: "luana@vetormetais.com.br", perfil: "Vendedor", status: "ativo", filiais: ["F-02"], mfa: false, lastAccess: "hoje · 08:56", sessoes: [{ disp: "Chrome · Android", local: "Betim, BR", ip: "45.174.30.12", ativa: true }] },
      { id: "U-06", nome: "Pedro Braga", email: "pedro.b@vetormetais.com.br", perfil: "Vendedor", status: "convidado", filiais: ["F-03"], mfa: false, lastAccess: "—", sessoes: [] },
    ],
    backups: [
      { ts: "hoje · 02:15", tipo: "Incremental", size: "1,9 GB", ok: true },
      { ts: "hoje · 00:45", tipo: "Full diário", size: "18,4 GB", ok: true },
      { ts: "ontem · 12:00", tipo: "Snapshot pré-migration", size: "18,1 GB", ok: true },
    ],
    data: {
      daily: [64000, 71000, 58000, 82000, 90000, 76000, 88000, 95000, 84000, 102000, 97000, 110000, 105000, 118000],
      kpi: { today: 118000, month: 1140000, ticket: 3940, overdue: 84200, overdueCount: 12, lowStock: 14 },
      topProducts: [
        { nome: "Chapa de aço 1/4\"", qtd: 5200, valor: 312000 },
        { nome: "Perfil U 3\" galvanizado", qtd: 3800, valor: 198000 },
        { nome: "Eletrodo 7018 (cx 20kg)", qtd: 1450, valor: 142000 },
        { nome: "Parafuso sext. M12 (milheiro)", qtd: 980, valor: 96500 },
        { nome: "Disco de corte 7\"", qtd: 2300, valor: 71300 },
      ],
      topClients: [
        { nome: "Construtora Delta", valor: 246000 },
        { nome: "Usimec Industrial", valor: 189000 },
        { nome: "Metalplan Estruturas", valor: 141000 },
        { nome: "Ferro & Cia", valor: 98700 },
      ],
      payMix: [
        { label: "Boleto", valor: 467000, cor: PAY.boleto },
        { label: "PIX", valor: 331000, cor: PAY.pix },
        { label: "Crédito cliente", valor: 228000, cor: PAY.credito },
        { label: "Transferência", valor: 114000, cor: PAY.cartao },
      ],
      branchPerf: [
        { filial: "Matriz — Contagem", vendas: 486000, ticket: 4320, margem: 26.4 },
        { filial: "Filial Betim", vendas: 371000, ticket: 3890, margem: 24.1 },
        { filial: "Filial Sorocaba", vendas: 283000, ticket: 3610, margem: 25.8 },
      ],
      queue: [
        { job: "Reprocessamento NF-e 44.902 (rejeitada)", status: "rodando", ts: "há 4s" },
        { job: "Manifestação destinatário — 38 CT-e", status: "fila", ts: "há 1min" },
        { job: "Envio DMS-e → prefeitura Betim", status: "fila", ts: "há 3min" },
        { job: "Backup incremental do banco", status: "ok", ts: "02:15" },
      ],
    },
  },
  {
    id: "T-0003", db: "ERP_EMPRESA_000003", razao: "Distribuidora Ipê Ltda", fantasia: "Ipê Atacado",
    cnpj: "45.678.912/0001-33", plan: "PROFISSIONAL", status: "trial", since: "28/08/2025",
    modules: ["Cadastros", "Vendas", "Estoque"],
    server: "sql-cluster-02.interno", storageMb: 640, schemaV: 14,
    filiais: [
      { id: "F-01", codigo: "MATRIZ", nome: "Matriz — Goiânia", cnpj: "45.678.912/0001-33", cidade: "Goiânia", uf: "GO" },
    ],
    users: [
      { id: "U-01", nome: "João Pedro Ipê", email: "joao@ipeatacado.com.br", perfil: "Administrador", status: "ativo", filiais: ["F-01"], mfa: true, lastAccess: "hoje · 10:02", sessoes: [{ disp: "Chrome · Windows 11", local: "Goiânia, BR", ip: "168.195.44.10", ativa: true }] },
      { id: "U-02", nome: "Bianca Santos", email: "bianca@ipeatacado.com.br", perfil: "Vendedor", status: "ativo", filiais: ["F-01"], mfa: false, lastAccess: "hoje · 09:48", sessões: [{ disp: "Chrome · Android", local: "Goiânia, BR", ip: "168.195.44.22", ativa: true }] },
    ],
    backups: [
      { ts: "hoje · 03:00", tipo: "Full diário", size: "640 MB", ok: true },
      { ts: "ontem · 03:00", tipo: "Full diário", size: "622 MB", ok: true },
    ],
    data: {
      daily: [4200, 3800, 5100, 4600, 5400, 6100, 5800, 6400, 6000, 6900, 7200, 6800, 7500, 8100],
      kpi: { today: 8100, month: 83900, ticket: 214, overdue: 0, overdueCount: 0, lowStock: 3 },
      topProducts: [
        { nome: "Café 500g torrado", qtd: 890, valor: 15900 },
        { nome: "Açúcar cristal 5kg", qtd: 410, valor: 12300 },
        { nome: "Óleo de soja 900ml (cx)", qtd: 360, valor: 11400 },
        { nome: "Farinha de trigo 1kg", qtd: 520, valor: 8900 },
      ],
      topClients: [
        { nome: "Mercadão Central", valor: 18400 },
        { nome: "Empório Goiás", valor: 12700 },
        { nome: "Bar do Cerrado", valor: 9800 },
      ],
      payMix: [
        { label: "PIX", valor: 54600, cor: PAY.pix },
        { label: "Cartão", valor: 17600, cor: PAY.cartao },
        { label: "Dinheiro", valor: 11700, cor: PAY.boleto },
      ],
      branchPerf: [{ filial: "Matriz — Goiânia", vendas: 83900, ticket: 214, margem: 22.9 }],
      queue: [
        { job: "Importação CSV de produtos (1.240)", status: "rodando", ts: "há 31s" },
        { job: "Backup diário do banco", status: "ok", ts: "03:00" },
      ],
    },
  },
];

/* ---------------- auditoria ---------------- */
export interface AuditRec {
  id: string; ts: string; user: string; modulo: string; acao: string; registro: string;
  antes?: string; depois?: string; ip: string; sev: "info" | "warn" | "crit";
}
export const AUDIT: Record<string, AuditRec[]> = {
  "T-0001": [
    { id: "A-90211", ts: "01/09/2026 14:32", user: "Anderson Souza", modulo: "Produtos", acao: "Alterou preço de venda", registro: "SSD NVMe 1TB Kingston", antes: "R$ 100,00", depois: "R$ 120,00", ip: "200.147.35.18", sev: "crit" },
    { id: "A-90209", ts: "01/09/2026 11:47", user: "Mariana Lopes", modulo: "Clientes", acao: "Alterou limite de crédito", registro: "TechMais Varejo", antes: "R$ 40.000,00", depois: "R$ 60.000,00", ip: "177.12.90.41", sev: "warn" },
    { id: "A-90204", ts: "01/09/2026 09:15", user: "Paula Ferraz", modulo: "Financeiro", acao: "Baixou título com desconto", registro: "Fatura 8.341 · InfoShop", antes: "R$ 12.480,00 em aberto", depois: "Baixado · R$ 12.105,60", ip: "187.55.10.233", sev: "info" },
    { id: "A-90198", ts: "31/08/2026 17:58", user: "Anderson Souza", modulo: "Permissões", acao: "Revogou permissão", registro: "Vendedor · Vendas.Reabrir", antes: "Permitido", depois: "Negado", ip: "200.147.35.18", sev: "crit" },
    { id: "A-90190", ts: "31/08/2026 16:20", user: "Carlos Mendes", modulo: "Vendas", acao: "Concedeu desconto acima do padrão", registro: "Pedido 22.114 · MegaByte", antes: "0%", depois: "8%", ip: "200.147.35.90", sev: "warn" },
    { id: "A-90182", ts: "31/08/2026 09:02", user: "sistema", modulo: "Segurança", acao: "Bloqueou login por força bruta", registro: "login: ricardo@aurora… · 6 tentativas", ip: "45.183.102.9", sev: "crit" },
    { id: "A-90177", ts: "30/08/2026 19:44", user: "Anderson Souza", modulo: "Usuários", acao: "Suspendeu usuário", registro: "Ricardo Alves (U-05)", antes: "Ativo", depois: "Suspenso", ip: "200.147.35.18", sev: "warn" },
  ],
  "T-0002": [
    { id: "A-88120", ts: "01/09/2026 13:05", user: "Sílvia Prado", modulo: "Fiscal", acao: "Cancelou NF-e", registro: "NF-e 44.871 · Construtora Delta", antes: "Autorizada", depois: "Cancelada (evento 110111)", ip: "177.44.8.30", sev: "crit" },
    { id: "A-88116", ts: "01/09/2026 10:22", user: "Otávio Ramos", modulo: "Compras", acao: "Aprovou pedido de compra", registro: "PC 5.512 · Usimec", antes: "R$ 86.400,00", depois: "Aprovado", ip: "177.44.8.19", sev: "info" },
    { id: "A-88109", ts: "01/09/2026 08:40", user: "Henrique Sá", modulo: "Financeiro", acao: "Estornou baixa de título", registro: "Fatura 12.902 · Ferro & Cia", antes: "Baixado", depois: "Em aberto (estorno)", ip: "201.20.55.8", sev: "warn" },
    { id: "A-88101", ts: "31/08/2026 15:12", user: "Beatriz Nunes", modulo: "Estoque", acao: "Transferiu saldo entre filiais", registro: "Chapa 1/4\" · 1.200 un", antes: "Matriz", depois: "Filial Sorocaba", ip: "189.90.12.77", sev: "info" },
    { id: "A-88094", ts: "31/08/2026 11:03", user: "Sílvia Prado", modulo: "Fiscal", acao: "Inutilizou numeração", registro: "NF-e série 4 · 44.890–44.893", antes: "Sequência livre", depois: "Inutilizada (evento 110111)", ip: "177.44.8.30", sev: "warn" },
  ],
  "T-0003": [
    { id: "A-77020", ts: "01/09/2026 09:51", user: "João Pedro Ipê", modulo: "Produtos", acao: "Importou catálogo via CSV", registro: "1.240 produtos", depois: "1.198 OK · 42 erros", ip: "168.195.44.10", sev: "info" },
    { id: "A-77016", ts: "31/08/2026 18:30", user: "Bianca Santos", modulo: "Vendas", acao: "Criou orçamento", registro: "Orçamento 1.082 · Empório Goiás", depois: "R$ 3.412,00", ip: "168.195.44.22", sev: "info" },
    { id: "A-77010", ts: "31/08/2026 09:12", user: "João Pedro Ipê", modulo: "Usuários", acao: "Convidou usuário", registro: "bianca@ipeatacado.com.br", depois: "Perfil Vendedor", ip: "168.195.44.10", sev: "info" },
  ],
};

/* ---------------- notificações ---------------- */
export interface Notif { id: string; kind: "warn" | "danger" | "info" | "ok"; titulo: string; desc: string; ts: string }
export const NOTIFS: Record<string, Notif[]> = {
  "T-0001": [
    { id: "N1", kind: "danger", titulo: "9 títulos vencidos", desc: "R$ 27.840 em aberto há mais de 3 dias. Maior: TechMais R$ 8.900 (12 dias).", ts: "há 22min" },
    { id: "N2", kind: "warn", titulo: "8 produtos abaixo do mínimo", desc: "SSD NVMe 1TB e Fonte 650W com cobertura menor que 5 dias.", ts: "há 1h" },
    { id: "N3", kind: "info", titulo: "Pedido aguardando aprovação", desc: "Pedido 22.119 com desconto de 10% acima da alçada do vendedor.", ts: "há 2h" },
    { id: "N4", kind: "ok", titulo: "Backup concluído", desc: "ERP_EMPRESA_000001 · Full diário 4,8 GB verificado.", ts: "00:30" },
  ],
  "T-0002": [
    { id: "N1", kind: "danger", titulo: "NF-e 44.902 rejeitada", desc: "Rejeição 778 — NCM inexistente. Reprocessamento em fila.", ts: "há 8min" },
    { id: "N2", kind: "warn", titulo: "Certificado A1 vence em 18 dias", desc: "Matriz Contagem · validade até 19/09/2026. Agendar renovação.", ts: "há 1h" },
    { id: "N3", kind: "warn", titulo: "14 itens abaixo do mínimo", desc: "Eletrodo 7018 e Disco 7\" em ponto de reposição.", ts: "há 3h" },
    { id: "N4", kind: "info", titulo: "38 CT-e aguardando manifestação", desc: "Prazo da SEFAZ: 72h para os CT-e de entrada.", ts: "há 5h" },
    { id: "N5", kind: "ok", titulo: "Backup + snapshot concluídos", desc: "18,4 GB com verificação de integridade OK.", ts: "00:45" },
  ],
  "T-0003": [
    { id: "N1", kind: "warn", titulo: "Trial termina em 6 dias", desc: "Plano PROFISSIONAL de avaliação até 07/09/2026. Contrate para não perder acesso.", ts: "há 30min" },
    { id: "N2", kind: "info", titulo: "Importação com 42 erros", desc: "Linhas sem GTIN ou NCM inválido. Revise o relatório.", ts: "há 1h" },
    { id: "N3", kind: "warn", titulo: "3 produtos abaixo do mínimo", desc: "Café 500g com cobertura de 4 dias.", ts: "há 4h" },
  ],
};

/* ---------------- logs ---------------- */
export interface LogRec { ts: string; level: "INFO" | "WARN" | "ERROR" | "DEBUG"; svc: string; msg: string; trace: string; tenant: string }
export const INITIAL_LOGS: LogRec[] = [
  { ts: "09:14:02.118", level: "INFO", svc: "identity", msg: "JWT emitido (access 15min) · claims: tenant=T-0001 perfil=Administrador", trace: "tr-8f2a1c", tenant: "T-0001" },
  { ts: "09:14:01.994", level: "INFO", svc: "tenancy", msg: "Rota resolvida → conexão ERP_EMPRESA_000001@sql-cluster-01", trace: "tr-8f2a1c", tenant: "T-0001" },
  { ts: "09:13:58.410", level: "INFO", svc: "identity", msg: "TOTP 2FA validado para usuario=U-01", trace: "tr-8f2a1b", tenant: "T-0001" },
  { ts: "09:13:57.226", level: "WARN", svc: "identity", msg: "Tentativa de login com senha inválida (3/5) · login=ricardo@…", trace: "tr-7c99d0", tenant: "T-0001" },
  { ts: "09:12:44.031", level: "INFO", svc: "fiscal-worker", msg: "NF-e 45.210 autorizada pela SEFAZ/SP (prot. 135260001123) · 812ms", trace: "tr-41bb07", tenant: "T-0001" },
  { ts: "09:12:40.875", level: "INFO", svc: "queue", msg: "job=emitir-nfe enfileirado · exchange=erp.fiscal · routing=tenant.T-0001", trace: "tr-41bb06", tenant: "T-0001" },
  { ts: "09:11:12.509", level: "ERROR", svc: "fiscal-worker", msg: "NF-e 44.902 rejeitada (778: NCM inexistente) → fila de reprocessamento", trace: "tr-22ce91", tenant: "T-0002" },
  { ts: "09:10:58.140", level: "INFO", svc: "db", msg: "Migration 0014 aplicada em ERP_EMPRESA_000003 · 340ms", trace: "tr-90ad12", tenant: "T-0003" },
  { ts: "09:10:31.772", level: "DEBUG", svc: "cache", msg: "HIT permissões usuario=U-02 (ttl 240s restantes)", trace: "tr-5d11ee", tenant: "T-0001" },
  { ts: "09:09:47.306", level: "INFO", svc: "backup-agent", msg: "Snapshot verificado · checksum SHA-256 confere", trace: "tr-61aa20", tenant: "T-0002" },
];
export const LOG_POOL: Omit<LogRec, "ts" | "trace">[] = [
  { level: "INFO", svc: "api", msg: "GET /api/v1/produtos?pagina=2 · 200 · 46ms", tenant: "T-0001" },
  { level: "INFO", svc: "tenancy", msg: "Rota resolvida → conexão ERP_EMPRESA_000002@sql-cluster-01", tenant: "T-0002" },
  { level: "DEBUG", svc: "cache", msg: "HIT tabela de preços T-0001 (ttl 118s)", tenant: "T-0001" },
  { level: "WARN", svc: "api", msg: "Rate limit 80% do limite para apikey=ak_aurora_web", tenant: "T-0001" },
  { level: "INFO", svc: "fiscal-worker", msg: "Lote RPS 884 enviado · NFS-e prefeitura SP", tenant: "T-0001" },
  { level: "ERROR", svc: "integracoes", msg: "Timeout gateway PIX (503) → retry 1/3 em 2s", tenant: "T-0002" },
  { level: "INFO", svc: "estoque", msg: "Movimentação transacional OK · venda 22.121 · -14 un · reserva liberada", tenant: "T-0001" },
  { level: "INFO", svc: "queue", msg: "job=gerar-relatorio concluído em 4,2s", tenant: "T-0003" },
  { level: "WARN", svc: "identity", msg: "Refresh token reutilizado detectado → sessão revogada (U-05)", tenant: "T-0001" },
  { level: "DEBUG", svc: "db", msg: "Plano de consulta usa IX_Movimentacoes_Filial_Data (sem scan)", tenant: "T-0002" },
];

/* ---------------- testes de isolamento §38 ---------------- */
export const BREACH_TESTS = [
  { nome: "Consulta direta a banco alheio", vetor: "Sessão da Empresa A tenta SELECT no banco da Empresa B", porque: "Impossível por construção: a conexão física da sessão aponta exclusivamente para ERP_EMPRESA_000001. Não existe query cruzada entre bancos." },
  { nome: "Payload com tenant_id adulterado", vetor: "POST /api/v1/pedidos com \"tenant_id\": \"T-0002\" no corpo", porque: "O backend ignora qualquer tenant vindo do request; o contexto autenticado (claim do JWT) prevalece. Tentativa gera evento de segurança." },
  { nome: "Registro de outro tenant por ID", vetor: "GET /api/v1/clientes/01J9X… (ID pertencente à Empresa B)", porque: "O ID só existe no banco da Empresa B. No banco da Empresa A a consulta retorna vazio — equivalente a 404, sem vazar existência." },
  { nome: "Enumeração/IDOR de identificadores", vetor: "Iterar IDs sequenciais buscando dados alheios", porque: "IDs públicos são ULID (128 bits, não sequenciais) e o escopo físico é o banco do tenant. Espaço de busca inviável." },
  { nome: "Parâmetros de query manipulados", vetor: "GET /api/v1/vendas?empresa=T-0002&filial=F-03", porque: "Parâmetros de tenant são descartados no middleware de tenancy; filial é validada contra usuario_filiais do banco do tenant." },
  { nome: "Token de outro contexto", vetor: "JWT da Empresa B apresentado em sessão roteada para a Empresa A", porque: "Claim tenant do token ≠ tenant resolvido do usuário → 401, refresh revogado e incidente registrado no log de segurança." },
];

/* ---------------- stack ---------------- */
export const STACK = [
  { camada: "Frontend", tech: "React + Vite (protótipo) · Next.js em produção", porque: "Design system próprio, SSR/rotas maduras e evolução para app mobile (React Native) compartilhando tipagem." },
  { camada: "Backend", tech: ".NET 10 · ASP.NET Core — modular monolith", porque: "LTS longo, performance de ponta, módulos com boundaries fortes prontos para extração futura sem reescrita." },
  { camada: "ORM / Migrations", tech: "EF Core 10 + migrations separadas (master × tenant)", porque: "Runner multi-tenant aplica o mesmo conjunto de migrations em cada banco, com versão registrada por banco." },
  { camada: "Bancos de dados", tech: "SQL Server 2022 (Linux/Docker) · 1 banco por empresa + ERP_MASTER", porque: "Múltiplos bancos por instância é o cenário natural do SQL Server; backup/log individual por banco. PostgreSQL suportado via abstração de provider." },
  { camada: "Cache & Sessão", tech: "Redis", porque: "Permissões, tabelas de preço e rate limit por tenant com TTL curto; invalidação por chave namespaced." },
  { camada: "Filas", tech: "RabbitMQ", porque: "Emissão fiscal, e-mails, relatórios e integrações fora do request; retry, DLQ e idempotência por job." },
  { camada: "Armazenamento", tech: "MinIO (S3-compatible)", porque: "XML de documentos fiscais, anexos e backups offsite imutáveis, sem vendor lock-in." },
  { camada: "Autenticação", tech: "JWT (15min) + refresh rotacionado + TOTP 2FA · Argon2id", porque: "Estado mínimo no request, detecção de roubo de token por reuso e MFA obrigatório para perfis críticos." },
  { camada: "Observabilidade", tech: "OpenTelemetry → Loki + Grafana + Tempo", porque: "Logs estruturados com tenant_id e trace_id ponta a ponta; SLO por módulo e alertas por tenant." },
  { camada: "Documentação de API", tech: "OpenAPI 3.1 / Swagger · versionamento /api/v1", porque: "Contrato gerado do código, portal para clientes e base para futuras APIs públicas/GraphQL." },
];

/* ---------------- alternativas de tenancy ---------------- */
export const TENANCY_ALTS = [
  { modelo: "Banco único, linha por tenant (tenant_id em todas as tabelas)", isolamento: "Lógico — depende de filtro correto em TODA query", custo: "Baixo", complexidade: "Alta em escala (índices compostos, ruído entre tenants)", veredicto: "Rejeitado — um único JOIN sem filtro vaza dados; violação do requisito crítico." },
  { modelo: "Schema por tenant no mesmo banco", isolamento: "Intermediário — mesmo engine, backups e failure domain compartilhados", custo: "Médio", complexidade: "Média (migração de schema por schema)", veredicto: "Rejeitado — um problema no banco atinge todas as empresas; backup individual impossível." },
  { modelo: "Database-per-tenant (escolhido)", isolamento: "Físico — conexão distinta, zero superfície de query cruzada", custo: "Médio-alto em operação (automatizado)", complexidade: "Média (runner de migrations multi-banco já desenhado)", veredicto: "ESCOLHIDO — atende §2/§38: o dado da Empresa B nunca está ao alcance da conexão da Empresa A." },
];

/* ---------------- SQL ---------------- */
export const SQL_MASTER = `-- ============================================================
-- ERP_MASTER · banco administrativo da plataforma (SQL Server)
-- Único banco compartilhado. NÃO armazena dado operacional
-- de nenhuma empresa — apenas localização e contrato.
-- ============================================================
CREATE TABLE Plans (
  Id              UNIQUEIDENTIFIER PRIMARY KEY,
  Codigo          VARCHAR(30)     NOT NULL UNIQUE,   -- BASICO | PROFISSIONAL | EMPRESARIAL | ENTERPRISE
  Modulos         NVARCHAR(600)   NOT NULL,          -- JSON: módulos habilitados no plano
  LimiteUsuarios  INT             NOT NULL,
  LimiteFiliais   INT             NOT NULL,
  ArmazenamentoMb BIGINT          NOT NULL
);

CREATE TABLE Tenants (
  Id          UNIQUEIDENTIFIER PRIMARY KEY,
  Slug        VARCHAR(40)      NOT NULL UNIQUE,
  RazaoSocial NVARCHAR(160)    NOT NULL,
  CnpjBase    CHAR(8)          NOT NULL UNIQUE,
  PlanId      UNIQUEIDENTIFIER NOT NULL REFERENCES Plans(Id),
  Status      VARCHAR(20)      NOT NULL DEFAULT 'TRIAL', -- TRIAL | ATIVA | SUSPENSA | CANCELADA
  TrialAte    DATETIME2        NULL,
  CriadoEm    DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE TenantDatabases (
  Id            UNIQUEIDENTIFIER PRIMARY KEY,
  TenantId      UNIQUEIDENTIFIER NOT NULL UNIQUE REFERENCES Tenants(Id),
  Servidor      VARCHAR(120)     NOT NULL,           -- instância/cluster que hospeda o banco
  DatabaseName  VARCHAR(60)      NOT NULL UNIQUE,    -- ERP_EMPRESA_000001
  ConnRef       VARBINARY(512)   NOT NULL,           -- conexão CRIPTOGRAFADA via KMS — nunca em texto puro
  SchemaVersion INT              NOT NULL DEFAULT 0, -- última migration aplicada neste banco
  CriadoEm      DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME()
);
CREATE INDEX IX_TenantDbs_Servidor ON TenantDatabases(Servidor);

CREATE TABLE Subscriptions (
  Id         UNIQUEIDENTIFIER PRIMARY KEY,
  TenantId   UNIQUEIDENTIFIER NOT NULL REFERENCES Tenants(Id),
  PlanId     UNIQUEIDENTIFIER NOT NULL REFERENCES Plans(Id),
  Inicio     DATETIME2 NOT NULL,
  ProximaCob DATETIME2 NULL,
  Status     VARCHAR(20) NOT NULL        -- ATIVA | INADIMPLENTE | CANCELADA
);

CREATE TABLE PlatformUsers (
  Id         UNIQUEIDENTIFIER PRIMARY KEY,
  Nome       NVARCHAR(120) NOT NULL,
  Login      VARCHAR(120)  NOT NULL UNIQUE,
  SenhaHash  NVARCHAR(200) NOT NULL,     -- Argon2id
  MfaAtivo   BIT           NOT NULL DEFAULT 1
);

CREATE TABLE PlatformAudit (
  Id        BIGINT IDENTITY PRIMARY KEY, -- append-only: sem UPDATE/DELETE (trigger de bloqueio)
  Ts        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  UserId    UNIQUEIDENTIFIER NULL,
  Acao      VARCHAR(120) NOT NULL,       -- provisionar_tenant, suspender_tenant, ...
  Detalhe   NVARCHAR(MAX) NULL,
  Ip        VARCHAR(45)  NULL
);`;

export const SQL_TENANT = `-- ============================================================
-- ERP_EMPRESA_00000X · banco operacional da empresa (ex.: A)
-- Provisionado automaticamente no aceite do contrato.
-- Isolamento físico total: nenhuma tabela aqui conhece outro tenant.
-- ============================================================
CREATE TABLE Empresa (
  Id               UNIQUEIDENTIFIER PRIMARY KEY,
  RazaoSocial      NVARCHAR(160) NOT NULL,
  RegimeTributario VARCHAR(20)   NOT NULL,   -- SIMPLES | PRESUMIDO | REAL
  CertificadoRef   VARBINARY(512) NULL,      -- A1 criptografado (KMS)
  CertificadoVal   DATETIME2      NULL       -- alerta automático de vencimento
);

CREATE TABLE Filiais (
  Id              UNIQUEIDENTIFIER PRIMARY KEY,
  EmpresaId       UNIQUEIDENTIFIER NOT NULL REFERENCES Empresa(Id),
  Codigo          VARCHAR(20)   NOT NULL UNIQUE,
  Cnpj            CHAR(14)      NOT NULL UNIQUE,
  InscEstadual    VARCHAR(20)   NULL,
  InscMunicipal   VARCHAR(20)   NULL,
  Matriz          BIT           NOT NULL DEFAULT 0
);

CREATE TABLE Perfis ( Id UNIQUEIDENTIFIER PRIMARY KEY, Nome VARCHAR(60) NOT NULL UNIQUE );

CREATE TABLE Permissoes (
  Id      UNIQUEIDENTIFIER PRIMARY KEY,
  Modulo  VARCHAR(40) NOT NULL,
  Acao    VARCHAR(40) NOT NULL,
  CONSTRAINT UQ_Permissao UNIQUE (Modulo, Acao)
);

CREATE TABLE PerfilPermissoes (
  PerfilId     UNIQUEIDENTIFIER NOT NULL REFERENCES Perfis(Id) ON DELETE CASCADE,
  PermissaoId  UNIQUEIDENTIFIER NOT NULL REFERENCES Permissoes(Id) ON DELETE CASCADE,
  PRIMARY KEY (PerfilId, PermissaoId)
);

CREATE TABLE Usuarios (
  Id          UNIQUEIDENTIFIER PRIMARY KEY,
  Nome        NVARCHAR(120) NOT NULL,
  Login       VARCHAR(120)  NOT NULL UNIQUE,
  SenhaHash   NVARCHAR(200) NOT NULL,          -- Argon2id; nunca texto puro
  PerfilId    UNIQUEIDENTIFIER NOT NULL REFERENCES Perfis(Id),
  FilialPadraoId UNIQUEIDENTIFIER NULL REFERENCES Filiais(Id),
  MfaAtivo    BIT NOT NULL DEFAULT 0,
  Status      VARCHAR(20) NOT NULL DEFAULT 'ATIVO',
  UltimoAcesso DATETIME2 NULL
);

-- Escopo por filial: sem linhas = somente filial padrão;
-- flag TODAS = acesso global. Regra aplicada no backend, sempre.
CREATE TABLE UsuarioFiliais (
  UsuarioId UNIQUEIDENTIFIER NOT NULL REFERENCES Usuarios(Id) ON DELETE CASCADE,
  FilialId  UNIQUEIDENTIFIER NOT NULL REFERENCES Filiais(Id) ON DELETE CASCADE,
  Todas     BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (UsuarioId, FilialId)
);

CREATE TABLE Sessoes (
  Id         UNIQUEIDENTIFIER PRIMARY KEY,
  UsuarioId  UNIQUEIDENTIFIER NOT NULL REFERENCES Usuarios(Id),
  Device     NVARCHAR(160) NULL, Ip VARCHAR(45) NULL,
  ExpiraEm   DATETIME2 NOT NULL,
  RevogadaEm DATETIME2 NULL
);

CREATE TABLE RefreshTokens (
  Hash           CHAR(64) PRIMARY KEY,         -- SHA-256 do token; token em si nunca persistido
  UsuarioId      UNIQUEIDENTIFIER NOT NULL REFERENCES Usuarios(Id),
  SessaoId       UNIQUEIDENTIFIER NOT NULL REFERENCES Sessoes(Id),
  ExpiraEm       DATETIME2 NOT NULL,
  SubstituidoPor CHAR(64) NULL                 -- rotação: reuso de token antigo revoga a família
);

CREATE TABLE AuditTrail (
  Id           BIGINT IDENTITY PRIMARY KEY,    -- append-only + cadeia de hashes (tamper-evident)
  Ts           DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  UsuarioId    UNIQUEIDENTIFIER NULL,
  Modulo       VARCHAR(40)  NOT NULL,
  Acao         VARCHAR(120) NOT NULL,
  RegistroRef  VARCHAR(120) NULL,
  Antes        NVARCHAR(MAX) NULL,             -- JSON do estado anterior
  Depois       NVARCHAR(MAX) NULL,             -- JSON do estado novo
  Ip           VARCHAR(45) NULL,
  HashAnterior CHAR(64) NULL
);

CREATE TABLE Sequencias (
  Chave VARCHAR(60) PRIMARY KEY,               -- series fiscais, números de pedido...
  Valor BIGINT NOT NULL
);

-- FASE 2+ · criadas pelas migrations de cada módulo, sempre transacionais:
-- Pessoas, Enderecos, Contatos, Clientes, Fornecedores, Funcionarios,
-- Produtos, GradesVariacoes, Estoques, MovimentacoesEstoque, PedidosVenda,
-- ItensPedido, Titulos, Parcelas, Caixas, ContasBancarias,
-- DocumentosFiscais, EventosFiscais, RegrasTributarias (com vigência)...
-- VENDA → reserva → faturamento → documento fiscal → financeiro → estoque
-- executa em UMA transação por banco: tudo commita ou nada acontece.`;

/* ---------------- estratégias ---------------- */
export const STRATEGIES = [
  {
    id: "b-migrations", titulo: "Migrations multi-tenant",
    pontos: [
      "Dois pipelines: migrations do ERP_MASTER (admin) e do conjunto de módulos (tenants).",
      "Runner central lê TenantDatabases.SchemaVersion e aplica, em ordem e dentro de transação, as migrations pendentes em cada banco.",
      "Falha em um banco isola apenas aquele banco: incidente aberto, demais seguem (blast radius mínimo).",
      "Migrations sempre compatíveis com a versão anterior (padrão expand → migrate → contract) para permitir deploy sem downtime.",
      "Snapshot/backup imediatamente antes de migration estrutural; rollback validado em banco sandbox.",
    ],
  },
  {
    id: "b-backup", titulo: "Backup & recuperação de desastre",
    pontos: [
      "Full diário + backup de log a cada 15 min por banco de empresa (SQL Server) — PITR por banco.",
      "Retenção de 35 dias + cópia offsite imutável (object storage compatível S3).",
      "Teste de restore automatizado semanal em banco sandbox, com verificação de checksum.",
      "ERP_MASTER com réplica e rotina própria: sem o master não se resolve tenant nenhum — prioridade máxima.",
      "Um problema em um banco jamais afeta os demais: jobs, janelas e armazenamentos independentes.",
    ],
  },
  {
    id: "b-logs", titulo: "Logs estruturados & monitoramento",
    pontos: [
      "Todo log carrega tenant_id, user_id e trace_id (OpenTelemetry) — correlação ponta a ponta.",
      "Nunca registrar senhas, tokens, conteúdo de certificado ou dados pessoais além do necessário.",
      "Métricas por banco: conexões ativas, tempo de query, filas, latência SEFAZ/gateways.",
      "Alertas: ERROR em fila fiscal, tentativa de acesso cross-tenant, refresh token reutilizado, backup falhado.",
      "Retenção hot 30 dias / cold 1 ano; exportação para SIEM quando o cliente exigir.",
    ],
  },
  {
    id: "b-auditoria", titulo: "Auditoria completa (§19)",
    pontos: [
      "AuditTrail append-only no banco da própria empresa: usuário, data/hora, IP, dispositivo, módulo, registro, antes/depois em JSON.",
      "Cadeia de hashes (cada linha referencia o hash da anterior) — adulteração fica evidenciável.",
      "Operações críticas obrigatórias: preços, limites de crédito, baixas/estornos, cancelamentos fiscais, permissões, certificados, usuários.",
      "Bloqueio de UPDATE/DELETE na tabela de auditoria via trigger — trilha não se apaga, nem por admin.",
      "Exemplo real: “Anderson alterou o preço do produto X de R$ 100,00 para R$ 120,00 em 01/09/2026 às 14:32” — consultável para sempre.",
    ],
  },
  {
    id: "b-testes", titulo: "Estratégia de testes (§37/§38)",
    pontos: [
      "Unitários por módulo de domínio; integração de API contra banco real efêmero por teste.",
      "Matriz de autorização testada: cada perfil × permissão × restrição de filial.",
      "Testes fiscais: cálculo de ICMS/IPI/PIS/COFINS por UF e regime; snapshots de XML esperados.",
      "Testes transacionais: venda completa deve mover estoque E gerar financeiro E emitir documento — ou falhar por inteiro.",
      "Bateria de isolamento obrigatória em todo PR (os 6 ataques do §38) — todos DEVEM falhar; qualquer sucesso bloqueia o merge.",
    ],
  },
];

/* ---------------- ADRs ---------------- */
export interface Adr { num: string; titulo: string; status: string; problema: string; alternativas: string[]; decisao: string; consequencias: string[] }
export const ADRS: Adr[] = [
  {
    num: "ADR-001", titulo: "Isolamento de dados: database-per-tenant", status: "ACEITA",
    problema: "O contrato do produto promete que a Empresa A jamais acessa dados da Empresa B. Como garantir isso estruturalmente, e não por disciplina de código?",
    alternativas: ["Banco único com tenant_id em todas as tabelas (mais barato, risco permanente de vazamento por query mal filtrada)", "Schema por tenant (mesmo failure domain, backup por empresa inviável)", "Banco por tenant (custo operacional maior, isolamento físico)"],
    decisao: "Database-per-tenant com ERP_MASTER central para localização/contratos. O tenant é resolvido pelo contexto autenticado no backend e a conexão é fisicamente direcionada ao banco da empresa.",
    consequencias: ["Exige runner de migrations multi-banco e provisionamento automatizado (aceitável, já desenhado)", "Custo de infra cresce linearmente com tenants — mitigado por agrupamento em clusters", "Backup/restore, performance e incidentes tornam-se isolados por empresa (vantagem contratual)"],
  },
  {
    num: "ADR-002", titulo: "Modular monolith em .NET 10 (não microserviços)", status: "ACEITA",
    problema: "O escopo cobre 15+ domínios. Microserviços agora imporiam custo operacional e latência; um monólito bagunçado condenaria a evolução.",
    alternativas: ["Microserviços por domínio desde o dia 1", "Monólito tradicional sem boundaries", "Modular monolith: módulos com API interna, regras de dependência e fronteiras testadas"],
    decisao: "Modular monolith: cada módulo (Vendas, Fiscal, Estoque...) é um pacote com contratos internos; apenas Fiscal e Notificações nascem como workers independentes por natureza assíncrona.",
    consequencias: ["Deploy único e simples no início; extração de um módulo vira tarefa mecânica quando houver demanda real", "Testes de contract entre módulos obrigatórios no CI"],
  },
  {
    num: "ADR-003", titulo: "Banco operacional: SQL Server 2022 (Linux)", status: "ACEITA",
    problema: "Database-per-tenant significa dezenas a milhares de bancos por instância ao longo dos anos. Qual engine suporta isso com operação madura?",
    alternativas: ["PostgreSQL 16 (excelente, porém gestão de muitos bancos + pooling por banco exige mais ferramental)", "SQL Server 2022 (múltiplos bancos por instância é o cenário nativo; equipe já domina)", "MySQL (limitações históricas em DDL transacional)"],
    decisao: "SQL Server 2022 em containers Linux como padrão; acesso sempre via EF Core com provider abstraído, mantendo PostgreSQL como alternativa viável sem reescrita.",
    consequencias: ["Backup de log, snapshot e DDL transacional nativos facilitam o runner de migrations", "Acesso via abstração impede SQL proprietário espalhado pelo domínio"],
  },
  {
    num: "ADR-004", titulo: "Autenticação: JWT curto + refresh rotacionado + TOTP", status: "ACEITA",
    problema: "Sessões longas são alvo de roubo; sessões só de cookie dificultam API e apps futuros. Como equilibrar segurança e UX?",
    alternativas: ["Sessão server-side clássica", "JWT de longa duração", "JWT de acesso curto (15min) + refresh token de uso único com rotação + 2FA TOTP para perfis críticos"],
    decisao: "Access token de 15 min com claims de tenant/perfil/filiais; refresh tokens hash-eados no banco, rotação a cada uso — reuso de token antigo revoga toda a família (detecção de roubo).",
    consequencias: ["Revogação rápida em caso de comprometimento", "2FA TOTP obrigatório para Administrador e perfis financeiro/fiscal", "Fluxos de recuperação de senha auditados e limitados por IP"],
  },
  {
    num: "ADR-005", titulo: "Motor fiscal desacoplado via fila (FiscalEngine)", status: "ACEITA",
    problema: "SEFAZ e prefeituras são instáveis e lentas. Regras fiscais mudam constantemente e não podem contaminar Vendas/Estoque/Financeiro.",
    alternativas: ["Emissão síncrona dentro do fluxo de venda", "Serviço fiscal próprio consumindo fila, com regras versionadas por vigência e armazenamento de XML"],
    decisao: "Worker FiscalEngine independente: Vendas publica 'faturar documento' na fila; o worker calcula (regras com vigência por UF/operação), assina com o certificado da filial, transmite, grava XML e devolve eventos.",
    consequencias: ["Indisponibilidade da SEFAZ vira fila + contingência, nunca venda travada", "Toda regra fiscal tem versão e vigência — nada é assumido permanente", "Idempotência por chave de acesso evita documento duplicado"],
  },
  {
    num: "ADR-006", titulo: "Identificadores públicos: ULID", status: "ACEITA",
    problema: "IDs sequenciais permitem enumeração (IDOR) e vazam volume de negócio; UUIDv4 puro fragmenta índices.",
    alternativas: ["IDENTITY BIGINT exposto na API", "UUIDv4 aleatório", "ULID: 128 bits, ordenável no tempo, não sequencial"],
    decisao: "ULID em toda referência pública de API e entre módulos; BIGINT identity restrito a interno de tabelas de altíssimo volume (logs, audit).",
    consequencias: ["Índices clustered com boa localidade temporal", "Enumeração de registros de terceiros inviável — reforça §38"],
  },
  {
    num: "ADR-007", titulo: "Soft delete seletivo + append-only fiscal/financeiro", status: "ACEITA",
    problema: "Excluir registros fiscais/financeiros é ilegal do ponto de vista de auditoria; apagar cadastros ativos quebra histórico.",
    alternativas: ["Hard delete geral", "Soft delete geral", "Soft delete apenas em cadastros; financeiro/fiscal/estoque estritamente append-only com estornos formais"],
    decisao: "Cadastros usam soft delete com trilha de auditoria. Lançamentos financeiros, documentos fiscais e movimentações de estoque são imutáveis — correções entram como estorno/contramovimento referenciando o original.",
    consequencias: ["Histórico íntegro e auditável para sempre", "Consultas precisam filtrar excluídos em cadastros (índoces parciais ajudam)"],
  },
];

/* ---------------- riscos ---------------- */
export const RISKS = [
  { risco: "Proliferação de bancos (centenas/milhares de tenants)", prob: "Alta", impacto: "Médio", mitigacao: "Agrupamento em clusters por capacidade, provisionamento 100% automatizado, telemetria de ocupação por instância e pool de conexões por banco com limites." },
  { risco: "Migrations aplicadas em N bancos com falhas parciais", prob: "Média", impacto: "Alto", mitigacao: "Runner transacional por banco, isolamento de falha + fila de retry, snapshot pré-migration e janela de rollback documentada." },
  { risco: "Complexidade fiscal brasileira (27 UFs + municípios)", prob: "Alta", impacto: "Alto", mitigacao: "Motor fiscal com regras versionadas por vigência, suíte de testes por UF/regime, contingência offline e consultoria fiscal contínua." },
  { risco: "Tentativas de acesso cross-tenant por bugs de código", prob: "Baixa", impacto: "Crítico", mitigacao: "Isolamento físico por banco + bateria §38 no CI + claim de tenant validada no backend + eventos de segurança com alerta imediato." },
  { risco: "Volume: milhões de movimentações de estoque/financeiro", prob: "Média", impacto: "Médio", mitigacao: "Índices cobrindo consultas-chave, particionamento por data quando justificado, filas para processamentos pesados e cache de leitura." },
  { risco: "Indisponibilidade SEFAZ/gateways bancários", prob: "Alta", impacto: "Médio", mitigacao: "Fila com retry exponencial + DLQ, contingência offline para NFC-e, painéis de status por integração e retentativa em lote." },
];

/* ---------------- roadmap ---------------- */
export interface Phase { n: number; titulo: string; status: "exec" | "aprovacao" | "plan"; escopo: string[]; gate: string; prog?: number; done?: string[]; pend?: string[] }
export const PHASES: Phase[] = [
  {
    n: 1, titulo: "Fundação", status: "exec", prog: 70,
    escopo: ["Banco master (ERP_MASTER)", "Provisionamento de banco por empresa", "Autenticação + 2FA", "Usuários & sessões", "RBAC granular", "Empresas & filiais", "Auditoria & logs", "Layout base & dashboard inicial"],
    gate: "Blueprint §47 aprovado + fundação validada em ambiente real",
    done: ["Arquitetura & blueprint §47 (este app)", "Layout base + navegação por módulos", "Dashboard inicial com indicadores", "Gestão de tenants, bancos e filiais", "Fluxo de autenticação com 2FA", "Matriz RBAC por perfil", "Trilhas de auditoria e logs estruturados"],
    pend: ["API REST real + OpenAPI", "Provisionamento automático (runner)", "Migrations multi-tenant no CI"],
  },
  {
    n: 2, titulo: "Cadastros", status: "aprovacao",
    escopo: ["Pessoas (física/jurídica, endereços, contatos)", "Clientes (crédito, condições, tabelas)", "Fornecedores", "Funcionários (módulo próprio, base p/ eSocial)", "Produtos (NCM, grades, lotes)", "Unidades, categorias, marcas"],
    gate: "Fase 1 concluída e aprovada",
  },
  {
    n: 3, titulo: "Estoque · Vendas · Compras", status: "plan",
    escopo: ["Movimentações transacionais de estoque", "Orçamento → pedido → venda → faturamento", "Reservas e transferências entre filiais", "Cotação → pedido de compra → recebimento", "Comissões e devoluções"],
    gate: "Cadastros estáveis + regras comerciais revisadas",
  },
  {
    n: 4, titulo: "Financeiro", status: "plan",
    escopo: ["Contas a receber/pagar com parcelas", "Juros, multas, renegociação", "Caixa (abertura, sangria, conferência)", "Contas bancárias, PIX, conciliação", "Centros de custo e rateios"],
    gate: "Fluxo venda→financeiro transacional validado",
  },
  {
    n: 5, titulo: "Fiscal brasileiro", status: "plan",
    escopo: ["Motor de regras com vigência (ICMS, IPI, PIS/COFINS, ISS, ST, DIFAL)", "NF-e · NFC-e · NFS-e · CT-e · MDF-e", "Eventos, cancelamento, CC-e, inutilização", "Certificados A1 (KMS) e alertas", "Armazenamento de XML + DANFE"],
    gate: "Motor fiscal homologado em ambiente de testes SEFAZ",
  },
  {
    n: 6, titulo: "Relatórios · BI · Integrações", status: "plan",
    escopo: ["Relatórios com filtros e exportação (PDF/Excel/CSV)", "Camada de BI (leitura em réplica)", "Integrações: bancos, gateways, e-commerce, contabilidade", "APIs públicas + webhooks", "Importações em lote (CSV/XML)"],
    gate: "Base de dados estabilizada em produção",
  },
];

/* ---------------- índice §47 ---------------- */
export const INDEX47 = [
  { n: 1, t: "Arquitetura geral", alvo: "b-arq" },
  { n: 2, t: "Stack recomendada", alvo: "b-stack" },
  { n: 3, t: "Diagrama lógico", alvo: "b-diagrama" },
  { n: 4, t: "Estratégia de multi-tenancy", alvo: "b-tenancy" },
  { n: 5, t: "Banco por empresa", alvo: "b-dbempresa" },
  { n: 6, t: "Banco master (ERP_MASTER)", alvo: "b-master" },
  { n: 7, t: "Banco inicial da empresa", alvo: "b-tenant" },
  { n: 8, t: "Autenticação", alvo: "b-auth" },
  { n: 9, t: "Autorização (RBAC)", alvo: "b-rbac" },
  { n: 10, t: "Estrutura de diretórios", alvo: "b-dirs" },
  { n: 11, t: "Migrations", alvo: "b-migrations" },
  { n: 12, t: "Backup & DR", alvo: "b-backup" },
  { n: 13, t: "Logs & monitoramento", alvo: "b-logs" },
  { n: 14, t: "Auditoria", alvo: "b-auditoria" },
  { n: 15, t: "Testes & isolamento", alvo: "b-testes" },
  { n: 16, t: "Roadmap", alvo: "roadmap" },
  { n: 17, t: "Riscos técnicos", alvo: "b-riscos" },
  { n: 18, t: "Decisões (ADRs)", alvo: "b-adrs" },
];

export const DIR_TREE = `nucleo-erp/
├─ apps/
│  ├─ web/                     # React + Vite → Next.js (este protótipo)
│  ├─ api/                     # ASP.NET Core · host do modular monolith
│  └─ workers/
│     ├─ fiscal-engine/        # NF-e/NFC-e/NFS-e · regras com vigência
│     ├─ notifications/        # e-mail · WhatsApp · SMS · push
│     └─ backup-agent/         # backup/restore verificado por banco
├─ modules/                    # fronteiras estritas · APIs internas
│  ├─ Identity/                # usuários · sessões · 2FA · Argon2id
│  ├─ Tenancy/                 # resolução de tenant · roteamento de conexão
│  ├─ Empresas/                # empresa · filiais · certificados
│  ├─ Pessoas/                 # pessoas · endereços · contatos
│  ├─ Clientes/  Fornecedores/  Funcionarios/
│  ├─ Produtos/                # itens · grades · tabelas de preço
│  ├─ Estoque/                 # movimentações transacionais
│  ├─ Vendas/  Compras/
│  ├─ Financeiro/              # títulos · caixa · bancos · centros de custo
│  ├─ Fiscal/                  # regras versionadas · eventos · XML
│  ├─ Permissoes/              # RBAC + escopo por filial
│  ├─ Auditoria/               # trilha append-only encadeada
│  └─ Relatorios/
├─ db/
│  ├─ master/migrations/       # ERP_MASTER
│  └─ tenant/migrations/       # aplicadas em cada banco de empresa
├─ infra/
│  ├─ docker/  k8s/  terraform/
│  └─ provision-tenant/        # cria banco + admin inicial + migra
├─ tests/
│  └─ tenant-isolation/        # bateria §38 — deve falhar sempre
└─ docs/                       # ADRs · runbooks · API · onboarding`;

/* ---------------- fluxo de autenticação ---------------- */
export const AUTH_FLOW = [
  { passo: "Credenciais", detalhe: "Senha verificada contra hash Argon2id. Contador de tentativas por login+IP com bloqueio progressivo; cada falha gera evento de segurança." },
  { passo: "Resolução do tenant", detalhe: "O tenant vem do usuário autenticado (Usuário → Tenant no ERP_MASTER) — NUNCA de parâmetro do frontend. A partir daqui, toda conexão é roteada ao banco da empresa." },
  { passo: "Segundo fator (TOTP)", detalhe: "Código RFC 6238 obrigatório para perfis críticos. Códigos de recuperação hash-eados; dispositivos confiáveis por 30 dias com revogação manual." },
  { passo: "JWT de acesso (15 min)", detalhe: "Claims: sub, tenant_id, perfil_id, filiais[], jti. Assinatura com chaves rotacionáveis; validação em cada request no backend." },
  { passo: "Refresh com rotação", detalhe: "Token de uso único (hash no banco). Reuso de token antigo = possível roubo → revoga a família inteira de sessões e notifica o usuário." },
  { passo: "Autorização por request", detalhe: "RBAC (perfil × permissão) + escopo de filiais + módulos contratados no plano. Tudo revalidado no backend; o frontend só esconde o que não pode." },
];

export const PROVISION_STEPS = [
  "INSERT em Tenants + Subscriptions (ERP_MASTER)",
  "CREATE DATABASE ERP_EMPRESA_00000X no cluster designado",
  "Migrations do conjunto de módulos aplicadas no novo banco",
  "Usuário administrador inicial + perfil padrão criados",
  "Registro em TenantDatabases (ConnRef criptografada via KMS)",
  "Backup inicial agendado + telemetria habilitada",
];

/* ---------------- contexto global da UI ---------------- */
export interface AppCtx {
  tenants: Tenant[];
  tenant: Tenant;
  connecting: boolean;
  switchTenant: (id: string) => void;
  filialId: string;
  setFilialId: (id: string) => void;
  view: ViewId;
  nav: (v: ViewId) => void;
  toast: (msg: string, kind?: "ok" | "warn" | "err" | "info") => void;
  userName: string;
  userPerfil: string;
  logout: () => void;
}
