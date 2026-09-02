import { useEffect, useState } from "react";
import { NAV } from "../data";

export type AprovacaoState = {
  status: "pendente" | "aprovado" | "revisao";
  quando?: string;
  nota?: string;
};

export default function Header({ aprovacao }: { aprovacao: AprovacaoState }) {
  const [progress, setProgress] = useState(0);
  const [active, setActive] = useState("capa");
  const [scrolled, setScrolled] = useState(false);

  useEffect(() => {
    const onScroll = () => {
      const h = document.documentElement;
      const total = h.scrollHeight - h.clientHeight;
      setProgress(total > 0 ? (h.scrollTop / total) * 100 : 0);
      setScrolled(h.scrollTop > 24);
    };
    onScroll();
    window.addEventListener("scroll", onScroll, { passive: true });
    return () => window.removeEventListener("scroll", onScroll);
  }, []);

  useEffect(() => {
    const secs = NAV.map((n) => document.getElementById(n.id)).filter(Boolean) as HTMLElement[];
    const io = new IntersectionObserver(
      (entries) => {
        entries.forEach((e) => {
          if (e.isIntersecting) setActive(e.target.id);
        });
      },
      { rootMargin: "-38% 0px -55% 0px" }
    );
    secs.forEach((s) => io.observe(s));
    return () => io.disconnect();
  }, []);

  const statusChip =
    aprovacao.status === "aprovado" ? (
      <span className="flex items-center gap-1.5 border border-moss-500/60 bg-moss-500/10 px-2 py-1 font-mono text-[10px] font-semibold tracking-widest text-moss-500">
        <span className="h-1.5 w-1.5 bg-moss-500" /> APROVADO
      </span>
    ) : aprovacao.status === "revisao" ? (
      <span className="flex items-center gap-1.5 border border-brass-500/60 bg-brass-500/10 px-2 py-1 font-mono text-[10px] font-semibold tracking-widest text-brass-500">
        <span className="h-1.5 w-1.5 bg-brass-500" /> EM REVISÃO
      </span>
    ) : (
      <span className="flex items-center gap-1.5 border border-steel-400/40 px-2 py-1 font-mono text-[10px] font-semibold tracking-widest text-steel-300">
        <span className="pulse-dot h-1.5 w-1.5 rounded-full bg-brass-500" /> AGUARDANDO APROVAÇÃO
      </span>
    );

  return (
    <header
      className={`fixed inset-x-0 top-0 z-50 border-b border-steel-400/15 bg-ink-950/92 backdrop-blur-sm transition-shadow ${
        scrolled ? "shadow-[0_10px_40px_rgba(0,0,0,0.45)]" : ""
      }`}
    >
      {/* progress */}
      <div className="absolute left-0 top-0 h-[2px] w-full bg-ink-800">
        <div className="h-full bg-brass-500 transition-[width] duration-150 ease-out" style={{ width: `${progress}%` }} />
      </div>

      <div className="mx-auto flex max-w-7xl items-center justify-between gap-4 px-4 py-3 sm:px-6">
        <a href="#capa" className="group flex items-center gap-3">
          <span className="flex h-9 w-9 items-center justify-center bg-brass-500 font-display text-lg font-bold text-ink-950 transition-transform group-hover:-rotate-6">
            M
          </span>
          <span className="leading-none">
            <span className="block font-display text-[17px] font-bold tracking-wide text-paper">MATRIZ</span>
            <span className="mt-1 block font-mono text-[9.5px] uppercase tracking-[0.22em] text-steel-400">
              Blueprint de Arquitetura · ERP SaaS
            </span>
          </span>
        </a>

        <div className="flex items-center gap-3">
          <span className="hidden font-mono text-[10.5px] tracking-widest text-steel-500 md:inline">v1.0 · 2026</span>
          {statusChip}
        </div>
      </div>

      {/* anchors */}
      <nav className="border-t border-steel-400/10">
        <div className="no-scrollbar mx-auto flex max-w-7xl gap-1 overflow-x-auto px-4 sm:px-6">
          {NAV.map((n) => (
            <a
              key={n.id}
              href={`#${n.id}`}
              className={`relative shrink-0 px-2.5 py-2.5 font-mono text-[10.5px] tracking-wider transition-colors ${
                active === n.id ? "text-brass-400" : "text-steel-400 hover:text-paper"
              }`}
            >
              <span className="mr-1.5 opacity-60">{n.num}</span>
              {n.label}
              <span
                className={`absolute inset-x-2 bottom-0 h-[2px] origin-left bg-brass-500 transition-transform duration-300 ${
                  active === n.id ? "scale-x-100" : "scale-x-0"
                }`}
              />
            </a>
          ))}
        </div>
      </nav>
    </header>
  );
}
