import { createContent, deleteContent, getContents, updateContent } from '../api/contentApi.js';
import { hasPermission } from '../authContext.js';
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
            submitting: false,
            contentsLoading: false,
            contents: [],
            selectedContentRevisionId: null,
            editingRevisionId: null,
            editingFieldValues: {},
            savingContent: false,
            deletingContent: false
        }
    },

    computed: {
        selectedContent() {
            return this.contents.find(content => content.revisionId === this.selectedContentRevisionId) || null;
        },

        isEditingSelectedContent() {
            return this.selectedContent != null && this.editingRevisionId === this.selectedContent.revisionId;
        },

        canViewContent() {
            return hasPermission('Content.View');
        },

        canCreateContent() {
            return hasPermission('Content.Create');
        },

        canEditContent() {
            return hasPermission('Content.Edit');
        },

        canDeleteContent() {
            return hasPermission('Content.Delete');
        }
    },

    mounted() {
        this.loadPageData();
    },

    watch: {
        contentType() {
            this.loadPageData();
        }
    },

    methods: {
        resolveContentErrorMessage(errorCode, fallbackMessage) {
            if (errorCode === 'PermissionDenied') {
                return '權限不足';
            }

            return errorCode || fallbackMessage;
        },

        async loadPageData() {
            if (!this.contentType) return;

            const tasks = [this.loadFields()];

            if (this.canViewContent) {
                tasks.push(this.loadContents());
            } else {
                this.contents = [];
                this.selectedContentRevisionId = null;
                this.cancelEditContent();
            }

            await Promise.all(tasks);
        },

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

        async loadContents() {

            if (!this.contentType) return;

            this.contentsLoading = true;

            try {

                const res = await getContents(this.contentType.typeId);

                this.contents = res.data || [];
                this.syncSelectedContent();

            } finally {

                this.contentsLoading = false;

            }
        },

        resetFieldValues() {
            this.fieldValues = this.fields.reduce((values, field) => {
                values[field.fieldId] = '';
                return values;
            }, {});
        },

        syncSelectedContent() {
            if (this.contents.length === 0) {
                this.selectedContentRevisionId = null;
                this.cancelEditContent();
                return;
            }

            const hasSelectedContent = this.contents.some(
                content => content.revisionId === this.selectedContentRevisionId
            );

            if (!hasSelectedContent) {
                this.selectedContentRevisionId = this.contents[0].revisionId;
            }
        },

        selectContent(revisionId) {
            this.selectedContentRevisionId = revisionId;
            this.cancelEditContent();
        },

        startEditContent() {
            if (!this.selectedContent) return;

            this.editingRevisionId = this.selectedContent.revisionId;
            this.editingFieldValues = this.selectedContent.fieldValues.reduce((values, field) => {
                values[field.fieldId] = field.fieldValue ?? '';
                return values;
            }, {});
        },

        cancelEditContent() {
            this.editingRevisionId = null;
            this.editingFieldValues = {};
        },

        async saveEditedContent() {
            if (!this.selectedContent) return;

            this.savingContent = true;

            try {
                const response = await updateContent({
                    contentId: this.selectedContent.contentId,
                    revisionId: this.selectedContent.revisionId,
                    fieldValues: this.selectedContent.fieldValues.map(field => ({
                        fieldId: field.fieldId,
                        fieldValue: this.editingFieldValues[field.fieldId] == null
                            ? ''
                            : String(this.editingFieldValues[field.fieldId])
                    }))
                });

                if (!response.success) {
                    alert(this.resolveContentErrorMessage(response.errorCode, '更新表單內容失敗'));
                    return;
                }

                alert('表單內容已更新');
                await this.loadContents();
                this.cancelEditContent();
            } catch (error) {
                console.error(error);
                alert('系統錯誤 -> 更新表單內容');
            } finally {
                this.savingContent = false;
            }
        },

        async removeSelectedContent() {
            if (!this.selectedContent) return;

            const confirmed = window.confirm('確定要刪除這筆內容嗎？');
            if (!confirmed) return;

            this.deletingContent = true;

            try {
                const response = await deleteContent({
                    contentId: this.selectedContent.contentId
                });

                if (!response.success) {
                    alert(this.resolveContentErrorMessage(response.errorCode, '刪除表單內容失敗'));
                    return;
                }

                alert('表單內容已刪除');
                await this.loadContents();
            } catch (error) {
                console.error(error);
                alert('系統錯誤 -> 刪除表單內容');
            } finally {
                this.deletingContent = false;
            }
        },

        formatCreatedAt(value) {
            if (!value) return '-';

            const date = new Date(value);

            if (Number.isNaN(date.getTime())) {
                return value;
            }

            return date.toLocaleString('zh-TW', {
                hour12: false
            });
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
                    alert(this.resolveContentErrorMessage(response.errorCode, '儲存表單內容失敗'));
                    return;
                }

                alert('表單內容已儲存');
                if (this.canViewContent) {
                    await this.loadContents();
                }
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
        <div class="dynamic-form-shell">
            <div v-if="loading" class="dynamic-form-loading">
                載入中...
            </div>

            <div v-else class="row g-0 align-items-start">

                <div class="col-12 col-xl-5">
                    <section class="dynamic-form-section dynamic-form-section-form">
                        <div class="dynamic-section-title">問卷填寫</div>

                        <div class="dynamic-form-toolbar">
                            <span class="dynamic-form-chip">
                                {{ fields.length }} 個欄位
                            </span>

                            <div class="d-flex align-items-center gap-2 flex-wrap justify-content-end">
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

                        <form @submit.prevent="submitContent">
                            <div
                                class="dynamic-form-input-group"
                                v-for="f in fields"
                                :key="f.fieldId"
                            >

                                <label class="form-label dynamic-form-label">
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
                                    class="form-control dynamic-form-control"
                                    type="text"
                                    v-model="fieldValues[f.fieldId]"
                                    :disabled="submitting"
                                />

                                <input
                                    v-else-if="f.fieldType === 'number'"
                                    class="form-control dynamic-form-control"
                                    type="number"
                                    v-model="fieldValues[f.fieldId]"
                                    :disabled="submitting"
                                />

                                <input
                                    v-else-if="f.fieldType === 'date'"
                                    class="form-control dynamic-form-control"
                                    type="date"
                                    v-model="fieldValues[f.fieldId]"
                                    :disabled="submitting"
                                />

                                <textarea
                                    v-else
                                    class="form-control dynamic-form-control"
                                    rows="4"
                                    v-model="fieldValues[f.fieldId]"
                                    :disabled="submitting"
                                ></textarea>

                            </div>

                            <div class="d-flex justify-content-end pt-2">
                                <button
                                    v-if="canCreateContent"
                                    type="submit"
                                    class="btn btn-primary px-4"
                                    :disabled="submitting || loading || fields.length === 0"
                                >
                                    {{ submitting ? '送出中...' : '送出' }}
                                </button>

                                <div
                                    v-else
                                    class="text-muted small"
                                >
                                    目前沒有內容新增權限
                                </div>
                            </div>
                        </form>
                    </section>
                </div>

                <div class="col-12 col-xl-7">
                    <section class="dynamic-form-section dynamic-form-section-content">
                        <div class="dynamic-section-title">填寫紀錄</div>

                        <div class="dynamic-form-toolbar dynamic-form-toolbar-end">
                            <button
                                v-if="canEditContent && selectedContent && !isEditingSelectedContent"
                                type="button"
                                class="btn btn-outline-success btn-sm"
                                @click="startEditContent"
                                :disabled="contentsLoading || deletingContent"
                            >
                                編輯內容
                            </button>

                            <button
                                v-if="canDeleteContent && selectedContent && !isEditingSelectedContent"
                                type="button"
                                class="btn btn-outline-danger btn-sm"
                                @click="removeSelectedContent"
                                :disabled="contentsLoading || deletingContent"
                            >
                                {{ deletingContent ? '刪除中...' : '刪除內容' }}
                            </button>

                            <button
                                v-if="canEditContent && isEditingSelectedContent"
                                type="button"
                                class="btn btn-outline-secondary btn-sm"
                                @click="cancelEditContent"
                                :disabled="savingContent"
                            >
                                取消
                            </button>

                            <button
                                v-if="canEditContent && isEditingSelectedContent"
                                type="button"
                                class="btn btn-success btn-sm"
                                @click="saveEditedContent"
                                :disabled="savingContent"
                            >
                                {{ savingContent ? '儲存中...' : '儲存內容' }}
                            </button>

                            <button
                                v-if="canViewContent"
                                type="button"
                                class="btn btn-outline-secondary btn-sm"
                                @click="loadContents"
                                :disabled="contentsLoading"
                            >
                                {{ contentsLoading ? '更新中...' : '重新整理' }}
                            </button>
                        </div>

                        <div v-if="!canViewContent" class="dynamic-form-empty-state">
                            目前沒有內容查閱權限。
                        </div>

                        <div v-else-if="contentsLoading" class="text-muted">
                            載入表單內容中...
                        </div>

                        <div
                            v-else-if="contents.length === 0"
                            class="dynamic-form-empty-state"
                        >
                            目前還沒有表單內容。
                        </div>

                        <div v-else class="dynamic-content-browser">
                            <div class="dynamic-content-timeline">
                                <button
                                    type="button"
                                    class="dynamic-content-summary"
                                    v-for="content in contents"
                                    :key="content.revisionId"
                                    :class="{ active: selectedContent && selectedContent.revisionId === content.revisionId }"
                                    @click="selectContent(content.revisionId)"
                                >
                                    <div class="dynamic-content-summary-top">
                                        <span class="dynamic-content-summary-title">
                                            {{ formatCreatedAt(content.createdAt) }}
                                        </span>

                                        <span class="dynamic-content-status">
                                            {{ content.status }}
                                        </span>
                                    </div>

                                    <div class="dynamic-content-summary-subtitle">
                                        {{ content.ownerUsername }}
                                    </div>
                                </button>
                            </div>

                            <article v-if="selectedContent" class="dynamic-content-card">
                                <div class="dynamic-content-meta">
                                    <div class="dynamic-content-meta-item dynamic-content-meta-item-wide">
                                        <div class="dynamic-content-meta-label">建立時間</div>
                                        <div class="dynamic-content-meta-value">{{ formatCreatedAt(selectedContent.createdAt) }}</div>
                                    </div>

                                    <div class="dynamic-content-meta-item">
                                        <div class="dynamic-content-meta-label">填寫人</div>
                                        <div class="dynamic-content-meta-value">{{ selectedContent.ownerUsername }}</div>
                                    </div>

                                    <div class="dynamic-content-meta-item dynamic-content-meta-status">
                                        <div class="dynamic-content-meta-label">狀態</div>
                                        <span class="dynamic-content-status">{{ selectedContent.status }}</span>
                                    </div>
                                </div>

                                <div
                                    v-if="!isEditingSelectedContent"
                                    class="dynamic-content-fields"
                                >
                                    <section
                                        class="dynamic-content-field"
                                        v-for="field in selectedContent.fieldValues"
                                        :key="selectedContent.revisionId + '-' + field.fieldId"
                                    >
                                        <div class="dynamic-content-field-label">{{ field.fieldName }}</div>
                                        <div class="dynamic-content-field-value text-break">{{ field.fieldValue || '未填寫' }}</div>
                                    </section>
                                </div>

                                <div v-else class="dynamic-content-fields">
                                    <section
                                        class="dynamic-content-field"
                                        v-for="field in selectedContent.fieldValues"
                                        :key="selectedContent.revisionId + '-edit-' + field.fieldId"
                                    >
                                        <label class="form-label dynamic-content-field-label">
                                            {{ field.fieldName }}
                                        </label>

                                        <input
                                            v-if="field.fieldType === 'text'"
                                            class="form-control dynamic-form-control"
                                            type="text"
                                            v-model="editingFieldValues[field.fieldId]"
                                            :disabled="savingContent"
                                        />

                                        <input
                                            v-else-if="field.fieldType === 'number'"
                                            class="form-control dynamic-form-control"
                                            type="number"
                                            v-model="editingFieldValues[field.fieldId]"
                                            :disabled="savingContent"
                                        />

                                        <input
                                            v-else-if="field.fieldType === 'date'"
                                            class="form-control dynamic-form-control"
                                            type="date"
                                            v-model="editingFieldValues[field.fieldId]"
                                            :disabled="savingContent"
                                        />

                                        <textarea
                                            v-else
                                            class="form-control dynamic-form-control"
                                            rows="4"
                                            v-model="editingFieldValues[field.fieldId]"
                                            :disabled="savingContent"
                                        ></textarea>
                                    </section>
                                </div>
                            </article>
                        </div>

                    </section>
                </div>

            </div>

        </div>
    `
};
