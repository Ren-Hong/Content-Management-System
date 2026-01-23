export async function getPermissionOptions() {
    return await fetch('api/permission/options', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({})
    }).then(r => r.json());
}
