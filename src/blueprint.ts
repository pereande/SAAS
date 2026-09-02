/* =============================================================
   MATRIZ-BP-001 · Blueprint Arquitetural — conteúdo (§47)
   Documento de fundação: nenhuma tela de CRUD aqui.
   ============================================================= */

export const DOC = {
  id: "MATRIZ-BP-001",
  titulo: "ERP SaaS Multiempresa",
  subtitulo: "Blueprint Arquitetural · Multi-tenancy, Fiscal BR e Modular Monolith",
  rev: "1.0",
  data: "02/2026",
  classificacao: "USO INTERNO · CONFIDENCIAL",
  autor: "Arquitetura · Eng. de Plataforma",
};

export interface SecDef { id: string; n: string; title: string; short: string; icon: string }

export const SECTIONS: SecDef[] = [
  { id: "arquitetura",  n: "01", title: "Arquitetura geral",               short: "Arquitetura",      icon: "layers" },
  { id: "stack",        n: "02", title: "Stack tecnológica",               short: "Stack",            icon: "bolt" },
  { id: "logico",       n: "03", title: "Diagrama lógico de requisição",   short: "Requisição",       icon: "route" },
  { id: "tenancy",      n: "04", title: "Multi-tenancy e isolamento",      short: "Multi-tenancy",    icon: "db" },
  { id: "master",       n: "05", title: "Banco master (ERP_MASTER)",       short: "Banco master",     icon: "net" },
  { id: "tenant-db",    n: "06", title: "Banco da empresa (ERP_EMPRESA_*)", short: "Banco empresa",   icon: "box" },
  { id: "authn",        n: "07", title: "Autenticação",                    short: "Autenticação",     icon: "key" },
  { id: "authz",        n: "08", title: "Autorização (RBAC)",              short: "RBAC",             icon: "shield" },
  { id: "dirs",         n: "09", title: "Estrutura de diretórios",         short: "Diretórios",       icon: "doc" },
  { id: "migrations",   n: "10", title: "Estratégia de migrations",        short: "Migrations",       icon: "swap" },
  { id: "backup",       n: "11", title: "Backup e recuperação",            short: "Backup & DR",      icon: "clock" },
  { id: "logs",         n: "12", title: "Logs e observabilidade",          short: "Logs",             icon: "terminal" },
  { id: "auditoria",    n: "13", title: "Auditoria",                       short: "Auditoria",        icon: "audit" },
  { id: "testes",       n: "14", title: "Estratégia de testes",            short: "Testes",           icon: "check" },
  { id: "roadmap",      n: "15", title: "Roadmap de desenvolvimento",      short: "Roadmap",          icon: "compass" },
  { id: "riscos",       n: "16", title: "Riscos técnicos",                 short: "Riscos",           icon: "alert" },
  { id: "adrs",         n: "17", title: "Decisões arquiteturais (ADRs)",   short: "ADRs",             icon: "seal" },
  { id: "aprovacao",    n: "18", title: "Aprovação e Fase 1",              short: "Aprovação",        icon: "out" },
];

export const PRINCIPLES = [
  "Segurança",
  "Integridade dos dados",
  "Isolamento entre empresas",
  "Consistência transacional",
  "Manutenção",
  "Escalabilidade",
  "Performance",
  "Experiência do usuário",
];

/* ---------------- §01 arquitetura ---------------- */

export interface ModDef { id: string; name: string; desc: string; kind: "core" | "dominio" | "especialista" }

export const MODULES: ModDef[] = [
  { id: "auth", name: "Auth", kind: "core", desc: "Login, JWT, refresh token, MFA, sessões, recuperação de senha e proteção contra brute force." },
  { id: "tenants", name: "Tenant Management", kind: "core", desc: "Provisionamento de bancos por empresa, resolução de contexto, plano/assinatura e limites." },
  { id: "identity", name: "Identity & RBAC", kind: "core", desc: "Usuários, perfis, permissões granulares, escopo por filial e políticas de autorização." },
  { id: "audit", name: "Auditoria", kind: "core", desc: "Trilha append-only de operações com antes/depois, usuário, IP e dispositivo." },
  { id: "cadastros", name: "Cadastros", kind: "dominio", desc: "Pessoas (PF/PJ), endereços, contatos, clientes, fornecedores e funcionários como módulos próprios — nunca uma tabela genérica." },
  { id: "produtos", name: "Produtos", kind: "dominio", desc: "Itens, NCM/CEST/GTIN, grades/variações, tabelas de preço, lotes, validade e série." },
  { id: "estoque", name: "Estoque", kind: "dominio", desc: "Saldos por filial/depósito, movimentos sempre rastreados, reservas, transferências e inventário." },
  { id: "vendas", name: "Vendas", kind: "dominio", desc: "Orçamento → pedido → faturamento, múltiplos pagamentos, comissões, devoluções e cancelamentos." },
  { id: "compras", name: "Compras", kind: "dominio", desc: "Solicitação → cotação → pedido → recebimento → entrada fiscal → contas a pagar." },
  { id: "financeiro", name: "Financeiro", kind: "dominio", desc: "Contas a receber/pagar, parcelas, juros/multa/desconto, caixa por filial, bancos e conciliação." },
  { id: "fiscal", name: "Fiscal Engine", kind: "especialista", desc: "Camada desacoplada: regras versionáveis por vigência, cálculo de ICMS/ST/IPI/PIS/COFINS/DIFAL, NF-e/NFC-e/NFS-e, eventos e XML." },
  { id: "relatorios", name: "Relatórios & BI", kind: "especialista", desc: "Filtros, exportação PDF/Excel/CSV e read models dedicados para não onerar o banco transacional." },
  { id: "notif", name: "Notificações", kind: "especialista", desc: "Canais interno, e-mail, WhatsApp, SMS e push com templates e preferências por usuário." },
  { id: "integr", name: "Integrações", kind: "especialista", desc: "Adapters para SEFAZ, bancos, gateways, marketplaces e contabilidade — sempre atrás de interfaces." },
];

export const ARCH_LAYERS: { id: string; label: string; tone: "edge" | "app" | "async" | "data" | "obs"; items: string[] }[] = [
  { id: "edge", label: "Borda", tone: "edge", items: ["TLS 1.3 / WAF", "CDN", "Rate limit global"] },
  { id: "clientes", label: "Clientes", tone: "edge", items: ["ERP SPA (React)", "Portal admin da plataforma", "API pública p/ integrações", "App móvel (futuro)"] },
  { id: "gateway", label: "API Gateway", tone: "app", items: ["Autenticação", "Versionamento /v1", "Throttling por plano", "OpenAPI"] },
  { id: "mono", label: "Monólito modular · .NET 10", tone: "app", items: [] },
  { id: "async", label: "Processamento assíncrono", tone: "async", items: ["RabbitMQ", "Workers: emissão fiscal", "Relatórios", "E-mails · Backups · Importações"] },
  { id: "data", label: "Dados", tone: "data", items: ["ERP_MASTER", "ERP_EMPRESA_000001…N", "Redis (cache/sessão/lock)", "MinIO/S3 (XML, PDF, backups)"] },
  { id: "obs", label: "Observabilidade", tone: "obs", items: ["OpenTelemetry", "Serilog → Seq", "Prometheus + Grafana", "Healthchecks"] },
];

export const SALE_FLOW = [
  { n: 1, t: "Pedido", d: "Validações comerciais, preço pela tabela do cliente, crédito e aprovação conforme perfil." },
  { n: 2, t: "Reserva de estoque", d: "Reserva transacional do saldo disponível na filial/depósito — nunca negativa." },
  { n: 3, t: "Faturamento", d: "Geração da venda e snapshot de valores. Tudo na mesma transação local do banco da empresa." },
  { n: 4, t: "Documento fiscal", d: "Evento publicado via Outbox → worker do Fiscal Engine emite NF-e/NFC-e na SEFAZ (fila, retry, contingência)." },
  { n: 5, t: "Financeiro", d: "Contas a receber geradas a partir das parcelas — dentro da transação, não 'depois, se der'." },
  { n: 6, t: "Movimento de estoque", d: "Saída registrada com origem=VENDA e referência ao documento. Saldo disponível atualizado; rastro completo." },
];

/* ---------------- §02 stack ---------------- */

