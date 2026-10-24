
using System.Collections.Generic;
using System.Threading.Tasks;
using TourisManager.Core.Entities;

namespace TourisManager.Core.Interfaces

{
    public interface IUserService
    {
        Task<(List<Account> Users, int TotalCount)> GetUsersAsync(
            string? searchString,
            int pageNumber,
            int pageSize);

        Task<Account?> GetByIdAsync(string id);

        Task<bool> CreateAsync(Account user);

        Task<bool> UpdateAsync(string id, Account user);

        Task<bool> DeleteAsync(string id);
    }
}
