using Microsoft.EntityFrameworkCore;
using TourisManager.Models.Entity;

namespace TourisManager.Models.Data
{
    public class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MyDbContext(serviceProvider.GetRequiredService<DbContextOptions<MyDbContext>>()))
            {
                // Tự động tạo DB và bảng nếu chưa tồn tại
                //context.Database.EnsureCreated();

                if (!context.Destinations.Any())
                {
                    var destinations = new Destination[]
                    {
                    new Destination
                    {
                        Name = "Hồ Hoàn Kiếm",
                        Description = "Trái tim của Hà Nội, nơi gắn liền với truyền thuyết Rùa Thần và Tháp Rùa cổ kính.",
                        Address = "Phường Hàng Trống, Quận Hoàn Kiếm, Hà Nội",
                        Latitude = 21.028511m,
                        Longitude = 105.854444m,
                        ImageUrl = "https://example.com/images/ho-hoan-kiem.jpg",
                        CustomIconUrl = "https://example.com/icons/lake.png",
                        CreatedAt = DateTime.Now
                    },
                    new Destination
                    {
                        Name = "Lăng Chủ tịch Hồ Chí Minh",
                        Description = "Nơi an nghỉ cuối cùng của Bác Hồ, vị lãnh tụ kính yêu của dân tộc Việt Nam.",
                        Address = "2 Hùng Vương, Phường Điện Biên, Quận Ba Đình, Hà Nội",
                        Latitude = 21.036783m,
                        Longitude = 105.834710m,
                        ImageUrl = "https://example.com/images/lang-bac.jpg",
                        CustomIconUrl = "https://example.com/icons/monument.png",
                        CreatedAt = DateTime.Now
                    },
                    new Destination
                    {
                        Name = "Văn Miếu - Quốc Tử Giám",
                        Description = "Trường đại học đầu tiên của Việt Nam, nơi thờ Khổng Tử và lưu giữ 82 bia Tiến sĩ.",
                        Address = "58 Quốc Tử Giám, Phường Văn Miếu, Quận Đống Đa, Hà Nội",
                        Latitude = 21.029344m,
                        Longitude = 105.836173m,
                        ImageUrl = "https://example.com/images/van-mieu.jpg",
                        CustomIconUrl = "https://example.com/icons/temple.png",
                        CreatedAt = DateTime.Now
                    },
                    new Destination
                    {
                        Name = "Chùa Trấn Quốc",
                        Description = "Ngôi chùa cổ nhất Hà Nội với lịch sử hơn 1500 năm, nằm trên đảo nhỏ ở Hồ Tây.",
                        Address = "Đường Thanh Niên, Phường Yên Phụ, Quận Tây Hồ, Hà Nội",
                        Latitude = 21.047805m,
                        Longitude = 105.836855m,
                        ImageUrl = "https://example.com/images/chua-tran-quoc.jpg",
                        CustomIconUrl = "https://example.com/icons/pagoda.png",
                        CreatedAt = DateTime.Now
                    },
                    new Destination
                    {
                        Name = "Hoàng thành Thăng Long",
                        Description = "Di sản văn hóa thế giới UNESCO, di tích lịch sử gắn liền với nhiều thời đại phong kiến.",
                        Address = "19 Hoàng Diệu, Phường Quán Thánh, Quận Ba Đình, Hà Nội",
                        Latitude = 21.035174m,
                        Longitude = 105.839818m,
                        ImageUrl = "https://example.com/images/hoang-thanh.jpg",
                        CustomIconUrl = "https://example.com/icons/castle.png",
                        CreatedAt = DateTime.Now
                    },
                    new Destination
                    {
                        Name = "Nhà thờ Lớn Hà Nội",
                        Description = "Công trình kiến trúc Gothic đặc trưng thời Pháp thuộc, điểm hẹn cafe quen thuộc.",
                        Address = "40 Nhà Chung, Phường Hàng Trống, Quận Hoàn Kiếm, Hà Nội",
                        Latitude = 21.028775m,
                        Longitude = 105.848981m,
                        ImageUrl = "https://example.com/images/nha-tho-lon.jpg",
                        CustomIconUrl = "https://example.com/icons/church.png",
                        CreatedAt = DateTime.Now
                    },
                    new Destination
                    {
                        Name = "Chùa Một Cột",
                        Description = "Kiến trúc độc đáo mô phỏng bông hoa sen nở trên mặt nước, biểu tượng văn hóa Hà Nội.",
                        Address = "Phường Đội Cấn, Quận Ba Đình, Hà Nội",
                        Latitude = 21.035848m,
                        Longitude = 105.833618m,
                        ImageUrl = "https://example.com/images/chua-mot-cot.jpg",
                        CustomIconUrl = "https://example.com/icons/pagoda.png",
                        CreatedAt = DateTime.Now
                    },
                    new Destination
                    {
                        Name = "Di tích Nhà tù Hỏa Lò",
                        Description = "Nơi giam giữ hàng ngàn chiến sĩ cách mạng Việt Nam, biểu tượng cho lòng kiên trung.",
                        Address = "1 Phố Hỏa Lò, Phường Trần Hưng Đạo, Quận Hoàn Kiếm, Hà Nội",
                        Latitude = 21.025281m,
                        Longitude = 105.846522m,
                        ImageUrl = "https://example.com/images/hoa-lo.jpg",
                        CustomIconUrl = "https://example.com/icons/history.png",
                        CreatedAt = DateTime.Now
                    },
                    new Destination
                    {
                        Name = "Chợ Đồng Xuân",
                        Description = "Khu chợ bán buôn lớn và lâu đời nhất Hà Nội, nằm ngay trong khu Phố Cổ.",
                        Address = "Phố Đồng Xuân, Phường Đồng Xuân, Quận Hoàn Kiếm, Hà Nội",
                        Latitude = 21.038162m,
                        Longitude = 105.849764m,
                        ImageUrl = "https://example.com/images/cho-dong-xuan.jpg",
                        CustomIconUrl = "https://example.com/icons/market.png",
                        CreatedAt = DateTime.Now
                    },
                    new Destination
                    {
                        Name = "Cầu Long Biên",
                        Description = "Cây cầu thép đầu tiên bắc qua sông Hồng, chứng nhân lịch sử qua hai cuộc chiến tranh.",
                        Address = "Cầu Long Biên, Phường Phúc Xá, Quận Ba Đình, Hà Nội",
                        Latitude = 21.042333m,
                        Longitude = 105.856944m,
                        ImageUrl = "https://example.com/images/cau-long-bien.jpg",
                        CustomIconUrl = "https://example.com/icons/bridge.png",
                        CreatedAt = DateTime.Now
                    }
                    };
                    context.Destinations.AddRange(destinations);
                    context.SaveChanges();
                }

                if (!context.Users.Any())
                {
                    var users = new User[] {
                    new User
                    {
                        UserId = "001",
                        Username = "admin",
                        Email = "admin@.com",
                        Password = "123",
                        Role = "admin"
                    },
                     new User
                    {
                        UserId = "002",
                        Username = "user1",
                        Email = "user1n@.com",
                        Password = "123",
                        Role = "user"
                    },
                      new User
                    {
                        UserId = "003",
                        Username = "user2",
                        Email = "user2@.com",
                        Password = "123",
                        Role = "user"
                    }
                };

                    context.Users.AddRange(users);
                    context.SaveChanges();
                }
            

            }
        }
    }
}