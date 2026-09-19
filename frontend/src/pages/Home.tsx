import { FormEvent, useEffect, useMemo, useState } from "react";
import { toast } from "sonner";
import {
  ArrowDownRight, ArrowUpRight, BarChart3, Bell, Boxes, Building2, CalendarDays, Check, ChevronDown, CircleHelp, CreditCard, Download, FileText, LayoutDashboard, Loader2, LogOut, Menu, Moon, Package, Plus, Search, Settings, ShoppingCart, Sparkles, Store, Sun, Truck, Users, Wallet, X,
} from "lucide-react";
import { api, hasLiveApi, type BranchRecord, type ClientRecord, type ProductRecord, type SessionUser } from "@/lib/api";

type HomeProps = { user: SessionUser; onLogout: () => void; darkMode: boolean; onToggleTheme: () => void };
type NavItem = { label: string; icon: typeof LayoutDashboard; section?: string; live?: boolean };

const navItems: NavItem[] = [
  { label: "Visão geral", icon: LayoutDashboard, live: true },
  { label: "Vendas", icon: ShoppingCart, section: "Operação" },
  { label: "Compras", icon: Truck },
  { label: "Estoque", icon: Boxes },
  { label: "Produtos", icon: Package },
  { label: "Clientes", icon: Users },
  { label: "Financeiro", icon: Wallet, section: "Gestão" },
  { label: "Relatórios", icon: BarChart3 },
  { label: "Empresas", icon: Building2 },
];

const activities = [
  { title: "Pedido #2084 aprovado", detail: "Casa Nativa • há 8 min", value: "+ R$ 1.280,00", tone: "green" },
  { title: "Estoque reabastecido", detail: "Café Origens • há 34 min", value: "128 itens", tone: "amber" },
  { title: "Novo cliente cadastrado", detail: "Marina Costa • há 1 h", value: "Cliente", tone: "blue" },
  { title: "Pagamento recebido", detail: "Fatura #1008 • há 2 h", value: "+ R$ 4.850,00", tone: "green" },
];

const bars = [35, 48, 42, 66, 52, 72, 61, 84, 68, 91, 76, 96];

function downloadCsv(filename: string, headers: string[], rows: Array<Array<string | number | null>>) {
  const escapeCell = (value: string | number | null) => `"${String(value ?? "").replace(/"/g, '""')}"`;
  const csv = [headers, ...rows].map((row) => row.map(escapeCell).join(",")).join("\n");
  const blob = new Blob(["\uFEFF" + csv], { type: "text/csv;charset=utf-8;" });
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = filename;
  document.body.appendChild(link);
  link.click();
  link.remove();
  URL.revokeObjectURL(url);
}

