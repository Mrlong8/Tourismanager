using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.DataProtection; // Thêm thư viện này
using Microsoft.EntityFrameworkCore;
using TourisManager.Core.Interfaces;
using TourisManager.Core.Services;
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

            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("TourisManagerDb")));

            // FIX 1: Cấu hình Data Protection để lưu Key mã hóa Cookie cố định
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "keys")));

            // FIX 2: Xóa các phần đăng ký DI trùng lặp
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<IFoodService, FoodService>();
            builder.Services.AddScoped<ITypeOfFoodService, TypeOfFoodService>();
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ChatAutoReplyService>();
            builder.Services.AddScoped<IWalletService, WalletService>();

            builder.Services
                 .AddAuthentication(options =>
                 {
                     options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                     options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                     options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                 })
                 .AddCookie(options =>
                 {
                     options.LoginPath = "/account/login";
                     options.LogoutPath = "/account/logout";
                     options.AccessDeniedPath = "/Profile/Index";
                     options.ExpireTimeSpan = TimeSpan.FromDays(7);
                     options.SlidingExpiration = true;
                 })
                 .AddGoogle(options =>
                 {
                     options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
                     options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
                 });

            builder.Services.AddHttpClient("VietQRClient", (serviceProvider, client) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();

                client.DefaultRequestHeaders.Add("x-client-id", config["VietQR:ClientId"]);
                client.DefaultRequestHeaders.Add("x-api-key", config["VietQR:ApiKey"]);
            });

            var app = builder.Build();

            // FIX 3: Gộp chung thành 1 hàm khởi tạo Database (có bắt lỗi try-catch)
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

            // FIX 4: Đổi tên route tránh trùng lặp ("areas" -> "areas_admin", "areas_dashboard"...)
            app.MapControllerRoute(
                name: "areas_admin",
                pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "areas_dashboard",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // FIX 5: app.Run() luôn luôn phải nằm ở dưới cùng
            app.Run();
        }
    }
}