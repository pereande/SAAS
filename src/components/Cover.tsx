import { useEffect, useRef, useState } from "react";
import { STATS, TICKER } from "../data";
import { CountUp, Reveal } from "../ui";

type Line = { txt: string; cls: string };

const SCENARIOS: { id: string; label: string; attack?: boolean; lines: Line[] }[] = [
  {
    id: "legit",
    label: "GET /api/v1/produtos · token ALFA",
    lines: [
      { txt: "$ request → TenantMiddleware · assinatura RS256 OK", cls: "text-steel-400" },
      { txt: "  resolve tenant 000001 · cache Redis · assinatura ATIVA", cls: "text-steel-400" },
      { txt: "  conexão → ERP_EMPRESA_000001 (allowlist OK)", cls: "text-jade-300" },
      { txt: "  200 OK · 128 produtos · 24 ms", cls: "text-moss-500" },
    ],
  },
  {
    id: "db-alheio",
    label: "Tentar abrir ERP_EMPRESA_000002 (ataque)",
    attack: true,
    lines: [
      { txt: "$ tentativa de conexão direta ao banco 000002", cls: "text-steel-400" },
      { txt: "  TenantGuard: credencial não pertence ao banco · allowlist falhou", cls: "text-ember-500" },
      { txt: "  403 TENANT_MISMATCH · conexão encerrada", cls: "text-ember-500" },
      { txt: "  auditoria → security.violation registrada no master", cls: "text-brass-400" },
    ],
  },
  {
    id: "jwt",
    label: "JWT com claim tenant adulterado (ataque)",
    attack: true,
    lines: [
      { txt: "$ request → claim tenant_id reescrito para 000002", cls: "text-steel-400" },
      { txt: "  verificação de assinatura RS256: ASSINATURA INVÁLIDA", cls: "text-ember-500" },
      { txt: "  401 INVALID_SIGNATURE · nenhum banco consultado", cls: "text-ember-500" },
      { txt: "  auditoria → security.violation registrada no master", cls: "text-brass-400" },
    ],
  },
  {
    id: "body",
    label: "POST com tenant_id no body (ataque)",
    attack: true,
    lines: [
      { txt: "$ POST /api/v1/vendas · body { \"tenant_id\": \"000002\" }", cls: "text-steel-400" },
      { txt: "  campo ignorado — contexto vem EXCLUSIVAMENTE do token", cls: "text-brass-400" },
      { txt: "  executado em ERP_EMPRESA_000001 (tenant autenticado)", cls: "text-jade-300" },
      { txt: "  200 OK · sem vazamento cross-tenant", cls: "text-moss-500" },
    ],
  },
];

