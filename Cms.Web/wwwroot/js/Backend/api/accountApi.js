export async function getAccountSummaries(payload) {
    return await fetch('/api/account/summaries', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function createAccount(payload) {
    return await fetch('/api/account/create', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function updateAccount(payload) {
    return await fetch('/api/account/update', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function resetPassword(payload) {
    return await fetch('/api/account/resetPassword', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function deleteAccount(payload) {
    return await fetch('/api/account/delete', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function logout() {
    const res = await fetch('/api/account/logout', {
        method: 'POST'
    });

    if (!res.ok) {
        throw new Error('Logout failed');
    }
}