import { getContentTypeOptions } from '../api/contentTypeApi.js';
import { ContentTypeCreateModal } from './ContentTypeCreateModal.js';
import { ContentTypeDeleteModal } from './ContentTypeDeleteModal.js';
import { ContentTypeEditModal } from './ContentTypeEditModal.js';
import { DynamicFormPage } from './DynamicFormPage.js';

export const InternalMedicine = {
    props: {
        departmentId: {
            type: String,
            required: true
        }
    },

    components: {
        ContentTypeCreateModal,
        ContentTypeDeleteModal,
        ContentTypeEditModal,
        DynamicFormPage
    },

    data() {
        return {
            contentTypes: [],
            currentType: null,
            loading: false,
            showCreateModal: false,
            showEditModal: false,
            showDeleteModal: false
        };
    },

    mounted() {
        if (this.departmentId) {
            this.loadContentTypeOptions();
        }
    },

    watch: {
        departmentId: {
            immediate: false,
            handler(newVal) {
                if (newVal) {
                    this.loadContentTypeOptions();
                }
            }
        }
    },

    methods: {
        async loadContentTypeOptions(selectedTypeId = null) {
            this.loading = true;

            try {
                const res = await getContentTypeOptions({
                    departmentId: this.departmentId
                });

                this.contentTypes = res.data || [];

                const targetTypeId = selectedTypeId || this.currentType?.typeId;
                const matchedType = targetTypeId
                    ? this.contentTypes.find(x => x.typeId === targetTypeId)
                    : null;

                this.currentType = matchedType || this.contentTypes[0] || null;
            } finally {
                this.loading = false;
            }
        },

        selectType(type) {
            this.currentType = type;
        },

        openCreateModal() {
            this.showCreateModal = true;
        },

        closeCreateModal() {
            this.showCreateModal = false;
        },

        async handleCreated(createdType) {
            this.showCreateModal = false;
            await this.loadContentTypeOptions(createdType?.typeId || null);
        },

        openEditModal() {
            if (!this.currentType) return;
            this.showEditModal = true;
        },

        closeEditModal() {
            this.showEditModal = false;
        },

        async handleUpdated(updatedType) {
            this.showEditModal = false;
            await this.loadContentTypeOptions(updatedType?.typeId || this.currentType?.typeId || null);
        },

        openDeleteModal() {
            if (!this.currentType) return;
            this.showDeleteModal = true;
        },

        closeDeleteModal() {
            this.showDeleteModal = false;
        },

        async handleDeleted(deletedType) {
            const deletedTypeId = deletedType?.typeId || this.currentType?.typeId || null;

            this.showDeleteModal = false;

            if (deletedTypeId && this.currentType?.typeId === deletedTypeId) {
                this.currentType = null;
            }

            await this.loadContentTypeOptions();
        }
    },

    template: `
        <div>
            <div class="d-flex align-items-center justify-content-between mb-3">
                <h1 class="mb-0">內科問卷管理</h1>

                <button
                    type="button"
                    class="btn btn-primary"
                    @click="openCreateModal"
                >
                    新增表單種類
                </button>
            </div>

            <ul class="nav nav-tabs">
                <li
                    class="nav-item"
                    v-for="t in contentTypes"
                    :key="t.typeId"
                >
                    <button
                        type="button"
                        class="nav-link"
                        :class="{ active: currentType && currentType.typeId === t.typeId }"
                        @click="selectType(t)"
                    >
                        {{ t.typeName }}
                    </button>
                </li>
            </ul>

            <div
                v-if="!loading && !currentType"
                class="alert alert-secondary mt-3"
            >
                目前還沒有表單種類，先新增一個。
            </div>

            <DynamicFormPage
                v-if="currentType"
                :content-type="currentType"
                @edit="openEditModal"
                @delete="openDeleteModal"
            />

            <ContentTypeCreateModal
                :show="showCreateModal"
                :department-id="departmentId"
                @close="closeCreateModal"
                @created="handleCreated"
            />

            <ContentTypeEditModal
                :show="showEditModal"
                :content-type="currentType"
                @close="closeEditModal"
                @updated="handleUpdated"
            />

            <ContentTypeDeleteModal
                :show="showDeleteModal"
                :content-type="currentType"
                @close="closeDeleteModal"
                @deleted="handleDeleted"
            />
        </div>
    `
};
