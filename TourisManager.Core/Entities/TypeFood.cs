using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TourisManager.Core.Entities
{
    [Table("TypeFood")]
    public class TypeFood
    {
        [Key]
        public string TypeFoodId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;

        // 1 - N
        public ICollection<Food> Foods { get; set; } = new List<Food>();
    }
}
