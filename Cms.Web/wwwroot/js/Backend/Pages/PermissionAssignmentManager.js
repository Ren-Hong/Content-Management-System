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

            page: 1,
            pageSize: 10,
            totalCount: 0,

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
                const res = await getPermissionAssignmentSummaries({
                    page: this.page,
                    pageSize: this.pageSize
                });

                this.permissionAssignmentSummaries = res.data.items;
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
            this.loadPermissionAssignmentSummaries();
        },

        openCreate(permissionAssignmentSummary) {
            this.showCreate = true;
            this.selectedPermissionAssignment = permissionAssignmentSummary;
        },

        openEdit(permissionAssignmentSummary) {
            this.selectedPermissionAssignment = permissionAssignmentSummary;
            this.showEdit = true;
        },

        openDelete(permissionAssignmentSummary) {
            this.selectedPermissionAssignment = permissionAssignmentSummary;
            this.showDelete = true;
        },

        async onPermissionAssignmentChanged() {
            this.showCreate = false;
            this.showEdit = false;
            this.showDelete = false;

            this.selectedPermissionAssignment = null;

            await this.loadPermissionAssignmentSummaries();
        }
    },

    template: `
        <div>
            <div class="d-flex justify-content-between mb-3">
                <h4>權限指派管理</h4>
            </div>

            <table class="table table-bordered bg-white">
                <thead>
                    <tr>
                        <th style="width:200px">帳戶</th>
                        <th>權限 / 部門</th>
                        <th style="width:160px">操作</th>
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="u in permissionAssignmentSummaries"
                        :key="u.accountId">

                        <td><strong>{{ u.username }}</strong></td>

                        <td>
                            <div v-if="u.permissionDepartments?.length">

                                <div v-for="p in u.permissionDepartments"
                                    :key="p.permissionId"
                                    class="mb-3">

                                    <!-- Permission -->
                                    <strong>{{ p.permissionName }}</strong>

                                    <!-- Departments -->
                                    <div v-for="d in p.departments"
                                        :key="d.departmentId"
                                        class="ms-3 mt-1 small text-muted">

                                        <span>{{ d.departmentName }}</span>

                                        <span class="ms-2">
                                            {{ d.validFrom?.substring(0,16).replace('T',' ') || '-' }}
                                            ~
                                            {{ d.validTo?.substring(0,16).replace('T',' ') || '-' }}
                                        </span>
                                    </div>

                                </div>
                            </div>

                            <div v-else class="text-muted">
                                尚未指派
                            </div>
                        </td>

                        <td class="text-nowrap">
                            <button class="btn btn-primary btn-sm me-1"
                                    @click="openCreate(u)">新增</button>

                            <button class="btn btn-success btn-sm me-1"
                                    @click="openEdit(u)">編輯</button>

                            <button class="btn btn-danger btn-sm"
                                    @click="openDelete(u)">刪除</button>
                        </td>
                    </tr>

                    <tr v-if="!permissionAssignmentSummaries.length">
                        <td colspan="3" class="text-center text-muted">
                            目前沒有資料
                        </td>
                    </tr>
                </tbody>
            </table>

            <!-- 分頁 -->
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

            <PermissionAssignmentCreateModal
                :show="showCreate"
                :permissionAssignment="selectedPermissionAssignment"
                @close="showCreate=false"
                @created="onPermissionAssignmentChanged"
            />

            <PermissionAssignmentEditModal
                :show="showEdit"
                :permissionAssignment="selectedPermissionAssignment"
                @close="showEdit=false"
                @updated="onPermissionAssignmentChanged"
            />

            <PermissionAssignmentDeleteModal
                :show="showDelete"
                :permissionAssignment="selectedPermissionAssignment"
                @close="showDelete=false"
                @deleted="onPermissionAssignmentChanged"
            />
        </div>
    `
};
