
export type Language = 'en' | 'bn';
export type Theme = 'light' | 'dark';

export interface User {
  id: string;
  name: string;
  email: string;
  role: 'admin' | 'user';
  status: 'active' | 'blocked';
}

export type FieldType = 'text' | 'number' | 'multiline' | 'checkbox' | 'date' | 'dropdown';

export interface CustomField {
  id: string;
  type: FieldType;
  label: string;
  description?: string;
  order: number;
  showInTable: boolean;
  options?: string[]; // For dropdown
  validation?: {
    minLength?: number;
    maxLength?: number;
    min?: number;
    max?: number;
    regex?: string;
  };
}

export type Category = 'Equipment' | 'Furniture' | 'Book' | 'Other';

export interface Inventory {
  id: string;
  name: string;
  description: string;
  category: Category;
  isPublic: boolean;
  imageUrl?: string;
  creatorId: string;
  creatorName: string;
  itemCount: number;
  tags: string[];
  fields: CustomField[];
  customIdFormat: string[];
  updatedAt: number;
  version: number; // For optimistic locking
  accessList: string[]; // User IDs with write access
}

export interface Item {
  id: string;
  inventoryId: string;
  customId: string;
  name: string;
  fields: Record<string, any>;
  likes: string[]; // User IDs
  createdAt: number;
  updatedAt: number;
  version: number;
}

export interface Post {
  id: string;
  inventoryId: string;
  userId: string;
  userName: string;
  content: string;
  timestamp: number;
}

export interface Translation {
  [key: string]: {
    en: string;
    bn: string;
  };
}
