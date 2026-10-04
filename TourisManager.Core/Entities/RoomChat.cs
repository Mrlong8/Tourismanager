using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TourisManager.Core.Entities
{
    [Table("RoomChat")]
    public class RoomChat
    {
        [Key]
        public string RoomChatId { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey(nameof(Location))]
        public string LocationId { get; set; } = string.Empty;
        [ForeignKey(nameof(Account))]
        public string AccountId { get; set; } = string.Empty;

        // Navigation
        public Location Location { get; set; } = null!;
        public Account Account { get; set; } = null!;

        // 1 - N
        public ICollection<Chat> Chats { get; set; } = new List<Chat>();
    }
}
