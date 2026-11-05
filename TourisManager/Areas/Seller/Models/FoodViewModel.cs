using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using TourisManager.Core.Entities;

namespace TourisManager.Areas.Seller.Models
{
    public class FoodViewModel
    {
        public string? FoodId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên món ăn")]
        [Display(Name = "Tên món ăn")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Đường dẫn ảnh")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hoặc nhập loại món ăn")]
        [Display(Name = "Loại món ăn")]
        public string TypeFoodId { get; set; } = string.Empty;

        [Display(Name = "Tên loại món mới")]
        public string? NewTypeName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá món ăn")]
        [Range(0, 100000000, ErrorMessage = "Giá món ăn phải từ 0 đ trở lên")]
        [Display(Name = "Giá bán (VNĐ)")]
        public decimal Price { get; set; }

        [Display(Name = "Kích cỡ")]
        public string? SizeName { get; set; } = "Tiêu chuẩn";

        public IFormFile? ImageFile { get; set; }
    }

    public class FoodListViewModel
    {
        public List<Food> Foods { get; set; } = new();
        public List<TypeFood> Categories { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string? SelectedTypeId { get; set; }
        public int TotalItems { get; set; }
        public int TotalCategories { get; set; }
        public decimal AvgPrice { get; set; }
    }
}

