using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TourisManager.Core.Entities
{
    [Table("Chat")]
    public class Chat
    {
        [Key]
        public string ChatId { get; set; }  = Guid.NewGuid().ToString();


        [ForeignKey(nameof(RoomChat))]
        public string RoomChatId { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;


        // Navigation
        public RoomChat RoomChat { get; set; } = null!;
    }
}
