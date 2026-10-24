// =========================================================================
// Profile Index JavaScript: Avatar Upload & Leaflet Interactive Map
// =========================================================================

let originalAvatarUrl = '';
let map, marker;
let reverseGeocodeTimeout = null;
let currentAbortController = null;
let reverseRequestId = 0;

document.addEventListener("DOMContentLoaded", function () {
    const avatarImg = document.getElementById('avatarPreview');
    if (avatarImg) originalAvatarUrl = avatarImg.src;

    initProfileMap();

    const addressInput = document.getElementById('addressInput');
    if (addressInput) {
        addressInput.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                searchAddressToLocation();
            }
        });
    }
});

// =========================================================================
// Helpers
// =========================================================================
function escapeHtml(str) {
    const div = document.createElement('div');
    div.textContent = str ?? '';
    return div.innerHTML;
}

// fetch JSON có timeout + hỗ trợ abort từ bên ngoài
async function fetchJson(url, { timeout = 6000, signal } = {}) {
    const controller = new AbortController();
    const timer = setTimeout(() => controller.abort(), timeout);
    if (signal) {
        if (signal.aborted) controller.abort();
        else signal.addEventListener('abort', () => controller.abort(), { once: true });
    }
    try {
        const res = await fetch(url, { signal: controller.signal });
        if (!res.ok) throw new Error('HTTP ' + res.status);
        return await res.json();
    } finally {
        clearTimeout(timer);
    }
}

function setStatus(text) {
    const statusMsg = document.getElementById('addressStatusMsg');
    const statusText = document.getElementById('addressStatusText');
    if (!statusMsg) return;
    if (text) {
        if (statusText) statusText.innerText = text;
        statusMsg.classList.remove('d-none');
    } else {
        statusMsg.classList.add('d-none');
    }
}
// =========================================================================
// 1. BẢN ĐỒ LEAFLET
// =========================================================================
function initProfileMap() {
    const mapElement = document.getElementById('map');
    if (!mapElement || typeof L === 'undefined') return;

    if (L.Icon && L.Icon.Default) {
        L.Icon.Default.imagePath = '/lib/leaflet/images/';
    }

    const latData = mapElement.getAttribute('data-lat');
    const lngData = mapElement.getAttribute('data-lng');
    const hasExistingCoords = mapElement.getAttribute('data-has-coords') === 'true';

    const defaultLat = hasExistingCoords && latData ? parseFloat(latData) : 21.028511;
    const defaultLng = hasExistingCoords && lngData ? parseFloat(lngData) : 105.804817;

    map = L.map('map', { zoomControl: true, scrollWheelZoom: true })
        .setView([defaultLat, defaultLng], hasExistingCoords ? 16 : 13);

    // Tile OpenStreetMap (không cần API key), tự chuyển sang Esri nếu OSM lỗi
    const osmLayer = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; OpenStreetMap contributors'
    });
    const esriLayer = L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}', {
        maxZoom: 19,
        attribution: 'Tiles &copy; Esri'
    });

    let tileFallbackUsed = false;
    osmLayer.on('tileerror', function () {
        if (tileFallbackUsed) return;
        tileFallbackUsed = true;
        map.removeLayer(osmLayer);
        esriLayer.addTo(map);
    });
    osmLayer.addTo(map);

    marker = L.marker([defaultLat, defaultLng], { draggable: true }).addTo(map);

    if (hasExistingCoords) {
        marker.bindPopup('<b>Vị trí đã lưu</b>').openPopup();
    }

    marker.on('dragend', function () {
        const p = marker.getLatLng();
        updateCoordinates(p.lat, p.lng, true);
    });

    map.on('click', function (e) {
        marker.setLatLng(e.latlng);
        updateCoordinates(e.latlng.lat, e.latlng.lng, true);
    });

    // Tự cập nhật kích thước khi khung bản đồ đổi (sidebar, resize, layout load xong)
    if (typeof ResizeObserver !== 'undefined') {
        new ResizeObserver(function () {
            if (map) map.invalidateSize();
        }).observe(mapElement);
    }
    setTimeout(() => map && map.invalidateSize(), 150);
    window.addEventListener('load', () => map && map.invalidateSize());
}
// =========================================================================
// 2. ĐỊNH VỊ GPS
// =========================================================================
function getCurrentLocation() {
    if (!navigator.geolocation) {
        alert('Trình duyệt của bạn không hỗ trợ định vị GPS.');
        return;
    }

    const btn = document.getElementById('btnGetLocation');
    const btnText = document.getElementById('btnGetLocationText');
    const btnIcon = document.getElementById('btnGetLocationIcon');

    if (btn) btn.disabled = true;
    if (btnText) btnText.innerText = 'Đang định vị...';
    if (btnIcon) btnIcon.className = 'spinner-border spinner-border-sm me-1';

    navigator.geolocation.getCurrentPosition(
        function (position) {
            const lat = position.coords.latitude;
            const lng = position.coords.longitude;

            if (map && marker) {
                map.setView([lat, lng], 16);
                marker.setLatLng([lat, lng]);
                marker.bindPopup('<b>Vị trí hiện tại của bạn</b>').openPopup();
            }
            updateCoordinates(lat, lng, true);
            resetLocationButton();
        },
        function (error) {
            let msg = 'Không thể lấy vị trí: ';
            switch (error.code) {
                case error.PERMISSION_DENIED: msg += 'Bạn đã từ chối quyền truy cập vị trí trên trình duyệt.'; break;
                case error.POSITION_UNAVAILABLE: msg += 'Thông tin vị trí không khả dụng.'; break;
                case error.TIMEOUT: msg += 'Yêu cầu định vị đã hết thời gian chờ.'; break;
                default: msg += 'Lỗi không xác định.'; break;
            }
            alert(msg);
            resetLocationButton();
        },
        { enableHighAccuracy: true, timeout: 8000, maximumAge: 30000 }
    );
}

