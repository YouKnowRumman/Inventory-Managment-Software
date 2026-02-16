const API_URL = import.meta.env.VITE_API_URL;

export const getUsers = async () => {
  const res = await fetch(`${API_URL}/api/users`);
  return res.json();
};
