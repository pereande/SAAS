import { useEffect, useRef, useState, type ReactNode, type CSSProperties } from "react";

/* ---------- hooks ---------- */

export function useInView<T extends HTMLElement>(threshold = 0.18, once = true) {
  const ref = useRef<T | null>(null);
  const [inView, setInView] = useState(false);

  useEffect(() => {
    const el = ref.current;
    if (!el) return;
    if (typeof IntersectionObserver === "undefined") {
      setInView(true);
      return;
    }
    const io = new IntersectionObserver(
      (entries) => {
        entries.forEach((e) => {
          if (e.isIntersecting) {
            setInView(true);
            if (once) io.unobserve(e.target);
          } else if (!once) {
            setInView(false);
          }
        });
      },
      { threshold, rootMargin: "0px 0px -8% 0px" }
    );
    io.observe(el);
    return () => io.disconnect();
  }, [threshold, once]);

  return { ref, inView };
}

/* ---------- Reveal ---------- */

export function Reveal({
  children,
  className = "",
  delay = 0,
  style,
}: {
  children: ReactNode;
  className?: string;
  delay?: number;
  style?: CSSProperties;
}) {
  const { ref, inView } = useInView<HTMLDivElement>();
  const reduced =
    typeof window !== "undefined" &&
    window.matchMedia &&
    window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  return (
    <div
      ref={ref}
      className={`${reduced ? "" : "rv"} ${inView ? "on" : ""} ${className}`}
      style={{ transitionDelay: `${delay}ms`, ...style }}
    >
      {children}
    </div>
  );
}

/* ---------- CountUp ---------- */

export function CountUp({ value, suffix = "" }: { value: number; suffix?: string }) {
  const { ref, inView } = useInView<HTMLSpanElement>(0.4);
  const [n, setN] = useState(0);

  useEffect(() => {
    if (!inView) return;
    let raf = 0;
    const t0 = performance.now();
    const dur = 900;
    const tick = (t: number) => {
      const p = Math.min(1, (t - t0) / dur);
      const eased = 1 - Math.pow(1 - p, 3);
      setN(Math.round(value * eased));
      if (p < 1) raf = requestAnimationFrame(tick);
    };
    raf = requestAnimationFrame(tick);
    return () => cancelAnimationFrame(raf);
  }, [inView, value]);

  return (
    <span ref={ref}>
      {n}
      {suffix}
    </span>
  );
}

/* ---------- Section head ---------- */

export function SectionHead({
  num,
  kicker,
  title,
  lead,
  tone = "light",
}: {
  num: string;
  kicker: string;
  title: string;
  lead?: string;
  tone?: "light" | "dark";
}) {
  return (
    <Reveal className="mb-10 md:mb-14">
      <div className="flex items-center gap-3">
        <span className="inline-block h-2.5 w-2.5 bg-brass-500" aria-hidden />
        <span
          className={`font-mono text-[11px] font-medium uppercase tracking-[0.28em] ${
            tone === "dark" ? "text-steel-400" : "text-steel-600"
          }`}
        >
          Seção {num} · {kicker}
        </span>
      </div>
      <h2
        className={`mt-4 font-display text-3xl font-bold leading-[1.05] tracking-tight sm:text-4xl lg:text-[2.75rem] ${
          tone === "dark" ? "text-paper" : "text-ink-900"
        }`}
      >
        {title}
      </h2>
      {lead && (
        <p
          className={`mt-4 max-w-3xl text-[15px] leading-relaxed sm:text-base ${
            tone === "dark" ? "text-steel-300" : "text-steel-700"
          }`}
        >
          {lead}
        </p>
      )}
    </Reveal>
  );
}

/* ---------- small atoms ---------- */

export function MonoTag({ children, tone = "steel" }: { children: ReactNode; tone?: "steel" | "brass" | "jade" | "ember" | "moss" }) {
  const map: Record<string, string> = {
    steel: "border-steel-400/40 text-steel-500",
    brass: "border-brass-500/50 text-brass-600",
    jade: "border-jade-500/50 text-jade-600",
    ember: "border-ember-500/50 text-ember-600",
    moss: "border-moss-500/50 text-moss-600",
  };
  return (
    <span className={`inline-block border px-1.5 py-0.5 font-mono text-[10.5px] font-medium leading-none ${map[tone]}`}>
      {children}
    </span>
  );
}

export function KeyBadge({ k }: { k: "PK" | "FK" | "UQ" | "IDX" }) {
  const map: Record<string, string> = {
    PK: "bg-brass-500 text-ink-950",
    FK: "bg-jade-500/15 text-jade-600 border border-jade-500/40",
    UQ: "bg-ink-900/8 text-ink-700 border border-ink-900/20",
    IDX: "bg-steel-400/15 text-steel-600 border border-steel-400/40",
  };
  return (
    <span className={`inline-block w-8 shrink-0 py-0.5 text-center font-mono text-[9.5px] font-semibold leading-none ${map[k]}`}>
      {k}
    </span>
  );
}

export function Check({ on = true, size = 14 }: { on?: boolean; size?: number }) {
  return (
    <svg width={size} height={size} viewBox="0 0 16 16" fill="none" aria-hidden>
      <path
        d="M2.5 8.5L6.5 12.5L13.5 3.5"
        stroke="currentColor"
        strokeWidth="2.2"
        strokeLinecap="square"
        opacity={on ? 1 : 0.25}
      />
    </svg>
  );
}

export function XMark({ size = 14 }: { size?: number }) {
  return (
    <svg width={size} height={size} viewBox="0 0 16 16" fill="none" aria-hidden>
      <path d="M3 3L13 13M13 3L3 13" stroke="currentColor" strokeWidth="2.2" strokeLinecap="square" />
    </svg>
  );
}

export function ArrowNE({ size = 12 }: { size?: number }) {
  return (
    <svg width={size} height={size} viewBox="0 0 16 16" fill="none" aria-hidden>
      <path d="M4 12L12 4M12 4H6M12 4V10" stroke="currentColor" strokeWidth="1.8" strokeLinecap="square" />
    </svg>
  );
}
