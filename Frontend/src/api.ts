// API Client for communicating with the backend
const API_URL = import.meta.env.VITE_API_URL || 'https://inventory-managment-software.onrender.com/api';

// Helper to handle API responses
async function handleResponse(response: Response) {
  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: 'Unknown error' }));
    throw new Error(error.message || `API Error: ${response.status}`);
  }
  return response.json().catch(() => null);
}

// ========================
// AUTHENTICATION ENDPOINTS
// ========================

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  username: string;
  password: string;
}

export interface AuthResponse {
  id: string;
  email: string;
  userName: string;
  isAdmin: boolean;
}

export async function login(data: LoginRequest) {
  const response = await fetch(`${API_URL}/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(data),
  });
  const result = await handleResponse(response);
  return result?.user || result;
}

export async function register(data: RegisterRequest) {
  const response = await fetch(`${API_URL}/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(data),
  });
  const result = await handleResponse(response);
  return result?.user || result;
}

export async function logout() {
  try {
    await fetch(`${API_URL}/auth/logout`, {
      method: 'POST',
      credentials: 'include',
    });
  } catch (err) {
    console.error('Logout error:', err);
  }
}

export async function getCurrentUser() {
  const response = await fetch(`${API_URL}/auth/me`, {
    credentials: 'include',
  });
  if (!response.ok) return null;
  return handleResponse(response);
}

// ========================
// INVENTORY ENDPOINTS
// ========================

export async function getInventories() {
  const response = await fetch(`${API_URL}/inventory`, {
    credentials: 'include',
  });
  return handleResponse(response);
}

export async function getInventoryById(id: string) {
  const response = await fetch(`${API_URL}/inventory/${id}`, {
    credentials: 'include',
  });
  return handleResponse(response);
}

export async function createInventory(data: any) {
  const response = await fetch(`${API_URL}/inventory`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(data),
  });
  return handleResponse(response);
}

export async function updateInventory(id: string, data: any) {
  const response = await fetch(`${API_URL}/inventory/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(data),
  });
  return handleResponse(response);
}

export async function deleteInventory(id: string) {
  const response = await fetch(`${API_URL}/inventory/${id}`, {
    method: 'DELETE',
    credentials: 'include',
  });
  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: 'Unknown error' }));
    throw new Error(error.message || `API Error: ${response.status}`);
  }
  return response.status === 204 || handleResponse(response);
}

// ========================
// ITEM ENDPOINTS
// ========================

export async function getItems(inventoryId: string) {
  const response = await fetch(`${API_URL}/item?inventoryId=${inventoryId}`, {
    credentials: 'include',
  });
  return handleResponse(response);
}

export async function getItemById(id: string) {
  const response = await fetch(`${API_URL}/item/${id}`, {
    credentials: 'include',
  });
  return handleResponse(response);
}

export async function createItem(data: any) {
  const response = await fetch(`${API_URL}/item`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(data),
  });
  return handleResponse(response);
}

export async function updateItem(id: string, data: any) {
  const response = await fetch(`${API_URL}/item/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(data),
  });
  return handleResponse(response);
}

export async function deleteItem(id: string) {
  const response = await fetch(`${API_URL}/item/${id}`, {
    method: 'DELETE',
    credentials: 'include',
  });
  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: 'Unknown error' }));
    throw new Error(error.message || `API Error: ${response.status}`);
  }
  return response.status === 204 || handleResponse(response);
}

// ========================
// COMMENT ENDPOINTS
// ========================

export async function getComments(itemId?: string, inventoryId?: string) {
  const params = new URLSearchParams();
  if (itemId) params.append('itemId', itemId);
  if (inventoryId) params.append('inventoryId', inventoryId);
  
  const response = await fetch(`${API_URL}/comments?${params}`, {
    credentials: 'include',
  });
  return handleResponse(response);
}

export async function createComment(data: any) {
  const response = await fetch(`${API_URL}/comments`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(data),
  });
  return handleResponse(response);
}

export async function deleteComment(id: string) {
  const response = await fetch(`${API_URL}/comments/${id}`, {
    method: 'DELETE',
    credentials: 'include',
  });
  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: 'Unknown error' }));
    throw new Error(error.message || `API Error: ${response.status}`);
  }
  return response.status === 204 || handleResponse(response);
}

// ========================
// LIKE ENDPOINTS
// ========================

export async function likeItem(itemId: string) {
  const response = await fetch(`${API_URL}/likes`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({ itemId }),
  });
  return handleResponse(response);
}

export async function unlikeItem(itemId: string) {
  const response = await fetch(`${API_URL}/likes/${itemId}`, {
    method: 'DELETE',
    credentials: 'include',
  });
  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: 'Unknown error' }));
    throw new Error(error.message || `API Error: ${response.status}`);
  }
  return response.status === 204 || handleResponse(response);
}

// ========================
// SEARCH ENDPOINTS
// ========================

export async function searchInventories(query: string) {
  const response = await fetch(`${API_URL}/search?query=${encodeURIComponent(query)}`, {
    credentials: 'include',
  });
  return handleResponse(response);
}

export async function advancedSearch(filters: any) {
  const response = await fetch(`${API_URL}/AdvancedSearch`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(filters),
  });
  return handleResponse(response);
}

// ========================
// STATISTICS ENDPOINTS
// ========================

export async function getStatistics(inventoryId: string) {
  const response = await fetch(`${API_URL}/statistics/${inventoryId}`, {
    credentials: 'include',
  });
  return handleResponse(response);
}

// ========================
// CUSTOM ID ENDPOINTS
// ========================

export async function generateCustomId(inventoryId: string, template?: string) {
  const response = await fetch(`${API_URL}/CustomId/generate`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({ inventoryId, template }),
  });
  return handleResponse(response);
}
