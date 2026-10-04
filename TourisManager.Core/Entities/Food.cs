using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TourisManager.Core.Entities
{
    public class Food
    {
        [Key]
        public string FoodId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        [ForeignKey(nameof(TypeFood))]
        public string TypeFoodId { get; set; } = string.Empty;
        [ForeignKey(nameof(Location))]
        public string LocationId { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; } = DateTime.Now;


        // Navigation
        public TypeFood TypeFood { get; set; } = null!;
        public Location Location { get; set; } 
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public ICollection<SavedFood> SavedFoods { get; set; }= new List<SavedFood>();
        public ICollection<FoodSize> FoodSizes { get; set; } = new List<FoodSize>();
    }
}
