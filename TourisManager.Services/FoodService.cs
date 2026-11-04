using Azure.Core;
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
            return await _db.Foods
                .Include(f => f.TypeFood)
                .Include(f => f.FoodSizes)
                .AsNoTracking()
                .ToListAsync();
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

        public async Task<Food?> GetFoodByIdAsync(string id)
        {
            return await _db.Foods
                .Include(f => f.TypeFood)
                .Include(f => f.FoodSizes)
                    .ThenInclude(fs => fs.Size)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FoodId == id);
        }
        public async Task<bool> InsertInvoidAsync(string accountId, CheckoutReques request)
        {
            var food = await _db.Foods.FirstOrDefaultAsync(f => f.FoodId == request.FoodId);
            if (food == null) return false;

            var newOrder = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                AccountId = accountId,
                LocationId = food.LocationId,
                TotalAmount = request.TotalPrice,

               
                PaymentMethodId = request.PaymentMethodID.ToString(), 

                PaymentStatus = false,
                OrderStatus = 0,       
                CreateAt = DateTime.Now,

                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        FoodId = request.FoodId,
                        Quantity = request.Quantity,
                        TotalPrice = request.TotalPrice
                    }
                }
            };
            try
            {
                await _db.Orders.AddAsync(newOrder);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
        

