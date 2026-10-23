
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourisManager.Core.Entities;

namespace TourisManager.Core.Interfaces
{
    public interface ILocationService
    {
        // Lấy toàn bộ địa điểm cho bản đồ
        List<Location> GetDataAll();

        // Tìm kiếm và lọc địa điểm
        IQueryable<Location> GetLocationsQuery(
            string? searchString,
            string? categoryId);

        // Lấy danh sách danh mục
        Task<List<Category>> GetCategoriesAsync();

        // Tìm địa điểm theo ID
        Task<Location?> GetByIdAsync(string id);

        // Lấy ID tài khoản
        Task<string?> GetFirstAccountIdAsync();

        // Thêm địa điểm
        Task CreateAsync(Location location);

        // Cập nhật địa điểm
        Task<bool> UpdateAsync(string id, Location location);

        // Xóa địa điểm
        Task<bool> DeleteAsync(string id);
    }
}
