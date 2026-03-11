const permissionCodes = new Set(window.frontendAuth?.permissions ?? []);

export function hasPermission(permissionCode) {
    return permissionCodes.has(permissionCode);
}

export function getPermissionCodes() {
    return Array.from(permissionCodes);
}
