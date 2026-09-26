using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
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

            // AppDbContext to Dependency Injection
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("TourisManagerDb")));

            // Đăng ký dịch vụ (Dependency Injection - DI)
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ILocationService, LocationService>();

            // CẤU HÌNH AUTHENTICATION
            builder.Services
                 .AddAuthentication(options =>
                 {
                     options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                     options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                     // 1. Đổi dòng này từ GoogleDefaults thành Cookie
                     options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                     // 2. Thêm dòng này để xử lý cấm truy cập (Forbid)
                     options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                 })
                 .AddCookie(options =>
                 {
                     options.LoginPath = "/account/login";
                     options.LogoutPath = "/account/logout";
                     options.AccessDeniedPath = "/Profile/Index";
                     options.ExpireTimeSpan = TimeSpan.FromDays(7);
                     options.SlidingExpiration = true;
                 });
                 //.AddGoogle(options =>
                 //{
                 //    options.ClientId = builder.Configuration["GoogleKeys:ClientId"] ?? "";
                 //    options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"] ?? "";
                 //});

            var app = builder.Build();

            // Initialize the database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"); 

            app.Run();
        }
    }
}