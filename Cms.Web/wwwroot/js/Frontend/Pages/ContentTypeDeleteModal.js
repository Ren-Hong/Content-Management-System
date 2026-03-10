import { deleteContentType } from '../api/contentTypeApi.js';

export const ContentTypeDeleteModal = {
    props: {
        show: {
            type: Boolean,
            required: true
        },
        contentType: {
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
                return;
            }

            this.modal.hide();
        }
    },

    mounted() {
        this.modal = new bootstrap.Modal(this.$refs.modal, {
            backdrop: 'static',
            keyboard: false
        });

        this.$refs.modal.addEventListener('hidden.bs.modal', () => {
            this.$emit('close');
        });
    },

    methods: {
        async submit() {
            if (!this.contentType?.typeId) return;

            this.submitting = true;

            try {
                const res = await deleteContentType({
                    typeId: this.contentType.typeId
                });

                if (!res.success) {
                    alert(res.errorCode || '刪除問卷失敗');
                    return;
                }

                this.$emit('deleted', {
                    typeId: this.contentType.typeId
                });
                this.modal.hide();
            } catch (err) {
                console.error(err);
                alert('系統錯誤 -> 刪除問卷');
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
                        <h5 class="modal-title">刪除內科問卷</h5>
                        <button
                            type="button"
                            class="btn-close btn-close-white"
                            :disabled="submitting"
                            data-bs-dismiss="modal">
                        </button>
                    </div>

                    <div class="modal-body">
                        <p class="mb-2">確定要刪除這份問卷嗎？</p>

                        <div class="alert alert-danger mb-3">
                            <strong>問卷名稱：</strong>
                            <span>{{ contentType?.typeName }}</span>
                        </div>

                        <div class="alert alert-danger small mb-0">
                            如果這份問卷已經有內容資料，系統會拒絕刪除。
                        </div>
                    </div>

                    <div class="modal-footer">
                        <button
                            type="button"
                            class="btn btn-secondary btn-sm"
                            :disabled="submitting"
                            data-bs-dismiss="modal">
                            取消
                        </button>

                        <button
                            type="button"
                            class="btn btn-danger btn-sm"
                            :disabled="submitting"
                            @click="submit"
                        >
                            {{ submitting ? '刪除中...' : '確認刪除' }}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `
};
