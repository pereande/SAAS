import { useEffect, useRef, useState } from "react";
import { CAMADAS, SAGA_PASSOS, type Componente } from "../data";
import { Reveal, SectionHead } from "../ui";

const LAYER_TONE = ["bg-brass-500", "bg-jade-500", "bg-paper", "bg-steel-400", "bg-jade-300", "bg-brass-400"];

function LayerDiagram() {
  const [sel, setSel] = useState<{ comp: Componente; layer: string } | null>({
    comp: CAMADAS[2].componentes[5],
    layer: CAMADAS[2].nome,
  });

  return (
    <div className="grid gap-6 lg:grid-cols-[1.15fr_1fr]">
      {/* layers */}
      <div className="flex flex-col gap-2">
        {CAMADAS.map((cam, i) => (
          <Reveal key={cam.id} delay={i * 70}>
            <div className="group border border-ink-900/15 bg-white transition-all hover:border-ink-900/40 hover:shadow-[0_8px_30px_rgba(11,27,46,0.1)]">
              <div className="flex items-center gap-3 border-b border-ink-900/10 px-4 py-2.5">
                <span className={`h-2 w-2 ${LAYER_TONE[i]}`} />
                <span className="font-mono text-[10px] uppercase tracking-[0.2em] text-steel-600">
                  L{i + 1} · {cam.nome}
                </span>
                <span className="ml-auto hidden font-mono text-[10px] text-steel-500 sm:block">{cam.desc}</span>
              </div>
              <div className="flex flex-wrap gap-1.5 p-3">
                {cam.componentes.map((cp) => {
                  const isSel = sel?.comp.nome === cp.nome;
                  return (
                    <button
                      key={cp.nome}
                      onClick={() => setSel({ comp: cp, layer: cam.nome })}
                      className={`border px-2.5 py-1.5 font-mono text-[11px] transition-all ${
                        isSel
                          ? "border-ink-900 bg-ink-900 text-brass-400 shadow-[3px_3px_0_rgba(242,165,22,0.9)]"
                          : "border-ink-900/20 bg-paper text-ink-700 hover:-translate-y-0.5 hover:border-ink-900/60"
                      }`}
                    >
                      {cp.nome}
                    </button>
                  );
                })}
              </div>
            </div>
          </Reveal>
        ))}
        <p className="mt-1 font-mono text-[10.5px] text-steel-600">▸ clique em um componente para ver a justificativa</p>
      </div>

      {/* detail */}
      <div className="lg:sticky lg:top-32 lg:self-start">
        {sel && (
          <div key={sel.comp.nome} className="animate-rowin corner-ticks border border-ink-900/20 bg-ink-900 p-6 text-paper">
            <p className="font-mono text-[10px] uppercase tracking-[0.24em] text-brass-400">
              Camada · {sel.layer}
            </p>
            <h3 className="mt-3 font-display text-2xl font-bold">{sel.comp.nome}</h3>
            <p className="mt-1 font-mono text-[11.5px] text-jade-300">{sel.comp.tech}</p>
            <div className="mt-5 border-t border-steel-400/20 pt-5">
              <p className="font-mono text-[10px] uppercase tracking-[0.2em] text-steel-400">Por quê</p>
              <p className="mt-2 text-[14.5px] leading-relaxed text-steel-300">{sel.comp.porque}</p>
            </div>
          </div>
        )}

        <Reveal delay={120}>
          <div className="mt-4 border border-ink-900/15 bg-paper-2/70 p-5">
            <p className="font-mono text-[10px] uppercase tracking-[0.2em] text-steel-600">Princípio do desenho</p>
            <p className="mt-2 text-sm leading-relaxed text-ink-700">
              Tudo que <strong>demora</strong> vai para fila; tudo que é <strong>regra fiscal</strong> vive no módulo
              fiscal; tudo que é <strong>identidade do tenant</strong> passa pelo core/tenant. As setas entre camadas só
              descem — dependências de domínio nunca sobem.
            </p>
          </div>
        </Reveal>
      </div>
    </div>
  );
}

