(function () {
    'use strict';
    const root = document.querySelector('.ld-page[data-location-id]');
    const source = document.getElementById('restaurant-menu-data');
    if (!root || !source) return;
    const money = value => new Intl.NumberFormat('vi-VN', { maximumFractionDigits: 0 }).format(value) + ' đ';
    const normalize = value => String(value).normalize('NFD').replace(/[\u0300-\u036f]/g, '').replace(/đ/g, 'd').replace(/Đ/g, 'D').toLowerCase();
    const message = document.getElementById('checkout-message');
    let menu;
    try { menu = new Map(JSON.parse(source.textContent).map(item => [item.id, item])); }
    catch (_) { message.textContent = 'Chưa đọc được thực đơn. Vui lòng tải lại trang.'; return; }
    const key = 'foodiemap.cart.v2:' + root.dataset.locationId;
    const quantities = new Map();
    const elements = {
        items: document.getElementById('cart-items'), empty: document.getElementById('cart-empty'),
        count: document.getElementById('cart-count'), total: document.getElementById('cart-total'),
        clear: document.getElementById('cart-clear'), submit: document.getElementById('checkout-submit'),
        status: document.getElementById('cart-status')
    };
    let busy = false;
    try {
        const saved = JSON.parse(sessionStorage.getItem(key));
        if (saved && saved.savedAt > Date.now() - 7200000 && Array.isArray(saved.items)) {
            saved.items.forEach(item => {
                if (menu.has(item.id) && Number.isInteger(item.quantity) && item.quantity >= 1 && item.quantity <= 99) quantities.set(item.id, item.quantity);
            });
        }
    } catch (_) { /* Cart remains usable when browser storage is unavailable. */ }

    function save() {
        try { sessionStorage.setItem(key, JSON.stringify({ savedAt: Date.now(), items: [...quantities].map(([id, quantity]) => ({ id, quantity })) })); }
        catch (_) { /* The current in-memory cart still works. */ }
    }
    function element(tag, className, text) {
        const node = document.createElement(tag);
        if (className) node.className = className;
        if (text !== undefined) node.textContent = text;
        return node;
    }
    function stepButton(item, step, quantity) {
        const button = element('button', '', step > 0 ? '+' : '−');
        button.type = 'button'; button.dataset.itemId = item.id; button.dataset.quantityStep = String(step);
        button.setAttribute('aria-label', (step > 0 ? 'Thêm ' : 'Bớt ') + item.name);
        button.disabled = busy || (step > 0 ? quantity >= 99 : quantity === 0);
        return button;
    }
    function render() {
        // Preserve keyboard focus when rebuilding cart rows after a quantity change.
        const active = document.activeElement;
        const focused = elements.items.contains(active) ? { id: active.dataset.itemId, step: active.dataset.quantityStep } : null;
        const fragment = document.createDocumentFragment(); let total = 0, count = 0;
        quantities.forEach((quantity, id) => {
            const item = menu.get(id); if (!item) return;
            total += item.price * quantity; count += quantity;
            const row = element('li', 'ld-cart-row'); row.dataset.cartItemId = id;
            const copy = element('div', 'ld-cart-line');
            copy.append(element('span', 'ld-cart-name', item.name + ' ×' + quantity), element('strong', '', money(item.price * quantity)));
            const foot = element('div', 'ld-cart-row-foot');
            foot.append(element('small', '', money(item.price) + ' / ' + item.unit));
            const controls = element('div', 'ld-quantity ld-cart-quantity'); controls.setAttribute('role', 'group'); controls.setAttribute('aria-label', 'Số lượng ' + item.name);
            controls.append(stepButton(item, -1, quantity), element('span', '', String(quantity)), stepButton(item, 1, quantity));
            foot.append(controls); row.append(copy, foot); fragment.append(row);
        });
        elements.items.replaceChildren(fragment);
        elements.empty.hidden = count > 0; elements.clear.hidden = count === 0;
        elements.count.textContent = count; elements.total.textContent = money(total);
        const mobile = document.getElementById('mobile-cart');
        if (mobile) { mobile.hidden = count === 0; document.getElementById('mobile-cart-count').textContent = count; document.getElementById('mobile-cart-total').textContent = money(total); }
        elements.submit.disabled = count === 0 || busy; elements.clear.disabled = busy;
        root.querySelectorAll('.ld-food-card[data-menu-id]').forEach(card => {
            const quantity = quantities.get(card.dataset.menuId) || 0;
            card.classList.toggle('is-selected', quantity > 0);
            const output = card.querySelector('output'); if (output) output.textContent = quantity;
            card.querySelectorAll('[data-quantity-step]').forEach(button => { button.disabled = busy || (Number(button.dataset.quantityStep) > 0 ? quantity >= 99 : quantity === 0); });
        });
        if (focused) {
            const row = [...elements.items.children].find(row => row.dataset.cartItemId === focused.id);
            const button = row && [...row.querySelectorAll('button')].find(button => button.dataset.quantityStep === focused.step && !button.disabled);
            (button || row?.querySelector('button:not(:disabled)') || elements.submit).focus({ preventScroll: true });
        }
    }
    root.addEventListener('click', event => {
        const button = event.target.closest('[data-quantity-step][data-item-id]');
        if (!button || button.disabled || busy) return;
        const id = button.dataset.itemId, item = menu.get(id); if (!item) return;
        const quantity = Math.max(0, Math.min(99, (quantities.get(id) || 0) + Number(button.dataset.quantityStep)));
        if (quantity > 0) quantities.set(id, quantity); else quantities.delete(id);
        save(); render(); message.textContent = '';
        elements.status.textContent = item.name + ': ' + quantity + ' ' + item.unit + '. Tổng tiền ' + elements.total.textContent;
    });
    elements.clear.addEventListener('click', () => {
        if (busy) return; quantities.clear(); save(); render(); message.textContent = '';
        elements.status.textContent = 'Đã xóa các món trong giỏ.';
    });
    elements.submit.addEventListener('click', async () => {
        if (busy || quantities.size === 0) return;
        busy = true; render(); message.textContent = '';
        const original = elements.submit.innerHTML; elements.submit.textContent = 'Đang chuẩn bị hóa đơn…';
        const abort = new AbortController(); const timeout = setTimeout(() => abort.abort(), 20000);
        try {
            const token = document.querySelector('#checkout-token input[name="__RequestVerificationToken"]')?.value;
            const response = await fetch(root.dataset.checkoutUrl, {
                method: 'POST', signal: abort.signal,
                headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token || '', 'X-Requested-With': 'XMLHttpRequest' },
                body: JSON.stringify({ locationId: root.dataset.locationId, items: [...quantities].map(([menuItemId, quantity]) => ({ menuItemId, quantity })) })
            });
            let data = {}; try { data = await response.json(); } catch (_) { /* A server error can return HTML. */ }
            if (!response.ok || !data.success || !data.redirectUrl) throw new Error(data.message || 'Chưa chuẩn bị được hóa đơn. Hãy tải lại trang và thử lại.');
            const destination = new URL(data.redirectUrl, window.location.origin);
            if (destination.origin !== window.location.origin) throw new Error('Đường dẫn hóa đơn không hợp lệ.');
            try { sessionStorage.setItem('foodiemap.checkout.v1', JSON.stringify(data.draft)); } catch (_) { /* Server session already holds the validated draft. */ }
            window.location.assign(destination.href);
        } catch (error) {
            message.textContent = error.name === 'AbortError' ? 'Kết nối mất nhiều thời gian. Giỏ món vẫn được giữ; bạn có thể thử lại.' : (error.message || 'Không kết nối được. Giỏ món vẫn được giữ.');
            busy = false; elements.submit.innerHTML = original; render();
        } finally { clearTimeout(timeout); }
    });
    const search = document.getElementById('menu-search');
    const clear = document.getElementById('menu-search-clear');
    const groups = [...root.querySelectorAll('.ld-menu-group')];
    let beforeSearch = null;
    function filter() {
        const query = normalize(search.value.trim()); let found = 0;
        if (query && beforeSearch === null) beforeSearch = groups.map(group => group.open);
        groups.forEach((group, index) => {
            let count = 0;
            group.querySelectorAll('.ld-food-card').forEach(card => {
                const match = !query || normalize(card.dataset.searchName).includes(query);
                card.hidden = !match; if (match) count++;
            });
            group.hidden = count === 0; found += count;
            if (query && count) group.open = true;
            if (!query && beforeSearch) group.open = beforeSearch[index];
        });
        if (!query) beforeSearch = null;
        clear.hidden = !query; document.getElementById('menu-no-results').hidden = found > 0;
    }
    if (search) { search.addEventListener('input', filter); clear.addEventListener('click', () => { search.value = ''; filter(); search.focus(); }); }
    root.querySelectorAll('[data-open-group]').forEach(link => link.addEventListener('click', () => {
        if (search?.value) { search.value = ''; filter(); }
        const group = document.getElementById(link.dataset.openGroup); if (group) group.open = true;
    }));
    render();
})();
