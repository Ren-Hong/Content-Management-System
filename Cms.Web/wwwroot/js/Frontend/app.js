import { Header } from './Layout/Header.js';
import { Sidebar } from './Layout/Sidebar.js';

import { Dashboard } from './Pages/Dashboard.js';
import { InternalMedicine } from './Pages/InternalMedicine.js';
import { Surgery } from './Pages/Surgery.js';
import { Emergency } from './Pages/Emergency.js';
import { Pediatrics } from './Pages/Pediatrics.js';
import { Nursing } from './Pages/Nursing.js';

const { createApp, markRaw } = Vue;

createApp({
    components: {
        Header,
        Sidebar
    },

    data() {
        return {
            currentPage: markRaw(Dashboard),
            currentDepartmentCode: 'DASH',
            currentDepartmentId: null,

            pages: {
                DASH: markRaw(Dashboard),
                MED: markRaw(InternalMedicine),
                SUR: markRaw(Surgery),
                ER: markRaw(Emergency),
                PED: markRaw(Pediatrics),
                NUR: markRaw(Nursing)
            }
        };
    },

    methods: {
        changePage(payload) {

            this.currentPage = payload.component;
            this.currentDepartmentCode = payload.departmentCode;
            this.currentDepartmentId = payload.departmentId;

        }
    },

    template: `
        <div class="frontend-layout min-vh-100">
    
            <header class="frontend-header shadow-sm px-4 py-3">
                <Header />
            </header>

            <div class="frontend-body d-flex">
            
                <aside class="frontend-sidebar p-3">
                    <Sidebar
                        :currentPage="currentPage"
                        :pages="pages"
                        :currentDepartmentCode="currentDepartmentCode"
                        @page-change="changePage"
                    />
                </aside>

                <main class="frontend-content p-4">
                    <component
                        :is="currentPage"
                        :department-id="currentDepartmentId"
                    />
                </main>

            </div>

        </div>
    `
}).mount('#app');