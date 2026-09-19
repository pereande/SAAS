import { FormEvent, useEffect, useState } from 'react';
import { api, request } from '../lib/api';
import { useAuth } from '../lib/auth';
import { formatCurrency } from '../lib/format';
import { Badge, btnDanger, btnGhost, btnPrimary, cardClass, EmptyState, ErrorText, Field, inputClass, Modal, PageHeader } from '../components/ui';

interface Product {
  id: string;
  code: string;
  name: string;
  description?: string;
  unitOfMeasure: string;
  costPrice?: number;
  salePrice?: number;
  minStock?: number;
  manageStock: boolean;
  isActive: boolean;
}

const emptyForm = {
  id: '',
  code: '',
  name: '',
  description: '',
  unitOfMeasure: 'UN',
  costPrice: '',
  salePrice: '',
  minStock: '',
  manageStock: true
};

export default function Products() {
  const { user } = useAuth();
  const hasTenant = !!user?.tenantId;
  const [products, setProducts] = useState<Product[]>([]);
  const [search, setSearch] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState(emptyForm);

  const load = () => {
    setLoading(true);
    request(`/products?pageNumber=1&pageSize=100${search ? `&name=${encodeURIComponent(search)}` : ''}`)
      .then((d: any) => setProducts(d.data?.data ?? []))
      .catch((e) => setError(e?.message))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    if (hasTenant) load();
    else setLoading(false);
  }, [hasTenant]);

  if (!hasTenant) {
    return <EmptyState message="Módulo disponível apenas para usuários de uma empresa (tenant)." />;
  }

  const openCreate = () => {
    setForm(emptyForm);
    setFormError(null);
    setModalOpen(true);
  };

  const openEdit = (p: Product) => {
    setForm({
      id: p.id,
      code: p.code,
      name: p.name,
      description: p.description || '',
      unitOfMeasure: p.unitOfMeasure,
      costPrice: p.costPrice?.toString() || '',
      salePrice: p.salePrice?.toString() || '',
      minStock: p.minStock?.toString() || '',
      manageStock: p.manageStock
    });
    setFormError(null);
    setModalOpen(true);
  };

  const handleSave = async (e: FormEvent) => {
    e.preventDefault();
    setFormError(null);
    setSaving(true);
    const payload: any = {
      name: form.name,
      code: form.code || null,
      description: form.description || null,
      unitOfMeasure: form.unitOfMeasure || 'UN',
      costPrice: form.costPrice ? parseFloat(form.costPrice) : null,
      salePrice: form.salePrice ? parseFloat(form.salePrice) : null,
      minStock: form.minStock ? parseFloat(form.minStock) : null,
      manageStock: form.manageStock,
      isActive: true
    };
    try {
      if (form.id) {
        await api(`/products/${form.id}`, { method: 'PUT', body: JSON.stringify(payload) });
      } else {
        await api('/products', { method: 'POST', body: JSON.stringify(payload) });
      }
      setModalOpen(false);
      load();
    } catch (err: any) {
      setFormError(err?.message || 'Falha ao salvar produto');
    } finally {
      setSaving(false);
    }
  };

  const handleDeactivate = async (p: Product) => {
    if (!window.confirm(`Desativar o produto "${p.name}"?`)) return;
    try {
      await api(`/products/${p.id}`, { method: 'DELETE' });
      load();
    } catch (err: any) {
      setError(err?.message);
    }
  };

  return (
    <div>
      <PageHeader
        title="Produtos"
        subtitle="Catálogo de produtos e serviços da sua empresa"
        action={<button className={btnPrimary} onClick={openCreate}>+ Novo produto</button>}
      />
      <ErrorText error={error} />

      <div className="mb-4 flex justify-end">
        <input
          className={`${inputClass} max-w-60`}
          placeholder="Buscar por nome..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && load()}
        />
      </div>

      <div className={`${cardClass} overflow-hidden`}>
        {loading ? (
          <EmptyState message="Carregando..." />
        ) : products.length === 0 ? (
          <EmptyState message="Nenhum produto encontrado." />
        ) : (
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-slate-200 bg-slate-50 text-left text-xs uppercase tracking-wide text-slate-500">
                <th className="px-5 py-3 font-medium">Código</th>
                <th className="px-5 py-3 font-medium">Produto</th>
                <th className="px-5 py-3 font-medium">Un.</th>
                <th className="px-5 py-3 text-right font-medium">Custo</th>
                <th className="px-5 py-3 text-right font-medium">Venda</th>
                <th className="px-5 py-3 text-right font-medium">Estoque mín.</th>
                <th className="px-5 py-3 font-medium">Ativo</th>
                <th className="px-5 py-3" />
              </tr>
            </thead>
            <tbody>
              {products.map((p) => (
                <tr key={p.id} className="border-b border-slate-50 last:border-0 hover:bg-slate-50/50">
                  <td className="px-5 py-3 font-medium text-slate-600">{p.code}</td>
                  <td className="px-5 py-3 text-slate-800">{p.name}</td>
                  <td className="px-5 py-3 text-slate-500">{p.unitOfMeasure}</td>
                  <td className="px-5 py-3 text-right text-slate-600">{formatCurrency(p.costPrice)}</td>
                  <td className="px-5 py-3 text-right font-medium text-slate-800">{formatCurrency(p.salePrice)}</td>
                  <td className="px-5 py-3 text-right text-slate-600">{p.minStock ?? '—'}</td>
                  <td className="px-5 py-3">
                    <Badge tone={p.isActive ? 'green' : 'gray'}>{p.isActive ? 'Sim' : 'Não'}</Badge>
                  </td>
                  <td className="px-5 py-3">
                    <div className="flex justify-end gap-2">
                      <button className={btnGhost} onClick={() => openEdit(p)}>Editar</button>
                      {p.isActive && <button className={btnDanger} onClick={() => handleDeactivate(p)}>Desativar</button>}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <Modal open={modalOpen} title={form.id ? 'Editar produto' : 'Novo produto'} onClose={() => setModalOpen(false)}>
        <form onSubmit={handleSave} className="space-y-4">
          <div className="grid grid-cols-2 gap-3">
            <Field label="Nome *">
              <input className={inputClass} value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
            </Field>
            <Field label="Código (vazio = automático)">
              <input className={inputClass} value={form.code} onChange={(e) => setForm({ ...form, code: e.target.value })} />
            </Field>
            <Field label="Unidade">
              <input className={inputClass} value={form.unitOfMeasure} onChange={(e) => setForm({ ...form, unitOfMeasure: e.target.value })} />
            </Field>
            <Field label="Estoque mínimo">
              <input type="number" min="0" className={inputClass} value={form.minStock} onChange={(e) => setForm({ ...form, minStock: e.target.value })} />
            </Field>
            <Field label="Preço de custo">
              <input type="number" step="0.01" min="0" className={inputClass} value={form.costPrice} onChange={(e) => setForm({ ...form, costPrice: e.target.value })} />
            </Field>
            <Field label="Preço de venda">
              <input type="number" step="0.01" min="0" className={inputClass} value={form.salePrice} onChange={(e) => setForm({ ...form, salePrice: e.target.value })} />
            </Field>
            <div className="col-span-2">
              <Field label="Descrição">
                <textarea className={inputClass} rows={2} value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
              </Field>
            </div>
          </div>
          <label className="flex items-center gap-2 text-sm text-slate-600">
            <input type="checkbox" checked={form.manageStock} onChange={(e) => setForm({ ...form, manageStock: e.target.checked })} />
            Controlar estoque deste produto
          </label>
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
