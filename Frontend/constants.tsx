
import { Translation, Category, User } from './types';

export const CATEGORIES: Category[] = ['Equipment', 'Furniture', 'Book', 'Other'];

export const TRANSLATIONS: Translation = {
  searchPlaceholder: { en: "Search inventories...", bn: "ইনভেন্টরি খুঁজুন..." },
  latestInventories: { en: "Latest Inventories", bn: "সাম্প্রতিক ইনভেন্টরি" },
  popularInventories: { en: "Popular Inventories", bn: "জনপ্রিয় ইনভেন্টরি" },
  tags: { en: "Tags", bn: "ট্যাগ" },
  login: { en: "Login", bn: "লগইন" },
  logout: { en: "Logout", bn: "লগআউট" },
  name: { en: "Name", bn: "নাম" },
  description: { en: "Description", bn: "বিবরণ" },
  creator: { en: "Creator", bn: "সৃষ্টিকর্তা" },
  items: { en: "Items", bn: "আইটেম" },
  discussion: { en: "Discussion", bn: "আলোচনা" },
  settings: { en: "Settings", bn: "সেটিংস" },
  customId: { en: "Custom ID", bn: "কাস্টম আইডি" },
  fields: { en: "Fields", bn: "ফিল্ডসমূহ" },
  access: { en: "Access", bn: "অ্যাক্সেস" },
  save: { en: "Save", bn: "সংরক্ষণ করুন" },
  delete: { en: "Delete", bn: "মুছে ফেলুন" },
  edit: { en: "Edit", bn: "সম্পাদনা করুন" },
  userManagement: { en: "User Management", bn: "ব্যবহারকারী ব্যবস্থাপনা" },
  role: { en: "Role", bn: "ভূমিকা" },
  status: { en: "Status", bn: "অবস্থা" },
  actions: { en: "Actions", bn: "পদক্ষেপ" },
  myInventories: { en: "My Inventories", bn: "আমার ইনভেন্টরি" },
  writeAccess: { en: "Write Access", bn: "রাইট অ্যাক্সেস" },
  statistics: { en: "Statistics", bn: "পরিসংখ্যান" },
  export: { en: "Export", bn: "রপ্তানি" },
  category: { en: "Category", bn: "বিভাগ" },
};

// Properly type MOCK_USERS as User[] to fix role/status string incompatibility
export const MOCK_USERS: User[] = [
  { id: '1', name: 'John Doe', email: 'john@example.com', role: 'admin', status: 'active' },
  { id: '2', name: 'Suhasini Dey', email: 'suhasini@example.com', role: 'user', status: 'active' },
  { id: '3', name: 'Rahim Ali', email: 'rahim@example.com', role: 'user', status: 'active' },
  { id: '4', name: 'Alfie Solomons', email: 'alfie@example.com', role: 'user', status: 'active' },
  { id: '5', name: 'Thomas Shelby', email: 'tommy@example.com', role: 'user', status: 'active' },
];

export const MOCK_INVENTORIES = [
  { id: 'inv1', name: 'Vintage Cameras', description: '20th century film cameras.', creatorName: 'John Doe', itemCount: 120, tags: ['Photography'], updatedAt: Date.now(), category: 'Equipment' },
  { id: 'inv2', name: 'Rare Coins', description: 'Ancient Roman currency.', creatorName: 'Rahim Ali', itemCount: 85, tags: ['History'], updatedAt: Date.now(), category: 'Other' },
  { id: 'inv3', name: 'Mechanical Keyboards', description: 'Custom keyboards.', creatorName: 'Suhasini Dey', itemCount: 42, tags: ['Tech'], updatedAt: Date.now(), category: 'Equipment' },
  { id: 'inv4', name: 'Classic Novels', description: 'First editions.', creatorName: 'Thomas Shelby', itemCount: 210, tags: ['Books'], updatedAt: Date.now(), category: 'Book' },
  { id: 'inv5', name: 'Antique Desks', description: 'Victorian era furniture.', creatorName: 'Alfie Solomons', itemCount: 15, tags: ['Antique'], updatedAt: Date.now(), category: 'Furniture' },
];
