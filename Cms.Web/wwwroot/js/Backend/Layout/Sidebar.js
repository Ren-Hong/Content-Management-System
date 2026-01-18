export const Sidebar = {
    props: ['currentPage'],
    emits: ['page-change'],
    template: `
        <nav class="sidebar">
            <div
                class="sidebar-item"
                :class="{ active: currentPage === 'accounts' }"
                @click="$emit('page-change', 'accounts')"
            >
                帳戶管理
            </div>

            <div
                class="sidebar-item"
                :class="{ active: currentPage === 'roles' }"
                @click="$emit('page-change', 'roles')"
            >
                角色管理
            </div>

            <div
                class="sidebar-item"
                :class="{ active: currentPage === 'permissions' }"
                @click="$emit('page-change', 'permissions')"
            >
                權限管理
            </div>
        </nav>
    `
};

// $emit('page-change', ...) 不是跟所有人講 它只會通知「直接包住它的父層」

// :class="{ 'fw-bold': currentPage === 'permissions' }" -> 「如果 currentPage 是 permissions，就幫我加上 fw-bold 這個 class；不是的話就不要加」fw-bold': true 或 false

// Vue 的 class 綁定規則（這個要記）
//  : class="{
//      'class-name': 條件
//  }"
// 如果 條件 為 true 加上 class-name 否則 不加