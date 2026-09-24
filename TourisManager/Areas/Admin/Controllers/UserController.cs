using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Data;
using TourisManager.Core.Entities;

namespace TourisManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/User
        public async Task<IActionResult> Index()
        {
            var users = await _context.Accounts.ToListAsync();

            return View(users);
        }

        // GET: /Admin/User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account user)
        {
            // Tạo AccountId trước
            user.AccountId = Guid.NewGuid().ToString();

            // Bỏ lỗi validation của AccountId
            ModelState.Remove(nameof(Account.AccountId));

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(user.Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                }
                _context.Accounts.Add(user);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // GET: /Admin/User/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Accounts.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: /Admin/User/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Account user)
        {
            if (id != user.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Accounts.Update(user);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // GET: /Admin/User/Delete/id
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Accounts
                .FirstOrDefaultAsync(x => x.AccountId == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: /Admin/User/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Accounts.FindAsync(id);

            if (user != null)
            {
                _context.Accounts.Remove(user);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}