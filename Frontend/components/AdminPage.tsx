
import React, { useState } from 'react';
import { 
  Shield, Trash2, UserX, UserCheck, ShieldAlert, 
  ShieldCheck, ArrowUpDown, ChevronDown, MoreHorizontal,
  Mail, Search
} from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';
import { Language, User } from '../types';
import { MOCK_USERS, TRANSLATIONS } from '../constants';

const AdminPage: React.FC<{ lang: Language, user: User | null }> = ({ lang, user }) => {
  const [users, setUsers] = useState<User[]>(MOCK_USERS);
  const [sortMode, setSortMode] = useState<'name' | 'email'>('name');
  const [searchTerm, setSearchTerm] = useState('');
  
  const t = (key: string) => TRANSLATIONS[key]?.[lang] || key;

  if (!user || user.role !== 'admin') {
    return (
      <div className="text-center py-40 space-y-4">
        <ShieldAlert className="w-16 h-16 text-red-500 mx-auto animate-bounce" />
        <h1 className="text-2xl font-black text-red-500 tracking-[0.3em] uppercase">ACCESS DENIED</h1>
        <p className="text-gray-500 font-medium">This command center is restricted to system architects.</p>
      </div>
    );
  }

  const toggleUserStatus = (uid: string) => {
    setUsers(prev => prev.map(u => 
      u.id === uid ? { ...u, status: u.status === 'active' ? 'blocked' : 'active' } : u
    ));
  };

  const toggleUserRole = (uid: string) => {
    setUsers(prev => prev.map(u => 
      u.id === uid ? { ...u, role: u.role === 'admin' ? 'user' : 'admin' } : u
    ));
  };

  const filteredUsers = users.filter(u => 
    u.name.toLowerCase().includes(searchTerm.toLowerCase()) || 
    u.email.toLowerCase().includes(searchTerm.toLowerCase())
  ).sort((a, b) => a[sortMode].localeCompare(b[sortMode]));

  return (
    <div className="space-y-10">
      {/* Admin Header */}
      <div className="flex flex-col lg:flex-row lg:items-center justify-between gap-8 pb-10 border-b border-gray-100 dark:border-gray-800">
        <div className="flex items-center space-x-6">
          <div className="w-16 h-16 bg-red-100 dark:bg-red-950/30 rounded-3xl flex items-center justify-center shadow-inner">
            <ShieldAlert className="w-8 h-8 text-red-600" />
          </div>
          <div className="space-y-1">
            <h1 className="text-4xl font-black tracking-tighter">{t('userManagement')}</h1>
            <p className="text-gray-500 dark:text-gray-400 font-medium text-lg italic">System Command & Member Lifecycle Controls</p>
          </div>
        </div>

        <div className="flex flex-wrap items-center gap-4">
          <div className="relative">
            <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
            <input 
              type="text" 
              placeholder="Locate member..." 
              value={searchTerm}
              onChange={e => setSearchTerm(e.target.value)}
              className="pl-12 pr-6 py-4 bg-white dark:bg-gray-900 border border-gray-100 dark:border-gray-800 rounded-3xl outline-none focus:ring-2 focus:ring-red-500/20 text-sm font-bold shadow-xl shadow-gray-200/20 dark:shadow-none w-64 transition-all"
            />
          </div>
          <div className="flex items-center bg-gray-100 dark:bg-gray-800 p-1.5 rounded-[1.5rem] shadow-inner">
            <button 
              onClick={() => setSortMode('name')}
              className={`px-6 py-2.5 text-[10px] font-black tracking-widest rounded-2xl transition-all ${sortMode === 'name' ? 'bg-white dark:bg-gray-700 text-red-600 dark:text-red-400 shadow-xl' : 'text-gray-500'}`}
            >
              BY NAME
            </button>
            <button 
              onClick={() => setSortMode('email')}
              className={`px-6 py-2.5 text-[10px] font-black tracking-widest rounded-2xl transition-all ${sortMode === 'email' ? 'bg-white dark:bg-gray-700 text-red-600 dark:text-red-400 shadow-xl' : 'text-gray-500'}`}
            >
              BY EMAIL
            </button>
          </div>
        </div>
      </div>

      {/* User Table */}
      <div className="bg-white dark:bg-gray-900 rounded-[3rem] border border-gray-100 dark:border-gray-800 shadow-[0_35px_60px_-15px_rgba(0,0,0,0.05)] overflow-hidden">
        <table className="w-full text-left">
          <thead className="bg-gray-50/50 dark:bg-gray-800/30">
            <tr>
              <th className="px-10 py-6 text-[10px] font-black uppercase tracking-[0.3em] text-gray-400">Core Identity</th>
              <th className="px-10 py-6 text-[10px] font-black uppercase tracking-[0.3em] text-gray-400">Permissions</th>
              <th className="px-10 py-6 text-[10px] font-black uppercase tracking-[0.3em] text-gray-400">Lifecycle</th>
              <th className="px-10 py-6 text-right text-[10px] font-black uppercase tracking-[0.3em] text-gray-400">Command</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-50 dark:divide-gray-800">
            <AnimatePresence mode="popLayout">
              {filteredUsers.map((u) => (
                <motion.tr 
                  layout
                  key={u.id} 
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  exit={{ opacity: 0 }}
                  className="hover:bg-red-50/20 dark:hover:bg-red-950/5 transition-all group"
                >
                  <td className="px-10 py-8">
                    <div className="flex items-center space-x-5">
                      <div className="w-14 h-14 rounded-[1.5rem] bg-gradient-to-br from-gray-200 to-gray-50 dark:from-gray-800 dark:to-gray-900 flex items-center justify-center font-black text-lg text-gray-400 group-hover:from-red-100 group-hover:to-red-50 dark:group-hover:from-red-950 dark:group-hover:to-red-900 group-hover:text-red-600 transition-all duration-500">
                        {u.name[0]}
                      </div>
                      <div className="space-y-0.5">
                        <div className="font-black text-base tracking-tight">{u.name}</div>
                        <div className="text-xs text-gray-400 font-medium flex items-center">
                          <Mail className="w-3 h-3 mr-1.5 opacity-40" />
                          {u.email}
                        </div>
                      </div>
                    </div>
                  </td>
                  <td className="px-10 py-8">
                    <button 
                      onClick={() => toggleUserRole(u.id)}
                      className={`px-4 py-1.5 rounded-xl text-[10px] font-black uppercase tracking-widest transition-all hover:scale-105 active:scale-95 ${
                        u.role === 'admin' 
                        ? 'bg-red-100 dark:bg-red-900/30 text-red-600 dark:text-red-400 border border-red-200 dark:border-red-800' 
                        : 'bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400 border border-blue-200 dark:border-blue-800'
                      }`}
                    >
                      {u.role}
                    </button>
                  </td>
                  <td className="px-10 py-8">
                    <div className="flex items-center space-x-2">
                      <div className={`w-2 h-2 rounded-full ${u.status === 'active' ? 'bg-green-500 shadow-[0_0_8px_rgba(34,197,94,0.5)] animate-pulse' : 'bg-gray-300 dark:bg-gray-700'}`} />
                      <span className={`text-[10px] font-black uppercase tracking-widest ${u.status === 'active' ? 'text-green-600' : 'text-gray-400'}`}>
                        {u.status}
                      </span>
                    </div>
                  </td>
                  <td className="px-10 py-8 text-right">
                    <div className="flex items-center justify-end space-x-2">
                      <button 
                        onClick={() => toggleUserStatus(u.id)}
                        className={`p-3 rounded-2xl transition-all ${u.status === 'active' ? 'hover:bg-amber-50 dark:hover:bg-amber-950/30 text-amber-500' : 'hover:bg-green-50 dark:hover:bg-green-950/30 text-green-500'}`}
                        title={u.status === 'active' ? 'Block User' : 'Restore User'}
                      >
                        {u.status === 'active' ? <UserX className="w-5 h-5" /> : <UserCheck className="w-5 h-5" />}
                      </button>
                      <button 
                        className="p-3 hover:bg-red-50 dark:hover:bg-red-950/30 rounded-2xl text-red-600 transition-all opacity-40 hover:opacity-100"
                        title="Delete Permanently"
                      >
                        <Trash2 className="w-5 h-5" />
                      </button>
                    </div>
                  </td>
                </motion.tr>
              ))}
            </AnimatePresence>
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default AdminPage;