function resetLocationButton() {
    const btn = document.getElementById('btnGetLocation');
    const btnText = document.getElementById('btnGetLocationText');
    const btnIcon = document.getElementById('btnGetLocationIcon');

    if (btn) btn.disabled = false;
    if (btnText) btnText.innerText = 'Định vị tôi';
    if (btnIcon) btnIcon.className = 'bi bi-geo-alt-fill';
}

// =========================================================================
// 3. CẬP NHẬT TỌA ĐỘ & REVERSE GEOCODING
// =========================================================================
function updateCoordinates(lat, lng, fetchAddress = true) {
    const latInput = document.getElementById('latInput');
    const lngInput = document.getElementById('lngInput');
    const coordText = document.getElementById('coordText');

    if (latInput) latInput.value = lat;
    if (lngInput) lngInput.value = lng;
    if (coordText) coordText.innerText = `Tọa độ: ${lat.toFixed(6)}, ${lng.toFixed(6)}`;

    if (fetchAddress) {
        clearTimeout(reverseGeocodeTimeout);
        reverseGeocodeTimeout = setTimeout(() => reverseGeocode(lat, lng), 300);
    }
}

// Ghép địa chỉ từ kết quả Photon
function formatPhotonAddress(p) {
    const street = [p.housenumber, p.street].filter(Boolean).join(' ');
    const parts = [p.name && p.name !== p.street ? p.name : null, street,
                   p.district || p.locality, p.city || p.county, p.state, p.country]
        .filter((v, i, arr) => v && arr.indexOf(v) === i);
    return parts.join(', ');
}

