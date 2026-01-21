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
                roleCodes: []   // ⭐ 多選
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
                    alert(res.errorCode || '角色選單載入失敗');
                    return;
                }

                this.roleOptions = res.data;
            } catch (err) {
                console.error(err);
                alert('系統錯誤（角色載入）');
            }
        },

        onRoleSelected(event) {
            const roleCode = event.target.value;
            if (!roleCode) return;

            // 避免重複加入
            if (!this.form.roleCodes.includes(roleCode)) {
                this.form.roleCodes.push(roleCode);
            }

            // 重置 select
            event.target.value = '';
        },

        removeRole(roleCode) {
            this.form.roleCodes =
                this.form.roleCodes.filter(code => code !== roleCode);
        },

        getRoleName(roleCode) {
            const role = this.roleOptions.find(r => r.roleCode === roleCode);
            return role ? role.roleName : roleCode;
        },

        resetForm() {
            this.form = {
                username: '',
                password: '',
                roleCodes: []   // ✅ 一定要是陣列
            };
        },

        async submit() {
            if (this.form.roleCodes.length === 0) {
                alert('請至少選擇一個角色');
                return;
            }

            this.submitting = true;

            try {
                let res = await createAccount(this.form);

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
                                    <option v-for="role in roleOptions"
                                            :key="role.roleCode"
                                            :value="role.roleCode">
                                        {{ role.roleName }}
                                    </option>
                                </select>
                            </div>

                            <div class="d-flex flex-wrap gap-2">
                                <span v-for="roleCode in form.roleCodes"
                                      :key="roleCode"
                                      class="badge bg-primary d-flex align-items-center">

                                    {{ getRoleName(roleCode) }}

                                    <button type="button"
                                            class="btn-close btn-close-white ms-2"
                                            @click="removeRole(roleCode)">
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
