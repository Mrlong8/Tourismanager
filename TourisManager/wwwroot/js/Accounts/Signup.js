
document.addEventListener('DOMContentLoaded', function () {
    // =========================================================================
    // 1. TÍNH NĂNG MẮT XEM MẬT KHẨU (PASSWORD & CONFIRM PASSWORD)
    // =========================================================================
    function setupPasswordToggle(inputId, toggleId) {
        const input = document.getElementById(inputId);
        const toggle = document.getElementById(toggleId);
 
        if (input && toggle) {
            toggle.addEventListener('click', function () {
                const isPassword = input.getAttribute('type') === 'password';
 
                if (isPassword) {
                    input.setAttribute('type', 'text');
                    toggle.classList.remove('bi-eye-slash');
                    toggle.classList.add('bi-eye');
                    toggle.setAttribute('title', 'Ẩn mật khẩu');
                } else {
                    input.setAttribute('type', 'password');
                    toggle.classList.remove('bi-eye');
                    toggle.classList.add('bi-eye-slash');
                    toggle.setAttribute('title', 'Hiện mật khẩu');
                }
            });
        }
    }
 
    setupPasswordToggle('passwordInput', 'togglePassword');
    setupPasswordToggle('confirmPasswordInput', 'toggleConfirmPassword');
 
    // =========================================================================
    // 2. ĐÁNH GIÁ ĐỘ MẠNH CỦA MẬT KHẨU (PASSWORD STRENGTH METER)
    // =========================================================================
    const passwordInput = document.getElementById('passwordInput');
    const strengthBar = document.getElementById('passwordStrengthBar');
    const strengthHint = document.getElementById('passwordHint');
 
    if (passwordInput && strengthBar) {
        passwordInput.addEventListener('input', function () {
            const val = this.value;
            let score = 0;
 
            if (val.length >= 6) score += 25;
            if (val.length >= 10) score += 25;
            if (/[A-Z]/.test(val)) score += 20;
            if (/[0-9]/.test(val)) score += 15;
            if (/[^A-Za-z0-9]/.test(val)) score += 15;
 
            strengthBar.style.width = score + '%';
 
            if (score <= 25) {
                strengthBar.style.backgroundColor = '#ef4444'; // Đỏ
                if (strengthHint) strengthHint.textContent = 'Mật khẩu yếu: Thêm chữ hoa, số và ký tự';
            } else if (score <= 65) {
                strengthBar.style.backgroundColor = '#f59e0b'; // Vàng
                if (strengthHint) strengthHint.textContent = 'Mật khẩu khá: Dùng thêm ký tự đặc biệt để an toàn hơn';
            } else {
                strengthBar.style.backgroundColor = '#10b981'; // Xanh
                if (strengthHint) strengthHint.textContent = 'Mật khẩu rất mạnh và an toàn';
            }
 
            if (val.length === 0) {
                strengthBar.style.width = '0%';
                if (strengthHint) strengthHint.textContent = 'Dùng chữ hoa, số và ký tự đặc biệt để tăng độ mạnh';
            }
        });
    }
 
    // =========================================================================
    // 3. KIỂM TRA MẬT KHẨU TRÙNG KHỚP & TRẠNG THÁI LOADING FORM
    // =========================================================================
    const signupForm = document.getElementById('signupForm') || document.querySelector('form');
    const confirmPasswordInput = document.getElementById('confirmPasswordInput');
    const confirmFeedback = document.getElementById('confirmPasswordFeedback');
 
    if (confirmPasswordInput && passwordInput) {
        confirmPasswordInput.addEventListener('input', function () {
            if (this.value && this.value !== passwordInput.value) {
                if (confirmFeedback) {
                    confirmFeedback.textContent = 'Mật khẩu xác nhận không trùng khớp!';
                    confirmFeedback.classList.remove('d-none');
                }
            } else {
                if (confirmFeedback) {
                    confirmFeedback.textContent = '';
                    confirmFeedback.classList.add('d-none');
                }
            }
        });
    }
 
    if (signupForm) {
        signupForm.addEventListener('submit', function (e) {
            if (passwordInput && confirmPasswordInput && passwordInput.value !== confirmPasswordInput.value) {
                e.preventDefault();
                alert('Mật khẩu xác nhận không khớp với mật khẩu đã nhập!');
                confirmPasswordInput.focus();
                return;
            }
 
            const submitBtn = signupForm.querySelector('.btn-signup-submit');
            if (submitBtn && signupForm.checkValidity()) {
                submitBtn.classList.add('disabled');
                submitBtn.innerHTML = `
                    <span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                    <span>Đang khởi tạo tài khoản...</span>
                `;
            }
        });
    }
});