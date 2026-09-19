import { FormEvent, useEffect, useState } from 'react';
import { api, request } from '../lib/api';
import { useAuth } from '../lib/auth';
import { formatDateTime, MOVEMENT_TYPES } from '../lib/format';
import { Badge, btnGhost, btnPrimary, cardClass, EmptyState, ErrorText, Field, inputClass, Modal, PageHeader } from '../components/ui';

interface InventoryRow {
  id: string;
  productCode: string;
  productName: string;
  branchName: string;
  quantity: number;
  availableQuantity: number;
  minStock?: number;
  location?: string;
}

interface Movement {
  id: string;
  productCode?: string;
  productName?: string;
  branchName?: string;
  movementType: number;
  quantity: number;
  newQuantity?: number;
  description?: string;
  createdAt: string;
}

interface ProductOption {
  id: string;
  code: string;
  name: string;
}

export default function Inventory() {
  const { user } = useAuth();
  const hasTenant = !!user?.tenantId;
  const [rows, setRows] = useState<InventoryRow[]>([]);
  const [movements, setMovements] = useState<Movement[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [products, setProducts] = useState<ProductOption[]>([]);
  const [form, setForm] = useState({ productId: '', type: 'in', quantity: '', description: '' });

  const load = () => {
    setLoading(true);
    Promise.all([
      request('/inventory?pageNumber=1&pageSize=100'),
      request('/inventory/movements?pageNumber=1&pageSize=15')
    ])
      .then(([inv, mov]: any[]) => {
        setRows(inv.data?.data ?? []);
        setMovements(mov.data?.data ?? []);
      })
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

  const openAdjust = async () => {
    setFormError(null);
    setForm({ productId: '', type: 'in', quantity: '', description: '' });
    setModalOpen(true);
    try {
      const d: any = await request('/products?pageNumber=1&pageSize=200');
      setProducts((d.data?.data ?? []).map((p: any) => ({ id: p.id, code: p.code, name: p.name })));
    } catch {
      // ignora
    }
  };

  const handleAdjust = async (e: FormEvent) => {
    e.preventDefault();
    setFormError(null);
    setSaving(true);
    try {
      await api('/inventory/adjust', {
        method: 'POST',
        body: JSON.stringify({
          productId: form.productId,
          type: form.type,
          quantity: parseFloat(form.quantity),
          description: form.description || null
        })
      });
      setModalOpen(false);
      load();
    } catch (err: any) {
      setFormError(err?.message || 'Falha ao ajustar estoque');
    } finally {
      setSaving(false);
    }
  };

  const isLowStock = (r: InventoryRow) =>
    r.minStock != null && r.availableQuantity < r.minStock;

  return (
    <div>
      <PageHeader
        title="Estoque"
        subtitle="Saldos e movimentações de estoque por filial"
        action={<button className={btnPrimary} onClick={openAdjust}>+ Ajuste manual</button>}
      />
      <ErrorText error={error} />

      <div className={`${cardClass} overflow-hidden`}>
        <div className="border-b border-slate-200 px-5 py-3 text-sm font-semibold text-slate-700">Saldos</div>
        {loading ? (
          <EmptyState message="Carregando..." />
        ) : rows.length === 0 ? (
          <EmptyState message="Nenhum registro de estoque. Lance entradas via Compras ou Ajuste manual." />
        ) : (
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-slate-100 bg-slate-50 text-left text-xs uppercase tracking-wide text-slate-500">
                <th className="px-5 py-3 font-medium">Código</th>
                <th className="px-5 py-3 font-medium">Produto</th>
                <th className="px-5 py-3 font-medium">Filial</th>
                <th className="px-5 py-3 font-medium">Local</th>
                <th className="px-5 py-3 text-right font-medium">Qtd.</th>
                <th className="px-5 py-3 text-right font-medium">Disponível</th>
                <th className="px-5 py-3 font-medium">Situação</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((r) => (
                <tr key={r.id} className="border-b border-slate-50 last:border-0 hover:bg-slate-50/50">
                  <td className="px-5 py-3 font-medium text-slate-600">{r.productCode}</td>
                  <td className="px-5 py-3 text-slate-800">{r.productName}</td>
                  <td className="px-5 py-3 text-slate-600">{r.branchName}</td>
                  <td className="px-5 py-3 text-slate-500">{r.location || '—'}</td>
                  <td className="px-5 py-3 text-right text-slate-800">{r.quantity}</td>
                  <td className="px-5 py-3 text-right font-medium text-slate-800">{r.availableQuantity}</td>
                  <td className="px-5 py-3">
                    <Badge tone={isLowStock(r) ? 'red' : 'green'}>
                      {isLowStock(r) ? 'Abaixo do mínimo' : 'OK'}
                    </Badge>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <div className={`${cardClass} mt-6 overflow-hidden`}>
        <div className="border-b border-slate-200 px-5 py-3 text-sm font-semibold text-slate-700">Últimas movimentações</div>
        {movements.length === 0 ? (
          <EmptyState message="Nenhuma movimentação registrada." />
        ) : (
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-slate-100 bg-slate-50 text-left text-xs uppercase tracking-wide text-slate-500">
                <th className="px-5 py-3 font-medium">Data</th>
                <th className="px-5 py-3 font-medium">Produto</th>
                <th className="px-5 py-3 font-medium">Tipo</th>
                <th className="px-5 py-3 text-right font-medium">Qtd.</th>
                <th className="px-5 py-3 text-right font-medium">Saldo</th>
                <th className="px-5 py-3 font-medium">Descrição</th>
              </tr>
            </thead>
            <tbody>
              {movements.map((m) => (
                <tr key={m.id} className="border-b border-slate-50 last:border-0">
                  <td className="px-5 py-2.5 text-slate-500">{formatDateTime(m.createdAt)}</td>
                  <td className="px-5 py-2.5 text-slate-800">{m.productName}</td>
                  <td className="px-5 py-2.5">
                    <Badge tone={m.movementType % 2 === 0 ? 'green' : 'yellow'}>
                      {MOVEMENT_TYPES[m.movementType] ?? m.movementType}
                    </Badge>
                  </td>
                  <td className="px-5 py-2.5 text-right text-slate-700">{m.quantity}</td>
                  <td className="px-5 py-2.5 text-right font-medium text-slate-800">{m.newQuantity ?? '—'}</td>
                  <td className="px-5 py-2.5 text-slate-500">{m.description || '—'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <Modal open={modalOpen} title="Ajuste manual de estoque" onClose={() => setModalOpen(false)}>
        <form onSubmit={handleAdjust} className="space-y-4">
          <Field label="Produto *">
            <select
              className={inputClass}
              value={form.productId}
              onChange={(e) => setForm({ ...form, productId: e.target.value })}
              required
            >
              <option value="">Selecione um produto...</option>
              {products.map((p) => (
                <option key={p.id} value={p.id}>{p.code} — {p.name}</option>
              ))}
            </select>
          </Field>
          <div className="grid grid-cols-2 gap-3">
            <Field label="Tipo *">
              <select className={inputClass} value={form.type} onChange={(e) => setForm({ ...form, type: e.target.value })}>
                <option value="in">Entrada</option>
                <option value="out">Saída</option>
              </select>
            </Field>
            <Field label="Quantidade *">
              <input
                type="number"
                step="0.01"
                min="0.01"
                className={inputClass}
                value={form.quantity}
                onChange={(e) => setForm({ ...form, quantity: e.target.value })}
                required
              />
            </Field>
          </div>
          <Field label="Descrição">
            <input className={inputClass} value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} placeholder="Ex.: Ajuste de inventário" />
          </Field>
          <ErrorText error={formError} />
          <div className="flex justify-end gap-2">
            <button type="button" className={btnGhost} onClick={() => setModalOpen(false)}>Cancelar</button>
            <button type="submit" className={btnPrimary} disabled={saving}>
              {saving ? 'Aplicando...' : 'Aplicar'}
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
