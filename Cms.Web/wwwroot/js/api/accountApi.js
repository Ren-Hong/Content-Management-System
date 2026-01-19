export async function getAccountSummaries() {
    return await fetch('/Account/GetAccountSummaries', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({})
    }).then(r => r.json());
}

export async function createAccount(payload) {
    return await fetch('/Account/CreateAccount', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}