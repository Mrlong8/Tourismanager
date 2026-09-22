document.addEventListener("DOMContentLoaded", function () {
    // 1. Khởi tạo bản đồ tại vị trí Hà Nội
    var map = L.map('map').setView([21.028889, 105.803177], 13);

    // 2. Định nghĩa các Layer bản đồ
    const osm = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    });

    const googleRoadmap = L.tileLayer('https://{s}.google.com/vt/lyrs=m&x={x}&y={y}&z={z}', {
        maxZoom: 20,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3'],
        attribution: 'Map data &copy; Google'
    });

    const googleSatellite = L.tileLayer('https://{s}.google.com/vt/lyrs=s&x={x}&y={y}&z={z}', {
        maxZoom: 20,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3'],
        attribution: 'Map data &copy; Google'
    });

    const googleTerrain = L.tileLayer('https://{s}.google.com/vt/lyrs=p&x={x}&y={y}&z={z}', {
        maxZoom: 20,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3'],
        attribution: 'Map data &copy; Google'
    });

    // Mặc định hiển thị Google Roadmap
    googleRoadmap.addTo(map);

    // 3. Thêm menu chuyển đổi giữa các loại bản đồ (Layer Control)
    const baseMaps = {
        "Google Roadmap": googleRoadmap,
        "Google Satellite": googleSatellite,
        "Google Terrain": googleTerrain,
        "OpenStreetMap": osm
    };
    L.control.layers(baseMaps).addTo(map);

    // 4. Custom Icon cho Marker
    var myIcon = L.icon({
        iconUrl: '/Image/Iconmap.jpg',
        iconSize: [40, 40],
        iconAnchor: [20, 40],
        popupAnchor: [0, -40]
    });

    // 5. Render danh sách địa điểm từ C# Model ra bản đồ
    if (window.locationsData && Array.isArray(window.locationsData)) {
        window.locationsData.forEach(item => {
            var lat = item.latitude || item.Latitude;
            var lng = item.longitude || item.Longitude;
            var name = item.name || item.Name || '';
            var address = item.address || item.Address || '';
            var description = item.description || item.Description || '';
            var imageUrl = item.imageUrl || item.ImageUrl || '#';

            if (lat && lng) {
                var marker = L.marker([lat, lng], { icon: myIcon });
                marker.addTo(map).bindPopup(`
                    <div class="popup-content">
                        <h3>${name}</h3>
                        <p>📍 ${address}</p>
                        <p>${description}</p>
                        <a href="${imageUrl}" class="detail-btn" target="_blank">
                            Chi tiết
                        </a>
                    </div>
                `);
            }
        });
    }
});