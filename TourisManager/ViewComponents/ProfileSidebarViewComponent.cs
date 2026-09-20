using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using TourisManager.Core.Entities;
using TourisManager.Services;

namespace TourisManager.ViewComponents
{
    public class ProfileSidebarViewComponent : ViewComponent
    {
        private readonly AccountService _accountService;
        public ProfileSidebarViewComponent(AccountService accountService)
        {
            _accountService = accountService;
        }
        public IViewComponentResult Invoke()
        {
            string? userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return View("Default",(Account?)null);
            }

            var account = _accountService.FindById(userId);
            return View("Default",account);
        }
    }
}
