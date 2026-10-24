
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourisManager.Core.Entities;
using TourisManager.Data;
using TourisManager.Core.Interfaces;
namespace TourisManager.Services
{
    public class ChatService : IChatService
    {
        private readonly AppDbContext _context;
        private readonly ChatAutoReplyService _autoReplyService;

        public ChatService(
            AppDbContext context,
            ChatAutoReplyService autoReplyService)
        {
            _context = context;
            _autoReplyService = autoReplyService;
        }

        public async Task<List<RoomChat>> GetRoomsAsync(
            string accountId,
            string role)
        {
            var query = _context.RoomChats
                .AsNoTracking()
                .Include(r => r.Location)
                .Include(r => r.Account)
                .AsQueryable();

            if (role == "Seller")
            {
                query = query.Where(r =>
                    r.Location != null &&
                    r.Location.AccountId == accountId);
            }
            else if (role != "Admin")
            {
                query = query.Where(r => r.AccountId == accountId);
            }

            return await query.ToListAsync();
        }

        public async Task<RoomChat?> GetRoomAsync(string roomChatId)
        {
            var room = await _context.RoomChats
                .Include(r => r.Location)
                .Include(r => r.Chats)
                .FirstOrDefaultAsync(r => r.RoomChatId == roomChatId);

            if (room != null)
            {
                room.Chats = room.Chats
                    .OrderBy(c => c.SentAt)
                    .ToList();
            }

            return room;
        }

        public async Task<RoomChat> GetOrCreateRoomAsync(
            string locationId,
            string accountId)
        {
            var room = await _context.RoomChats
                .FirstOrDefaultAsync(r =>
                    r.LocationId == locationId &&
                    r.AccountId == accountId);

            if (room != null)
                return room;

            room = new RoomChat
            {
                RoomChatId = Guid.NewGuid().ToString(),
                LocationId = locationId,
                AccountId = accountId
            };

            _context.RoomChats.Add(room);
            await _context.SaveChangesAsync();

            return room;
        }

        public async Task SendMessageAsync(
            string roomChatId,
            string accountId,
            string messageText,
            bool shouldAutoReply)
        {
            var message = new Chat
            {
                ChatId = Guid.NewGuid().ToString(),
                RoomChatId = roomChatId,
                MessageText = messageText.Trim(),
                SenderAccountId = accountId,
                SentAt = DateTime.Now
            };

            _context.Chats.Add(message);
            await _context.SaveChangesAsync();

            if (shouldAutoReply)
            {
                var autoReply = new Chat
                {
                    ChatId = Guid.NewGuid().ToString(),
                    RoomChatId = roomChatId,
                    SenderAccountId = null,
                    MessageText = _autoReplyService.GetAutoReply(messageText),
                    SentAt = DateTime.Now
                };

                _context.Chats.Add(autoReply);
                await _context.SaveChangesAsync();
            }
        }
    }
}
