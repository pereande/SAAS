import type { ReactNode } from "react";

interface IcProps {
  className?: string;
}

function I({ children, className = "w-5 h-5" }: IcProps & { children: ReactNode }) {
  return (
    <svg
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth={1.7}
      strokeLinecap="round"
      strokeLinejoin="round"
      className={className}
      aria-hidden="true"
    >
      {children}
    </svg>
  );
}

/* Marca: hexágono com núcleo roteado — "matriz" que conecta bancos */
export const IcLogo = ({ className = "w-6 h-6" }: IcProps) => (
  <svg viewBox="0 0 24 24" fill="none" className={className} aria-hidden="true">
    <path d="M12 2.2 21 7.4v9.2l-9 5.2-9-5.2V7.4l9-5.2Z" stroke="currentColor" strokeWidth={1.6} strokeLinejoin="round" />
    <path d="M12 8.4v-3M15.1 13.8l2.6 1.5M8.9 13.8l-2.6 1.5" stroke="currentColor" strokeWidth={1.4} strokeLinecap="round" opacity={0.75} />
    <circle cx="12" cy="11.4" r="2.6" fill="currentColor" stroke="none" />
    <circle cx="12" cy="4.6" r="1.1" fill="currentColor" stroke="none" />
    <circle cx="18.3" cy="15.7" r="1.1" fill="currentColor" stroke="none" />
    <circle cx="5.7" cy="15.7" r="1.1" fill="currentColor" stroke="none" />
  </svg>
);

