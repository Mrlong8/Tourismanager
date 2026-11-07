// File: wwwroot/js/FoodOrder/QuickOrder.js

function openModal(foodId) {
    $.ajax({
        url: '/Food/LoadQuickOrderModal', // Thay @Url.Action bằng đường dẫn chuỗi trực tiếp
        type: 'GET',
        data: { id: foodId },
        success: function (html) {
            $('#quickOrderModalContent').html(html);
            updateModalPrice();

            $('#quickOrderModal').appendTo("body");

            var modalEl = document.getElementById('quickOrderModal');
            var myModal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
            myModal.show();
        },
        error: function (xhr) {
            alert('Lỗi tải dữ liệu món ăn: ' + (xhr.responseText || 'Không tìm thấy thông tin món ăn!'));
        }
    });
}

function updateQty(change) {
    let currentQty = parseInt($('#inputQty').val()) || 1;
    let newQty = currentQty + change;
    if (newQty >= 1) {
        $('#inputQty').val(newQty);
        updateModalPrice();
    }
}

function updateModalPrice() {
    let price = parseFloat($('input[name="foodSize"]:checked').val()) || 0;
    let qty = parseInt($('#inputQty').val()) || 1;
    let total = price * qty;

    $('#modalDisplayPrice').text(price.toLocaleString('vi-VN') + ' đ');
    $('#modalTotalPrice').text(total.toLocaleString('vi-VN') + ' đ');
}