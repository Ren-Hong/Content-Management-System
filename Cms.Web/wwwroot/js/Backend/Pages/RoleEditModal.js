import { updateRole } from '../api/roleApi.js';
import { getPermissionOptions } from '../api/permissionApi.js';

import { RoleStatusText } from '../constants/roleStatus.js';

export const RoleEditModal = {
    props: {
        show: { // RoleManager 的 showEdit
            type: Boolean,
            required: true
        },
        role: { // RolManager 的 selectedRole
            type: Object,
            default : null
        }
    },

    emits: ['close', 'updated'],

    data() {
        return {
            RoleStatusText, 

            modal: null,
            submitting: false,
            permissionOptions: [],
            form: {
                roleName: '',
                status: '',
                permissionIds: []   // ⭐ 多選
            }
        };
    },

    watch: {
        async show(val) {
            if (val) {
                await this.loadPermissionOptions(); 

                this.fillForm();
                this.modal.show();
            } else {
                this.modal.hide();
            }
        }
    },

    mounted() {
        this.modal = new bootstrap.Modal(this.$refs.modal);

        // 使用者點 X / 點背景 關閉 modal
        this.$refs.modal.addEventListener('hidden.bs.modal', () => {
            this.$emit('close');
        });
    },

    methods: {
        async loadPermissionOptions() {
            try {
                let res = await getPermissionOptions();

                if (!res.success) {
                    alert(res.errorCode || '權限選單載入失敗');
                    return;
                }

                this.permissionOptions = res.data;
            } catch (err) {
                console.error(err);
                alert('API路徑或Json格式不對');
            }
        },

        onPermissionSelected(event) {
            const permissionId = event.target.value;
            if (!permissionId) return;

            // 避免重複加入
            if (!this.form.permissionIds.includes(permissionId)) {
                this.form.permissionIds.push(permissionId);
            }

            // 重置 select
            event.target.value = '';
        },

        removePermission(permissionId) {
            this.form.permissionIds =
                this.form.permissionIds.filter(id => id !== permissionId);
        },

        getPermissionName(permissionId) {
            const permission = this.permissionOptions.find(r => r.permissionId === permissionId);
            return permission ? permission.permissionName : permissionId;
        },

        fillForm() { // 填入本來的資料
            this.form.roleName = this.role.roleName;
            this.form.permissionIds = this.role?.permissions?.map(r => r.permissionId) ?? [];
            this.form.status = this.role.status ?? '';
        },

        async submit() {

            if (this.form.permissionIds.length === 0) {
                alert('請至少選擇一個角色');
                return;
            }
            this.submitting = true;

            try {
                let res = await updateRole(this.form);

                if (!res.success) {
                    alert(res.errorCode || '更新角色失敗');
                    return;
                }

                this.$emit('updated');
            } catch (err) {
                console.error(err);
                alert('api錯誤');
            } finally {
                this.submitting = false;
            }
        }
    },

    template: `
        <div class="modal fade" tabindex="-1" ref="modal">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content border-success">

                    <div class="modal-header bg-success text-white">
                        <h5 class="modal-title">編輯角色</h5>
                        <button type="button"
                                class="btn-close"
                                :disabled="submitting"
                                data-bs-dismiss="modal"></button>
                    </div>

                    <div class="modal-body">
                        <div class="row">
                            <div class="col-6">
                                <div class="alert alert-success mb-3">
                                    <strong>角色名稱 : </strong>
                                    <span>{{ role?.roleName }}</span>
                                </div>
                            </div>

                            <div class="col-6">
                                <div class="alert alert-success mb-3">
                                    <strong>角色狀態 : </strong>
                                    <span>{{ role ? RoleStatusText[role.status] : '' }}</span>
                                </div>
                            </div>
                        </div>

                        <div class="alert alert-success mb-3">

                            <div class="mb-2">
                                <label class="form-label">權限</label>

                                <select class="form-select"
                                        @change="onPermissionSelected($event)">
                                    <option value="">請選擇權限</option>
                                    <option v-for="permission in permissionOptions"
                                            :key="permission.permissionId"
                                            :value="permission.permissionId">
                                        {{ permission.permissionName }}
                                    </option>
                                </select>
                            </div>

                            <div class="d-flex flex-wrap gap-2">
                                <span v-for="permissionId in form.permissionIds"
                                      :key="permissionId"
                                      class="badge bg-primary d-flex align-items-center">

                                    {{ getPermissionName(permissionId) }}

                                    <button type="button"
                                            class="btn-close btn-close-white ms-2"
                                            @click="removePermission(permissionId)">
                                    </button>
                                </span>
                            </div>

                        </div>

                        <div class="alert alert-success mb-3">
                            <label class="form-label">狀態</label>
                            <select class="form-select" v-model="form.status">
                                <option value="Enable">啟用</option>
                                <option value="Disabled">停用</option>
                            </select>
                        </div>

                        <div class="alert alert-success small">
                            變更將立即生效
                        </div>
                    </div>

                    <div class="modal-footer">
                        <button class="btn btn-secondary btn-sm"
                                :disabled="submitting"
                                data-bs-dismiss="modal">
                            取消
                        </button>
                        <button class="btn btn-success btn-sm"
                                :disabled="submitting"
                                @click="submit">
                            {{ submitting ? '儲存中...' : '儲存' }}
                        </button>
                    </div>

                </div>
            </div>
        </div>
    `
};
