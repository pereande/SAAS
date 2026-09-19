export type SessionUser = {
  id: string;
  name: string;
  email: string;
  role: string;
  initials: string;
  demo?: boolean;
};

export type ClientRecord = {
  id: string;
  code: string;
  name: string;
  email: string;
  phone: string;
  company: string | null;
  status: "Ativo" | "Pendente";
};

export type ClientInput = {
  name: string;
  email: string;
  phone?: string;
  company?: string;
  code?: string;
};

export type ProductRecord = {
  id: string;
  sku: string;
  name: string;
  category: string | null;
  price: number;
  stock: number;
  active: boolean;
};

export type ProductInput = {
  name: string;
  sku: string;
  category?: string;
  price: number;
  stock: number;
  code?: string;
};

export type BranchRecord = {
  id: string;
  code: string;
  name: string;
  cnpj: string | null;
  city: string | null;
  state: string | null;
  isHeadquarters: boolean;
  isActive: boolean;
};

export type BranchInput = {
  code: string;
  name: string;
  cnpj?: string;
  city?: string;
  state?: string;
  isHeadquarters: boolean;
};

type ApiEnvelope = {
  success?: boolean;
  message?: string;
  data?: {
    token?: string;
    user?: { id?: string; username?: string; email?: string; fullName?: string; firstName?: string; roles?: string[] };
  };
  token?: string;
};

const API_URL = (import.meta.env.VITE_API_URL ?? "").replace(/\/$/, "");
const TENANT_ID = import.meta.env.VITE_TENANT_ID ?? "";
const SESSION_KEY = "erp-saas-session";
const TOKEN_KEY = "erp-saas-token";

function userFromApi(payload: ApiEnvelope): SessionUser {
  const account = payload.data?.user;
  const name = account?.fullName || account?.username || account?.email || "Usuário";
  const initials = name.split(/[\s@]+/).filter(Boolean).slice(0, 2).map((part) => part[0]?.toUpperCase()).join("");
  return { id: account?.id || "api-user", name, email: account?.email || account?.username || "", role: account?.roles?.[0] || "Usuário", initials: initials || "US" };
}

async function requestResource<T>(resource: string, method: string, body?: unknown): Promise<T> {
  const token = localStorage.getItem(TOKEN_KEY);
  const response = await fetch(`${API_URL}/api/${resource}`, { method, headers: { "Content-Type": "application/json", ...(token ? { Authorization: `Bearer ${token}` } : {}), ...(TENANT_ID ? { "X-Tenant-Id": TENANT_ID } : {}) }, ...(body ? { body: JSON.stringify(body) } : {}) });
  if (!response.ok) throw new Error(`Endpoint /api/${resource} ainda não está disponível.`);
  if (response.status === 204) return undefined as T;
  return (await response.json()) as T;
}

export const api = {
  async login(username: string, password: string): Promise<SessionUser> {
    const response = await fetch(`${API_URL}/api/auth/login`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ username, password }) });
    const payload = (await response.json().catch(() => ({}))) as ApiEnvelope;
    if (!response.ok || payload.success === false) throw new Error(payload.message || "Não foi possível entrar. Confira seus dados.");
    const token = payload.data?.token || payload.token;
    if (token) localStorage.setItem(TOKEN_KEY, token);
    const user = userFromApi(payload);
    localStorage.setItem(SESSION_KEY, JSON.stringify(user));
    return user;
  },
  async getSession(): Promise<SessionUser | null> {
    const value = localStorage.getItem(SESSION_KEY);
    if (!value) return null;
    try { return JSON.parse(value) as SessionUser; } catch { localStorage.removeItem(SESSION_KEY); return null; }
  },
  logout() { localStorage.removeItem(SESSION_KEY); localStorage.removeItem(TOKEN_KEY); },
  async listClients() { return requestResource<ClientRecord[]>("clients", "GET"); },
  async createClient(client: ClientInput) { return requestResource<ClientRecord>("clients", "POST", client); },
  async updateClient(id: string, client: Partial<ClientInput>) { return requestResource<ClientRecord>(`clients/${id}`, "PUT", client); },
  async deleteClient(id: string) { await requestResource(`clients/${id}`, "DELETE"); },
  async listProducts() { return requestResource<ProductRecord[]>("products", "GET"); },
  async createProduct(product: ProductInput) { return requestResource<ProductRecord>("products", "POST", product); },
  async listBranches() { return requestResource<BranchRecord[]>("branches", "GET"); },
  async createBranch(branch: BranchInput) { return requestResource<BranchRecord>("branches", "POST", branch); },
  async updateBranch(id: string, branch: Partial<BranchInput>) { return requestResource<BranchRecord>(`branches/${id}`, "PUT", branch); },
  async deleteBranch(id: string) { await requestResource(`branches/${id}`, "DELETE"); },
};

export function hasLiveApi() { return true; }
