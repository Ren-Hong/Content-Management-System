import { getDepartmentsForSidebar } from '../api/departmentApi.js';

export const Sidebar = {

    props: {
        currentPage: Object,
        pages: Object,
        currentDepartmentCode: String
    },

    emits: ['page-change'],

    data() {
        return {
            menus: []
        };
    },

    async mounted() {
        const res = await getDepartmentsForSidebar();
        this.menus = res.data;
    },

    template: `
        <nav class="frontend-sidebar">

            <div
                v-for="menu in menus"
                :key="menu.departmentCode"
                class="frontend-sidebar-item"
                :class="{ active: currentDepartmentCode === menu.departmentCode }"
                @click="$emit('page-change', {
                    component: pages[menu.departmentCode],
                    departmentCode: menu.departmentCode,
                    departmentId: menu.departmentId
                })"
            >
                {{ menu.departmentName }}
            </div>

        </nav>
    `
};