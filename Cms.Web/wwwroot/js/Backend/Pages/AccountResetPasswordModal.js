import { resetPassword } from '../api/accountApi.js';

export const AccountResetPasswordModal = {
    props: {
        show: {
            type: Boolean,
            required: true
        },
        account: {
            type: Object,
            default: null
        }
    },

    emits: ['close', 'updated'],

    data() {
        return {
            modal: null,
            submitting: false,
            form: {
                password: ''
            }
        };
    },

    watch: {
        show(val) {
            if (!this.modal) return;

            if (val) {
                this.resetForm();
                this.modal.show();
            } else {
                this.modal.hide();
            }
        }
    },

    mounted() {
        this.modal = new bootstrap.Modal(this.$refs.modal);

        // 使用者點右上角 X 或背景
        this.$refs.modal.addEventListener('hidden.bs.modal', () => {
            this.$emit('close');
        });
    },

    methods: {
        resetForm() {
            this.form.password = '';
        },

        async submit() {
            if (!this.form.password) {
                alert('請輸入新密碼');
                return;
            }

            this.submitting = true;

            try {
                let res = await resetPassword({
                    username: this.account.username,
                    newPassword: this.form.password
                });

                if (!res.success) {
                    alert(res.errorCode || '重設密碼失敗');
                    return;
                }

                this.$emit('updated');
                this.modal.hide();
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
                <div class="modal-content border-warning">

                    <div class="modal-header bg-warning text-white">
                        <h5 class="modal-title">重設密碼</h5>
                        <button type="button"
                                class="btn-close"
                                :disabled="submitting"
                                data-bs-dismiss="modal"></button>
                    </div>

                    <div class="modal-body">

                        <div class="alert alert-warning mb-3">
                            <strong>帳號名稱 : {{ account?.username }}</strong>
                        </div>

                        <input class="form-control mb-3"
                               type="password"
                               placeholder="新密碼"
                               v-model="form.password">

                        <div class="alert alert-warning small">
                            重設後，需用新密碼登入
                        </div>
                    </div>

                    <div class="modal-footer">
                        <button class="btn btn-secondary btn-sm"
                                :disabled="submitting"
                                data-bs-dismiss="modal">
                            取消
                        </button>
                        <button class="btn btn-warning btn-sm text-white"
                                :disabled="submitting"
                                @click="submit">
                            {{ submitting ? '處理中...' : '確認重設' }}
                        </button>
                    </div>

                </div>
            </div>
        </div>
    `
};
