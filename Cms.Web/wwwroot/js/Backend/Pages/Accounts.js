import { getAccountSummaries } from '../../api/accountApi.js';
import { createAccount } from '../../api/accountApi.js';
import { AccountCreateModal } from './AccountCreateModal.js';

export const Accounts = {
    components: {
        AccountCreateModal
    },

    data() {
        return {
            accounts: [],
            loading: false,
            showCreateModal: false,
            newAccount: {
                username: '',
                password: '',
                role: ''
            }
        };
    },

    mounted() {
        this.loadAccounts();
    },

    methods: {
        async loadAccounts() {
            this.loading = true;
            try {
                const res = await getAccountSummaries();
                this.accounts = res.data;
            } finally {
                this.loading = false;
            }
        },

        openCreateModal() {
            this.showCreateModal = true;
        },

        async onAccountCreated() {
            this.showCreateModal = false;
            await this.loadAccounts();
        }
    },

    template: `
        <div>
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h4 class="mb-0">帳戶管理</h4>

                <button class="btn btn-primary btn-sm" @click="openCreateModal">
                    新增帳戶
                </button>
            </div>

            <div v-if="loading" class="text-muted mb-2">
                載入中...
            </div>

            <table class="table table-bordered bg-white">
                <thead>
                    <tr>
                        <th>帳號</th>
                        <th>角色</th>
                        <th>狀態</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="acc in accounts" :key="acc.id">
                        <td>{{ acc.username }}</td>
                        <td>{{ acc.roles.join(', ') }}</td>
                        <td>{{ acc.status }}</td>
                    </tr>
                </tbody>
            </table>
        </div>

        <AccountCreateModal
            :show="showCreateModal"
            @close="showCreateModal = false"
            @created="onAccountCreated"
        />
    `
};

// created：我這個元件「存在了」
// mounted：畫面真的被「插到瀏覽器」了
// updated：data 改 → 畫面改（少用）
// unmounted：元件被移除，一定要清資源

// 取的所有帳戶
async function fetchGetAccountSummaries() {
    let res = await fetch('/Account/GetAccountSummaries', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
        })
    }).then(r => r.json());

    return await res;
}