export const STACK = [
  { layer: "Frontend ERP", tech: "React 18 + Vite · Tailwind · TanStack Query · RHF + Zod", why: "Aplicação de alta densidade (tabelas, formulários longos, PDV). Tipagem ponta a ponta com o contrato OpenAPI." },
  { layer: "Backend", tech: "ASP.NET Core · .NET 10 · EF Core · MediatR (CQRS-lite) · FluentValidation", why: "Monólito modular com limites físicos por módulo. Maturidade empresarial, performance e ecossistema fiscal BR (ACBr.NET e provedores)." },
  { layer: "Banco da empresa", tech: "PostgreSQL 16 (um banco por tenant)", why: "Menor custo por banco, JSONB para snapshots fiscais, WAL/PITR por banco, particionamento nativo para movimentos e auditoria." },
  { layer: "Banco master", tech: "PostgreSQL 16 (instância dedicada, HA)", why: "Mesma engine reduz carga cognitiva; instância separada para que um tenant ruidoso nunca toque a plataforma." },
  { layer: "Cache / sessão / lock", tech: "Redis 7", why: "Sessões revogáveis, refresh tokens, trava de numeração fiscal e cache de conexão por tenant." },
  { layer: "Filas", tech: "RabbitMQ (outbox + workers)", why: "Emissão fiscal, relatórios, e-mails e importações fora do caminho crítico. Retry e dead-letter nativos." },
  { layer: "Storage", tech: "MinIO / S3", why: "XML fiscais, DANFE, anexos e backups fora do banco, com retenção e versionamento." },
  { layer: "Autenticação", tech: "JWT RS256 + refresh rotation · Argon2id · TOTP (2FA)", why: "Access token curto (15 min), refresh com detecção de reuso, hash resistente a GPU/ASIC." },
  { layer: "Observabilidade", tech: "OpenTelemetry · Serilog → Seq · Prometheus + Grafana", why: "Traces com tenant_id e correlation_id, alertas de erro e dashboards por plano." },
  { layer: "Infra", tech: "Docker + Compose → Kubernetes · Linux · Caddy", why: "Começa simples e previsível; K8s somente quando a operação exigir. Secrets via Vault." },
  { layer: "Qualidade", tech: "xUnit · Testcontainers · Playwright", why: "Testes de integração contra PostgreSQL real e suite de isolamento cross-tenant obrigatória no CI." },
  { layer: "Contrato de API", tech: "OpenAPI 3.1 / Swagger + versionamento /vN", why: "Documentação viva, codegen do client do frontend e base para futura API pública e GraphQL." },
];

export const STACK_NOTE_SQLSERVER =
  "Alternativa considerada: SQL Server (experiência do time). Mantida viável via EF Core com provider trocável — porém o custo por banco e o ferramental de PITR/backup por tenant favorecem PostgreSQL como padrão da plataforma.";

/* ---------------- §03 fluxo de requisição ---------------- */

export const REQ_PIPELINE = [
  { t: "Cliente", d: "SPA chama /api/v1/… via HTTPS com Bearer token." },
  { t: "Borda", d: "TLS, WAF e rate limit por IP + por plano. Rejeição antes de tocar a aplicação." },
  { t: "Gateway", d: "Roteamento por versão, throttle fino e correlação (correlation-id gerado aqui)." },
  { t: "Autenticação", d: "Assinatura RS256, expiração e revogação da sessão em Redis. Falha → 401." },
  { t: "TenantContext", d: "tenant_id extraído da claim assinada — jamais de query/body/header customizado — e publicado em AsyncLocal." },
  { t: "Autorização", d: "Política RBAC (módulo:ação) + escopo de filial + módulo contratado no plano. Falha → 403." },
  { t: "Handler do módulo", d: "Regra de negócio em transação local; escrita no Outbox para efeitos assíncronos." },
  { t: "TenantDbContext", d: "Connection string resolvida pelo TenantRouter a partir do master (cache em Redis). Sem contexto → exceção fail-closed." },
  { t: "Resposta + auditoria", d: "200/201 padronizado (RFC 7807 para erros), log estruturado e trilha de auditoria quando aplicável." },
];

/* ---------------- §04 multi-tenancy ---------------- */

export const TENANCY_CMP = {
  cols: ["Isolamento real", "Custo inicial", "Ops / complexidade", "Migração de schema", "Ruído entre tenants", "Compliance (LGPD/fiscal)"],
  rows: [
    { strat: "Banco por tenant", sel: true, vals: ["Físico", "Médio", "Média (N bancos)", "Runner multi-banco", "Zero", "Forte"] },
    { strat: "Schema por tenant", sel: false, vals: ["Lógico", "Baixo", "Média", "Por schema", "Baixo", "Média"] },
    { strat: "Banco único + tenant_id", sel: false, vals: ["Por código", "Mínimo", "Baixa", "Trivial", "Alto", "Frágil"] },
  ],
};

export const TENANT_RULES = [
  "O tenant vem exclusivamente da sessão autenticada (claim assinada no JWT + sessão em Redis). Nenhum endpoint aceita tenant_id como parâmetro.",
  "TenantRouter resolve a connection string em ERP_MASTER.tenant_databases e cacheia em Redis com TTL curto.",
  "Todo DbContext exige TenantContext presente — ausência lança exceção (fail-closed, nunca conexão padrão).",
  "Credenciais dos bancos vivem em Vault; o master guarda apenas a referência (vault_key), nunca a senha.",
  "Provisionamento automático: nova assinatura → criação de ERP_EMPRESA_000NNN → baseline de migrations → registro no master → usuário admin inicial.",
];

export interface SimStep { gate: string; ok: boolean; note: string }
export interface SimScenario {
  id: string; title: string; actor: string; target: string; desc: string;
  steps: SimStep[];
  verdict: { code: string; ok: boolean; text: string; audit?: string };
}

export const SIM_SCENARIOS: SimScenario[] = [
  {
    id: "legit",
    title: "Fluxo legítimo",
    actor: "Maria · Vendedora · Empresa A (tenant 1001) · Filial Matriz",
    target: "GET /api/v1/vendas/2041",
    desc: "Usuária autorizada consultando uma venda do próprio tenant.",
    steps: [
      { gate: "TLS + WAF + rate limit", ok: true, note: "HTTPS 1.3, nenhum padrão de ataque, cota do plano OK." },
      { gate: "Assinatura JWT", ok: true, note: "RS256 confere, exp válido, claims íntegras." },
      { gate: "Sessão ativa", ok: true, note: "Sessão #a41f em Redis, dispositivo reconhecido, MFA cumprido." },
      { gate: "TenantContext", ok: true, note: "tenant_id=1001 extraído da claim assinada." },
      { gate: "RBAC + filial", ok: true, note: "Perfil 'Vendas' tem vendas.ver; Matriz no escopo da usuária." },
      { gate: "Roteamento de banco", ok: true, note: "TenantRouter → ERP_EMPRESA_0001001." },
      { gate: "Consulta escopada", ok: true, note: "Registro 2041 existe no banco da Empresa A." },
    ],
    verdict: { code: "200 OK", ok: true, text: "Payload retornado em 38 ms. Leitura registrada na auditoria do tenant.", audit: "READ vendas/2041 · maria@empresaA · 10.2.4.18" },
  },
  {
    id: "forged",
    title: "Token adulterado",
    actor: "Atacante · token da Empresa B com claim tenant_id forjada para 1001",
    target: "GET /api/v1/vendas/2041 (Empresa A)",
    desc: "Tentativa clássica: trocar o tenant dentro do JWT para 'entrar' em outra empresa.",
    steps: [
      { gate: "TLS + WAF + rate limit", ok: true, note: "Tráfego cifrado, sem bloqueio na borda." },
      { gate: "Assinatura JWT", ok: false, note: "Claims alteradas → assinatura não confere com a chave pública. Rejeição imediata, 401." },
    ],
    verdict: { code: "401 Unauthorized", ok: false, text: "A falsificação quebra a assinatura criptográfica. Nenhuma camada posterior é executada.", audit: "AUTH_FAIL token inválido · IP em log de abuso · contador anti-força-bruta++" },
  },
  {
    id: "tampered-id",
    title: "ID manipulado na URL",
    actor: "Carlos · usuário válido · Empresa B (tenant 1002)",
    target: "GET /api/v1/vendas/2041 (registro da Empresa A)",
    desc: "Usuário legítimo de outra empresa tenta acessar o registro trocando apenas o ID.",
    steps: [
      { gate: "TLS + WAF + rate limit", ok: true, note: "Requisição íntegra." },
      { gate: "Assinatura JWT", ok: true, note: "Token de Carlos é autêntico." },
      { gate: "Sessão ativa", ok: true, note: "Sessão válida." },
      { gate: "TenantContext", ok: true, note: "tenant_id=1002 — derivado da sessão, não do pedido." },
      { gate: "RBAC + filial", ok: true, note: "Carlos tem vendas.ver no próprio tenant." },
      { gate: "Roteamento de banco", ok: true, note: "TenantRouter → ERP_EMPRESA_0001002 (banco da Empresa B)." },
      { gate: "Consulta escopada", ok: false, note: "A venda 2041 não existe no banco da Empresa B — ela está fisicamente em outro servidor de dados." },
    ],
    verdict: { code: "404 Not Found", ok: false, text: "Vazamento impossível por construção: os dados da Empresa A nunca trafegam, nem filtrados. ID certo, banco errado — e o banco é decidido pela sessão.", audit: "READ vendas/2041 → não encontrado no tenant 1002 (tentativa registrada)" },
  },
  {
    id: "param",
    title: "Parâmetros forjados (?tenant=…)",
    actor: "Script automatizado · credencial válida da Empresa B",
    target: "GET /api/v1/vendas?tenant_id=1001&filial_id=7",
    desc: "Tentativa de sequestrar o contexto enviando tenant/filial como parâmetros de API.",
    steps: [
      { gate: "TLS + WAF + rate limit", ok: true, note: "Sem bloqueio de borda." },
      { gate: "Assinatura JWT", ok: true, note: "Token válido da Empresa B." },
      { gate: "Sessão ativa", ok: true, note: "Sessão válida." },
      { gate: "TenantContext", ok: true, note: "Parâmetros ignorados pelo model binding — nenhum handler recebe tenant_id como entrada. Contexto = 1002 (sessão)." },
      { gate: "RBAC + filial", ok: true, note: "Escopo de filial vem de usuario_filiais, não do query string." },
      { gate: "Roteamento de banco", ok: true, note: "→ ERP_EMPRESA_0001002." },
      { gate: "Consulta escopada", ok: false, note: "Sem filtros correspondentes → resultado vazio no banco da Empresa B." },
    ],
    verdict: { code: "200 { itens: [] }", ok: false, text: "Superfície de ataque eliminada no design: não existe entrada capaz de apontar para outro banco.", audit: "ANOMALY parâmetros de tenant ignorados · marcado para revisão de abuso" },
  },
];

