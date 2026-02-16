
import React, { useState, useEffect } from 'react';
import { Package, Pencil, Plus, ExternalLink, AlertCircle, Loader } from 'lucide-react';
import { Language, User } from '../types';
import { TRANSLATIONS } from '../constants';
import { useNavigate } from 'react-router-dom';
import * as api from '../src/api';

const PersonalPage: React.FC<{ lang: Language, user: User | null }> = ({ lang, user }) => {
  const navigate = useNavigate();
  const t = (key: string) => TRANSLATIONS[key]?.[lang] || key;
  const [myInventories, setMyInventories] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!user) {
      setLoading(false);
      return;
    }

    const fetchInventories = async () => {
      try {
        setLoading(true);
        const data = await api.getInventories();
        // Filter inventories created by this user
        const userInventories = data.filter((inv: any) => inv.ownerId === user.id || inv.ownerName === user.name);
        setMyInventories(userInventories);
        setError(null);
      } catch (err) {
        const errorMsg = err instanceof Error ? err.message : 'Failed to load inventories';
        setError(errorMsg);
        console.error('Error fetching inventories:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchInventories();
  }, [user]);

  if (!user) return <div className="text-center py-20">Please login to view your profile.</div>;

  const InventoryList = ({ title, items, icon: Icon }: any) => (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div className="flex items-center space-x-2">
          <Icon className="w-5 h-5 text-blue-600" />
          <h2 className="text-xl font-bold">{title}</h2>
        </div>
        <button className="text-xs font-bold text-blue-600 flex items-center px-3 py-1 bg-blue-50 dark:bg-blue-900/30 rounded-lg hover:bg-blue-100 transition-colors">
          <Plus className="w-3 h-3 mr-1" /> CREATE NEW
        </button>
      </div>
      <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-sm border border-gray-100 dark:border-gray-700 overflow-hidden">
        <table className="w-full text-left">
          <thead className="bg-gray-50 dark:bg-gray-700/50">
            <tr>
              <th className="px-6 py-4 text-sm font-semibold text-gray-500">{t('name')}</th>
              <th className="px-6 py-4 text-sm font-semibold text-gray-500">Items</th>
              <th className="px-6 py-4 text-sm font-semibold text-gray-500">Last Updated</th>
              <th className="px-6 py-4 text-right"></th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100 dark:divide-gray-700">
            {items.length === 0 ? (
              <tr>
                <td colSpan={4} className="px-6 py-8 text-center text-gray-500">
                  No inventories found
                </td>
              </tr>
            ) : (
              items.map((inv: any) => (
                <tr key={inv.id} className="hover:bg-gray-50 dark:hover:bg-gray-700/30 cursor-pointer group" onClick={() => navigate(`/inventory/${inv.id}`)}>
                  <td className="px-6 py-4">
                    <div className="font-semibold text-gray-900 dark:text-white group-hover:text-blue-500 transition-colors">{inv.title}</div>
                  </td>
                  <td className="px-6 py-4">
                    <span className="text-sm text-gray-500">{inv.itemCount || 0}</span>
                  </td>
                  <td className="px-6 py-4">
                     <span className="text-sm text-gray-400">{new Date(inv.updatedAt).toLocaleDateString()}</span>
                  </td>
                  <td className="px-6 py-4 text-right">
                    <div className="flex items-center justify-end space-x-2">
                      <button className="p-2 hover:bg-blue-50 dark:hover:bg-blue-900/20 text-blue-600 rounded-lg opacity-0 group-hover:opacity-100 transition-opacity">
                        <Pencil className="w-4 h-4" />
                      </button>
                      <ExternalLink className="w-4 h-4 text-gray-300" />
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );

  return (
    <div className="space-y-12 max-w-5xl mx-auto">
      {error && (
        <div className="p-4 bg-red-100 dark:bg-red-900/30 border border-red-400 dark:border-red-700 rounded text-red-800 dark:text-red-300 flex items-center">
          <AlertCircle className="w-5 h-5 mr-3 flex-shrink-0" />
          <span>{error}</span>
        </div>
      )}

      <div className="flex items-center space-x-6 p-8 bg-white dark:bg-gray-800 rounded-3xl border border-gray-100 dark:border-gray-700 shadow-sm relative overflow-hidden group">
        <div className="absolute top-0 right-0 p-8 opacity-5">
           <Package className="w-32 h-32" />
        </div>
        <div className="w-24 h-24 rounded-3xl bg-gradient-to-tr from-blue-600 to-indigo-600 flex items-center justify-center text-white text-4xl font-black shadow-xl shadow-blue-500/20">
          {user.name[0]}
        </div>
        <div className="space-y-1">
          <h1 className="text-3xl font-bold">{user.name}</h1>
          <p className="text-gray-500">{user.email}</p>
          <div className="flex items-center space-x-3 pt-2">
            <span className="px-2 py-1 bg-blue-100 dark:bg-blue-900 text-blue-700 dark:text-blue-300 rounded text-[10px] font-black tracking-widest uppercase">{user.role}</span>
            <span className="px-2 py-1 bg-green-100 dark:bg-green-900 text-green-700 dark:text-green-300 rounded text-[10px] font-black tracking-widest uppercase">Verified</span>
          </div>
        </div>
      </div>

      {loading ? (
        <div className="flex items-center justify-center py-20">
          <div className="text-center">
            <Loader className="w-8 h-8 animate-spin text-blue-600 mx-auto mb-4" />
            <p className="text-gray-500 font-bold">Loading inventories...</p>
          </div>
        </div>
      ) : (
        <>
          <InventoryList title={t('myInventories')} items={myInventories} icon={Package} />
        </>
      )}
    </div>
  );
}
};

export default PersonalPage;