export const IcDash = (p: IcProps) => (
  <I {...p}>
    <rect x="3.5" y="3.5" width="7" height="7" rx="1.2" />
    <rect x="13.5" y="3.5" width="7" height="7" rx="1.2" />
    <rect x="3.5" y="13.5" width="7" height="7" rx="1.2" />
    <rect x="13.5" y="13.5" width="7" height="7" rx="1.2" />
  </I>
);
export const IcDb = (p: IcProps) => (
  <I {...p}>
    <ellipse cx="12" cy="5.2" rx="7.5" ry="2.7" />
    <path d="M4.5 5.2v13.6c0 1.5 3.4 2.7 7.5 2.7s7.5-1.2 7.5-2.7V5.2" />
    <path d="M4.5 12c0 1.5 3.4 2.7 7.5 2.7s7.5-1.2 7.5-2.7" />
  </I>
);
export const IcBuild = (p: IcProps) => (
  <I {...p}>
    <rect x="4.5" y="3.5" width="15" height="17" rx="1" />
    <path d="M9 7.5h1.6M13.4 7.5H15M9 11.5h1.6M13.4 11.5H15M10.2 20.5v-3.6h3.6v3.6" />
  </I>
);
export const IcUsers = (p: IcProps) => (
  <I {...p}>
    <circle cx="9" cy="8.2" r="3.1" />
    <path d="M3.4 19.4c.6-3.1 2.8-5 5.6-5s5 1.9 5.6 5" />
    <circle cx="16.6" cy="9.2" r="2.3" />
    <path d="M16.4 14.6c2.3.3 3.9 1.9 4.4 4.3" />
  </I>
);
export const IcShield = (p: IcProps) => (
  <I {...p}>
    <path d="M12 3l7 2.6v5.5c0 4.5-3 7.7-7 9.1-4-1.4-7-4.6-7-9.1V5.6L12 3Z" />
    <path d="M9 11.6l2.1 2.1 4-4.1" />
  </I>
);
export const IcAudit = (p: IcProps) => (
  <I {...p}>
    <rect x="4" y="3.5" width="16" height="17" rx="1.6" />
    <path d="M7.4 8.4l1.4 1.4 2.4-2.5M7.4 14.4l1.4 1.4 2.4-2.5M13.6 9h3.4M13.6 15h3.4" />
  </I>
);
export const IcTerminal = (p: IcProps) => (
  <I {...p}>
    <rect x="3" y="4" width="18" height="16" rx="1.8" />
    <path d="M7 9.2l3 2.8-3 2.8M12.6 15h4.4" />
  </I>
);
export const IcCompass = (p: IcProps) => (
  <I {...p}>
    <circle cx="12" cy="12" r="8.6" />
    <path d="M15.6 8.4l-2.2 5-5 2.2 2.2-5 5-2.2Z" />
  </I>
);
export const IcRoute = (p: IcProps) => (
  <I {...p}>
    <circle cx="5.5" cy="5.5" r="2" />
    <circle cx="18.5" cy="18.5" r="2" />
    <path d="M7.5 5.5H15a3.5 3.5 0 010 7H9a3.5 3.5 0 000 7h7.4" />
  </I>
);
export const IcBell = (p: IcProps) => (
  <I {...p}>
    <path d="M6.2 9.3a5.8 5.8 0 1111.6 0c0 4.8 1.9 5.9 1.9 5.9H4.3s1.9-1.1 1.9-5.9Z" />
    <path d="M10.3 19.6a1.9 1.9 0 003.4 0" />
  </I>
);
export const IcSearch = (p: IcProps) => (
  <I {...p}>
    <circle cx="11" cy="11" r="6.3" />
    <path d="M19.8 19.8l-4.2-4.2" />
  </I>
);
export const IcChevD = (p: IcProps) => (
  <I {...p}>
    <path d="M6 9.3l6 6 6-6" />
  </I>
);
export const IcChevR = (p: IcProps) => (
  <I {...p}>
    <path d="M9.3 6l6 6-6 6" />
  </I>
);
export const IcLock = (p: IcProps) => (
  <I {...p}>
    <rect x="5" y="10.6" width="14" height="9.4" rx="1.6" />
    <path d="M8.2 10.6V8a3.8 3.8 0 017.6 0v2.6" />
    <circle cx="12" cy="15.3" r="1.1" fill="currentColor" stroke="none" />
  </I>
);
export const IcCheck = (p: IcProps) => (
  <I {...p}>
    <path d="M5 12.6l4.4 4.4L19 7.4" />
  </I>
);
export const IcX = (p: IcProps) => (
  <I {...p}>
    <path d="M6 6l12 12M18 6L6 18" />
  </I>
);
export const IcAlert = (p: IcProps) => (
  <I {...p}>
    <path d="M10.3 4.5L2.7 17.8a1.9 1.9 0 001.7 2.9h15.2a1.9 1.9 0 001.7-2.9L13.7 4.5a1.95 1.95 0 00-3.4 0Z" />
    <path d="M12 10v4.4M12 17.6h.01" />
  </I>
);
export const IcInfo = (p: IcProps) => (
  <I {...p}>
    <circle cx="12" cy="12" r="8.6" />
    <path d="M12 11.2v5M12 7.6h.01" />
  </I>
);
export const IcPlug = (p: IcProps) => (
  <I {...p}>
    <path d="M9 3.5V8M15 3.5V8" />
    <path d="M6.5 8h11v3a5.5 5.5 0 01-11 0V8Z" />
    <path d="M12 16.5v4" />
  </I>
);
export const IcKey = (p: IcProps) => (
  <I {...p}>
    <circle cx="8" cy="15" r="4.2" />
    <path d="M11.2 11.8L20 3M16.8 6.2l2.6 2.6M14 9l1.9 1.9" />
  </I>
);
export const IcClock = (p: IcProps) => (
  <I {...p}>
    <circle cx="12" cy="12" r="8.6" />
    <path d="M12 7.2V12l3.4 2" />
  </I>
);
export const IcPlus = (p: IcProps) => (
  <I {...p}>
    <path d="M12 5v14M5 12h14" />
  </I>
);
export const IcSwap = (p: IcProps) => (
  <I {...p}>
    <path d="M4 7.2h13.2L14 4M20 16.8H6.8L10 20" />
  </I>
);
export const IcOut = (p: IcProps) => (
  <I {...p}>
    <path d="M9.5 4H6.5a2 2 0 00-2 2v12a2 2 0 002 2h3" />
    <path d="M15 8.2l3.8 3.8-3.8 3.8M18.5 12H9.8" />
  </I>
);
export const IcEye = (p: IcProps) => (
  <I {...p}>
    <path d="M2.8 12S6.2 5.6 12 5.6 21.2 12 21.2 12 17.8 18.4 12 18.4 2.8 12 2.8 12Z" />
    <circle cx="12" cy="12" r="2.7" />
  </I>
);
export const IcLayers = (p: IcProps) => (
  <I {...p}>
    <path d="M12 3.4l8.6 4.8L12 13 3.4 8.2 12 3.4Z" />
    <path d="M3.9 12.6L12 17.1l8.1-4.5M3.9 16.6L12 21.1l8.1-4.5" />
  </I>
);
export const IcBolt = (p: IcProps) => (
  <I {...p}>
    <path d="M13.2 2.8L5 13.4h6l-1.4 7.8 8.4-10.8h-6.2l1.4-7.6Z" />
  </I>
);
export const IcDoc = (p: IcProps) => (
  <I {...p}>
    <path d="M6 3.5h7.6L18 7.9v12.6H6V3.5Z" />
    <path d="M13.5 3.5v4.5H18M9 12.5h6M9 16h6" />
  </I>
);
export const IcBox = (p: IcProps) => (
  <I {...p}>
    <path d="M12 3l8 4.4v9.2L12 21l-8-4.4V7.4L12 3Z" />
    <path d="M4.2 7.5L12 11.8l7.8-4.3M12 11.8V21" />
  </I>
);
export const IcCart = (p: IcProps) => (
  <I {...p}>
    <circle cx="9.3" cy="19" r="1.5" />
    <circle cx="16.8" cy="19" r="1.5" />
    <path d="M3.5 4.5H6l2.2 10.3h9.7l2.3-7.8H7" />
  </I>
);
export const IcTruck = (p: IcProps) => (
  <I {...p}>
    <path d="M2.8 6h11v9.5h-11zM13.8 9.2h3.9l3.5 3.4v2.9h-2.6" />
    <circle cx="7" cy="17.6" r="1.7" />
    <circle cx="16.6" cy="17.6" r="1.7" />
    <path d="M8.7 15.5h6.2" />
  </I>
);
export const IcCoin = (p: IcProps) => (
  <I {...p}>
    <rect x="2.6" y="6.2" width="18.8" height="11.6" rx="1.6" />
    <circle cx="12" cy="12" r="2.6" />
    <path d="M6 9.6h.01M18 14.4h.01" />
  </I>
);
export const IcSeal = (p: IcProps) => (
  <I {...p}>
    <circle cx="12" cy="9.5" r="5.6" />
    <path d="M9.6 9.5l1.7 1.7 3.1-3.2" />
    <path d="M8.6 14.1L7.2 21l4.8-2.4L16.8 21l-1.4-6.9" />
  </I>
);
export const IcNet = (p: IcProps) => (
  <I {...p}>
    <circle cx="12" cy="5" r="2.1" />
    <circle cx="5" cy="18.4" r="2.1" />
    <circle cx="19" cy="18.4" r="2.1" />
    <path d="M12 7.1v3.4M12 10.5l-5.4 6M12 10.5l5.4 6" />
  </I>
);
export const IcDots = (p: IcProps) => (
  <I {...p}>
    <path d="M6 12h.01M12 12h.01M18 12h.01" strokeWidth={2.6} />
  </I>
);
