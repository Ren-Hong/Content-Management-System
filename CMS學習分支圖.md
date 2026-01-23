CMS
├─ ① 基礎系統層（目前正在做）
│   ├─ Authentication（登入）
│   ├─ Authorization（Role / Permission）
│   ├─ 使用者 / 角色管理
│   └─ 系統設定
│
├─ ② 內容模型層（CMS 的核心）
│   ├─ Content Type（文章 / 頁面 / 公告）
│   ├─ 欄位定義（title / body / status）
│   ├─ 狀態機（草稿 / 發佈 / 下架）
│   └─ 版本概念（revision）
│
├─ ③ 資料歸屬 / 可見性
│   ├─ Owner-based（誰建立的）
│   ├─ Scope-based（部門 / 縣市 / 組織）
│   ├─ Assignment-based（指派）
│   └─ 混合模型
│
├─ ④ 工作流程（Workflow）
│   ├─ 審核流程
│   ├─ 多角色狀態轉換
│   └─ 發佈條件
│
├─ ⑤ 內容呈現層
│   ├─ 前台 API
│   ├─ 頁面組裝
│   └─ 快取策略
│
├─ ⑥ 搜尋 / 分類
│   ├─ 標籤 / 分類
│   ├─ 關鍵字搜尋
│   └─ 排序 / 篩選
│
├─ ⑦ 檔案 / 資源管理
│   ├─ 圖片 / 檔案上傳
│   ├─ 權限 / 存取
│   └─ CDN / 儲存
│
├─ ⑧ 多語系 / 國際化
│   ├─ 內容翻譯
│   ├─ 語系 fallback
│   └─ URL / SEO
│
└─ ⑨ 系統級能力（進階）
    ├─ 快取 / 效能
    ├─ Audit / Log
    ├─ 排程（定時發佈）
    └─ API / Headless CMS