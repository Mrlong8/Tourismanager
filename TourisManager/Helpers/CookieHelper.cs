

using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TourisManager.Helpers
{
    public static class CookieHelper
    {

        public static void Create(HttpContext context,string key, string value,DateTime expire)
        {
            var options = new CookieOptions
            {
                Expires = expire,
                HttpOnly = true, // Tăng cường bảo mật, chống script JS can thiệp
                IsEssential = true, // Đảm bảo cookie hoạt động kể cả khi bật chính sách GDPR
                Secure = true // Chỉ gửi qua HTTPS (Khuyên dùng)
            };
            context.Response.Cookies.Append(key,value,options);

        }

        public static string? Get(HttpContext context,string key)
        {
            return context.Request.Cookies[key];
        }

        public static void Delete(HttpContext context, string key)
        {
            context.Response.Cookies.Delete(key);
        }


    }
}
