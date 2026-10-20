using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TourisManager.Core.Entities;
using TourisManager.Core.Interfaces;
using TourisManager.Data;

namespace TourisManager.Services
{
    public class TypeOfFoodService : ITypeOfFoodService
    {
        private AppDbContext _db;

        public TypeOfFoodService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<TypeFood>> GetListTypeFoodsAsync()
        {
            return await _db.TypeFoods.AsNoTracking().ToListAsync();
        }

    }
}
