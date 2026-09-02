import { useState } from "react";
import { ACOES_RBAC, AUTH_PASSOS, ESCOPOS_RBAC, MODULOS_RBAC, PERFIS_RBAC } from "../data";
import { Check, Reveal, SectionHead } from "../ui";

function AuthTimeline() {
  return (
    <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
      {AUTH_PASSOS.map((p, i) => (
        <Reveal key={p.t} delay={i * 70}>
          <div className="group flex h-full flex-col border border-steel-400/20 bg-ink-900/60 p-5 transition-all hover:-translate-y-1 hover:border-brass-500/60">
            <div className="flex items-center gap-3">
              <span className="flex h-8 w-8 items-center justify-center bg-ink-800 font-display text-[15px] font-bold text-brass-400 transition-colors group-hover:bg-brass-500 group-hover:text-ink-950">
                {i + 1}
              </span>
              <p className="font-display text-[15px] font-bold leading-tight text-paper">{p.t}</p>
            </div>
            <p className="mt-3 flex-1 text-[12.5px] leading-relaxed text-steel-300">{p.d}</p>
            <div className="mt-4 flex flex-wrap gap-1.5">
              {p.chips.map((ch) => (
                <span key={ch} className="border border-jade-500/40 px-2 py-0.5 font-mono text-[9.5px] uppercase tracking-wider text-jade-300">
                  {ch}
                </span>
              ))}
            </div>
          </div>
        </Reveal>
      ))}
    </div>
  );
}

function RbacMatrix() {
  const [perfilIdx, setPerfilIdx] = useState(0);
  const [perms, setPerms] = useState<Set<string>>(() => new Set(buildPerms(0)));

  function buildPerms(idx: number): string[] {
    const p = PERFIS_RBAC[idx];
    if (p.perms === "TODAS") return MODULOS_RBAC.flatMap((m) => ACOES_RBAC.map((a) => `${m}.${a}`));
    return p.perms;
  }

  const pick = (idx: number) => {
    setPerfilIdx(idx);
    setPerms(new Set(buildPerms(idx)));
  };

  const toggle = (key: string) => {
    setPerms((s) => {
      const n = new Set(s);
      if (n.has(key)) n.delete(key);
      else n.add(key);
      return n;
    });
  };

  const total = MODULOS_RBAC.length * ACOES_RBAC.length;

  return (
    <div>
      {/* perfis */}
      <Reveal>
        <div className="flex flex-wrap gap-2">
          {PERFIS_RBAC.map((p, i) => (
            <button
              key={p.nome}
              onClick={() => pick(i)}
              className={`border px-4 py-2.5 text-left transition-all ${
                perfilIdx === i
                  ? "border-brass-500 bg-brass-500 text-ink-950 shadow-[4px_4px_0_rgba(22,166,153,0.7)]"
                  : "border-steel-400/30 bg-ink-900/50 text-steel-300 hover:border-steel-300 hover:text-paper"
              }`}
            >
              <span className="block font-display text-[13.5px] font-bold">{p.nome}</span>
              <span className={`block font-mono text-[9.5px] uppercase tracking-wider ${perfilIdx === i ? "text-ink-800" : "text-steel-500"}`}>
                {p.perms === "TODAS" ? `${total}/${total} permissões` : `${p.perms.length}/${total} permissões`}
              </span>
            </button>
          ))}
        </div>
        <p className="mt-3 max-w-2xl text-[12.5px] leading-relaxed text-steel-400">
          {PERFIS_RBAC[perfilIdx].desc} <span className="text-brass-400">▸</span> edite as células: a matriz vira tela de
          configuração no ERP, nunca código.
        </p>
      </Reveal>

      {/* matrix */}
      <Reveal delay={120}>
        <div className="mt-5 overflow-x-auto border border-steel-400/20 bg-ink-900/60">
          <table className="w-full min-w-[640px] border-collapse">
            <thead>
              <tr className="border-b border-steel-400/20">
                <th className="px-4 py-3 text-left font-mono text-[10px] uppercase tracking-[0.2em] text-steel-400">
                  módulo.acao
                </th>
                {ACOES_RBAC.map((a) => (
                  <th key={a} className="px-2 py-3 text-center font-mono text-[10px] uppercase tracking-[0.18em] text-steel-400">
                    {a}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {MODULOS_RBAC.map((m) => (
                <tr key={m} className="border-b border-steel-400/10 last:border-b-0">
                  <td className="whitespace-nowrap px-4 py-1.5 font-display text-[13px] font-bold text-paper">{m}</td>
                  {ACOES_RBAC.map((a) => {
                    const key = `${m}.${a}`;
                    const on = perms.has(key);
                    return (
                      <td key={key} className="px-2 py-1.5 text-center">
                        <button
                          onClick={() => toggle(key)}
                          aria-label={`${key}: ${on ? "concedida" : "negada"}`}
                          className={`mx-auto flex h-7 w-7 items-center justify-center border transition-all ${
                            on
                              ? "border-moss-500 bg-moss-500/20 text-moss-500 hover:bg-moss-500/35"
                              : "border-steel-400/25 text-steel-600 hover:border-steel-400/60 hover:text-steel-400"
                          }`}
                        >
                          <Check on={on} size={12} />
                        </button>
                      </td>
                    );
                  })}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </Reveal>

      {/* escopos */}
      <Reveal delay={180}>
        <div className="mt-6 grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
          {ESCOPOS_RBAC.map((e) => (
            <div key={e.t} className="border border-steel-400/20 bg-ink-900/50 p-4 transition-colors hover:border-jade-500/50">
              <p className="font-display text-[13.5px] font-bold text-jade-300">{e.t}</p>
              <p className="mt-1.5 text-[12px] leading-relaxed text-steel-400">{e.d}</p>
            </div>
          ))}
        </div>
      </Reveal>
    </div>
  );
}

export default function Security() {
  return (
    <section className="relative bg-ink-950 py-20 text-paper lg:py-28">
      <div className="bp-dots-dark pointer-events-none absolute inset-0 opacity-30" aria-hidden />

      {/* AUTH */}
      <div id="auth" className="relative mx-auto max-w-7xl scroll-mt-32 px-4 sm:px-6">
        <SectionHead
          num="05"
          kicker="Autenticação · requisito 17 e 24"
          title="Tokens curtos, refresh que denuncia roubo e 2FA no caminho."
          lead="A identidade é a chave do cofre multi-tenant: é o claim tenant_id assinado dentro do JWT que alimenta o ADR-001 em cada request. Tudo abaixo é validado no backend — o frontend jamais decide."
          tone="dark"
        />
        <AuthTimeline />
      </div>

      {/* RBAC */}
      <div id="rbac" className="relative mx-auto mt-24 max-w-7xl scroll-mt-32 px-4 sm:px-6 lg:mt-32">
        <SectionHead
          num="06"
          kicker="Autorização · RBAC granular · requisito 18"
          title="Usuário → perfil → permissões, com escopo de filial por cima."
          lead="Permissões no formato módulo.ação, herdadas por perfis e restringidas por empresa, filial, módulo contratado e operação. Quatro perfis de fábrica — e a matriz abaixo é interativa: monte o seu."
          tone="dark"
        />
        <RbacMatrix />
      </div>
    </section>
  );
}
