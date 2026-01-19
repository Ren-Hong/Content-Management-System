import { createAccount } from '../../api/accountApi.js';

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
            form: {
                username: '',
                password: '',
                roleCode: ''
            },
            submitting: false
        };
    },

    watch: {
        show(val) {
            if (val) {
                this.resetForm();
            }
        }
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
                const res = await createAccount(this.form);

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
        <div v-if="show" class="modal-backdrop-custom">
            <div class="card p-3" style="max-width:400px;margin:auto;">
                <h5 class="mb-3">新增帳戶</h5>

                <input class="form-control mb-2"
                       placeholder="帳號"
                       v-model="form.username">

                <input class="form-control mb-2"
                       type="password"
                       placeholder="密碼"
                       v-model="form.password">

                <select class="form-select mb-3"
                        v-model="form.roleCode">
                    <option value="">請選擇角色</option>
                    <option value="Admin">管理員</option>
                    <option value="User">User</option>
                </select>

                <div class="d-flex justify-content-end gap-2">
                    <button class="btn btn-secondary btn-sm"
                            :disabled="submitting"
                            @click="$emit('close')">
                        取消
                    </button>
                    <button class="btn btn-primary btn-sm"
                            :disabled="submitting"
                            @click="submit">
                        建立
                    </button>
                </div>
            </div>
        </div>
    `
};
