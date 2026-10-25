
using System.Collections.Generic;
using System.Threading.Tasks;
using TourisManager.Core.Entities;

namespace TourisManager.Core.Interfaces

{
    public interface IAdminService
    {
        Task<(int TotalUsers, int TotalLocations)> GetDashboardStatsAsync();

        Task<List<Account>> GetUsersAsync();
        Task<Dictionary<string, int>> GetUsersByRoleAsync();

        Task<Dictionary<string, int>> GetLocationsByAreaAsync();
    }
}
