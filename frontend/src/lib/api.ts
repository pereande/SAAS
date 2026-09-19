const TOKEN_KEY = 'erp_token';

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

export function setToken(token: string | null): void {
  if (token) {
    localStorage.setItem(TOKEN_KEY, token);
  } else {
    localStorage.removeItem(TOKEN_KEY);
  }
}

export class ApiError extends Error {
  errors?: Record<string, string[]>;
  status: number;

  constructor(message: string, status: number, errors?: Record<string, string[]>) {
    super(message);
    this.status = status;
    this.errors = errors;
  }
}

interface Envelope<T> {
  success: boolean;
  message?: string;
  errorCode?: string;
  errors?: Record<string, string[]>;
  data?: T;
}

/**
 * Faz uma requisição à API e retorna o campo `data` do envelope de resposta.
 */
export async function api<T = any>(path: string, options: RequestInit = {}): Promise<T> {
  const envelope = await request<Envelope<T>>(path, options);
  return envelope.data as T;
}

/**
 * Faz uma requisição à API e retorna o envelope completo
 * (útil para respostas paginadas, que têm totalCount).
 */
export async function request<T = any>(path: string, options: RequestInit = {}): Promise<T> {
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...((options.headers as Record<string, string>) || {})
  };
  const token = getToken();
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const res = await fetch(`/api${path}`, { ...options, headers });
  let body: any = null;
  try {
    body = await res.json();
  } catch {
    // resposta sem corpo
  }

  if (!res.ok) {
    if (res.status === 401) {
      setToken(null);
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }
    const detail = body?.errors ? Object.values(body.errors).flat().join(' • ') : null;
    throw new ApiError(detail || body?.message || `Falha na requisição (${res.status})`, res.status, body?.errors);
  }

  return body as T;
}

export interface PagedResult<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
