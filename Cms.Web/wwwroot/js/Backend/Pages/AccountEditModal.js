import { updateAccount } from '../api/accountApi.js';
import { getRoleOptions } from '../api/roleApi.js';
import { getDepartmentOptions } from '../api/departmentApi.js';

import { AccountStatusText } from '../constants/accountStatus.js';

export const AccountEditModal = {
    props: {
        show: { // AccountManager 的 showEdit
            type: Boolean,
            required: true
        },
        account: { // AccountManager 的 selectedAccount
            type: Object,
            default : null
        }
    },

    emits: ['close', 'updated'],

    data() {
        return {
            AccountStatusText, 

            modal: null,
            roleOptions: [],
            departmentOptions: [],
            form: {
                username: '',
                status: '',
                roleAssignments: [
                    {
                        roleId: '',
                        departmentIds: []
                    }
                ]
            },
            submitting: false,
        };
    },

    watch: {
        async show(val) {
            if (val) {
                await this.loadRoleOptions(); 
                await this.loadDepartmentOptions();

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
                alert('API路徑或Json格式不對');
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

        fillForm() { // 填入本來的資料, 原始資料來自AccountManager
            this.form.username = this.account.username;
            this.form.status = this.account.status;

            this.form.roleAssignments =
                this.account.roleAssignments.map(ra => ({
                    roleId: ra.roleId,
                    departmentIds: ra.departments.map(d => d.departmentId)
                }));
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
                let res = await updateAccount(this.form);

                if (!res.success) {
                    alert(`錯誤代碼 -> ${res.errorCode}`);
                    return;
                }

                this.$emit('updated');
            } catch (err) {
                console.error(err);
                alert('系統錯誤 -> 更新帳戶');
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
                        <h5 class="modal-title">編輯帳戶</h5>
                        <button type="button"
                                class="btn-close"
                                :disabled="submitting"
                                data-bs-dismiss="modal"></button>
                    </div>

                    <div class="modal-body">
                        <div class="row">
                            <div class="col-6">
                                <div class="alert alert-success mb-3">
                                    <strong>帳號名稱 : </strong>
                                    <span>{{ account?.username }}</span>
                                </div>
                            </div>

                            <div class="col-6">
                                <div class="alert alert-success mb-3">
                                    <strong>帳號狀態 : </strong>
                                    <span>{{ account ? AccountStatusText[account.status] : '' }}</span>
                                </div>
                            </div>
                        </div>

                        <div class="alert alert-success mb-3">

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

                        <div class="alert alert-success mb-3">
                            <label class="form-label">狀態</label>
                            <select class="form-select" v-model="form.status">
                                <option value="Enable">啟用</option>
                                <option value="Disabled">停用</option>
                                <option value="Locked">鎖帳</option>
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