/* ---------------- §05 banco master ---------------- */

export interface ColDef { n: string; t: string; k?: string }
export interface TableDef { name: string; desc: string; cols: ColDef[] }

export const MASTER_TABLES: TableDef[] = [
  {
    name: "tenants", desc: "Empresas contratantes — identidade do tenant.",
    cols: [
      { n: "id", t: "uuid", k: "PK" },
      { n: "slug", t: "varchar(40)", k: "UQ" },
      { n: "razao_social", t: "varchar(160)" },
      { n: "cnpj", t: "char(14)", k: "UQ" },
      { n: "status", t: "enum(trial,ativa,suspensa,cancelada)" },
      { n: "criado_em", t: "timestamptz" },
      { n: "excluido_em", t: "timestamptz null" },
    ],
  },
  {
    name: "plans", desc: "Planos comerciais (BÁSICO → ENTERPRISE) e limites.",
    cols: [
      { n: "id", t: "serial", k: "PK" },
      { n: "codigo", t: "varchar(30)", k: "UQ" },
      { n: "max_usuarios", t: "int" },
      { n: "max_filiais", t: "int" },
      { n: "max_armazenamento_gb", t: "int" },
      { n: "preco_mensal", t: "numeric(12,2)" },
    ],
  },
  {
    name: "plan_modules", desc: "Quais módulos cada plano habilita (multimódulo).",
    cols: [
      { n: "plan_id", t: "int", k: "FK→plans" },
      { n: "module", t: "varchar(30)" },
      { n: "", t: "", k: "PK(plan_id,module)" },
    ],
  },
  {
    name: "subscriptions", desc: "Assinatura vigente, trial, suspensão e cancelamento.",
    cols: [
      { n: "id", t: "uuid", k: "PK" },
      { n: "tenant_id", t: "uuid", k: "FK→tenants UQ" },
      { n: "plan_id", t: "int", k: "FK→plans" },
      { n: "status", t: "enum(trial,ativa,suspensa,cancelada)" },
      { n: "trial_termina_em", t: "timestamptz null" },
      { n: "periodo_atual_fim", t: "timestamptz" },
    ],
  },
  {
    name: "tenant_databases", desc: "Localização do banco de cada empresa — o coração do roteamento.",
    cols: [
      { n: "id", t: "serial", k: "PK" },
      { n: "tenant_id", t: "uuid", k: "FK→tenants UQ" },
      { n: "db_name", t: "varchar(60)", k: "UQ" },
      { n: "cluster_ref", t: "varchar(40)", k: "IDX" },
      { n: "vault_key", t: "varchar(120)" },
      { n: "schema_version", t: "int" },
      { n: "provisionado_em", t: "timestamptz" },
    ],
  },
  {
    name: "tenant_modules", desc: "Overrides: módulos ligados/desligados por empresa.",
    cols: [
      { n: "tenant_id", t: "uuid", k: "FK→tenants" },
      { n: "module", t: "varchar(30)" },
      { n: "enabled", t: "boolean" },
    ],
  },
  {
    name: "platform_users", desc: "Administradores da plataforma (SaaS), não das empresas.",
    cols: [
      { n: "id", t: "uuid", k: "PK" },
      { n: "email", t: "citext", k: "UQ" },
      { n: "password_hash", t: "text" },
      { n: "role", t: "enum(owner,admin,suporte)" },
      { n: "mfa_enabled", t: "boolean" },
    ],
  },
  {
    name: "global_migrations", desc: "Versionamento do schema do próprio master.",
    cols: [
      { n: "version", t: "varchar(40)", k: "PK" },
      { n: "aplicada_em", t: "timestamptz" },
      { n: "checksum", t: "varchar(64)" },
    ],
  },
  {
    name: "feature_flags", desc: "Chaves globais e rollout gradual por plano/tenant.",
    cols: [
      { n: "key", t: "varchar(60)", k: "PK" },
      { n: "enabled", t: "boolean" },
      { n: "rollout_pct", t: "smallint" },
    ],
  },
  {
    name: "integration_credentials", desc: "Credenciais externas (bancos, gateways) — só referências ao Vault.",
    cols: [
      { n: "id", t: "uuid", k: "PK" },
      { n: "tenant_id", t: "uuid", k: "FK→tenants IDX" },
      { n: "provider", t: "varchar(40)" },
      { n: "vault_ref", t: "varchar(120)" },
      { n: "ativa", t: "boolean" },
    ],
  },
  {
    name: "platform_audit", desc: "Trilha administrativa da plataforma (suspensões, provisionamentos…).",
    cols: [
      { n: "id", t: "bigserial", k: "PK" },
      { n: "actor_id", t: "uuid", k: "FK→platform_users" },
      { n: "tenant_id", t: "uuid null" },
      { n: "acao", t: "varchar(60)", k: "IDX" },
      { n: "detalhe", t: "jsonb" },
      { n: "em", t: "timestamptz", k: "IDX" },
    ],
  },
];

/* ---------------- §06 banco da empresa ---------------- */

export interface SchemaGroup { module: string; tables: TableDef[] }

