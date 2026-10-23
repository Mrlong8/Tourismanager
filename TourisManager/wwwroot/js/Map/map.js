(function () {
    "use strict";

    function initMapPage() {
        const root = document.getElementById("touris-map");
        if (!root) return;

        document.body.classList.add("tm-map-active");

        const byId = id => document.getElementById(id);
        const list = byId("map-list");
        const search = byId("map-search");
        const areaSelect = byId("map-area");
        const sidebar = byId("map-sidebar");
        const openList = byId("map-open-list");
        const message = byId("map-message");
        const reducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
        const collator = new Intl.Collator("vi", { sensitivity: "base", numeric: true });
        const numberFormat = new Intl.NumberFormat("vi-VN");
        const appBase = new URL(root.dataset.appBase || "/", window.location.origin);
        const detailsBase = (root.dataset.detailsUrl || new URL("Location/Details", appBase).pathname).replace(/\/$/, "");
        const markers = new Map();
        const cards = new Map();
        const state = { type: "", area: "", keyword: "", selectedId: null, sidebarOpen: true };
        const placeSvg = '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M20 10c0 6-8 11-8 11S4 16 4 10a8 8 0 1 1 16 0Z"/><circle cx="12" cy="10" r="2.5"/></svg>';
        const foodSvg = '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M5 3v6c0 3 6 3 6 0V3M8 3v18M19 3c-4 3-4 9 0 9V3Zm0 9v9"/></svg>';
        const searchSvg = '<svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="10.5" cy="10.5" r="6.5"/><path d="m16 16 4.5 4.5"/></svg>';
        let map = null;
        let markerLayer = null;
        let filtered = [];
        let debounceTimer;
        let messageTimer;
        let userMarker;
        let accuracyCircle;
        let rows = [];

        function normalize(value) {
            return String(value == null ? "" : value).normalize("NFD")
                .replace(/[\u0300-\u036f]/g, "").replace(/đ/g, "d").replace(/Đ/g, "D")
                .toLowerCase().replace(/\s+/g, " ").trim();
        }

        function showMessage(text, persistent) {
            window.clearTimeout(messageTimer);
            message.textContent = text;
            message.hidden = false;
            if (!persistent) messageTimer = window.setTimeout(() => { message.hidden = true; }, 5000);
        }

        function coordinate(value, limit) {
            if (value === null || value === undefined || String(value).trim() === "") return null;
            const parsed = Number(value);
            return Number.isFinite(parsed) && Math.abs(parsed) <= limit ? parsed : null;
        }

        function isRestaurant(row, categories) {
            const restaurantCategory = categories.some(category => {
                const name = normalize(category);
                return ["am thuc", "an uong", "food", "food and drink", "food & drink", "cafe", "ca phe"].includes(name)
                    || /(^|[\s/&-])(nha hang|quan an|quan nhau|restaurant|restaurants|eatery|quan ca phe)([\s/&-]|$)/.test(name);
            });
            if (restaurantCategory) return true;
            const placeCategory = categories.some(category => /(^|\s)(dia diem|du lich|tham quan|di tich|bao tang|pho di bo)(\s|$)/.test(normalize(category)));
            return !placeCategory && row.hasMenu === true;
        }

        try {
            const data = JSON.parse(byId("map-locations-data").textContent || "[]");
            if (!Array.isArray(data)) throw new Error("Invalid locations data");
            rows = data.map((row, index) => {
                const categoryNames = Array.isArray(row.categoryNames) ? row.categoryNames.filter(Boolean) : [];
                const latitude = coordinate(row.latitude, 90);
                const longitude = coordinate(row.longitude, 180);
                const addressParts = String(row.address || "").split(",").map(part => part.trim()).filter(Boolean);
                const areaName = String(row.areaName || (addressParts.length > 1 ? addressParts[addressParts.length - 1] : ""));
                const item = {
                    id: String(row.locationId == null ? index : row.locationId),
                    name: String(row.name || "Chưa có tên"),
                    address: String(row.address || "Chưa cập nhật địa chỉ"),
                    imageUrl: String(row.imageUrl || ""),
                    areaName,
                    areaKey: row.areaId ? "id:" + row.areaId : areaName ? "name:" + normalize(areaName) : "",
                    type: isRestaurant(row, categoryNames) ? "restaurant" : "place",
                    latitude,
                    longitude,
                    hasCoordinates: latitude !== null && longitude !== null
                };
                item.searchText = normalize([item.name, item.address, item.areaName, ...categoryNames].join(" "));
                return item;
            }).sort((a, b) => collator.compare(a.name, b.name));
        } catch (error) {
            showMessage("Không đọc được dữ liệu địa điểm. Bạn thử tải lại trang nhé.", true);
        }

        const locationsById = new Map(rows.map(row => [row.id, row]));
        const knownAreas = new Map();
        rows.forEach(row => {
            if (row.areaKey.startsWith("id:") && row.areaName) knownAreas.set(normalize(row.areaName), row.areaKey);
        });
        rows.forEach(row => {
            if (row.areaKey.startsWith("name:") && knownAreas.has(normalize(row.areaName))) row.areaKey = knownAreas.get(normalize(row.areaName));
        });
        const areas = new Map();
        rows.forEach(row => { if (row.areaKey && row.areaName) areas.set(row.areaKey, row.areaName); });
        [...areas.entries()].sort((a, b) => collator.compare(a[1], b[1])).forEach(([key, name]) => {
            areaSelect.add(new Option(name, key));
        });

        function makeElement(tag, className, text) {
            const element = document.createElement(tag);
            if (className) element.className = className;
            if (text !== undefined) element.textContent = text;
            return element;
        }

        function makePhoto(row, isPopup) {
            const photo = makeElement("div", "tm-photo" + (row.type === "restaurant" ? " is-restaurant" : "") + (isPopup ? " tm-popup-photo" : ""));
            photo.innerHTML = row.type === "restaurant" ? foodSvg : placeSvg;
            if (!row.imageUrl.trim()) return photo;
            try {
                const path = row.imageUrl.trim().replace(/^~\//, appBase.pathname);
                const url = new URL(path, appBase);
                if (!["http:", "https:"].includes(url.protocol)) return photo;
                const img = document.createElement("img");
                img.alt = "";
                img.loading = isPopup ? "eager" : "lazy";
                img.decoding = "async";
                img.referrerPolicy = "no-referrer";
                img.addEventListener("error", () => img.remove(), { once: true });
                img.src = url.href;
                photo.append(img);
            } catch (error) {
                return photo;
            }
            return photo;
        }

        function typeLabel(row) {
            return row.type === "restaurant" ? "Nhà hàng" : "Địa điểm";
        }

        function makeBadge(row) {
            return makeElement("span", "tm-place-type" + (row.type === "restaurant" ? " is-restaurant" : ""), typeLabel(row));
        }

        function detailsUrl(row) {
            return detailsBase + "/" + encodeURIComponent(row.id);
        }

        function makeCard(row) {
            const card = makeElement("article", "tm-place-card");
            card.dataset.locationId = row.id;
            const main = makeElement("button", "tm-place-main");
            main.type = "button";
            main.dataset.locationId = row.id;
            main.setAttribute("aria-label", "Xem " + row.name + " trên bản đồ");
            const copy = makeElement("div", "tm-place-copy");
            const address = makeElement("p", "tm-place-address", row.address);
            address.title = row.address;
            copy.append(makeBadge(row), makeElement("h2", "tm-place-name", row.name), address);
            if (!row.hasCoordinates) copy.append(makeElement("span", "tm-coordinate-note", "Chưa có tọa độ"));
            main.append(makePhoto(row, false), copy);
            const footer = makeElement("div", "tm-place-footer");
            const viewButton = makeElement("button", "tm-place-action", "Trên bản đồ");
            viewButton.type = "button";
            viewButton.dataset.locationId = row.id;
            viewButton.disabled = !row.hasCoordinates;
            const detailLink = makeElement("a", "tm-place-action is-secondary", "Chi tiết ↗");
            detailLink.href = detailsUrl(row);
            detailLink.setAttribute("aria-label", "Chi tiết " + row.name);
            footer.append(viewButton, detailLink);
            card.append(main, footer);
            cards.set(row.id, card);
            return card;
        }

        function makePopup(row) {
            const popup = makeElement("div", "tm-popup");
            const body = makeElement("div", "tm-popup-body");
            const actions = makeElement("div", "tm-popup-actions");
            const details = makeElement("a", "tm-place-action", "Xem chi tiết ↗");
            details.href = detailsUrl(row);
            const reviews = makeElement("a", "tm-place-action is-secondary", "Đánh giá");
            reviews.href = detailsUrl(row) + "#comments";
            actions.append(details, reviews);
            body.append(makeBadge(row), makeElement("h2", "tm-popup-title", row.name), makeElement("p", "tm-popup-address", row.address), actions);
            popup.append(makePhoto(row, true), body);
            return popup;
        }

        function setSelected(id, scrollList) {
            state.selectedId = id;
            cards.forEach((card, key) => {
                const active = key === id;
                card.classList.toggle("is-selected", active);
                const button = card.querySelector(".tm-place-main");
                if (active) button.setAttribute("aria-current", "true");
                else button.removeAttribute("aria-current");
            });
            markers.forEach((marker, key) => {
                const icon = marker.getElement();
                if (icon) icon.classList.toggle("is-selected", key === id);
                marker.setZIndexOffset(key === id ? 1000 : 0);
            });
            const card = cards.get(id);
            if (scrollList && card) {
                const top = card.offsetTop;
                if (top < list.scrollTop || top + card.offsetHeight > list.scrollTop + list.clientHeight) {
                    list.scrollTo({ top: Math.max(0, top - 8), behavior: reducedMotion ? "auto" : "smooth" });
                }
            }
        }

        function mapPadding() {
            const mobile = window.matchMedia("(max-width: 767px)").matches;
            return {
                paddingTopLeft: [state.sidebarOpen && !mobile ? sidebar.offsetWidth + 42 : 24, mobile ? 165 : 125],
                paddingBottomRight: [66, state.sidebarOpen && mobile ? sidebar.offsetHeight + 43 : 45]
            };
        }

        function fitResults() {
            if (!map) return;
            const coordinates = filtered.filter(row => row.hasCoordinates).map(row => [row.latitude, row.longitude]);
            if (!coordinates.length) {
                showMessage(filtered.length ? "Các kết quả này chưa có tọa độ để hiển thị trên bản đồ." : "Chưa có kết quả để hiển thị trên bản đồ.");
                return;
            }
            map.fitBounds(L.latLngBounds(coordinates), { ...mapPadding(), maxZoom: 15, animate: !reducedMotion });
        }

        function setSidebar(open) {
            state.sidebarOpen = open;
            sidebar.hidden = !open;
            openList.hidden = open;
            openList.setAttribute("aria-expanded", String(open));
            root.classList.toggle("is-sidebar-hidden", !open);
        }

        function selectLocation(id) {
            const row = locationsById.get(id);
            if (!row) return;
            setSelected(id, true);
            if (!row.hasCoordinates) {
                showMessage("Địa điểm này chưa được cập nhật tọa độ. Bạn vẫn có thể mở trang chi tiết.");
                return;
            }
            if (!map) {
                showMessage("Chưa tải được bản đồ. Bạn thử tải lại trang nhé.");
                return;
            }
            if (window.matchMedia("(max-width: 767px)").matches) setSidebar(false);
            const marker = markers.get(id);
            const popup = marker.getPopup();
            const padding = mapPadding();
            popup.options.autoPanPaddingTopLeft = L.point(padding.paddingTopLeft);
            popup.options.autoPanPaddingBottomRight = L.point(padding.paddingBottomRight);
            map.setView([row.latitude, row.longitude], Math.max(map.getZoom(), 15), { animate: false });
            const open = () => { if (state.selectedId === id) marker.openPopup(); };
            if (typeof markerLayer.zoomToShowLayer === "function") markerLayer.zoomToShowLayer(marker, open);
            else open();
        }

        function makeEmpty() {
            const empty = makeElement("div", "tm-empty");
            empty.innerHTML = searchSvg;
            empty.append(makeElement("h2", "", rows.length ? "Không tìm thấy kết quả" : "Chưa có dữ liệu"));
            empty.append(makeElement("p", "", rows.length ? "Thử tên, địa chỉ khác hoặc bỏ bớt bộ lọc để tìm được nhiều nơi hơn." : "Địa điểm và nhà hàng sẽ xuất hiện khi được thêm vào hệ thống."));
            if (rows.length) {
                const reset = makeElement("button", "tm-place-action", "Xóa bộ lọc");
                reset.type = "button";
                reset.dataset.resetFilters = "true";
                empty.append(reset);
            }
            return empty;
        }

        function renderResults(shouldFit) {
            const tokens = normalize(state.keyword).split(" ").filter(Boolean);
            const scope = rows.filter(row => (!state.area || row.areaKey === state.area) && tokens.every(token => row.searchText.includes(token)));
            filtered = scope.filter(row => !state.type || row.type === state.type);
            const placeCount = scope.filter(row => row.type === "place").length;
            byId("map-place-count").textContent = numberFormat.format(placeCount);
            byId("map-restaurant-count").textContent = numberFormat.format(scope.length - placeCount);
            byId("map-list-badge").textContent = numberFormat.format(filtered.length);
            byId("map-list-title").textContent = state.type === "restaurant" ? "Nhà hàng" : state.type === "place" ? "Địa điểm" : "Địa điểm & nhà hàng";
            const count = byId("map-result-count");
            count.replaceChildren(makeElement("strong", "", numberFormat.format(filtered.length)), document.createTextNode(" kết quả"));
            byId("map-reset").hidden = !state.type && !state.area && !state.keyword.trim();
            byId("map-clear-search").hidden = !search.value;
            root.querySelectorAll("[data-map-type]").forEach(button => {
                button.setAttribute("aria-pressed", String(button.dataset.mapType === state.type));
            });
            if (map) map.closePopup();
            setSelected(null, false);
            cards.clear();
            const fragment = document.createDocumentFragment();
            filtered.forEach(row => fragment.append(makeCard(row)));
            if (!filtered.length) fragment.append(makeEmpty());
            list.replaceChildren(fragment);
            list.scrollTop = 0;
            if (markerLayer) {
                markerLayer.clearLayers();
                const visibleMarkers = filtered.filter(row => row.hasCoordinates).map(row => markers.get(row.id));
                if (typeof markerLayer.addLayers === "function") markerLayer.addLayers(visibleMarkers);
                else visibleMarkers.forEach(marker => markerLayer.addLayer(marker));
            }
            if (shouldFit && filtered.some(row => row.hasCoordinates)) fitResults();
        }

        function resetFilters() {
            window.clearTimeout(debounceTimer);
            state.type = "";
            state.area = "";
            state.keyword = "";
            search.value = "";
            areaSelect.value = "";
            renderResults(true);
        }

        function resizeMap() {
            const height = document.fullscreenElement === root ? window.innerHeight : window.innerHeight - root.getBoundingClientRect().top;
            root.style.height = Math.max(260, height) + "px";
            if (map) map.invalidateSize({ pan: false });
        }

        resizeMap();
        window.addEventListener("resize", resizeMap);
        if (window.visualViewport) window.visualViewport.addEventListener("resize", resizeMap);
        const header = document.querySelector("header");
        if (header && "ResizeObserver" in window) new ResizeObserver(resizeMap).observe(header);
        setSidebar(!window.matchMedia("(max-width: 767px)").matches);

        if (typeof window.L !== "undefined") {
            map = L.map("map", { zoomControl: false, minZoom: 3, maxZoom: 20, scrollWheelZoom: true, fadeAnimation: !reducedMotion, zoomAnimation: !reducedMotion, markerZoomAnimation: !reducedMotion }).setView([21.028889, 105.8342], 12);
            const layers = {
                road: L.tileLayer("https://{s}.google.com/vt/lyrs=m&x={x}&y={y}&z={z}", { maxZoom: 20, subdomains: ["mt0", "mt1", "mt2", "mt3"], attribution: "Map data &copy; Google" }),
                satellite: L.tileLayer("https://{s}.google.com/vt/lyrs=s&x={x}&y={y}&z={z}", { maxZoom: 20, subdomains: ["mt0", "mt1", "mt2", "mt3"], attribution: "Imagery &copy; Google" }),
                osm: L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", { maxZoom: 19, attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors' })
            };
            let activeLayer = "road";
            let tileErrors = 0;
            function switchLayer(key) {
                if (!layers[key] || key === activeLayer) return;
                map.removeLayer(layers[activeLayer]);
                activeLayer = key;
                tileErrors = 0;
                layers[key].addTo(map);
                root.querySelectorAll("[data-map-layer]").forEach(button => button.setAttribute("aria-pressed", String(button.dataset.mapLayer === key)));
            }
            Object.entries(layers).forEach(([key, layer]) => {
                layer.on("tileload", () => { if (key === activeLayer) tileErrors = 0; });
                layer.on("tileerror", () => {
                    if (key !== activeLayer || ++tileErrors < 5) return;
                    if (key !== "osm") {
                        switchLayer("osm");
                        showMessage("Đang chuyển sang OSM vì chưa tải được nền Google.");
                    } else if (tileErrors === 5) showMessage("Chưa tải được nền bản đồ. Bạn kiểm tra kết nối mạng nhé.");
                });
            });
            layers.road.addTo(map);
            root.querySelectorAll("[data-map-layer]").forEach(button => button.addEventListener("click", () => switchLayer(button.dataset.mapLayer)));
            markerLayer = typeof L.markerClusterGroup === "function" ? L.markerClusterGroup({
                maxClusterRadius: 48,
                showCoverageOnHover: false,
                spiderfyOnMaxZoom: true,
                disableClusteringAtZoom: 17,
                animate: !reducedMotion,
                iconCreateFunction: cluster => L.divIcon({ className: "tm-cluster", html: "<span>" + numberFormat.format(cluster.getChildCount()) + "</span>", iconSize: L.point(44, 44) })
            }) : L.featureGroup();
            markerLayer.addTo(map);
            rows.filter(row => row.hasCoordinates).forEach(row => {
                const icon = L.divIcon({
                    className: "tm-marker",
                    html: '<div class="tm-pin' + (row.type === "restaurant" ? " is-restaurant" : "") + '">' + (row.type === "restaurant" ? foodSvg : placeSvg) + "</div>",
                    iconSize: [39, 46], iconAnchor: [19, 43], popupAnchor: [0, -43], tooltipAnchor: [0, -42]
                });
                const tooltip = makeElement("span", "", row.name);
                const marker = L.marker([row.latitude, row.longitude], { icon, title: row.name, alt: row.name, keyboard: true });
                marker.bindTooltip(tooltip, { direction: "top" });
                marker.bindPopup(() => makePopup(row), { maxWidth: 300, minWidth: 250, className: "tm-location-popup", autoPanPaddingTopLeft: L.point(mapPadding().paddingTopLeft), autoPanPaddingBottomRight: L.point(mapPadding().paddingBottomRight) });
                marker.on("click", () => setSelected(row.id, true));
                marker.on("add", () => {
                    const element = marker.getElement();
                    if (element) element.classList.toggle("is-selected", row.id === state.selectedId);
                });
                markers.set(row.id, marker);
            });
            map.on("zoomend", () => {
                byId("map-zoom-in").disabled = map.getZoom() >= map.getMaxZoom();
                byId("map-zoom-out").disabled = map.getZoom() <= map.getMinZoom();
            });
        } else {
            showMessage("Chưa tải được thư viện bản đồ. Danh sách và tìm kiếm vẫn sử dụng được.", true);
            ["map-fit", "map-locate", "map-zoom-in", "map-zoom-out"].forEach(id => { byId(id).disabled = true; });
            root.querySelectorAll("[data-map-layer]").forEach(button => { button.disabled = true; });
        }

        search.addEventListener("input", () => {
            byId("map-clear-search").hidden = !search.value;
            window.clearTimeout(debounceTimer);
            debounceTimer = window.setTimeout(() => { state.keyword = search.value; renderResults(true); }, 180);
        });
        byId("map-search-form").addEventListener("submit", event => {
            event.preventDefault();
            window.clearTimeout(debounceTimer);
            state.keyword = search.value;
            renderResults(true);
            search.blur();
        });
        byId("map-clear-search").addEventListener("click", () => {
            window.clearTimeout(debounceTimer);
            search.value = "";
            state.keyword = "";
            renderResults(true);
            search.focus();
        });
        root.querySelectorAll("[data-map-type]").forEach(button => button.addEventListener("click", () => {
            state.type = state.type === button.dataset.mapType ? "" : button.dataset.mapType;
            state.keyword = search.value;
            window.clearTimeout(debounceTimer);
            renderResults(true);
        }));
        areaSelect.addEventListener("change", () => {
            state.area = areaSelect.value;
            state.keyword = search.value;
            window.clearTimeout(debounceTimer);
            renderResults(true);
        });
        byId("map-reset").addEventListener("click", resetFilters);
        list.addEventListener("click", event => {
            if (event.target.closest("[data-reset-filters]")) resetFilters();
            const button = event.target.closest("button[data-location-id]");
            if (button && !button.disabled) selectLocation(button.dataset.locationId);
        });
        byId("map-close-list").addEventListener("click", () => { setSidebar(false); openList.focus(); });
        openList.addEventListener("click", () => { setSidebar(true); byId("map-close-list").focus(); });
        byId("map-fit").addEventListener("click", fitResults);
        byId("map-zoom-in").addEventListener("click", () => { if (map) map.zoomIn(); });
        byId("map-zoom-out").addEventListener("click", () => { if (map) map.zoomOut(); });

        byId("map-locate").addEventListener("click", () => {
            if (!map) return;
            if (!navigator.geolocation) {
                showMessage("Trình duyệt này chưa hỗ trợ xác định vị trí.");
                return;
            }
            const button = byId("map-locate");
            button.disabled = true;
            button.classList.add("is-loading");
            button.setAttribute("aria-busy", "true");
            const done = () => { button.disabled = false; button.classList.remove("is-loading"); button.removeAttribute("aria-busy"); };
            showMessage("Đang xác định vị trí của bạn…", true);
            navigator.geolocation.getCurrentPosition(position => {
                done();
                const point = [position.coords.latitude, position.coords.longitude];
                if (userMarker) map.removeLayer(userMarker);
                if (accuracyCircle) map.removeLayer(accuracyCircle);
                accuracyCircle = L.circle(point, { radius: position.coords.accuracy, color: "#4285f4", fillColor: "#4285f4", fillOpacity: .09, weight: 1, interactive: false }).addTo(map);
                userMarker = L.circleMarker(point, { radius: 7, color: "#fff", fillColor: "#4285f4", fillOpacity: 1, weight: 3 }).addTo(map);
                userMarker.bindTooltip(makeElement("span", "", "Vị trí của bạn"));
                map.setView(point, 15);
                showMessage("Đã hiển thị vị trí của bạn trên bản đồ.");
            }, error => {
                done();
                showMessage(error.code === 1 ? "Bạn chưa cho phép truy cập vị trí. Hãy bật quyền vị trí cho website trong trình duyệt." : error.code === 3 ? "Xác định vị trí mất quá lâu. Bạn thử lại nhé." : "Chưa xác định được vị trí. Bạn kiểm tra GPS hoặc kết nối mạng nhé.");
            }, { enableHighAccuracy: true, timeout: 12000, maximumAge: 60000 });
        });

        const fullscreenButton = byId("map-fullscreen");
        fullscreenButton.hidden = !document.fullscreenEnabled;
        fullscreenButton.addEventListener("click", async () => {
            try {
                if (document.fullscreenElement === root) await document.exitFullscreen();
                else await root.requestFullscreen();
            } catch (error) {
                showMessage("Trình duyệt chưa cho phép mở bản đồ toàn màn hình.");
            }
        });
        document.addEventListener("fullscreenchange", () => {
            const full = document.fullscreenElement === root;
            fullscreenButton.setAttribute("aria-pressed", String(full));
            fullscreenButton.setAttribute("aria-label", full ? "Thoát toàn màn hình" : "Mở toàn màn hình");
            resizeMap();
        });

        renderResults(false);
        window.requestAnimationFrame(resizeMap);
    }

    if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", initMapPage);
    else initMapPage();
})();
