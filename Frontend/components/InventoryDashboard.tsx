
import React, { useState, useEffect, useCallback } from 'react';
import { useParams } from 'react-router-dom';
import { 
  Package, MessageSquare, Settings as SettingsIcon, Fingerprint, 
  ListChecks, Users, Plus, Trash2, Edit, AlertCircle, 
  GripVertical, Search, BarChart3, Download, ExternalLink, 
  HelpCircle, Heart, Lock, Globe, X, ArrowUpDown, MoreVertical,
  Shield, Image as ImageIcon, TrendingUp as TrendingUpIcon, FileText as FileTextIcon
} from 'lucide-react';
import { motion, AnimatePresence, Reorder } from 'framer-motion';
import { Inventory, User, CustomField, Category } from '../types';
import { MOCK_INVENTORIES, TRANSLATIONS, CATEGORIES, MOCK_USERS } from '../constants';
import ChatRoom from './ChatRoom';

const InventoryDashboard: React.FC<{ lang: 'en' | 'bn', user: User | null }> = ({ lang, user }) => {
  const { id } = useParams();
  const [inventory, setInventory] = useState<Inventory | null>(null);
  const [activeTab, setActiveTab] = useState('items');
  const [isDirty, setIsDirty] = useState(false);
  const [lastSaved, setLastSaved] = useState(Date.now());
  const [items, setItems] = useState<any[]>([]);
  const [editItem, setEditItem] = useState<any>(null);
  const [searchUser, setSearchUser] = useState('');
  
  const t = (key: string) => TRANSLATIONS[key]?.[lang] || key;

  useEffect(() => {
    const inv = MOCK_INVENTORIES.find(i => i.id === id);
    if (inv) {
      setInventory({
        ...inv,
        creatorId: '1',
        category: (inv.category as Category) || 'Equipment',
        isPublic: true,
        version: 1,
        accessList: ['2'],
        fields: [
          { id: 'f1', type: 'text', label: 'Item Name', showInTable: true, order: 0, description: 'The official identifier for the asset.' },
          { id: 'f2', type: 'number', label: 'Asset Value', showInTable: true, order: 1, description: 'Current market value in USD.', validation: { min: 0, max: 1000000 } },
          { id: 'f3', type: 'dropdown', label: 'Operational State', showInTable: true, order: 2, description: 'Technical condition of the unit.', options: ['Optimal', 'Service Needed', 'Decommissioned'] }
        ],
        customIdFormat: ['prefix', 'year', 'seq']
      } as Inventory);
      
      setItems([
        { id: '1', name: 'Nikon F3 High Speed', customId: 'AST-2024-001', fields: { f1: 'Nikon F3 High Speed', f2: 1200, f3: 'Optimal' }, likes: ['1'] },
        { id: '2', name: 'Leica M6 Platinum', customId: 'AST-2024-002', fields: { f1: 'Leica M6 Platinum', f2: 8500, f3: 'Optimal' }, likes: [] },
        { id: '3', name: 'Hasselblad 500C', customId: 'AST-2024-003', fields: { f1: 'Hasselblad 500C', f2: 4500, f3: 'Service Needed' }, likes: ['2', '4'] },
      ]);
    }
  }, [id]);

  const isOwner = user?.id === inventory?.creatorId || user?.role === 'admin';
  const hasWriteAccess = isOwner || inventory?.isPublic || inventory?.accessList.includes(user?.id || '');

  const saveInventory = useCallback(() => {
    if (!inventory) return;
    setInventory(prev => prev ? { ...prev, version: prev.version + 1 } : null);
    setIsDirty(false);
    setLastSaved(Date.now());
  }, [inventory]);

  useEffect(() => {
    if (!isDirty || !hasWriteAccess) return;
    const interval = setInterval(saveInventory, 8500);
    return () => clearInterval(interval);
  }, [isDirty, inventory, hasWriteAccess, saveInventory]);

  if (!inventory) return <div className="text-center py-20 font-bold text-executive-textSecondary">INITIALIZING ARCHIVE...</div>;

  const tabs = [
    { id: 'items', label: t('items'), icon: Package },
    { id: 'chat', label: t('discussion'), icon: MessageSquare },
    ...(isOwner ? [
      { id: 'stats', label: t('statistics'), icon: BarChart3 },
      { id: 'fields', label: t('fields'), icon: ListChecks },
      { id: 'id', label: t('customId'), icon: Fingerprint },
      { id: 'access', label: t('access'), icon: Users },
      { id: 'settings', label: t('settings'), icon: SettingsIcon },
      { id: 'export', label: t('export'), icon: Download }
    ] : [])
  ];

  const handleIdFormatReorder = (newOrder: string[]) => {
    setInventory({ ...inventory, customIdFormat: newOrder });
    setIsDirty(true);
  };

  const handleAddField = () => {
    const newField: CustomField = {
      id: `f${Date.now()}`,
      label: 'New Parameter',
      type: 'text',
      order: inventory.fields.length,
      showInTable: false,
      description: 'System metadata field.'
    };
    setInventory({ ...inventory, fields: [...inventory.fields, newField] });
    setIsDirty(true);
  };

  return (
    <div className="space-y-6 max-w-7xl mx-auto">
      {/* Vault Header */}
      <div className="flex flex-col md:flex-row md:items-end justify-between gap-6 pb-6 border-b border-executive-border dark:border-command-elevated">
        <div className="space-y-2">
          <div className="flex flex-wrap items-center gap-3">
            <h1 className="text-2xl md:text-3xl font-black uppercase tracking-tight text-executive-textPrimary dark:text-command-textPrimary">
              {inventory.name}
            </h1>
            <span className="px-2 py-0.5 bg-executive-brand/5 dark:bg-command-brand/10 text-executive-brand dark:text-command-brand text-[9px] font-black rounded border border-executive-brand/20 dark:border-command-brand/30 uppercase tracking-widest whitespace-nowrap">
              {inventory.category}
            </span>
          </div>
          <p className="text-executive-textSecondary dark:text-command-textSecondary text-sm font-medium max-w-2xl leading-relaxed">
            {inventory.description}
          </p>
        </div>
        
        <div className="flex flex-row md:flex-col items-center md:items-end justify-between md:justify-end gap-3 w-full md:w-auto">
          <AnimatePresence>
            {isDirty && (
              <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0 }}
                className="px-2 py-1 bg-executive-warning/10 text-executive-warning text-[9px] font-bold uppercase rounded border border-executive-warning/30 flex items-center shadow-sm"
              >
                <AlertCircle className="w-3 h-3 mr-1.5" /> Modified
              </motion.div>
            )}
          </AnimatePresence>
          <div className="text-[10px] font-bold text-executive-textSecondary dark:text-command-textSecondary uppercase tracking-widest bg-executive-alt dark:bg-command-elevated px-3 py-1.5 rounded border border-executive-border dark:border-command-elevated shadow-sm flex items-center">
            <div className="w-1.5 h-1.5 rounded-full bg-executive-success dark:bg-command-success mr-2 animate-pulse" />
            Sync v{inventory.version} &bull; {new Date(lastSaved).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})}
          </div>
        </div>
      </div>

      {/* Navigation Tabs - Optimized for Mobile */}
      <div className="overflow-x-auto border-b border-executive-border dark:border-command-elevated no-scrollbar -mx-4 px-4 sm:mx-0 sm:px-0">
        <div className="flex min-w-max gap-1">
          {tabs.map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`flex items-center px-4 py-3 text-[10px] font-black transition-all border-b-2 uppercase tracking-widest ${
                activeTab === tab.id 
                ? 'border-executive-brand text-executive-brand dark:border-command-brand dark:text-command-brand' 
                : 'border-transparent text-executive-textSecondary dark:text-command-textSecondary hover:text-executive-textPrimary dark:hover:text-command-textPrimary'
              }`}
            >
              <tab.icon className="w-3.5 h-3.5 mr-2" />
              {tab.label}
            </button>
          ))}
        </div>
      </div>

      {/* Tab Content Rendering */}
      <div className="py-4">
        <AnimatePresence mode="wait">
          <motion.div key={activeTab} initial={{ opacity: 0, y: 5 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -5 }} transition={{ duration: 0.2 }}>
            
            {/* ITEMS TAB */}
            {activeTab === 'items' && (
              <div className="space-y-4">
                <div className="flex flex-col sm:flex-row items-center justify-between bg-executive-surface dark:bg-command-surface p-3 rounded border border-executive-border dark:border-command-elevated shadow-executive dark:shadow-none gap-3">
                  <div className="flex w-full sm:w-auto space-x-2">
                    {hasWriteAccess && (
                      <button className="flex-1 sm:flex-none flex items-center justify-center px-4 py-2 bg-executive-brand dark:bg-command-brand text-white text-[10px] font-black rounded border border-transparent hover:brightness-110 transition-all uppercase tracking-widest shadow-sm">
                        <Plus className="w-3.5 h-3.5 mr-2" /> Record Entry
                      </button>
                    )}
                  </div>
                  <div className="relative w-full sm:w-64">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-3.5 h-3.5 text-executive-textSecondary/50" />
                    <input type="text" placeholder="Global search..." className="w-full pl-9 pr-4 py-2 text-xs bg-executive-alt dark:bg-command-elevated border border-executive-border dark:border-command-elevated rounded focus:outline-none focus:border-executive-brand dark:focus:border-command-brand transition-all font-medium text-executive-textPrimary dark:text-command-textPrimary" />
                  </div>
                </div>

                <div className="bg-executive-surface dark:bg-command-surface rounded border border-executive-border dark:border-command-elevated overflow-x-auto shadow-executive dark:shadow-none">
                  <table className="w-full text-left min-w-[600px]">
                    <thead className="bg-executive-alt dark:bg-command-elevated">
                      <tr>
                        <th className="px-6 py-4 text-[10px] font-black uppercase text-executive-textSecondary dark:text-command-textSecondary tracking-widest border-b border-executive-border dark:border-command-elevated">Identifier</th>
                        {inventory.fields.filter(f => f.showInTable).map(f => (
                          <th key={f.id} className="px-6 py-4 text-[10px] font-black uppercase text-executive-textSecondary dark:text-command-textSecondary tracking-widest border-b border-executive-border dark:border-command-elevated">{f.label}</th>
                        ))}
                        <th className="px-6 py-4 text-right border-b border-executive-border dark:border-command-elevated"></th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-executive-border dark:divide-command-elevated text-executive-textPrimary dark:text-command-textPrimary font-medium">
                      {items.map(item => (
                        <tr key={item.id} className="hover:bg-executive-alt/30 dark:hover:bg-command-elevated/20 transition-all group" onClick={() => hasWriteAccess && setEditItem(item)}>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <code className="text-[10px] font-mono px-2 py-1 bg-executive-alt dark:bg-command-elevated rounded border border-executive-border dark:border-command-elevated text-executive-brand dark:text-command-brand font-bold">
                              {item.customId}
                            </code>
                          </td>
                          {inventory.fields.filter(f => f.showInTable).map(f => (
                            <td key={f.id} className="px-6 py-4 text-xs">
                              {f.type === 'number' ? `$${item.fields[f.id].toLocaleString()}` : item.fields[f.id]}
                            </td>
                          ))}
                          <td className="px-6 py-4 text-right">
                            <div className="flex items-center justify-end space-x-3 opacity-0 group-hover:opacity-100 transition-opacity">
                              <button className="p-1.5 text-executive-brand dark:text-command-brand hover:bg-executive-brand/10 dark:hover:bg-command-brand/20 rounded"><Edit className="w-3.5 h-3.5" /></button>
                              <button className="p-1.5 text-executive-danger dark:text-command-danger hover:bg-executive-danger/10 dark:hover:bg-command-danger/20 rounded"><Trash2 className="w-3.5 h-3.5" /></button>
                            </div>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            )}

            {/* STATISTICS TAB */}
            {activeTab === 'stats' && (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 animate-in fade-in">
                <div className="bg-executive-surface dark:bg-command-surface p-6 rounded border border-executive-border dark:border-command-elevated shadow-executive dark:shadow-none">
                  <div className="flex items-center justify-between mb-6">
                    <h3 className="text-[10px] font-black uppercase text-executive-textSecondary dark:text-command-textSecondary tracking-widest">Vault Volume</h3>
                    <BarChart3 className="w-4 h-4 text-executive-brand dark:text-command-brand" />
                  </div>
                  <div className="flex items-baseline space-x-2">
                    <span className="text-4xl font-black text-executive-brand dark:text-command-brand">{items.length}</span>
                    <span className="text-[10px] font-black text-executive-textSecondary uppercase tracking-widest">Active Units</span>
                  </div>
                  <div className="mt-8 flex items-center justify-between">
                    <div className="space-y-1">
                      <p className="text-[9px] font-black text-gray-400 uppercase tracking-tighter">Capacity Used</p>
                      <p className="text-xs font-bold">42.5%</p>
                    </div>
                    <div className="w-24 h-1.5 bg-executive-alt dark:bg-command-elevated rounded overflow-hidden">
                      <div className="h-full bg-executive-brand dark:bg-command-brand w-[42%]" />
                    </div>
                  </div>
                </div>

                <div className="bg-executive-surface dark:bg-command-surface p-6 rounded border border-executive-border dark:border-command-elevated shadow-executive dark:shadow-none">
                  <div className="flex items-center justify-between mb-6">
                    <h3 className="text-[10px] font-black uppercase text-executive-textSecondary dark:text-command-textSecondary tracking-widest">Inventory Valuation</h3>
                    <TrendingUpIcon className="w-4 h-4 text-executive-success dark:text-command-success" />
                  </div>
                  <div className="flex items-baseline space-x-2">
                    <span className="text-4xl font-black text-executive-success dark:text-command-success">${items.reduce((acc, it) => acc + (it.fields.f2 || 0), 0).toLocaleString()}</span>
                    <span className="text-[10px] font-black text-executive-textSecondary uppercase tracking-widest">USD Total</span>
                  </div>
                  <p className="mt-8 text-[10px] font-bold text-executive-textSecondary leading-relaxed italic">
                    Aggregated value calculated based on recorded asset acquisition costs.
                  </p>
                </div>

                <div className="bg-executive-surface dark:bg-command-surface p-6 rounded border border-executive-border dark:border-command-elevated shadow-executive dark:shadow-none">
                  <div className="flex items-center justify-between mb-6">
                    <h3 className="text-[10px] font-black uppercase text-executive-textSecondary dark:text-command-textSecondary tracking-widest">Health Index</h3>
                    <Shield className="w-4 h-4 text-executive-accent" />
                  </div>
                  <div className="space-y-4">
                    <div className="flex justify-between items-center border-b border-executive-border dark:border-command-elevated pb-2">
                      <span className="text-[10px] font-black text-executive-textSecondary uppercase">Optimal</span>
                      <span className="text-xs font-black text-executive-success">66%</span>
                    </div>
                    <div className="flex justify-between items-center border-b border-executive-border dark:border-command-elevated pb-2">
                      <span className="text-[10px] font-black text-executive-textSecondary uppercase">Service Req.</span>
                      <span className="text-xs font-black text-executive-warning">33%</span>
                    </div>
                    <div className="flex justify-between items-center">
                      <span className="text-[10px] font-black text-executive-textSecondary uppercase">Offline</span>
                      <span className="text-xs font-black text-gray-400">0%</span>
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* FIELDS TAB */}
            {activeTab === 'fields' && (
              <div className="max-w-3xl space-y-6">
                <div className="flex items-center justify-between bg-executive-surface dark:bg-command-surface p-4 rounded border border-executive-border dark:border-command-elevated shadow-sm">
                  <div>
                    <h2 className="text-sm font-black uppercase tracking-widest">Schema Architect</h2>
                    <p className="text-[10px] font-bold text-executive-textSecondary uppercase mt-1">Configure metadata structures for your inventory entries.</p>
                  </div>
                  <button onClick={handleAddField} className="flex items-center px-4 py-2 bg-executive-brand dark:bg-command-brand text-white text-[10px] font-black rounded hover:brightness-110 transition-all uppercase tracking-widest shadow-sm">
                    <Plus className="w-3.5 h-3.5 mr-2" /> Define Field
                  </button>
                </div>

                <div className="space-y-3">
                  {inventory.fields.map((field, idx) => (
                    <div key={field.id} className="bg-executive-surface dark:bg-command-surface p-4 rounded border border-executive-border dark:border-command-elevated flex flex-col md:flex-row gap-4 items-start md:items-center shadow-executive group">
                      <div className="flex items-center space-x-3 w-full md:w-1/3">
                        <GripVertical className="w-4 h-4 text-gray-300 cursor-grab active:cursor-grabbing" />
                        <div className="flex flex-col flex-1">
                          <input 
                            type="text" 
                            value={field.label}
                            onChange={(e) => {
                              const newFields = [...inventory.fields];
                              newFields[idx].label = e.target.value;
                              setInventory({...inventory, fields: newFields});
                              setIsDirty(true);
                            }}
                            className="text-xs font-black uppercase tracking-widest bg-transparent border-none focus:ring-0 text-executive-textPrimary dark:text-command-textPrimary w-full"
                          />
                          <span className="text-[9px] font-bold text-executive-textSecondary uppercase tracking-tighter px-2 mt-0.5">{field.type} Field</span>
                        </div>
                      </div>
                      
                      <div className="flex-1 flex items-center space-x-6 w-full md:w-auto">
                        <label className="flex items-center text-[10px] font-black text-executive-textSecondary uppercase cursor-pointer select-none">
                          <input 
                            type="checkbox" 
                            checked={field.showInTable} 
                            onChange={(e) => {
                              const newFields = [...inventory.fields];
                              newFields[idx].showInTable = e.target.checked;
                              setInventory({...inventory, fields: newFields});
                              setIsDirty(true);
                            }}
                            className="mr-2 rounded border-executive-border text-executive-brand focus:ring-executive-brand" 
                          /> 
                          Visible in Grid
                        </label>
                        <select className="bg-executive-alt dark:bg-command-elevated border border-executive-border dark:border-command-elevated rounded px-3 py-1.5 text-[10px] font-black uppercase tracking-widest focus:outline-none text-executive-textSecondary dark:text-command-textSecondary">
                          <option value="text">STRING</option>
                          <option value="number">NUMERIC</option>
                          <option value="dropdown">PICKLIST</option>
                          <option value="multiline">LONG TEXT</option>
                        </select>
                      </div>

                      <button 
                        onClick={() => {
                          setInventory({...inventory, fields: inventory.fields.filter((_, i) => i !== idx)});
                          setIsDirty(true);
                        }}
                        className="p-2 text-gray-300 hover:text-executive-danger transition-colors opacity-0 group-hover:opacity-100"
                      >
                        <Trash2 className="w-3.5 h-3.5" />
                      </button>
                    </div>
                  ))}
                </div>
              </div>
            )}

            {/* EXPORT TAB */}
            {activeTab === 'export' && (
              <div className="max-w-xl bg-executive-surface dark:bg-command-surface p-10 rounded border border-executive-border dark:border-command-elevated shadow-executive dark:shadow-none text-center space-y-8 animate-in zoom-in-95">
                <div className="w-16 h-16 bg-executive-brand/10 dark:bg-command-brand/20 text-executive-brand dark:text-command-brand mx-auto rounded flex items-center justify-center border border-executive-brand/20">
                  <Download className="w-8 h-8" />
                </div>
                <div>
                  <h2 className="text-xl font-black uppercase tracking-tight">Extract Archival Data</h2>
                  <p className="text-executive-textSecondary dark:text-command-textSecondary text-xs mt-3 leading-relaxed font-medium uppercase tracking-widest">
                    Generate standard file formats for external compliance audits or secondary operational processing.
                  </p>
                </div>
                <div className="grid grid-cols-2 gap-4 pt-4">
                  <button className="flex flex-col items-center justify-center p-6 bg-executive-alt dark:bg-command-elevated border border-executive-border dark:border-command-elevated rounded hover:border-executive-brand dark:hover:border-command-brand transition-all group shadow-sm">
                    <FileTextIcon className="w-6 h-6 mb-3 text-executive-textSecondary group-hover:text-executive-brand" />
                    <span className="text-[10px] font-black uppercase tracking-[0.2em] text-executive-textSecondary group-hover:text-executive-brand">ARCHIVE.CSV</span>
                  </button>
                  <button className="flex flex-col items-center justify-center p-6 bg-executive-alt dark:bg-command-elevated border border-executive-border dark:border-command-elevated rounded hover:border-executive-brand dark:hover:border-command-brand transition-all group shadow-sm">
                    <ImageIcon className="w-6 h-6 mb-3 text-executive-textSecondary group-hover:text-executive-brand" />
                    <span className="text-[10px] font-black uppercase tracking-[0.2em] text-executive-textSecondary group-hover:text-executive-brand">CATALOGUE.JSON</span>
                  </button>
                </div>
              </div>
            )}

            {/* ID BUILDER TAB */}
            {activeTab === 'id' && (
              <div className="max-w-xl bg-executive-surface dark:bg-command-surface p-8 rounded border border-executive-border dark:border-command-elevated shadow-executive dark:shadow-none space-y-8">
                <div className="flex items-center justify-between border-b border-executive-border dark:border-command-elevated pb-4">
                  <h2 className="text-lg font-black uppercase tracking-widest flex items-center">
                    <Fingerprint className="w-5 h-5 mr-3 text-executive-brand dark:text-command-brand" /> ID Protocol Architect
                  </h2>
                  <div className="group relative">
                    <HelpCircle className="w-4 h-4 text-executive-textSecondary cursor-help hover:text-executive-brand" />
                    <div className="absolute right-0 top-full mt-3 w-64 p-4 bg-executive-textPrimary text-white text-[9px] font-medium leading-relaxed rounded shadow-2xl opacity-0 group-hover:opacity-100 transition-all pointer-events-none z-50 uppercase tracking-widest border border-white/10">
                      Rearrange logical components to define the generation rule for system-wide asset identifiers.
                    </div>
                  </div>
                </div>
                
                <div className="p-10 bg-executive-alt dark:bg-command-elevated border-2 border-dashed border-executive-border dark:border-command-elevated rounded text-center shadow-inner">
                  <span className="text-3xl font-mono font-black tracking-[0.2em] text-executive-brand dark:text-command-brand">
                    {inventory.customIdFormat.map(part => part.toUpperCase().slice(0, 3)).join('-')}
                  </span>
                </div>
                
                <div className="space-y-4">
                  <span className="text-[10px] font-black uppercase text-executive-textSecondary dark:text-command-textSecondary tracking-widest block">Operational Rule Sequence</span>
                  <Reorder.Group axis="y" values={inventory.customIdFormat} onReorder={handleIdFormatReorder} className="space-y-2">
                    {inventory.customIdFormat.map(part => (
                      <Reorder.Item key={part} value={part} className="flex items-center justify-between p-3 bg-executive-surface dark:bg-command-surface border border-executive-border dark:border-command-elevated rounded cursor-grab active:cursor-grabbing group shadow-sm">
                        <div className="flex items-center">
                          <GripVertical className="w-4 h-4 mr-3 text-gray-300" />
                          <span className="text-[10px] font-black uppercase tracking-widest">{part}</span>
                        </div>
                        <button onClick={() => { setInventory({...inventory, customIdFormat: inventory.customIdFormat.filter(p => p !== part)}); setIsDirty(true); }} className="p-1 text-gray-300 hover:text-executive-danger transition-colors">
                          <Trash2 className="w-4 h-4" />
                        </button>
                      </Reorder.Item>
                    ))}
                  </Reorder.Group>
                  <button onClick={() => { setInventory({...inventory, customIdFormat: [...inventory.customIdFormat, 'type']}); setIsDirty(true); }} className="w-full py-3 border border-dashed border-executive-border dark:border-command-elevated rounded text-[10px] font-black uppercase tracking-widest text-executive-textSecondary hover:text-executive-brand hover:border-executive-brand transition-all">
                    + Insert Identity Parameter
                  </button>
                </div>
              </div>
            )}

            {/* ACCESS TAB */}
            {activeTab === 'access' && (
              <div className="max-w-xl bg-executive-surface dark:bg-command-surface p-8 rounded border border-executive-border dark:border-command-elevated shadow-executive dark:shadow-none space-y-6">
                <h2 className="text-lg font-black uppercase tracking-widest mb-2 flex items-center border-b border-executive-border dark:border-command-elevated pb-4">
                  <Users className="w-5 h-5 mr-3 text-executive-brand dark:text-command-brand" /> Authorization Control
                </h2>
                
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-executive-textSecondary/50" />
                  <input type="text" placeholder="Authorized user email..." value={searchUser} onChange={e => setSearchUser(e.target.value)} className="w-full pl-10 pr-4 py-3 bg-executive-alt dark:bg-command-elevated border border-executive-border dark:border-command-elevated rounded text-xs font-bold focus:outline-none focus:border-executive-brand transition-all placeholder:text-gray-400" />
                  <AnimatePresence>
                    {searchUser.length > 2 && (
                      <motion.div initial={{ opacity: 0, y: 5 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0 }} className="absolute top-full left-0 right-0 mt-2 bg-executive-surface dark:bg-command-surface border border-executive-border dark:border-command-elevated rounded shadow-2xl z-[70] max-h-48 overflow-y-auto custom-scrollbar">
                        {MOCK_USERS.filter(u => u.name.toLowerCase().includes(searchUser.toLowerCase()) || u.email.toLowerCase().includes(searchUser.toLowerCase())).map(u => (
                          <button key={u.id} onClick={() => { setInventory({...inventory, accessList: [...inventory.accessList, u.id]}); setSearchUser(''); setIsDirty(true); }} className="w-full flex items-center p-3 hover:bg-executive-alt dark:hover:bg-command-elevated text-left transition-colors border-b border-executive-border dark:border-command-elevated last:border-0">
                            <div className="w-8 h-8 bg-executive-brand text-white rounded flex items-center justify-center font-bold text-xs mr-3 shrink-0">{u.name[0]}</div>
                            <div>
                              <p className="text-xs font-bold text-executive-textPrimary dark:text-command-textPrimary">{u.name}</p>
                              <p className="text-[9px] text-executive-textSecondary uppercase tracking-tighter">{u.email}</p>
                            </div>
                          </button>
                        ))}
                      </motion.div>
                    )}
                  </AnimatePresence>
                </div>

                <div className="space-y-4 pt-4">
                  <span className="text-[10px] font-black uppercase text-executive-textSecondary dark:text-command-textSecondary tracking-widest block">Permission Group</span>
                  <div className="divide-y divide-executive-border dark:divide-command-elevated border border-executive-border dark:border-command-elevated rounded overflow-hidden">
                    <div className="flex items-center justify-between p-4 bg-executive-alt/50 dark:bg-command-elevated/30">
                      <div className="flex items-center">
                        <div className="w-9 h-9 bg-executive-brand text-white rounded flex items-center justify-center font-black text-xs shadow-sm">J</div>
                        <div className="ml-3">
                          <p className="text-xs font-bold text-executive-textPrimary dark:text-command-textPrimary">John Doe</p>
                          <p className="text-[9px] text-executive-textSecondary uppercase tracking-widest">Architect / Lead</p>
                        </div>
                      </div>
                      <span className="px-2 py-0.5 bg-executive-accent/10 text-executive-accent text-[9px] font-black rounded border border-executive-accent/20 uppercase">OWNER</span>
                    </div>
                    {inventory.accessList.map(uid => {
                      const u = MOCK_USERS.find(m => m.id === uid);
                      if (!u) return null;
                      return (
                        <div key={uid} className="flex items-center justify-between p-4 group bg-executive-surface dark:bg-command-surface">
                          <div className="flex items-center">
                            <div className="w-9 h-9 bg-executive-alt dark:bg-command-elevated text-executive-textSecondary rounded flex items-center justify-center font-black text-xs border border-executive-border dark:border-command-elevated group-hover:bg-executive-brand group-hover:text-white transition-all">{u.name[0]}</div>
                            <div className="ml-3">
                              <p className="text-xs font-bold text-executive-textPrimary dark:text-command-textPrimary">{u.name}</p>
                              <p className="text-[9px] text-executive-textSecondary uppercase tracking-tighter">{u.email}</p>
                            </div>
                          </div>
                          <div className="flex items-center space-x-3">
                            <span className="px-2 py-0.5 bg-executive-brand/10 text-executive-brand text-[9px] font-black rounded border border-executive-brand/20 uppercase tracking-widest">WRITER</span>
                            <button onClick={() => { setInventory({...inventory, accessList: inventory.accessList.filter(id => id !== uid)}); setIsDirty(true); }} className="text-executive-textSecondary hover:text-executive-danger opacity-0 group-hover:opacity-100 transition-all p-1">
                              <X className="w-3.5 h-3.5" />
                            </button>
                          </div>
                        </div>
                      );
                    })}
                  </div>
                </div>
              </div>
            )}

            {/* SETTINGS TAB */}
            {activeTab === 'settings' && (
              <div className="max-w-3xl space-y-6 animate-in fade-in slide-in-from-bottom-2">
                <div className="bg-executive-surface dark:bg-command-surface p-8 rounded border border-executive-border dark:border-command-elevated shadow-executive dark:shadow-none space-y-8">
                  <h2 className="text-sm font-black uppercase tracking-widest border-b border-executive-border dark:border-command-surface pb-4 flex items-center">
                    <SettingsIcon className="w-5 h-5 mr-3 text-executive-brand dark:text-command-brand" /> Vault Metadata Configuration
                  </h2>
                  
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <div className="space-y-2">
                      <label className="text-[10px] font-black uppercase text-executive-textSecondary dark:text-command-textSecondary tracking-widest">Global Label</label>
                      <input 
                        type="text" 
                        value={inventory.name}
                        onChange={e => { setInventory({...inventory, name: e.target.value}); setIsDirty(true); }}
                        className="w-full px-4 py-2.5 bg-executive-alt dark:bg-command-elevated border border-executive-border dark:border-command-surface rounded text-sm font-bold text-executive-textPrimary dark:text-command-textPrimary focus:border-executive-brand outline-none transition-all shadow-sm"
                      />
                    </div>
                    <div className="space-y-2">
                      <label className="text-[10px] font-black uppercase text-executive-textSecondary dark:text-command-textSecondary tracking-widest">Primary Classification</label>
                      <select 
                        value={inventory.category}
                        onChange={e => { setInventory({...inventory, category: e.target.value as Category}); setIsDirty(true); }}
                        className="w-full px-4 py-2.5 bg-executive-alt dark:bg-command-elevated border border-executive-border dark:border-command-surface rounded text-sm font-bold text-executive-textPrimary dark:text-command-textPrimary focus:border-executive-brand outline-none transition-all shadow-sm"
                      >
                        {CATEGORIES.map(c => <option key={c} value={c}>{c}</option>)}
                      </select>
                    </div>
                  </div>

                  <div className="space-y-2">
                    <label className="text-[10px] font-black uppercase text-executive-textSecondary dark:text-command-textSecondary tracking-widest">Executive Summary (Markdown Supported)</label>
                    <textarea 
                      rows={5}
                      value={inventory.description}
                      onChange={e => { setInventory({...inventory, description: e.target.value}); setIsDirty(true); }}
                      className="w-full px-4 py-3 bg-executive-alt dark:bg-command-elevated border border-executive-border dark:border-command-surface rounded text-sm font-medium text-executive-textPrimary dark:text-command-textPrimary focus:border-executive-brand outline-none transition-all shadow-sm custom-scrollbar leading-relaxed"
                    />
                  </div>

                  <div className="p-5 bg-executive-alt dark:bg-command-elevated rounded border border-executive-border dark:border-command-elevated flex items-center justify-between">
                    <div className="space-y-1">
                      <h4 className="text-[11px] font-black uppercase tracking-tight text-executive-textPrimary dark:text-command-textPrimary">Public Record Authorization</h4>
                      <p className="text-[9px] text-executive-textSecondary dark:text-command-textSecondary uppercase tracking-widest font-bold">Allow non-authorized verified users to submit entries.</p>
                    </div>
                    <button 
                      onClick={() => { setInventory({...inventory, isPublic: !inventory.isPublic}); setIsDirty(true); }}
                      className={`relative w-12 h-6 rounded-full transition-colors shadow-inner ${inventory.isPublic ? 'bg-executive-success dark:bg-command-success' : 'bg-executive-textSecondary dark:bg-command-elevated'}`}
                    >
                      <div className={`absolute top-1 left-1 w-4 h-4 bg-white rounded-full transition-transform ${inventory.isPublic ? 'translate-x-6' : ''}`} />
                    </button>
                  </div>
                </div>

                <div className="bg-executive-surface dark:bg-command-surface p-8 rounded border border-executive-danger/30 dark:border-command-danger/30 shadow-executive dark:shadow-none space-y-6">
                  <div className="flex items-center space-x-3 text-executive-danger dark:text-command-danger">
                    <Shield className="w-5 h-5" />
                    <h2 className="text-xs font-black uppercase tracking-widest">Secure Decommissioning</h2>
                  </div>
                  <p className="text-[10px] font-bold text-executive-textSecondary dark:text-command-textSecondary uppercase tracking-widest leading-relaxed">
                    Permanently purge this inventory vault and all associated archival data. This operation is irreversible and audited.
                  </p>
                  <button className="px-6 py-3 bg-executive-danger/10 hover:bg-executive-danger text-executive-danger hover:text-white text-[10px] font-black uppercase rounded border border-executive-danger/30 transition-all tracking-widest">
                    Execute Permanent Purge
                  </button>
                </div>
              </div>
            )}

            {activeTab === 'chat' && <ChatRoom inventoryId={inventory.id} user={user} />}
          </motion.div>
        </AnimatePresence>
      </div>

      {/* Item Modal ( disciplined styling ) */}
      <AnimatePresence>
        {editItem && (
          <div className="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-executive-textPrimary/40 dark:bg-black/60 backdrop-blur-sm">
            <motion.div initial={{ scale: 0.98, opacity: 0 }} animate={{ scale: 1, opacity: 1 }} exit={{ scale: 0.98, opacity: 0 }}
              className="bg-executive-surface dark:bg-command-surface w-full max-w-xl rounded border border-executive-border dark:border-command-elevated shadow-2xl overflow-hidden"
            >
              <div className="p-6 border-b border-executive-border dark:border-command-elevated flex justify-between items-center bg-executive-alt dark:bg-command-elevated">
                <h3 className="text-[11px] font-black uppercase tracking-widest flex items-center text-executive-textPrimary dark:text-command-textPrimary">
                  <Edit className="w-4 h-4 mr-3 text-executive-brand" /> Modify Archival Entry
                </h3>
                <button onClick={() => setEditItem(null)} className="p-1.5 hover:bg-executive-border dark:hover:bg-command-surface rounded transition-colors">
                  <X className="w-4 h-4 text-executive-textSecondary" />
                </button>
              </div>
              <div className="p-8 space-y-6 max-h-[70vh] overflow-y-auto custom-scrollbar">
                {inventory.fields.map(f => (
                  <div key={f.id} className="space-y-2">
                    <label className="text-[10px] font-black uppercase text-executive-textSecondary dark:text-command-textSecondary tracking-widest">{f.label}</label>
                    <input 
                      type={f.type === 'number' ? 'number' : 'text'} 
                      defaultValue={editItem.fields[f.id]}
                      className="w-full px-4 py-2.5 bg-executive-alt dark:bg-command-elevated border border-executive-border dark:border-command-elevated rounded text-sm font-bold text-executive-textPrimary dark:text-command-textPrimary focus:border-executive-brand dark:focus:border-command-brand outline-none transition-all shadow-sm" 
                    />
                    {f.description && <p className="text-[9px] text-gray-400 italic font-medium">Protocol: {f.description}</p>}
                  </div>
                ))}
              </div>
              <div className="p-6 bg-executive-alt dark:bg-command-elevated border-t border-executive-border dark:border-command-elevated flex justify-end space-x-3">
                <button onClick={() => setEditItem(null)} className="px-5 py-2 text-[10px] font-black uppercase tracking-widest text-executive-textSecondary hover:text-executive-textPrimary transition-colors">Abort</button>
                <button onClick={() => setEditItem(null)} className="px-8 py-2.5 bg-executive-brand dark:bg-command-brand text-white text-[10px] font-black uppercase rounded shadow-lg shadow-executive-brand/20 transition-all hover:brightness-110 tracking-[0.2em]">Update Archival</button>
              </div>
            </motion.div>
          </div>
        )}
      </AnimatePresence>
    </div>
  );
};

export default InventoryDashboard;
