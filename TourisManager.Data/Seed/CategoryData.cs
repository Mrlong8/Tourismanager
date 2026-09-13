using System;
using System.Collections.Generic;
using TourisManager.Core.Entities;

namespace TourisManager.Data.Seed
{
    public static class CategoryData
    {
        public static List<Category> GetCategories()
        {
            return new List<Category>
            {
                new Category
                {
                    CategoryId = "cat-01",
                    Name = "Di tích Lịch sử & Văn hóa",
                    Type = "Cultural & Heritage"
                },
                new Category
                {
                    CategoryId = "cat-02",
                    Name = "Danh lam Cảnh quan Tự nhiên",
                    Type = "Nature & Landscape"
                },
                new Category
                {
                    CategoryId = "cat-03",
                    Name = "Ẩm thực & Phố đi bộ",
                    Type = "Food & Culinary"
                },
                new Category
                {
                    CategoryId = "cat-04",
                    Name = "Nhà hàng & Quán ăn Đô thị",
                    Type = "Restaurant & Dining"
                },
                new Category
                {
                    CategoryId = "cat-05",
                    Name = "Bảo tàng & Không gian Nghệ thuật",
                    Type = "Museum & Art"
                },
                new Category
                {
                    CategoryId = "cat-06",
                    Name = "Khu Vui chơi & Giải trí",
                    Type = "Entertainment & Park"
                },
                new Category
                {
                    CategoryId = "cat-07",
                    Name = "Quán Cà phê & Check-in",
                    Type = "Cafe & Social"
                },
                new Category
                {
                    CategoryId = "cat-08",
                    Name = "Chợ đêm & Trung tâm Mua sắm",
                    Type = "Shopping & Night Market"
                },
                new Category
                {
                    CategoryId = "cat-09",
                    Name = "Khu Nghỉ dưỡng & Resort Spa",
                    Type = "Resort & Relaxation"
                },
                new Category
                {
                    CategoryId = "cat-10",
                    Name = "Địa điểm Du lịch Tâm linh",
                    Type = "Spiritual & Religious"
                }
            };
        }
    }
}