import { createContent } from '../api/contentApi.js';
import { getContentFields } from '../api/contentFieldApi.js';

export const DynamicFormPage = {
    emits: ['edit', 'delete'],

    props: {
        contentType: {
            type: Object,
            required: true
        }
    },

    data() {
        return {
            fields: [],
            fieldValues: {},
            loading: false,
            submitting: false
        }
    },

    mounted() {
        this.loadFields();
    },

    watch: {
        contentType() {
            this.loadFields();
        }
    },

    methods: {

        async loadFields() {

            if (!this.contentType) return;

            this.loading = true;

            try {

                const res = await getContentFields(this.contentType.typeId);

                this.fields = res.data || [];
                this.resetFieldValues();

            } finally {

                this.loading = false;

            }
        },

        resetFieldValues() {
            this.fieldValues = this.fields.reduce((values, field) => {
                values[field.fieldId] = '';
                return values;
            }, {});
        },

        async submitContent() {
            if (!this.contentType?.typeId) return;

            this.submitting = true;

            try {
                const response = await createContent({
                    typeId: this.contentType.typeId,
                    fieldValues: this.fields.map(field => ({
                        fieldId: field.fieldId,
                        fieldValue: this.fieldValues[field.fieldId] == null
                            ? ''
                            : String(this.fieldValues[field.fieldId])
                    }))
                });

                if (!response.success) {
                    alert(response.errorCode || '儲存表單內容失敗');
                    return;
                }

                alert('表單內容已儲存');
                this.resetFieldValues();
            } catch (error) {
                console.error(error);
                alert('系統錯誤 -> 儲存表單內容');
            } finally {
                this.submitting = false;
            }
        }

    },

    template: `
        <div class="card p-3 border-top-0">

            <div class="d-flex align-items-center justify-content-between mb-3">
                <h4 class="mb-0">
                    {{ contentType.typeName }}
                </h4>

                <div class="d-flex gap-2">
                    <button
                        type="button"
                        class="btn btn-outline-success btn-sm"
                        @click="$emit('edit', contentType)"
                    >
                        編輯
                    </button>

                    <button
                        type="button"
                        class="btn btn-outline-danger btn-sm"
                        @click="$emit('delete', contentType)"
                    >
                        刪除
                    </button>
                </div>
            </div>

            <div v-if="loading">
                載入中...
            </div>

            <form
                v-else
                @submit.prevent="submitContent"
            >

                <div
                    class="mb-3"
                    v-for="f in fields"
                    :key="f.fieldId"
                >

                    <label class="form-label">
                        {{ f.fieldName || f.fieldCode }}
                        <span
                            v-if="f.isRequired"
                            class="text-danger"
                        >
                            *
                        </span>
                    </label>

                    <input
                        v-if="f.fieldType === 'text'"
                        class="form-control"
                        type="text"
                        v-model="fieldValues[f.fieldId]"
                        :disabled="submitting"
                    />

                    <input
                        v-else-if="f.fieldType === 'number'"
                        class="form-control"
                        type="number"
                        v-model="fieldValues[f.fieldId]"
                        :disabled="submitting"
                    />

                    <input
                        v-else-if="f.fieldType === 'date'"
                        class="form-control"
                        type="date"
                        v-model="fieldValues[f.fieldId]"
                        :disabled="submitting"
                    />

                    <textarea
                        v-else
                        class="form-control"
                        rows="3"
                        v-model="fieldValues[f.fieldId]"
                        :disabled="submitting"
                    ></textarea>

                </div>

                <div class="d-flex justify-content-end pt-2">
                    <button
                        type="submit"
                        class="btn btn-primary"
                        :disabled="submitting || loading || fields.length === 0"
                    >
                        {{ submitting ? '送出中...' : '送出' }}
                    </button>
                </div>

            </form>

        </div>
    `
};
