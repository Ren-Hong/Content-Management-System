import { Header } from './Layout/Header.js';
import { Sidebar } from './Layout/Sidebar.js';

import { AccountManager } from './Pages/AccountManager.js';
import { RoleManager } from './Pages/RoleManager.js';
import { PermissionAssignmentManager } from './Pages/PermissionAssignmentManager.js';

const { createApp, markRaw } = Vue;

createApp({
    components: {
        Header,
        Sidebar
    },

    data() {
        return {
            currentPage: markRaw(AccountManager),

            pages: {
                AccountManager: markRaw(AccountManager),
                RoleManager: markRaw(RoleManager),
                PermissionAssignmentManager : markRaw(PermissionAssignmentManager)
            }
        };
    },

    template: `
        <div class="backend-layout min-vh-100">
    
            <header class="backend-header shadow-sm px-4 py-3">
                <Header />
            </header>

            <div class="backend-body d-flex">
            
                <aside class="backend-sidebar p-3">
                    <Sidebar
                        :currentPage="currentPage"
                        :pages="pages"
                        @page-change="currentPage = $event"
                    />
                </aside>

                <main class="backend-content p-4">
                    <component :is="currentPage" />
                </main>

            </div>

        </div>
    `
}).mount('#app');

//:currentPage="currentPage" -> v-bind:currentPage="currentPage" 「把 父層的 currentPage 這個變數傳給 Sidebar 元件的 props，名字叫 currentPage」

// v-bind:currentPage="currentPage" 左邊：子元件「props 的名字」 右邊：父層「data 裡的變數」

// data：我自己的狀態（我管的） props：別人給我的資料（我不能亂改）

// $event 是 Vue template 裡的「固定代表事件資料的關鍵字」 $emit 發出去的資料，會自動變成 $event

// : 的完整寫法是 v-bind: 意思是把右邊的 JavaScript 表達式綁定(不是字串本身喔, 是之前宣告過的變數)到左邊這個 attribute 上
// @ 的完整寫法是 v-on:   意思是「監聽一個叫 page-change 的事件事件發生時執行後面的 JS」
// <component v-bind:is="currentPage" /> -> 「Vue，請你看一下現在的 currentPage 是什麼如果是 'accountManager'，就幫我渲染 < AccountManager />如果是 'roleManager'，就渲染 < RoleManager />以此類推」
