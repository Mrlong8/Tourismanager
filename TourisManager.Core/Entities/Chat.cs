using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourisManager.Core.Entities
{
    [Table("Chat")]
    public class Chat
    {
        [Key]
        public string ChatId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(RoomChat))]
        public string RoomChatId { get; set; } = string.Empty;

        public string MessageText { get; set; } = string.Empty;

        // Tài khoản gửi tin nhắn
        public string? SenderAccountId { get; set; }
        // Tài khoản gửi tin nhắn (null nếu là trợ lý tự động)
        [ForeignKey(nameof(SenderAccountId))]
        public Account? SenderAccount { get; set; }

        // Thời gian gửi tin nhắn
        public DateTime SentAt { get; set; } = DateTime.Now;

        public RoomChat RoomChat { get; set; } = null!;
    }
}
