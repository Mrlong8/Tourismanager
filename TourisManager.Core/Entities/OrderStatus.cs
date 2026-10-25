using System;
using System.Collections.Generic;
using System.Text;

namespace TourisManager.Core.Entities
{
    public enum OrderStatus
    {
        Pending = 0,
        Shipping = 1,
        Completed = 2,
        Cancelled = 3,
    }
}