async function reverseGeocode(lat, lng) {
    const addressInput = document.getElementById('addressInput');
    if (!addressInput) return;

    if (currentAbortController) currentAbortController.abort();
    currentAbortController = new AbortController();
    const signal = currentAbortController.signal;
    const myId = ++reverseRequestId;
    const isStale = () => myId !== reverseRequestId;

    setStatus('Đang tự động xác định tên địa chỉ...');

    let address = null;

    // Nguồn 1: Nominatim (chi tiết nhất) - timeout dài hơn, thử tối đa 2 lần
    for (let attempt = 0; attempt < 2 && !address; attempt++) {
        try {
            const data = await fetchJson(
                `https://nominatim.openstreetmap.org/reverse?format=jsonv2&zoom=18&accept-language=vi&lat=${lat}&lon=${lng}`,
                { timeout: 10000, signal }
            );
            if (data && data.display_name) address = data.display_name;
        } catch (e) {
            if (isStale()) return;
            console.warn('Nominatim reverse lỗi (lần ' + (attempt + 1) + '):', e);
        }
    }

    // Nguồn 2: Photon (có số nhà, tên đường)
    if (!address && !isStale()) {
        try {
            const data = await fetchJson(
                `https://photon.komoot.io/reverse?lat=${lat}&lon=${lng}`,
                { timeout: 8000, signal }
            );
            const f = data && data.features && data.features[0];
            if (f && f.properties) address = formatPhotonAddress(f.properties);
        } catch (e) {
            if (isStale()) return;
            console.warn('Photon reverse lỗi:', e);
        }
    }

    // Nguồn 3: BigDataCloud (chỉ tới cấp phường/xã)
    if (!address && !isStale()) {
        try {
            const d = await fetchJson(
                `https://api.bigdatacloud.net/data/reverse-geocode-client?latitude=${lat}&longitude=${lng}&localityLanguage=vi`,
                { timeout: 6000, signal }
            );
            const parts = [d.locality, d.city, d.principalSubdivision, d.countryName]
                .filter((v, i, arr) => v && arr.indexOf(v) === i);
            if (parts.length) address = parts.join(', ');
        } catch (e) {
            if (isStale()) return;
            console.warn('BigDataCloud lỗi:', e);
        }
    }

    if (isStale()) return;

    if (address) addressInput.value = address;
    setStatus(null);
}
// =========================================================================
// 4. TÌM KIẾM ĐỊA CHỈ -> NHẢY BẢN ĐỒ VÀ GHIM
// =========================================================================
// Tạo các phiên bản rút gọn dần của địa chỉ (bỏ bớt phần đầu, mã bưu chính, "Việt Nam")
function buildQueryCandidates(raw) {
    const parts = raw.split(',').map(s => s.trim()).filter(Boolean)
        .filter(p => !/^\d{4,6}$/.test(p) && !/^vi[eệ]t\s?nam$/i.test(p));

    const candidates = [raw];
    for (let i = 1; i < parts.length - 1 && candidates.length < 4; i++) {
        candidates.push(parts.slice(i).join(', '));
    }
    return [...new Set(candidates)];
}

async function geocodeNominatim(q) {
    const data = await fetchJson(
        `https://nominatim.openstreetmap.org/search?format=json&accept-language=vi&countrycodes=vn&limit=1&q=${encodeURIComponent(q)}`,
        { timeout: 6000 }
    );
    if (data && data.length) {
        return { lat: parseFloat(data[0].lat), lng: parseFloat(data[0].lon), name: data[0].display_name };
    }
    return null;
}

async function geocodePhoton(q) {
    // lat/lon để ưu tiên kết quả gần Hà Nội
    const data = await fetchJson(
        `https://photon.komoot.io/api/?limit=1&lat=21.0285&lon=105.8048&q=${encodeURIComponent(q)}`,
        { timeout: 6000 }
    );
    const f = data && data.features && data.features[0];
    if (f && f.geometry && f.geometry.coordinates) {
        const p = f.properties || {};
        const name = [p.name, p.street, p.district, p.city, p.state, p.country]
            .filter(Boolean).join(', ');
        return { lat: f.geometry.coordinates[1], lng: f.geometry.coordinates[0], name };
    }
    return null;
}

