using System;
using System.Collections.Generic;
using System.Text;
using TourisManager.Core.Entities;
using TourisManager.Services;

namespace TourisManager.Core.Interfaces
{
    public interface IFoodService
    {
        Task<List<Food>> GetListFoodAsync();
        Task<IEnumerable<Food>> FilterFoodsAsync(string typeID);
        Task<Food?> GetFoodByIdAsync(string id);
         Task<bool> InsertInvoidAsync( string accountId,CheckoutReques reques);
    }
}
