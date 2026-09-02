import { FLUXO_TENANT, OPCOES_TENANCY, REGRAS_ISOLAMENTO } from "../data";
import { Check, Reveal, SectionHead, XMark } from "../ui";

export default function Tenancy() {
  return (
    <section id="tenancy" className="relative scroll-mt-28 bg-ink-950 py-20 text-paper lg:py-28">
      <div className="bp-grid-dark pointer-events-none absolute inset-0" aria-hidden />
      <div className="relative mx-auto max-w-7xl px-4 sm:px-6">
        <SectionHead
          num="03"
          kicker="Multi-tenancy · o coração do projeto"
          title="Database-per-tenant. O dado do vizinho não existe no meu banco."
          lead="O requisito nº 2 é claro: isolamento pela arquitetura, não por filtros de código. Avaliamos as três estratégias clássicas e apenas uma sobrevive ao teste de um vazamento catastrófico — porque nela, um bug de query não mistura empresas: simplesmente não há o que misturar."
          tone="dark"
        />

        {/* comparison */}
        <div className="grid gap-4 lg:grid-cols-3">
          {OPCOES_TENANCY.map((op, i) => (
            <Reveal key={op.nome} delay={i * 100}>
              <div
                className={`relative flex h-full flex-col border p-5 transition-all hover:-translate-y-1 ${
                  op.escolhido
                    ? "border-brass-500 bg-ink-900 shadow-[0_0_0_1px_rgba(242,165,22,0.4),0_18px_50px_rgba(0,0,0,0.5)]"
                    : "border-steel-400/20 bg-ink-900/50 hover:border-steel-400/45"
                }`}
              >
                {op.escolhido && (
                  <span className="absolute -top-3 right-4 bg-brass-500 px-2.5 py-1 font-mono text-[9.5px] font-bold tracking-[0.18em] text-ink-950">
                    ESCOLHIDA
                  </span>
                )}
                <p className="font-mono text-[10px] uppercase tracking-[0.22em] text-steel-400">Opção {i + 1}</p>
                <h3 className={`mt-2 font-display text-xl font-bold ${op.escolhido ? "text-brass-400" : "text-paper"}`}>{op.nome}</h3>
                <p className="mt-2 text-[12.5px] leading-relaxed text-steel-300">{op.como}</p>

                <div className="mt-4 space-y-1.5">
                  {op.pros.map((p) => (
                    <p key={p} className="flex items-start gap-2 text-[12px] leading-snug text-jade-300">
                      <span className="mt-0.5 shrink-0"><Check size={12} /></span> {p}
                    </p>
                  ))}
                  {op.contras.map((p) => (
                    <p key={p} className={`flex items-start gap-2 text-[12px] leading-snug ${op.escolhido ? "text-brass-400/90" : "text-ember-500/90"}`}>
                      <span className="mt-0.5 shrink-0"><XMark size={12} /></span> {p}
                    </p>
                  ))}
                </div>
              </div>
            </Reveal>
          ))}
        </div>

        {/* resolution flow */}
        <div className="mt-16 grid gap-8 lg:grid-cols-[1fr_360px]">
          <div>
            <Reveal>
              <h3 className="font-display text-2xl font-bold">Como um request encontra o banco certo</h3>
              <p className="mt-2 max-w-2xl text-[13.5px] leading-relaxed text-steel-300">
                Seis passos, todos server-side. Em nenhum momento o frontend participa da decisão — ele nem fica sabendo
                qual é o nome do banco.
              </p>
            </Reveal>
            <ol className="mt-7 space-y-0">
              {FLUXO_TENANT.map((f, i) => (
                <Reveal key={f.t} delay={i * 80}>
                  <li className="group relative flex gap-4 border-l border-steel-400/25 pb-7 pl-6 last:pb-0">
                    <span className="absolute -left-[13px] top-0 flex h-[26px] w-[26px] items-center justify-center border border-steel-400/40 bg-ink-950 font-mono text-[11px] font-semibold text-brass-400 transition-colors group-hover:border-brass-500">
                      {i + 1}
                    </span>
                    <div>
                      <p className="font-display text-[15px] font-bold text-paper">{f.t}</p>
                      <p className="mt-1 text-[13px] leading-relaxed text-steel-400">{f.d}</p>
                    </div>
                  </li>
                </Reveal>
              ))}
            </ol>
          </div>

          <div className="lg:sticky lg:top-32 lg:self-start">
            <Reveal delay={150}>
              <div className="border border-ember-500/40 bg-ink-900 p-5">
                <p className="font-mono text-[10px] uppercase tracking-[0.22em] text-ember-500">Regras inegociáveis</p>
                <ul className="mt-4 space-y-3">
                  {REGRAS_ISOLAMENTO.map((r) => (
                    <li key={r} className="flex items-start gap-2.5 text-[12.5px] leading-snug text-steel-300">
                      <span className="mt-[3px] h-1.5 w-1.5 shrink-0 bg-brass-500" />
                      {r}
                    </li>
                  ))}
                </ul>
              </div>
            </Reveal>
            <Reveal delay={240}>
              <div className="mt-4 border border-steel-400/20 bg-ink-900/60 p-5">
                <p className="font-mono text-[10px] uppercase tracking-[0.22em] text-steel-400">Nomenclatura</p>
                <div className="mt-3 space-y-2 font-mono text-[11.5px] leading-relaxed">
                  <p className="text-jade-300">ERP_MASTER <span className="text-steel-500">→ plataforma</span></p>
                  <p className="text-paper">ERP_EMPRESA_000001 <span className="text-steel-500">→ Empresa A</span></p>
                  <p className="text-paper">ERP_EMPRESA_000002 <span className="text-steel-500">→ Empresa B</span></p>
                  <p className="text-paper">ERP_EMPRESA_00000N <span className="text-steel-500">→ Empresa N</span></p>
                </div>
                <p className="mt-4 text-[11.5px] leading-relaxed text-steel-400">
                  Um usuário da Empresa A que tente ler dados da Empresa B recebe <span className="font-mono text-ember-500">404</span> —
                  não por filtro, mas porque o registro <em>fisicamente não existe</em> no banco dele.
                </p>
              </div>
            </Reveal>
          </div>
        </div>
      </div>
    </section>
  );
}
