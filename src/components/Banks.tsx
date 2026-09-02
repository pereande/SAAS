import { useMemo, useState } from "react";
import { MASTER_TABELAS, TENANT_GRUPOS, type Tabela } from "../data";
import { KeyBadge, Reveal, SectionHead } from "../ui";

type Mode = "master" | "tenant";

const TENANT_FLAT: { grupo: string; t: Tabela }[] = TENANT_GRUPOS.flatMap((g) =>
  g.tables.map((t) => ({ grupo: g.grupo, t }))
);

export default function Banks() {
  const [mode, setMode] = useState<Mode>("master");
  const [selMaster, setSelMaster] = useState("tenant_databases");
  const [selTenant, setSelTenant] = useState("auditoria");

  const tables = useMemo(() => (mode === "master" ? MASTER_TABELAS : TENANT_FLAT.map((x) => x.t)), [mode]);
  const selName = mode === "master" ? selMaster : selTenant;
  const sel = tables.find((t) => t.nome === selName) ?? tables[0];
  const selGrupo = mode === "tenant" ? TENANT_FLAT.find((x) => x.t.nome === sel?.nome)?.grupo : undefined;

  const setSel = (nome: string) => (mode === "master" ? setSelMaster(nome) : setSelTenant(nome));

  return (
    <section id="bancos" className="relative scroll-mt-28 bg-paper py-20 lg:py-28">
      <div className="bp-grid-light pointer-events-none absolute inset-0 [mask-image:linear-gradient(to_bottom,transparent,black_30%,black_70%,transparent)]" aria-hidden />
      <div className="relative mx-auto max-w-7xl px-4 sm:px-6">
        <SectionHead
          num="04"
          kicker="Modelo de bancos · requisitos 4, 5, 6 e 7"
          title="ERP_MASTER guarda o mapa. ERP_EMPRESA_* guarda o negócio."
          lead="O banco master conhece cada empresa, seu plano, seus módulos contratados e — principalmente — em qual banco físico ela vive. Tudo o que é operação (cadastros, estoque, vendas, financeiro, fiscal, auditoria) existe apenas dentro do banco da própria empresa. Explore os schemas iniciais abaixo; são a fundação da fase 1 e 2."
        />

        {/* db switcher */}
        <Reveal>
          <div className="flex flex-wrap items-center gap-2">
            <button
              onClick={() => setMode("master")}
              className={`flex items-center gap-2 border px-4 py-2.5 font-mono text-[12px] font-semibold transition-all ${
                mode === "master"
                  ? "border-ink-900 bg-ink-900 text-brass-400 shadow-[4px_4px_0_rgba(242,165,22,0.85)]"
                  : "border-ink-900/25 bg-white text-ink-700 hover:border-ink-900/60"
              }`}
            >
              <span className={`h-2 w-2 ${mode === "master" ? "bg-brass-500" : "bg-jade-500"}`} />
              ERP_MASTER
              <span className="text-[10px] opacity-60">· 9 tabelas</span>
            </button>
            <button
              onClick={() => setMode("tenant")}
              className={`flex items-center gap-2 border px-4 py-2.5 font-mono text-[12px] font-semibold transition-all ${
                mode === "tenant"
                  ? "border-ink-900 bg-ink-900 text-brass-400 shadow-[4px_4px_0_rgba(242,165,22,0.85)]"
                  : "border-ink-900/25 bg-white text-ink-700 hover:border-ink-900/60"
              }`}
            >
              <span className={`h-2 w-2 ${mode === "tenant" ? "bg-brass-500" : "bg-jade-500"}`} />
              ERP_EMPRESA_000001
              <span className="text-[10px] opacity-60">· {TENANT_FLAT.length} tabelas</span>
            </button>
            <span
              className="flex cursor-not-allowed items-center gap-2 border border-ink-900/10 bg-paper-2 px-4 py-2.5 font-mono text-[12px] text-steel-500"
              title="Você não está autenticado neste tenant"
            >
              <LockIcon /> ERP_EMPRESA_000002 · bloqueado
            </span>
          </div>
        </Reveal>

        <Reveal delay={100}>
          <div className="mt-5 grid gap-0 overflow-hidden border border-ink-900/15 bg-white lg:grid-cols-[300px_1fr]">
            {/* list */}
            <div className="term-scroll max-h-[540px] overflow-y-auto border-b border-ink-900/15 lg:max-h-none lg:border-b-0 lg:border-r">
              {mode === "master" ? (
                <ul>
                  {MASTER_TABELAS.map((t) => (
                    <li key={t.nome}>
                      <button
                        onClick={() => setSel(t.nome)}
                        className={`flex w-full items-center justify-between border-l-2 px-4 py-2.5 text-left font-mono text-[12px] transition-all ${
                          selName === t.nome
                            ? "border-brass-500 bg-brass-100/50 text-ink-900"
                            : "border-transparent text-steel-600 hover:bg-paper hover:text-ink-900"
                        }`}
                      >
                        {t.nome}
                        <span className="text-[9.5px] text-steel-500">{t.cols.length} col</span>
                      </button>
                    </li>
                  ))}
                </ul>
              ) : (
                TENANT_GRUPOS.map((g) => (
                  <div key={g.grupo}>
                    <p className="sticky top-0 z-10 border-b border-ink-900/10 bg-ink-900 px-4 py-2 font-mono text-[9.5px] font-semibold uppercase tracking-[0.2em] text-brass-400">
                      {g.grupo}
                    </p>
                    <ul>
                      {g.tables.map((t) => (
                        <li key={t.nome}>
                          <button
                            onClick={() => setSel(t.nome)}
                            className={`flex w-full items-center justify-between border-l-2 px-4 py-2.5 text-left font-mono text-[12px] transition-all ${
                              selName === t.nome
                                ? "border-brass-500 bg-brass-100/50 text-ink-900"
                                : "border-transparent text-steel-600 hover:bg-paper hover:text-ink-900"
                            }`}
                          >
                            {t.nome}
                            <span className="text-[9.5px] text-steel-500">{t.cols.length} col</span>
                          </button>
                        </li>
                      ))}
                    </ul>
                  </div>
                ))
              )}
            </div>

            {/* detail */}
            <div key={`${mode}-${sel?.nome}`} className="animate-rowin p-5 sm:p-7">
              <div className="flex flex-wrap items-center gap-3">
                <h3 className="font-mono text-xl font-semibold text-ink-900">{sel?.nome}</h3>
                {selGrupo && (
                  <span className="border border-jade-500/50 px-2 py-0.5 font-mono text-[10px] uppercase tracking-widest text-jade-600">
                    {selGrupo}
                  </span>
                )}
                {mode === "master" && (
                  <span className="border border-brass-500/60 px-2 py-0.5 font-mono text-[10px] uppercase tracking-widest text-brass-600">
                    banco da plataforma
                  </span>
                )}
              </div>
              <p className="mt-1.5 text-[13px] text-steel-600">{sel?.desc}</p>

              <ul className="mt-5 divide-y divide-ink-900/8 border-y border-ink-900/10">
                {sel?.cols.map((col) => (
                  <li key={col.n} className="flex items-center gap-3 py-2">
                    {col.k ? <KeyBadge k={col.k} /> : <span className="w-8 shrink-0" />}
                    <span className="font-mono text-[12.5px] font-semibold text-ink-900">{col.n}</span>
                    <span className="ml-auto text-right font-mono text-[11px] leading-snug text-steel-600">{col.t}</span>
                  </li>
                ))}
              </ul>

              <div className="mt-4 flex flex-wrap gap-x-5 gap-y-1.5">
                <span className="flex items-center gap-1.5 font-mono text-[10px] text-steel-600"><KeyBadge k="PK" /> chave primária</span>
                <span className="flex items-center gap-1.5 font-mono text-[10px] text-steel-600"><KeyBadge k="FK" /> chave estrangeira</span>
                <span className="flex items-center gap-1.5 font-mono text-[10px] text-steel-600"><KeyBadge k="UQ" /> unique constraint</span>
                <span className="flex items-center gap-1.5 font-mono text-[10px] text-steel-600"><KeyBadge k="IDX" /> índice</span>
              </div>
            </div>
          </div>
        </Reveal>

        <Reveal delay={160}>
          <div className="mt-6 grid gap-4 lg:grid-cols-3">
            {[
              { t: "Integridade primeiro", d: "PKs, FKs, uniques e constraints no banco — a última linha de defesa vive no schema, não no código. Soft delete só onde faz sentido (cadastros); trilhas fiscais e financeiras jamais." },
              { t: "Nada de dado calculável", d: "estoque_saldos.disponivel é gerado; comissão é apurada; margem é derivada. Armazenamos o evento, nunca o achismo." },
              { t: "Pronto para milhões", d: "Índices por tenant desde o dia 1 e particionamento previsto para estoque_movimentacoes, auditoria e documentos_fiscais quando o volume pedir." },
            ].map((x) => (
              <div key={x.t} className="border border-ink-900/15 bg-paper-2/70 p-5 transition-all hover:-translate-y-1 hover:border-brass-500/70">
                <p className="font-display text-[14px] font-bold text-ink-900">{x.t}</p>
                <p className="mt-2 text-[12.5px] leading-relaxed text-steel-600">{x.d}</p>
              </div>
            ))}
          </div>
        </Reveal>
      </div>
    </section>
  );
}

function LockIcon() {
  return (
    <svg width="12" height="12" viewBox="0 0 16 16" fill="none" aria-hidden>
      <rect x="3" y="7" width="10" height="7" stroke="currentColor" strokeWidth="1.6" />
      <path d="M5 7V5a3 3 0 016 0v2" stroke="currentColor" strokeWidth="1.6" />
    </svg>
  );
}
