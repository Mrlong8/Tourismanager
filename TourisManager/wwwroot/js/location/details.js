(function () {
    'use strict';
    const root = document.querySelector('.ld-page');
    if (!root) return;
    const groups = [...root.querySelectorAll('.ld-menu-group')];
    const toggle = document.getElementById('menu-toggle-all');
    function syncToggle() {
        if (!toggle) return;
        const all = groups.length > 0 && groups.every(group => group.open);
        toggle.textContent = all ? 'Thu gọn tất cả' : 'Mở tất cả';
        toggle.setAttribute('aria-expanded', String(all));
    }
    groups.forEach(group => group.addEventListener('toggle', syncToggle));
    if (toggle) toggle.addEventListener('click', () => {
        const open = !groups.every(group => group.open);
        groups.forEach(group => { group.open = open; });
        syncToggle();
    });
    root.querySelectorAll('img').forEach(img => {
        img.addEventListener('error', () => {
            if (img.classList.contains('ld-food-image')) {
                if (!img.dataset.failed) { img.dataset.failed = 'true'; img.src = root.dataset.imageFallback || '/Image/menu/image-unavailable.svg'; }
            } else { img.remove(); }
        });
    });
    const form = document.getElementById('review-form');
    if (form) {
        const rating = document.getElementById('rvRating');
        const buttons = [...document.querySelectorAll('#starPick button')];
        function paint(value) { buttons.forEach(button => { button.classList.toggle('on', Number(button.dataset.v) <= value); button.setAttribute('aria-pressed', String(Number(button.dataset.v) === Number(rating.value))); }); }
        buttons.forEach(button => {
            button.addEventListener('click', () => { rating.value = button.dataset.v; paint(Number(rating.value)); });
            button.addEventListener('mouseenter', () => paint(Number(button.dataset.v)));
        });
        document.getElementById('starPick').addEventListener('mouseleave', () => paint(Number(rating.value)));
        form.addEventListener('submit', async event => {
            event.preventDefault();
            const msg = document.getElementById('rvMsg');
            const submit = document.getElementById('rvSubmit');
            msg.className = 'ld-form-msg err';
            if (!Number(rating.value) || !form.elements.Content.value.trim()) { msg.textContent = 'Vui lòng chọn số sao và nhập nội dung.'; return; }
            submit.disabled = true;
            msg.textContent = 'Đang gửi đánh giá…';
            try {
                const response = await fetch(form.action, { method: 'POST', body: new FormData(form), headers: { 'X-Requested-With': 'XMLHttpRequest' } });
                let data = {}; try { data = await response.json(); } catch (_) { /* expired token may return HTML */ }
                if (!response.ok || !data.success) throw new Error(data.message || 'Chưa lưu được đánh giá. Hãy tải lại trang và thử lại.');
                location.reload();
            } catch (error) { msg.textContent = error.message; submit.disabled = false; }
        });
    }
    const share = document.getElementById('btnShare');
    if (share) share.addEventListener('click', async () => {
        try {
            if (navigator.share) await navigator.share({ title: document.title, url: location.href });
            else if (navigator.clipboard) { await navigator.clipboard.writeText(location.href); share.textContent = 'Đã chép link'; }
        } catch (_) { /* Share cancellation needs no error message. */ }
    });
})();
