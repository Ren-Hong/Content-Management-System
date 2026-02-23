import { createAccount } from '../api/accountApi.js';
import { getRoleOptions } from '../api/roleApi.js';
import { getDepartmentOptions } from '../api/departmentApi.js';

export const AccountCreateModal = {
    props: {
        show: {
            type: Boolean,
            required: true
        }
    },

    emits: ['close', 'created'],

    data() {
        return {
            modal: null,        // Bootstrap Modal instance
            roleOptions: [],
            departmentOptions: [],
            form: {
                username: '',
                password: '',
                roleAssignments: [
                    {
                        roleId: null,
                        departmentIds: []
                    }
                ]
            },
            submitting: false
        };
    },

    watch: {                        // watch =「當某個值改變時，我要額外做一件事」
        async show(val) {                 //「只要 show 這個 prop 的值改變，如果它變成 true，我就把表單清空。」
            if (val) {
                await this.loadRoleOptions(); 
                await this.loadDepartmentOptions();

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
        async loadRoleOptions() {
            try {
                let res = await getRoleOptions();

                if (!res.success) {
                    alert(`錯誤代碼 -> ${res.errorCode}`);
                    return;
                }

                this.roleOptions = res.data;
            } catch (err) {
                console.error(err);
                alert('系統錯誤 -> 角色選單載入');
            }
        },

        async loadDepartmentOptions() {
            try {
                let res = await getDepartmentOptions();

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

        onRoleSelected(event) {
            const roleId = event.target.value;
            if (!roleId) return;

            // 避免重複加入：檢查 roleAssignments 是否已有此 roleId
            const exists = this.form.roleAssignments.some(x => x.roleId === roleId);
            if (!exists) {
                this.form.roleAssignments.push({
                    roleId: roleId,
                    departmentIds: []   // 先空陣列（之後你要加部門 UI 來填）
                });
            }

            event.target.value = '';
        },

        removeRole(roleId) {
            this.form.roleAssignments = this.form.roleAssignments.filter(x => x.roleId !== roleId);
        },

        getRoleName(roleId) {
            const role = this.roleOptions.find(r => r.roleId === roleId);
            return role ? role.roleName : roleId;
        },

        resetForm() {
            this.form = {
                username: '',
                password: '',
                roleAssignments: []
            };
        },

        async submit() {
            if (!this.form.roleAssignments || this.form.roleAssignments.length === 0) {
                alert('請至少選擇一個角色');
                return;
            }

            if (this.form.roleAssignments.some(r => !r.departmentIds || r.departmentIds.length === 0)) {
                alert('每個角色至少選一個部門');
                return;
            }

            this.submitting = true;

            try {
                let res = await createAccount(this.form);

                if (!res.success) {
                    alert(`錯誤代碼 -> ${res.errorCode}`);
                    return;
                }

                this.$emit('created');
            } catch (err) {
                console.error(err);
                alert('系統錯誤 -> 創建帳戶');
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
                        <h5 class="modal-title">新增帳戶</h5>
                        <button type="button"
                                class="btn-close"
                                :disabled="submitting"
                                data-bs-dismiss="modal">
                        </button>
                    </div>

                    <div class="modal-body">

                        <div class="alert alert-primary mb-3">
                            <label class="form-label">帳號</label>

                            <input class="form-control"
                                    v-model="form.username">
                        </div>

                        <div class="alert alert-primary mb-3">
                            <label class="form-label">密碼</label>

                            <input class="form-control"
                                    type="password"
                                    v-model="form.password">
                        </div>

                        <div class="alert alert-primary mb-3">

                            <div class="mb-2">
                                <label class="form-label">角色</label>

                                <select class="form-select"
                                        @change="onRoleSelected($event)">
                                    <option value="">請選擇角色</option>
                                    <option v-for="r in roleOptions"
                                            :key="r.roleId"
                                            :value="r.roleId">
                                        {{ r.roleName }}
                                    </option>
                                </select>
                            </div>

                            <div class="d-flex flex-wrap gap-3 mb-3"
                                v-for="ra in form.roleAssignments" 
                                :key="ra.roleId">

                                <strong>{{ getRoleName(ra.roleId) }}</strong>

                                <button type="button"
                                        class="btn-close"
                                        @click="removeRole(ra.roleId)">
                                </button>

                                <div class="d-flex flex-wrap gap-3">
                                    <div v-for="d in departmentOptions"
                                        :key="d.departmentId">

                                        <label>
                                            <input type="checkbox"
                                                    :value="d.departmentId"
                                                    v-model="ra.departmentIds">

                                            {{ d.departmentName }}
                                        </label>
                                    </div>
                                </div>
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
