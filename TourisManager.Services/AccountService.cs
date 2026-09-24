using TourisManager.Core.Entities;

namespace TourisManager.Services
{
    public interface AccountService
    {
        bool Create(Account account);
        Account? Login(string usernameOrEmail, string password);
        Account FindByUsernameOrEmail(string usernameOrEmail); 
        bool Update(Account account);
        Account? FindById(string accountId);

        bool IsUserNameExists(string UserName);
        bool IsUserEmailExists(string Email);
    }
}