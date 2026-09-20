// Khai báo hàm Arrow Function để xử lý AJAX Upload Avatar
// Tham số 'input' đại diện cho thẻ HTML <input type="file"> truyền vào
const uploadAvatarAjax = (input) => {
    if(!input.files || !input.files[0]) return ; // nếu chưa chọn file thì thoát hàm

    const statusDiv = document.getElementById('uploadStatus'); // lấy thẻ hiển thị trạng thái
    if(statusDiv){
        // đang tải ảnh lên
        statusDiv.innerHTML = '<span class="text-info"><i class="bi bi-arrow-repeat spin"></i> Đang tải ảnh lên...</span>';
    }

    // 1. Khởi tạo đối tượng FormData để đóng gói dữ liệu gửi qua HTTP POST
    const formData = new FormData();
    // Gắn file ảnh được chọn vào formData với key là "avatarFile" (phải trùng với tham số C# trong Controller)
    formData.append("avatarFile", input.files[0]);

    // 2. Lấy thẻ input ẩn chứa AntiForgeryToken trong Form để phòng chống tấn công CSRF
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenInput ? tokenInput.value : '';

    // 3. Sử dụng fetch API để gửi request AJAX ngầm về Server
    fetch('/account/upload-avatar', {
        method: 'POST',
        headers: {
            // Đính kèm token xác thực vào Header của request
            'RequestVerificationToken': token
        },
        body: formData // Truyền dữ liệu file ảnh trong Body
    })
    .then(response => response.json()) // Chuyển kết quả trả về từ Server sang dạng JSON
    .then(data => {
        // Nếu Server xử lý lưu ảnh thành công (data.success === true)
        if (data.success) {
            // Lấy thẻ <img> hiển thị avatar trên View
            const avatarPreview = document.getElementById('avatarPreview');
            if (avatarPreview) {
                // Cập nhật đường dẫn src của ảnh bằng URL mới do Server trả về
                avatarPreview.src = data.avatarUrl;
            }
            // Thông báo tải ảnh thành công lên giao diện
            if (statusDiv) {
                statusDiv.innerHTML = '<span class="text-success"><i class="bi bi-check-circle"></i> Đổi Avatar thành công!</span>';
            }
        } else {
            // Trường hợp Server báo lỗi (file sai định dạng, dung lượng quá lớn,...)
            if (statusDiv) {
                statusDiv.innerHTML = `<span class="text-danger">${data.message || 'Tải ảnh thất bại!'}</span>`;
            }
        }
    })
    .catch(error => {
        // Bắt lỗi hệ thống (mất mạng, URL API không tồn tại, lỗi máy chủ 500)
        console.error('Lỗi Upload:', error);
        if (statusDiv) {
            statusDiv.innerHTML = '<span class="text-danger">Lỗi kết nối máy chủ!</span>';
        }
    });

}