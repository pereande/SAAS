import { FormEvent, useState } from 'react';
import { Navigate, useNavigate } from 'react-router-dom';
import { useAuth } from '../lib/auth';
import { btnPrimary, ErrorText, inputClass } from '../components/ui';

export default function Login() {
  const { login, user, loading } = useAuth();
  const navigate = useNavigate();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  if (loading) {
    return <div className="flex h-full items-center justify-center bg-slate-100 text-sm text-slate-500">Carregando...</div>;
  }
  if (user) return <Navigate to="/" replace />;

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      await login(username, password);
      navigate('/');
    } catch (err: any) {
      setError(err?.message || 'Falha no login');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="flex h-full items-center justify-center bg-gradient-to-br from-slate-900 via-slate-800 to-indigo-950 p-4">
      <div className="w-full max-w-md">
        <div className="mb-6 text-center">
          <div className="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-xl bg-indigo-600 text-xl font-bold text-white shadow-lg">
            E
          </div>
          <h1 className="text-2xl font-semibold text-white">ERP SaaS</h1>
          <p className="mt-1 text-sm text-slate-400">Sistema de Gestão Empresarial Multiempresa</p>
        </div>
        <form onSubmit={handleSubmit} className="rounded-2xl bg-white p-6 shadow-xl">
          <div className="space-y-4">
            <label className="block">
              <span className="mb-1 block text-xs font-medium text-slate-600">Usuário ou e-mail</span>
              <input
                className={inputClass}
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                placeholder="admin@erpsaas.com"
                autoFocus
              />
            </label>
            <label className="block">
              <span className="mb-1 block text-xs font-medium text-slate-600">Senha</span>
              <input
                type="password"
                className={inputClass}
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="••••••••"
              />
            </label>
            <ErrorText error={error} />
            <button type="submit" className={`${btnPrimary} w-full py-2.5`} disabled={submitting}>
              {submitting ? 'Entrando...' : 'Entrar'}
            </button>
          </div>
          <div className="mt-5 rounded-lg bg-slate-50 p-3 text-xs leading-5 text-slate-500">
            <div className="mb-0.5 font-medium text-slate-600">Contas de demonstração (senha: Admin@123)</div>
            <div>Platforma: admin@erpsaas.com</div>
            <div>Empresa demo: admin@demo.com.br</div>
          </div>
        </form>
      </div>
    </div>
  );
}
