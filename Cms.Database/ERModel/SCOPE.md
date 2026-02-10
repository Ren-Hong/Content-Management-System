┌──────────────┐
│   Scopes     │
│──────────────│
│ ScopeId (PK) │◄──────────────┐
│ ScopeCode    │               │
│ Description  │               │
└──────────────┘               │
                               │
                               │
┌──────────────────────────┐   │
│ RolePermissions          │   ←「策略層」
│──────────────────────────│   │
│ RoleId        (PK, FK) ──┼───┼──────► Roles.RoleId
│ PermissionId  (PK, FK) ──┼───┼──────► Permissions.PermissionId
│ ScopeId       (FK) ──────┼───┘
│ CreatedAt                │
└──────────────────────────┘


┌───────────────────┐
│ Departments       │
│───────────────────│
│ DepartmentId (PK) │◄───────────┐
│ DepartmentCode    │            │
│ DepartmentName    │            │
│ Status            │            │
│ CreatedAt         │            │
│ UpdatedAt         │            │
└───────────────────┘            │
                                 │
                                 │
┌──────────────────────────┐     │
│ AccountDepartments       │     │   ←「人屬於哪」
│──────────────────────────│     │
│ AccountId     (PK, FK) ──┼─────┼────► Accounts.AccountId
│ DepartmentId  (PK, FK) ──┼─────┘
│ IsPrimary                │ ← 是否是本身部門
│ CreatedAt                │
└──────────────────────────┘


┌────────────────────────────────────┐
│ DepartmentPermissionAssignments    │   ←「例外 / 指派 / Delegation」
│────────────────────────────────────│
│ AccountId     (PK, FK) ────────────┼────? Accounts.AccountId
│ DepartmentId  (PK, FK) ────────────┼────? Departments.DepartmentId
│ PermissionId  (PK, FK) ────────────┼────? Permissions.PermissionId
│ ValidFrom                          │
│ ValidTo                            │
│ CreatedAt                          │
└────────────────────────────────────┘