export const TENANT_SCHEMA: SchemaGroup[] = [
  {
    module: "Organização & acesso",
    tables: [
      { name: "filiais", desc: "Matriz e filiais do mesmo tenant (CNPJ raiz comum).", cols: [
        { n: "id", t: "uuid", k: "PK" }, { n: "cnpj", t: "char(14)", k: "UQ" }, { n: "nome", t: "varchar(160)" },
        { n: "ie / im", t: "varchar" }, { n: "regime_tributario", t: "enum(simples,presumido,real)" },
        { n: "matriz", t: "boolean", k: "IDX" }, { n: "ativa", t: "boolean" },
      ]},
      { name: "filial_usuarios", desc: "Quem acessa cada filial — base do escopo de autorização.", cols: [
        { n: "filial_id", t: "uuid", k: "FK→filiais" }, { n: "usuario_id", t: "uuid", k: "FK→usuarios" }, { n: "", t: "", k: "PK(fialial,usuario)" },
      ]},
      { name: "usuarios", desc: "Contas de acesso da empresa.", cols: [
        { n: "id", t: "uuid", k: "PK" }, { n: "login", t: "varchar(60)", k: "UQ" }, { n: "email", t: "citext", k: "UQ" },
        { n: "password_hash", t: "text" }, { n: "mfa_secret_ref", t: "varchar null" }, { n: "status", t: "enum(ativo,bloqueado,inativo)" },
        { n: "ultimo_acesso_em", t: "timestamptz" }, { n: "ultimo_ip", t: "inet" },
      ]},
      { name: "sessoes", desc: "Refresh tokens e dispositivos revogáveis.", cols: [
        { n: "id", t: "uuid", k: "PK" }, { n: "usuario_id", t: "uuid", k: "FK→usuarios IDX" }, { n: "refresh_hash", t: "text", k: "UQ" },
        { n: "dispositivo", t: "varchar(160)" }, { n: "ip", t: "inet" }, { n: "expira_em", t: "timestamptz" }, { n: "revogada_em", t: "timestamptz null" },
      ]},
      { name: "perfis / permissoes", desc: "perfis, permissoes (módulo:ação), perfil_permissoes e usuario_perfis.", cols: [
        { n: "permissao.modulo", t: "varchar(30)", k: "IDX" }, { n: "permissao.acao", t: "varchar(30)" }, { n: "", t: "", k: "UQ(modulo,acao)" },
        { n: "perfil_permissoes", t: "N×N", k: "FK ambas" }, { n: "usuario_perfis", t: "N×N", k: "FK ambas" },
      ]},
    ],
  },
  {
    module: "Pessoas & relacionamentos",
    tables: [
      { name: "pessoas", desc: "Núcleo cadastral PF/PJ — compartilhado entre filiais.", cols: [
        { n: "id", t: "uuid", k: "PK" }, { n: "tipo", t: "enum(F,J)" }, { n: "cpf", t: "char(11) null", k: "UQ" },
        { n: "cnpj", t: "char(14) null", k: "UQ" }, { n: "nome / razao_social", t: "varchar(160)" }, { n: "nascimento / fundacao", t: "date null" },
        { n: "status", t: "enum(ativo,inativo)" },
      ]},
      { name: "pessoa_enderecos", desc: "1×N — vários endereços, um principal.", cols: [
        { n: "pessoa_id", t: "uuid", k: "FK→pessoas IDX" }, { n: "tipo", t: "enum(cobranca,entrega,fiscal,outro)" },
        { n: "cep / logradouro / uf", t: "varchar" }, { n: "principal", t: "boolean" },
      ]},
      { name: "pessoa_contatos", desc: "1×N — telefone, celular, WhatsApp, e-mail.", cols: [
        { n: "pessoa_id", t: "uuid", k: "FK→pessoas IDX" }, { n: "tipo", t: "enum(tel,cel,wpp,email,outro)" },
        { n: "valor", t: "varchar(120)" }, { n: "principal", t: "boolean" },
      ]},
      { name: "clientes", desc: "Módulo próprio: relação comercial 1×1 com pessoa.", cols: [
        { n: "id", t: "uuid", k: "PK" }, { n: "pessoa_id", t: "uuid", k: "FK→pessoas UQ" }, { n: "limite_credito", t: "numeric(14,2)" },
        { n: "condicao_pagamento_id", t: "uuid", k: "FK" }, { n: "tabela_preco_id", t: "uuid", k: "FK" }, { n: "vendedor_id", t: "uuid", k: "FK→funcionarios" },
        { n: "ind_ie", t: "enum(1,2,9)" }, { n: "consumidor_final", t: "boolean" },
      ]},
      { name: "fornecedores", desc: "Módulo próprio, nunca 'pessoa com flag'.", cols: [
        { n: "id", t: "uuid", k: "PK" }, { n: "pessoa_id", t: "uuid", k: "FK→pessoas UQ" }, { n: "condicao_id", t: "uuid", k: "FK" },
        { n: "prazo_medio_dias", t: "smallint" }, { n: "ativo", t: "boolean", k: "IDX" },
      ]},
      { name: "funcionarios", desc: "Estrutura independente, referenciando pessoa. Pronta para folha/eSocial.", cols: [
        { n: "id", t: "uuid", k: "PK" }, { n: "pessoa_id", t: "uuid", k: "FK→pessoas UQ" }, { n: "matricula", t: "varchar(20)", k: "UQ" },
        { n: "cargo / departamento_id", t: "varchar / FK" }, { n: "admissao / desligamento", t: "date" }, { n: "salario", t: "numeric(12,2)" },
        { n: "supervisor_id", t: "uuid", k: "FK→funcionarios" }, { n: "centro_custo_id", t: "uuid", k: "FK" }, { n: "comissao_pct", t: "numeric(5,2)" },
      ]},
    ],
  },
  {
    module: "Catálogo",
    tables: [
      { name: "produtos", desc: "Item completo: fiscal, dimensões, flags de uso e controles.", cols: [
        { n: "id", t: "uuid", k: "PK" }, { n: "codigo", t: "varchar(30)", k: "UQ" }, { n: "gtin", t: "varchar(14)", k: "UQ" },
        { n: "ncm", t: "char(8)", k: "IDX" }, { n: "cest", t: "char(7) null" }, { n: "unidade_id / unid_trib", t: "FK / char(6)" },
        { n: "custo / preco_base", t: "numeric(14,4)" }, { n: "estoque_min / max", t: "numeric(14,3)" },
        { n: "flags", t: "venda·compra·estoque·composto·grade" }, { n: "controle", t: "lote·validade·serie" },
      ]},
      { name: "produto_variacoes", desc: "Grades (cor/tamanho/voltagem) com GTIN próprio.", cols: [
        { n: "produto_id", t: "uuid", k: "FK→produtos IDX" }, { n: "atributos", t: "jsonb" }, { n: "gtin", t: "varchar(14)", k: "UQ" }, { n: "sku", t: "varchar(40)", k: "UQ" },
      ]},
      { name: "tabelas_preco(_itens)", desc: "Múltiplas tabelas por cliente/região/campanha.", cols: [
        { n: "tabela.nome / vigencia", t: "varchar / daterange" }, { n: "item.produto_id", t: "uuid", k: "FK IDX" },
        { n: "item.preco", t: "numeric(14,4)" }, { n: "item.desconto_pct", t: "numeric(5,2)" },
      ]},
      { name: "produto_fornecedores", desc: "Principal + alternativos com último custo.", cols: [
        { n: "produto_id", t: "uuid", k: "FK" }, { n: "fornecedor_id", t: "uuid", k: "FK" }, { n: "principal", t: "boolean" }, { n: "ultimo_custo", t: "numeric(14,4)" },
      ]},
    ],
  },
  {
    module: "Estoque",
    tables: [
      { name: "depositos", desc: "Por filial; um padrão de venda e um de recebimento.", cols: [
        { n: "id", t: "uuid", k: "PK" }, { n: "filial_id", t: "uuid", k: "FK→filiais IDX" }, { n: "nome", t: "varchar(60)" }, { n: "padrao", t: "boolean" },
      ]},
      { name: "estoque_saldos", desc: "Saldo derivado dos movimentos — nunca editado à mão.", cols: [
        { n: "filial_id · deposito_id · produto_id", t: "uuid×3", k: "PK composta" },
        { n: "disponivel", t: "numeric(14,3)" }, { n: "reservado", t: "numeric(14,3)" }, { n: "atualizado_em", t: "timestamptz" },
      ]},
      { name: "estoque_movimentos", desc: "Append-only: toda alteração tem origem tipada.", cols: [
        { n: "id", t: "bigserial", k: "PK" }, { n: "filial_id · deposito_id · produto_id", t: "uuid×3", k: "IDX" },
        { n: "tipo", t: "enum(entrada,saida,transferencia,ajuste,perda,devolucao,reserva)" },
        { n: "qtd", t: "numeric(14,3)" }, { n: "origem_tipo", t: "varchar(30)", k: "IDX" }, { n: "origem_id", t: "uuid" },
        { n: "usuario_id", t: "uuid" }, { n: "em", t: "timestamptz", k: "IDX" },
      ]},
      { name: "reservas", desc: "Comprometimento por pedido com expiração automática.", cols: [
        { n: "pedido_id", t: "uuid", k: "FK IDX" }, { n: "produto_id", t: "uuid" }, { n: "qtd", t: "numeric(14,3)" }, { n: "status", t: "enum(ativa,consumida,cancelada)" },
      ]},
    ],
  },
  {
    module: "Vendas & compras",
    tables: [
      { name: "orcamentos / pedidos(_itens)", desc: "Funil comercial; pedido gera reserva.", cols: [
        { n: "pedido.id", t: "uuid", k: "PK" }, { n: "cliente_id · filial_id · vendedor_id", t: "uuid×3", k: "IDX" },
        { n: "status", t: "enum(aberto,aprovado,faturado,cancelado)" }, { n: "itens: produto, qtd, preço, desconto", t: "numeric" },
      ]},
      { name: "vendas(_itens)(_pagamentos)", desc: "Faturamento com múltiplas formas de pagamento.", cols: [
        { n: "venda.id", t: "uuid", k: "PK" }, { n: "pedido_id", t: "uuid null", k: "FK" }, { n: "status", t: "enum(faturada,devolvida,cancelada)" },
        { n: "pagto.forma", t: "enum(pix,cartao,dinheiro,boleto,transferencia,credito)" }, { n: "pagto.valor / nsu", t: "numeric / varchar" },
      ]},
      { name: "pedidos_compra(_itens)", desc: "Do pedido ao recebimento parcial.", cols: [
        { n: "id", t: "uuid", k: "PK" }, { n: "fornecedor_id · filial_id", t: "uuid×2", k: "IDX" }, { n: "status", t: "enum(aberto,aprovado,recebido_parcial,recebido,cancelado)" },
        { n: "previsao_entrega", t: "date" },
      ]},
      { name: "entradas(_itens)", desc: "Entrada fiscal: alimenta estoque, custo médio e contas a pagar.", cols: [
        { n: "id", t: "uuid", k: "PK" }, { n: "pedido_compra_id", t: "uuid", k: "FK" }, { n: "documento_fiscal_id", t: "uuid", k: "FK" },
        { n: "frete / despesas_acessorias", t: "numeric(14,2)" }, { n: "impostos", t: "jsonb" },
      ]},
    ],
  },
  {
    module: "Financeiro",
    tables: [
      { name: "contas_receber / pagar", desc: "Títulos parcelados com encargos e baixas.", cols: [
        { n: "id", t: "uuid", k: "PK" }, { n: "cliente_id / fornecedor_id", t: "uuid", k: "IDX" }, { n: "origem", t: "venda_id / entrada_id", k: "FK" },
        { n: "parcela", t: "smallint" }, { n: "vencimento", t: "date", k: "IDX" }, { n: "valor / juros / multa / desconto", t: "numeric(14,2)" },
        { n: "status", t: "enum(aberta,parcial,recebida,vencida,renegociada,baixada)" },
      ]},
      { name: "caixas / caixa_movimentos", desc: "Abertura/fechamento por filial e operador.", cols: [
        { n: "caixa.filial_id · operador_id", t: "uuid×2", k: "IDX" }, { n: "aberto_em / fechado_em", t: "timestamptz" },
        { n: "mov.tipo", t: "enum(sangria,suprimento,venda,troco)" }, { n: "mov.valor", t: "numeric(14,2)" }, { n: "diferenca_conferencia", t: "numeric(14,2)" },
      ]},
      { name: "contas_bancarias", desc: "Por filial, prontas para conciliação futura.", cols: [
        { n: "filial_id", t: "uuid", k: "FK IDX" }, { n: "banco / agencia / conta", t: "varchar" }, { n: "pix_chave", t: "varchar null" }, { n: "conciliacao_ativa", t: "boolean" },
      ]},
      { name: "categorias / centros_custo", desc: "Plano de contas gerencial para rateios e BI.", cols: [
        { n: "categoria.tipo", t: "enum(receita,despesa)" }, { n: "categoria.pai_id", t: "uuid", k: "FK→self" },
        { n: "centro.departamento", t: "varchar" }, { n: "centro.filial_id", t: "uuid null" },
      ]},
    ],
  },
  {
    module: "Fiscal",
    tables: [
      { name: "series_fiscais", desc: "Numeração por filial + modelo (55, 65, NFS-e).", cols: [
        { n: "filial_id", t: "uuid", k: "FK IDX" }, { n: "modelo", t: "char(2)" }, { n: "serie", t: "int" }, { n: "proximo_numero", t: "bigint" }, { n: "", t: "", k: "UQ(filial,modelo,serie)" },
      ]},
      { name: "documentos_fiscais", desc: "NF-e/NFC-e/NFS-e/CT-e/MDF-e com snapshot de cálculo.", cols: [
        { n: "id", t: "uuid", k: "PK" }, { n: "filial_id", t: "uuid", k: "IDX" }, { n: "tipo", t: "enum(NFE,NFCE,NFSE,CTE,MDFE)" },
        { n: "chave", t: "char(44)", k: "UQ" }, { n: "status", t: "enum(autorizada,rejeitada,denegada,cancelada,contingencia)" },
        { n: "xml_ref", t: "varchar (S3)" }, { n: "impostos_snapshot", t: "jsonb" }, { n: "emitida_em", t: "timestamptz", k: "IDX" },
      ]},
      { name: "documento_eventos", desc: "Cancelamento, CC-e, inutilização, manifestação.", cols: [
        { n: "documento_id", t: "uuid", k: "FK IDX" }, { n: "tipo", t: "enum(cancelamento,cce,inutilizacao,manifestacao,consulta)" },
        { n: "protocolo", t: "varchar(20)" }, { n: "xml_ref", t: "varchar" },
      ]},
      { name: "regras_fiscais", desc: "Versionáveis por vigência — nenhuma regra é eterna.", cols: [
        { n: "id", t: "serial", k: "PK" }, { n: "operacao / uf_origem / uf_destino", t: "varchar×3", k: "IDX" },
        { n: "regime / ncm", t: "enum / char(8)" }, { n: "regra", t: "jsonb (ICMS, ST, DIFAL, FCP, PIS/COFINS…)" },
        { n: "vigencia", t: "daterange", k: "IDX GIST" }, { n: "versao", t: "int" },
      ]},
      { name: "certificados", desc: "A1 por filial: thumbprint + validade; a senha fica no Vault.", cols: [
        { n: "filial_id", t: "uuid", k: "FK IDX" }, { n: "tipo", t: "enum(A1)" }, { n: "thumbprint", t: "varchar(64)" },
        { n: "validade", t: "date", k: "IDX" }, { n: "vault_ref", t: "varchar(120)" }, { n: "status", t: "enum(ativo,expirando,expirado)" },
      ]},
    ],
  },
  {
    module: "Sistema",
    tables: [
      { name: "auditoria", desc: "Append-only, particionada por mês — sem UPDATE/DELETE.", cols: [
        { n: "id", t: "bigserial", k: "PK" }, { n: "usuario_id · usuario_nome", t: "uuid / varchar" }, { n: "ip · user_agent", t: "inet / text" },
        { n: "filial_id", t: "uuid", k: "IDX" }, { n: "modulo · operacao", t: "varchar", k: "IDX" }, { n: "entidade · registro_id", t: "varchar / uuid", k: "IDX" },
        { n: "antes / depois", t: "jsonb" }, { n: "em", t: "timestamptz", k: "IDX" },
      ]},
      { name: "notificacoes", desc: "Caixa interna + despacho para canais externos.", cols: [
        { n: "usuario_id", t: "uuid", k: "IDX" }, { n: "tipo", t: "varchar(40)" }, { n: "titulo / corpo", t: "text" },
        { n: "canais", t: "enum[] (interno,email,wpp,sms,push)" }, { n: "lida_em", t: "timestamptz null" },
      ]},
      { name: "integracoes_log", desc: "Cada chamada externa com payload em storage.", cols: [
        { n: "provider", t: "varchar(40)", k: "IDX" }, { n: "direcao", t: "enum(envio,recebimento)" }, { n: "status", t: "enum(ok,erro,retry)" },
        { n: "payload_ref", t: "varchar" }, { n: "tentativas", t: "smallint" }, { n: "em", t: "timestamptz", k: "IDX" },
      ]},
    ],
  },
];

