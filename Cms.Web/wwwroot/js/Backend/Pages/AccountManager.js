import { getAccountSummaries } from '../api/accountApi.js';

import { AccountStatusText } from '../constants/accountStatus.js';

import { AccountCreateModal } from './AccountCreateModal.js';
import { AccountEditModal } from './AccountEditModal.js';
import { AccountResetPasswordModal } from './AccountResetPasswordModal.js';
import { AccountDeleteModal } from './AccountDeleteModal.js';

export const AccountManager = {
    components: {
        AccountCreateModal,
        AccountEditModal,
        AccountResetPasswordModal,
        AccountDeleteModal
    },

    data() {
        return {
            AccountStatusText,   // 👈 關鍵在這一行

            accountSummaries: [],

            page: 1,
            pageSize: 10,
            totalCount: 0,

            loading: false,

            showCreate: false,
            showEdit: false,
            showResetPassword: false,
            showDelete: false,

            selectedAccount: null
        };
    },

    mounted() {
        this.loadAccountSummaries();
    },

    methods: {
        async loadAccountSummaries() {
            this.loading = true;
            try {
                const res = await getAccountSummaries({
                    page: this.page,
                    pageSize: this.pageSize,
                    keyword: document.getElementById('accountSearch')?.value
                });

                this.accountSummaries = res.data.items;
                this.totalCount = res.data.totalCount;
            } finally {
                this.loading = false;
            }
        },
        
        changePage(p) {
            if (p < 1) return;

            const maxPage = Math.ceil(this.totalCount / this.pageSize);
            if (p > maxPage) return;

            this.page = p;
            this.loadAccountSummaries();
        },

        openCreate() {
            this.showCreate = true;
        },

        openEdit(accountSummary) {
            this.selectedAccount = accountSummary;
            this.showEdit = true;
        },

        openResetPassword(accountSummary) {
            this.selectedAccount = accountSummary;
            this.showResetPassword = true;
        },

        openDelete(accountSummary) {
            this.selectedAccount = accountSummary;
            this.showDelete = true;
        },

        async onAccountChanged() {
            this.showCreate = false;
            this.showEdit = false;
            this.showResetPassword = false;
            this.showDelete = false;

            this.selectedAccount = null;

            await this.loadAccountSummaries();
        }
    },

    template: `
        <div>
            <h1 class="text-center">帳戶管理</h1>
            
            <div class="d-flex justify-content-between align-items-center mb-3">
                <div class="input-group w-25">

                    <input
                        id="accountSearch"
                        type="text"
                        class="form-control"
                        placeholder="搜尋帳號或角色名稱" />

                    <button
                        class="btn btn-outline-secondary"
                        onclick="searchAccounts()">
                        🔍 搜尋
                    </button>

                </div>

                <button class="btn btn-primary" @click="openCreate">
                    新增帳戶
                </button>
            </div>

            <table class="table table-bordered table-striped table-hover">
                <thead class="table-dark">
                    <tr>
                    <th>帳號</th>
                    <th>角色</th>
                    <th>部門</th>
                    <th>狀態</th>
                    <th style="width:180px">操作</th>
                    </tr>
                </thead>
                
                <tbody>
                    <template v-for="a in accountSummaries" :key="a.accountId">

                        <!-- 有 roleAssignments：用 rowspan 顯示 -->
                        <template v-if="a.roleAssignments && a.roleAssignments.length > 0">
                            <tr v-for="(ra, idx) in a.roleAssignments" :key="a.accountId + '-' + ra.roleId">

                                <!-- 帳號：只在第一列顯示一次 -->
                                <td v-if="idx === 0" :rowspan="a.roleAssignments.length">
                                    {{ a.username }}
                                </td>

                                <!-- 角色 -->
                                <td>
                                    {{ ra.roleName }}
                                </td>

                                <!-- 這個角色的部門（badge 方式最清楚） -->
                                <td>
                                    <div class="d-flex flex-wrap gap-2">
                                        <span v-for="d in ra.departments"
                                            :key="d.departmentId"
                                            class="badge bg-secondary">
                                            {{ d.departmentName }}
                                        </span>
                                    </div>
                                </td>

                                <!-- 狀態：也只顯示一次 -->
                                <td v-if="idx === 0" :rowspan="a.roleAssignments.length">
                                    {{ AccountStatusText[a.status] }}
                                </td>

                                <!-- 操作：也只顯示一次 -->
                                <td v-if="idx === 0" :rowspan="a.roleAssignments.length" class="text-nowrap">
                                    <button class="btn btn-success btn-sm me-1"
                                            @click="openEdit(a)">編輯</button>

                                    <button class="btn btn-warning btn-sm me-1 text-white"
                                            @click="openResetPassword(a)">重設密碼</button>

                                    <button class="btn btn-danger btn-sm"
                                            @click="openDelete(a)">刪除</button>
                                </td>

                            </tr>
                        </template>

                        <!-- 沒有 roleAssignments：顯示一列空白 -->
                        <tr v-else>
                            <td>{{ a.username }}</td>
                            <td class="text-muted">—</td>
                            <td class="text-muted">—</td>
                            <td>{{ AccountStatusText[a.status] }}</td>
                            <td class="text-nowrap">
                                <button class="btn btn-success btn-sm me-1"
                                        @click="openEdit(a)">編輯</button>

                                <button class="btn btn-warning btn-sm me-1 text-white"
                                        @click="openResetPassword(a)">重設密碼</button>

                                <button class="btn btn-danger btn-sm"
                                        @click="openDelete(a)">刪除</button>
                            </td>
                        </tr>

                    </template>
                </tbody>
            </table>

            <nav class="mt-3">
                <ul class="pagination pagination-sm">

                    <li class="page-item" :class="{ disabled: page === 1 }">
                        <a class="page-link" href="#" @click.prevent="changePage(page - 1)">
                            上一頁
                        </a>
                    </li>

                    <li class="page-item disabled">
                        <span class="page-link">
                            第 {{ page }} 頁 / 共 {{ Math.ceil(totalCount / pageSize) }} 頁
                        </span>
                    </li>

                    <li class="page-item" :class="{ disabled: page >= Math.ceil(totalCount / pageSize) }">
                        <a class="page-link" href="#" @click.prevent="changePage(page + 1)">
                            下一頁
                        </a>
                    </li>

                </ul>
            </nav>

            <AccountCreateModal
                :show="showCreate"
                @close="showCreate=false"
                @created="onAccountChanged"
            />

            <AccountEditModal
                :show="showEdit"
                :account="selectedAccount"
                @close="showEdit=false"
                @updated="onAccountChanged"
            />

            <AccountResetPasswordModal
                :show="showResetPassword"
                :account="selectedAccount"
                @close="showResetPassword=false"
                @updated="onAccountChanged"
            />

            <AccountDeleteModal
                :show="showDelete"
                :account="selectedAccount"
                @close="showDelete=false"
                @deleted="onAccountChanged"
            />
        </div>
    `
};