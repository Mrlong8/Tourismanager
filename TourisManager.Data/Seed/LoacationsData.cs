using System;
using System.Collections.Generic;
using TourisManager.Core.Entities;

namespace TourisManager.Data.Seed
{
    public static class LocationData
    {
        public static List<Location> GetLocations()
        {
            return new List<Location>
            {
                // 1. Hồ Hoàn Kiếm
                new Location
                {
                    LocationId = "loc-01",
                    Name = "Hồ Hoàn Kiếm & Đền Ngọc Sơn",
                    Description = "Trái tim của thủ đô Hà Nội, nổi tiếng với Tháp Rùa cổ kính, cầu Thê Húc màu đỏ son và không gian phố đi bộ nhộn nhịp vào cuối tuần.",
                    CategoryId = "cat-01", // Di tích lịch sử / Văn hóa
                    Address = "Phố Đinh Tiên Hoàng, Phường Hàng Trống, Quận Hoàn Kiếm, Hà Nội",
                    Latitude = 21.028511m,
                    Longitude = 105.854167m,
                    ImageUrl = "https://images.unsplash.com/photo-1599707367072-cd6ada2bc375?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-01",
                    CreateAt = new DateTime(2026, 1, 1, 8, 0, 0)
                },
                // 2. Hoàng Thành Thăng Long
                new Location
                {
                    LocationId = "loc-02",
                    Name = "Hoàng Thành Thăng Long",
                    Description = "Quần thể di tích gắn liền với lịch sử kinh thành Thăng Long - Hà Nội, Di sản Thế giới được UNESCO công nhận.",
                    CategoryId = "cat-01",
                    Address = "19C Hoàng Diệu, Phường Điện Biên, Quận Ba Đình, Hà Nội",
                    Latitude = 21.034789m,
                    Longitude = 105.839815m,
                    ImageUrl = "https://images.unsplash.com/photo-1627918571827-010e9791497a?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-01",
                    CreateAt = new DateTime(2026, 1, 2, 8, 0, 0)
                },
                // 3. Văn Miếu - Quốc Tử Giám
                new Location
                {
                    LocationId = "loc-03",
                    Name = "Văn Miếu - Quốc Tử Giám",
                    Description = "Trường Đại học đầu tiên của Việt Nam, nơi thờ Khổng Tử và lưu giữ 82 bia Tiến sĩ vinh danh tri thức xưa.",
                    CategoryId = "cat-01",
                    Address = "58 Phố Quốc Tử Giám, Phường Văn Miếu, Quận Đống Đa, Hà Nội",
                    Latitude = 21.029358m,
                    Longitude = 105.835974m,
                    ImageUrl = "https://images.unsplash.com/photo-1590059207002-368297b69a83?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-01",
                    CreateAt = new DateTime(2026, 1, 3, 8, 0, 0)
                },
                // 4. Lăng Chủ tịch Hồ Chí Minh
                new Location
                {
                    LocationId = "loc-04",
                    Name = "Lăng Chủ tịch Hồ Chí Minh",
                    Description = "Nơi giữ gìn thi hài Chủ tịch Hồ Chí Minh, nằm trên quảng trường Ba Đình lịch sử.",
                    CategoryId = "cat-01",
                    Address = "2 Hùng Vương, Phường Điện Biên, Quận Ba Đình, Hà Nội",
                    Latitude = 21.036802m,
                    Longitude = 105.834641m,
                    ImageUrl = "https://images.unsplash.com/photo-1583417319070-4a69db38a482?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-01",
                    CreateAt = new DateTime(2026, 1, 4, 8, 0, 0)
                },
                // 5. Chùa Trấn Quốc
                new Location
                {
                    LocationId = "loc-05",
                    Name = "Chùa Trấn Quốc",
                    Description = "Ngôi chùa cổ nhất Hà Nội với lịch sử hơn 1500 năm, nằm trên hòn đảo phía Đông Hồ Tây.",
                    CategoryId = "cat-10", // Du lịch tâm linh
                    Address = "46 Đường Thanh Niên, Phường Yên Phụ, Quận Tây Hồ, Hà Nội",
                    Latitude = 21.047805m,
                    Longitude = 105.836814m,
                    ImageUrl = "https://images.unsplash.com/photo-1528127269322-539801943592?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-01",
                    CreateAt = new DateTime(2026, 1, 5, 8, 0, 0)
                },
                // 6. Nhà thờ Lớn Hà Nội
                new Location
                {
                    LocationId = "loc-06",
                    Name = "Nhà thờ Lớn Hà Nội (St. Joseph's Cathedral)",
                    Description = "Công trình kiến trúc Gothic tuyệt đẹp được xây dựng từ thời Pháp thuộc, điểm check-in trà chanh quen thuộc.",
                    CategoryId = "cat-07", // Cà phê & Check-in
                    Address = "40 Phố Nhà Chung, Phường Hàng Trống, Quận Hoàn Kiếm, Hà Nội",
                    Latitude = 21.028787m,
                    Longitude = 105.849015m,
                    ImageUrl = "https://images.unsplash.com/photo-1509030450996-939a26353926?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-02",
                    CreateAt = new DateTime(2026, 1, 6, 8, 0, 0)
                },
                // 7. Chợ Đồng Xuân
                new Location
                {
                    LocationId = "loc-07",
                    Name = "Chợ Đồng Xuân",
                    Description = "Khu chợ bán buôn lớn nhất Phố Cổ Hà Nội, nơi hội tụ hàng ngàn mặt hàng và vô số món ăn ẩm thực đường phố.",
                    CategoryId = "cat-08", // Chợ đêm & Mua sắm
                    Address = "Phố Đồng Xuân, Phường Đồng Xuân, Quận Hoàn Kiếm, Hà Nội",
                    Latitude = 21.038198m,
                    Longitude = 105.849646m,
                    ImageUrl = "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-02",
                    CreateAt = new DateTime(2026, 1, 7, 8, 0, 0)
                },
                // 8. Cầu Long Biên
                new Location
                {
                    LocationId = "loc-08",
                    Name = "Cầu Long Biên",
                    Description = "Cây cầu thép lịch sử vươn qua sông Hồng do kiến trúc sư Gustave Eiffel thiết kế, chứng nhân lịch sử Hà Nội.",
                    CategoryId = "cat-01",
                    Address = "Cầu Long Biên, Phường Đồng Xuân, Quận Hoàn Kiếm, Hà Nội",
                    Latitude = 21.042308m,
                    Longitude = 105.857317m,
                    ImageUrl = "https://images.unsplash.com/photo-1616486338812-3dadae4b4ace?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-02",
                    CreateAt = new DateTime(2026, 1, 8, 8, 0, 0)
                },
                // 9. Bảo tàng Dân tộc học Việt Nam
                new Location
                {
                    LocationId = "loc-09",
                    Name = "Bảo tàng Dân tộc học Việt Nam",
                    Description = "Nơi trưng bày sinh động văn hóa, phong tục tập quán của 54 dân tộc anh em trên khắp dải đất Việt Nam.",
                    CategoryId = "cat-05", // Bảo tàng & Nghệ thuật
                    Address = "Đường Nguyễn Văn Huyên, Phường Nghĩa Đô, Quận Cầu Giấy, Hà Nội",
                    Latitude = 21.040608m,
                    Longitude = 105.798606m,
                    ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-03",
                    CreateAt = new DateTime(2026, 1, 9, 8, 0, 0)
                },
                // 10. Phố Cổ Hà Nội (Hà Nội Old Quarter)
                new Location
                {
                    LocationId = "loc-10",
                    Name = "Khu Phố Cổ Hà Nội (36 Phố Phường)",
                    Description = "Khu vực lưu giữ kiến trúc đầu thế kỷ 20 với các con phố hàng nghề truyền thống và nhịp sống về đêm sôi động.",
                    CategoryId = "cat-03", // Ẩm thực & Phố đi bộ
                    Address = "Quận Hoàn Kiếm, Hà Nội",
                    Latitude = 21.033722m,
                    Longitude = 105.850974m,
                    ImageUrl = "https://images.unsplash.com/photo-1508804185872-d7badad00f7d?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-03",
                    CreateAt = new DateTime(2026, 1, 10, 8, 0, 0)
                },
                // 11. Vườn Quốc gia Ba Vì
                new Location
                {
                    LocationId = "loc-11",
                    Name = "Vườn Quốc gia Ba Vì",
                    Description = "Khu du lịch sinh thái nổi tiếng với không khí mát mẻ, nhà kính xương rồng và đồi hoa dại tuyệt đẹp.",
                    CategoryId = "cat-02", // Sinh thái & Tự nhiên
                    Address = "Xã Tản Lĩnh, Huyện Ba Vì, Hà Nội",
                    Latitude = 21.077222m,
                    Longitude = 105.362500m,
                    ImageUrl = "https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-03",
                    CreateAt = new DateTime(2026, 1, 11, 8, 0, 0)
                },
                // 12. Làng cổ Đường Lâm
                new Location
                {
                    LocationId = "loc-12",
                    Name = "Làng cổ Đường Lâm",
                    Description = "Ngôi làng cổ nổi tiếng với những ngôi nhà đá ong hàng trăm năm tuổi và cổng làng cây đa giếng nước cổ kính.",
                    CategoryId = "cat-01",
                    Address = "Xã Đường Lâm, Thị xã Sơn Tây, Hà Nội",
                    Latitude = 21.155833m,
                    Longitude = 105.473056m,
                    ImageUrl = "https://images.unsplash.com/photo-1544644181-1484b3fdfc62?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-01",
                    CreateAt = new DateTime(2026, 1, 12, 8, 0, 0)
                },
                // 13. Vịnh Hạ Long (Quảng Ninh)
                new Location
                {
                    LocationId = "loc-13",
                    Name = "Vịnh Hạ Long",
                    Description = "Một trong 7 Kỳ quan Thiên nhiên Mới của Thế giới với hàng ngàn đảo đá vôi kỳ vĩ rải rác trên biển ngọc.",
                    CategoryId = "cat-02",
                    Address = "TP. Hạ Long, Tỉnh Quảng Ninh",
                    Latitude = 20.910051m,
                    Longitude = 107.183902m,
                    ImageUrl = "https://images.unsplash.com/photo-1528127269322-539801943592?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-01",
                    CreateAt = new DateTime(2026, 1, 13, 8, 0, 0)
                },
                // 14. Quần thể Danh thắng Tràng An (Ninh Bình)
                new Location
                {
                    LocationId = "loc-14",
                    Name = "Danh thắng Tràng An - Ninh Bình",
                    Description = "Di sản thế giới kép đầu tiên tại Đông Nam Á với tour chèo thuyền xuyên qua các hang động đá vôi nước trong vắt.",
                    CategoryId = "cat-02",
                    Address = "Xã Trường Yên, Huyện Hoa Lư, Tỉnh Ninh Bình",
                    Latitude = 20.253639m,
                    Longitude = 105.908056m,
                    ImageUrl = "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-02",
                    CreateAt = new DateTime(2026, 1, 14, 8, 0, 0)
                },
                // 15. Đỉnh Fansipan - Sapa
                new Location
                {
                    LocationId = "loc-15",
                    Name = "Đỉnh Fansipan - Nóc nhà Đông Dương",
                    Description = "Đỉnh núi cao nhất 3.143m thuộc dãy Hoàng Liên Sơn, chinh phục bằng cáp treo hiện đại săn mây bồng bềnh.",
                    CategoryId = "cat-02",
                    Address = "Thị xã Sa Pa, Tỉnh Lào Cai",
                    Latitude = 22.303333m,
                    Longitude = 103.775000m,
                    ImageUrl = "https://images.unsplash.com/photo-1540555700478-4be289fbecef?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-02",
                    CreateAt = new DateTime(2026, 1, 15, 8, 0, 0)
                },
                // 16. Phố cổ Hội An (Quảng Nam)
                new Location
                {
                    LocationId = "loc-16",
                    Name = "Đô thị cổ Hội An",
                    Description = "Thương cảng cổ truyền thống Á Đông được bảo tồn nguyên vẹn với đèn lồng rực rỡ và những bức tường vàng đặc trưng.",
                    CategoryId = "cat-01",
                    Address = "TP. Hội An, Tỉnh Quảng Nam",
                    Latitude = 15.880058m,
                    Longitude = 108.338047m,
                    ImageUrl = "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-03",
                    CreateAt = new DateTime(2026, 1, 16, 8, 0, 0)
                },
                // 17. Cầu Vàng - Bà Nà Hills (Đà Nẵng)
                new Location
                {
                    LocationId = "loc-17",
                    Name = "Cầu Vàng (Golden Bridge) - Bà Nà Hills",
                    Description = "Cây cầu có thiết kế độc đáo hình bàn tay khổng lồ nâng đỡ dải lụa vàng giữa mây trời chốn tiên cảnh.",
                    CategoryId = "cat-06", // Vui chơi giải trí
                    Address = "Thôn An Sơn, Xã Hòa Ninh, Huyện Hòa Vàng, Đà Nẵng",
                    Latitude = 15.996167m,
                    Longitude = 107.986500m,
                    ImageUrl = "https://images.unsplash.com/photo-1570168007204-dfb528c6958f?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-03",
                    CreateAt = new DateTime(2026, 1, 17, 8, 0, 0)
                },
                // 18. Cố đô Huế
                new Location
                {
                    LocationId = "loc-18",
                    Name = "Quần thể Di tích Cố đô Huế",
                    Description = "Kinh đô lịch sử triều Nguyễn với Đại Nội cung điện, lăng tẩm các vị vua và vẻ đẹp thơ mộng bên dòng sông Hương.",
                    CategoryId = "cat-01",
                    Address = "Phường Thuận Thành, TP. Huế, Tỉnh Thừa Thiên Huế",
                    Latitude = 16.469722m,
                    Longitude = 107.577778m,
                    ImageUrl = "https://images.unsplash.com/photo-1569154941061-e231b4725ef1?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-01",
                    CreateAt = new DateTime(2026, 1, 18, 8, 0, 0)
                },
                // 19. Bãi biển Sao - Phú Quốc
                new Location
                {
                    LocationId = "loc-19",
                    Name = "Bãi Sao - Đảo Ngọc Phú Quốc",
                    Description = "Bãi biển đẹp bậc nhất Phú Quốc với bờ cát trắng mịn như kem, làn nước xanh ngọc bích và những hàng dừa nghiêng bóng.",
                    CategoryId = "cat-09", // Nghỉ dưỡng
                    Address = "Phường An Thới, TP. Phú Quốc, Tỉnh Kiên Giang",
                    Latitude = 10.052608m,
                    Longitude = 104.032608m,
                    ImageUrl = "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-02",
                    CreateAt = new DateTime(2026, 1, 19, 8, 0, 0)
                },
                // 20. Chợ nổi Cái Răng (Cần Thơ)
                new Location
                {
                    LocationId = "loc-20",
                    Name = "Chợ nổi Cái Răng",
                    Description = "Nét văn hóa sông nước miền Tây đặc sắc, nơi hàng trăm chiếc ghe xuồng tập trung buôn bán nông sản từ sáng sớm.",
                    CategoryId = "cat-03",
                    Address = "46 Đường Hai Bà Trưng, Phường Tân An, Quận Ninh Kiều, Cần Thơ",
                    Latitude = 10.005556m,
                    Longitude = 105.746111m,
                    ImageUrl = "https://images.unsplash.com/photo-1552465011-b4e21bf6e79a?auto=format&fit=crop&w=1200&q=80",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/854/854878.png",
                    CreateBy = "usr-03",
                    CreateAt = new DateTime(2026, 1, 20, 8, 0, 0)
                }
            };
        }
    }
}