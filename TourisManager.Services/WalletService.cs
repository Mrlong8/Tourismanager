using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TourisManager.Core.Entities;
using TourisManager.Core.Interfaces;
using TourisManager.Data;

namespace TourisManager.Services
{
    public class WalletService : IWalletService
    {

        private AppDbContext _db;
        public WalletService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Wallet> GetInformationWalletAsync(string accountId)
        {
            var walletInfo   = await _db.Wallets
                .Include(w => w.WalletTransactions)
                .Include(w => w.Account)
                    .ThenInclude(a => a.BankAccounts)
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.AccountId == accountId);

            return walletInfo;
        }
        public async Task<bool> UpdateBankInfoAsync(BankAccount bankAccount)
        {
            var result = await _db.BankAccounts
                .FirstOrDefaultAsync(b =>
                    b.BankAccountId == bankAccount.BankAccountId &&
                    b.AccountId == bankAccount.AccountId);

            if (result == null)
            {
                // Tạo mã tự động
                bankAccount.BankAccountId = Guid.NewGuid().ToString();

                bankAccount.IsDefault = true;

                _db.BankAccounts.Add(bankAccount);
            }
            else
            {
                result.BankCode = bankAccount.BankCode;
                result.AccountNumber = bankAccount.AccountNumber;
                result.AccountHolderName = bankAccount.AccountHolderName;
            }

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<BankAccount?> GetBankAccountAsync(string accountId)
        {
            return await _db.BankAccounts
                .AsNoTracking()
                .Where(b => b.AccountId == accountId)
                .OrderByDescending(b => b.IsDefault)
                .FirstOrDefaultAsync();
        }
    }
}
