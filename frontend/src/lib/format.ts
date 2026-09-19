export const BRL = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' });

export function formatCurrency(value?: number | null): string {
  if (value == null) return '—';
  return BRL.format(value);
}

export function formatDateTime(value?: string | null): string {
  if (!value) return '—';
  const date = new Date(value);
  return date.toLocaleString('pt-BR', { dateStyle: 'short', timeStyle: 'short' });
}

export function formatDate(value?: string | null): string {
  if (!value) return '—';
  return new Date(value).toLocaleDateString('pt-BR');
}

export const SALE_STATUS = [
  'Rascunho',
  'Confirmada',
  'Em separação',
  'Separada',
  'Em transporte',
  'Entregue',
  'Faturada',
  'Cancelada'
];

export const PURCHASE_STATUS = [
  'Rascunho',
  'Confirmada',
  'Em aprovação',
  'Aprovada',
  'Em transporte',
  'Recebida',
  'Faturada',
  'Cancelada'
];

export const MOVEMENT_TYPES = [
  'Entrada',
  'Saída',
  'Ajuste (+)',
  'Ajuste (-)',
  'Transf. saída',
  'Transf. entrada',
  'Devol. venda',
  'Devol. compra'
];

export const TENANT_STATUS = ['Ativo', 'Inativo', 'Suspenso', 'Trial', 'Excluído'];

type BadgeTone = 'gray' | 'blue' | 'green' | 'yellow' | 'red' | 'purple';

export function statusTone(status: number, cancelledIndex: number): BadgeTone {
  if (status === cancelledIndex) return 'red';
  if (status === 0) return 'gray';
  if (status === 1) return 'blue';
  if (status === 5 || status === 6) return 'green';
  return 'yellow';
}