/* ---------------- §07 autenticação ---------------- */

export const AUTHN_FLOW = [
  { t: "Credenciais + throttle", d: "POST /auth/login com captcha após 3 falhas e lockout progressivo por conta e por IP." },
  { t: "Verificação Argon2id", d: "Hash com params OWASP; comparação em tempo constante. Senha nunca trafega em log." },
  { t: "Segundo fator (TOTP)", d: "Obrigatório para perfis administrativos; código de 6 dígitos com janela ±1 e backup codes de uso único." },
  { t: "Emissão de tokens", d: "Access JWT RS256 (15 min) com sub, tenant_id, filiais[], perfis[]. Refresh opaco (30 dias) gravado em sessoes." },
  { t: "Rotação de refresh", d: "Cada uso gera novo refresh e revoga o anterior. Reuso de token antigo → revogação total (detecção de roubo)." },
  { t: "Sessões e dispositivos", d: "Lista de sessões ativas por usuário com revogação individual ou total; expiração absoluta de 12 h." },
];

export const JWT_SAMPLE = `{
  "alg": "RS256", "kid": "matriz-2026-01"
}
{
  "sub": "9f3c…",            // usuário
  "tenant_id": "1001",        // empresa — assinado, não editável
  "filiais": ["mz", "f01"],   // escopo de estabelecimentos
  "perfis": ["vendas", "fin"],
  "sid": "a41f…",             // id da sessão (p/ revogação)
  "exp": 1772123456           // 15 minutos
}`;

/* ---------------- §08 RBAC ---------------- */

export const RBAC_ACTIONS = ["ver", "criar", "editar", "excluir", "exportar", "aprovar", "cancelar", "desconto", "emitir", "baixar"];

