import { createContentType } from '../api/contentTypeApi.js';

function createEmptyField() {
    return {
        fieldName: '',
        fieldType: 'text',
        isRequired: false
    };
}

export const ContentTypeCreateModal = {
    props: {
        show: {
            type: Boolean,
            required: true
        },
        departmentId: {
            type: String,
            required: true
        }
    },

    emits: ['close', 'created'],

    data() {
        return {
            modal: null,
            submitting: false,
            form: {
                typeName: '',
                fields: [createEmptyField()]
            }
        };
    },

    watch: {
        show(val) {
            if (!this.modal) return;

            if (val) {
                this.resetForm();
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

        if (this.show) {
            this.resetForm();
            this.modal.show();
        }
    },

    methods: {
        resetForm() {
            this.form = {
                typeName: '',
                fields: [createEmptyField()]
            };
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
                alert('請輸入表單種類名稱');
                return;
            }

            if (this.form.fields.some(x => !x.fieldName.trim())) {
                alert('請把每個欄位名稱都填寫完整');
                return;
            }

            this.submitting = true;

            try {
                const res = await createContentType({
                    departmentId: this.departmentId,
                    typeName: this.form.typeName.trim(),
                    fields: this.form.fields.map(x => ({
                        fieldName: x.fieldName.trim(),
                        fieldType: x.fieldType,
                        isRequired: x.isRequired
                    }))
                });

                if (!res.success) {
                    alert(res.errorCode || '建立表單種類失敗');
                    return;
                }

                this.$emit('created', res.data);
                this.modal.hide();
            } catch (err) {
                console.error(err);
                alert('系統錯誤 -> 建立表單種類');
            } finally {
                this.submitting = false;
            }
        }
    },

    template: `
        <div class="modal fade" tabindex="-1" ref="modal">
            <div class="modal-dialog modal-dialog-centered modal-lg">
                <div class="modal-content border-primary">

                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title">新增內科問卷</h5>
                        <button
                            type="button"
                            class="btn-close"
                            :disabled="submitting"
                            data-bs-dismiss="modal">
                        </button>
                    </div>

                    <div class="modal-body">
                        <div class="mb-4">
                            <label class="form-label">表單種類名稱</label>
                            <input
                                class="form-control"
                                v-model="form.typeName"
                                :disabled="submitting"
                                placeholder="例如：住院評估表"
                            />
                        </div>

                        <div class="d-flex align-items-center justify-content-between mb-3">
                            <div>
                                <h6 class="mb-1">欄位設定</h6>
                                <small class="text-muted">設定這個表單要有哪些欄位</small>
                            </div>

                            <button
                                type="button"
                                class="btn btn-outline-primary btn-sm"
                                :disabled="submitting"
                                @click="addField">
                                新增欄位
                            </button>
                        </div>

                        <div class="d-flex flex-column gap-3">
                            <div
                                class="border rounded-3 p-3 bg-light"
                                v-for="(field, index) in form.fields"
                                :key="index"
                            >
                                <div class="row g-3 align-items-end">
                                    <div class="col-md-5">
                                        <label class="form-label">欄位名稱</label>
                                        <input
                                            class="form-control"
                                            v-model="field.fieldName"
                                            :disabled="submitting"
                                            placeholder="例如：病患姓名"
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
                                                :id="'required-' + index"
                                            />
                                            <label
                                                class="form-check-label"
                                                :for="'required-' + index"
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
                            class="btn btn-primary btn-sm"
                            :disabled="submitting"
                            @click="submit"
                        >
                            {{ submitting ? '建立中...' : '建立表單種類' }}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `
};
