/* ============================================================
   MATRIZ · Blueprint de Arquitetura v1.0
   Conteúdo técnico do dossiê — ERP SaaS Multiempresa
   ============================================================ */

export type NavItem = { id: string; num: string; label: string };

export const NAV: NavItem[] = [
  { id: "capa", num: "00", label: "Visão geral" },
  { id: "arquitetura", num: "01", label: "Arquitetura" },
  { id: "stack", num: "02", label: "Stack" },
  { id: "tenancy", num: "03", label: "Multi-tenancy" },
  { id: "bancos", num: "04", label: "Modelo de bancos" },
  { id: "auth", num: "05", label: "Autenticação" },
  { id: "rbac", num: "06", label: "Autorização · RBAC" },
  { id: "auditoria", num: "07", label: "Auditoria & logs" },
  { id: "operacao", num: "08", label: "Estrutura & operação" },
  { id: "qualidade", num: "09", label: "Testes & riscos" },
  { id: "roadmap", num: "10", label: "Roadmap & ADRs" },
  { id: "aprovacao", num: "11", label: "Aprovação" },
];

export const STATS = [
  { v: 47, l: "requisitos mapeados" },
  { v: 18, l: "entregas neste blueprint" },
  { v: 6, l: "fases de roadmap" },
  { v: 6, l: "decisões arquiteturais" },
  { v: 12, l: "domínios modularizados" },
];

export const TICKER = [
  "DATABASE-PER-TENANT",
  "NUNCA CONFIE NO FRONTEND",
  "RBAC GRANULAR POR OPERAÇÃO",
  "AUDITORIA IMUTÁVEL",
  "SAGA TRANSACIONAL DE VENDA",
  "FISCAL DESACOPLADO EM WORKER",
  "MIGRATIONS VERSIONADAS",
  "BACKUP INDIVIDUAL POR TENANT",
  "JWT + REFRESH COM ROTAÇÃO",
  "API /v1 · OPENAPI 3.1",
];

/* ---------------- 01 · Arquitetura ---------------- */

export type Componente = { nome: string; tech: string; porque: string };
export type Camada = { id: string; nome: string; desc: string; componentes: Componente[] };

export const CAMADAS: Camada[] = [
  {
    id: "canais",
    nome: "Canais",
    desc: "Superfícies de consumo da plataforma.",
    componentes: [
      { nome: "Web ERP", tech: "Next.js 15 · React 19 · TS", porque: "App autenticado SPA-style com design system próprio; desktop-first, responsivo até mobile." },
      { nome: "Portal do assinante", tech: "Next.js (SSR público)", porque: "Contratação de planos, faturas, status da assinatura e convite de usuários fora do ERP." },
      { nome: "API pública", tech: "REST /api/v1 · OpenAPI 3.1", porque: "Integrações de clientes (e-commerce, contabilidade) com chaves por tenant, rate limit por plano e versionamento." },
      { nome: "App mobile", tech: "Futuro (fase 6+)", porque: "A mesma API /v1 alimenta apps futuros sem mudança de backend — por isso a API é tratada como produto." },
    ],
  },
  {
    id: "edge",
    nome: "Edge & segurança",
    desc: "Fronteira de rede: TLS, filtragem e limites.",
    componentes: [
      { nome: "Reverse proxy + WAF", tech: "TLS 1.3 · OWASP ruleset", porque: "Terminação HTTPS, proteção contra XSS/SQLi na borda e headers de segurança (CSP, HSTS)." },
      { nome: "Rate limiting", tech: "por tenant × plano (Redis)", porque: "Um tenant não pode degradar os demais — limite por IP, por usuário e por chave de API." },
      { nome: "CDN de estáticos", tech: "assets imutáveis", porque: "Performance de carregamento do ERP em qualquer região do Brasil." },
    ],
  },
  {
    id: "core",
    nome: "Núcleo aplicacional",
    desc: "Monolito modular .NET 10 — um deploy, doze bounded contexts.",
    componentes: [
      { nome: "Identity & Acesso", tech: "módulo", porque: "Usuários, login, 2FA, sessões, dispositivos, recuperação de senha." },
      { nome: "Tenants & Empresas", tech: "módulo", porque: "Resolução de tenant, empresas, filiais, módulos contratados." },
      { nome: "Cadastros", tech: "módulo", porque: "Pessoas (+endereços/contatos), clientes, fornecedores e funcionários como estruturas independentes." },
      { nome: "Produtos", tech: "módulo", porque: "Catálogo rico (NCM, CEST, grades, lotes) com múltiplas tabelas de preço." },
      { nome: "Estoque", tech: "módulo", porque: "Saldo por filial/depósito derivado de movimentações imutáveis — nunca editado direto." },
      { nome: "Vendas", tech: "módulo", porque: "Orçamento → pedido → venda em saga transacional (reserva, faturamento, fiscal, financeiro)." },
      { nome: "Compras", tech: "módulo", porque: "Solicitação → cotação → pedido → recebimento → entrada fiscal → contas a pagar." },
      { nome: "Financeiro", tech: "módulo", porque: "A receber/pagar com parcelas, caixa por operador, bancos e conciliação." },
      { nome: "Fiscal", tech: "módulo + adapters", porque: "Regras versionadas (ICMS/ST/IPI/PIS/COFINS/DIFAL/FCP) e adaptação por UF/município — isolado do resto do ERP." },
      { nome: "Relatórios & BI", tech: "módulo", porque: "Geração pesada em fila; camada de BI preparada para o futuro sem reescrita." },
      { nome: "Notificações", tech: "módulo", porque: "Canais desacoplados (interna, e-mail, WhatsApp, SMS, push) via adapters." },
      { nome: "Integrações", tech: "módulo", porque: "SEFAZ, bancos, gateways, marketplaces — cada um atrás de um adapter com contrato estável." },
    ],
  },
  {
    id: "async",
    nome: "Processamento assíncrono",
    desc: "Nada que demora bloqueia a interface — tudo passa por fila.",
    componentes: [
      { nome: "Fiscal.Worker", tech: "RabbitMQ", porque: "Emissão NF-e/NFC-e/NFS-e, eventos, cancelamento e contingência fora do request HTTP." },
      { nome: "Notificações.Worker", tech: "RabbitMQ", porque: "E-mail/WhatsApp/SMS/push com retry, dead-letter e log de entrega." },
      { nome: "Relatórios.Worker", tech: "RabbitMQ", porque: "PDF/Excel/CSV pesados gerados fora da API; usuário é notificado quando termina." },
      { nome: "Backup.Worker", tech: "RabbitMQ · cron", porque: "Backups por tenant, verificação de integridade e offload para storage de objetos." },
      { nome: "Integrações.Worker", tech: "RabbitMQ", porque: "Conciliação bancária, importação de XML, sincronização com marketplaces." },
    ],
  },
  {
    id: "dados",
    nome: "Dados",
    desc: "Isolamento físico: um banco por empresa, mais o banco master.",
    componentes: [
      { nome: "ERP_MASTER", tech: "PostgreSQL 16", porque: "Tenants, planos, assinaturas, módulos contratados e o mapa tenant → banco. Único banco compartilhado da plataforma." },
      { nome: "ERP_EMPRESA_XXXXXX", tech: "PostgreSQL 16 · 1 por empresa", porque: "Todo dado operacional da empresa: cadastros, estoque, vendas, financeiro, fiscal, auditoria." },
      { nome: "Redis", tech: "cache · sessão · rate limit", porque: "Hot path do tenant resolver, locks distribuídos e limites de API." },
      { nome: "MinIO / S3", tech: "storage de objetos", porque: "XMLs fiscais, DANFEs, anexos e backups — criptografados em repouso." },
    ],
  },
  {
    id: "plataforma",
    nome: "Plataforma & observabilidade",
    desc: "Operação, segredos e telemetria de ponta a ponta.",
    componentes: [
      { nome: "OpenTelemetry", tech: "Grafana · Loki · Tempo", porque: "Logs estruturados, traces com correlation_id cruzando HTTP e filas, métricas por tenant." },
      { nome: "Vault", tech: "HashiCorp", porque: "Senhas de banco, chaves de assinatura JWT e certificados A1 — nada sensível em repositório." },
      { nome: "CI/CD", tech: "GitHub Actions · Docker · Linux", porque: "Pipeline com suite de isolamento cross-tenant obrigatória antes de qualquer deploy." },
      { nome: "Provisionamento", tech: "worker + templates", porque: "Nova assinatura cria o banco da empresa, aplica migrations e seed automaticamente." },
    ],
  },
];

