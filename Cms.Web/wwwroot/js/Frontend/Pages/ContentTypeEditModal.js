import { getContentFields } from '../api/contentFieldApi.js';
import { updateContentType } from '../api/contentTypeApi.js';

function createEmptyField() {
    return {
        fieldId: null,
        fieldName: '',
        fieldType: 'text',
        isRequired: false
    };
}

export const ContentTypeEditModal = {
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

    emits: ['close', 'updated'],

    data() {
        return {
            modal: null,
            loading: false,
            submitting: false,
            form: {
                typeId: '',
                typeName: '',
                fields: [createEmptyField()]
            }
        };
    },

    watch: {
        async show(val) {
            if (!this.modal) return;

            if (val) {
                await this.fillForm();
                this.modal.show();
                return;
            }

            this.modal.hide();
        }
    },

    mounted() {
        this.modal = new bootstrap.Modal(this.$refs.modal);

        this.$refs.modal.addEventListener('hidden.bs.modal', () => {
            this.$emit('close');
        });
    },

    methods: {
        async fillForm() {
            if (!this.contentType?.typeId) return;

            this.loading = true;

            try {
                const res = await getContentFields(this.contentType.typeId);
                const fields = (res.data || []).map(x => ({
                    fieldId: x.fieldId,
                    fieldName: x.fieldName || x.fieldCode,
                    fieldType: x.fieldType,
                    isRequired: x.isRequired
                }));

                this.form = {
                    typeId: this.contentType.typeId,
                    typeName: this.contentType.typeName || '',
                    fields: fields.length > 0 ? fields : [createEmptyField()]
                };
            } finally {
                this.loading = false;
            }
        },

        addField() {
            this.form.fields.push(createEmptyField());
        },

        removeField(index) {
            if (this.form.fields.length === 1) {
                this.form.fields = [createEmptyField()];
                return;
            }

            this.form.fields.splice(index, 1);
        },

        async submit() {
            if (!this.form.typeName.trim()) {
                alert('請輸入問卷名稱');
                return;
            }

            if (this.form.fields.some(x => !x.fieldName.trim())) {
                alert('請把每個欄位名稱都填寫完整');
                return;
            }

            this.submitting = true;

            try {
                const res = await updateContentType({
                    typeId: this.form.typeId,
                    typeName: this.form.typeName.trim(),
                    fields: this.form.fields.map(x => ({
                        fieldId: x.fieldId,
                        fieldName: x.fieldName.trim(),
                        fieldType: x.fieldType,
                        isRequired: x.isRequired
                    }))
                });

                if (!res.success) {
                    alert(res.errorCode || '更新問卷失敗');
                    return;
                }

                this.$emit('updated', {
                    typeId: this.form.typeId
                });
                this.modal.hide();
            } catch (err) {
                console.error(err);
                alert('系統錯誤 -> 更新問卷');
            } finally {
                this.submitting = false;
            }
        }
    },

    template: `
        <div class="modal fade" tabindex="-1" ref="modal">
            <div class="modal-dialog modal-dialog-centered modal-lg">
                <div class="modal-content border-success">
                    <div class="modal-header bg-success text-white">
                        <h5 class="modal-title">編輯內科問卷</h5>
                        <button
                            type="button"
                            class="btn-close"
                            :disabled="submitting"
                            data-bs-dismiss="modal">
                        </button>
                    </div>

                    <div class="modal-body">
                        <div v-if="loading" class="text-center py-4">
                            載入中...
                        </div>

                        <template v-else>
                            <div class="mb-4">
                                <label class="form-label">問卷名稱</label>
                                <input
                                    class="form-control"
                                    v-model="form.typeName"
                                    :disabled="submitting"
                                />
                            </div>

                            <div class="d-flex align-items-center justify-content-between mb-3">
                                <div>
                                    <h6 class="mb-1">欄位設定</h6>
                                    <small class="text-muted">可以新增、修改或移除欄位</small>
                                </div>

                                <button
                                    type="button"
                                    class="btn btn-outline-success btn-sm"
                                    :disabled="submitting"
                                    @click="addField">
                                    新增欄位
                                </button>
                            </div>

                            <div class="d-flex flex-column gap-3">
                                <div
                                    class="border rounded-3 p-3 bg-light"
                                    v-for="(field, index) in form.fields"
                                    :key="field.fieldId || index"
                                >
                                    <div class="row g-3 align-items-end">
                                        <div class="col-md-5">
                                            <label class="form-label">欄位名稱</label>
                                            <input
                                                class="form-control"
                                                v-model="field.fieldName"
                                                :disabled="submitting"
                                            />
                                        </div>

                                        <div class="col-md-4">
                                            <label class="form-label">欄位型別</label>
                                            <select
                                                class="form-select"
                                                v-model="field.fieldType"
                                                :disabled="submitting"
                                            >
                                                <option value="text">單行文字</option>
                                                <option value="textarea">多行文字</option>
                                                <option value="number">數字</option>
                                                <option value="date">日期</option>
                                            </select>
                                        </div>

                                        <div class="col-md-2">
                                            <div class="form-check mb-2">
                                                <input
                                                    class="form-check-input"
                                                    type="checkbox"
                                                    v-model="field.isRequired"
                                                    :disabled="submitting"
                                                    :id="'edit-required-' + index"
                                                />
                                                <label
                                                    class="form-check-label"
                                                    :for="'edit-required-' + index"
                                                >
                                                    必填
                                                </label>
                                            </div>
                                        </div>

                                        <div class="col-md-1 text-end">
                                            <button
                                                type="button"
                                                class="btn btn-outline-danger btn-sm"
                                                :disabled="submitting"
                                                @click="removeField(index)"
                                            >
                                                刪除
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </template>
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
                            class="btn btn-success btn-sm"
                            :disabled="submitting || loading"
                            @click="submit"
                        >
                            {{ submitting ? '更新中...' : '儲存變更' }}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `
};
