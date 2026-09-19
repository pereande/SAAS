import { FormEvent, useEffect, useState } from 'react';
import { api, request } from '../lib/api';
import { useAuth } from '../lib/auth';
import { btnDanger, btnGhost, btnPrimary, cardClass, EmptyState, ErrorText, Field, inputClass, Modal, PageHeader } from '../components/ui';

interface Person {
  id: string;
  code: string;
  name: string;
  document?: string;
  email?: string;
  phone?: string;
  type: 'client' | 'supplier';
  isActive: boolean;
}

export default function Persons() {
  const { user } = useAuth();
  const hasTenant = !!user?.tenantId;
  const [tab, setTab] = useState<'client' | 'supplier'>('client');
  const [persons, setPersons] = useState<Person[]>([]);
  const [search, setSearch] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState({ name: '', document: '', email: '', phone: '' });

  const load = () => {
    setLoading(true);
    request(`/persons?type=${tab}&pageNumber=1&pageSize=100${search ? `&name=${encodeURIComponent(search)}` : ''}`)
      .then((d: any) => setPersons(d.data?.data ?? []))
      .catch((e) => setError(e?.message))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    if (hasTenant) load();
    else setLoading(false);
  }, [tab, hasTenant]);

  if (!hasTenant) {
    return <EmptyState message="Módulo disponível apenas para usuários de uma empresa (tenant)." />;
  }

  const handleCreate = async (e: FormEvent) => {
    e.preventDefault();
    setFormError(null);
    setSaving(true);
    try {
      await api('/persons', {
        method: 'POST',
        body: JSON.stringify({ type: tab, ...form })
      });
      setModalOpen(false);
      setForm({ name: '', document: '', email: '', phone: '' });
      load();
    } catch (err: any) {
      setFormError(err?.message || 'Falha ao criar');
    } finally {
      setSaving(false);
    }
  };

  const handleDeactivate = async (p: Person) => {
    if (!window.confirm(`Desativar "${p.name}"?`)) return;
    try {
      await api(`/persons/${p.id}?type=${tab}`, { method: 'DELETE' });
      load();
    } catch (err: any) {
      setError(err?.message);
    }
  };

  return (
    <div>
      <PageHeader
        title="Pessoas"
        subtitle="Clientes e fornecedores da sua empresa"
        action={<button className={btnPrimary} onClick={() => setModalOpen(true)}>+ Novo</button>}
      />
      <ErrorText error={error} />

      <div className="mb-4 flex flex-wrap items-center gap-2">
        {(['client', 'supplier'] as const).map((t) => (
          <button
            key={t}
            onClick={() => setTab(t)}
            className={`rounded-lg px-3.5 py-1.5 text-sm font-medium ${
              tab === t ? 'bg-indigo-600 text-white' : 'border border-slate-300 bg-white text-slate-600 hover:bg-slate-50'
            }`}
          >
            {t === 'client' ? 'Clientes' : 'Fornecedores'}
          </button>
        ))}
        <input
          className={`${inputClass} ml-auto max-w-60`}
          placeholder="Buscar por nome..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && load()}
        />
      </div>

      <div className={`${cardClass} overflow-hidden`}>
        {loading ? (
          <EmptyState message="Carregando..." />
        ) : persons.length === 0 ? (
          <EmptyState message={tab === 'client' ? 'Nenhum cliente encontrado.' : 'Nenhum fornecedor encontrado.'} />
        ) : (
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-slate-200 bg-slate-50 text-left text-xs uppercase tracking-wide text-slate-500">
                <th className="px-5 py-3 font-medium">Código</th>
                <th className="px-5 py-3 font-medium">Nome</th>
                <th className="px-5 py-3 font-medium">Documento</th>
                <th className="px-5 py-3 font-medium">E-mail</th>
                <th className="px-5 py-3 font-medium">Telefone</th>
                <th className="px-5 py-3" />
              </tr>
            </thead>
            <tbody>
              {persons.map((p) => (
                <tr key={p.id} className="border-b border-slate-50 last:border-0 hover:bg-slate-50/50">
                  <td className="px-5 py-3 font-medium text-slate-700">{p.code}</td>
                  <td className="px-5 py-3 text-slate-800">
                    {p.name} {!p.isActive && <span className="text-xs text-rose-500">(inativo)</span>}
                  </td>
                  <td className="px-5 py-3 text-slate-600">{p.document || '—'}</td>
                  <td className="px-5 py-3 text-slate-600">{p.email || '—'}</td>
                  <td className="px-5 py-3 text-slate-600">{p.phone || '—'}</td>
                  <td className="px-5 py-3 text-right">
                    {p.isActive && (
                      <button className={btnDanger} onClick={() => handleDeactivate(p)}>Desativar</button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <Modal open={modalOpen} title={tab === 'client' ? 'Novo cliente' : 'Novo fornecedor'} onClose={() => setModalOpen(false)}>
        <form onSubmit={handleCreate} className="space-y-4">
          <Field label="Nome *">
            <input className={inputClass} value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
          </Field>
          <Field label="CPF / CNPJ">
            <input className={inputClass} value={form.document} onChange={(e) => setForm({ ...form, document: e.target.value })} />
          </Field>
          <Field label="E-mail">
            <input type="email" className={inputClass} value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
          </Field>
          <Field label="Telefone">
            <input className={inputClass} value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} />
          </Field>
          <ErrorText error={formError} />
          <div className="flex justify-end gap-2">
            <button type="button" className={btnGhost} onClick={() => setModalOpen(false)}>Cancelar</button>
            <button type="submit" className={btnPrimary} disabled={saving}>
              {saving ? 'Salvando...' : 'Salvar'}
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
