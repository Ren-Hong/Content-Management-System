export async function getRoleOptions() {
    return await fetch('api/role/options', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({})
    }).then(r => r.json());
}

export async function getRoleSummaries(payload) {
    return await fetch('/api/role/summaries', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function createRole(payload) {
    return await fetch('/api/role/create', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function updateRole(payload) {
    return await fetch('/api/role/update', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function deleteRole(payload) {
    return await fetch('/api/role/delete', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}
