/**
 * TourisManager - Login Scripts
 * Đường dẫn khuyến nghị: ~/js/Accounts/Login.js
 *
 * Danh sách tính năng:
 * 1. Mắt bật/tắt hiển thị mật khẩu (Show/Hide Password với icon bi-eye / bi-eye-slash)
 * 2. Ghi nhớ đăng nhập (Remember Me qua LocalStorage hoặc chuẩn bị cho Cookie)
 * 3. Hỗ trợ nút Quên mật khẩu (Mở Modal Bootstrap 5 hoặc điều hướng)
 * 4. Hiệu ứng tương tác Focus và trạng thái Loading khi nhấn Đăng nhập
 */
 
document.addEventListener('DOMContentLoaded', function () {
    // =========================================================================
    // 1. TÍNH NĂNG MẮT XEM MẬT KHẨU (TOGGLE PASSWORD VISIBILITY)
    // =========================================================================
    const passwordInput = document.getElementById('passwordInput');
    const togglePasswordBtn = document.getElementById('togglePassword');
 
    if (togglePasswordBtn && passwordInput) {
        togglePasswordBtn.addEventListener('click', function () {
            // Kiểm tra loại hiện tại của input
            const isPassword = passwordInput.getAttribute('type') === 'password';
 
            if (isPassword) {
                // Đổi sang text để thấy mật khẩu
                passwordInput.setAttribute('type', 'text');
                togglePasswordBtn.classList.remove('bi-eye-slash');
                togglePasswordBtn.classList.add('bi-eye');
                togglePasswordBtn.setAttribute('title', 'Ẩn mật khẩu');
            } else {
                // Đổi lại thành password
                passwordInput.setAttribute('type', 'password');
                togglePasswordBtn.classList.remove('bi-eye');
                togglePasswordBtn.classList.add('bi-eye-slash');
                togglePasswordBtn.setAttribute('title', 'Hiện mật khẩu');
            }
        });
    }
 
    // =========================================================================
    // 2. TÍNH NĂNG GHI NHỚ ĐĂNG NHẬP (REMEMBER ME)
    // (Lưu username vào LocalStorage phía Client + Form Post gửi boolean lên Server)
    // =========================================================================
    const usernameInput = document.querySelector('input[name="UsernameOrEmail"]');
    const rememberMeCheckbox = document.getElementById('rememberMe');
    const loginForm = document.getElementById('loginForm') || document.querySelector('form');
 
    const STORAGE_KEY_USERNAME = 'tourismanager_remembered_username';
    const STORAGE_KEY_CHECKED = 'tourismanager_remember_checked';
 
    // Đọc trạng thái đã lưu từ lần đăng nhập trước
    try {
        const savedUsername = localStorage.getItem(STORAGE_KEY_USERNAME);
        const isChecked = localStorage.getItem(STORAGE_KEY_CHECKED) === 'true';
 
        if (isChecked && rememberMeCheckbox) {
            rememberMeCheckbox.checked = true;
            if (savedUsername && usernameInput && !usernameInput.value) {
                usernameInput.value = savedUsername;
            }
        }
    } catch (err) {
        console.warn('LocalStorage không khả dụng hoặc bị chặn:', err);
    }
 
    // Khi người dùng bấm submit form
    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            // Cập nhật trạng thái ghi nhớ
            if (rememberMeCheckbox && usernameInput) {
                try {
                    if (rememberMeCheckbox.checked) {
                        localStorage.setItem(STORAGE_KEY_USERNAME, usernameInput.value.trim());
                        localStorage.setItem(STORAGE_KEY_CHECKED, 'true');
                    } else {
                        localStorage.removeItem(STORAGE_KEY_USERNAME);
                        localStorage.removeItem(STORAGE_KEY_CHECKED);
                    }
                } catch (err) {
                    console.warn('Không thể lưu vào localStorage:', err);
                }
            }
 
            // Hiệu ứng Loading nút bấm nếu form đã điền đầy đủ
            const submitBtn = loginForm.querySelector('.btn-login');
            if (submitBtn && loginForm.checkValidity()) {
                submitBtn.classList.add('disabled');
                submitBtn.innerHTML = `
                    <span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                    <span>Đang đăng nhập...</span>
                `;
            }
        });
    }
 
    // =========================================================================
    // 3. TÍNH NĂNG QUÊN MẬT KHẨU (FORGOT PASSWORD HOOK)
    // =========================================================================
    const btnSubmitForgot = document.getElementById('btnSubmitForgot');
    const forgotEmailInput = document.getElementById('forgotEmailInput');
 
    if (btnSubmitForgot && forgotEmailInput) {
        btnSubmitForgot.addEventListener('click', function () {
            const email = forgotEmailInput.value.trim();
            if (!email) {
                alert('Vui lòng nhập địa chỉ email của bạn!');
                forgotEmailInput.focus();
                return;
            }
 
            // Hiệu ứng gửi yêu cầu
            btnSubmitForgot.disabled = true;
            btnSubmitForgot.innerHTML = `
                <span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span> Đang gửi...
            `;
 
            // Giả lập hoặc gọi AJAX đến Controller của bạn (ví dụ /Account/ForgotPassword)
            setTimeout(() => {
                alert('Yêu cầu đã được ghi nhận. Vui lòng kiểm tra hộp thư email ' + email);
                btnSubmitForgot.disabled = false;
                btnSubmitForgot.innerHTML = 'Gửi yêu cầu';
                
                // Đóng Modal Bootstrap nếu có
                const forgotModalEl = document.getElementById('forgotPasswordModal');
                if (forgotModalEl && typeof bootstrap !== 'undefined') {
                    const modal = bootstrap.Modal.getInstance(forgotModalEl);
                    if (modal) modal.hide();
                }
            }, 1000);
        });
    }
});
 