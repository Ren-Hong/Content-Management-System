import { createPermissionAssignment } from '../api/permissionAssignmentApi.js';
import { getPermissionOptions } from '../api/permissionApi.js';
import { getDepartmentOptions } from '../api/departmentApi.js';

export const PermissionAssignmentCreateModal = {
    props: {
        show: {
            type: Boolean,
            required: true
        },
        permissionAssignment: Object
    },

    emits: ['close', 'created'],

    data() {
        return {
            modal: null,

            permissionOptions: [],
            departmentOptions: [],

            submitting: false,

            form: {
                accountId: '',
                departmentId: '',
                permissionId: '',
                validFrom: '',
                validTo: ''
            }
        };
    },

    watch: {
        async show(val) {
            if (val) {
                await Promise.all([
                    this.loadPermissionOptions(),
                    this.loadDepartmentOptions()
                ]);

                this.resetForm();
                this.modal.show();
            } else {
                this.modal.hide();
            }
        }
    },

    mounted() {
        this.modal = new bootstrap.Modal(this.$refs.modal);

        this.$refs.modal.addEventListener('hidden.bs.modal', () => {
            this.$emit('close');
        });
    },

    methods: {
        async loadPermissionOptions() {
            try {
                const res = await getPermissionOptions();
                if (!res.success) {
                    alert(`錯誤代碼 -> ${res.errorCode}`);
                    return;
                }
                this.permissionOptions = res.data;
            } catch (err) {
                console.error(err);
                alert('系統錯誤 -> 權限選單載入');
            }
        },

        async loadDepartmentOptions() {
            try {
                const res = await getDepartmentOptions();
                if (!res.success) {
                    alert(`錯誤代碼 -> ${res.errorCode}`);
                    return;
                }
                this.departmentOptions = res.data;
            } catch (err) {
                console.error(err);
                alert('系統錯誤 -> 部門選單載入');
            }
        },

        resetForm() {
            this.form = {
                accountId: this.permissionAssignment.accountId ?? '',
                departmentId: '',
                permissionId: '',
                validFrom: '',
                validTo: ''
            };
        },

        async submit() {
            if (!this.form.departmentId) {
                alert('請選擇部門');
                return;
            }

            if (!this.form.permissionId) {
                alert('請選擇權限');
                return;
            }

            this.submitting = true;

            try {
                const res = await createPermissionAssignment(this.form);
                if (!res.success) {
                    alert(`錯誤代碼 -> ${res.errorCode}`);
                    return;
                }

                this.$emit('created');
                this.modal.hide();
            } catch (err) {
                console.error(err);
                alert('系統錯誤 -> 創建權限指派');
            } finally {
                this.submitting = false;
            }
        }
    },

    template: `
        <div class="modal fade" tabindex="-1" ref="modal">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">

                    <div class="modal-header">
                        <h5 class="modal-title">新增部門權限指派</h5>
                        <button type="button"
                                class="btn-close"
                                data-bs-dismiss="modal"
                                :disabled="submitting">
                        </button>
                    </div>

                    <div class="modal-body">

                        <div class="mb-2 text-muted small">
                            帳戶：{{ permissionAssignment?.username }}
                        </div>

                        <div class="mb-3">
                            <label class="form-label">部門</label>
                            <select class="form-select"
                                    v-model="form.departmentId">
                                <option value="">請選擇部門</option>
                                <option v-for="d in departmentOptions"
                                        :key="d.departmentId"
                                        :value="d.departmentId">
                                    {{ d.departmentName }}
                                </option>
                            </select>
                        </div>

                        <div class="mb-3">
                            <label class="form-label">權限</label>
                            <select class="form-select"
                                    v-model="form.permissionId">
                                <option value="">請選擇權限</option>
                                <option v-for="p in permissionOptions"
                                        :key="p.permissionId"
                                        :value="p.permissionId">
                                    {{ p.permissionName }}
                                </option>
                            </select>
                        </div>

                        <div class="row">
                            <div class="col">
                                <label class="form-label">有效起日</label>
                                <input type="date"
                                    class="form-control"
                                    v-model="form.validFrom">
                            </div>

                            <div class="col">
                                <label class="form-label">有效迄日</label>
                                <input type="date"
                                    class="form-control"
                                    v-model="form.validTo">
                            </div>
                        </div>

                    </div>

                    <div class="modal-footer">
                        <button class="btn btn-secondary"
                                data-bs-dismiss="modal"
                                :disabled="submitting">
                            取消
                        </button>

                        <button class="btn btn-primary"
                                @click="submit"
                                :disabled="submitting">
                            {{ submitting ? '建立中...' : '建立' }}
                        </button>
                    </div>

                </div>
            </div>
        </div>
    `
};
