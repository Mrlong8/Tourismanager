using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TourisManager.Core.Entities;
using TourisManager.Core.Interfaces;
using TourisManager.Data;

namespace TourisManager.Services
{
    public class FoodService : IFoodService
    {
        private AppDbContext _db;
        public FoodService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<Food>> GetListFoodAsync()
        {
            return await  _db.Foods.
                            Include(f => f.TypeFood).
                            Include(f => f.FoodSizes).
                            AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Food>> FilterFoodsAsync(string typeId)
        {
            var query = _db.Foods
                .Include(f => f.TypeFood)
                .Include(f => f.FoodSizes)
                .AsQueryable();

            if (!string.IsNullOrEmpty(typeId))
                query = query.Where(f => f.TypeFoodId == typeId);

            return await query.ToListAsync();
        }
    }
}
