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

            pages: {
                Dashboard: markRaw(Dashboard),
                InternalMedicine: markRaw(InternalMedicine),
                Surgery: markRaw(Surgery),
                Emergency: markRaw(Emergency),
                Pediatrics: markRaw(Pediatrics),
                Nursing: markRaw(Nursing)
            }
        };
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
                        @page-change="currentPage = $event"
                    />
                </aside>

                <main class="frontend-content p-4">
                    <component :is="currentPage" />
                </main>

            </div>

        </div>
    `
}).mount('#app');