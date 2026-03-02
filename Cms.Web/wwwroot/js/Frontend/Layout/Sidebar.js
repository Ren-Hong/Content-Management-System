export const Sidebar = {
    props: [
        'currentPage',
        'pages'
    ],

    emits: ['page-change'],

    data() {
        return {
            menus: [
                { key: 'Dashboard', title: '儀表板' },

                { key: 'InternalMedicine', title: '內科' },
                { key: 'Surgery', title: '外科' },
                { key: 'Emergency', title: '急診科' },
                { key: 'Pediatrics', title: '小兒科' },
                { key: 'Nursing', title: '護理部' }
            ]
        };
    },

    template: `
        <nav class="frontend-sidebar">

            <div
                v-for="menu in menus"
                :key="menu.key"
                class="frontend-sidebar-item"
                :class="{ active: currentPage === pages[menu.key] }"
                @click="$emit('page-change', pages[menu.key])"
            >
                {{ menu.title }}
            </div>

        </nav>
    `
};