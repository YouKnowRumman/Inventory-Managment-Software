
import React from 'react';
import { useNavigate } from 'react-router-dom';
import { TrendingUp, Clock, Hash, ChevronRight, Layers } from 'lucide-react';
import { MOCK_INVENTORIES, TRANSLATIONS } from '../constants';
import { Language } from '../types';

const MainDashboard: React.FC<{ lang: Language }> = ({ lang }) => {
  const navigate = useNavigate();
  const t = (key: string) => TRANSLATIONS[key]?.[lang] || key;

  const tags = Array.from(new Set(MOCK_INVENTORIES.flatMap(i => i.tags)));
  const top5 = [...MOCK_INVENTORIES].sort((a, b) => b.itemCount - a.itemCount).slice(0, 5);

  return (
    <div className="space-y-12 animate-in fade-in slide-in-from-bottom-4 duration-1000">
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-12">
        <div className="lg:col-span-2 space-y-6">
          <div className="flex items-center justify-between pb-4 border-b border-executive-border dark:border-command-elevated">
            <div className="flex items-center space-x-3">
              <Clock className="w-6 h-6 text-executive-brand dark:text-command-brand" />
              <h2 className="text-2xl font-black tracking-tight uppercase">{t('latestInventories')}</h2>
            </div>
            <button className="text-[10px] font-black text-executive-brand dark:text-command-brand hover:underline tracking-widest">VIEW ALL</button>
          </div>
          <div className="bg-executive-surface dark:bg-command-surface rounded border border-executive-border dark:border-command-elevated shadow-executive dark:shadow-none overflow-hidden transition-theme">
            <table className="w-full text-left">
              <thead className="bg-executive-alt dark:bg-command-elevated">
                <tr>
                  <th className="px-8 py-5 text-[10px] font-black text-executive-textSecondary dark:text-command-textSecondary uppercase tracking-widest">{t('name')}</th>
                  <th className="px-8 py-5 text-[10px] font-black text-executive-textSecondary dark:text-command-textSecondary uppercase tracking-widest hidden md:table-cell">{t('description')}</th>
                  <th className="px-8 py-5 text-[10px] font-black text-executive-textSecondary dark:text-command-textSecondary uppercase tracking-widest">{t('creator')}</th>
                  <th className="px-8 py-5"></th>
                </tr>
              </thead>
              <tbody className="divide-y divide-executive-border dark:divide-command-elevated">
                {MOCK_INVENTORIES.map((inv) => (
                  <tr 
                    key={inv.id} 
                    className="hover:bg-executive-alt/50 dark:hover:bg-command-elevated/20 cursor-pointer transition-all group"
                    onClick={() => navigate(`/inventory/${inv.id}`)}
                  >
                    <td className="px-8 py-6">
                      <div className="font-bold text-executive-textPrimary dark:text-command-textPrimary group-hover:text-executive-brand dark:group-hover:text-command-brand transition-colors">{inv.name}</div>
                    </td>
                    <td className="px-8 py-6 hidden md:table-cell">
                      <p className="text-sm text-executive-textSecondary dark:text-command-textSecondary line-clamp-1 italic font-medium">{inv.description}</p>
                    </td>
                    <td className="px-8 py-6">
                      <span className="inline-flex items-center px-3 py-1 rounded border border-executive-border dark:border-command-elevated text-[10px] font-black uppercase tracking-tighter bg-executive-alt dark:bg-command-elevated text-executive-textSecondary dark:text-command-textSecondary">
                        {inv.creatorName}
                      </span>
                    </td>
                    <td className="px-8 py-6 text-right">
                      <ChevronRight className="w-4 h-4 text-executive-border dark:text-command-textSecondary group-hover:text-executive-brand dark:group-hover:text-command-brand group-hover:translate-x-1 transition-all inline" />
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>

        <div className="space-y-10">
          <div className="space-y-6">
            <div className="flex items-center space-x-3 pb-4 border-b border-executive-border dark:border-command-elevated">
              <TrendingUp className="w-6 h-6 text-executive-success dark:text-command-success" />
              <h2 className="text-2xl font-black tracking-tight uppercase">{t('popularInventories')}</h2>
            </div>
            <div className="bg-executive-surface dark:bg-command-surface rounded p-4 border border-executive-border dark:border-command-elevated shadow-executive dark:shadow-none space-y-2 transition-theme">
              {top5.map((inv, idx) => (
                <div 
                  key={inv.id} 
                  className="flex items-center justify-between p-4 rounded hover:bg-executive-alt dark:hover:bg-command-elevated transition-all group cursor-pointer border border-transparent hover:border-executive-border dark:hover:border-command-surface" 
                  onClick={() => navigate(`/inventory/${inv.id}`)}
                >
                  <div className="flex items-center space-x-4">
                    <span className="text-3xl font-black text-executive-brand/20 dark:text-command-brand/30 group-hover:text-executive-brand/40 dark:group-hover:text-command-brand/50 transition-colors font-mono tracking-tighter">
                      0{idx + 1}
                    </span>
                    <span className="font-black text-sm text-executive-textPrimary dark:text-command-textPrimary group-hover:text-executive-brand dark:group-hover:text-command-brand transition-colors uppercase tracking-tight">{inv.name}</span>
                  </div>
                  <div className="flex flex-col items-end">
                    <span className="text-xs font-black text-executive-brand dark:text-command-brand">{inv.itemCount}</span>
                    <span className="text-[8px] font-bold text-executive-textSecondary dark:text-command-textSecondary uppercase tracking-widest">ITEMS</span>
                  </div>
                </div>
              ))}
            </div>
          </div>

          <div className="space-y-6">
            <div className="flex items-center space-x-3 pb-4 border-b border-executive-border dark:border-command-elevated">
              <Hash className="w-6 h-6 text-executive-accent" />
              <h2 className="text-2xl font-black tracking-tight uppercase">{t('tags')}</h2>
            </div>
            <div className="flex flex-wrap gap-2">
              {tags.map((tag) => (
                <button
                  key={tag}
                  className="px-4 py-2 bg-executive-surface dark:bg-command-surface border border-executive-border dark:border-command-elevated rounded text-[10px] font-black uppercase tracking-widest text-executive-textSecondary dark:text-command-textSecondary hover:border-executive-brand hover:text-executive-brand dark:hover:border-command-brand transition-all shadow-sm active:scale-95"
                >
                  {tag}
                </button>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default MainDashboard;
