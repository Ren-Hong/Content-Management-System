import { createAccount } from '../api/accountApi.js';

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
            form: {
                username: '',
                password: '',
                roleCode: ''
            },
            submitting: false
        };
    },

    watch: {                        // watch =「當某個值改變時，我要額外做一件事」
        show(val) {                 //「只要 show 這個 prop 的值改變，如果它變成 true，我就把表單清空。」
            if (val) {
                this.resetForm();
                this.modal.show();
            } else {
                this.modal.hide();
            }
        }
    },

    mounted() {
        // Bootstrap 5 Modal 初始化
        this.modal = new bootstrap.Modal(this.$refs.modal, {
            backdrop: 'static', // 點背景不關（可依需求）
            keyboard: true
        });

        // 當使用者用 ESC / X 關閉 modal
        this.$refs.modal.addEventListener('hidden.bs.modal', () => {
            this.$emit('close');
        });
    },

    methods: {
        resetForm() {
            this.form = {
                username: '',
                password: '',
                roleCode: ''
            };
        },

        async submit() {
            if (!this.form.roleCode) {
                alert('請選擇角色');
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
                <div class="modal-content">

                    <div class="modal-header">
                        <h5 class="modal-title">新增帳戶</h5>
                        <button type="button"
                                class="btn-close"
                                :disabled="submitting"
                                data-bs-dismiss="modal">
                        </button>
                    </div>

                    <div class="modal-body">

                        <input class="form-control mb-2"
                                placeholder="帳號"
                                v-model="form.username">

                        <input class="form-control mb-2"
                                type="password"
                                placeholder="密碼"
                                v-model="form.password">

                        <select class="form-select"
                                v-model="form.roleCode">
                            <option value="">請選擇角色</option>
                            <option value="Admin">管理員</option>
                            <option value="User">使用者</option>
                        </select>

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
