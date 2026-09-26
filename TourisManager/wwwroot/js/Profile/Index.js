// Biến toàn cục lưu lại URL ảnh ban đầu để tính năng "Cancel" có thể khôi phục lại
let originalAvatarUrl = '';

document.addEventListener("DOMContentLoaded", function () {
    const avatarImg = document.getElementById('avatarPreview');
    if (avatarImg) {
        originalAvatarUrl = avatarImg.src; 
    }
});

// 1. Hàm xem trước ảnh & Hiển thị cụm nút Cancel/Upload
function previewAvatar(input) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            // Đổi src ảnh thành ảnh vừa chọn
            document.getElementById('avatarPreview').src = e.target.result;
            
            // Ẩn nút Edit
            document.getElementById('defaultAvatarActions').classList.add('d-none');
            
            // Hiện cụm Cancel / Upload
            const uploadActions = document.getElementById('uploadAvatarActions');
            uploadActions.classList.remove('d-none');
            uploadActions.classList.add('d-flex');
            
            // Xóa thông báo cũ nếu có
            document.getElementById('uploadStatus').innerHTML = '';
        }
        reader.readAsDataURL(input.files[0]);
    }
}

// 2. Hàm Hủy (Cancel)
function cancelAvatarUpload() {
    // Khôi phục lại ảnh ban đầu
    document.getElementById('avatarPreview').src = originalAvatarUrl;
    
    // Xóa dữ liệu file trong thẻ input
    document.getElementById('avatarFile').value = '';
    
    // Ẩn cụm Cancel / Upload
    const uploadActions = document.getElementById('uploadAvatarActions');
    uploadActions.classList.remove('d-flex');
    uploadActions.classList.add('d-none');
    
    // Hiện lại nút Edit
    document.getElementById('defaultAvatarActions').classList.remove('d-none');
}

// 3. Hàm đẩy ảnh lên Server bằng AJAX
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
        headers: {
            'RequestVerificationToken': token
        },
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Cập nhật URL ảnh gốc mới
            originalAvatarUrl = data.avatarUrl;
            
            // Reset giao diện về trạng thái nút Edit
            cancelAvatarUpload();
            
            // Đảm bảo ảnh mới được hiển thị
            document.getElementById('avatarPreview').src = originalAvatarUrl;
            
            statusDiv.innerHTML = '<span class="text-success fw-medium"><i class="bi bi-check-circle-fill me-1"></i>Đổi Avatar thành công!</span>';
            
            // Tự động ẩn thông báo sau 3 giây
            setTimeout(() => {
                statusDiv.innerHTML = '';
            }, 3000);
        } else {
            statusDiv.innerHTML = `<span class="text-danger fw-medium">${data.message || 'Tải ảnh thất bại!'}</span>`;
        }
    })
    .catch(error => {
        console.error('Lỗi Upload:', error);
        statusDiv.innerHTML = '<span class="text-danger fw-medium">Lỗi kết nối máy chủ!</span>';
    });
}