export default function Home({ user, onLogout, darkMode, onToggleTheme }: HomeProps) {
  const [active, setActive] = useState("Visão geral");
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [period, setPeriod] = useState("Este mês");
  const [searchOpen, setSearchOpen] = useState(false);
  const firstName = user.name.split(" ")[0] || "Admin";
  const activeMeta = useMemo(() => navItems.find((item) => item.label === active), [active]);
  const ActiveIcon = activeMeta?.icon ?? LayoutDashboard;

  function selectNav(label: string, live?: boolean) {
    setActive(label);
    setSidebarOpen(false);
    if (!live) toast.info(`${label} está pronto para conectar aos endpoints da API.`);
  }

  return (
    <div className="app-shell">
      {sidebarOpen && <button className="mobile-scrim" aria-label="Fechar menu" onClick={() => setSidebarOpen(false)} />}
      <aside className={`sidebar ${sidebarOpen ? "sidebar--open" : ""}`}>
        <div className="sidebar-top">
          <div className="app-logo"><div className="brand-mark brand-mark--sidebar">E</div><div><strong>ERP<span className="brand-dot">.</span>flow</strong><small>business OS</small></div></div>
          <button className="mobile-close" aria-label="Fechar menu" onClick={() => setSidebarOpen(false)}><X size={18} /></button>
        </div>
        <div className="workspace-switcher"><div className="workspace-icon"><Store size={16} /></div><div><strong>Casa Nativa</strong><span>Plano profissional</span></div><ChevronDown size={15} /></div>
        <nav className="main-nav" aria-label="Navegação principal">
          {navItems.map((item, index) => <div key={item.label}>{item.section && <p className="nav-section">{item.section}</p>}<button className={`nav-item ${active === item.label ? "nav-item--active" : ""}`} onClick={() => selectNav(item.label, item.live)}><item.icon size={18} /><span>{item.label}</span>{item.label === "Relatórios" && <span className="nav-badge">3</span>}</button></div>)}
        </nav>
        <div className="sidebar-bottom"><button className="nav-item" onClick={() => toast.info("Central de ajuda em preparação.")}><CircleHelp size={18} /><span>Ajuda e suporte</span></button><button className="nav-item" onClick={() => toast.info("Configurações em preparação.")}><Settings size={18} /><span>Configurações</span></button><div className="sidebar-divider" /><div className="sidebar-user"><div className="avatar avatar--small">{user.initials}</div><div><strong>{user.name}</strong><span>{user.role}</span></div><button className="icon-button icon-button--light" aria-label="Sair" onClick={onLogout}><LogOut size={16} /></button></div></div>
      </aside>

      <main className="main-content">
        <header className="topbar"><div className="topbar-left"><button className="mobile-menu" aria-label="Abrir menu" onClick={() => setSidebarOpen(true)}><Menu size={21} /></button><div className="breadcrumb"><span>Workspace</span><span className="breadcrumb-slash">/</span><strong>{active}</strong></div></div><div className="topbar-actions">{searchOpen ? <div className="top-search"><Search size={17} /><input autoFocus placeholder="Buscar no ERP…" onBlur={() => setSearchOpen(false)} /></div> : <button className="topbar-icon" aria-label="Buscar" onClick={() => setSearchOpen(true)}><Search size={19} /></button>}<button className="topbar-icon" aria-label={darkMode ? "Usar modo claro" : "Usar modo escuro"} onClick={onToggleTheme}>{darkMode ? <Sun size={18} /> : <Moon size={18} />}</button><button className="topbar-icon notification-button" aria-label="Notificações" onClick={() => toast.info("Você não tem novas notificações.")}><Bell size={19} /><i /></button><div className="topbar-divider" /><button className="user-menu" onClick={() => toast.info("Perfil em preparação.")}><div className="avatar">{user.initials}</div><div className="user-menu-copy"><strong>{user.name}</strong><span>Administrador</span></div><ChevronDown size={15} /></button></div></header>

        <div className="page-content">
          <div className="page-heading"><div><p className="eyebrow">quarta-feira, 10 de setembro de 2026</p><h1>Bom dia, {firstName} <span className="wave">✦</span></h1><p className="page-subtitle">Aqui está o que está acontecendo no seu negócio hoje.</p></div><div className="heading-actions"><button className="secondary-button"><CalendarDays size={16} />{period}<ChevronDown size={14} /></button><button className="primary-button" onClick={() => toast.success("Novo registro iniciado.")}><Plus size={17} />Novo registro</button></div></div>
          {active === "Clientes" ? <Customers /> : active === "Produtos" ? <Products /> : active === "Empresas" ? <Branches /> : active !== "Visão geral" && <div className="module-banner"><div className="module-banner-icon"><ActiveIcon size={21} /></div><div><strong>{active}</strong><p>Essa área está pronta para receber os dados da sua API.</p></div><button className="secondary-button" onClick={() => setActive("Visão geral")}>Voltar ao resumo</button></div>}
          {active === "Visão geral" && <>
            <div className="metric-grid"><MetricCard icon={Wallet} label="Receita total" value="R$ 84.290" change="18,6%" caption="vs. mês anterior" tone="navy" /><MetricCard icon={ShoppingCart} label="Pedidos" value="1.284" change="12,4%" caption="vs. mês anterior" tone="green" /><MetricCard icon={Users} label="Clientes ativos" value="2.847" change="8,2%" caption="vs. mês anterior" tone="blue" /><MetricCard icon={Package} label="Itens em estoque" value="8.492" change="3,1%" caption="vs. mês anterior" tone="amber" negative /></div>
            <div className="dashboard-grid"><section className="panel revenue-panel"><div className="panel-heading"><div><p className="eyebrow">performance</p><h2>Receita ao longo do tempo</h2></div><div className="legend"><span><i className="legend-dot legend-dot--primary" />Receita</span><span><i className="legend-dot legend-dot--muted" />Meta</span></div></div><div className="chart-wrap"><div className="chart-y-axis"><span>R$ 10k</span><span>R$ 7,5k</span><span>R$ 5k</span><span>R$ 2,5k</span><span>R$ 0</span></div><div className="chart-area"><div className="chart-lines"><i /><i /><i /><i /><i /></div><svg viewBox="0 0 760 250" preserveAspectRatio="none" className="line-chart" role="img" aria-label="Gráfico de receita crescente"><defs><linearGradient id="chartFill" x1="0" y1="0" x2="0" y2="1"><stop offset="0%" stopColor="#79a89b" stopOpacity=".3" /><stop offset="100%" stopColor="#79a89b" stopOpacity="0" /></linearGradient></defs><path d="M0,205 C50,180 70,190 110,166 S170,166 210,141 S270,151 315,120 S365,132 410,100 S465,108 510,76 S570,93 610,56 S680,67 760,27 L760,250 L0,250 Z" fill="url(#chartFill)" /><path d="M0,205 C50,180 70,190 110,166 S170,166 210,141 S270,151 315,120 S365,132 410,100 S465,108 510,76 S570,93 610,56 S680,67 760,27" fill="none" stroke="#4e8277" strokeWidth="3" strokeLinecap="round" /></svg><div className="chart-x-axis"><span>Jan</span><span>Fev</span><span>Mar</span><span>Abr</span><span>Mai</span><span>Jun</span><span>Jul</span><span>Ago</span><span>Set</span><span>Out</span><span>Nov</span><span>Dez</span></div></div></div></section><section className="panel quick-panel"><div className="panel-heading"><div><p className="eyebrow">atalhos</p><h2>Acesso rápido</h2></div><button className="more-button" onClick={() => toast.info("Personalização dos atalhos em breve.")}>•••</button></div><div className="quick-grid"><QuickAction icon={ShoppingCart} label="Nova venda" tone="green" /><QuickAction icon={Users} label="Novo cliente" tone="blue" /><QuickAction icon={Package} label="Adicionar produto" tone="amber" /><QuickAction icon={FileText} label="Emitir relatório" tone="purple" /></div><div className="insight-card"><div className="insight-icon"><Sparkles size={17} /></div><div><strong>Uma boa semana começa agora</strong><p>Você está 18% acima da sua meta mensal.</p></div></div></section></div>
            <div className="lower-grid"><section className="panel activity-panel"><div className="panel-heading"><div><p className="eyebrow">tempo real</p><h2>Atividade recente</h2></div><button className="link-button" onClick={() => toast.info("Histórico completo em breve.")}>Ver tudo <ArrowUpRight size={15} /></button></div><div className="activity-list">{activities.map((activity) => <div className="activity-row" key={activity.title}><div className={`activity-icon activity-icon--${activity.tone}`}>{activity.tone === "green" ? <ArrowUpRight size={16} /> : activity.tone === "amber" ? <Boxes size={16} /> : <Users size={16} />}</div><div className="activity-copy"><strong>{activity.title}</strong><span>{activity.detail}</span></div><strong className={`activity-value activity-value--${activity.tone}`}>{activity.value}</strong></div>)}</div></section><section className="panel stock-panel"><div className="panel-heading"><div><p className="eyebrow">inventário</p><h2>Saúde do estoque</h2></div><button className="more-button" onClick={() => selectNav("Estoque")}>•••</button></div><div className="stock-total"><div><span>Total em produtos</span><strong>R$ 246.580,00</strong></div><div className="stock-donut"><span>78<small>%</small></span></div></div><div className="stock-bars">{bars.map((height, index) => <i key={index} style={{ height: `${height}%` }} className={height > 88 ? "stock-bar--high" : ""} />)}</div><div className="stock-legend"><span><i className="stock-dot stock-dot--good" />Saudável <b>72%</b></span><span><i className="stock-dot stock-dot--warn" />Atenção <b>18%</b></span><span><i className="stock-dot stock-dot--low" />Baixo <b>10%</b></span></div></section></div>
          </>}
        </div><footer className="app-footer"><span>ERP<span className="brand-dot">.</span>flow</span><span>Dados atualizados há 2 minutos</span><span>© 2026</span></footer>
      </main>
    </div>
  );
}

