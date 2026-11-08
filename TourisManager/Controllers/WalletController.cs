using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TourisManager.Core.Entities;
using TourisManager.Core.Interfaces;
using TourisManager.Models.ViewModels;

namespace TourisManager.Controllers
{
    public class WalletController : Controller
    {
        private IWalletService _walletservice;
        private readonly IHttpClientFactory _httpClientFactory;

        public WalletController(IWalletService waletservice, IHttpClientFactory httpClientFactory)
        {
            _walletservice = waletservice;
            _httpClientFactory = httpClientFactory;
        }


        public async Task<IActionResult> Index()
        {
            var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(accountId))
            {
                return RedirectToAction("Login", "Account");
            }

            var wallet = await _walletservice.GetInformationWalletAsync(accountId);

            if (wallet == null)
            {
                wallet = new Wallet
                {
                    AccountId = accountId,
                    Balance = 0,
                    FrozenBalance = 0,
                    Currency = "VND",
                    WalletTransactions = new List<WalletTransaction>()
                };
            }

            // Lấy ngân hàng độc lập với ví: có dữ liệu thì điền sẵn, chưa có thì null
            ViewBag.BankAccount = await _walletservice.GetBankAccountAsync(accountId);

            return View(wallet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBankInfo(UpdateBankInfoVm vm)
        {
            var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(accountId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                // Hiển thị lỗi cụ thể thay vì thông báo chung chung
                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();

                TempData["ErrorMessage"] = firstError ?? "Thông tin ngân hàng không hợp lệ.";
                return RedirectToAction("Index");
            }

            // Map từ VM sang entity. AccountId lấy từ đăng nhập, KHÔNG lấy từ form.
            var bankAccount = new BankAccount
            {
                BankAccountId = vm.BankAccountId ?? string.Empty,
                AccountId = accountId,
                BankCode = vm.BankCode.Trim(),
                AccountNumber = vm.AccountNumber.Trim(),
                AccountHolderName = vm.AccountHolderName.Trim().ToUpperInvariant()
            };

            var result = await _walletservice.UpdateBankInfoAsync(bankAccount);

            if (result)
                TempData["SuccessMessage"] = "Thông tin ngân hàng đã được cập nhật thành công.";
            else
                TempData["ErrorMessage"] = "Không thể lưu thông tin ngân hàng.";

            return RedirectToAction("Index");
        }
    }
}
