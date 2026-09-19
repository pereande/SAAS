import { FormEvent, useState } from "react";
import { ArrowRight, Eye, EyeOff, LockKeyhole, Sparkles } from "lucide-react";
import { toast } from "sonner";
import { api, type SessionUser } from "@/lib/api";

type LoginProps = { onLogin: (user: SessionUser) => void };

export default function Login({ onLogin }: LoginProps) {
  const [username, setUsername] = useState("admin@erpsaas.com");
  const [password, setPassword] = useState("Admin@123");
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setLoading(true);
    try {
      const user = await api.login(username, password);
      toast.success("Login realizado com sucesso.");
      onLogin(user);
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Não foi possível entrar.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="login-page">
      <section className="login-story">
        <div className="story-grid" />
        <div className="login-brand">
          <div className="brand-mark">E</div>
          <span>ERP<span className="brand-dot">.</span>flow</span>
        </div>
        <div className="story-copy">
          <p className="eyebrow eyebrow--light"><Sparkles size={14} /> gestão com clareza</p>
          <h1>Seu negócio,<br /><em>em movimento.</em></h1>
          <p className="story-description">Uma visão mais simples para decisões mais inteligentes. Reúna vendas, estoque e financeiro em um único lugar.</p>
        </div>
        <div className="story-footer">
          <div className="story-proof"><span className="proof-avatars"><i>MC</i><i>RL</i><i>BS</i></span><span>+ 240 empresas crescendo juntas</span></div>
          <span className="story-version">v1.0 • ambiente seguro</span>
        </div>
      </section>

      <section className="login-panel">
        <div className="login-form-wrap">
          <div className="mobile-brand login-brand"><div className="brand-mark">E</div><span>ERP<span className="brand-dot">.</span>flow</span></div>
          <div className="login-heading">
            <p className="eyebrow">área restrita</p>
            <h2>Bom ter você aqui.</h2>
            <p>Entre para acompanhar o pulso da sua operação.</p>
          </div>
          <form className="login-form" onSubmit={handleSubmit}>
            <label htmlFor="username">E-mail corporativo</label>
            <div className="input-shell"><span className="input-prefix">@</span><input id="username" type="email" autoComplete="email" value={username} onChange={(event) => setUsername(event.target.value)} placeholder="voce@empresa.com" required /></div>
            <div className="password-label"><label htmlFor="password">Senha</label><button type="button" className="text-button" onClick={() => toast.info("Em breve: recuperação de senha.")}>Esqueci minha senha</button></div>
            <div className="input-shell"><LockKeyhole size={17} className="input-icon" /><input id="password" type={showPassword ? "text" : "password"} autoComplete="current-password" value={password} onChange={(event) => setPassword(event.target.value)} placeholder="••••••••" required /><button type="button" className="icon-button" aria-label={showPassword ? "Ocultar senha" : "Mostrar senha"} onClick={() => setShowPassword((value) => !value)}>{showPassword ? <EyeOff size={18} /> : <Eye size={18} />}</button></div>
            <button className="primary-button login-submit" type="submit" disabled={loading}>{loading ? "Entrando…" : "Entrar no ERP"}<ArrowRight size={18} /></button>
          </form>
          <p className="login-legal">Ao entrar, você concorda com os <button className="text-button" onClick={() => toast.info("Termos de uso em preparação.")}>termos de uso</button> e a <button className="text-button" onClick={() => toast.info("Política de privacidade em preparação.")}>política de privacidade</button>.</p>
        </div>
      </section>
    </main>
  );
}
