export async function logout() {
    const res = await fetch('/api/account/logout', {
        method: 'POST'
    });

    if (!res.ok) {
        throw new Error('Logout failed');
    }
}