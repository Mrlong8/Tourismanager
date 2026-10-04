using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourisManager.Core.Entities
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public string CategoryId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;

        // N - N thông qua LocationCategory
        public ICollection<LocationCategory> LocationCategories { get; set; } = new List<LocationCategory>();
    }
}