/* ---------------- 02 · Stack ---------------- */

export type StackRow = { area: string; tech: string; porque: string };

export const STACK: StackRow[] = [
  { area: "Frontend ERP", tech: "Next.js 15 · React 19 · TypeScript · TanStack Query", porque: "Design system próprio, estado de servidor com cache e invalidação, base para PWA e app futuro." },
  { area: "Backend", tech: ".NET 10 · ASP.NET Core — monolito modular", porque: "Performance, maturidade corporativa e módulos por bounded context prontos para extração futura." },
  { area: "Acesso a dados", tech: "EF Core 10 + SQL direto em relatórios", porque: "Migrations versionadas, interceptors para auditoria e escape hatch para consultas analíticas." },
  { area: "Bancos", tech: "PostgreSQL 16 (master + tenants)", porque: "Custo de licenciamento compatível com SaaS, PITR/WAL, particionamento e replicas nativas (ver ADR-003)." },
  { area: "Cache & limites", tech: "Redis 7", porque: "Tenant resolver, refresh tokens revogáveis, rate limiting por plano e locks de caixa/estoque." },
  { area: "Filas", tech: "RabbitMQ 3.13", porque: "Workers desacoplados com retry, dead-letter e priorização para fiscal e relatórios." },
  { area: "Objetos", tech: "MinIO (compatível S3)", porque: "XML/DANFE/anexos/backups com versionamento e criptografia; migrável para S3 sem trocar código." },
  { area: "Autenticação", tech: "JWT RS256 15min + refresh com rotação + TOTP", porque: "Tokens curtos limitam dano de vazamento; rotação detecta reuso; senhas com Argon2id." },
  { area: "Fiscal", tech: "Fiscal.Worker + adapters SEFAZ/NFS-e", porque: "Cada UF/município atrás de um adapter; regras versionadas em banco, nunca hardcoded." },
  { area: "Observabilidade", tech: "OpenTelemetry · Grafana · Loki · Tempo", porque: "Correlation_id único de HTTP → fila → worker → banco; dashboards e alertas por tenant." },
  { area: "Segredos", tech: "HashiCorp Vault", porque: "Credenciais de banco por tenant, chaves JWT rotacionáveis e certificados A1 protegidos." },
  { area: "Infra", tech: "Docker · Linux · Terraform · GitHub Actions", porque: "Ambientes reproduzíveis; infraestrutura como código; deploy azul/verde do monolito." },
  { area: "Contrato de API", tech: "REST /api/v1 · OpenAPI 3.1 · ProblemDetails (RFC 7807)", porque: "Versionamento explícito, documentação viva e erros padronizados — sem stack trace para o cliente." },
  { area: "Testes", tech: "xUnit · Testcontainers · Playwright", porque: "PostgreSQL real em container nos testes de integração; E2E nos fluxos críticos e de isolamento." },
];

/* ---------------- Saga de venda ---------------- */

export const SAGA_PASSOS = [
  { id: "pedido", t: "Pedido confirmado", d: "Validações comerciais, preço da tabela, crédito do cliente." },
  { id: "reserva", t: "Reserva de estoque", d: "Saldo comprometido por depósito/filial, com expiração." },
  { id: "fat", t: "Faturamento", d: "Geração da venda e snapshot fiscal por item (NCM, CFOP, CST)." },
  { id: "nfe", t: "NF-e na fila", d: "Fiscal.Worker assina, transmite à SEFAZ e guarda XML + protocolo." },
  { id: "fin", t: "Contas a receber", d: "Parcelas geradas conforme condição de pagamento; comissão apurada." },
  { id: "mov", t: "Movimentação de estoque", d: "Saída imutável registrada; saldo disponível recalculado." },
  { id: "aud", t: "Auditoria completa", d: "Cada passo deixa rastro: usuário, IP, valores antes/depois." },
];

/* ---------------- 03 · Multi-tenancy ---------------- */

export type OpcaoTenancy = {
  nome: string; como: string; pros: string[]; contras: string[]; escolhido?: boolean;
};

export const OPCOES_TENANCY: OpcaoTenancy[] = [
  {
    nome: "Banco compartilhado",
    como: "Todas as empresas no mesmo banco, separadas por uma coluna tenant_id em cada tabela.",
    pros: ["Operação simples", "Um único backup", "Custo mínimo inicial"],
    contras: [
      "Isolamento apenas lógico — um bug de query mistura empresas",
      "Vizinho barulhento degrada todos",
      "Restauração individual impossível",
      "Viola o requisito nº 2 do projeto",
    ],
  },
  {
    nome: "Schema por tenant",
    como: "Mesma instância, um schema PostgreSQL por empresa.",
    pros: ["Separação razoável", "Migração de instância única"],
    contras: [
      "Mesma instância = mesmo ponto de falha e de carga",
      "Migrations ×N schemas na mesma operação",
      "Isolamento ainda parcial (superuser, extensões)",
    ],
  },
  {
    nome: "Database por tenant",
    como: "ERP_EMPRESA_XXXXXX — um banco físico exclusivo por empresa, mapeado no ERP_MASTER.",
    pros: [
      "Isolamento físico — o dado do vizinho não existe no meu banco",
      "Backup/restore individual por empresa",
      "Escala e manutenção independentes",
      "Argumento comercial real de segurança",
    ],
    contras: ["Mais bancos para operar → mitigado com automação total (provisionamento, template de migrations, pgbouncer)"],
    escolhido: true,
  },
];

export const FLUXO_TENANT = [
  { t: "JWT assinado chega à API", d: "O claim tenant_id está dentro do token assinado — não no body, não na query." },
  { t: "Middleware valida assinatura e vigência", d: "Assinatura inválida ou expirada → 401 imediato, sem tocar em banco." },
  { t: "TenantProvider consulta o ERP_MASTER", d: "Cache em Redis (5 min): tenant → host, database, status, módulos contratados." },
  { t: "Status da assinatura conferido", d: "Trial vencida ou suspensa → 403 com ProblemDetails; módulos fora do plano → 402." },
  { t: "Connection string montada do Vault", d: "Credenciais por banco buscadas em segredos — nunca em código ou config do app." },
  { t: "DbContext aponta para ERP_EMPRESA_XXXXXX", d: "Toda query nasce dentro do TenantContext. Qualquer tenant_id enviado pelo frontend é sumariamente ignorado." },
];

export const REGRAS_ISOLAMENTO = [
  "tenant_id vem do token — nunca do body, nunca da query string",
  "nome do banco validado por allowlist: ^ERP_EMPRESA_\\d{6}$",
  "toda query de domínio nasce dentro do TenantContext",
  "usuário da Empresa A recebe 404 — o registro da Empresa B fisicamente não existe no banco dele",
  "suite de intrusão cross-tenant roda em todo PR, antes do merge",
];

/* ---------------- 04 · Modelo de bancos ---------------- */

export type Coluna = { n: string; t: string; k?: "PK" | "FK" | "UQ" | "IDX" };
export type Tabela = { nome: string; desc: string; cols: Coluna[] };
export type GrupoTabelas = { grupo: string; tables: Tabela[] };

export const c = (n: string, t: string, k?: Coluna["k"]): Coluna => ({ n, t, k });

