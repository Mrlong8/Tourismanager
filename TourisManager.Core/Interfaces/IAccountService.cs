using TourisManager.Core.Entities;

namespace TourisManager.Services
{
    public interface IAccountService
    {
        Task<bool> CreateAsync(Account account);
        Task<Account?> LoginAsync(string usernameOrEmail, string password);
        Task<Account?> FindByUsernameOrEmailAsync(string usernameOrEmail);
        Task<bool> UpdateAsync(Account account);
        Task<Account?> FindByIdAsync(string accountId);

        Task<bool> IsUserNameExistsAsync(string userName);
        Task<bool> IsUserEmailExistsAsync(string email);
        Task<(bool Success, string Message)> ChangePasswordAsync(string accountID, string newPassword, string CurentPassword);
    }
}