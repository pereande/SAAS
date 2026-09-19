import { FormEvent, useEffect, useState } from 'react';
import { api, request } from '../lib/api';
import { useAuth } from '../lib/auth';
import { formatCurrency, formatDateTime, PURCHASE_STATUS, statusTone } from '../lib/format';
import { Badge, btnDanger, btnGhost, btnPrimary, btnSuccess, cardClass, EmptyState, ErrorText, Field, inputClass, Modal, PageHeader } from '../components/ui';

interface Purchase {
  id: string;
  number: string;
  purchaseDate: string;
  status: number;
  supplierId: string;
  total: number;
}

interface PurchaseItem {
  id: string;
  productCode?: string;
  productName?: string;
  quantity: number;
  unitPrice: number;
  total: number;
}

interface PurchaseDetails extends Purchase {
  subtotal: number;
  discount: number;
  items: PurchaseItem[];
}

interface ProductOption {
  id: string;
  code: string;
  name: string;
  costPrice?: number;
}

interface SupplierOption {
  id: string;
  name: string;
}

interface ItemForm {
  productId: string;
  quantity: string;
  unitPrice: string;
}

export default function Purchases() {
  const { user } = useAuth();
  const hasTenant = !!user?.tenantId;
  const [purchases, setPurchases] = useState<Purchase[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [products, setProducts] = useState<ProductOption[]>([]);
  const [suppliers, setSuppliers] = useState<SupplierOption[]>([]);
  const [detail, setDetail] = useState<PurchaseDetails | null>(null);
  const [form, setForm] = useState({ supplierId: '', discount: '', notes: '' });
  const [items, setItems] = useState<ItemForm[]>([{ productId: '', quantity: '1', unitPrice: '' }]);

  const load = () => {
    setLoading(true);
    request('/purchases?pageNumber=1&pageSize=50')
      .then((d: any) => setPurchases(d.data?.data ?? []))
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

  const openCreate = async () => {
    setFormError(null);
    setForm({ supplierId: '', discount: '', notes: '' });
    setItems([{ productId: '', quantity: '1', unitPrice: '' }]);
    setModalOpen(true);
    try {
      const [prods, sups]: any[] = await Promise.all([
        request('/products?pageNumber=1&pageSize=200&isActive=true'),
        request('/persons?type=supplier&pageNumber=1&pageSize=200')
      ]);
      setProducts(prods.data?.data ?? []);
      setSuppliers((sups.data?.data ?? []).map((s: any) => ({ id: s.id, name: s.name })));
    } catch {
      // ignora
    }
  };

  const setItem = (index: number, patch: Partial<ItemForm>) => {
    setItems((list) => list.map((it, i) => (i === index ? { ...it, ...patch } : it)));
  };

  const onProductChange = (index: number, productId: string) => {
    const product = products.find((p) => p.id === productId);
    setItem(index, { productId, unitPrice: product?.costPrice?.toString() || '' });
  };

  const handleCreate = async (e: FormEvent) => {
    e.preventDefault();
    setFormError(null);
    setSaving(true);
    try {
      await api('/purchases', {
        method: 'POST',
        body: JSON.stringify({
          supplierId: form.supplierId,
          discount: form.discount ? parseFloat(form.discount) : 0,
          notes: form.notes || null,
          items: items
            .filter((i) => i.productId)
            .map((i) => ({
              productId: i.productId,
              quantity: parseFloat(i.quantity),
              unitPrice: i.unitPrice ? parseFloat(i.unitPrice) : null
            }))
        })
      });
      setModalOpen(false);
      load();
    } catch (err: any) {
      setFormError(err?.message || 'Falha ao criar compra');
    } finally {
      setSaving(false);
    }
  };

  const doAction = async (purchase: Purchase, action: 'confirm' | 'receive' | 'cancel') => {
    if (action === 'cancel' && !window.confirm(`Cancelar a compra ${purchase.number}?`)) return;
    try {
      const body = action === 'cancel' ? JSON.stringify({ reason: 'Cancelado pelo usuário' }) : undefined;
      await api(`/purchases/${purchase.id}/${action}`, { method: 'POST', ...(body ? { body, headers: { 'Content-Type': 'application/json' } } : {}) });
      load();
    } catch (err: any) {
      setError(err?.message);
    }
  };

  const openDetail = async (id: string) => {
    try {
      const d = await api<PurchaseDetails>(`/purchases/${id}`);
      setDetail(d);
    } catch (err: any) {
      setError(err?.message);
    }
  };

  return (
    <div>
      <PageHeader
        title="Compras"
        subtitle="Pedidos de compra e recebimentos"
        action={<button className={btnPrimary} onClick={openCreate}>+ Nova compra</button>}
      />
      <ErrorText error={error} />

      <div className={`${cardClass} overflow-hidden`}>
        {loading ? (
          <EmptyState message="Carregando..." />
        ) : purchases.length === 0 ? (
          <EmptyState message="Nenhuma compra registrada." />
        ) : (
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-slate-200 bg-slate-50 text-left text-xs uppercase tracking-wide text-slate-500">
                <th className="px-5 py-3 font-medium">Número</th>
                <th className="px-5 py-3 font-medium">Data</th>
                <th className="px-5 py-3 font-medium">Status</th>
                <th className="px-5 py-3 text-right font-medium">Total</th>
                <th className="px-5 py-3" />
              </tr>
            </thead>
            <tbody>
              {purchases.map((p) => (
                <tr key={p.id} className="border-b border-slate-50 last:border-0 hover:bg-slate-50/50">
                  <td className="px-5 py-3 font-medium text-slate-700">
                    <button className="hover:text-indigo-600 hover:underline" onClick={() => openDetail(p.id)}>{p.number}</button>
                  </td>
                  <td className="px-5 py-3 text-slate-500">{formatDateTime(p.purchaseDate)}</td>
                  <td className="px-5 py-3">
                    <Badge tone={statusTone(p.status, 7)}>{PURCHASE_STATUS[p.status] ?? p.status}</Badge>
                  </td>
                  <td className="px-5 py-3 text-right font-medium text-slate-800">{formatCurrency(p.total)}</td>
                  <td className="px-5 py-3">
                    <div className="flex justify-end gap-2">
                      {p.status === 0 && (
                        <>
                          <button className={btnSuccess} onClick={() => doAction(p, 'confirm')}>Confirmar</button>
                          <button className={btnDanger} onClick={() => doAction(p, 'cancel')}>Cancelar</button>
                        </>
                      )}
                      {p.status === 1 && (
                        <>
                          <button className={btnSuccess} onClick={() => doAction(p, 'receive')}>Receber</button>
                          <button className={btnDanger} onClick={() => doAction(p, 'cancel')}>Cancelar</button>
                        </>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <Modal open={modalOpen} title="Nova compra" onClose={() => setModalOpen(false)} wide>
        <form onSubmit={handleCreate} className="space-y-4">
          <div className="grid grid-cols-3 gap-3">
            <Field label="Fornecedor *">
              <select className={inputClass} value={form.supplierId} onChange={(e) => setForm({ ...form, supplierId: e.target.value })} required>
                <option value="">Selecione...</option>
                {suppliers.map((s) => (
                  <option key={s.id} value={s.id}>{s.name}</option>
                ))}
              </select>
            </Field>
            <Field label="Desconto (R$)">
              <input type="number" step="0.01" min="0" className={inputClass} value={form.discount} onChange={(e) => setForm({ ...form, discount: e.target.value })} />
            </Field>
            <Field label="Observações">
              <input className={inputClass} value={form.notes} onChange={(e) => setForm({ ...form, notes: e.target.value })} />
            </Field>
          </div>

          <div>
            <div className="mb-1.5 flex items-center justify-between">
              <span className="text-xs font-medium text-slate-600">Itens</span>
              <button type="button" className="text-xs font-medium text-indigo-600 hover:underline" onClick={() => setItems([...items, { productId: '', quantity: '1', unitPrice: '' }])}>
                + Adicionar item
              </button>
            </div>
            <div className="space-y-2">
              {items.map((item, index) => (
                <div key={index} className="grid grid-cols-[1fr_90px_110px_32px] items-center gap-2">
                  <select className={inputClass} value={item.productId} onChange={(e) => onProductChange(index, e.target.value)}>
                    <option value="">Produto...</option>
                    {products.map((p) => (
                      <option key={p.id} value={p.id}>{p.code} — {p.name}</option>
                    ))}
                  </select>
                  <input
                    type="number" min="0.01" step="0.01" className={inputClass} placeholder="Qtd."
                    value={item.quantity} onChange={(e) => setItem(index, { quantity: e.target.value })}
                  />
                  <input
                    type="number" min="0" step="0.01" className={inputClass} placeholder="Custo"
                    value={item.unitPrice} onChange={(e) => setItem(index, { unitPrice: e.target.value })}
                  />
                  <button
                    type="button"
                    className="rounded-lg p-1.5 text-slate-400 hover:bg-rose-50 hover:text-rose-600"
                    onClick={() => setItems(items.filter((_, i) => i !== index))}
                    disabled={items.length === 1}
                    aria-label="Remover item"
                  >
                    ✕
                  </button>
                </div>
              ))}
            </div>
          </div>

          <ErrorText error={formError} />
          <div className="flex justify-end gap-2">
            <button type="button" className={btnGhost} onClick={() => setModalOpen(false)}>Cancelar</button>
            <button type="submit" className={btnPrimary} disabled={saving}>
              {saving ? 'Salvando...' : 'Criar compra'}
            </button>
          </div>
        </form>
      </Modal>

      <Modal open={!!detail} title={`Compra ${detail?.number ?? ''}`} onClose={() => setDetail(null)}>
        {detail && (
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <Badge tone={statusTone(detail.status, 7)}>{PURCHASE_STATUS[detail.status] ?? detail.status}</Badge>
              <span className="text-xs text-slate-500">{formatDateTime(detail.purchaseDate)}</span>
            </div>
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-slate-200 text-left text-xs uppercase text-slate-500">
                  <th className="py-2 font-medium">Produto</th>
                  <th className="py-2 text-right font-medium">Qtd.</th>
                  <th className="py-2 text-right font-medium">Unit.</th>
                  <th className="py-2 text-right font-medium">Total</th>
                </tr>
              </thead>
              <tbody>
                {detail.items.map((i) => (
                  <tr key={i.id} className="border-b border-slate-50 last:border-0">
                    <td className="py-2 text-slate-800">{i.productName}</td>
                    <td className="py-2 text-right text-slate-600">{i.quantity}</td>
                    <td className="py-2 text-right text-slate-600">{formatCurrency(i.unitPrice)}</td>
                    <td className="py-2 text-right font-medium text-slate-800">{formatCurrency(i.total)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            <div className="space-y-1 text-sm">
              <div className="flex justify-between text-slate-600"><span>Subtotal</span><span>{formatCurrency(detail.subtotal)}</span></div>
              <div className="flex justify-between text-slate-600"><span>Desconto</span><span>-{formatCurrency(detail.discount)}</span></div>
              <div className="flex justify-between text-base font-semibold text-slate-800"><span>Total</span><span>{formatCurrency(detail.total)}</span></div>
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
}
