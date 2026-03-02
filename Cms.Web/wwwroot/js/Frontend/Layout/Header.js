import { logout } from '../api/accountApi.js';

export const Header = {
    methods: {
        async logout() {
            if (!confirm('確定要登出？')) return;

            await logout();
            location.href = '/login';
        }
    },
    template: `
        <div class="d-flex justify-content-between align-items-center w-100">

            <!-- 左側 -->
            <div class="d-flex align-items-center">
                <img 
                    src="/img/logo.png" 
                    alt="logo" 
                    style="height:32px; margin-right:10px;"
                />

                <div class="fw-bold">
                    亞東紀念醫院 — 表單管理系統
                </div>
            </div>

            <!-- 右側 -->
            <button
                class="btn btn-outline-danger btn-sm"
                @click="logout"
            >
                登出
            </button>

        </div>
    `
};