function MetricCard({ icon: Icon, label, value, change, caption, tone, negative = false }: { icon: typeof Wallet; label: string; value: string; change: string; caption: string; tone: string; negative?: boolean }) {
  return <article className={`metric-card metric-card--${tone}`}><div className="metric-top"><div className="metric-icon"><Icon size={19} /></div><span className={negative ? "metric-change metric-change--negative" : "metric-change"}>{negative ? <ArrowDownRight size={14} /> : <ArrowUpRight size={14} />}{change}</span></div><p>{label}</p><strong>{value}</strong><span className="metric-caption">{caption}</span></article>;
}

function QuickAction({ icon: Icon, label, tone }: { icon: typeof Wallet; label: string; tone: string }) {
  const [loading, setLoading] = useState(false);
  async function start() {
    setLoading(true);
    await new Promise((resolve) => setTimeout(resolve, 500));
    setLoading(false);
    toast.success(`${label}: fluxo iniciado.`);
  }
  return <button className="quick-action" onClick={start} disabled={loading}><span className={`quick-icon quick-icon--${tone}`}>{loading ? <Loader2 size={17} className="spin" /> : <Icon size={19} />}</span><span>{loading ? "Abrindo…" : label}</span>{loading ? <Loader2 size={15} className="spin quick-arrow" /> : <ArrowUpRight size={15} className="quick-arrow" />}</button>;
}

