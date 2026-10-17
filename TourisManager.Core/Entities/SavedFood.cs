using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TourisManager.Core.Entities
{
    public class SavedFood
    {
        [ForeignKey(nameof(Food))]
        public string FoodId { get; set; } = string.Empty;
        [ForeignKey(nameof(Account))]
        public string AccountId { get; set; } = string.Empty;


        // Navigation
        public Food Food { get; set; } = null!;
        public Account Account { get; set; } = null!;
    }
}
