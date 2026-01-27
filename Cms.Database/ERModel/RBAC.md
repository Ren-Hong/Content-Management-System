┌────────────┐
│  username  │
│────────────│
│ AccountId  │
│ username   │
│ Status     │
│ CreatedAt  │
└─────┬──────┘
      │ 1
      │
      │ N
┌─────▼──────────┐
│  AccountRole   │   ← 人可以有多個角色
│────────────────│
│ AccountId (FK) │
│ RoleId (FK)    │
└─────┬──────────┘
      │ N
      │
      │ 1
┌─────▼───────┐
│    Role     │
│─────────────│
│ RoleId      │
│ RoleCode    │  ← Admin / Editor / Reviewer
│ RoleName    │
│ IsSystem    │
└─────┬───────┘
      │ 1
      │
      │ N
┌─────▼─────────────┐
│  RolePermission   │   ← 角色 = 權限集合
│───────────────────│
│ RoleId (FK)       │
│ PermissionId (FK) │
└─────┬─────────────┘
      │ N
      │
      │ 1
┌─────▼───────────┐
│   Permission    │
│─────────────────│
│ PermissionId    │
│ PermissionCode  │  ← Content.Publish
│ Description     │
│ CreatedAt       │
└─────────────────┘