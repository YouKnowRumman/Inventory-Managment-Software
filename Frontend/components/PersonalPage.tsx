
import React from 'react';
import { Package, Pencil, Plus, ExternalLink } from 'lucide-react';
import { Language, User } from '../types';
import { MOCK_INVENTORIES, TRANSLATIONS } from '../constants';
import { useNavigate } from 'react-router-dom';

const PersonalPage: React.FC<{ lang: Language, user: User | null }> = ({ lang, user }) => {
  const navigate = useNavigate();
  const t = (key: string) => TRANSLATIONS[key]?.[lang] || key;

  if (!user) return <div className="text-center py-20">Please login to view your profile.</div>;

  const myInventories = MOCK_INVENTORIES.filter(i => i.creatorName === user.name);
  const sharedInventories = MOCK_INVENTORIES.slice(0, 1); // Mock shared access

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
            {items.map((inv: any) => (
              <tr key={inv.id} className="hover:bg-gray-50 dark:hover:bg-gray-700/30 cursor-pointer group" onClick={() => navigate(`/inventory/${inv.id}`)}>
                <td className="px-6 py-4">
                  <div className="font-semibold text-gray-900 dark:text-white group-hover:text-blue-500 transition-colors">{inv.name}</div>
                </td>
                <td className="px-6 py-4">
                  <span className="text-sm text-gray-500">{inv.itemCount}</span>
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
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );

  return (
    <div className="space-y-12 max-w-5xl mx-auto">
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

      <InventoryList title={t('myInventories')} items={myInventories} icon={Package} />
      <InventoryList title={t('writeAccess')} items={sharedInventories} icon={Pencil} />
    </div>
  );
};

export default PersonalPage;