export const RBAC_DEFAULTS: Record<string, string[]> = {
  "Clientes":   ["ver", "criar", "editar", "exportar"],
  "Produtos":   ["ver", "criar", "editar", "exportar"],
  "Vendas":     ["ver", "criar", "editar", "cancelar", "aprovar", "desconto"],
  "Compras":    ["ver", "criar", "aprovar"],
  "Estoque":    ["ver", "criar", "exportar"],
  "Financeiro": ["ver", "criar", "baixar", "exportar"],
  "Fiscal":     ["ver", "emitir", "cancelar"],
};

/* ---------------- §09 diretórios ---------------- */

export interface DirNode { name: string; note?: string; children?: DirNode[] }

export const DIR_TREE: DirNode = {
  name: "matriz-erp/",
  children: [
    { name: "apps/", children: [
      { name: "web/", note: "SPA do ERP (React + Vite) — consome /api/v1" },
      { name: "portal/", note: "Admin da plataforma (tenants, planos, provisionamento)" },
    ]},
    { name: "backend/", children: [
      { name: "src/", children: [
        { name: "Matriz.Core/", note: "Kernel: TenantContext, RBAC, eventos de domínio, Outbox, auditoria" },
        { name: "Matriz.Infrastructure/", note: "EF Core, TenantRouter, Redis, RabbitMQ, S3, Vault" },
        { name: "Matriz.Api/", note: "Composition root, middleware, versionamento, OpenAPI" },
        { name: "Matriz.Worker/", note: "Consumidores de fila: fiscal, relatórios, e-mail, backup" },
        { name: "modules/", children: [
          { name: "Matriz.Module.Auth/" }, { name: "Matriz.Module.Tenants/" }, { name: "Matriz.Module.Identity/" },
          { name: "Matriz.Module.Cadastros/", note: "Pessoas, clientes, fornecedores, funcionários" },
          { name: "Matriz.Module.Produtos/" }, { name: "Matriz.Module.Estoque/" }, { name: "Matriz.Module.Vendas/" },
          { name: "Matriz.Module.Compras/" }, { name: "Matriz.Module.Financeiro/" },
          { name: "Matriz.Module.Fiscal/", note: "Engine desacoplado: regras versionáveis + adapters SEFAZ" },
          { name: "Matriz.Module.Relatorios/" }, { name: "Matriz.Module.Notificacoes/" }, { name: "Matriz.Module.Integracoes/" },
        ]},
      ]},
      { name: "tests/", children: [
        { name: "Unit/" }, { name: "Integration/", note: "Testcontainers PostgreSQL" },
        { name: "Isolation/", note: "Suite cross-tenant obrigatória (§38)" },
      ]},
    ]},
    { name: "database/", children: [
      { name: "master/migrations/", note: "Schema do ERP_MASTER" },
      { name: "tenant/migrations/", note: "Baseline aplicado a cada ERP_EMPRESA_*" },
      { name: "seed/", note: "Tabelas de referência: NCM, CFOP, CST, unidades…" },
    ]},
    { name: "infra/", children: [
      { name: "docker-compose.yml" }, { name: "k8s/", note: "Helm charts (fase de escala)" }, { name: "terraform/" },
    ]},
    { name: "docs/", children: [
      { name: "adrs/", note: "Decisões numeradas e imutáveis" }, { name: "api/", note: "OpenAPI + guias" }, { name: "runbooks/", note: "DR, restore, contingência SEFAZ" },
    ]},
  ],
};

/* ---------------- §10 migrations ---------------- */

export const MIGRATION_STEPS = [
  { t: "Duas linhas de versão", d: "master/migrations (plataforma) e tenant/migrations (schema das empresas) evoluem separadas, com compatibilidade retroativa obrigatória (expand → migrate → contract)." },
  { t: "Provisionamento", d: "Nova empresa → CREATE DATABASE ERP_EMPRESA_000NNN → aplicação do baseline completo → registro em tenant_databases com schema_version." },
  { t: "Runner multi-tenant", d: "Job percorre tenants em lotes com advisory lock: aplica pendências, grava em __migrations do próprio banco e atualiza schema_version no master." },
  { t: "Canário", d: "Migração roda primeiro em um tenant interno de homologação; só depois segue para produção em ondas." },
  { t: "Falha isolada", d: "Erro em um banco pausa aquele tenant (modo manutenção de schema) e alerta — nunca interrompe os demais nem o master." },
];

export const MIGRATION_RULES = [
  "Rollback = forward-fix: migrações são cumulativas e idempotentes; 'voltar' é aplicar a próxima correção.",
  "Nenhuma alteração manual de schema em produção — todo DDL nasce de migration versionada.",
  "Migrações longas (backfill) rodam fora do horário comercial, em lotes, com progresso auditável.",
  "CI valida migrations contra um banco efêmero + snapshot do schema para detectar drift.",
];

/* ---------------- §11 backup ---------------- */

export const BACKUP_MATRIX = [
  { scope: "ERP_MASTER", full: "Diário 01h", inc: "WAL contínuo (PITR)", retention: "90 dias + anual", verify: "Restore semanal automatizado", rpo: "≤ 5 min", rto: "≤ 30 min" },
  { scope: "ERP_EMPRESA_* (cada banco)", full: "Diário 02h (escalonado)", inc: "WAL por cluster", retention: "30d diário · 12 sem. · 12 meses", verify: "Restore amostral semanal + checksum", rpo: "≤ 15 min", rto: "≤ 2 h" },
  { scope: "Storage (XML, PDF, anexos)", full: "Replicação entre regiões", inc: "Versionamento de objetos", retention: "XML fiscal: 5 anos (legal)", verify: "Listagem + hash mensal", rpo: "≤ 1 h", rto: "≤ 4 h" },
];

export const BACKUP_RULES = [
  "Backup de um tenant é um artefato independente: restaurar a Empresa A jamais toca a Empresa B.",
  "Game-day de DR trimestral: restauração completa de um tenant em ambiente isolado, cronometrada.",
  "Criptografia em repouso (AES-256) e chaves separadas por classe de dado.",
  "Cópias off-site em região distinta, imutáveis (object lock) contra ransomware.",
];

/* ---------------- §12 logs ---------------- */

export const LOG_PIPELINE = [
  { t: "Aplicação", d: "Serilog com enriquecimento automático: correlation_id, tenant_id, user_id, filial, módulo, versão." },
  { t: "Transporte", d: "Streams estruturados em JSON; erros com stack completo apenas no backend — nunca na resposta." },
  { t: "Seq / índice", d: "Busca por tenant + período; retenção de 90 dias quentes + arquivo frio." },
  { t: "Métricas", d: "OpenTelemetry → Prometheus: p95 por rota, taxa de erro, profundidade de fila, conexões por banco." },
  { t: "Alertas", d: "Picos de 401/403, rejeições SEFAZ, filas represadas e falhas de migração disparam on-call." },
];

export const LOG_SAMPLE = `{ "ts": "2026-02-11T14:32:07Z", "level": "Information",
  "corr": "8f2ab1", "tenant": "1001", "user": "maria",
  "filial": "mz", "mod": "Vendas",
  "msg": "Venda faturada", "venda": "2041",
  "itens": 4, "total": 1289.9, "dur_ms": 63 }`;

export const LOG_NEVER = ["senhas ou hashes", "chaves de certificado", "tokens completos", "CPF/CNPJ sem máscara", "payloads de cartão (PCI)"];

/* ---------------- §13 auditoria ---------------- */

export interface AuditEntry {
  id: string; user: string; when: string; ip: string; dev: string;
  mod: string; op: string; entity: string; rec: string;
  before?: string; after?: string; filial: string;
}

export const AUDIT_SAMPLES: AuditEntry[] = [
  { id: "aud_88231", user: "anderson", when: "01/09/2026 14:32", ip: "10.2.4.18", dev: "Chrome · Windows", mod: "Produtos", op: "UPDATE", entity: "produtos", rec: "PRD-0193 · Cabo HDMI 2.1", before: "preco_base: R$ 100,00", after: "preco_base: R$ 120,00", filial: "Matriz" },
  { id: "aud_88229", user: "maria", when: "01/09/2026 14:18", ip: "10.2.4.22", dev: "ERP SPA · Linux", mod: "Vendas", op: "CREATE", entity: "vendas", rec: "VND-2041", after: "total: R$ 1.289,90 · 3× sem juros · cliente C-118", filial: "Matriz" },
  { id: "aud_88210", user: "carlos.fiscal", when: "01/09/2026 11:05", ip: "10.2.5.3", dev: "ERP SPA · Windows", mod: "Fiscal", op: "CANCEL", entity: "documentos_fiscais", rec: "NFe 000.118 (chave …8842)", before: "status: autorizada", after: "status: cancelada · evento 110111 · protocolo 135260008812", filial: "Filial 01" },
  { id: "aud_88197", user: "diretoria", when: "01/09/2026 09:47", ip: "187.44.10.9", dev: "Safari · macOS", mod: "Identity", op: "UPDATE", entity: "perfil_permissoes", rec: "perfil 'Vendas'", before: "vendas.desconto: até 5%", after: "vendas.desconto: até 10%", filial: "Todas" },
  { id: "aud_88190", user: "—", when: "01/09/2026 03:12", ip: "45.190.22.7", dev: "desconhecido", mod: "Auth", op: "LOGIN_FAIL", entity: "usuarios", rec: "login 'admin' · 5 tentativas", after: "conta bloqueada 30 min · IP em allowlist de abuso", filial: "—" },
];