export default function Cover() {
  const [lines, setLines] = useState<Line[]>([
    { txt: "$ matriz-tenant-sim v1.0 · sessão: Empresa ALFA (tenant 000001)", cls: "text-steel-400" },
    { txt: "  pronto. execute um cenário ao lado →", cls: "text-steel-500" },
  ]);
  const [running, setRunning] = useState(false);
  const [blocked, setBlocked] = useState(0);
  const timers = useRef<number[]>([]);
  const boxRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => () => timers.current.forEach(clearTimeout), []);
  useEffect(() => {
    const el = boxRef.current;
    if (el) el.scrollTop = el.scrollHeight;
  }, [lines]);

  const run = (id: string) => {
    if (running) return;
    const sc = SCENARIOS.find((s) => s.id === id);
    if (!sc) return;
    setRunning(true);
    setLines((l) => [...l, { txt: "", cls: "" }]);
    sc.lines.forEach((ln, i) => {
      timers.current.push(
        window.setTimeout(() => {
          setLines((l) => [...l, ln]);
          if (i === sc.lines.length - 1) {
            setRunning(false);
            if (sc.attack) setBlocked((b) => b + 1);
          }
        }, 240 * (i + 1))
      );
    });
  };

  return (
    <section id="capa" className="relative overflow-hidden bg-ink-950 text-paper">
      <div className="bp-grid-dark pointer-events-none absolute inset-0" aria-hidden />
      <div className="bp-dots-dark pointer-events-none absolute inset-x-0 top-0 h-64 opacity-40 [mask-image:linear-gradient(to_bottom,black,transparent)]" aria-hidden />

      <div className="relative mx-auto max-w-7xl px-4 pb-16 pt-32 sm:px-6 lg:pt-36">
        <div className="grid items-start gap-12 lg:grid-cols-[1.05fr_1fr] lg:gap-10">
          {/* left: manifesto */}
          <div>
            <Reveal>
              <p className="font-mono text-[11px] uppercase tracking-[0.3em] text-brass-400">
                Dossiê técnico · versão 1.0 · confidencial
              </p>
              <h1 className="mt-5 font-display font-bold leading-[0.95] tracking-tight">
                <span className="block text-[clamp(3.2rem,9vw,6.5rem)] text-paper">MATRIZ</span>
                <span className="mt-2 block text-[clamp(1.15rem,2.6vw,1.8rem)] font-semibold text-steel-300">
                  ERP SaaS multiempresa, <span className="text-brass-400">um banco por empresa.</span>
                </span>
              </h1>
            </Reveal>

            <Reveal delay={120}>
              <p className="mt-7 max-w-xl text-[15px] leading-relaxed text-steel-300">
                Antes de qualquer tela de cadastro, este blueprint responde às <strong className="text-paper">47 seções do
                escopo</strong> com 18 entregas de arquitetura: isolamento físico de dados, RBAC granular, saga
                transacional de venda, fiscal brasileiro desacoplado e um roadmap em 6 fases. Nada de CRUD — uma fundação
                para um produto comercial de longo prazo.
              </p>
            </Reveal>

            <Reveal delay={200}>
              <div className="mt-8 flex flex-wrap items-center gap-3">
                <a
                  href="#aprovacao"
                  className="group inline-flex items-center gap-2 bg-brass-500 px-5 py-3 font-display text-sm font-bold uppercase tracking-wider text-ink-950 transition-all hover:-translate-y-0.5 hover:bg-brass-400 hover:shadow-[0_10px_30px_rgba(242,165,22,0.35)]"
                >
                  Ir para a aprovação
                  <span className="transition-transform group-hover:translate-x-1">→</span>
                </a>
                <a
                  href="#tenancy"
                  className="inline-flex items-center gap-2 border border-steel-400/40 px-5 py-3 font-display text-sm font-semibold uppercase tracking-wider text-steel-300 transition-colors hover:border-brass-500 hover:text-brass-400"
                >
                  Ver estratégia de tenancy
                </a>
              </div>
            </Reveal>

            <Reveal delay={280}>
              <dl className="mt-10 grid grid-cols-2 gap-px border border-steel-400/15 bg-steel-400/15 sm:grid-cols-5">
                {STATS.map((s) => (
                  <div key={s.l} className="bg-ink-950 px-4 py-4">
                    <dt className="order-2 mt-1 block font-mono text-[9.5px] uppercase tracking-[0.14em] text-steel-500">
                      {s.l}
                    </dt>
                    <dd className="font-display text-2xl font-bold text-paper">
                      <CountUp value={s.v} />
                    </dd>
                  </div>
                ))}
              </dl>
            </Reveal>
          </div>

          {/* right: live isolation demo */}
          <Reveal delay={160}>
            <div className="corner-ticks border border-steel-400/25 bg-ink-900/80">
              <div className="flex items-center justify-between border-b border-steel-400/15 px-4 py-2.5">
                <div className="flex items-center gap-1.5">
                  <span className="h-2.5 w-2.5 rounded-full bg-ember-500/80" />
                  <span className="h-2.5 w-2.5 rounded-full bg-brass-500/80" />
                  <span className="h-2.5 w-2.5 rounded-full bg-moss-500/80" />
                </div>
                <span className="font-mono text-[10.5px] tracking-widest text-steel-400">
                  SIMULADOR DE ISOLAMENTO · REQUISITO Nº 2 & 38
                </span>
              </div>

              <div className="grid gap-0 sm:grid-cols-[200px_1fr]">
                <div className="border-b border-steel-400/15 p-3 sm:border-b-0 sm:border-r">
                  <p className="px-1 pb-2 font-mono text-[9.5px] uppercase tracking-[0.18em] text-steel-500">
                    Cenários de request
                  </p>
                  <div className="flex flex-col gap-1.5">
                    {SCENARIOS.map((s) => (
                      <button
                        key={s.id}
                        onClick={() => run(s.id)}
                        disabled={running}
                        className={`border px-2.5 py-2 text-left font-mono text-[10.5px] leading-snug transition-all disabled:opacity-50 ${
                          s.attack
                            ? "border-ember-500/40 text-ember-500 hover:bg-ember-500/10"
                            : "border-jade-500/40 text-jade-300 hover:bg-jade-500/10"
                        }`}
                      >
                        {s.label}
                      </button>
                    ))}
                  </div>
                  <p className="mt-3 border border-steel-400/20 px-2.5 py-2 font-mono text-[10px] leading-relaxed text-steel-400">
                    ataques bloqueados:{" "}
                    <span className="font-semibold text-brass-400">{blocked}/3</span>
                  </p>
                </div>

                <div ref={boxRef} className="term-scroll h-64 overflow-y-auto bg-ink-950/70 p-4 font-mono text-[11.5px] leading-[1.75] sm:h-[300px]">
                  {lines.map((l, i) => (
                    <p key={i} className={`${l.cls} animate-rowin whitespace-pre-wrap`}>
                      {l.txt}
                    </p>
                  ))}
                  <span className="animate-blinkc inline-block h-3.5 w-2 translate-y-0.5 bg-brass-500" />
                </div>
              </div>

              <div className="border-t border-steel-400/15 px-4 py-2.5">
                <p className="font-mono text-[10px] leading-relaxed text-steel-500">
                  <span className="text-brass-400">▸</span> Na vida real, esta suite roda automatizada em todo PR —
                  seção <a href="#qualidade" className="text-steel-300 underline decoration-steel-500 underline-offset-2 hover:text-brass-400">Testes & riscos</a>.
                </p>
              </div>
            </div>
          </Reveal>
        </div>
      </div>

      {/* ticker */}
      <div className="relative border-y border-steel-400/15 bg-ink-900/60 py-3">
        <div className="flex overflow-hidden">
          <div className="animate-marquee flex shrink-0 items-center">
            {[...TICKER, ...TICKER].map((t, i) => (
              <span key={i} className="flex items-center font-mono text-[11px] tracking-[0.22em] text-steel-400">
                <span className="px-6">{t}</span>
                <span className="text-brass-500">✳</span>
              </span>
            ))}
          </div>
        </div>
      </div>
    </section>
  );
}
