using System.ComponentModel.DataAnnotations.Schema;

namespace TourisManager.Core.Entities
{
    [Table("ReviewContent")]
    public class ReviewContent
    {
        public string ReviewContentId { get; set; }
        public string ReviewId { get; set; }
        public string ContentUrl { get; set; } = string.Empty; // Lưu đường dẫn ảnh/video đính kèm trong review

        // Navigation Property
        public Review? Review { get; set; }
    }
}