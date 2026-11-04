document.addEventListener('DOMContentLoaded', function () {
    const wrapper = document.getElementById('profileLayoutWrapper') || document.querySelector('.profile-layout-wrapper');
    const toggleBtn = document.getElementById('btnToggleSidebar') || document.querySelector('.btn-toggle-sidebar');
    const clockElement = document.getElementById('liveClock');
    const STORAGE_KEY_SIDEBAR_COLLAPSED = 'tm_profile_sidebar_collapsed';

    // 1. Tự động xử lý lỗi hiển thị ảnh (Auto-fallback cho ảnh bị hỏng/trống)
    const allImages = document.querySelectorAll('img');
    allImages.forEach(img => {
        img.addEventListener('error', function () {
            // Ngăn chặn lặp vô hạn nếu ảnh fallback cũng lỗi
            this.onerror = null;
            // Gán URL ảnh mặc định (Bạn có thể đổi sang ảnh trong folder wwwroot của bạn)
            this.src = 'https://cdn-icons-png.flaticon.com/512/149/149071.png';
        });
    });

    // 2. Tự động tạo Overlay nền tối cho Mobile Menu
    let overlay = document.querySelector('.profile-mobile-overlay');
    if (!overlay) {
        overlay = document.createElement('div');
        overlay.className = 'profile-mobile-overlay';
        document.body.appendChild(overlay);
    }

    if (wrapper && toggleBtn) {
        // KHÔI PHỤC TRẠNG THÁI TỪ LOCALSTORAGE
        const isDesktop = window.innerWidth >= 768;
        if (isDesktop) {
            const isCollapsed = localStorage.getItem(STORAGE_KEY_SIDEBAR_COLLAPSED) === 'true';
            if (isCollapsed) {
                wrapper.classList.add('is-collapsed');
            }
            updateToggleIcon(isCollapsed, false);
        } else {
            updateToggleIcon(false, false);
        }

        // SỰ KIỆN CLICK NÚT TOGGLE
        toggleBtn.addEventListener('click', function (e) {
            e.preventDefault();

            // Chế độ Mobile (< 768px): Trượt từ ngoài vào
            if (window.innerWidth < 768) {
                const isMobileOpen = wrapper.classList.toggle('mobile-open');
                if (isMobileOpen) {
                    overlay.classList.add('show');
                    updateToggleIcon(false, true);
                } else {
                    overlay.classList.remove('show');
                    updateToggleIcon(false, false);
                }
            }
            // Chế độ Desktop: Thụt vào / Phóng to
            else {
                const willCollapse = !wrapper.classList.contains('is-collapsed');
                if (willCollapse) {
                    wrapper.classList.add('is-collapsed');
                } else {
                    wrapper.classList.remove('is-collapsed');
                }
                localStorage.setItem(STORAGE_KEY_SIDEBAR_COLLAPSED, willCollapse ? 'true' : 'false');
                updateToggleIcon(willCollapse, false);
            }
        });

        // Bấm ra ngoài khoảng đen sẽ đóng menu (Mobile)
        overlay.addEventListener('click', function () {
            wrapper.classList.remove('mobile-open');
            overlay.classList.remove('show');
            updateToggleIcon(false, false);
        });
    }

    // Đổi Icon nút bấm cho phù hợp ngữ cảnh
    function updateToggleIcon(collapsed, isMobileOpen) {
        if (!toggleBtn) return;
        const icon = toggleBtn.querySelector('i');
        if (!icon) return;

        if (window.innerWidth < 768) {
            if (isMobileOpen) {
                icon.className = 'bi bi-x-lg text-orange';
            } else {
                icon.className = 'bi bi-list text-orange';
            }
        } else {
            if (collapsed) {
                icon.className = 'bi bi-layout-sidebar text-orange';
            } else {
                icon.className = 'bi bi-layout-sidebar-inset text-orange';
            }
        }
    }

    // 3. Cập nhật đồng hồ thời gian thực
    if (clockElement) {
        function updateClock() {
            const now = new Date();
            const hours = String(now.getHours()).padStart(2, '0');
            const minutes = String(now.getMinutes()).padStart(2, '0');
            const seconds = String(now.getSeconds()).padStart(2, '0');
            clockElement.textContent = `${hours}:${minutes}:${seconds}`;
        }
        updateClock();
        setInterval(updateClock, 1000);
    }

    // Đóng menu tự động khi bấm vào link trên mobile
    const menuLinks = document.querySelectorAll('.sidebar-link-item');
    menuLinks.forEach(link => {
        link.addEventListener('click', () => {
            if (window.innerWidth < 768 && wrapper && overlay) {
                wrapper.classList.remove('mobile-open');
                overlay.classList.remove('show');
                updateToggleIcon(false, false);
            }
        });
    });

    // Resize lại màn hình thì reset cho chuẩn Desktop/Mobile
    window.addEventListener('resize', () => {
        if (!wrapper) return;
        if (window.innerWidth >= 768) {
            wrapper.classList.remove('mobile-open');
            if (overlay) overlay.classList.remove('show');
            const isCollapsed = localStorage.getItem(STORAGE_KEY_SIDEBAR_COLLAPSED) === 'true';
            if (isCollapsed) wrapper.classList.add('is-collapsed');
            else wrapper.classList.remove('is-collapsed');
            updateToggleIcon(isCollapsed, false);
        } else {
            wrapper.classList.remove('is-collapsed');
            updateToggleIcon(false, wrapper.classList.contains('mobile-open'));
        }
    });
});