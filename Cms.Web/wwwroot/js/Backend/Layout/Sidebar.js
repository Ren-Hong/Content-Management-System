export const Sidebar = {
    props: {
        currentPage: Object,
        pages: Object
    },

    emits: ['page-change'],

    data() {
        return {
            menus: [
                {
                    key: 'AccountManager',
                    title: '帳戶管理'
                },
                {
                    key: 'RoleManager',
                    title: '角色管理'
                },
                {
                    key: 'PermissionAssignmentManager',
                    title: '權限指派'
                }
            ]
        };
    },

    template: `
        <nav class="backend-sidebar">

            <div
                v-for="menu in menus"
                :key="menu.key"
                class="backend-sidebar-item"
                :class="{ active: currentPage === pages[menu.key] }"
                @click="$emit('page-change', pages[menu.key])"
            >
                {{ menu.title }}
            </div>

        </nav>
    `
};