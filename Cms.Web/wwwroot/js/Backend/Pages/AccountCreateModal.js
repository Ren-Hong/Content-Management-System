import { createAccount } from '../api/accountApi.js';
import { getRoleOptions } from '../api/RoleApi.js';

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
            form: {
                username: '',
                password: '',
                roleIds: []   // ⭐ 多選
            },
            submitting: false
        };
    },

    watch: {                        // watch =「當某個值改變時，我要額外做一件事」
        async show(val) {                 //「只要 show 這個 prop 的值改變，如果它變成 true，我就把表單清空。」
            if (val) {
                await this.loadRoleOptions(); 

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

        onRoleSelected(event) {
            const roleId = event.target.value;
            if (!roleId) return;

            // 避免重複加入
            if (!this.form.roleIds.includes(roleId)) {
                this.form.roleIds.push(roleId);
            }

            // 重置 select
            event.target.value = '';
        },

        removeRole(roleId) {
            this.form.roleIds = this.form.roleIds.filter(id => id !== roleId);
        },

        getRoleName(roleId) {
            const role = this.roleOptions.find(r => r.roleId === roleId);
            return role ? role.roleName : roleId;
        },

        resetForm() {
            this.form = {
                username: '',
                password: '',
                roleIds: []   // ✅ 一定要是陣列
            };
        },

        async submit() {
            if (this.form.roleIds.length === 0) {
                alert('請至少選擇一個角色');
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
                                        {{ role.roleName }}
                                    </option>
                                </select>
                            </div>

                            <div class="d-flex flex-wrap gap-2">
                                <span v-for="roleId in form.roleIds"
                                      :key="roleId"
                                      class="badge bg-primary d-flex align-items-center">

                                    {{ getRoleName(roleId) }}

                                    <button type="button"
                                            class="btn-close btn-close-white ms-2"
                                            @click="removeRole(roleId)">
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
