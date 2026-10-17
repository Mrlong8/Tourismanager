using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TourisManager.Core.Entities
{
    public class Size
    {
        [Key]
        public string SizeId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;


        // 1 - N
        public ICollection<FoodSize> FoodSizes { get; set; }= new List<FoodSize>();
    }
}
