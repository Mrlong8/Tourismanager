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
            builder.Services.AddScoped<AccountService, AccountServiceImpl>();
            builder.Services.AddScoped<LocationService, LocationServiceImpl>();

            // CẤU HÌNH AUTHENTICATION (Gộp chung Cookie + Google vào 1 chuỗi liên tục)

            builder.Services
                 .AddAuthentication(options =>
                 {
                     options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                     options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                 })
                 .AddCookie(options =>
                 {
                     options.LoginPath = "/account/login";
                     options.LogoutPath = "/account/logout";
                     options.AccessDeniedPath = "/account/login";
                     options.ExpireTimeSpan = TimeSpan.FromDays(7);
                     options.SlidingExpiration = true;
                 })
                 .AddGoogle(options =>
                 {
                     options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
                     options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;

                     //options.CallbackPath = "/signin-google";
                 });

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

            //------------------

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // hoặc app.MapStaticAssets();

            app.UseRouting();

            app.UseSession();

            // DEBUG GOOGLE LOGIN
            //app.Use(async (context, next) =>
            //{
            //    if (context.Request.Path.StartsWithSegments("/signin-google"))
            //    {
            //        Console.WriteLine("========== GOOGLE CALLBACK ==========");
            //        Console.WriteLine($"URL: {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");

            //        Console.WriteLine("Cookies received by ASP.NET:");

            //        foreach (var cookie in context.Request.Cookies)
            //        {
            //            Console.WriteLine($"  {cookie.Key} = {cookie.Value}");
            //        }

            //        Console.WriteLine("=====================================");
            //    }

            //    await next();
            //});

            app.UseAuthentication();
            app.UseAuthorization();

            //------------------

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