export const MASTER_TABELAS: Tabela[] = [
  { nome: "tenants", desc: "Cada empresa contratante da plataforma.", cols: [c("id", "uuid", "PK"), c("slug", "varchar(32)", "UQ"), c("razao_social", "varchar(160)"), c("status", "ativa | trial | suspensa | cancelada", "IDX"), c("criado_em", "timestamptz")] },
  { nome: "plans", desc: "Catálogo de planos do SaaS.", cols: [c("id", "uuid", "PK"), c("codigo", "BASICO | PROFISSIONAL | EMPRESARIAL | ENTERPRISE", "UQ"), c("limites_json", "jsonb — usuários, filiais, storage"), c("preco_mensal", "numeric(12,2)")] },
  { nome: "subscriptions", desc: "Assinatura vigente de cada tenant.", cols: [c("id", "uuid", "PK"), c("tenant_id", "uuid → tenants", "FK"), c("plan_id", "uuid → plans", "FK"), c("status", "ativa | trial | suspensa | cancelada", "IDX"), c("trial_ate", "date"), c("proxima_renovacao", "date")] },
  { nome: "modules", desc: "Módulos comercializáveis do ERP.", cols: [c("id", "uuid", "PK"), c("codigo", "VENDAS | ESTOQUE | FINANCEIRO | FISCAL | COMPRAS | BI", "UQ"), c("nome", "varchar(60)")] },
  { nome: "tenant_modules", desc: "Quais módulos cada empresa contratou.", cols: [c("tenant_id", "uuid → tenants", "PK"), c("module_id", "uuid → modules", "PK"), c("habilitado", "boolean")] },
  { nome: "tenant_databases", desc: "O mapa sagrado: tenant → banco físico.", cols: [c("id", "uuid", "PK"), c("tenant_id", "uuid → tenants", "UQ"), c("host", "varchar(120)"), c("database_name", "ERP_EMPRESA_XXXXXX", "UQ"), c("schema_version", "int", "IDX"), c("status", "provisionando | ativa | migrando")] },
  { nome: "platform_users", desc: "Administradores da plataforma (não são usuários de tenant).", cols: [c("id", "uuid", "PK"), c("email", "citext", "UQ"), c("senha_hash", "argon2id"), c("mfa_secret_enc", "bytea — cifrado no Vault"), c("status", "ativo | bloqueado")] },
  { nome: "platform_events", desc: "Log estruturado da plataforma.", cols: [c("id", "ulid", "PK"), c("tipo", "varchar(60)", "IDX"), c("tenant_id", "uuid → tenants"), c("payload_json", "jsonb"), c("criado_em", "timestamptz", "IDX")] },
  { nome: "billing_events", desc: "Cobranças, pagamentos e estornos das assinaturas.", cols: [c("id", "uuid", "PK"), c("subscription_id", "uuid → subscriptions", "FK"), c("tipo", "cobranca | pagamento | estorno"), c("valor", "numeric(12,2)"), c("gateway_ref", "varchar(80)"), c("criado_em", "timestamptz")] },
];

