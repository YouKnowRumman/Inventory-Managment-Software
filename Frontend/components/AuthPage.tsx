
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Mail, Lock, User as UserIcon, Facebook, Globe, CheckCircle, AlertCircle, Loader } from 'lucide-react';
import * as api from '../src/api.js';

interface FormData {
  email: string;
  password: string;
  username?: string;
}

const AuthPage: React.FC<{ lang: string, setUser: any }> = ({ lang, setUser }) => {
  const [tab, setTab] = useState<'login' | 'register'>('login');
  const [confirmed, setConfirmed] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState<FormData>({
    email: '',
    password: '',
    username: ''
  });
  const navigate = useNavigate();

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    setError(null);
  };

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.email || !formData.password) {
      setError('Email and password are required');
      return;
    }

    try {
      setLoading(true);
      setError(null);
      const user = await api.login({
        email: formData.email,
        password: formData.password
      });

      if (user) {
        setUser(user);
        localStorage.setItem('user', JSON.stringify(user));
        navigate('/');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Login failed');
    } finally {
      setLoading(false);
    }
  };

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.email || !formData.password || !formData.username) {
      setError('All fields are required');
      return;
    }

    try {
      setLoading(true);
      setError(null);
      const user = await api.register({
        email: formData.email,
        username: formData.username,
        password: formData.password
      });

      if (user) {
        setUser(user);
        localStorage.setItem('user', JSON.stringify(user));
        navigate('/');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Registration failed');
    } finally {
      setLoading(false);
    }
  };

  if (confirmed) {
    return (
      <div className="max-w-md mx-auto py-20 text-center space-y-4 animate-in zoom-in duration-300">
        <CheckCircle className="w-16 h-16 text-green-500 mx-auto" />
        <h2 className="text-2xl font-bold">Check your email</h2>
        <p className="text-gray-500">We've sent a confirmation link to your email address. Please click it to activate your account.</p>
        <button onClick={() => navigate('/')} className="text-blue-600 font-bold underline">Back to App</button>
      </div>
    );
  }

  return (
    <div className="max-w-md mx-auto bg-white dark:bg-gray-900 rounded-3xl p-8 shadow-xl border border-gray-100 dark:border-gray-800">
      {/* Error Alert */}
      {error && (
        <div className="mb-6 p-4 bg-red-100 dark:bg-red-900/30 border border-red-400 dark:border-red-700 rounded-lg flex items-start space-x-3">
          <AlertCircle className="w-5 h-5 text-red-600 flex-shrink-0 mt-0.5" />
          <span className="text-sm text-red-800 dark:text-red-300">{error}</span>
        </div>
      )}

      <div className="flex p-1 bg-gray-100 dark:bg-gray-800 rounded-2xl mb-8">
        <button 
          onClick={() => setTab('login')}
          disabled={loading}
          className={`flex-1 py-3 text-sm font-bold rounded-xl transition-all ${tab === 'login' ? 'bg-white dark:bg-gray-700 shadow-sm' : 'text-gray-500'} ${loading ? 'opacity-50 cursor-not-allowed' : ''}`}
        >
          LOGIN
        </button>
        <button 
          onClick={() => setTab('register')}
          disabled={loading}
          className={`flex-1 py-3 text-sm font-bold rounded-xl transition-all ${tab === 'register' ? 'bg-white dark:bg-gray-700 shadow-sm' : 'text-gray-500'} ${loading ? 'opacity-50 cursor-not-allowed' : ''}`}
        >
          REGISTER
        </button>
      </div>

      <form className="space-y-4" onSubmit={tab === 'login' ? handleLogin : handleRegister}>
        {tab === 'register' && (
          <div className="relative">
            <UserIcon className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
            <input 
              type="text" 
              name="username"
              placeholder="Full Name" 
              value={formData.username}
              onChange={handleInputChange}
              disabled={loading}
              className="w-full pl-12 pr-4 py-3 bg-gray-50 dark:bg-gray-800 border border-transparent rounded-xl focus:border-blue-500 outline-none disabled:opacity-50"
              required 
            />
          </div>
        )}
        <div className="relative">
          <Mail className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input 
            type="email" 
            name="email"
            placeholder="Email Address" 
            value={formData.email}
            onChange={handleInputChange}
            disabled={loading}
            className="w-full pl-12 pr-4 py-3 bg-gray-50 dark:bg-gray-800 border border-transparent rounded-xl focus:border-blue-500 outline-none disabled:opacity-50"
            required 
          />
        </div>
        <div className="relative">
          <Lock className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input 
            type="password" 
            name="password"
            placeholder="Password" 
            value={formData.password}
            onChange={handleInputChange}
            disabled={loading}
            className="w-full pl-12 pr-4 py-3 bg-gray-50 dark:bg-gray-800 border border-transparent rounded-xl focus:border-blue-500 outline-none disabled:opacity-50"
            required 
          />
        </div>

        <button 
          type="submit"
          disabled={loading}
          className="w-full py-4 bg-blue-600 text-white font-black rounded-2xl hover:bg-blue-700 shadow-lg shadow-blue-500/30 transition-all uppercase tracking-widest text-sm disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center"
        >
          {loading ? (
            <>
              <Loader className="w-4 h-4 mr-2 animate-spin" />
              {tab === 'login' ? 'Signing In...' : 'Creating Account...'}
            </>
          ) : (
            tab === 'login' ? 'Sign In' : 'Create Account'
          )}
        </button>
      </form>

      <div className="relative my-8 text-center">
        <div className="absolute top-1/2 left-0 w-full h-px bg-gray-200 dark:bg-gray-800 -z-10" />
        <span className="bg-white dark:bg-gray-900 px-4 text-xs font-bold text-gray-400">SOCIAL LOGIN - COMING SOON</span>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <button type="button" disabled className="flex items-center justify-center py-3 border border-gray-200 dark:border-gray-800 rounded-xl bg-gray-50 dark:bg-gray-800/30 cursor-not-allowed opacity-60">
          <Globe className="w-4 h-4 mr-2 text-red-500" /> <span className="text-sm font-bold">Google</span>
        </button>
        <button type="button" disabled className="flex items-center justify-center py-3 border border-gray-200 dark:border-gray-800 rounded-xl bg-gray-50 dark:bg-gray-800/30 cursor-not-allowed opacity-60">
          <Facebook className="w-4 h-4 mr-2 text-blue-600" /> <span className="text-sm font-bold">Facebook</span>
        </button>
      </div>
    </div>
  );
};

export default AuthPage;
