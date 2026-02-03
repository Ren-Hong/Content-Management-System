import { createRole } from '../api/roleApi.js';
import { getPermissionOptions } from '../api/permissionApi.js';
import { getScopeOptions } from '../api/scopeApi.js';

export const RoleCreateModal = {
    props: {
        show: {
            type: Boolean,
            required: true
        }
    },

    emits: ['close', 'created'],

    data() {
        return {
            modal: null,        

            permissionOptions: [],

            scopeOptions: [],
            defaultScopeCode: 'Department',
            defaultScopeId: null,

            form: {
                roleName: '',
                roleCode: '',
                permissionScopes: [] // PermissionId, ScopeId
            },

            submitting: false
        };
    },

    watch: {                        
        async show(val) {                
            if (val) {
                await Promise.all([
                    this.loadPermissionOptions(),
                    this.loadScopeOptions()
                ]);

                this.resetForm();
                this.modal.show();
            } else {
                this.modal.hide();
            }
        }
    },

    mounted() {
        this.modal = new bootstrap.Modal(this.$refs.modal);

        this.$refs.modal.addEventListener('hidden.bs.modal', () => {
            this.$emit('close');
        });
    },

    methods: {
        async loadPermissionOptions() {
            try {
                let res = await getPermissionOptions();

                if (!res.success) {
                    alert(`錯誤代碼 -> ${res.errorCode}`);
                    return;
                }

                this.permissionOptions = res.data;
            } catch (err) {
                console.error(err);
                alert('系統錯誤 -> 權限選單載入');
            }
        },

        async loadScopeOptions() {
            try {
                const res = await getScopeOptions();
                if (!res.success) {
                    alert(`錯誤代碼 -> ${res.errorCode}`);
                    return;
                }
                this.scopeOptions = res.data;

                // ✅ 用 ScopeCode 找 Department 的 GUID
                const defaultScope = this.scopeOptions.find(
                    s => s.scopeCode === this.defaultScopeCode
                );

                this.defaultScopeId = defaultScope
                    ? defaultScope.scopeId
                    : null;

            } catch (err) {
                console.error(err);
                alert('系統錯誤 -> 範圍選單載入');
            }
        },

        onPermissionSelected(event) {
            const permissionId = event.target.value;
            if (!permissionId) return;

            if (!this.defaultScopeId) {
                alert('Scope 尚未載入完成，請稍後再試');
                event.target.value = '';
                return;
            }

            const exists = this.form.permissionScopes.some(x => x.permissionId === permissionId);
            if (!exists) {
                this.form.permissionScopes.push({
                    permissionId,
                    scopeId: this.defaultScopeId
                });
            }

            event.target.value = '';
        },

        removePermission(permissionId) {
            this.form.permissionScopes = this.form.permissionScopes.filter(x => x.permissionId !== permissionId);
        },

        updateScope(permissionId, event) {  
            const scopeId = event.target.value; // GUID string
            const item = this.form.permissionScopes.find(x => x.permissionId === permissionId);
            if (item) item.scopeId = scopeId;
        },

        getPermissionName(permissionId) {
            const p = this.permissionOptions.find(r => r.permissionId === permissionId);
            return p ? p.permissionName : permissionId;
        },

        getScopeName(scopeId) { 
            const s = this.scopeOptions.find(x => x.scopeId === scopeId);
            return s ? (s.scopeName || s.scopeCode) : scopeId;
        },

        resetForm() {
            this.form = {
                roleName: '',
                roleCode: '',
                permissionScopes: []  
            };
        },

        async submit() {
            if (this.form.permissionScopes.length === 0) {
                alert('請至少選擇一個權限');
                return;
            }

            this.submitting = true;

            try {
                let res = await createRole(this.form);

                if (!res.success) {
                    alert(`錯誤代碼 -> ${res.errorCode}`);
                    return;
                }

                this.$emit('created');
            } catch (err) {
                console.error(err);
                alert('系統錯誤 -> 創建角色');
            } finally {
                this.submitting = false;
            }
        }
    },

    template: `
        <div class="modal fade" tabindex="-1" ref="modal">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content border-primary">

                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title">新增角色</h5>
                        <button type="button"
                                class="btn-close"
                                :disabled="submitting"
                                data-bs-dismiss="modal">
                        </button>
                    </div>

                    <div class="modal-body">

                        <div class="alert alert-primary mb-3">
                            <label class="form-label">角色名稱</label>

                            <input class="form-control"
                                    v-model="form.roleName">
                        </div>

                        <div class="alert alert-primary mb-3">
                            <label class="form-label">角色代碼(程式用)</label>

                            <input class="form-control"
                                    v-model="form.roleCode">
                        </div>

                        <div class="alert alert-primary mb-3">

                            <div class="mb-2">
                                <label class="form-label">權限</label>

                                <select class="form-select"
                                        @change="onPermissionSelected($event)">
                                    <option value="">請選擇權限</option>
                                    <option v-for="p in permissionOptions"
                                            :key="p.permissionId"
                                            :value="p.permissionId">
                                        {{ p.permissionName }}
                                    </option>
                                </select>
                            </div>

                            <div class="d-flex flex-column gap-2">
                                <div v-for="ps in form.permissionScopes"
                                    :key="ps.permissionId"
                                    class="d-flex align-items-center gap-2">

                                    <span class="badge bg-primary">
                                        {{ getPermissionName(ps.permissionId) }}
                                    </span>

                                    <!-- ✅ Scope 下拉 -->
                                    <select
                                        class="form-select form-select-sm w-auto"
                                        :value="ps.scopeId"
                                        @change="updateScope(ps.permissionId, $event)"
                                    >
                                        <option v-for="s in scopeOptions"
                                                :key="s.scopeId"
                                                :value="s.scopeId">
                                        {{ s.scopeName }}
                                        </option>
                                    </select>

                                    <!-- ✅ 移除 -->
                                    <button type="button"
                                            class="btn btn-danger btn-sm"
                                            :disabled="submitting"
                                            @click="removePermission(ps.permissionId)">
                                        移除
                                    </button>

                                </div>
                            </div>

                        </div>
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
