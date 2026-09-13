using System;
using System.Collections.Generic;
using TourisManager.Core.Entities;

namespace TourisManager.Data.Seed
{
    public static class RestaurantData
    {
        public static List<Restaurant> GetRestaurants()
        {
            return new List<Restaurant>
            {
                // 1. Phở Thìn Bờ Hồ
                new Restaurant
                {
                    RestaurantId = "res-01",
                    Name = "Phở Thìn Bờ Hồ",
                    Description = "Quán phở truyền thống nổi tiếng từ năm 1955 với nước dùng trong, ngọt thanh vị xương hầm và thịt bò tái lăn đậm đà.",
                    CategoryId = "cat-04", // Nhà hàng & Quán ăn
                    Address = "61 Phố Đinh Tiên Hoàng, Phường Lý Thái Tổ, Quận Hoàn Kiếm, Hà Nội",
                    Latitude = 21.030112m,
                    Longitude = 105.853215m,
                    CreateBy = "usr-01",
                    CreateAt = new DateTime(2026, 1, 1, 8, 0, 0)
                },
                // 2. Bún Chả Hương Liên (Bún Chả Obama)
                new Restaurant
                {
                    RestaurantId = "res-02",
                    Name = "Bún Chả Hương Liên (Bún Chả Obama)",
                    Description = "Quán bún chả nổi tiếng thế giới từng đón tiếp Tổng thống Mỹ Barack Obama năm 2016, chả nướng than hoa thơm lừng.",
                    CategoryId = "cat-04",
                    Address = "24 Phố Lê Văn Hưu, Phường Phạm Đình Hổ, Quận Hai Bà Trưng, Hà Nội",
                    Latitude = 21.018915m,
                    Longitude = 105.853982m,
                    CreateBy = "usr-01",
                    CreateAt = new DateTime(2026, 1, 2, 8, 0, 0)
                },
                // 3. Chả Cá Lăng Thăng Long
                new Restaurant
                {
                    RestaurantId = "res-03",
                    Name = "Chả Cá Thăng Long",
                    Description = "Một trong những địa điểm thưởng thức chả cá lăng nướng chảo chuẩn vị Hà Thành với thì là, hành lá và mắm tôm ngon xuất sắc.",
                    CategoryId = "cat-04",
                    Address = "21 Phố Đường Thành, Phường Cửa Đông, Quận Hoàn Kiếm, Hà Nội",
                    Latitude = 21.033605m,
                    Longitude = 105.845425m,
                    CreateBy = "usr-02",
                    CreateAt = new DateTime(2026, 1, 3, 8, 0, 0)
                },
                // 4. Bún Đậu Mắm Tôm Ngõ Tràng Thiện
                new Restaurant
                {
                    RestaurantId = "res-04",
                    Name = "Bún Đậu Mắm Tôm Ngõ Tràng Tiền",
                    Description = "Quán bún đậu ngõ nhỏ phố cổ nổi tiếng với đậu rán giòn tan, chả cốm béo ngậy và mắm tôm pha cực vừa miệng.",
                    CategoryId = "cat-03", // Ẩm thực đường phố
                    Address = "2 Ngõ Tràng Tiền, Phường Tràng Tiền, Quận Hoàn Kiếm, Hà Nội",
                    Latitude = 21.025988m,
                    Longitude = 105.855210m,
                    CreateBy = "usr-02",
                    CreateAt = new DateTime(2026, 1, 4, 8, 0, 0)
                },
                // 5. Bánh Mì Dân Tổ
                new Restaurant
                {
                    RestaurantId = "res-05",
                    Name = "Bánh Mì Dân Tổ Hà Nội",
                    Description = "Thương hiệu bánh mì đêm khống đụng hàng với nhân pate, bơ, trứng, xíu mại được xào chung quánh quyện béo ngậy.",
                    CategoryId = "cat-03",
                    Address = "32 Phố Trần Nhật Duật, Phường Đồng Xuân, Quận Hoàn Kiếm, Hà Nội",
                    Latitude = 21.037542m,
                    Longitude = 105.853821m,
                    CreateBy = "usr-02",
                    CreateAt = new DateTime(2026, 1, 5, 8, 0, 0)
                },
                // 6. Phở Cuốn Hương Mai - Trúc Bạch
                new Restaurant
                {
                    RestaurantId = "res-06",
                    Name = "Phở Cuốn Hương Mai",
                    Description = "Nơi khởi nguồn cho món phở cuốn Hà Nội. Bánh phở mỏng dai cuốn thịt bò xào thơm cùng rau sống chấm nước mắm chua ngọt.",
                    CategoryId = "cat-04",
                    Address = "25 Phố Ngũ Xã, Phường Trúc Bạch, Quận Ba Đình, Hà Nội",
                    Latitude = 21.047120m,
                    Longitude = 105.842315m,
                    CreateBy = "usr-03",
                    CreateAt = new DateTime(2026, 1, 6, 8, 0, 0)
                },
                // 7. Nét Huế - Ẩm Thực Cố Đô
                new Restaurant
                {
                    RestaurantId = "res-07",
                    Name = "Nhà Hàng Nét Huế",
                    Description = "Chuỗi nhà hàng mang trọn vị ẩm thực xứ Huế giữa lòng Hà Nội với Bún bò Huế, bánh ram ít, bánh lọc và nem lụi nướng.",
                    CategoryId = "cat-04",
                    Address = "43 Phố Mai Hắc Đế, Phường Bùi Thị Xuân, Quận Hai Bà Trưng, Hà Nội",
                    Latitude = 21.014382m,
                    Longitude = 105.850912m,
                    CreateBy = "usr-03",
                    CreateAt = new DateTime(2026, 1, 7, 8, 0, 0)
                },
                // 8. Bún Thăng Hàng Hòm
                new Restaurant
                {
                    RestaurantId = "res-08",
                    Name = "Bún Thang Bà Đức - Hàng Hòm",
                    Description = "Món bún đòi hỏi sự cầu kỳ bậc nhất Hà Thành với nước dùng gà thanh trong, trứng tráng mỏng thái chỉ và giò lụa mỏng.",
                    CategoryId = "cat-04",
                    Address = "48 Phố Hàng Hòm, Phường Hàng Gai, Quận Hoàn Kiếm, Hà Nội",
                    Latitude = 21.032105m,
                    Longitude = 105.849120m,
                    CreateBy = "usr-03",
                    CreateAt = new DateTime(2026, 1, 8, 8, 0, 0)
                },
                // 9. Quán Ăn Ngon - Lý Thường Kiệt
                new Restaurant
                {
                    RestaurantId = "res-09",
                    Name = "Quán Ăn Ngon",
                    Description = "Không gian biệt thự cổ quy tụ hàng trăm món ăn đặc sản 3 miền Bắc - Trung - Nam trong không gian hoài cổ.",
                    CategoryId = "cat-04",
                    Address = "18 Phố Phan Bội Châu, Phường Cửa Nam, Quận Hoàn Kiếm, Hà Nội",
                    Latitude = 21.026815m,
                    Longitude = 105.842890m,
                    CreateBy = "usr-01",
                    CreateAt = new DateTime(2026, 1, 9, 8, 0, 0)
                },
                // 10. Cà Phê Giảng (Bánh & Cà phê Trứng)
                new Restaurant
                {
                    RestaurantId = "res-10",
                    Name = "Cà Phê Giảng (Cà Phê Trứng Cổ Nhất Hà Nội)",
                    Description = "Nơi khai sinh ra món Cà phê trứng huyền thoại từ năm 1946 với lớp kem trứng béo ngậy quyện vị đắng cà phê nguyên chất.",
                    CategoryId = "cat-07", // Cà phê & Check-in
                    Address = "39 Phố Nguyễn Hữu Huân, Phường Lý Thái Tổ, Quận Hoàn Kiếm, Hà Nội",
                    Latitude = 21.032481m,
                    Longitude = 105.854915m,
                    CreateBy = "usr-01",
                    CreateAt = new DateTime(2026, 1, 10, 8, 0, 0)
                }
            };
        }
    }
}