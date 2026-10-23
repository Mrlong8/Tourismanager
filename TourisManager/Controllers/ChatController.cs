
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TourisManager.Services;
using TourisManager.Core.Interfaces;
namespace TourisManager.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        // Danh sách phòng chat
        public async Task<IActionResult> Index()
        {
            var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(accountId))
                return RedirectToAction("Login", "Account");

            var rooms = await _chatService.GetRoomsAsync(accountId, role);

            return View(rooms);
        }

        // Mở phòng chat
        public async Task<IActionResult> Room(string id)
        {
            var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(accountId))
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(id))
                return NotFound();

            var room = await _chatService.GetRoomAsync(id);

            if (room == null)
                return NotFound();

            if (role == "Seller")
            {
                if (room.Location == null ||
                    room.Location.AccountId != accountId)
                    return Forbid();
            }
            else if (role != "Admin" && room.AccountId != accountId)
            {
                return Forbid();
            }

            ViewBag.CurrentAccountId = accountId;
            ViewBag.CurrentRole = role;

            return View(room);
        }

        // Bắt đầu trò chuyện
        [HttpGet]
        public async Task<IActionResult> Start(string locationId)
        {
            var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(accountId))
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(locationId))
                return NotFound();

            var room = await _chatService.GetOrCreateRoomAsync(
                locationId, accountId);

            return RedirectToAction(nameof(Room), new { id = room.RoomChatId });
        }

        // Gửi tin nhắn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(
            string roomChatId,
            string messageText)
        {
            var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(accountId))
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(roomChatId))
                return NotFound();

            var room = await _chatService.GetRoomAsync(roomChatId);

            if (room == null)
                return NotFound();

            // Kiểm tra quyền gửi tin nhắn
            if (role == "Seller")
            {
                if (room.Location == null ||
                    room.Location.AccountId != accountId)
                    return Forbid();
            }
            else if (role != "Admin" && room.AccountId != accountId)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(messageText))
                return RedirectToAction(nameof(Room), new { id = roomChatId });

            bool shouldAutoReply = role != "Admin" && role != "Seller";

            await _chatService.SendMessageAsync(
                roomChatId,
                accountId,
                messageText,
                shouldAutoReply);

            return RedirectToAction(nameof(Room), new { id = roomChatId });
        }
    }
}
