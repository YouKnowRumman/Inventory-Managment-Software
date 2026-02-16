const API_URL = import.meta.env.VITE_API_URL;

export async function getInventories() {
    const response = await fetch(`${API_URL}/inventory`, {
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error("Failed to fetch inventories");
    }

    return response.json();
}
