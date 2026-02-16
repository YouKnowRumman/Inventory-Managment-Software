
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Mail, Lock, User as UserIcon, Facebook, Globe, CheckCircle } from 'lucide-react';
import { MOCK_USERS } from '../constants';

const AuthPage: React.FC<{ lang: string, setUser: any }> = ({ lang, setUser }) => {
  const [tab, setTab] = useState<'login' | 'register'>('login');
  const [confirmed, setConfirmed] = useState(false);
  const navigate = useNavigate();

  const handleLogin = (e: React.FormEvent) => {
    e.preventDefault();
    const u = MOCK_USERS[0];
    setUser(u);
    localStorage.setItem('user', JSON.stringify(u));
    navigate('/');
  };

  const handleRegister = (e: React.FormEvent) => {
    e.preventDefault();
    setConfirmed(true);
  };

  if (confirmed) {
    return (
      <div className="max-w-md mx-auto py-20 text-center space-y-4 animate-in zoom-in duration-300">
        <CheckCircle className="w-16 h-16 text-green-500 mx-auto" />
        <h2 className="text-2xl font-bold">Check your email</h2>
        <p className="text-gray-500">We've sent a confirmation link to your email address. Please click it to activate your account.</p>
        <button onClick={() => setConfirmed(false)} className="text-blue-600 font-bold underline">Back to Login</button>
      </div>
    );
  }

  return (
    <div className="max-w-md mx-auto bg-white dark:bg-gray-900 rounded-3xl p-8 shadow-xl border border-gray-100 dark:border-gray-800">
      <div className="flex p-1 bg-gray-100 dark:bg-gray-800 rounded-2xl mb-8">
        <button 
          onClick={() => setTab('login')}
          className={`flex-1 py-3 text-sm font-bold rounded-xl transition-all ${tab === 'login' ? 'bg-white dark:bg-gray-700 shadow-sm' : 'text-gray-500'}`}
        >
          LOGIN
        </button>
        <button 
          onClick={() => setTab('register')}
          className={`flex-1 py-3 text-sm font-bold rounded-xl transition-all ${tab === 'register' ? 'bg-white dark:bg-gray-700 shadow-sm' : 'text-gray-500'}`}
        >
          REGISTER
        </button>
      </div>

      <form className="space-y-4" onSubmit={tab === 'login' ? handleLogin : handleRegister}>
        {tab === 'register' && (
          <div className="relative">
            <UserIcon className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
            <input type="text" placeholder="Full Name" className="w-full pl-12 pr-4 py-3 bg-gray-50 dark:bg-gray-800 border border-transparent rounded-xl focus:border-blue-500 outline-none" required />
          </div>
        )}
        <div className="relative">
          <Mail className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input type="email" placeholder="Email Address" className="w-full pl-12 pr-4 py-3 bg-gray-50 dark:bg-gray-800 border border-transparent rounded-xl focus:border-blue-500 outline-none" required />
        </div>
        <div className="relative">
          <Lock className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input type="password" placeholder="Password" className="w-full pl-12 pr-4 py-3 bg-gray-50 dark:bg-gray-800 border border-transparent rounded-xl focus:border-blue-500 outline-none" required />
        </div>
        
        <button className="w-full py-4 bg-blue-600 text-white font-black rounded-2xl hover:bg-blue-700 shadow-lg shadow-blue-500/30 transition-all uppercase tracking-widest text-sm">
          {tab === 'login' ? 'Sign In' : 'Create Account'}
        </button>
      </form>

      <div className="relative my-8 text-center">
        <div className="absolute top-1/2 left-0 w-full h-px bg-gray-200 dark:bg-gray-800 -z-10" />
        <span className="bg-white dark:bg-gray-900 px-4 text-xs font-bold text-gray-400">OR CONTINUE WITH</span>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <button className="flex items-center justify-center py-3 border border-gray-200 dark:border-gray-800 rounded-xl hover:bg-gray-50 dark:hover:bg-gray-800 transition-all">
          <Globe className="w-4 h-4 mr-2 text-red-500" /> <span className="text-sm font-bold">Google</span>
        </button>
        <button className="flex items-center justify-center py-3 border border-gray-200 dark:border-gray-800 rounded-xl hover:bg-gray-50 dark:hover:bg-gray-800 transition-all">
          <Facebook className="w-4 h-4 mr-2 text-blue-600" /> <span className="text-sm font-bold">Facebook</span>
        </button>
      </div>
    </div>
  );
};

export default AuthPage;
