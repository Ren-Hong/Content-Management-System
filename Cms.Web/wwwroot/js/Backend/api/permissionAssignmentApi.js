export async function getPermissionAssignmentSummaries() {
    return await fetch('/api/PermissionAssignment/summaries', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({})
    }).then(r => r.json());
}

export async function createPermissionAssignment(payload) {
    return await fetch('/api/permissionAssignment/create', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}