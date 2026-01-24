import { getRoleSummaries } from '../api/roleApi.js';

import { RoleStatusText } from '../constants/roleStatus.js';

import { RoleCreateModal } from './RoleCreateModal.js';
import { RoleEditModal } from './RoleEditModal.js';
import { RoleDeleteModal } from './RoleDeleteModal.js';

export const RoleManager = {
    components: {
        RoleCreateModal,
        RoleEditModal,
        RoleDeleteModal
    },

    data() {
        return {
            RoleStatusText,

            roleSummaries: [],
            loading: false,

            showCreate: false,
            showEdit: false,
            showDelete: false,

            selectedRole: null
        };
    },

    mounted() {
        this.loadRoleSummaries();
    },

    methods: {
        async loadRoleSummaries() {
            this.loading = true;
            try {
                const res = await getRoleSummaries();
                this.roleSummaries = res.data;
            } finally {
                this.loading = false;
            }
        },

        openCreate() {
            this.showCreate = true;
        },

        openEdit(roleSummary) {
            this.selectedRole = roleSummary;
            this.showEdit = true;
        },

        openDelete(roleSummary) {
            this.selectedRole = roleSummary;
            this.showDelete = true;
        },

        async onRoleChanged() {
            this.showCreate = false;
            this.showEdit = false;
            this.showDelete = false;

            this.selectedRole = null;

            await this.loadRoleSummaries();
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
                    <tr v-for="roleSummary in roleSummaries" :key="roleSummary.roleId">
                        <td>{{ roleSummary.roleName }}</td>
                        <td>{{ roleSummary.permissions?.map(p => p.permissionName).join(', ') }}</td>
                        <td>{{ RoleStatusText[roleSummary.status] }}</td>
                        <td class="text-nowrap">
                            <button class="btn btn-success btn-sm me-1"
                                    @click="openEdit(roleSummary)">編輯</button>

                            <button class="btn btn-danger btn-sm"
                                    @click="openDelete(roleSummary)">刪除</button>
                        </td>
                    </tr>
                </tbody>
            </table>

            <RoleCreateModal
                :show="showCreate"
                @close="showCreate=false"
                @created="onRoleChanged"
            />

            <RoleEditModal
                :show="showEdit"
                :role="selectedRole"
                @close="showEdit=false"
                @updated="onRoleChanged"
            />

            <RoleDeleteModal
                :show="showDelete"
                :role="selectedRole"
                @close="showDelete=false"
                @deleted="onRoleChanged"
            />
        </div>
    `
};