export const TENANT_GRUPOS: GrupoTabelas[] = [
  {
    grupo: "Estrutura & acesso",
    tables: [
      { nome: "filiais", desc: "Matriz e filiais — mesmo tenant, CNPJs distintos.", cols: [c("id", "uuid", "PK"), c("cnpj", "char(14)", "UQ"), c("razao_social", "varchar(160)"), c("regime_tributario", "simples | presumido | real"), c("matriz", "boolean"), c("status", "ativa | inativa", "IDX")] },
      { nome: "usuarios", desc: "Usuários da empresa (vínculo com pessoa opcional).", cols: [c("id", "uuid", "PK"), c("login", "varchar(40)", "UQ"), c("email", "citext", "UQ"), c("senha_hash", "argon2id"), c("filial_padrao_id", "uuid → filiais", "FK"), c("mfa_ativo", "boolean"), c("ultimo_acesso", "timestamptz")] },
      { nome: "perfis", desc: "Papéis RBAC da empresa.", cols: [c("id", "uuid", "PK"), c("nome", "varchar(60)", "UQ"), c("descricao", "varchar(200)")] },
      { nome: "permissoes", desc: "Permissões granulares por módulo.ação.", cols: [c("id", "uuid", "PK"), c("codigo", "ex.: vendas.cancelar", "UQ"), c("modulo", "varchar(30)", "IDX"), c("acao", "varchar(30)")] },
      { nome: "perfil_permissoes", desc: "N:N entre perfil e permissão.", cols: [c("perfil_id", "uuid → perfis", "PK"), c("permissao_id", "uuid → permissoes", "PK")] },
      { nome: "usuario_filiais", desc: "Quais filiais cada usuário acessa.", cols: [c("usuario_id", "uuid → usuarios", "PK"), c("filial_id", "uuid → filiais", "PK")] },
      { nome: "sessoes", desc: "Refresh tokens e dispositivos.", cols: [c("id", "uuid", "PK"), c("usuario_id", "uuid → usuarios", "FK"), c("device", "varchar(120)"), c("ip", "inet", "IDX"), c("refresh_hash", "varchar(90)"), c("expira_em", "timestamptz", "IDX"), c("revogada_em", "timestamptz")] },
      { nome: "auditoria", desc: "Trilha imutável — append-only, sem UPDATE/DELETE.", cols: [c("id", "ulid", "PK"), c("usuario_id", "uuid", "IDX"), c("usuario_nome", "varchar(100)"), c("ip", "inet"), c("device", "varchar(120)"), c("operacao", "criar | alterar | excluir | cancelar | emitir…", "IDX"), c("modulo", "varchar(30)", "IDX"), c("entidade", "varchar(40)", "IDX"), c("entidade_id", "varchar(40)"), c("campo", "varchar(60)"), c("valor_ant", "text"), c("valor_novo", "text"), c("correlacao_id", "uuid", "IDX"), c("criado_em", "timestamptz", "IDX")] },
    ],
  },
  {
    grupo: "Pessoas & papéis",
    tables: [
      { nome: "pessoas", desc: "Cadastro-base compartilhado entre filiais.", cols: [c("id", "uuid", "PK"), c("tipo", "fisica | juridica"), c("cpf_cnpj", "char(14)", "UQ"), c("nome_razao", "varchar(160)", "IDX"), c("fantasia", "varchar(160)"), c("nascimento_fundacao", "date"), c("status", "ativa | inativa", "IDX")] },
      { nome: "pessoa_enderecos", desc: "Múltiplos endereços por pessoa.", cols: [c("id", "uuid", "PK"), c("pessoa_id", "uuid → pessoas", "FK"), c("tipo", "cobranca | entrega | fiscal"), c("cep", "char(8)"), c("municipio", "varchar(80)"), c("uf", "char(2)"), c("principal", "boolean")] },
      { nome: "pessoa_contatos", desc: "Telefones, WhatsApp, e-mails.", cols: [c("id", "uuid", "PK"), c("pessoa_id", "uuid → pessoas", "FK"), c("tipo", "tel | cel | whatsapp | email"), c("valor", "varchar(120)", "IDX"), c("principal", "boolean")] },
      { nome: "clientes", desc: "Papel comercial — estende pessoa, estrutura própria.", cols: [c("id", "uuid", "PK"), c("pessoa_id", "uuid → pessoas", "UQ"), c("limite_credito", "numeric(14,2)"), c("condicao_pagto_id", "uuid", "FK"), c("tabela_preco_id", "uuid", "FK"), c("vendedor_id", "uuid → funcionarios", "FK"), c("contribuinte_icms", "boolean"), c("ind_presenca", "smallint")] },
      { nome: "fornecedores", desc: "Papel de supply — estende pessoa, estrutura própria.", cols: [c("id", "uuid", "PK"), c("pessoa_id", "uuid → pessoas", "UQ"), c("condicao_pagto_id", "uuid", "FK"), c("prazo_medio_dias", "int"), c("categoria", "varchar(40)", "IDX"), c("status", "ativo | inativo")] },
      { nome: "funcionarios", desc: "Módulo independente — pronto para folha/eSocial futuro.", cols: [c("id", "uuid", "PK"), c("pessoa_id", "uuid → pessoas", "UQ"), c("matricula", "varchar(20)", "UQ"), c("cargo", "varchar(60)"), c("departamento", "varchar(40)", "IDX"), c("admissao", "date"), c("salario", "numeric(12,2)"), c("centro_custo_id", "uuid", "FK"), c("comissao_pct", "numeric(5,2)"), c("situacao", "ativo | ferias | desligado", "IDX")] },
    ],
  },
  {
    grupo: "Catálogo & preços",
    tables: [
      { nome: "produtos", desc: "Cadastro completo, compartilhado entre filiais.", cols: [c("id", "uuid", "PK"), c("codigo_interno", "varchar(30)", "UQ"), c("gtin", "varchar(14)", "UQ"), c("descricao", "varchar(160)", "IDX"), c("desc_fiscal", "varchar(190)"), c("ncm", "char(8)", "IDX"), c("cest", "char(7)"), c("unidade_id", "uuid", "FK"), c("grupo_id", "uuid", "FK"), c("marca_id", "uuid", "FK"), c("estoque_min", "numeric(12,3)"), c("custo", "numeric(14,4)"), c("preco_venda", "numeric(14,2)"), c("flags", "venda/compra/estoque/composto/grade")] },
      { nome: "unidades", desc: "UN, KG, CX, M…", cols: [c("id", "uuid", "PK"), c("sigla", "varchar(6)", "UQ"), c("descricao", "varchar(40)")] },
      { nome: "marcas", desc: "Marcas dos produtos.", cols: [c("id", "uuid", "PK"), c("nome", "varchar(60)", "UQ")] },
      { nome: "grupos", desc: "Grupos e subgrupos em árvore.", cols: [c("id", "uuid", "PK"), c("nome", "varchar(60)"), c("grupo_pai_id", "uuid → grupos", "FK")] },
      { nome: "tabelas_preco", desc: "Múltiplas políticas de preço.", cols: [c("id", "uuid", "PK"), c("nome", "varchar(60)", "UQ"), c("fator_base", "numeric(6,3)"), c("moeda", "char(3)")] },
      { nome: "tabela_preco_itens", desc: "Preço por produto × tabela.", cols: [c("tabela_id", "uuid", "PK"), c("produto_id", "uuid", "PK"), c("preco", "numeric(14,2)")] },
      { nome: "produto_fornecedores", desc: "Fornecedor principal e alternativos.", cols: [c("produto_id", "uuid", "PK"), c("fornecedor_id", "uuid", "PK"), c("principal", "boolean"), c("ultimo_custo", "numeric(14,4)")] },
    ],
  },
  {
    grupo: "Estoque",
    tables: [
      { nome: "depositos", desc: "Por filial: próprio, terceiro, consignação.", cols: [c("id", "uuid", "PK"), c("filial_id", "uuid → filiais", "FK"), c("nome", "varchar(60)"), c("tipo", "proprio | terceiro | consignacao")] },
      { nome: "estoque_saldos", desc: "Saldo por produto × depósito — derivado das movimentações.", cols: [c("produto_id", "uuid", "PK"), c("deposito_id", "uuid", "PK"), c("atual", "numeric(14,3)"), c("comprometido", "numeric(14,3)"), c("disponivel", "gerado = atual − comprometido")] },
      { nome: "estoque_movimentacoes", desc: "Imutável: entrada, saída, transferência, ajuste, perda…", cols: [c("id", "ulid", "PK"), c("produto_id", "uuid", "IDX"), c("deposito_origem_id", "uuid", "FK"), c("deposito_destino_id", "uuid", "FK"), c("tipo", "entrada | saida | transferencia | ajuste | perda | devolucao | reserva", "IDX"), c("quantidade", "numeric(14,3)"), c("documento_tipo", "venda | compra | manual…"), c("documento_id", "uuid", "IDX"), c("usuario_id", "uuid", "IDX"), c("filial_id", "uuid", "IDX"), c("criado_em", "timestamptz", "IDX")] },
      { nome: "reservas", desc: "Estoque comprometido por pedido.", cols: [c("id", "uuid", "PK"), c("pedido_id", "uuid", "FK"), c("produto_id", "uuid", "FK"), c("quantidade", "numeric(14,3)"), c("status", "ativa | consumida | expirada", "IDX"), c("expira_em", "timestamptz")] },
    ],
  },
  {
    grupo: "Vendas",
    tables: [
      { nome: "pedidos", desc: "Orçamento, pedido e venda no mesmo ciclo de vida.", cols: [c("id", "uuid", "PK"), c("numero", "seq por filial × série", "UQ"), c("tipo", "orcamento | pedido | venda", "IDX"), c("filial_id", "uuid", "IDX"), c("cliente_id", "uuid", "IDX"), c("vendedor_id", "uuid", "IDX"), c("status", "aberto | reservado | faturado | cancelado", "IDX"), c("total", "numeric(14,2)")] },
      { nome: "pedido_itens", desc: "Itens com snapshot fiscal no momento da venda.", cols: [c("id", "uuid", "PK"), c("pedido_id", "uuid", "FK"), c("produto_id", "uuid", "FK"), c("quantidade", "numeric(14,3)"), c("preco_unit", "numeric(14,4)"), c("desconto", "numeric(14,2)"), c("cfop", "varchar(4)"), c("ncm", "char(8)")] },
      { nome: "pagamentos", desc: "Múltiplas formas por venda.", cols: [c("id", "uuid", "PK"), c("pedido_id", "uuid", "FK"), c("forma", "pix | cartao | dinheiro | boleto | transferencia | credito", "IDX"), c("valor", "numeric(14,2)"), c("parcelas", "int"), c("adquirente_ref", "varchar(80)")] },
    ],
  },
  {
    grupo: "Compras",
    tables: [
      { nome: "solicitacoes", desc: "Demanda interna de compra.", cols: [c("id", "uuid", "PK"), c("filial_id", "uuid", "IDX"), c("solicitante_id", "uuid", "FK"), c("status", "aberta | cotando | aprovada", "IDX")] },
      { nome: "cotacoes", desc: "Cotações com múltiplos fornecedores.", cols: [c("id", "uuid", "PK"), c("solicitacao_id", "uuid", "FK"), c("fornecedor_id", "uuid", "FK"), c("validade", "date"), c("total", "numeric(14,2)")] },
      { nome: "pedidos_compra", desc: "Pedido firmado com o fornecedor.", cols: [c("id", "uuid", "PK"), c("fornecedor_id", "uuid", "FK"), c("filial_id", "uuid", "IDX"), c("status", "emitido | recebido_parcial | recebido | cancelado", "IDX"), c("frete", "numeric(14,2)"), c("despesas_acessorias", "numeric(14,2)")] },
      { nome: "entradas", desc: "Recebimento físico + entrada fiscal.", cols: [c("id", "uuid", "PK"), c("pedido_compra_id", "uuid", "FK"), c("doc_fiscal_id", "uuid", "FK"), c("recebido_em", "timestamptz"), c("usuario_id", "uuid")] },
    ],
  },
  {
    grupo: "Financeiro",
    tables: [
      { nome: "centros_custo", desc: "Estrutura de análise gerencial.", cols: [c("id", "uuid", "PK"), c("nome", "varchar(60)"), c("departamento", "varchar(40)")] },
      { nome: "categorias_financeiras", desc: "Receitas e despesas classificáveis.", cols: [c("id", "uuid", "PK"), c("nome", "varchar(60)"), c("tipo", "receita | despesa", "IDX"), c("centro_custo_id", "uuid", "FK")] },
      { nome: "contas_receber", desc: "Títulos originados de vendas ou lançamentos.", cols: [c("id", "uuid", "PK"), c("cliente_id", "uuid", "IDX"), c("origem", "venda | manual | renegociacao", "IDX"), c("documento_ref", "uuid", "IDX"), c("total", "numeric(14,2)"), c("status", "aberto | parcial | quitado | baixado", "IDX")] },
      { nome: "cr_parcelas", desc: "Parcelas com baixa parcial, juros, multa e desconto.", cols: [c("id", "uuid", "PK"), c("conta_id", "uuid → contas_receber", "FK"), c("numero", "int"), c("vencimento", "date", "IDX"), c("valor", "numeric(14,2)"), c("valor_recebido", "numeric(14,2)"), c("juros", "numeric(14,2)"), c("desconto", "numeric(14,2)"), c("recebido_em", "timestamptz")] },
      { nome: "contas_pagar", desc: "Obrigações com fornecedores e despesas.", cols: [c("id", "uuid", "PK"), c("fornecedor_id", "uuid", "IDX"), c("origem", "compra | despesa | recorrencia", "IDX"), c("total", "numeric(14,2)"), c("status", "aberto | parcial | quitado", "IDX")] },
      { nome: "caixa_sessoes", desc: "Abertura/fechamento por operador e filial.", cols: [c("id", "uuid", "PK"), c("filial_id", "uuid", "IDX"), c("operador_id", "uuid", "FK"), c("abertura_em", "timestamptz"), c("valor_abertura", "numeric(14,2)"), c("fechamento_em", "timestamptz"), c("valor_conferido", "numeric(14,2)"), c("diferenca", "numeric(14,2)"), c("status", "aberto | fechado", "IDX")] },
      { nome: "caixa_movimentos", desc: "Vendas, sangrias e suprimentos da sessão.", cols: [c("id", "ulid", "PK"), c("sessao_id", "uuid", "FK"), c("tipo", "venda | sangria | suprimento", "IDX"), c("meio", "dinheiro | pix | cartao"), c("valor", "numeric(14,2)"), c("criado_em", "timestamptz", "IDX")] },
      { nome: "contas_bancarias", desc: "Contas por filial, prontas para integração.", cols: [c("id", "uuid", "PK"), c("filial_id", "uuid", "FK"), c("banco", "varchar(40)"), c("agencia", "varchar(10)"), c("conta", "varchar(20)"), c("pix_chave", "varchar(80)"), c("saldo_conciliado", "numeric(14,2)")] },
      { nome: "conciliacao_lancamentos", desc: "Extrato × títulos, match manual ou automático.", cols: [c("id", "uuid", "PK"), c("conta_id", "uuid", "FK"), c("data", "date", "IDX"), c("valor", "numeric(14,2)"), c("ref_banco", "varchar(80)"), c("titulo_match_id", "uuid", "IDX"), c("status", "pendente | conciliado", "IDX")] },
    ],
  },
  {
    grupo: "Fiscal",
    tables: [
      { nome: "regras_fiscais", desc: "Tributação versionada por operação × UF × NCM.", cols: [c("id", "uuid", "PK"), c("versao", "int", "IDX"), c("operacao", "varchar(40)", "IDX"), c("uf_origem", "char(2)"), c("uf_destino", "char(2)"), c("ncm", "char(8)", "IDX"), c("regime", "simples | presumido | real"), c("regra_json", "jsonb — ICMS, ST, IPI, PIS, COFINS, DIFAL, FCP"), c("vigencia_de", "date"), c("vigencia_ate", "date")] },
      { nome: "documentos_fiscais", desc: "NF-e, NFC-e, NFS-e, CT-e, MDF-e.", cols: [c("id", "ulid", "PK"), c("filial_id", "uuid", "IDX"), c("modelo", "55 | 65 | NFSe | 57 | 58", "IDX"), c("serie", "varchar(3)"), c("numero", "bigint", "IDX"), c("chave", "char(44)", "UQ"), c("natureza_operacao", "varchar(60)"), c("destinatario_id", "uuid", "IDX"), c("status", "autorizada | rejeitada | cancelada | contingencia", "IDX"), c("xml_path", "varchar(200)"), c("protocolo", "varchar(20)"), c("emitido_em", "timestamptz", "IDX")] },
      { nome: "doc_fiscal_itens", desc: "Tributos por item — o coração do cálculo fiscal.", cols: [c("id", "uuid", "PK"), c("documento_id", "uuid", "FK"), c("produto_id", "uuid", "FK"), c("cst_csosn", "varchar(4)"), c("icms_base", "numeric(14,2)"), c("icms_valor", "numeric(14,2)"), c("icms_st_valor", "numeric(14,2)"), c("ipi_valor", "numeric(14,2)"), c("pis_valor", "numeric(14,2)"), c("cofins_valor", "numeric(14,2)"), c("difal_valor", "numeric(14,2)"), c("fcp_valor", "numeric(14,2)")] },
      { nome: "eventos_fiscais", desc: "Cancelamento, CCe, inutilização, manifestação.", cols: [c("id", "uuid", "PK"), c("documento_id", "uuid", "FK"), c("tipo", "cancelamento | cce | inutilizacao | manifestacao", "IDX"), c("payload_json", "jsonb"), c("protocolo", "varchar(20)"), c("criado_em", "timestamptz")] },
      { nome: "certificados", desc: "Certificados A1 por filial — senha jamais em texto puro.", cols: [c("id", "uuid", "PK"), c("filial_id", "uuid", "FK"), c("tipo", "A1"), c("thumbprint", "varchar(60)", "UQ"), c("validade_ate", "date", "IDX"), c("arquivo_ref", "Vault/MinIO criptografado"), c("status", "valido | vencendo | vencido", "IDX")] },
    ],
  },
];

