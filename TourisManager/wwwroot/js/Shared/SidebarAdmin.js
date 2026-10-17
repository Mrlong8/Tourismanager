/**
 * ==========================================================================
 * TourisManager - Admin Sidebar JavaScript
 * Đường dẫn: ~/js/Admin/SidebarAdmin.js (hoặc wwwroot/js/Admin/SidebarAdmin.js)
 * 
 * Tính năng chính:
 * 1. Nút bấm thu gọn / thụt vào (Collapse 1/5 xuống 70px) và mở rộng
 * 2. Lưu trạng thái thu gọn vào localStorage (không bị mất khi tải lại trang)
 * 3. Đồng hồ kỹ thuật số trực tiếp (Live Admin Clock)
 * 4. Hỗ trợ cuộn độc lập cho Sidebar và Content Viewport
 * 5. Khởi tạo mượt mà không xung đột thư viện bên ngoài
 * ==========================================================================
 */
 
document.addEventListener('DOMContentLoaded', function () {
    // 1. LẤY CÁC PHẦN TỬ GIAO DIỆN
    const wrapper = document.querySelector('.admin-layout-wrapper');
    const toggleBtn = document.getElementById('btnToggleAdminSidebar');
    const clockEl = document.getElementById('adminLiveClock');
 
    // 2. KHÔI PHỤC TRẠNG THÁI THU GỌN TỪ LOCALSTORAGE
    if (wrapper && toggleBtn) {
        const isCollapsedSaved = localStorage.getItem('tourisAdmin_sidebarCollapsed') === 'true';
        if (isCollapsedSaved) {
            wrapper.classList.add('is-collapsed');
            updateToggleIcon(true);
        }
 
        // 3. SỰ KIỆN NÚT BẤM THU GỌN / MỞ RỘNG (COLLAPSE / EXPAND)
        toggleBtn.addEventListener('click', function (e) {
            e.preventDefault();
            const willCollapse = !wrapper.classList.contains('is-collapsed');
            
            if (willCollapse) {
                wrapper.classList.add('is-collapsed');
            } else {
                wrapper.classList.remove('is-collapsed');
            }
            
            localStorage.setItem('tourisAdmin_sidebarCollapsed', willCollapse ? 'true' : 'false');
            updateToggleIcon(willCollapse);
        });
    }
 
    function updateToggleIcon(collapsed) {
        if (!toggleBtn) return;
        const icon = toggleBtn.querySelector('i');
        if (icon) {
            if (collapsed) {
                icon.className = 'bi bi-layout-sidebar text-primary';
                toggleBtn.setAttribute('title', 'Mở rộng thanh Menu (1/5)');
            } else {
                icon.className = 'bi bi-layout-sidebar-inset';
                toggleBtn.setAttribute('title', 'Thu gọn thanh Menu');
            }
        }
    }
 
    // 4. ĐỒNG HỒ THỜI GIAN THỰC (LIVE DIGITAL CLOCK)
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
 
    // 5. TỰ ĐỘNG ĐÁNH DẤU MENU ACTIVE DỰA VÀO ĐƯỜNG DẪN HIỆN TẠI
    const currentPath = window.location.pathname.toLowerCase();
    const menuLinks = document.querySelectorAll('.admin-link-item');
 
    menuLinks.forEach(function (link) {
        const href = (link.getAttribute('href') || '').toLowerCase();
        if (href && href !== '#' && currentPath.includes(href)) {
            // Xóa active ở các link khác
            menuLinks.forEach(function (item) { item.classList.remove('active'); });
            link.classList.add('active');
        }
    });
});
 