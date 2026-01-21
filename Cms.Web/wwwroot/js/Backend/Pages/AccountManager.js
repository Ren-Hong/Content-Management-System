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
                const res = await getAccountSummaries();
                this.accountSummaries = res.data;
            } finally {
                this.loading = false;
            }
        },

        openCreate() {
            this.showCreate = true;
        },

        openEdit(acc) {
            this.selectedAccount = acc;
            this.showEdit = true;
        },

        openResetPassword(acc) {
            this.selectedAccount = acc;
            this.showResetPassword = true;
        },

        openDelete(acc) {
            this.selectedAccount = acc;
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
            <div class="d-flex justify-content-between mb-3">
                <h4>帳戶管理</h4>
                <button class="btn btn-primary btn-sm" @click="openCreate">
                    新增帳戶
                </button>
            </div>

            <table class="table table-bordered bg-white">
                <thead>
                    <tr>
                        <th>帳號</th>
                        <th>角色</th>
                        <th>狀態</th>
                        <th style="width:180px">操作</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="acc in accountSummaries" :key="acc.id">
                        <td>{{ acc.username }}</td>
                        <td>{{ acc.roles?.map(r => r.roleName).join(', ') }}</td>
                        <td>{{ AccountStatusText[acc.status] }}</td>
                        <td class="text-nowrap">
                            <button class="btn btn-success btn-sm me-1"
                                    @click="openEdit(acc)">編輯</button>

                            <button class="btn btn-warning btn-sm me-1 text-white"
                                    @click="openResetPassword(acc)">重設密碼</button>

                            <button class="btn btn-danger btn-sm"
                                    @click="openDelete(acc)">刪除</button>
                        </td>
                    </tr>
                </tbody>
            </table>

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