/* ---------------- 05/06 · Auth & RBAC ---------------- */

export const AUTH_PASSOS = [
  { t: "POST /api/v1/auth/login", d: "Login + senha com throttle: 5 tentativas por IP/15min e bloqueio progressivo contra brute force.", chips: ["throttle", "CSRF token", "captcha em reincidência"] },
  { t: "Verificação Argon2id", d: "Hash resistente a GPU/ASIC (m=64MB, t=3, p=4). Nunca se compara texto puro; nunca se revela se login ou senha falhou.", chips: ["argon2id", "resposta genérica"] },
  { t: "Desafio 2FA (TOTP)", d: "Se o usuário tem MFA ativo: código RFC 6238 de 6 dígitos. Códigos de recuperação são de uso único e guardados com hash.", chips: ["TOTP", "recuperação one-time"] },
  { t: "Access JWT emitido — 15 min", d: "Claims: sub, tenant_id, filial_id, perfis, jti. Assinatura RS256 com chaves rotacionáveis no Vault.", chips: ["RS256", "ttl 15min", "tenant_id assinado"] },
  { t: "Refresh token — 30 dias, com rotação", d: "Armazenado com hash na tabela sessoes. Cada uso gera um novo e invalida o anterior; reuso de token antigo revoga a sessão inteira (detecção de roubo).", chips: ["rotação", "detecção de reuso", "revogação"] },
  { t: "Todo request: TenantContext + RBAC", d: "O middleware reconstrói o contexto do tenant pelo token e revalida a permissão no backend. O frontend apenas esconde o que não pode ver — quem decide é o servidor.", chips: ["autorização server-side", "403 ProblemDetails"] },
];

export const MODULOS_RBAC = ["Clientes", "Produtos", "Estoque", "Vendas", "Financeiro", "Fiscal"];
export const ACOES_RBAC = ["ver", "criar", "editar", "excluir", "exportar", "cancelar"];