type Customer = ClientRecord;

const initialBranches: BranchRecord[] = [];

function formatCnpj(value: string) {
  return value.replace(/\D/g, "").slice(0, 14).replace(/^(\d{2})(\d)/, "$1.$2").replace(/^(\d{2})\.(\d{3})(\d)/, "$1.$2.$3").replace(/\.(\d{3})(\d)/, ".$1/$2").replace(/(\d{4})(\d)/, "$1-$2");
}

function isValidCnpj(value: string) {
  const digits = value.replace(/\D/g, "");
  if (digits.length !== 14 || /^([0-9])\1+$/.test(digits)) return false;
  const calculate = (length: number) => {
    let sum = 0;
    let weight = length - 5;
    for (let index = 0; index < length; index += 1) {
      sum += Number(digits[index]) * weight;
      weight = weight === 2 ? 9 : weight - 1;
    }
    const remainder = sum % 11;
    return remainder < 2 ? 0 : 11 - remainder;
  };
  return calculate(12) === Number(digits[12]) && calculate(13) === Number(digits[13]);
}

function Branches() {
  const tenantId = import.meta.env.VITE_TENANT_ID ?? "";
  const [branches, setBranches] = useState<BranchRecord[]>(() => {
    const saved = localStorage.getItem("erp-saas-branches");
    return saved ? JSON.parse(saved) : [];
  });
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<BranchRecord | null>(null);
  const [saving, setSaving] = useState(false);
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);
  const pageSize = 5;
  const [form, setForm] = useState({ code: "Matriz", name: "", cnpj: "", city: "", state: "" });
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (!hasLiveApi()) return;
    api.listBranches().then((items) => {
      setBranches(items);
      localStorage.setItem("erp-saas-branches", JSON.stringify(items));
    }).catch(() => toast.info("API de filiais indisponível; exibindo dados locais."));
  }, []);

  const filtered = branches.filter((branch) => `${branch.code} ${branch.name} ${branch.cnpj ?? ""} ${branch.city ?? ""} ${branch.state ?? ""}`.toLowerCase().includes(query.toLowerCase()));
  const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize));
  const visible = filtered.slice((page - 1) * pageSize, page * pageSize);
  const mainBranch = branches.find((branch) => branch.isHeadquarters && branch.isActive);

  function updateForm(key: keyof typeof form, value: string) {
    const nextValue = key === "cnpj" ? formatCnpj(value) : value;
    setForm((current) => ({ ...current, [key]: nextValue }));
    if (errors[key]) setErrors((current) => ({ ...current, [key]: "" }));
  }

  function openCreate() {
    setEditing(null);
    setForm({ code: "Matriz", name: "", cnpj: "", city: "", state: "" });
    setErrors({});
    setFormOpen(true);
  }

  function openEdit(branch: BranchRecord) {
    setEditing(branch);
    setForm({ code: branch.code, name: branch.name, cnpj: branch.cnpj ?? "", city: branch.city ?? "", state: branch.state ?? "" });
    setErrors({});
    setFormOpen(true);
  }

  function validate() {
    const next: Record<string, string> = {};
    if (!form.code.trim()) next.code = "Informe o código da filial.";
    if (!form.name.trim()) next.name = "Informe o nome da filial.";
    if (form.cnpj && !isValidCnpj(form.cnpj)) next.cnpj = "Digite um CNPJ válido com 14 números.";
    if (form.state && !/^[A-Za-z]{2}$/.test(form.state)) next.state = "Use duas letras, por exemplo SP.";
    setErrors(next);
    return Object.keys(next).length === 0;
  }

  async function saveBranch(event: FormEvent) {
    event.preventDefault();
    if (!tenantId) { toast.error("VITE_TENANT_ID não está configurado."); return; }
    if (!validate()) { toast.error("Corrija os campos destacados antes de salvar."); return; }
    if (!editing && form.code.toLowerCase() === "matriz" && mainBranch) { toast.error("Já existe uma filial principal ativa para este tenant."); return; }
    setSaving(true);
    const payload = { ...form, cnpj: form.cnpj || undefined, city: form.city || undefined, state: form.state.toUpperCase() || undefined, isHeadquarters: editing?.isHeadquarters ?? true };
    try {
      let saved: BranchRecord;
      if (hasLiveApi()) {
        saved = editing ? await api.updateBranch(editing.id, payload) : await api.createBranch(payload);
      } else {
        saved = { id: editing?.id ?? `local-branch-${Date.now()}`, ...payload, cnpj: payload.cnpj || null, city: payload.city || null, state: payload.state || null, isActive: editing?.isActive ?? true };
      }
      const next = editing ? branches.map((item) => item.id === editing.id ? saved : item) : [saved, ...branches];
      setBranches(next);
      localStorage.setItem("erp-saas-branches", JSON.stringify(next));
      setFormOpen(false);
      setEditing(null);
      toast.success(editing ? "Filial atualizada." : "Filial principal cadastrada e vinculada ao tenant.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Não foi possível salvar a filial.");
    } finally { setSaving(false); }
  }

  async function removeBranch(branch: BranchRecord) {
    if (!window.confirm(`Excluir a filial ${branch.name}? Esta ação não poderá ser desfeita.`)) return;
    try {
      if (hasLiveApi()) await api.deleteBranch(branch.id);
      const next = branches.filter((item) => item.id !== branch.id);
      setBranches(next);
      localStorage.setItem("erp-saas-branches", JSON.stringify(next));
      setPage(Math.min(page, Math.max(1, Math.ceil((filtered.length - 1) / pageSize))));
      toast.success("Filial excluída.");
    } catch (error) { toast.error(error instanceof Error ? error.message : "Não foi possível excluir a filial."); }
  }

  return <div className="customers-page"><div className="customers-heading"><div><p className="eyebrow">estrutura da empresa</p><h2>Filiais</h2><p className="page-subtitle">Gerencie as filiais vinculadas ao tenant atual.</p></div><button className="primary-button" onClick={openCreate}><Plus size={17} />Cadastrar filial</button></div>
    <div className="panel" style={{ marginBottom: 18 }}><div className="form-title"><div className="quick-icon quick-icon--blue"><Building2 size={18} /></div><div><strong>Vínculo seguro</strong><span>Tenant: {tenantId || "não configurado"}</span></div></div><p className="page-subtitle" style={{ margin: "12px 0 0" }}>O tenant é enviado automaticamente no header <strong>X-Tenant-Id</strong>. A filial principal é identificada pelo sistema.</p></div>
    {formOpen && <form className="customer-form panel" onSubmit={saveBranch} noValidate><div className="form-title"><div className="quick-icon quick-icon--green"><Store size={18} /></div><div><strong>{editing ? "Editar filial" : "Nova filial principal"}</strong><span>Os campos com erro aparecem destacados.</span></div></div><div className="form-grid"><Field label="Código" value={form.code} placeholder="Matriz" onChange={(value) => updateForm("code", value)} required error={errors.code} /><Field label="Nome da filial" value={form.name} placeholder="Casa Nativa Matriz" onChange={(value) => updateForm("name", value)} required error={errors.name} /><Field label="CNPJ" value={form.cnpj} placeholder="00.000.000/0000-00" onChange={(value) => updateForm("cnpj", value)} error={errors.cnpj} /><Field label="Cidade" value={form.city} placeholder="São Paulo" onChange={(value) => updateForm("city", value)} /><Field label="UF" value={form.state} placeholder="SP" onChange={(value) => updateForm("state", value)} error={errors.state} /></div><div className="form-actions"><button type="button" className="secondary-button" onClick={() => setFormOpen(false)}>Cancelar</button><button className="primary-button" type="submit" disabled={saving}>{saving ? <><Loader2 size={16} className="spin" />Salvando…</> : <><Check size={16} />{editing ? "Atualizar filial" : "Salvar filial principal"}</>}</button></div></form>}
    <section className="panel customer-list"><div className="customer-list-top"><div><p className="eyebrow">cadastro ativo</p><h3>{filtered.length} filial(is) encontrada(s)</h3></div><div className="customer-search"><Search size={16} /><input value={query} onChange={(event) => { setQuery(event.target.value); setPage(1); }} placeholder="Buscar filial…" /></div></div><div className="customer-table"><div className="customer-table-head"><span>Filial</span><span>CNPJ</span><span>Localização</span><span>Tipo</span><span>Ações</span></div>{visible.map((branch) => <div className="customer-row" key={branch.id}><div className="customer-person"><div className="quick-icon quick-icon--green"><Store size={16} /></div><div><strong>{branch.name}</strong><span>{branch.code}</span></div></div><span className="customer-contact">{branch.cnpj || "Não informado"}</span><span className="customer-company">{branch.city || "—"}{branch.state ? ` / ${branch.state}` : ""}</span><span className="status-pill status-pill--active">{branch.isHeadquarters ? "Matriz" : "Filial"}</span><span className="row-actions"><button className="row-action" onClick={() => openEdit(branch)} aria-label={`Editar ${branch.name}`}>Editar</button><button className="row-action row-action--danger" onClick={() => removeBranch(branch)} aria-label={`Excluir ${branch.name}`}>Excluir</button></span></div>)}{visible.length === 0 && <div className="empty-state">Nenhuma filial encontrada.</div>}</div><div className="form-actions" style={{ justifyContent: "space-between" }}><span className="page-subtitle">Página {page} de {totalPages}</span><div><button className="secondary-button" disabled={page === 1} onClick={() => setPage((current) => current - 1)}>Anterior</button><button className="secondary-button" disabled={page === totalPages} onClick={() => setPage((current) => current + 1)} style={{ marginLeft: 8 }}>Próxima</button></div></div></section>
  </div>;
}

