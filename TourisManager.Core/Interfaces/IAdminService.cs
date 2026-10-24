
using System.Collections.Generic;
using System.Threading.Tasks;
using TourisManager.Core.Entities;

namespace TourisManager.Core.Interfaces

{
    public interface IAdminService
    {
        Task<(int TotalUsers, int TotalLocations)> GetDashboardStatsAsync();

        Task<List<Account>> GetUsersAsync();
    }
}
