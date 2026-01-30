┌────────────────────────────────────────────┐
│           Authorization Request            │
│                                            │
│  UserId + PermissionCode + Resource        │
│  例如 : Content.View + ContentId           │
└────────────────────────────────────────────┘
                      │
                      ▼
┌────────────────────────────────────────────┐
│ Step 1：Role能不能做?                       │
│                                            │
│ User 是否透過角色擁有這個 Permission？       │
│ (Accounts → AccountRoles → RolePermissions)│
└────────────────────────────────────────────┘
            │ Yes                      │ No
            ▼                          ▼
┌────────────────────────────┐   ┌──────────────────┐
│ Step 2：Scope（做到哪？）   │   │      DENY        │
│                            │   │  沒有這個能力     │
└────────────────────────────┘   └──────────────────┘
            │
            ▼
┌────────────────────────────────────────────┐
│ 取得 Scope（RolePermissionScopes）          │
│                                            │
│ Scope = Global / Department / Self         │
└────────────────────────────────────────────┘
            │
            ▼
┌────────────────────────────────────────────┐
│ Scope == Global ?                            │
│                                            │
│  Yes → ALLOW（直接通過）                     │
└────────────────────────────────────────────┘
            │ No
            ▼
┌────────────────────────────────────────────┐
│ Scope == Department ?                      │
│                                            │
│ Content.OwnerDepartmentId                  │
│        == User.PrimaryDepartmentId ?       │
│                                            │
│  Yes → ALLOW                               │
└────────────────────────────────────────────┘
            │ No
            ▼
┌────────────────────────────────────────────┐
│ Scope == Self ?                            │
│                                            │
│ Content.OwnerId == User.AccountId ?        │
│                                            │
│  Yes → ALLOW                               │
└────────────────────────────────────────────┘
            │ No
            ▼
┌────────────────────────────────────────────┐
│ Step 3：Assignment（有沒有被特別指派？）     │
│                                            │
│ 檢查 DepartmentPermissionAssignments       │
│                                            │
│ UserId + PermissionCode +                  │
│ Content.OwnerDepartmentId                  │
│                                            │
│ 並且：                                      │
│ ValidFrom <= Now <= ValidTo                │
└────────────────────────────────────────────┘
            │ Yes                      │ No
            ▼                          ▼
┌──────────────────┐          ┌──────────────────┐
│      ALLOW       │          │       DENY       │
│（例外授權成立）    │          │（完全沒權限）     │
└──────────────────┘          └──────────────────┘