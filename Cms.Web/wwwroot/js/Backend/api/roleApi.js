export async function getRoleOptions() {
    return await fetch('/Role/GetRoleOptions', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({})
    }).then(r => r.json());
}
