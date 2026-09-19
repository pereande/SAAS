import { useEffect, useState } from "react";
import { Toaster } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import ErrorBoundary from "./components/ErrorBoundary";
import { ThemeProvider } from "./contexts/ThemeContext";
import Home from "./pages/Home";
import Login from "./pages/Login";
import { api, type SessionUser } from "./lib/api";

function App() {
  const [user, setUser] = useState<SessionUser | null>(null);
  const [checkingSession, setCheckingSession] = useState(true);
  const [darkMode, setDarkMode] = useState(() => localStorage.getItem("erp-saas-theme") === "dark");

  useEffect(() => {
    document.documentElement.classList.toggle("dark", darkMode);
    localStorage.setItem("erp-saas-theme", darkMode ? "dark" : "light");
  }, [darkMode]);

  useEffect(() => {
    let active = true;
    api.getSession().then((session) => {
      if (active) {
        setUser(session);
        setCheckingSession(false);
      }
    });
    return () => {
      active = false;
    };
  }, []);

  const handleLogin = (nextUser: SessionUser) => setUser(nextUser);
  const handleLogout = () => {
    api.logout();
    setUser(null);
  };

  return (
    <ErrorBoundary>
      <ThemeProvider defaultTheme="light">
        <TooltipProvider>
          <Toaster position="top-right" />
          {checkingSession ? (
            <div className="app-loading">
              <div className="brand-mark brand-mark--small">E</div>
              <span>Carregando seu espaço de gestão…</span>
            </div>
          ) : user ? (
            <Home user={user} onLogout={handleLogout} darkMode={darkMode} onToggleTheme={() => setDarkMode((value) => !value)} />
          ) : (
            <Login onLogin={handleLogin} />
          )}
        </TooltipProvider>
      </ThemeProvider>
    </ErrorBoundary>
  );
}

export default App;
