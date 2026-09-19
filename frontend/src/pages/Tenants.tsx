import { FormEvent, useEffect, useState } from 'react';
import { api, request } from '../lib/api';
import { formatDate, TENANT_STATUS } from '../lib/format';
import { useAuth } from '../lib/auth';
import { Badge, btnDanger, btnGhost, btnPrimary, cardClass, EmptyState, ErrorText, Field, inputClass, Modal, PageHeader } from '../components/ui';

interface Tenant {
  id: string;
  name: string;
  cnpj: string;
  email: string;
  phone?: string;
  status: number;
  maxUsers?: number;
  trialEnd?: string;
  createdAt: string;
}

const MODULES = ['sales', 'purchases', 'inventory', 'financial', 'fiscal', 'reports', 'dashboard'];

function TenantStatusBadge({ status }: { status: number }) {
  const tones = ['green', 'gray', 'red', 'yellow', 'red'];
  return <Badge tone={tones[status] || 'gray'}>{TENANT_STATUS[status] ?? status}</Badge>;
}

export default function Tenants() {
  const { user } = useAuth();
  const isAdmin = !!user?.roles?.includes('Admin');
  const [tenants, setTenants] = useState<Tenant[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formError, setFormError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const emptyForm = {
    name: '',
    cnpj: '',
    email: '',
    phone: '',
    maxUsers: '10',
    trialDays: '30',
    modules: ['sales', 'purchases', 'inventory', 'financial', 'dashboard'] as string[],
    adminEmail: '',
    adminPassword: 'Admin@123'
  };
  const [form, setForm] = useState(emptyForm);

  const load = () => {
    setLoading(true);
    request('/tenants?pageNumber=1&pageSize=100')
      .then((d: any) => setTenants(d.data?.data ?? []))
      .catch((e) => setError(e?.message))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    if (isAdmin) load();
    else setLoading(false);
  }, [isAdmin]);

  if (!isAdmin) {
    return <EmptyState message="Acesso restrito a administradores da plataforma." />;
  }

  const toggleModule = (m: string) => {
    setForm((f) => ({
      ...f,
      modules: f.modules.includes(m) ? f.modules.filter((x) => x !== m) : [...f.modules, m]
    }));
  };

  const handleCreate = async (e: FormEvent) => {
    e.preventDefault();
    setFormError(null);
    setSaving(true);
    try {
      await api('/tenants', {
        method: 'POST',
        body: JSON.stringify({
          name: form.name,
          cnpj: form.cnpj,
          email: form.email,
          phone: form.phone,
          maxUsers: parseInt(form.maxUsers) || undefined,
          trialDays: parseInt(form.trialDays) || undefined,
          enabledModules: form.modules,
          adminUser: {
            username: form.adminEmail,
            email: form.adminEmail,
            password: form.adminPassword,
            firstName: 'Admin',
            lastName: form.name,
            roles: ['TenantAdmin']
          }
        })
      });
      setModalOpen(false);
      setForm(emptyForm);
      load();
    } catch (err: any) {
      setFormError(err?.message || 'Falha ao criar empresa');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (t: Tenant) => {
    if (!window.confirm(`Excluir a empresa "${t.name}"? Os dados do banco dela serão mantidos.`)) return;
    try {
      await api(`/tenants/${t.id}`, { method: 'DELETE' });
      load();
    } catch (err: any) {
      setError(err?.message);
    }
  };

  return (
    <div>
      <PageHeader
        title="Empresas (Tenants)"
        subtitle="Empresas contratantes da plataforma"
        action={
          <button className={btnPrimary} onClick={() => setModalOpen(true)}>
            + Nova empresa
          </button>
        }
      />
      <ErrorText error={error} />
      <div className={`${cardClass} overflow-hidden`}>
        {loading ? (
          <EmptyState message="Carregando..." />
        ) : tenants.length === 0 ? (
          <EmptyState message="Nenhuma empresa encontrada." />
        ) : (
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-slate-200 bg-slate-50 text-left text-xs uppercase tracking-wide text-slate-500">
                <th className="px-5 py-3 font-medium">Empresa</th>
                <th className="px-5 py-3 font-medium">CNPJ</th>
                <th className="px-5 py-3 font-medium">Contato</th>
                <th className="px-5 py-3 font-medium">Usuários máx.</th>
                <th className="px-5 py-3 font-medium">Status</th>
                <th className="px-5 py-3 font-medium">Criada em</th>
                <th className="px-5 py-3" />
              </tr>
            </thead>
            <tbody>
              {tenants.map((t) => (
                <tr key={t.id} className="border-b border-slate-50 last:border-0 hover:bg-slate-50/50">
                  <td className="px-5 py-3 font-medium text-slate-800">{t.name}</td>
                  <td className="px-5 py-3 text-slate-600">{t.cnpj}</td>
                  <td className="px-5 py-3 text-slate-600">{t.email}</td>
                  <td className="px-5 py-3 text-slate-600">{t.maxUsers ?? '—'}</td>
                  <td className="px-5 py-3"><TenantStatusBadge status={t.status} /></td>
                  <td className="px-5 py-3 text-slate-500">{formatDate(t.createdAt)}</td>
                  <td className="px-5 py-3 text-right">
                    <button className={btnDanger} onClick={() => handleDelete(t)}>Excluir</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <Modal open={modalOpen} title="Nova empresa" onClose={() => setModalOpen(false)}>
        <form onSubmit={handleCreate} className="space-y-4">
          <div className="grid grid-cols-2 gap-3">
            <div className="col-span-2">
              <Field label="Nome da empresa *">
                <input className={inputClass} value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
              </Field>
            </div>
            <Field label="CNPJ *">
              <input className={inputClass} value={form.cnpj} onChange={(e) => setForm({ ...form, cnpj: e.target.value })} placeholder="12345678000199" required />
            </Field>
            <Field label="Telefone">
              <input className={inputClass} value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} />
            </Field>
            <div className="col-span-2">
              <Field label="E-mail da empresa *">
                <input type="email" className={inputClass} value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} required />
              </Field>
            </div>
            <Field label="Máx. usuários">
              <input type="number" min="1" className={inputClass} value={form.maxUsers} onChange={(e) => setForm({ ...form, maxUsers: e.target.value })} />
            </Field>
            <Field label="Trial (dias)">
              <input type="number" min="0" className={inputClass} value={form.trialDays} onChange={(e) => setForm({ ...form, trialDays: e.target.value })} />
            </Field>
          </div>
          <div>
            <span className="mb-1.5 block text-xs font-medium text-slate-600">Módulos habilitados</span>
            <div className="flex flex-wrap gap-1.5">
              {MODULES.map((m) => (
                <button
                  type="button"
                  key={m}
                  onClick={() => toggleModule(m)}
                  className={`rounded-full px-3 py-1 text-xs font-medium ${
                    form.modules.includes(m) ? 'bg-indigo-100 text-indigo-700' : 'bg-slate-100 text-slate-500'
                  }`}
                >
                  {m}
                </button>
              ))}
            </div>
          </div>
          <div className="rounded-lg border border-slate-200 p-3">
            <div className="mb-2 text-xs font-semibold text-slate-600">Usuário administrador da empresa</div>
            <div className="space-y-3">
              <Field label="E-mail do admin *">
                <input type="email" className={inputClass} value={form.adminEmail} onChange={(e) => setForm({ ...form, adminEmail: e.target.value })} required />
              </Field>
              <Field label="Senha *">
                <input className={inputClass} value={form.adminPassword} onChange={(e) => setForm({ ...form, adminPassword: e.target.value })} required />
              </Field>
            </div>
          </div>
          <ErrorText error={formError} />
          <div className="flex justify-end gap-2">
            <button type="button" className={btnGhost} onClick={() => setModalOpen(false)}>Cancelar</button>
            <button type="submit" className={btnPrimary} disabled={saving}>
              {saving ? 'Criando...' : 'Criar empresa'}
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
