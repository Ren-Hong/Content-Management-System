import { deleteAccount } from '../api/accountApi.js';

export const AccountDeleteModal = {
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

    emits: ['close', 'deleted'],

    data() {
        return {
            modal: null,
            submitting: false
        };
    },

    watch: {
        show(val) {
            if (!this.modal) return;

            if (val) {
                this.modal.show();
            } else {
                this.modal.hide();
            }
        }
    },

    mounted() {
        this.modal = new bootstrap.Modal(this.$refs.modal, {
            backdrop: 'static',
            keyboard: false
        });

        // 使用者點 X / 點背景關閉
        this.$refs.modal.addEventListener('hidden.bs.modal', () => {
            this.$emit('close');
        });
    },

    methods: {
        async submit() {
            if (!this.account) return;

            this.submitting = true;

            try {
                const res = await deleteAccount({
                    accountId: this.account.id
                });

                if (!res.success) {
                    alert(res.errorCode || '刪除失敗');
                    return;
                }

                this.$emit('deleted');
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
                <div class="modal-content border-danger">

                    <div class="modal-header bg-danger text-white">
                        <h5 class="modal-title">刪除帳戶</h5>
                        <button type="button"
                                class="btn-close btn-close-white"
                                :disabled="submitting"
                                data-bs-dismiss="modal"></button>
                    </div>

                    <div class="modal-body">
                        <p class="mb-2">
                            確定要刪除以下帳戶嗎？
                        </p>

                        <div class="alert alert-danger mb-3">
                            <strong>帳號名稱 : {{ account?.username }}</strong>
                        </div>

                        <div class="alert alert-danger small">
                            ⚠ 此動作無法復原
                        </div>
                    </div>

                    <div class="modal-footer">
                        <button class="btn btn-secondary btn-sm"
                                :disabled="submitting"
                                data-bs-dismiss="modal">
                            取消
                        </button>

                        <button class="btn btn-danger btn-sm"
                                :disabled="submitting"
                                @click="submit">
                            {{ submitting ? '刪除中...' : '確認刪除' }}
                        </button>
                    </div>

                </div>
            </div>
        </div>
    `
};