export const PERFIS_RBAC: { nome: string; desc: string; perms: string[] | "TODAS" }[] = [
  {
    nome: "Administrador",
    desc: "Acesso total ao tenant, incluindo exclusões e administração de usuários.",
    perms: "TODAS",
  },
  {
    nome: "Gerente",
    desc: "Gestão completa, sem poderes destrutivos sobre trilhas fiscais e financeiras.",
    perms: [
      "Clientes.ver", "Clientes.criar", "Clientes.editar", "Clientes.excluir", "Clientes.exportar", "Clientes.cancelar",
      "Produtos.ver", "Produtos.criar", "Produtos.editar", "Produtos.excluir", "Produtos.exportar",
      "Estoque.ver", "Estoque.criar", "Estoque.editar", "Estoque.exportar",
      "Vendas.ver", "Vendas.criar", "Vendas.editar", "Vendas.exportar", "Vendas.cancelar",
      "Financeiro.ver", "Financeiro.criar", "Financeiro.editar", "Financeiro.exportar",
      "Fiscal.ver", "Fiscal.criar", "Fiscal.exportar", "Fiscal.cancelar",
    ],
  },
  {
    nome: "Vendedor",
    desc: "Opera carteira de clientes e pedidos; enxerga estoque e preço, não altera nada fora do pedido.",
    perms: [
      "Clientes.ver", "Clientes.criar", "Clientes.editar",
      "Produtos.ver", "Produtos.exportar",
      "Estoque.ver",
      "Vendas.ver", "Vendas.criar", "Vendas.editar", "Vendas.exportar",
      "Financeiro.ver",
    ],
  },
  {
    nome: "Operador de caixa",
    desc: "PDV e rotinas de caixa: vendas à vista, sangria, suprimento e fechamento.",
    perms: [
      "Clientes.ver", "Clientes.criar",
      "Produtos.ver",
      "Estoque.ver",
      "Vendas.ver", "Vendas.criar",
      "Financeiro.ver", "Financeiro.criar", "Financeiro.editar",
      "Fiscal.ver",
    ],
  },
];

export const ESCOPOS_RBAC = [
  { t: "Por empresa", d: "Todo RBAC vive dentro do banco do tenant — perfis de uma empresa não existem para outra." },
  { t: "Por filial", d: "usuario_filiais restringe vendas, caixa, estoque e documentos às filiais liberadas para o usuário." },
  { t: "Por módulo", d: "Módulo não contratado no plano nem aparece: a verificação de tenant_modules precede qualquer check de permissão." },
  { t: "Por operação", d: "Permissões especiais além do CRUD: vendas.desconto_limite, vendas.reabrir, financeiro.estornar, fiscal.inutilizar." },
];

/* ---------------- 07 · Auditoria & logs ---------------- */

export const AUDIT_EXEMPLO = {
  usuario: "Anderson Ribeiro",
  acao: "alterou o preço de venda",
  entidade: "Produto · Cafeteira Inox 20L (cod. 004821)",
  antes: "R$ 100,00",
  depois: "R$ 120,00",
  quando: "01/09/2026 · 14:32:07",
  ip: "200.155.34.18",
  device: "Chrome 128 · Windows 11",
  correlacao: "8f3c2ab1-9e44-4d1c-b7aa-02de91c44a17",
};

export const OPERACOES_CRITICAS = [
  "alteração de preço ou custo de produto",
  "cancelamento de venda ou de documento fiscal",
  "baixa, estorno ou renegociação de título",
  "ajuste manual de estoque",
  "mudança de permissão ou de perfil",
  "criação/bloqueio de usuário e reset de 2FA",
  "importação de certificado digital A1",
  "inutilização de numeração fiscal",
];

export const LOGS_PILARES = [
  { t: "Estruturado sempre", d: "Serilog com campos tipados (tenant_id, usuario_id, modulo, duracao_ms). Log de texto livre só em debug." },
  { t: "Correlation_id de ponta a ponta", d: "O mesmo id viaja do request HTTP → fila → worker → banco. Um incidente se reconstrói inteiro no Grafana." },
  { t: "PII mascarada", d: "CPF/CNPJ, cartão e senha nunca aparecem em log — mascaramento automático no sink." },
  { t: "Auditoria ≠ log", d: "Auditoria é dado imutável no banco do tenant; log é telemetria. Não se apaga trilha de auditoria nem com expiração de log." },
  { t: "Erro: técnico dentro, amigável fora", d: "Stack trace completa no Loki; para o cliente, ProblemDetails (RFC 7807) com código e mensagem segura." },
];

/* ---------------- 08 · Estrutura & operação ---------------- */

export type NoArvore = { nome: string; nota?: string; filhos?: NoArvore[] };

export const ARVORE: NoArvore = {
  nome: "matriz-erp/",
  nota: "monorepo",
  filhos: [
    {
      nome: "apps/",
      filhos: [
        { nome: "web/", nota: "Next.js — ERP autenticado + portal do assinante" },
        { nome: "api/", nota: "ASP.NET Core — host único do monolito modular" },
      ],
    },
    {
      nome: "workers/",
      filhos: [
        { nome: "fiscal.worker/", nota: "NF-e · NFC-e · NFS-e · eventos · contingência" },
        { nome: "notificacoes.worker/", nota: "e-mail · WhatsApp · SMS · push" },
        { nome: "relatorios.worker/", nota: "PDF · Excel · CSV em fila" },
        { nome: "backup.worker/", nota: "backup por tenant + verificação de integridade" },
        { nome: "integracoes.worker/", nota: "bancos · marketplaces · XML de entrada" },
      ],
    },
    {
      nome: "modules/",
      nota: "um pacote por bounded context",
      filhos: [
        { nome: "identity/ · tenants/ · cadastros/ · produtos/" },
        { nome: "estoque/ · vendas/ · compras/ · financeiro/" },
        { nome: "fiscal/ · relatorios/ · notificacoes/ · integracoes/" },
      ],
    },
    {
      nome: "core/",
      nota: "compartilhado — sem regras de domínio",
      filhos: [
        { nome: "kernel/", nota: "result types, ids (ULID), clock, domain events" },
        { nome: "tenant/", nota: "TenantProvider, resolução, allowlist de banco" },
        { nome: "security/", nota: "JWT, RBAC, Argon2id, 2FA" },
        { nome: "data/", nota: "EF Core, interceptors de auditoria, migrations" },
        { nome: "messaging/ · web/ · telemetry/" },
      ],
    },
    {
      nome: "db/",
      filhos: [
        { nome: "master/migrations/", nota: "schema do ERP_MASTER" },
        { nome: "tenant-template/migrations/", nota: "aplicado em cada ERP_EMPRESA_*" },
        { nome: "tenant-template/seeds/", nota: "CFOP, NCM, unidades, perfil padrão" },
      ],
    },
    { nome: "infra/", nota: "docker-compose · terraform · manifests" },
    { nome: "docs/", nota: "ADRs · OpenAPI · runbooks · onboarding" },
    { nome: "tests/", nota: "unit · integration · isolation (cross-tenant) · e2e" },
  ],
};

export const MIGRATIONS_LANES = [
  {
    titulo: "Lane MASTER",
    cor: "jade",
    passos: [
      "Migrations EF Core em db/master, revisadas em PR",
      "Aplicadas automaticamente no deploy da plataforma",
      "Alterações registradas em platform_events",
    ],
  },
  {
    titulo: "Lane TENANT (template)",
    cor: "brass",
    passos: [
      "Template versionado: 0001_init, 0002_…, 000N_…",
      "Provisionamento aplica até a versão mais recente",
      "Upgrade roda por tenant, controlado por tenant_databases.schema_version",
      "Rollout escalonado: canário em 1 tenant → 10% → 100%",
      "Rollback individual por tenant, sem afetar os demais",
    ],
  },
];

export const PROVISIONING = [
  "Assinatura aprovada no portal (webhook de billing)",
  "Backup.Worker cria o banco ERP_EMPRESA_XXXXXX na instância do plano",
  "Template de migrations aplicado + seeds fiscais (CFOP, NCM, unidades)",
  "Registro em tenant_databases com credenciais geradas no Vault",
  "Filial matriz + usuário administrador criados no banco novo",
  "E-mail com credenciais provisórias e trilha em platform_events",
];

export const BACKUP_PLANOS = [
  { plano: "BÁSICO", retencao: "7 dias", rpo: "24h", rto: "8h" },
  { plano: "PROFISSIONAL", retencao: "15 dias", rpo: "12h", rto: "4h" },
  { plano: "EMPRESARIAL", retencao: "30 dias", rpo: "1h", rto: "2h" },
  { plano: "ENTERPRISE", retencao: "sob contrato", rpo: "15 min (PITR)", rto: "30 min" },
];