/* ---------------- §14 testes ---------------- */

export interface TestSuite { suite: string; tool: string; gate: string; cases: { name: string; expect: string }[] }

export const TEST_SUITES: TestSuite[] = [
  {
    suite: "Isolamento cross-tenant (OBRIGATÓRIO · §38)", tool: "xUnit + Testcontainers", gate: "Bloqueia merge — qualquer falha aqui congela o release",
    cases: [
      { name: "acesso_a_outro_banco_eh_bloqueado", expect: "Conexão fora do TenantRouter → exceção fail-closed" },
      { name: "alterar_outro_tenant_falha", expect: "Escrita com contexto trocado → 403 + auditoria" },
      { name: "consulta_cross_tenant_retorna_vazio", expect: "Registro de A é invisível para B (404/vazio)" },
      { name: "ids_manipulados_nao_vazam_dados", expect: "ID de outro banco → 404, nunca 200 com dados alheios" },
      { name: "parametros_de_api_sao_ignorados", expect: "?tenant_id=… não altera o contexto resolvido" },
      { name: "token_de_outro_contexto_eh_rejeitado", expect: "JWT válido de B contra recursos de A → 401/403" },
    ],
  },
  {
    suite: "Unidade — domínio", tool: "xUnit", gate: "Cobertura ≥ 80% nos núcleos",
    cases: [
      { name: "estoque_nunca_negativo", expect: "Reserva > disponível → DomainException" },
      { name: "juros_multa_desconto", expect: "Cálculo financeiro com rounding bancário" },
      { name: "icms_st_difal", expect: "Cenários por UF/regime contra tabela de expectativas" },
      { name: "parcelamento", expect: "Soma das parcelas == total (ajuste na última)" },
    ],
  },
  {
    suite: "Integração", tool: "Testcontainers (PostgreSQL + Redis)", gate: "Pipeline verde antes de staging",
    cases: [
      { name: "venda_transacional", expect: "Venda + reserva + contas a receber + movimento em uma transação; falha → tudo revertido" },
      { name: "migrations_tenant", expect: "Baseline sobe limpo em banco vazio e idempotente na re-execução" },
      { name: "outbox_entrega", expect: "Evento persistido e publicado exatamente uma vez (idempotência no consumer)" },
    ],
  },
  {
    suite: "API (contrato)", tool: "OpenAPI diff + xUnit", gate: "Quebra de contrato exige versão nova /vN",
    cases: [
      { name: "erros_rfc7807", expect: "4xx/5xx com problem+json padronizado, sem stack trace" },
      { name: "rate_limit_por_plano", expect: "429 com Retry-After quando a cota estoura" },
    ],
  },
  {
    suite: "E2E críticos", tool: "Playwright", gate: "Fluxos de receita antes de cada release",
    cases: [
      { name: "login_mfa_ate_dashboard", expect: "Sessão criada, filial padrão carregada" },
      { name: "pedido_faturado_gera_cadeia_completa", expect: "Estoque ↓, título a receber ↑, NF-e na fila" },
    ],
  },
];

/* ---------------- §16 riscos ---------------- */

export interface Risk { id: string; title: string; prob: number; impact: number; desc: string; mitigation: string[] }

export const RISKS: Risk[] = [
  { id: "R1", title: "Vazamento cross-tenant", prob: 2, impact: 5, desc: "O risco existencial do SaaS: um tenant enxergar dados de outro.", mitigation: ["Database-per-tenant (isolamento físico)", "Suite de isolamento §38 bloqueando merge", "Fail-closed no TenantDbContext", "Revisão de código obrigatória em tudo que toca TenantContext"] },
  { id: "R2", title: "Exaustão de conexões (N bancos)", prob: 3, impact: 3, desc: "Dezenas de connection strings × pool podem esgotar o servidor.", mitigation: ["PgBouncer em modo transacional", "Pools pequenos por tenant com lazy-open e idle timeout agressivo", "Conexões cacheadas por cluster, não por tenant"] },
  { id: "R3", title: "Migração falhando em N bancos", prob: 3, impact: 3, desc: "Um DDL ruim replicado para todos os bancos simultaneamente.", mitigation: ["Canário em tenant de homologação", "Ondas progressivas + advisory lock", "Falha isola um banco, não a plataforma"] },
  { id: "R4", title: "Complexidade e mudança fiscal", prob: 4, impact: 3, desc: "Legislação brasileira muda constantemente; regras hardcoded apodrecem.", mitigation: ["regras_fiscais versionadas por vigência", "Fiscal Engine desacoplado com testes de cenário por UF", "Snapshot de cálculo por documento (regra da época)"] },
  { id: "R5", title: "Indisponibilidade da SEFAZ", prob: 3, impact: 4, desc: "Autorizadoras fora do ar travam o faturamento.", mitigation: ["Fila com retry exponencial", "Contingência EPEC/offline prevista em produto", "Dashboard de status por UF"] },
  { id: "R6", title: "Inconsistência venda→estoque→financeiro", prob: 2, impact: 5, desc: "Venda gravada sem baixar estoque ou gerar título — o clássico.", mitigation: ["Transação local única para o núcleo síncrono", "Outbox para efeitos assíncronos com idempotência", "Teste de integração 'venda_transacional' no CI"] },
  { id: "R7", title: "Master como ponto único", prob: 1, impact: 5, desc: "Sem o master, nenhum tenant é roteado.", mitigation: ["HA com réplica síncrona + failover automático", "Cache do roteamento em Redis com TTL estendido em emergência", "Backup com RPO ≤ 5 min e game-day trimestral"] },
  { id: "R8", title: "Performance em milhões de registros", prob: 3, impact: 3, desc: "Movimentos, títulos e auditoria crescem sem limite.", mitigation: ["Particionamento por mês (movimentos, auditoria)", "Paginação obrigatória em listas", "Read models para relatórios/BI fora do banco transacional"] },
  { id: "R9", title: "Dependência de conhecimento-chave", prob: 3, impact: 2, desc: "Fiscal e multi-tenancy exigem conhecimento profundo e concentrado.", mitigation: ["ADRs documentando o porquê", "Runbooks de operação", "Pair review em módulos críticos"] },
  { id: "R10", title: "Crescimento descontrolado de escopo", prob: 4, impact: 2, desc: "Construir os 40 requisitos de uma vez dilui a qualidade da fundação.", mitigation: ["Gates de aprovação entre fases (§39)", "Critérios de saída objetivos por fase", "Multimódulo: vender só o que está pronto"] },
];

/* ---------------- §17 ADRs ---------------- */

export interface Adr { code: string; title: string; status: string; problema: string; alternativas: { nome: string; nota: string }[]; decisao: string; consequencias: string }

