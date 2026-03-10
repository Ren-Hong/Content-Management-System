┌──────────────────────────┐                     ┌──────────────────────────┐
│      DataStructure       │                     │       DataEntities       │
│   這種類型的內容長什麼樣子   │                     │    儲存使用者送的表單資料    │
└──────────────────────────┘                     └──────────────────────────┘

┌──────────────────┐                                ┌──────────────────┐
│   ContentType    │                                │      Content     │
├──────────────────┤                                ├──────────────────┤
│ PK TypeId        │                                │ PK ContentId     │
│ TypeCode         │                                │ FK TypeId        │
│ TypeName         │                                │                  │
│ DepartmentId     │                                │  Status          │
│ Description      │                                │ CreatedAt        │
│ IsEnabled        │                                └─────────┬────────┘
└─────────┬────────┘                                          │ 1
          │ 1                                                 │
          │                                                   │ N
          │ N                                       ┌─────────▼────────┐
┌─────────▼────────┐                                │ ContentRevision  │
│ ContentTypeField │                                ├──────────────────┤
├──────────────────┤                                │ PK RevisionId    │
│ PK (TypeId,      │                                │ FK ContentId     │
│     FieldId)     │                                │ Version          │
│ FK TypeId        │                                │ CreatedAt        │
│ FK FieldId       │                                └─────────┬────────┘
│ SortOrder        │                                          │ 1
└─────────┬────────┘                                          │ 
          │ N                                                 │ N
          │                                         ┌─────────▼──────────┐
          │ 1                                       │ ContentFieldValue  │
┌─────────▼────────┐                                ├────────────────────┤
│  ContentField    │◄───────────────────────────────│ PK (RevisionId,    │
├──────────────────┤          1           N         │     FieldId)       │
│ PK FieldId       │                                │ FK RevisionId      │
│ FieldCode        │                                │ FK FieldId         │
│ FieldType        │                                │ FieldValue         │
│ IsRequired       │                                └────────────────────┘
│ SortOrder        │
└──────────────────┘                                       

文章
 │
 ├── 有很多版本
 │
 ├── 每個版本有很多欄位值
 │
 ├── 欄位值對應欄位規格
 │
 └── 欄位規格來自文章種類

 Content (文章)
   │
   └── ContentRevision (版本)
           │
           └── ContentFieldValue (欄位值)
                   │
                   └── ContentField (欄位規格)
                           │
                           └── ContentType (文章種類)