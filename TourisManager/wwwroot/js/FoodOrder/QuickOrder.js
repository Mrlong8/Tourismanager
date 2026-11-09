// File: wwwroot/js/FoodOrder/QuickOrder.js

function openModal(foodId) {
    $.ajax({
        url: '/Food/LoadQuickOrderModal',
        type: 'GET',
        data: { id: foodId },
        success: function (html) {
            $('#quickOrderModalContent').html(html);
            $('#quickOrderModal').appendTo("body");
            updateModalPrice();

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

// Cập nhật giá tiền trên giao diện Modal
function updateModalPrice() {
    let selectedSize = $('input[name="foodSize"]:checked');
    let price = 0;

    if (selectedSize.length > 0) {
        price = parseFloat(selectedSize.val()) || 0;
    } else {
        price = parseFloat($('#modalBasePrice').val()) || 0;
    }

    let qty = parseInt($('#inputQty').val()) || 1;
    let itemTotal = price * qty;

    let deliveryFee = parseFloat($('#modalDeliveryFee').attr('data-fee')) || 0;
    let finalTotal = itemTotal + deliveryFee;

    $('#modalDisplayPrice').text(price.toLocaleString('vi-VN') + ' đ');
    $('#modalTotalPrice').text(finalTotal.toLocaleString('vi-VN') + ' đ');

    // ĐÃ XÓA ĐOẠN GỌI generateDynamicQR Ở ĐÂY
}

// Chuyển đổi hiển thị giao diện tùy theo phương thức thanh toán được chọn
function togglePaymentMethod() {
    var isQR = $('#payQR').is(':checked');
    var $qrContainer = $('#qrCodeContainer');
    var $btnCheckout = $('#btnCheckout');

    if (isQR) {
        // Chỉ đổi text nút, KHÔNG HIỆN MÃ QR Ở ĐÂY
        $btnCheckout.text("Tạo đơn và thanh toán QR").removeClass('btn-danger').addClass('btn-primary');
        updateModalPrice();
    } else {
        $qrContainer.addClass('d-none'); // Đảm bảo ẩn đi nếu người dùng đổi ý sang COD
        $btnCheckout.text("Thanh Toán khi nhận hàng").removeClass('btn-primary').addClass('btn-danger');
    }
}

// Tạo mã QR sau khi đã nhận được OrderId từ server
function generateDynamicQRAfterSubmit(orderId, totalAmount) {
    const bankId = 'MB';
    const accountNo = '0347109478';
    const template = 'compact';
    const accountName = 'BUI XUAN LONG';

    // Lấy 8 ký tự đầu của GUID để làm mã chuyển khoản cho ngắn gọn
    const shortOrderId = orderId.substring(0, 8).toUpperCase();
    const description = 'DH' + shortOrderId;

    const qrUrl = `https://img.vietqr.io/image/${bankId}-${accountNo}-${template}.png?amount=${Math.round(totalAmount)}&addInfo=${encodeURIComponent(description)}&accountName=${encodeURIComponent(accountName)}`;

    $('#qrCodeContainer').removeClass('d-none');
    $('#qrCodeImg').attr('src', qrUrl);
    $('#qrCodeContainer .text-danger').text(description);
}

let paymentCheckInterval;

// Gọi API định kỳ kiểm tra trạng thái đơn hàng và chuyển trang khi thanh toán thành công
function checkOrderStatus(orderId) {
    paymentCheckInterval = setInterval(function () {
        $.ajax({
            url: `/Food/CheckPaymentStatus?orderId=${orderId}`,
            type: 'GET',
            success: function (res) {
                if (res.isPaid) {
                    clearInterval(paymentCheckInterval);
                    alert("Thanh toán thành công!");
                    window.location.href = '/Profile/my-order';
                }
            }
        });
    }, 3000);
}

function processCheckout() {
    let foodId = $('#modalFoodId').val();
    let selectedSize = $('input[name="foodSize"]:checked');

    // SỬA LỖI: Dùng .attr('data-size-id')
    let sizeId = selectedSize.length > 0 ? selectedSize.attr('data-size-id') : null;

    let pricePerItem = 0;
    if (selectedSize.length > 0) {
        pricePerItem = parseFloat(selectedSize.val()) || 0;
    } else {
        pricePerItem = parseFloat($('#modalBasePrice').val()) || 0;
    }

    let qty = parseInt($('#inputQty').val()) || 1;
    let paymentMethodId = parseInt($('input[name="paymentMethod"]:checked').val()) || 1;

    // SỬA LỖI: Dùng .attr('data-fee')
    let deliveryFee = parseFloat($('#modalDeliveryFee').attr('data-fee')) || 0;

    let totalPrice = (pricePerItem * qty) + deliveryFee;

    let request = {
        FoodId: String(foodId),
        SizeId: sizeId,
        Quantity: qty,
        TotalPrice: totalPrice,
        DeliveryFee: deliveryFee,
        PaymentMethodID: paymentMethodId
    };

    let $btn = $('#btnCheckout');
    let originalText = $btn.text();
    $btn.prop('disabled', true).text('Đang xử lý...');

    $.ajax({
        url: '/Food/SubmitOrder',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(request),
        success: function (response) {
            if (response.success) {
                if (response.isQR) {
                    // Đơn hàng QR đã tạo thành công, tiến hành render mã QR với OrderId thực tế
                    generateDynamicQRAfterSubmit(response.orderId, response.totalAmount);
                    $('#btnCheckout').hide(); // Ẩn nút đi, chỉ chờ khách chuyển tiền

                    // Bắt đầu lắng nghe tự động (Polling hoặc dùng SignalR)
                    checkOrderStatus(response.orderId);
                } else {
                    // Trả tiền mặt (COD)
                    var modalEl = document.getElementById('quickOrderModal');
                    var myModal = bootstrap.Modal.getInstance(modalEl);
                    if (myModal) myModal.hide();
                    window.location.href = response.redirectUrl;
                }
            } else {
                alert('Lỗi: ' + response.message);
                $btn.prop('disabled', false).text(originalText);
            }
        },
        error: function (xhr) {
            alert('Có lỗi xảy ra khi kết nối với máy chủ, vui lòng thử lại!');
            $btn.prop('disabled', false).text(originalText);
        }
    });
}