export const BACKUP_NOTAS = [
  "pg_basebackup completo semanal + arquivamento contínuo de WAL",
  "Offload criptografado para MinIO/S3 em região distinta",
  "Restore de verificação automatizado toda semana — backup que não restaura não existe",
  "Simulação de desastre (DR drill) trimestral com métricas de RPO/RTO",
  "ERP_MASTER com RPO de 15 min — é o cérebro da plataforma",
  "Restauração é individual por tenant: um incidente derruba um banco, nunca os vizinhos",
];

/* ---------------- 09 · Testes & riscos ---------------- */

export const PIRAMIDE = [
  { nome: "E2E — Playwright", pct: 10, desc: "Fluxos críticos: venda completa com saga, emissão fiscal simulada, conciliação — e a suite de isolamento cross-tenant." },
  { nome: "Integração — Testcontainers", pct: 20, desc: "API + PostgreSQL real em container: migrations, transações de estoque/financeiro, concorrência de caixa." },
  { nome: "Unitários — xUnit", pct: 70, desc: "Regras de domínio puras: cálculo de tributos, juros/multa/desconto, margem, reserva de estoque, RBAC." },
];

export const TESTES_ISOLAMENTO = [
  { t: "Acesso direto ao banco de outro tenant", d: "Credenciais da Empresa A contra ERP_EMPRESA_000002.", r: "FALHA — credenciais por banco + allowlist de conexão" },
  { t: "Claim tenant adulterado no JWT", d: "Token da Empresa A com tenant_id reescrito para 000002.", r: "FALHA — assinatura RS256 inválida → 401" },
  { t: "Consulta a id de outro tenant", d: "GET /api/v1/clientes/{id} usando um id que existe na Empresa B.", r: "FALHA — 404: o registro fisicamente não existe no banco da Empresa A" },
  { t: "Corpo de request com tenant_id alheio", d: "POST incluindo \"tenant_id\": \"000002\" no payload.", r: "FALHA — campo ignorado; o contexto vem exclusivamente do token" },
  { t: "Query string manipulada", d: "GET /api/v1/produtos?tenant_id=000002.", r: "FALHA — parâmetro inexistente na rota; sumariamente descartado" },
  { t: "Token de outro contexto reaproveitado", d: "Token válido obtido em sessão/audience diferente.", r: "FALHA — validação de issuer/audience + heurísticas de dispositivo + rotação de refresh" },
];

export type Risco = { id: string; t: string; d: string; prob: number; imp: number; mit: string };

export const RISCOS: Risco[] = [
  { id: "R1", t: "Explosão de conexões", d: "Centenas de tenants × pools de conexão podem saturar instâncias.", prob: 4, imp: 4, mit: "pgbouncer em modo transaction, pools dinâmicos por grupo de instâncias e ociosidade agressiva." },
  { id: "R2", t: "Complexidade fiscal por UF", d: "ICMS-ST, DIFAL e NFS-e municipal mudam com frequência e por localidade.", prob: 4, imp: 5, mit: "Serviço fiscal desacoplado, regras versionadas em banco, homologação por UF antes de liberar emissão." },
  { id: "R3", t: "Migrations longas × N bancos", d: "Alterar schema em centenas de bancos de uma vez é arriscado.", prob: 3, imp: 3, mit: "Template versionado, rollout canário por tenant e rollback individual controlado por schema_version." },
  { id: "R4", t: "Vizinho barulhento", d: "Um tenant com carga anormal degrada outros da mesma instância.", prob: 2, imp: 3, mit: "Agrupamento por porte de plano, limites de conexão por banco e rate limit por tenant na borda." },
  { id: "R5", t: "Custo de banco dedicado em planos pequenos", d: "DB por tenant pode ficar caro em massa de microempresas.", prob: 3, imp: 3, mit: "Instâncias compartilhadas com bancos físicos separados (o isolamento se mantém), scale vertical por tier." },
  { id: "R6", t: "Gestão de certificados A1", d: "Certificado vencido paralisa a emissão fiscal do cliente.", prob: 2, imp: 4, mit: "Armazenamento no Vault, alerta com 30/15/7 dias, fluxo de renovação guiado e contingência documentada." },
  { id: "R7", t: "Curva .NET + PostgreSQL da equipe", d: "Time com forte base SQL Server migrando de stack.", prob: 2, imp: 2, mit: "EF Core abstrai boa parte do dialeto; treinamento dirigido, runbooks e pair programming nas primeiras migrations." },
];

/* ---------------- 10 · Roadmap & ADRs ---------------- */

export type Fase = { num: number; titulo: string; escopo: string[]; dur: string; atual?: boolean };

export const FASES: Fase[] = [
  { num: 1, titulo: "Fundação", dur: "~8 semanas", atual: true, escopo: [
    "ERP_MASTER + provisionamento automático de banco por empresa",
    "Autenticação (JWT + refresh com rotação + 2FA) e sessões",
    "Usuários, perfis e RBAC granular com escopo por filial",
    "Empresa e filiais (matriz/filiais, CNPJs, regimes)",
    "Auditoria imutável + logs estruturados com correlation_id",
    "Layout-base do ERP e dashboard inicial",
  ]},
  { num: 2, titulo: "Cadastros", dur: "~8 semanas", escopo: [
    "Pessoas com endereços e contatos múltiplos",
    "Clientes, fornecedores e funcionários como módulos independentes",
    "Produtos completos (NCM, CEST, grades, lotes, validade)",
    "Unidades, marcas, grupos e múltiplas tabelas de preço",
    "Importação CSV/Excel de clientes, fornecedores e produtos",
  ]},
  { num: 3, titulo: "Operação", dur: "~10 semanas", escopo: [
    "Estoque por filial/depósito: entradas, saídas, transferências, ajustes, inventário",
    "Movimentações imutáveis — saldo sempre derivado e rastreável",
    "Vendas: orçamento → pedido → venda com saga transacional",
    "Múltiplas formas de pagamento, comissão, devoluções",
    "Compras: solicitação → cotação → pedido → recebimento → entrada",
  ]},
  { num: 4, titulo: "Financeiro", dur: "~8 semanas", escopo: [
    "Contas a receber/pagar com parcelas, juros, multa e baixa parcial",
    "Caixa por operador: abertura, sangria, suprimento, conferência",
    "Contas bancárias por filial e conciliação de extratos",
    "Centros de custo, categorias e rateios por filial/período",
  ]},
  { num: 5, titulo: "Fiscal", dur: "~12 semanas", escopo: [
    "Regras fiscais versionadas por operação × UF × NCM",
    "Fiscal.Worker: NF-e (55), NFC-e (65) e NFS-e com adapters",
    "Eventos: cancelamento, CCe, inutilização, manifestação",
    "Armazenamento de XML/DANFE e certificado A1 no Vault",
    "Contingência e reprocessamento de rejeições",
  ]},
  { num: 6, titulo: "Inteligência & integrações", dur: "contínuo", escopo: [
    "Relatórios configuráveis (PDF/Excel/CSV) e camada de BI",
    "Dashboards por perfil com indicadores do negócio",
    "Notificações multicanal: interna, e-mail, WhatsApp, SMS, push",
    "Integrações bancárias e PIX, gateways, marketplaces",
    "API pública v1 consolidada para ecossistema",
  ]},
];

export type Adr = { id: string; titulo: string; tags: string[]; problema: string; alternativas: { n: string; p: string; c: string }[]; decisao: string; impacto: string };

