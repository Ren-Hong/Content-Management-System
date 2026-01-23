import { createRole } from '../api/roleApi.js';
import { getPermissionOptions } from '../api/PermissionApi.js';

export const RoleCreateModal = {
    props: {
        show: {
            type: Boolean,
            required: true
        }
    },

    emits: ['close', 'created'],

    data() {
        return {
            modal: null,        
            permissionOptions: [],
            form: {
                roleName: '',
                roleCode: '',
                permissionIds: [] 
            },
            submitting: false
        };
    },

    watch: {                        // watch =「當某個值改變時，我要額外做一件事」
        async show(val) {                 //「只要 show 這個 prop 的值改變，如果它變成 true，我就把表單清空。」
            if (val) {
                await this.loadPermissionOptions(); 

                this.resetForm();
                this.modal.show();
            } else {
                this.modal.hide();
            }
        }
    },

    mounted() {
        // Bootstrap 5 Modal 初始化
        this.modal = new bootstrap.Modal(this.$refs.modal);

        // 當使用者用 ESC / X 關閉 modal
        this.$refs.modal.addEventListener('hidden.bs.modal', () => {
            this.$emit('close');
        });
    },

    methods: {
        async loadPermissionOptions() {
            try {
                let res = await getPermissionOptions();

                if (!res.success) {
                    alert(res.errorCode || ' 權限選單載入失敗');
                    return;
                }

                this.permissionOptions = res.data;
            } catch (err) {
                console.error(err);
                alert('系統錯誤（權限選單載入）');
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
            this.form.permissionIds = this.form.permissionIds.filter(id => id !== permissionId);
        },

        getPermissionName(permissionId) {
            const permission = this.permissionOptions.find(r => r.permissionId === permissionId);
            return permission ? permission.permissionName : permissionId;
        },

        resetForm() {
            this.form = {
                roleName: '',
                roleCode: '',
                permissionIds: []   
            };
        },

        async submit() {
            if (this.form.permissionIds.length === 0) {
                alert('請至少選擇一個權限');
                return;
            }

            this.submitting = true;

            try {
                let res = await createRole(this.form);

                if (!res.success) {
                    alert(res.errorCode || '新增失敗');
                    return;
                }

                this.$emit('created');
            } catch (err) {
                console.error(err);
                alert('系統錯誤');
            } finally {
                this.submitting = false;
            }
        }
    },

    template: `
        <div class="modal fade" tabindex="-1" ref="modal">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content border-primary">

                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title">新增角色</h5>
                        <button type="button"
                                class="btn-close"
                                :disabled="submitting"
                                data-bs-dismiss="modal">
                        </button>
                    </div>

                    <div class="modal-body">

                        <div class="alert alert-primary mb-3">
                            <label class="form-label">角色名稱</label>

                            <input class="form-control"
                                    v-model="form.roleName">
                        </div>

                        <div class="alert alert-primary mb-3">
                            <label class="form-label">角色代碼(程式用)</label>

                            <input class="form-control"
                                    v-model="form.roleCode">
                        </div>

                        <div class="alert alert-primary mb-3">

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
                    </div>

                    <div class="modal-footer">

                        <button class="btn btn-secondary btn-sm"
                                :disabled="submitting"
                                data-bs-dismiss="modal">
                            取消
                        </button>

                        <button class="btn btn-primary btn-sm"
                                :disabled="submitting"
                                @click="submit">
                            {{ submitting ? '建立中...' : '建立' }}
                        </button>

                    </div>

                </div>
            </div>
        </div>
    `
};
