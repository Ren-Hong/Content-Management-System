export async function createContent(payload) {
    return await fetch('/api/content/create', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(response => response.json());
}
