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
    document.getElementById('order-start').addEventListener('click', () => {
        groups.forEach(group => { group.open = true; });
        syncToggle();
    });
    root.querySelectorAll('img').forEach(img => {
        img.addEventListener('error', () => img.remove(), { once: true });
        if (img.complete && img.naturalWidth === 0) img.remove();
    });
    const message = document.getElementById('order-message');
    root.addEventListener('click', async event => {
        const button = event.target.closest('[data-food-id]');
        if (!button || button.disabled) return;
        button.disabled = true;
        message.textContent = 'Đang mở món ăn…';
        try {
            if (!window.jQuery || !window.bootstrap?.Modal) throw new Error('Thiếu thư viện đặt món. Vui lòng tải lại trang.');
            const url = new URL(root.dataset.quickOrderUrl, window.location.origin);
            url.searchParams.set('id', button.dataset.foodId);
            const response = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
            if (!response.ok) throw new Error('Chưa tải được món ăn. Vui lòng thử lại.');
            // The existing partial contains checkout functions; jQuery executes those scripts.
            window.jQuery('#quickOrderContent').html(await response.text());
            window.bootstrap.Modal.getOrCreateInstance(document.getElementById('quickOrderModal')).show();
            message.textContent = '';
        } catch (error) { message.textContent = error.message || 'Không thể mở món ăn.'; }
        finally { button.disabled = false; }
    });
    window.updateModalPrice = function () {
        const modal = document.getElementById('quickOrderModal');
        const price = Number(modal.querySelector('input[name="foodSize"]:checked')?.value) || 0;
        const quantity = Number(modal.querySelector('#inputQty')?.value) || 1;
        const format = value => new Intl.NumberFormat('vi-VN').format(value) + ' đ';
        const display = modal.querySelector('#modalDisplayPrice');
        const total = modal.querySelector('#modalTotalPrice');
        if (display) display.textContent = format(price);
        if (total) total.textContent = format(price * quantity);
    };
    window.updateQty = function (step) {
        const input = document.querySelector('#quickOrderModal #inputQty');
        if (input) input.value = Math.max(1, Math.min(99, (Number(input.value) || 1) + Number(step)));
        window.updateModalPrice();
    };
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
    share.addEventListener('click', async () => {
        try {
            if (navigator.share) await navigator.share({ title: document.title, url: location.href });
            else if (navigator.clipboard) { await navigator.clipboard.writeText(location.href); share.textContent = 'Đã chép link'; }
        } catch (_) { /* Share cancellation needs no error message. */ }
    });
})();
