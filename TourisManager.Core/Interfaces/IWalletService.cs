using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TourisManager.Core.Entities;

namespace TourisManager.Core.Interfaces
{
    public interface IWalletService 
    {

        Task<Wallet> GetInformationWalletAsync(string accountId);
        Task<bool> UpdateBankInfoAsync(BankAccount bankAccount);
        Task<BankAccount?> GetBankAccountAsync(string accountId);
    }
}
