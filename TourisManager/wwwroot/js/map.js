document.addEventListener("DOMContentLoaded", function () {
    // Tạo bản đồ
    var map = L.map('map').setView([21.028889, 105.803177], 13);

    // các Layer bản đồ
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

    // Layer Control
    const baseMaps = {
        "Google Roadmap": googleRoadmap,
        "Google Satellite": googleSatellite,
        "Google Terrain": googleTerrain,
        "OpenStreetMap": osm
    };
    L.control.layers(baseMaps).addTo(map);

    // Icon & Marker
    var myIcon = L.icon({
        iconUrl: '/Image/Iconmap.jpg', // Lưu tệp my-icon.png ở thư mục wwwroot
        iconSize: [40, 40],
        iconAnchor: [20, 20],
        popupAnchor: [0, -20]
    });


    fetch('/api/Destination')
        .then(response => {
            if (!response.ok) {
                throw new Error('Lỗi kết nối API');
            }
            return response.json();
        })
        .then(data => {
            data.forEach(item => {

              
                // Xử lý đọc thuộc tính viết hoa hoặc viết thường
                var lat = item.latitude ;
                var lng = item.longitude ;
                var name = item.name;
                var address = item.address ;
                var description = item.description;
                var imageUrl = item.imageUrl || '#';

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
        })
        .catch(error => {
            console.error('Lỗi tải dữ liệu bản đồ:', error);
        });

    // var singleMarker = L.marker([21.028889, 105.803177], { icon: myIcon });
    // singleMarker.addTo(map)
    //     .bindPopup(`
    //         <div class="popup-content">
    //             <h3>Đại học Giao thông Vận tải</h3>
    //             <p>📍 3 Cầu Giấy, Hà Nội</p>
    //             <p>Trường đại học đào tạo về giao thông vận tải.</p>
    //             <a href="https:www.utc.edu.vn/" class="detail-btn">
    //                 Chi tiết
    //             </a>
    //         </div>
    //     `);
});