async function searchAddressToLocation() {
    const addressInput = document.getElementById('addressInput');
    if (!addressInput || !addressInput.value.trim()) {
        alert('Vui lòng nhập địa chỉ cần tìm kiếm!');
        return;
    }

    const raw = addressInput.value.trim();
    const btn = document.getElementById('btnSearchAddress');
    if (btn) btn.disabled = true;
    setStatus('Đang tìm kiếm vị trí trên bản đồ...');

    let result = null;
    try {
        const candidates = buildQueryCandidates(raw);

        for (const q of candidates) {
            try { result = await geocodeNominatim(q); } catch (e) { console.warn('Nominatim lỗi:', e); }
            if (result) break;
        }
        if (!result) {
            for (const q of candidates) {
                try { result = await geocodePhoton(q); } catch (e) { console.warn('Photon lỗi:', e); }
                if (result) break;
            }
        }
    } finally {
        setStatus(null);
        if (btn) btn.disabled = false;
    }

    if (!result) {
        alert('Không tìm thấy vị trí khớp với địa chỉ vừa nhập. Vui lòng thử từ khóa ngắn gọn hơn (vd: "Phố Huế, Hà Nội") hoặc ghim trực tiếp trên bản đồ.');
        return;
    }

    if (map && marker) {
        map.setView([result.lat, result.lng], 16);
        marker.setLatLng([result.lat, result.lng]);
        marker.bindPopup(`<b>${escapeHtml(result.name)}</b>`).openPopup();
    }
    updateCoordinates(result.lat, result.lng, false); // không ghi đè chuỗi người dùng vừa gõ
}

// =========================================================================
// 5. UPLOAD AVATAR AJAX
// =========================================================================
function previewAvatar(input) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById('avatarPreview').src = e.target.result;
            document.getElementById('defaultAvatarActions').classList.add('d-none');
            const uploadActions = document.getElementById('uploadAvatarActions');
            uploadActions.classList.remove('d-none');
            uploadActions.classList.add('d-flex');
            document.getElementById('uploadStatus').innerHTML = '';
        };
        reader.readAsDataURL(input.files[0]);
    }
}

function cancelAvatarUpload() {
    document.getElementById('avatarPreview').src = originalAvatarUrl;
    document.getElementById('avatarFile').value = '';
    const uploadActions = document.getElementById('uploadAvatarActions');
    uploadActions.classList.remove('d-flex');
    uploadActions.classList.add('d-none');
    document.getElementById('defaultAvatarActions').classList.remove('d-none');
}

function submitAvatarAjax() {
    const input = document.getElementById('avatarFile');
    if (!input.files || !input.files[0]) return;

    const statusDiv = document.getElementById('uploadStatus');
    statusDiv.innerHTML = '<span class="text-info spinner-border spinner-border-sm me-1"></span><span class="text-info">Đang tải ảnh...</span>';

    const formData = new FormData();
    formData.append("avatarFile", input.files[0]);

    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenInput ? tokenInput.value : '';

    fetch('/profile/upload-avatar', {
        method: 'POST',
        headers: { 'RequestVerificationToken': token },
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            originalAvatarUrl = data.avatarUrl;
            cancelAvatarUpload();
            document.getElementById('avatarPreview').src = originalAvatarUrl;
            statusDiv.innerHTML = '<span class="text-success fw-medium"><i class="bi bi-check-circle-fill me-1"></i>Đổi Avatar thành công!</span>';
            setTimeout(() => { statusDiv.innerHTML = ''; }, 3000);
        } else {
            statusDiv.textContent = data.message || 'Tải ảnh thất bại!';
            statusDiv.className = 'small mt-1 text-danger fw-medium';
        }
    })
    .catch(error => {
        console.error('Lỗi Upload:', error);
        statusDiv.innerHTML = '<span class="text-danger fw-medium">Lỗi kết nối máy chủ!</span>';
    });
}