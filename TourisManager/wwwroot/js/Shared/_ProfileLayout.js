
 
document.addEventListener('DOMContentLoaded', function () {
    const layoutWrapper = document.getElementById('profileLayoutWrapper') || document.querySelector('.profile-layout-wrapper');
    const toggleBtn = document.getElementById('btnToggleSidebar') || document.querySelector('.btn-toggle-sidebar');
    const clockElement = document.getElementById('liveClock');
 
    const STORAGE_KEY_SIDEBAR_COLLAPSED = 'tm_profile_sidebar_collapsed';
 
    // 1. Khôi phục trạng thái thụt vào đã lưu trước đó trong LocalStorage
    if (layoutWrapper) {
        try {
            const isCollapsed = localStorage.getItem(STORAGE_KEY_SIDEBAR_COLLAPSED) === 'true';
            if (isCollapsed) {
                layoutWrapper.classList.add('is-collapsed');
            }
        } catch (e) {
            console.warn('LocalStorage không khả dụng:', e);
        }
    }
 
    // 2. Sự kiện khi click nút bấm để thụt vào (hamburger)
    if (toggleBtn && layoutWrapper) {
        toggleBtn.addEventListener('click', function () {
            // Đảo ngược trạng thái class 'is-collapsed'
            layoutWrapper.classList.toggle('is-collapsed');
            const collapsedNow = layoutWrapper.classList.contains('is-collapsed');
            
            // Cập nhật tooltip hoặc icon nếu cần
            toggleBtn.setAttribute('aria-expanded', (!collapsedNow).toString());
 
            // Lưu trạng thái vào localStorage
            try {
                localStorage.setItem(STORAGE_KEY_SIDEBAR_COLLAPSED, collapsedNow ? 'true' : 'false');
            } catch (e) {
                console.warn(e);
            }
        });
    }
 
    // 3. Cập nhật đồng hồ thời gian thực (ví dụ: 13:00:20)
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
});
 