using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourisManager.Core.Entities;
using TourisManager.Data;
using System;
using System.Linq;



namespace TourisManager.Controllers
{
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;

        public ChatController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(accountId))
            {
                return Content("Không tìm thấy AccountId trong Session. Hãy kiểm tra đăng nhập.");
            }

            var rooms = await _context.RoomChats
                .AsNoTracking()
                .Include(r => r.Location)
                .Where(r => r.AccountId == accountId)
                .ToListAsync();

            return View(rooms);
        }

        public async Task<IActionResult> Room(string id)
        {
            var accountId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(accountId))
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(id))
                return NotFound();

            var room = await _context.RoomChats
                .AsNoTracking()
                .Include(r => r.Location)
                .Include(r => r.Chats)
                .FirstOrDefaultAsync(r => r.RoomChatId == id);

            if (room == null)
                return NotFound();

            if (room.AccountId != accountId)
                return Forbid();

            room.Chats = room.Chats
                .OrderBy(c => c.ChatId)
                .ToList();

            return View(room);
        }

        [HttpGet]
        public async Task<IActionResult> Start(string locationId)
        {
            var accountId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(accountId))
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(locationId))
                return NotFound();

            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.LocationId == locationId);

            if (location == null)
                return NotFound();

            var room = await _context.RoomChats
                .FirstOrDefaultAsync(r =>
                    r.LocationId == locationId &&
                    r.AccountId == accountId);

            if (room == null)
            {
                room = new RoomChat
                {
                    RoomChatId = Guid.NewGuid().ToString(),
                    LocationId = locationId,
                    AccountId = accountId
                };

                _context.RoomChats.Add(room);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Room),
                new { id = room.RoomChatId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(string roomChatId, string messageText)
        {
            var accountId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(accountId))
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(messageText))
                return RedirectToAction(nameof(Room), new { id = roomChatId });

            var room = await _context.RoomChats.FirstOrDefaultAsync(r =>
                r.RoomChatId == roomChatId &&
                r.AccountId == accountId);

            if (room == null)
                return NotFound();
            // Lưu tin nhắn của khách
            var chat = new Chat
            {
                ChatId = Guid.NewGuid().ToString(),
                RoomChatId = roomChatId,
                MessageText = messageText.Trim(),
                SenderAccountId=accountId,
                SentAt= DateTime.Now
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            // Tạo tin nhắn trả lời tự động
            var autoReply = new Chat
            {
                ChatId = Guid.NewGuid().ToString(),
                RoomChatId = roomChatId,
                SenderAccountId = null,
                MessageText = GetAutoReply(messageText),
                SentAt = DateTime.Now
            };

            _context.Chats.Add(autoReply);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Room), new { id = roomChatId });
        }
        private string GetAutoReply(string message)
        {
            message = message.ToLower().Trim();

            if (message.Contains("giờ mở cửa") ||
                message.Contains("mấy giờ mở cửa") ||
                message.Contains("mở cửa"))
            {
                return "Bạn vui lòng cho biết thời gian muốn đến. " +
                       "Nhà hàng sẽ xác nhận giờ mở cửa chính xác cho bạn nhé!";
            }

            if (message.Contains("địa chỉ") ||
                message.Contains("ở đâu"))
            {
                return "Bạn có thể xem địa chỉ nhà hàng ở phần thông tin " +
                       "phía trên cuộc trò chuyện nhé!";
            }

            if (message.Contains("menu") ||
                message.Contains("thực đơn") ||
                message.Contains("món ăn"))
            {
                return "Bạn có thể xem thông tin món ăn trên trang nhà hàng. " +
                       "Nếu cần thêm thông tin, nhà hàng sẽ hỗ trợ bạn nhé!";
            }

            if (message.Contains("giá") ||
                message.Contains("bao nhiêu tiền"))
            {
                return "Bạn vui lòng cho biết món ăn hoặc dịch vụ muốn hỏi giá. " +
                       "Nhà hàng sẽ hỗ trợ bạn nhé!";
            }

            if (message.Contains("xin chào") ||
                message.Contains("hello") ||
                message.Contains("chào"))
            {
                return "Xin chào bạn! Bạn cần tư vấn về nhà hàng hoặc món ăn nào?";
            }

            return "Cảm ơn bạn đã liên hệ! Nhà hàng đã nhận được tin nhắn " +
                   "và sẽ phản hồi bạn sớm nhất có thể.";
        }
    }
}