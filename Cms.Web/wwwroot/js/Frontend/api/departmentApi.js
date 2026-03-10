export async function getDepartmentsForSidebar() {
    return await fetch('/api/department/sidebar', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({})
    }).then(r => r.json());
}