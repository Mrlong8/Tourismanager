
document.addEventListener("DOMContentLoaded", function () {
    const button = document.getElementById("floatingChatButton");

    if (!button) return;

    let isDragging = false;
    let startX = 0;
    let startY = 0;
    let startLeft = 0;
    let startTop = 0;
    let moved = false;

    // Khôi phục vị trí đã lưu
    const savedPosition = localStorage.getItem("floatingChatPosition");

    if (savedPosition) {
        const position = JSON.parse(savedPosition);

        button.style.left = position.left + "px";
        button.style.top = position.top + "px";
        button.style.right = "auto";
        button.style.bottom = "auto";
    }

    // Bắt đầu nhấn giữ
    button.addEventListener("pointerdown", function (e) {
        if (e.button !== 0) return;

        isDragging = true;
        moved = false;

        const rect = button.getBoundingClientRect();

        startX = e.clientX;
        startY = e.clientY;
        startLeft = rect.left;
        startTop = rect.top;

        button.setPointerCapture(e.pointerId);
        e.preventDefault();
    });

    // Kéo nút Chat
    button.addEventListener("pointermove", function (e) {
        if (!isDragging) return;

        const dx = e.clientX - startX;
        const dy = e.clientY - startY;

        if (Math.abs(dx) > 5 || Math.abs(dy) > 5) {
            moved = true;
        }

        if (!moved) return;

        const maxLeft = window.innerWidth - button.offsetWidth;
        const maxTop = window.innerHeight - button.offsetHeight;

        const left = Math.max(0, Math.min(maxLeft, startLeft + dx));
        const top = Math.max(0, Math.min(maxTop, startTop + dy));

        button.style.left = left + "px";
        button.style.top = top + "px";
        button.style.right = "auto";
        button.style.bottom = "auto";
    });

    // Kết thúc kéo
    function stopDragging() {
        if (!isDragging) return;

        isDragging = false;

        if (moved) {
            const rect = button.getBoundingClientRect();

            localStorage.setItem(
                "floatingChatPosition",
                JSON.stringify({
                    left: rect.left,
                    top: rect.top
                })
            );
        }
    }

    button.addEventListener("pointerup", stopDragging);
    button.addEventListener("pointercancel", stopDragging);

    // Không mở trang Chat nếu vừa kéo nút
    button.addEventListener("click", function (e) {
        if (moved) {
            e.preventDefault();
            e.stopPropagation();
            moved = false;
        }
    }, true);
});
