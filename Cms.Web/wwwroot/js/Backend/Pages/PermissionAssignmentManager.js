import { getPermissionAssignmentSummaries } from '../api/permissionAssignmentApi.js';

import { PermissionAssignmentCreateModal } from './PermissionAssignmentCreateModal.js';
import { PermissionAssignmentEditModal } from './PermissionAssignmentEditModal.js';
import { PermissionAssignmentDeleteModal } from './PermissionAssignmentDeleteModal.js';

export const PermissionAssignmentManager = {
    components: {
        PermissionAssignmentCreateModal,
        PermissionAssignmentEditModal,
        PermissionAssignmentDeleteModal
    },

    data() {
        return {
            permissionAssignmentSummaries: [],
            loading: false,

            showCreate: false,
            showEdit: false,
            showDelete: false,

            selectedPermissionAssignment: null
        };
    },

    mounted() {
        this.loadPermissionAssignmentSummaries();
    },

    methods: {
        async loadPermissionAssignmentSummaries() {
            this.loading = true;
            try {
                const res = await getPermissionAssignmentSummaries();
                this.permissionAssignmentSummaries = res.data;
            } finally {
                this.loading = false;
            }
        },

        openCreate() {
            this.showCreate = true;
        },

        openEdit(permissionAssignmentSummary) {
            this.selectedRole = roleSummary;
            this.showEdit = true;
        },

        openDelete(permissionAssignmentSummary) {
            this.selectedRole = roleSummary;
            this.showDelete = true;
        },

        async onPermissionAssignmentChanged() {
            this.showCreate = false;
            this.showEdit = false;
            this.showDelete = false;

            this.selectedRole = null;

            await this.loadPermissionAssignmentSummaries();
        }
    },

    template: `
        <div>
            <div class="d-flex justify-content-between mb-3">
                <h4>權限指派管理</h4>
                <button class="btn btn-primary btn-sm" @click="openCreate">
                    新增權限指派
                </button>
            </div>

            <table class="table table-bordered bg-white">
                <thead>
                    <tr>
                        <th style="width:200px">被指派帳戶</th>
                        <th>權限</th>
                        <th style="width:220px">起訖</th>
                        <th style="width:160px">操作</th>
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="u in permissionAssignmentSummaries"
                        :key="u.accountId">

                        <!-- 帳戶 -->
                        <td>
                            <strong>{{ u.username }}</strong>
                        </td>

                        <!-- 權限 + 部門（核心） -->
                        <td>
                            <div v-if="u.assignments?.length">
                                <div v-for="pa in u.assignments"
                                    :key="pa.permissionId"
                                    style="margin-bottom:8px;">

                                    <!-- Permission -->
                                    <div>
                                        <strong>{{ pa.permissionName }}</strong>
                                        <span class="badge bg-success ms-2">
                                            部門
                                        </span>
                                    </div>

                                    <!-- Departments -->
                                    <div class="text-muted"
                                        style="margin-left:12px;font-size:13px;">
                                        {{ pa.departments
                                            .map(d => d.departmentName)
                                            .join('、') }}
                                    </div>
                                </div>
                            </div>

                            <div v-else class="text-muted">
                                尚未指派任何權限
                            </div>
                        </td>

                        <!-- 有效期間 -->
                        <td>
                            <div>{{ u.validFrom || '-' }}</div>
                            <div>~ {{ u.validTo || '-' }}</div>
                        </td>

                        <!-- 操作 -->
                        <td class="text-nowrap">
                            <button class="btn btn-success btn-sm me-1"
                                    @click="openEdit(u)">
                                編輯
                            </button>

                            <button class="btn btn-danger btn-sm"
                                    @click="openDelete(u)">
                                刪除
                            </button>
                        </td>
                    </tr>

                    <tr v-if="!permissionAssignmentSummaries.length">
                        <td colspan="4"
                            class="text-center text-muted">
                            目前沒有資料
                        </td>
                    </tr>
                </tbody>
            </table>

            <PermissionAssignmentCreateModal
                :show="showCreate"
                @close="showCreate=false"
                @created="onPermissionAssignmentChanged"
            />

            <PermissionAssignmentEditModal
                :show="showEdit"
                :role="selectedPermissionAssignment"
                @close="showEdit=false"
                @updated="onPermissionAssignmentChanged"
            />

            <PermissionAssignmentDeleteModal
                :show="showDelete"
                :role="selectedPermissionAssignment"
                @close="showDelete=false"
                @deleted="onPermissionAssignmentChanged"
            />
        </div>
    `
};