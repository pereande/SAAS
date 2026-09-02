import { STACK } from "../data";
import { Reveal, SectionHead } from "../ui";

export default function Stack() {
  return (
    <section id="stack" className="relative scroll-mt-28 border-t border-ink-900/10 bg-paper-2/60 py-20 lg:py-28">
      <div className="relative mx-auto max-w-7xl px-4 sm:px-6">
        <SectionHead
          num="02"
          kicker="Stack recomendada"
          title="Cada escolha tem um motivo — e um plano de saída."
          lead="Nenhuma tecnologia entra por moda. A tabela abaixo registra o quê, o porquê e a porta de escape de cada decisão; as três escolhas estruturais (tenancy, monolito modular e banco) estão detalhadas como ADRs na seção 10."
        />

        <Reveal>
          <div className="overflow-x-auto border border-ink-900/15 bg-white">
            <table className="w-full min-w-[720px] border-collapse text-left">
              <thead>
                <tr className="border-b border-ink-900/15 bg-ink-900 text-paper">
                  <th className="px-4 py-3 font-mono text-[10px] font-semibold uppercase tracking-[0.2em] text-steel-300">Camada</th>
                  <th className="px-4 py-3 font-mono text-[10px] font-semibold uppercase tracking-[0.2em] text-steel-300">Tecnologia</th>
                  <th className="px-4 py-3 font-mono text-[10px] font-semibold uppercase tracking-[0.2em] text-steel-300">Justificativa</th>
                </tr>
              </thead>
              <tbody>
                {STACK.map((r, i) => (
                  <tr
                    key={r.area}
                    className={`group border-b border-ink-900/8 transition-colors last:border-b-0 hover:bg-brass-100/40 ${
                      i % 2 === 1 ? "bg-paper/70" : ""
                    }`}
                  >
                    <td className="whitespace-nowrap px-4 py-3 align-top">
                      <span className="font-mono text-[10px] text-brass-600">{String(i + 1).padStart(2, "0")}</span>
                      <span className="ml-2 font-display text-[13.5px] font-bold text-ink-900">{r.area}</span>
                    </td>
                    <td className="px-4 py-3 align-top font-mono text-[11.5px] leading-relaxed text-jade-600">{r.tech}</td>
                    <td className="px-4 py-3 align-top text-[13px] leading-relaxed text-steel-700">{r.porque}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </Reveal>

        <Reveal delay={120}>
          <div className="mt-6 grid gap-4 sm:grid-cols-3">
            {[
              { t: "Porta de saída preservada", d: "EF Core abstrai o dialeto do banco; MinIO fala S3; adapters isolam SEFAZ, gateways e notificações." },
              { t: "Um deploy do núcleo", d: "Monolito modular: CI/CD simples agora, extração de serviços possível depois — sem reescrever domínios." },
              { t: "Operação Linux-first", d: "Docker + Terraform + GitHub Actions do dia 1; ambientes idênticos do notebook ao prod." },
            ].map((c) => (
              <div key={c.t} className="border border-ink-900/15 bg-white p-5 transition-all hover:-translate-y-1 hover:border-brass-500/70 hover:shadow-[0_12px_30px_rgba(11,27,46,0.12)]">
                <p className="font-display text-[14px] font-bold text-ink-900">{c.t}</p>
                <p className="mt-2 text-[12.5px] leading-relaxed text-steel-600">{c.d}</p>
              </div>
            ))}
          </div>
        </Reveal>
      </div>
    </section>
  );
}