const initialCustomers: Customer[] = [
  { id: "demo-client-1", code: "CLI-0001", name: "Marina Costa", email: "marina@casanativa.com", phone: "(11) 98812-4430", company: "Casa Nativa", status: "Ativo" },
  { id: "demo-client-2", code: "CLI-0002", name: "Rafael Lima", email: "rafael@origens.com", phone: "(21) 99721-0312", company: "Origens Café", status: "Ativo" },
  { id: "demo-client-3", code: "CLI-0003", name: "Bianca Souza", email: "bianca@atelierb.com", phone: "(31) 99118-2020", company: "Atelier B", status: "Pendente" },
];

const initialProducts: ProductRecord[] = [
  { id: "demo-product-1", name: "Café Origens 250g", sku: "CAF-250-ORG", category: "Alimentos", price: 29.9, stock: 148, active: true },
  { id: "demo-product-2", name: "Caneca Aurora", sku: "CAN-AUR-001", category: "Casa", price: 48.0, stock: 32, active: true },
  { id: "demo-product-3", name: "Cesta Manhã", sku: "CES-MAN-004", category: "Kits", price: 86.5, stock: 8, active: true },
];

function Customers() {
  const [customers, setCustomers] = useState<Customer[]>(() => { const saved = localStorage.getItem("erp-saas-customers"); return saved ? JSON.parse(saved) : initialCustomers; });
  const [query, setQuery] = useState("");
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<Customer | null>(null);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState({ name: "", email: "", phone: "", company: "" });
  useEffect(() => { if (!hasLiveApi()) return; api.listClients().then((items) => { setCustomers(items); localStorage.setItem("erp-saas-customers", JSON.stringify(items)); }).catch(() => toast.info("API de clientes indisponível; exibindo dados locais.")); }, []);
  const filtered = customers.filter((customer) => `${customer.name} ${customer.email} ${customer.company}`.toLowerCase().includes(query.toLowerCase()));
  function updateForm(key: keyof typeof form, value: string) { setForm((current) => ({ ...current, [key]: value })); }
  function openEdit(customer: Customer) { setEditing(customer); setForm({ name: customer.name, email: customer.email, phone: customer.phone, company: customer.company ?? "" }); setFormOpen(true); }
  async function saveCustomer(event: FormEvent) {
    event.preventDefault(); setSaving(true);
    const payload = { ...form };
    try {
      let saved: Customer = { id: editing?.id ?? `local-client-${Date.now()}`, code: editing?.code ?? `CLI-${Date.now()}`, status: editing?.status ?? "Ativo", ...payload };
      if (hasLiveApi()) saved = editing ? await api.updateClient(editing.id, payload) : await api.createClient(payload);
      const next = editing ? customers.map((item) => item.id === editing.id ? saved : item) : [saved, ...customers];
      setCustomers(next); localStorage.setItem("erp-saas-customers", JSON.stringify(next)); setForm({ name: "", email: "", phone: "", company: "" }); setEditing(null); setFormOpen(false); toast.success(editing ? "Cliente atualizado." : "Cliente cadastrado com sucesso.");
    } catch (error) { toast.error(error instanceof Error ? `${error.message} O registro local foi mantido como rascunho.` : "Não foi possível salvar."); }
    finally { setSaving(false); }
  }
  async function removeCustomer(customer: Customer) {
    if (!window.confirm(`Excluir ${customer.name}?`)) return;
    try { if (hasLiveApi()) await api.deleteClient(customer.id); const next = customers.filter((item) => item.id !== customer.id); setCustomers(next); localStorage.setItem("erp-saas-customers", JSON.stringify(next)); toast.success("Cliente excluído."); } catch (error) { toast.error(error instanceof Error ? error.message : "Não foi possível excluir."); }
  }
  function cancelEdit() { setFormOpen(false); setEditing(null); setForm({ name: "", email: "", phone: "", company: "" }); }
  function exportCustomers() { downloadCsv("clientes.csv", ["Nome", "E-mail", "Telefone", "Empresa", "Status"], filtered.map((customer) => [customer.name, customer.email, customer.phone, customer.company, customer.status])); toast.success(`${filtered.length} cliente(s) exportado(s).`); }
  return <div className="customers-page"><div className="customers-heading"><div><p className="eyebrow">relacionamento</p><h2>Clientes</h2><p className="page-subtitle">Mantenha sua base organizada e próxima.</p></div><div className="heading-actions"><button className="secondary-button" onClick={exportCustomers} disabled={filtered.length === 0}><Download size={16} />Exportar CSV</button><button className="primary-button" onClick={() => { setEditing(null); setFormOpen((open) => !open); }}><Plus size={17} />{formOpen && !editing ? "Fechar formulário" : "Novo cliente"}</button></div></div>{formOpen && <form className="customer-form panel" onSubmit={saveCustomer}><div className="form-title"><div className="quick-icon quick-icon--green"><Users size={18} /></div><div><strong>{editing ? "Editar cliente" : "Novo cliente"}</strong><span>Preencha os dados principais para começar.</span></div></div><div className="form-grid"><Field label="Nome completo" value={form.name} placeholder="Ex.: Ana Martins" onChange={(value) => updateForm("name", value)} required /><Field label="E-mail" value={form.email} placeholder="ana@empresa.com" type="email" onChange={(value) => updateForm("email", value)} required /><Field label="Telefone" value={form.phone} placeholder="(11) 99999-9999" onChange={(value) => updateForm("phone", value)} /><Field label="Empresa" value={form.company} placeholder="Nome da empresa" onChange={(value) => updateForm("company", value)} /></div><div className="form-actions"><button type="button" className="secondary-button" onClick={cancelEdit}>Cancelar</button><button className="primary-button" type="submit" disabled={saving}>{saving ? <><Loader2 size={16} className="spin" />Salvando…</> : <><Check size={16} />{editing ? "Atualizar cliente" : "Salvar cliente"}</>}</button></div></form>}<section className="panel customer-list"><div className="customer-list-top"><div><p className="eyebrow">base ativa</p><h3>{customers.length} clientes cadastrados</h3></div><div className="customer-search"><Search size={16} /><input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Buscar cliente…" /></div></div><div className="customer-table"><div className="customer-table-head"><span>Cliente</span><span>Contato</span><span>Empresa</span><span>Status</span><span>Ações</span></div>{filtered.map((customer) => <div className="customer-row" key={customer.id}><div className="customer-person"><div className="avatar">{customer.name.split(" ").map((part) => part[0]).slice(0, 2).join("")}</div><div><strong>{customer.name}</strong><span>{customer.email}</span></div></div><span className="customer-contact">{customer.phone}</span><span className="customer-company">{customer.company || "—"}</span><span><i className={`status-pill status-pill--${customer.status === "Ativo" ? "active" : "pending"}`}>{customer.status}</i></span><span className="row-actions"><button className="row-action" onClick={() => openEdit(customer)} aria-label={`Editar ${customer.name}`}>Editar</button><button className="row-action row-action--danger" onClick={() => removeCustomer(customer)} aria-label={`Excluir ${customer.name}`}>Excluir</button></span></div>)}{filtered.length === 0 && <div className="empty-state">Nenhum cliente encontrado.</div>}</div></section></div>;
}