export const ADRS: Adr[] = [
  {
    code: "ADR-001", title: "Database-per-tenant", status: "Aceita",
    problema: "Como garantir que dados de empresas diferentes jamais se misturem?",
    alternativas: [
      { nome: "Banco único + tenant_id", nota: "Barato, mas o isolamento depende de disciplina em toda query — uma vírgula errada vaza dados." },
      { nome: "Schema por tenant", nota: "Meio-termo; ainda compartilha engine, locks e plano de execução." },
    ],
    decisao: "Um banco físico por empresa (ERP_EMPRESA_000NNN) com roteamento pelo contexto autenticado.",
    consequencias: "Ops mais elaborado (N bancos, runner de migrations), em troca de isolamento físico, compliance simples e venda/restore por cliente.",
  },
  {
    code: "ADR-002", title: "Monólito modular, não microserviços", status: "Aceita",
    problema: "Como estruturar 15+ domínios sem o custo prematuro de serviços distribuídos?",
    alternativas: [
      { nome: "Microserviços desde o dia 1", nota: "Transações distribuídas, deploy complexo, latência — antes de ter escala que justifique." },
      { nome: "Monólito tradicional", nota: "Rápido no início, vira big ball of mud sem limites explícitos." },
    ],
    decisao: "Modular monolith em .NET 10: módulos com boundaries físicos, comunicação interna por eventos de domínio, extração futura por costura limpa.",
    consequencias: "Deploy único e transações locais simples; exige disciplina de dependências (Core ← módulos, nunca entre módulos).",
  },
  {
    code: "ADR-003", title: "PostgreSQL como engine padrão", status: "Aceita",
    problema: "PostgreSQL ou SQL Server para N bancos por instância?",
    alternativas: [
      { nome: "SQL Server", nota: "Familiar ao time; porém custo por banco e licenciamento pesam no modelo SaaS." },
    ],
    decisao: "PostgreSQL 16, mantendo portabilidade via EF Core (provider trocável) para não queimar a experiência do time.",
    consequencias: "PITR por banco, JSONB para snapshots fiscais, particionamento nativo; curva de adaptação inicial do time coberta por treinamento.",
  },
  {
    code: "ADR-004", title: "JWT curto + refresh com rotação e sessões server-side", status: "Aceita",
    problema: "Como ter stateless suficiente para escala, mas revogação imediata quando preciso?",
    alternativas: [
      { nome: "Sessão pura (cookie)", nota: "Revogação fácil, mas acopla toda request ao store de sessão." },
      { nome: "JWT longo sem rotação", nota: "Janela de roubo enorme em caso de vazamento." },
    ],
    decisao: "Access JWT 15 min (claims de contexto) + refresh opaco 30 dias com rotação, detecção de reuso e lista de sessões revogáveis.",
    consequencias: "Revogação instantânea por sessão/dispositivo; exige Redis altamente disponível para checagem de sid.",
  },
  {
    code: "ADR-005", title: "Fiscal Engine desacoplado com regras versionáveis", status: "Aceita",
    problema: "Regras fiscais espalhadas em vendas/produtos/financeiro tornam o sistema refém da legislação.",
    alternativas: [
      { nome: "Regras inline nos módulos", nota: "Rápido de escrever, impossível de auditar e atualizar por vigência." },
    ],
    decisao: "Módulo Fiscal com tabelas de regras por operação/UF/regime/NCM com daterange de vigência; cálculo sempre gera snapshot por documento.",
    consequencias: "Documento antigo reaberto recalcula com a regra da época; novos tributos entram sem tocar os módulos de negócio.",
  },
  {
    code: "ADR-006", title: "Outbox para efeitos assíncronos", status: "Aceita",
    problema: "Emissão fiscal, e-mail e relatórios não podem nem travar a venda, nem se perder se a venda commitar.",
    alternativas: [
      { nome: "Publicar na fila dentro da transação", nota: "Risco de fantasma (fila sem banco) ou banco sem fila." },
      { nome: "Cron varrendo tabelas", nota: "Latência alta e polling custoso." },
    ],
    decisao: "Transactional outbox: eventos gravados na mesma transação do banco da empresa; relay publica no RabbitMQ com idempotência.",
    consequencias: "Exatamente-uma-entrega prática nos consumers; NF-e emitida segundos após o faturamento, com retry e DLQ.",
  },
  {
    code: "ADR-007", title: "Auditoria append-only particionada", status: "Aceita",
    problema: "Operações críticas não podem desaparecer nem ser reescritas — nem por DBA.",
    alternativas: [
      { nome: "Tabela comum com UPDATE", nota: "A trilha deixa de ser evidência." },
    ],
    decisao: "auditoria sem permissão de UPDATE/DELETE (role separada), particionada por mês, antes/depois em JSONB, export frio após 12 meses.",
    consequencias: "Crescimento gerenciado por particionamento; trilha serve como prova em disputas e suporte.",
  },
  {
    code: "ADR-008", title: "Segredos em Vault, referências no banco", status: "Aceita",
    problema: "Senhas de banco por tenant, certificados A1 e credenciais bancárias não podem viver em tabelas ou repositório.",
    alternativas: [
      { nome: "Criptografia em coluna com chave no app", nota: "A chave acaba no deploy — teatro de segurança." },
    ],
    decisao: "Vault (secrets engine) com rotação automática; banco guarda apenas vault_ref; acesso ao Vault por workload identity.",
    consequencias: "Rotação sem redeploy; perda do Vault = runbook de DR específico, testado em game-day.",
  },
];

/* ---------------- §15 roadmap ---------------- */

export interface Phase { n: number; title: string; dur: string; goal: string; deliv: string[]; exit: string[] }

export const PHASES: Phase[] = [
  {
    n: 1, title: "Fundação", dur: "Este documento + implementação guiada",
    goal: "A plataforma existe: tenants isolados, gente autenticada e autorizada, tudo auditado.",
    deliv: [
      "ERP_MASTER + provisionamento automático de ERP_EMPRESA_000NNN",
      "Auth completo (JWT + refresh + MFA + sessões)",
      "RBAC granular com escopo por filial",
      "Empresa/matriz/filiais e compartilhado × exclusivo",
      "Auditoria append-only + logs estruturados",
      "Layout base do ERP + dashboard inicial",
      "Suite de isolamento §38 verde no CI",
    ],
    exit: ["Nenhum dos 6 ataques de §38 vaza dados", "Tenant criado do zero em < 5 min por job", "Cobertura dos núcleos ≥ 80%"],
  },
  {
    n: 2, title: "Cadastros", dur: "Ciclo seguinte",
    goal: "Dados mestres profissionais, compartilhados entre filiais com exceções controladas.",
    deliv: ["Pessoas PF/PJ + endereços + contatos", "Clientes com crédito, tabela e vendedor", "Fornecedores com condição e prazo", "Funcionários como módulo próprio", "Produtos, unidades, grupos, marcas, categorias"],
    exit: ["Importação CSV/Excel validada com log de erros", "Duplicidade bloqueada por CPF/CNPJ/GTIN", "Auditoria cobrindo todos os cadastros"],
  },
  {
    n: 3, title: "Estoque · Vendas · Compras", dur: "Ciclo seguinte",
    goal: "O coração operacional transacionando com consistência total.",
    deliv: ["Saldos por filial/depósito + movimentos rastreados", "Orçamento → pedido → venda (balcão inclusa)", "Múltiplos pagamentos, comissões, devoluções", "Compra: solicitação → cotação → pedido → entrada"],
    exit: ["Teste venda_transacional verde (tudo ou nada)", "Estoque jamais negativo sob concorrência", "Reserva liberada automaticamente no cancelamento"],
  },
  {
    n: 4, title: "Financeiro", dur: "Ciclo seguinte",
    goal: "Dinheiro sob controle: títulos, caixa e bancos por filial.",
    deliv: ["Contas a receber/pagar com parcelas e encargos", "Renegociação e baixa parcial", "Caixa com abertura/fechamento/sangria/suprimento", "Contas bancárias + base de conciliação", "Centros de custo e categorias"],
    exit: ["DRE gerencial fecha com o razão dos títulos", "Caixa cego: conferência aponta diferença", "Aging de inadimplência por cliente e filial"],
  },
  {
    n: 5, title: "Fiscal", dur: "Ciclo seguinte",
    goal: "Emissão brasileira de ponta a ponta, com contingência.",
    deliv: ["Regras versionáveis + motor de cálculo", "NF-e e NFC-e com eventos, CC-e e cancelamento", "Certificado A1 via Vault + alertas de validade", "Armazenamento de XML + DANFE", "Entrada fiscal integrada à compra"],
    exit: ["Homologação SEFAZ aprovada em 2 UFs", "Rejeições tratadas com fila e reprocesso", "Snapshot fiscal por documento auditável"],
  },
  {
    n: 6, title: "Relatórios · BI · Integrações", dur: "Ciclo seguinte",
    goal: "Decisão sobre dados e ecossistema conectado.",
    deliv: ["Relatórios com filtros e exportação PDF/Excel/CSV", "Read models + dashboards configuráveis por perfil", "Adapters: bancos/PIX, gateways, e-commerce, contabilidade", "API pública /v1 documentada (OpenAPI)"],
    exit: ["Relatório pesado fora do banco transacional", "Webhook de integração com retry e log", "SLA de API medido e publicado"],
  },
];

/* ---------------- controle do documento ---------------- */

export const REVISIONS = [
  { rev: "0.1", date: "05/01/2026", author: "Arquitetura", desc: "Rascunho inicial a partir dos requisitos §1–§46" },
  { rev: "0.9", date: "28/01/2026", author: "Arquitetura + Eng. de Plataforma", desc: "Revisão de segurança, testes de isolamento e ADRs" },
  { rev: "1.0", date: "11/02/2026", author: "Arquitetura", desc: "Submetido para aprovação — fundação da Fase 1" },
];
