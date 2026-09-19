import { ReactNode } from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../lib/auth';
import { getToken } from '../lib/api';

function isPlatformAdmin(roles: string[] | undefined): boolean {
  return !!roles?.includes('Admin');
}

export default function Layout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const admin = isPlatformAdmin(user?.roles);
  const hasTenant = !!user?.tenantId;

  const navLink = (to: string, label: string, icon: string) => (
    <NavLink
      to={to}
      className={({ isActive }) =>
        `flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors ${
          isActive ? 'bg-indigo-600 text-white' : 'text-slate-300 hover:bg-slate-800 hover:text-white'
        }`
      }
    >
      <span className="text-base">{icon}</span>
      {label}
    </NavLink>
  );

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  return (
    <div className="flex h-full">
      <aside className="flex w-60 shrink-0 flex-col bg-slate-900">
        <div className="flex items-center gap-2.5 px-5 py-5">
          <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-indigo-600 text-sm font-bold text-white">
            E
          </div>
          <div>
            <div className="text-sm font-semibold text-white">ERP SaaS</div>
            <div className="text-[11px] text-slate-400">Gestão Empresarial</div>
          </div>
        </div>
        <nav className="flex-1 space-y-1 px-3 py-2">
          {navLink('/', 'Dashboard', '📊')}
          {admin && navLink('/tenants', 'Empresas', '🏢')}
          {hasTenant && navLink('/persons', 'Pessoas', '👥')}
          {hasTenant && navLink('/products', 'Produtos', '📦')}
          {hasTenant && navLink('/inventory', 'Estoque', '🏬')}
          {hasTenant && navLink('/sales', 'Vendas', '💰')}
          {hasTenant && navLink('/purchases', 'Compras', '🛒')}
        </nav>
        <div className="border-t border-slate-800 px-4 py-3 text-[11px] text-slate-500">
          v1.0.0 · .NET 8 + PostgreSQL
        </div>
      </aside>
      <div className="flex min-w-0 flex-1 flex-col">
        <header className="flex items-center justify-end gap-3 border-b border-slate-200 bg-white px-6 py-3">
          {user && (
            <div className="flex items-center gap-3">
              <div className="text-right">
                <div className="text-sm font-medium text-slate-800">{user.fullName}</div>
                <div className="text-xs text-slate-500">
                  {user.roles.join(', ')}
                  {getToken() ? '' : ''}
                </div>
              </div>
              <div className="flex h-9 w-9 items-center justify-center rounded-full bg-indigo-100 text-sm font-semibold text-indigo-700">
                {user.fullName.slice(0, 1).toUpperCase()}
              </div>
              <button onClick={handleLogout} className="text-xs font-medium text-slate-500 hover:text-rose-600">
                Sair
              </button>
            </div>
          )}
        </header>
        <main className="flex-1 overflow-y-auto bg-slate-50 px-6 py-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
