using System;
using System.Collections.Generic;
using System.Text;
using TourisManager.Core.Entities;

namespace TourisManager.Services
{
    public interface LocationService
    {
        List<Location> GetDataAll(); 
    }
}
