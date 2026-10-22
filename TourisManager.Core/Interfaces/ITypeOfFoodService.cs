using System;
using System.Collections.Generic;
using System.Text;
using TourisManager.Core.Entities;

namespace TourisManager.Core.Interfaces
{
    public interface ITypeOfFoodService
    {
        Task<List<TypeFood>> GetListTypeFoodsAsync();
    }
}
