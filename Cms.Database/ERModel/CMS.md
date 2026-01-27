┌──────────────────────────┐
│        ContentType       │   ← 內容種類（文章 / 頁面 / 公告）
│──────────────────────────│
│ TypeId (PK)              │
│ TypeCode                 │
│ TypeName                 │
│ Description              │
│ IsEnabled                │
│ CreatedAt                │
└─────────────┬────────────┘
              │ 1
              │
              │ N
┌─────────────▼──────────────┐
│     ContentTypeField       │   ← 內容類型有哪些欄位（結構）
│────────────────────────────│
│ TypeId (FK)                │
│ FieldId (FK)               │
│ SortOrder                  │
└─────────────┬──────────────┘
              │ N
              │
              │ 1
┌─────────────▼──────────────┐
│        ContentField        │   ← 欄位定義（title / body）
│────────────────────────────│
│ FieldId (PK)               │
│ FieldCode                  │
│ FieldType                  │   ← text richtext number boolean datetime enum reference
│ IsRequired                 │
│ SortOrder                  │
│ CreatedAt                  │
└─────────────┬──────────────┘
              │ 1
              │
              │ N
┌─────────────▼──────────────┐
│    ContentFieldValue       │   ← 真正的資料值
│────────────────────────────│
│ RevisionId (FK)            │
│ FieldId (FK)               │
│ FieldValue                 │
└─────────────┬──────────────┘
              │ N
              │
              │ 1
┌─────────────▼──────────────┐
│     ContentRevision        │   ← 每次修改一個版本
│────────────────────────────│
│ RevisionId (PK)            │
│ ContentId (FK)             │
│ Version                    │
│ CreatedAt                  │
└─────────────┬──────────────┘
              │ N
              │
              │ 1
┌─────────────▼──────────────┐
│          Content           │   ← 一筆實際內容（不含文字）
│────────────────────────────│
│ ContentId (PK)             │
│ TypeId (FK)                │
│ Status                     │  Draft / Published
│ OwnerId (FK)               │  → Accounts.AccountId
│ CreatedAt                  │
└────────────────────────────┘
