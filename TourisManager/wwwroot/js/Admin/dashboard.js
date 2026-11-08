document.addEventListener("DOMContentLoaded", function () {

    // Đọc dữ liệu được truyền từ Index.cshtml
    const dataElement = document.getElementById("dashboard-data");

    if (!dataElement) {
        console.error("Không tìm thấy dữ liệu Dashboard.");
        return;
    }

    const dashboardData = JSON.parse(dataElement.textContent);

    // Kiểm tra thư viện Chart.js
    if (typeof Chart === "undefined") {
        console.error("Không thể tải thư viện Chart.js.");
        return;
    }

    // Dữ liệu thống kê
    const usersByRole = dashboardData.usersByRole || {};
    const locationsByArea = dashboardData.locationsByArea || {};

    // Vẽ biểu đồ User
    createUsersChart(usersByRole);

    // Vẽ biểu đồ Location
    createLocationsChart(locationsByArea);
});


/* =====================================
   BIỂU ĐỒ USER THEO VAI TRÒ
===================================== */

function createUsersChart(data) {

    const labels = Object.keys(data);
    const values = Object.values(data);

    const canvas = document.getElementById("usersByRoleChart");
    const container = document.getElementById("usersChartContainer");
    const emptyMessage = document.getElementById("usersChartEmpty");

    if (labels.length === 0) {
        container.hidden = true;
        emptyMessage.hidden = false;
        return;
    }

    new Chart(canvas, {
        type: "bar",

        data: {
            labels: labels,

            datasets: [{
                label: "Số lượng tài khoản",
                data: values,

                backgroundColor: "rgba(37, 99, 235, 0.85)",
                borderColor: "#2563EB",
                borderWidth: 1,
                borderRadius: 6,
                borderSkipped: false,
                barThickness: 25
            }]
        },

        options: {
            indexAxis: "y",
            responsive: true,
            maintainAspectRatio: false,

            plugins: {
                legend: {
                    display: false
                }
            },

            scales: {
                x: {
                    beginAtZero: true,
                    ticks: {
                        precision: 0,
                        stepSize: 1
                    }
                },

                y: {
                    grid: {
                        display: false
                    }
                }
            }
        }
    });
}


/* =====================================
   BIỂU ĐỒ LOCATION THEO KHU VỰC
===================================== */
function createLocationsChart(data) {

    // Sắp xếp khu vực theo số lượng địa điểm giảm dần
    const sortedAreas = Object.entries(data)
        .sort((a, b) => b[1] - a[1]);

    // Lấy 10 khu vực có nhiều địa điểm nhất
    const top10 = sortedAreas.slice(0, 10);

    // Tính tổng số địa điểm của các khu vực còn lại
    const otherTotal = sortedAreas
        .slice(10)
        .reduce((total, area) => total + area[1], 0);

    // Tạo dữ liệu biểu đồ
    const labels = top10.map(area => area[0]);
    const values = top10.map(area => area[1]);

    // Gộp các khu vực còn lại thành một nhóm
    if (otherTotal > 0) {
        labels.push("Khu vực khác");
        values.push(otherTotal);
    }

    const canvas = document.getElementById("locationsByAreaChart");
    const container = document.getElementById("locationsChartContainer");
    const emptyMessage = document.getElementById("locationsChartEmpty");

    if (labels.length === 0) {
        container.hidden = true;
        emptyMessage.hidden = false;
        return;
    }

    new Chart(canvas, {
        type: "bar",

        data: {
            labels: labels,

            datasets: [{
                label: "Số lượng địa điểm",
                data: values,

                backgroundColor: labels.map(label =>
                    label === "Khu vực khác"
                        ? "rgba(148, 163, 184, 0.75)"
                        : "rgba(22, 163, 74, 0.85)"
                ),

                borderColor: labels.map(label =>
                    label === "Khu vực khác"
                        ? "#94A3B8"
                        : "#16A34A"
                ),

                borderWidth: 1,
                borderRadius: 6,
                borderSkipped: false,
                maxBarThickness: 45
            }]
        },

        options: {
            responsive: true,
            maintainAspectRatio: false,

            plugins: {
                legend: {
                    display: false
                },

                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return " " + context.parsed.y + " địa điểm";
                        }
                    }
                }
            },

            scales: {
                x: {
                    grid: {
                        display: false
                    },

                    ticks: {
                        autoSkip: false,
                        maxRotation: 35,
                        minRotation: 0
                    }
                },

                y: {
                    beginAtZero: true,

                    ticks: {
                        precision: 0
                    }
                }
            }
        }
    });
}