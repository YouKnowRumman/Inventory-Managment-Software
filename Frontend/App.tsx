import React, { useState, useEffect } from 'react';
import { HashRouter, Routes, Route, Link, useLocation } from 'react-router-dom';
import { 
  Sun, Moon, Globe, LogOut, Search, Settings, 
  Layers, Shield, LayoutDashboard, Menu, X
} from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';
import { Theme, Language, User } from './types';
import { TRANSLATIONS } from './constants';
import MainDashboard from './components/MainDashboard';
import InventoryDashboard from './components/InventoryDashboard';
import PersonalPage from './components/PersonalPage';
import AdminPage from './components/AdminPage';
import AuthPage from './components/AuthPage';

const App: React.FC = () => {
  const [theme, setTheme] = useState<Theme>(() => (localStorage.getItem('theme') as Theme) || 'light');
  const [lang, setLang] = useState<Language>(() => (localStorage.getItem('lang') as Language) || 'en');
  const [user, setUser] = useState<User | null>(() => {
    const saved = localStorage.getItem('user');
    return saved ? JSON.parse(saved) : null;
  });
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  useEffect(() => {
    localStorage.setItem('theme', theme);
    if (theme === 'dark') {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
  }, [theme]);

  useEffect(() => {
    localStorage.setItem('lang', lang);
  }, [lang]);

  const toggleTheme = () => setTheme(prev => prev === 'light' ? 'dark' : 'light');
  const t = (key: string) => TRANSLATIONS[key]?.[lang] || key;

  return (
    <HashRouter>
      <div className="min-h-screen flex flex-col transition-theme font-sans">
        <header className="sticky top-0 z-[60] bg-executive-brand dark:bg-command-bg border-b border-executive-brand dark:border-command-elevated text-white shadow-md">
          <div className="max-w-7xl mx-auto px-4 h-16 flex items-center justify-between gap-4">
            <Link to="/" className="flex items-center space-x-2 shrink-0 group">
              <div className="w-8 h-8 bg-white/15 rounded flex items-center justify-center border border-white/20">
                <Layers className="text-white w-5 h-5" />
              </div>
              <span className="font-bold text-lg tracking-tight uppercase">OmniVault</span>
            </Link>

            <div className="flex-1 max-w-sm hidden lg:block">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-white/50" />
                <input
                  type="text"
                  placeholder={t('searchPlaceholder')}
                  className="w-full pl-10 pr-4 py-2 bg-white/10 hover:bg-white/15 border border-white/10 focus:border-white/30 rounded outline-none transition-all text-sm placeholder:text-white/50"
                />
              </div>
            </div>

            <div className="flex items-center space-x-1 lg:space-x-3">
              <nav className="hidden md:flex items-center space-x-5 mr-2">
                {user?.role === 'admin' && (
                  <Link to="/admin" className="text-[10px] font-black uppercase tracking-widest flex items-center text-white/80 hover:text-white transition-colors py-1 border-b border-transparent hover:border-executive-accent">
                    <Shield className="w-4 h-4 mr-2 text-executive-accent" /> Admin
                  </Link>
                )}
                {user && (
                  <Link to={`/profile/${user.id}`} className="text-[10px] font-black uppercase tracking-widest flex items-center text-white/80 hover:text-white transition-colors py-1 border-b border-transparent hover:border-executive-accent">
                    <LayoutDashboard className="w-4 h-4 mr-2" /> Dashboard
                  </Link>
                )}
              </nav>

              <div className="flex items-center">
                <button
                  onClick={() => setLang(lang === 'en' ? 'bn' : 'en')}
                  className="p-2 hover:bg-white/10 rounded transition-all flex items-center space-x-1"
                  title="Switch Language"
                >
                  <Globe className="w-4 h-4 text-white/70" />
                  <span className="text-[10px] font-black uppercase">{lang}</span>
                </button>

                <button
                  onClick={toggleTheme}
                  className="p-2 hover:bg-white/10 rounded transition-all"
                  title="Toggle Theme"
                >
                  {theme === 'light' ? <Moon className="w-4 h-4" /> : <Sun className="w-4 h-4 text-command-accent" />}
                </button>

                <button 
                  onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
                  className="md:hidden p-2 hover:bg-white/10 rounded transition-all ml-1"
                >
                  {isMobileMenuOpen ? <X className="w-5 h-5" /> : <Menu className="w-5 h-5" />}
                </button>
              </div>

              {user ? (
                <div className="hidden md:flex items-center space-x-3 ml-2 pl-4 border-l border-white/10">
                  <div className="flex flex-col items-end">
                    <span className="text-xs font-bold leading-none">{user.name}</span>
                    <span className="text-[9px] font-black uppercase text-white/50 tracking-widest mt-1">{user.role}</span>
                  </div>
                  <button
                    onClick={() => { setUser(null); localStorage.removeItem('user'); }}
                    className="p-2 text-white/70 hover:text-white hover:bg-white/10 rounded transition-all"
                  >
                    <LogOut className="w-4 h-4" />
                  </button>
                </div>
              ) : (
                <Link
                  to="/auth"
                  className="px-4 py-2 bg-executive-accent text-white text-[10px] font-black rounded hover:brightness-110 transition-all uppercase tracking-widest hidden md:block"
                >
                  {t('login')}
                </Link>
              )}
            </div>
          </div>
        </header>

        {/* Mobile Menu */}
        <AnimatePresence>
          {isMobileMenuOpen && (
            <motion.div
              initial={{ height: 0, opacity: 0 }}
              animate={{ height: 'auto', opacity: 1 }}
              exit={{ height: 0, opacity: 0 }}
              className="md:hidden bg-executive-brand dark:bg-command-elevated border-b border-white/10 overflow-hidden text-white z-[55] sticky top-16"
            >
              <div className="px-4 py-6 space-y-4">
                {user?.role === 'admin' && (
                  <Link to="/admin" onClick={() => setIsMobileMenuOpen(false)} className="block text-xs font-black uppercase tracking-widest py-2 border-b border-white/10">Admin Control</Link>
                )}
                {user && (
                  <Link to={`/profile/${user.id}`} onClick={() => setIsMobileMenuOpen(false)} className="block text-xs font-black uppercase tracking-widest py-2 border-b border-white/10">My Dashboard</Link>
                )}
                {!user ? (
                  <Link to="/auth" onClick={() => setIsMobileMenuOpen(false)} className="block text-xs font-black uppercase tracking-widest py-2 border-b border-white/10">Login</Link>
                ) : (
                  <button onClick={() => { setUser(null); localStorage.removeItem('user'); setIsMobileMenuOpen(false); }} className="w-full text-left text-xs font-black uppercase tracking-widest py-2">Logout</button>
                )}
              </div>
            </motion.div>
          )}
        </AnimatePresence>

        <main className="flex-1 max-w-7xl mx-auto px-4 py-6 w-full">
          <AnimatePresence mode="wait">
            <Routes>
              <Route path="/" element={<PageWrapper><MainDashboard lang={lang} /></PageWrapper>} />
              <Route path="/auth" element={<PageWrapper><AuthPage lang={lang} setUser={setUser} /></PageWrapper>} />
              <Route path="/inventory/:id" element={<PageWrapper><InventoryDashboard lang={lang} user={user} /></PageWrapper>} />
              <Route path="/profile/:id" element={<PageWrapper><PersonalPage lang={lang} user={user} /></PageWrapper>} />
              <Route path="/admin" element={<PageWrapper><AdminPage lang={lang} user={user} /></PageWrapper>} />
            </Routes>
          </AnimatePresence>
        </main>

        <footer className="bg-executive-surface dark:bg-command-elevated border-t border-executive-border dark:border-command-elevated py-8 transition-theme mt-auto">
          <div className="max-w-7xl mx-auto px-4 flex flex-col md:flex-row justify-between items-center gap-4">
            <p className="text-[10px] font-bold text-executive-textSecondary dark:text-command-textSecondary uppercase tracking-widest">
              OmniVault &copy; 2024 &bull; Global Operational Intelligence
            </p>
            <div className="flex space-x-6">
              <a href="#" className="text-[10px] font-bold text-executive-textSecondary dark:text-command-textSecondary uppercase tracking-widest hover:text-executive-brand dark:hover:text-command-brand">Compliance</a>
              <a href="#" className="text-[10px] font-bold text-executive-textSecondary dark:text-command-textSecondary uppercase tracking-widest hover:text-executive-brand dark:hover:text-command-brand">Protocol</a>
              <a href="#" className="text-[10px] font-bold text-executive-textSecondary dark:text-command-textSecondary uppercase tracking-widest hover:text-executive-brand dark:hover:text-command-brand">API</a>
            </div>
          </div>
        </footer>
      </div>
    </HashRouter>
  );
};

const PageWrapper: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const location = useLocation();
  return (
    <motion.div
      key={location.pathname}
      initial={{ opacity: 0, x: 5 }}
      animate={{ opacity: 1, x: 0 }}
      exit={{ opacity: 0, x: -5 }}
      transition={{ duration: 0.2 }}
    >
      {children}
    </motion.div>
  );
}

export default App;