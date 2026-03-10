export async function getContentTypeOptions(payload) {
    return await fetch('/api/contenttype/options', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function createContentType(payload) {
    return await fetch('/api/contenttype/create', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function updateContentType(payload) {
    return await fetch('/api/contenttype/update', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function deleteContentType(payload) {
    return await fetch('/api/contenttype/delete', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}