export const ADRS: Adr[] = [
  {
    id: "ADR-001", titulo: "Database-per-tenant como estratégia de isolamento", tags: ["tenancy", "segurança"],
    problema: "O requisito nº 2 exige que os dados de uma empresa jamais se misturem com os de outra — e que o isolamento seja arquitetural, não um filtro de código.",
    alternativas: [
      { n: "Banco compartilhado + tenant_id", p: "operação simples", c: "isolamento lógico: um único bug de query vaza dados entre empresas; restauração individual impossível" },
      { n: "Schema por tenant", p: "separação intermediária", c: "mesma instância = mesmo ponto de falha, carga compartilhada e superuser comum" },
      { n: "Database por tenant", p: "isolamento físico real", c: "mais bancos para operar — resolvido com provisionamento e migrations automáticas" },
    ],
    decisao: "Um banco físico por empresa (ERP_EMPRESA_XXXXXX), mapeado no ERP_MASTER e resolvido exclusivamente pelo token autenticado.",
    impacto: "Backup/restore e escala individuais por cliente; a suite de testes de intrusão vira garantia executável de que o vazamento é fisicamente improvável, não apenas logicamente evitado.",
  },
  {
    id: "ADR-002", titulo: "Monolito modular antes de microserviços", tags: ["arquitetura"],
    problema: "Microserviços desde o dia 1 multiplicariam complexidade operacional (deploy, observabilidade, transações distribuídas) antes de haver escala que justifique.",
    alternativas: [
      { n: "Microserviços por domínio", p: "escala independente", c: "sagas distribuídas, latência de rede, custo de plataforma — inviável para o time inicial" },
      { n: "Monólito tradicional", p: "simples", c: "acoplamento cresce até virar big ball of mud" },
      { n: "Monolito modular", p: "um deploy, módulos com fronteiras nítidas", c: "exige disciplina de dependências entre módulos" },
    ],
    decisao: "Monolito modular em .NET 10: um pacote por bounded context, dependências apenas via contratos internos. Workers já nascem separados (fiscal, notificações, relatórios).",
    impacto: "Extração futura de qualquer módulo para serviço independente vira uma mudança de empacotamento, não uma reescrita — porque as fronteiras já existem.",
  },
  {
    id: "ADR-003", titulo: "PostgreSQL 16 em vez de SQL Server", tags: ["dados", "custo"],
    problema: "SaaS com centenas de bancos de tenant precisa de custo de licenciamento previsível, replicação flexível e operação Linux nativa — sem abrir mão de integridade transacional.",
    alternativas: [
      { n: "SQL Server", p: "domínio atual da equipe", c: "licenciamento por core pesado em escala SaaS; operação Windows acrescida" },
      { n: "PostgreSQL", p: "licença livre, WAL/PITR maduros, particionamento,生态 rica", c: "curva de aprendizado do time" },
    ],
    decisao: "PostgreSQL 16 para master e tenants, com EF Core abstraindo o dialeto e migrations versionadas — mantendo a porta aberta para o SQL Server se o contexto de negócio mudar.",
    impacto: "Custo marginal por novo tenant próximo de zero; risco R7 (curva da equipe) tratado com treinamento — e a abstração EF Core impede lock-in do dialeto no código de domínio.",
  },
  {
    id: "ADR-004", titulo: "Fiscal como serviço desacoplado em worker", tags: ["fiscal", "filas"],
    problema: "Autorizadores SEFAZ têm instabilidade própria; regras mudam por UF e por data. Acoplar isso ao request de venda tornaria o ERP refém da SEFAZ.",
    alternativas: [
      { n: "Emissão síncrona na venda", p: "simples", c: "timeout da SEFAZ trava o PDV inteiro" },
      { n: "Terceirizar 100% a um provedor fiscal", p: "rápido", c: "dependência total e custo por documento" },
      { n: "Fiscal.Worker próprio + adapters", p: "controle e resiliência", c: "exige investimento inicial maior" },
    ],
    decisao: "Worker dedicado consumindo filas, com um adapter por UF/município e regras versionadas em tabelas — nunca em código.",
    impacto: "Venda não bloqueia na SEFAZ; rejeições viram reprocessamento automático; entrar em uma nova UF é implementar um adapter, não mexer no ERP.",
  },
  {
    id: "ADR-005", titulo: "JWT curto + refresh com rotação + TOTP", tags: ["segurança"],
    problema: "Sessões longas em cookie são simples, mas fracas contra roubo; tokens longos ampliam o dano de um vazamento. Um ERP com dados fiscais e financeiros precisa de mais.",
    alternativas: [
      { n: "Sessão server-side em cookie", p: "revogação trivial", c: "acoplamento a estado, pior para API pública futura" },
      { n: "JWT longo sem rotação", p: "stateless", c: "janela de dano de semanas se vazar" },
      { n: "JWT 15 min + refresh rotacionado + TOTP", p: "stateless com revogação prática", c: "complexidade moderada de implementação" },
    ],
    decisao: "Access token de 15 minutos com tenant_id assinado, refresh de 30 dias com rotação e detecção de reuso, TOTP opcional e senhas Argon2id.",
    impacto: "Roubo de token tem janela curta e deixa rastro (reuso detectado); o claim tenant_id assinado é a base técnica que torna o ADR-001 aplicável em cada request.",
  },
  {
    id: "ADR-006", titulo: "RBAC granular com escopos de filial e operação", tags: ["segurança", "usabilidade"],
    problema: "Permissões binárias por módulo (tem/não tem) não atendem realidade de filiais: o gerente da Filial 01 não pode ver o caixa da Filial 02.",
    alternativas: [
      { n: "ACL por recurso", p: "flexível", c: "gestão ingovernável com milhares de registros" },
      { n: "RBAC simples por módulo", p: "simples", c: "não expressa filial nem operações especiais" },
      { n: "RBAC módulo.acao + escopos", p: "granular e administrável", c: "matriz de perfis exige governança" },
    ],
    decisao: "Permissões no formato modulo.acao (vendas.cancelar, financeiro.estornar) vinculadas a perfis, com escopos por filial (usuario_filiais) e verificação de módulo contratado antes de tudo.",
    impacto: "Perfis ficam legíveis para o cliente final (Administrador, Gerente, Vendedor, Operador) e a matriz de permissões vira tela de configuração — não código.",
  },
];

/* ---------------- 11 · Aprovação ---------------- */

export const ENTREGAS: { n: number; t: string; ancora: string }[] = [
  { n: 1, t: "Arquitetura geral", ancora: "arquitetura" },
  { n: 2, t: "Stack tecnológica recomendada", ancora: "stack" },
  { n: 3, t: "Diagrama lógico dos componentes", ancora: "arquitetura" },
  { n: 4, t: "Estratégia de multi-tenancy", ancora: "tenancy" },
  { n: 5, t: "Estratégia de banco por empresa", ancora: "tenancy" },
  { n: 6, t: "Banco administrativo / master", ancora: "bancos" },
  { n: 7, t: "Estrutura inicial do banco da empresa", ancora: "bancos" },
  { n: 8, t: "Modelo de autenticação", ancora: "auth" },
  { n: 9, t: "Modelo de autorização (RBAC)", ancora: "rbac" },
  { n: 10, t: "Estrutura de diretórios", ancora: "operacao" },
  { n: 11, t: "Estratégia de migrations", ancora: "operacao" },
  { n: 12, t: "Estratégia de backup", ancora: "operacao" },
  { n: 13, t: "Estratégia de logs", ancora: "auditoria" },
  { n: 14, t: "Estratégia de auditoria", ancora: "auditoria" },
  { n: 15, t: "Estratégia de testes", ancora: "qualidade" },
  { n: 16, t: "Roadmap de desenvolvimento", ancora: "roadmap" },
  { n: 17, t: "Riscos técnicos", ancora: "qualidade" },
  { n: 18, t: "Decisões arquiteturais (ADRs)", ancora: "roadmap" },
];

export const QUESTOES_ABERTAS = [
  { t: "PostgreSQL 16 × SQL Server", d: "Confirmar a decisão do ADR-003. Impacta licenciamento, operação e o plano de treinamento da equipe (risco R7)." },
  { t: "Planos e limites por tier", d: "Definir usuários, filiais, armazenamento e volume de documentos de BÁSICO a ENTERPRISE — alimenta tenant_modules e o billing." },
  { t: "Escopo fiscal inicial (fase 5)", d: "Quais UFs e municípios entram na primeira onda: NF-e (55) + NFC-e (65) nacionais e qual padrão de NFS-e adotar primeiro." },
  { t: "Integrações bancárias prioritárias", d: "Começar com importação OFX manual + PIX estático e quais bancos via API entram na fase 6." },
];
