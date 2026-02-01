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
                <h4>角色管理</h4>
                <button class="btn btn-primary btn-sm" @click="openCreate">
                    新增角色
                </button>
            </div>

            <table class="table table-bordered bg-white">
                <thead>
                    <tr>
                        <th>角色</th>
                        <th>權限</th>
                        <th>狀態</th>
                        <th style="width:180px">操作</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="r in roleSummaries" :key="r.roleId">
                        <td>{{ r.roleName }}</td>

                        <td>
                            <div v-if="r.permissionScopes?.length">
                                <div v-for="p in r.permissionScopes"
                                     :key="p.permissionId"
                                     class="mb-1">

                                    <strong>{{ p.permissionName }}</strong>

                                    <span v-for="s in p.scopes"
                                          :key="s.scopeId"
                                          class="badge ms-1"
                                          :class="{
                                            'bg-success': s.scopeName === '所屬部門',
                                            'bg-warning text-dark': s.scopeName === '明確指派',
                                            'bg-primary': s.scopeName === '全系統範圍',
                                            'bg-danger': s.scopeName === '僅限本人'
                                          }"
                                    >
                                        {{ s.scopeName }}
                                    </span>
                                </div>
                            </div>
                        </td>

                        <td>{{ RoleStatusText[r.status] }}</td>
                        <td class="text-nowrap">
                            <button class="btn btn-success btn-sm me-1"
                                    @click="openEdit(r)">編輯</button>

                            <button class="btn btn-danger btn-sm"
                                    @click="openDelete(r)">刪除</button>
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