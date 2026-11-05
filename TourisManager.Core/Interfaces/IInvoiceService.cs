using System;
using System.Collections.Generic;
using System.Text;
using TourisManager.Core.Entities;

namespace TourisManager.Core.Services
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Order>> GetAllInvoiceAsync();
        Task<IEnumerable<Order>> GetAllInvoiceByIDAsync(string accountId);
        Task<IEnumerable<Order>> GetActiveOrdersByCustomerAsync(string accountId);


    }
}