function Products() {
  const [products, setProducts] = useState<ProductRecord[]>(() => { const saved = localStorage.getItem("erp-saas-products"); return saved ? JSON.parse(saved) : initialProducts; });
  const [formOpen, setFormOpen] = useState(false); const [saving, setSaving] = useState(false); const [query, setQuery] = useState("");
  const [form, setForm] = useState({ name: "", sku: "", category: "", price: "", stock: "" });
  useEffect(() => { if (!hasLiveApi()) return; api.listProducts().then((items) => { setProducts(items); localStorage.setItem("erp-saas-products", JSON.stringify(items)); }).catch(() => toast.info("API de produtos indisponível; exibindo dados locais.")); }, []);
  const filtered = products.filter((product) => `${product.name} ${product.sku} ${product.category}`.toLowerCase().includes(query.toLowerCase()));
  async function saveProduct(event: FormEvent) { event.preventDefault(); setSaving(true); const payload = { name: form.name, sku: form.sku, category: form.category || undefined, price: Number(form.price), stock: Number(form.stock) }; try { let saved: ProductRecord = { id: `local-product-${Date.now()}`, ...payload, category: form.category || null, active: true }; if (hasLiveApi()) saved = await api.createProduct(payload); const next = [saved, ...products]; setProducts(next); localStorage.setItem("erp-saas-products", JSON.stringify(next)); setForm({ name: "", sku: "", category: "", price: "", stock: "" }); setFormOpen(false); toast.success("Produto adicionado ao estoque."); } catch (error) { toast.error(error instanceof Error ? `${error.message} O item foi salvo localmente.` : "Não foi possível salvar."); } finally { setSaving(false); } }
  function updateForm(key: keyof typeof form, value: string) { setForm((current) => ({ ...current, [key]: value })); }
  function exportProducts() { downloadCsv("produtos.csv", ["Nome", "SKU", "Categoria", "Preço", "Estoque"], filtered.map((product) => [product.name, product.sku, product.category, product.price.toFixed(2).replace(".", ","), product.stock])); toast.success(`${filtered.length} produto(s) exportado(s).`); }
  return <div className="customers-page"><div className="customers-heading"><div><p className="eyebrow">inventário</p><h2>Produtos</h2><p className="page-subtitle">Cadastre itens e acompanhe o saldo disponível.</p></div><div className="heading-actions"><button className="secondary-button" onClick={exportProducts} disabled={filtered.length === 0}><Download size={16} />Exportar CSV</button><button className="primary-button" onClick={() => setFormOpen((open) => !open)}><Plus size={17} />{formOpen ? "Fechar formulário" : "Adicionar produto"}</button></div></div>{formOpen && <form className="customer-form panel" onSubmit={saveProduct}><div className="form-title"><div className="quick-icon quick-icon--amber"><Package size={18} /></div><div><strong>Novo produto</strong><span>O item aparecerá no seu estoque imediatamente.</span></div></div><div className="form-grid"><Field label="Nome do produto" value={form.name} placeholder="Ex.: Café especial 250g" onChange={(value) => updateForm("name", value)} required /><Field label="SKU" value={form.sku} placeholder="CAF-250-001" onChange={(value) => updateForm("sku", value)} required /><Field label="Categoria" value={form.category} placeholder="Alimentos" onChange={(value) => updateForm("category", value)} required /><Field label="Preço (R$)" value={form.price} placeholder="0,00" type="number" onChange={(value) => updateForm("price", value)} required /><Field label="Quantidade inicial" value={form.stock} placeholder="0" type="number" onChange={(value) => updateForm("stock", value)} required /></div><div className="form-actions"><button type="button" className="secondary-button" onClick={() => setFormOpen(false)}>Cancelar</button><button className="primary-button" type="submit" disabled={saving}>{saving ? <><Loader2 size={16} className="spin" />Salvando…</> : <><Check size={16} />Salvar produto</>}</button></div></form>}<section className="panel customer-list"><div className="customer-list-top"><div><p className="eyebrow">estoque atual</p><h3>{products.length} produtos cadastrados</h3></div><div className="customer-search"><Search size={16} /><input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Buscar produto…" /></div></div><div className="customer-table product-table"><div className="customer-table-head"><span>Produto</span><span>SKU</span><span>Categoria</span><span>Preço</span><span>Saldo</span></div>{filtered.map((product) => <div className="customer-row" key={product.id}><div className="customer-person"><div className="quick-icon quick-icon--amber"><Package size={16} /></div><div><strong>{product.name}</strong><span>Atualizado agora</span></div></div><span className="customer-contact">{product.sku}</span><span className="customer-company">{product.category}</span><span className="customer-contact">R$ {product.price.toFixed(2).replace(".", ",")}</span><span className={product.stock < 10 ? "stock-low" : "stock-ok"}>{product.stock} un.</span></div>)}{filtered.length === 0 && <div className="empty-state">Nenhum produto encontrado.</div>}</div></section></div>;
}

function Field({ label, value, placeholder, type = "text", required, error, onChange }: { label: string; value: string; placeholder: string; type?: string; required?: boolean; error?: string; onChange: (value: string) => void }) {
  return <label className={`form-field ${error ? "form-field--error" : ""}`}><span>{label}{required && " *"}</span><input type={type} value={value} placeholder={placeholder} onChange={(event) => onChange(event.target.value)} required={required} aria-invalid={Boolean(error)} aria-describedby={error ? `${label}-error` : undefined} />{error && <small id={`${label}-error`} className="field-error">{error}</small>}</label>;
}
