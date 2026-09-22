using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using TourisManager.Data;
using TourisManager.Data.Seed;
using TourisManager.Services;


namespace TourisManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();

            //AppDbContext to Dependency Injection (xây dựng csdl)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("TourisManagerDb")));

            // đăng ký dịch vụ (Dependency Injection - DI)
            builder.Services.AddScoped<AccountService, AccountServiceImpl>();
            builder.Services.AddScoped<LocationService, LocationServiceImpl>();
            // Cấu hình ASP.NET Core Cookie Authentication
            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/account/login";// chưa đăng nhập chuyển về trang login
                    options.LogoutPath = "/account/logout";// đăng xuất
                    options.AccessDeniedPath = "/account/access-denied"; // đăng nhập nhưng không đủ quyền
                    options.ExpireTimeSpan = TimeSpan.FromDays(7); // thời gian lưu cookie
                    options.SlidingExpiration = true;//Nếu user tiếp tục sử dụng hệ thống, thời hạn cookie có thể được gia hạn theo cơ chế sliding expiration.
                });


            var app = builder.Build();

            app.UseSession(); // Session dùng để lưu dữ liệu tạm thời giữa các request.
                              // Ví dụ: giỏ hàng, trạng thái tạm thời,...

            //Initialize the database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Lấy DbContext từ hệ thống DI
                    var context = services.GetRequiredService<AppDbContext>();

                    // Gọi hàm nạp dữ liệu mẫu
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Đã xảy ra lỗi trong quá trình khởi tạo dữ liệu mẫu.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();//Đọc authentication information từ request và xác định "User này là ai?"
            app.UseAuthorization();// thực hiện phân quyền

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

     
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