function Saga() {
  const [step, setStep] = useState(-1); // -1 idle, 0..6 running, 7 done
  const timer = useRef<number | null>(null);

  useEffect(() => () => { if (timer.current) window.clearInterval(timer.current); }, []);

  const run = () => {
    if (step >= 0 && step < SAGA_PASSOS.length) return;
    setStep(0);
    let i = 0;
    timer.current = window.setInterval(() => {
      i += 1;
      if (i > SAGA_PASSOS.length) {
        if (timer.current) window.clearInterval(timer.current);
        return;
      }
      setStep(i);
    }, 620);
  };

  const reset = () => {
    if (timer.current) window.clearInterval(timer.current);
    setStep(-1);
  };

  const done = step > SAGA_PASSOS.length - 1;

  return (
    <Reveal>
      <div className="border border-ink-900/15 bg-white">
        <div className="flex flex-wrap items-center justify-between gap-3 border-b border-ink-900/10 px-5 py-3.5">
          <div>
            <h3 className="font-display text-lg font-bold text-ink-900">A saga transacional de uma venda</h3>
            <p className="font-mono text-[10.5px] uppercase tracking-[0.18em] text-steel-600">
              requisito nº 42 · nada fica pela metade
            </p>
          </div>
          <div className="flex gap-2">
            <button
              onClick={run}
              disabled={step >= 0 && !done}
              className="bg-ink-900 px-4 py-2 font-display text-xs font-bold uppercase tracking-wider text-brass-400 transition-all hover:-translate-y-0.5 disabled:opacity-40"
            >
              {step === -1 ? "▶ Executar fluxo" : done ? "▶ Executar de novo" : "Executando…"}
            </button>
            {step !== -1 && (
              <button
                onClick={reset}
                className="border border-ink-900/25 px-4 py-2 font-display text-xs font-semibold uppercase tracking-wider text-steel-600 transition-colors hover:border-ember-500 hover:text-ember-600"
              >
                zerar
              </button>
            )}
          </div>
        </div>

        <ol className="grid gap-0 sm:grid-cols-2 lg:grid-cols-7">
          {SAGA_PASSOS.map((p, i) => {
            const state = step === -1 ? "idle" : step === i ? "active" : step > i ? "done" : "pending";
            return (
              <li key={p.id} className={`relative border-ink-900/10 p-4 transition-colors duration-300 sm:border-r sm:last:border-r-0 ${
                state === "active" ? "bg-ink-900" : state === "done" ? "bg-moss-100/60" : "bg-transparent"
              } ${i < 6 ? "border-b lg:border-b-0" : ""} ${i % 2 === 1 ? "sm:[&:nth-child(2n)]:border-r-0 sm:border-r" : ""}`}>
                <div className="flex items-center gap-2">
                  <span
                    className={`flex h-6 w-6 shrink-0 items-center justify-center font-mono text-[11px] font-semibold transition-all ${
                      state === "active"
                        ? "bg-brass-500 text-ink-950"
                        : state === "done"
                        ? "bg-moss-500 text-white"
                        : "border border-ink-900/25 text-steel-600"
                    }`}
                  >
                    {state === "done" ? "✓" : i + 1}
                  </span>
                  <span className={`font-display text-[12.5px] font-bold leading-tight ${state === "active" ? "text-paper" : "text-ink-900"}`}>
                    {p.t}
                  </span>
                </div>
                <p className={`mt-2 text-[11.5px] leading-snug ${state === "active" ? "text-steel-300" : "text-steel-600"}`}>
                  {p.d}
                </p>
                {state === "active" && <span className="absolute inset-x-4 bottom-0 h-0.5 bg-brass-500" />}
              </li>
            );
          })}
        </ol>

        <div className={`border-t border-ink-900/10 px-5 py-3 transition-colors ${done ? "bg-moss-100/60" : "bg-paper-2/60"}`}>
          <p className={`font-mono text-[11px] leading-relaxed ${done ? "text-moss-600" : "text-steel-600"}`}>
            {step === -1 && "▸ Cada passo confirma antes do próximo. Sem transação, sem venda."}
            {step >= 0 && !done && `▸ executando passo ${Math.min(step + 1, 7)} de 7…`}
            {done && "▸ venda confirmada · 7 eventos de auditoria gravados · XML na fila do Fiscal.Worker"}
          </p>
          <p className="mt-1 font-mono text-[10.5px] text-steel-500">
            Compensação prevista: NF-e rejeitada → libera reserva de estoque, estorna o financeiro e marca o pedido como
            rejeitado — automaticamente.
          </p>
        </div>
      </div>
    </Reveal>
  );
}

export default function Architecture() {
  return (
    <section id="arquitetura" className="relative scroll-mt-28 bg-paper py-20 lg:py-28">
      <div className="bp-grid-light pointer-events-none absolute inset-0 [mask-image:linear-gradient(to_bottom,black,transparent_60%)]" aria-hidden />
      <div className="relative mx-auto max-w-7xl px-4 sm:px-6">
        <SectionHead
          num="01"
          kicker="Arquitetura geral"
          title="Um monolito modular com fronteiras de serviço — e um worker fiscal que ninguém derruba."
          lead="Seis camadas, doze bounded contexts empacotados como módulos, e tudo que demora — emissão fiscal, relatórios, notificações, backups — fora do request HTTP. O diagrama abaixo é o mapa inteiro do sistema; cada componente carrega sua própria justificativa."
        />
        <LayerDiagram />

        <div className="mt-16" id="saga">
          <Saga />
        </div>
      </div>
    </section>
  );
}
