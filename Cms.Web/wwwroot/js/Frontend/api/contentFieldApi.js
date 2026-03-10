export async function getContentFields(typeId) {
    return await fetch(`/api/contenttype/${typeId}/fields`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    }).then(r => r.json());
}