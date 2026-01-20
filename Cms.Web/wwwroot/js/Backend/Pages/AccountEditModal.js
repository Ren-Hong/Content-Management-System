import { updateAccount } from '../api/accountApi.js';
import { getRoleOptions } from '../api/RoleApi.js';

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
            submitting: false,
            roleOptions: [],
            form: {
                username: '',
                roleCode: '',
                status: ''
            }
        };
    },

    watch: {
        async show(val) {
            if (val) {
                await this.loadRoleOptions(); 

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
                    alert(res.errorCode || '角色選單載入失敗');
                    return;
                }

                this.roleOptions = res.data;
            } catch (err) {
                console.error(err);
                alert('系統錯誤（角色載入）');
            }
        },

        fillForm() { // 填入本來的資料
            this.form.username = this.account.username;
            this.form.roleCode = this.account.roles?.[0]?.roleCode ?? ''; // 未來可能有很多角色
            this.form.status = this.account.status ?? '';
        },

        async submit() {
            this.submitting = true;

            try {
                let res = await updateAccount(this.form);

                if (!res.success) {
                    alert(res.errorCode || '更新帳戶失敗');
                    return;
                }

                this.$emit('updated');
                //this.modal.hide();
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
                                    <strong>帳號名稱 : {{ account?.username }}</strong>
                                </div>
                            </div>

                            <div class="col-6">
                                <div class="alert alert-success mb-3">
                                    <strong>帳號狀態 : {{ account ? AccountStatusText[account.status] : '' }}</strong>
                                </div>
                            </div>
                        </div>

                        <div class="alert alert-success mb-3">
                            <strong>目前角色 : {{ account?.roles?.map(r => r.roleName).join(', ')}}</strong>
                        </div>

                        <div class="alert alert-success mb-3">
                            <label class="form-label">角色</label>
                            <select class="form-select" v-model="form.roleCode">
                                <option v-for="role in roleOptions"
                                        :key="role.roleCode"
                                        :value="role.roleCode">
                                    {{ role.roleName }}
                                </option>
                            </select>
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
