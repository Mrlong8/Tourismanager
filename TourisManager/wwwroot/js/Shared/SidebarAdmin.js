/**
 * ==========================================================================
 * TourisManager - Admin Sidebar JavaScript
 * ==========================================================================
 */
document.addEventListener('DOMContentLoaded', function () {
    const wrapper = document.querySelector('.admin-layout-wrapper');
    const toggleBtn = document.getElementById('btnToggleAdminSidebar');
    const clockEl = document.getElementById('adminLiveClock');

    // Tự động tạo Overlay nền tối cho Mobile Menu
    let overlay = document.querySelector('.admin-mobile-overlay');
    if (!overlay) {
        overlay = document.createElement('div');
        overlay.className = 'admin-mobile-overlay';
        document.body.appendChild(overlay);
    }

    if (wrapper && toggleBtn) {
        // KHÔI PHỤC TRẠNG THÁI DESKTOP TỪ LOCALSTORAGE (Tránh nháy hình)
        const isDesktop = window.innerWidth >= 768;
        if (isDesktop) {
            const isCollapsedSaved = localStorage.getItem('tourisAdmin_sidebarCollapsed') === 'true';
            if (isCollapsedSaved) {
                wrapper.classList.add('is-collapsed');
            }
            updateToggleIcon(isCollapsedSaved, false);
        } else {
            // Khởi tạo icon cho Mobile (Hamburger)
            updateToggleIcon(false, false);
        }

        // XỬ LÝ SỰ KIỆN CLICK NÚT TOGGLE
        toggleBtn.addEventListener('click', function (e) {
            e.preventDefault();

            // Xử lý riêng cho Mobile (< 768px)
            if (window.innerWidth < 768) {
                const isMobileOpen = wrapper.classList.toggle('mobile-open');
                if (isMobileOpen) {
                    overlay.classList.add('show');
                    updateToggleIcon(false, true); // Đổi icon thành X
                } else {
                    overlay.classList.remove('show');
                    updateToggleIcon(false, false); // Trả lại icon Hamburger
                }
            }
            // Xử lý cho Desktop (>= 768px)
            else {
                const willCollapse = !wrapper.classList.contains('is-collapsed');
                if (willCollapse) {
                    wrapper.classList.add('is-collapsed');
                } else {
                    wrapper.classList.remove('is-collapsed');
                }
                localStorage.setItem('tourisAdmin_sidebarCollapsed', willCollapse ? 'true' : 'false');
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

    // HÀM ĐỔI ICON NÚT BẤM (Cực kỳ quan trọng để xử lý Responsive)
    function updateToggleIcon(collapsed, isMobileOpen) {
        if (!toggleBtn) return;
        const icon = toggleBtn.querySelector('i');
        if (!icon) return;

        if (window.innerWidth < 768) {
            // Chế độ Mobile
            if (isMobileOpen) {
                icon.className = 'bi bi-x-lg text-primary'; // Nút X (Đóng menu)
                toggleBtn.setAttribute('title', 'Đóng thanh Menu');
            } else {
                icon.className = 'bi bi-list text-primary'; // Nút Hamburger (Mở menu)
                toggleBtn.setAttribute('title', 'Mở Menu Quản lý');
            }
        } else {
            // Chế độ Desktop
            if (collapsed) {
                icon.className = 'bi bi-layout-sidebar text-primary';
                toggleBtn.setAttribute('title', 'Mở rộng thanh Menu');
            } else {
                icon.className = 'bi bi-layout-sidebar-inset';
                toggleBtn.setAttribute('title', 'Thu gọn thanh Menu');
            }
        }
    }

    // ĐỒNG HỒ LIVE TRÊN HEADER
    if (clockEl) {
        function updateClock() {
            const now = new Date();
            const hours = String(now.getHours()).padStart(2, '0');
            const minutes = String(now.getMinutes()).padStart(2, '0');
            const seconds = String(now.getSeconds()).padStart(2, '0');
            clockEl.textContent = hours + ':' + minutes + ':' + seconds;
        }
        updateClock();
        setInterval(updateClock, 1000);
    }

    // ACTIVE MENU HIỆN TẠI (Ghim màu trạng thái) & ĐÓNG MENU MOBILE KHI CHUYỂN TRANG
    const currentPath = window.location.pathname.toLowerCase();
    const menuLinks = document.querySelectorAll('.admin-link-item');

    menuLinks.forEach(function (link) {
        const href = (link.getAttribute('href') || '').toLowerCase();
        // Kiểm tra pathname có khớp (tránh active nhầm thẻ A rỗng)
        if (href && href !== '#' && href !== '/' && currentPath.startsWith(href)) {
            // Trường hợp đặc biệt: trang chủ admin (thường là /Admin)
            if (href === '/admin' && currentPath !== '/admin' && currentPath !== '/admin/') {
                // Không làm gì, tránh /admin/user cũng active nút /admin
            } else {
                menuLinks.forEach(item => item.classList.remove('active'));
                link.classList.add('active');
            }
        }

        // Tự động đóng menu trên Mobile khi bấm vào link
        link.addEventListener('click', () => {
            if (window.innerWidth < 768 && wrapper && overlay) {
                wrapper.classList.remove('mobile-open');
                overlay.classList.remove('show');
                updateToggleIcon(false, false);
            }
        });
    });

    // RESET TRẠNG THÁI KHI XOAY/ĐỔI KÍCH THƯỚC MÀN HÌNH (Responsive Event)
    window.addEventListener('resize', () => {
        if (!wrapper) return;

        if (window.innerWidth >= 768) {
            // Trở về Desktop
            wrapper.classList.remove('mobile-open');
            if (overlay) overlay.classList.remove('show');
            const isCollapsedSaved = localStorage.getItem('tourisAdmin_sidebarCollapsed') === 'true';
            if (isCollapsedSaved) {
                wrapper.classList.add('is-collapsed');
            } else {
                wrapper.classList.remove('is-collapsed');
            }
            updateToggleIcon(isCollapsedSaved, false);
        } else {
            // Trở về Mobile
            wrapper.classList.remove('is-collapsed'); // Bỏ trạng thái collapse của Desktop
            updateToggleIcon(false, wrapper.classList.contains('mobile-open'));
        }
    });
});