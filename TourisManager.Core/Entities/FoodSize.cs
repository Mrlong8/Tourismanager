using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Text;

namespace TourisManager.Core.Entities
{
    [Table("FoodSize")]
    public class FoodSize
    {
        [ForeignKey(nameof(Food))]
        public string FoodId { get; set; } = string.Empty;
        [ForeignKey(nameof(Size))]
        public string SizeId { get; set; } = string.Empty;
        public decimal Price { get; set; }


        // Navigation
        public Food Food { get; set; } = null!;
        public Size Size { get; set; } = null!;
    }
}
