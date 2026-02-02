export async function getPermissionAssignmentSummaries() {
    return await fetch('/api/PermissionAssignment/summaries', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({})
    }).then(r => r.json());
}