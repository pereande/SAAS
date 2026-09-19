import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../lib/auth';
import { api, request } from '../lib/api';
import { formatCurrency, formatDateTime, SALE_STATUS, statusTone } from '../lib/format';
import { Badge, cardClass, EmptyState } from '../components/ui';

interface StatCardProps {
  label: string;
  value: string | number;
  to: string;
  icon: string;
}

function StatCard({ label, value, to, icon }: StatCardProps) {
  return (
    <Link to={to} className={`${cardClass} p-5 transition-shadow hover:shadow-md`}>
      <div className="flex items-center justify-between">
        <div>
          <div className="text-xs font-medium text-slate-500">{label}</div>
          <div className="mt-1 text-2xl font-semibold text-slate-800">{value}</div>
        </div>
        <span className="text-2xl">{icon}</span>
      </div>
    </Link>
  );
}

export default function Dashboard() {
  const { user } = useAuth();
  const [stats, setStats] = useState<{ products?: number; sales?: number; purchases?: number; tenants?: number; salesTotal?: number }>({});
  const [recentSales, setRecentSales] = useState<any[]>([]);

  const isAdmin = !!user?.roles?.includes('Admin');
  const hasTenant = !!user?.tenantId;

  useEffect(() => {
    if (isAdmin) {
      request('/tenants?pageNumber=1&pageSize=5')
        .then((d: any) => setStats((s) => ({ ...s, tenants: d.data?.totalCount ?? 0 })))
        .catch(() => {});
    }
    if (hasTenant) {
      const load = async () => {
        try {
          const [prod, inv, sal, pur] = await Promise.all([
            request('/products?pageNumber=1&pageSize=1'),
            request('/inventory?pageNumber=1&pageSize=1'),
            request('/sales?pageNumber=1&pageSize=5'),
            request('/purchases?pageNumber=1&pageSize=1')
          ]);
          setStats((s) => ({
            ...s,
            products: prod.data?.totalCount ?? 0,
            inventory: inv.data?.totalCount ?? 0,
            sales: sal.data?.totalCount ?? 0,
            purchases: pur.data?.totalCount ?? 0
          }));
          setRecentSales(sal.data?.data ?? []);
        } catch {
          // dashboard falha silenciosamente
        }
      };
      load();
    }
  }, [isAdmin, hasTenant]);

  return (
    <div>
      <h1 className="text-xl font-semibold text-slate-800">Olá, {user?.fullName} 👋</h1>
      <p className="mt-0.5 text-sm text-slate-500">
        Bem-vindo ao ERP SaaS. {hasTenant ? 'Aqui está um resumo da sua empresa.' : 'Você é administrador da plataforma. Gerencie as empresas contratantes.'}
      </p>

      <div className="mt-6 grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {isAdmin && <StatCard label="Empresas" value={stats.tenants ?? '—'} to="/tenants" icon="🏢" />}
        {hasTenant && <StatCard label="Produtos" value={stats.products ?? '—'} to="/products" icon="📦" />}
        {hasTenant && <StatCard label="Registros de estoque" value={stats.inventory ?? '—'} to="/inventory" icon="🏬" />}
        {hasTenant && <StatCard label="Vendas" value={stats.sales ?? '—'} to="/sales" icon="💰" />}
        {hasTenant && <StatCard label="Compras" value={stats.purchases ?? '—'} to="/purchases" icon="🛒" />}
      </div>

      {hasTenant && recentSales.length > 0 && (
        <div className={`${cardClass} mt-6 overflow-hidden`}>
          <div className="border-b border-slate-200 px-5 py-3 text-sm font-semibold text-slate-700">
            Últimas vendas
          </div>
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-slate-100 text-left text-xs uppercase tracking-wide text-slate-500">
                <th className="px-5 py-2.5 font-medium">Número</th>
                <th className="px-5 py-2.5 font-medium">Data</th>
                <th className="px-5 py-2.5 font-medium">Cliente</th>
                <th className="px-5 py-2.5 font-medium">Status</th>
                <th className="px-5 py-2.5 text-right font-medium">Total</th>
              </tr>
            </thead>
            <tbody>
              {recentSales.map((s) => (
                <tr key={s.id} className="border-b border-slate-50 last:border-0">
                  <td className="px-5 py-2.5 font-medium text-slate-700">{s.number}</td>
                  <td className="px-5 py-2.5 text-slate-500">{formatDateTime(s.saleDate)}</td>
                  <td className="px-5 py-2.5 text-slate-700">{s.clientName || '—'}</td>
                  <td className="px-5 py-2.5">
                    <Badge tone={statusTone(s.status, 7)}>{SALE_STATUS[s.status] ?? s.status}</Badge>
                  </td>
                  <td className="px-5 py-2.5 text-right font-medium text-slate-800">{formatCurrency(s.total)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
