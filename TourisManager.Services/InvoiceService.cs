using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;
using TourisManager.Core.Entities;
using TourisManager.Core.Services;
using TourisManager.Data;
using TourisManager.Services;

namespace TourisManager.Services
{
    public class InvoiceService : IInvoiceService
    {
        private AppDbContext _db;

        public InvoiceService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Order>> GetAllInvoiceAsync()
        {
            var invoiceList = await _db.Orders
                .Include(o => o.Account)
                    .ThenInclude(a => a.DetailAccount) 

                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                        .ThenInclude(f => f.TypeFood)

                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                        .ThenInclude(f => f.FoodSizes)
                            .ThenInclude(fs => fs.Size)
                .AsNoTracking()
                .ToListAsync();

            return invoiceList;
        }

        public async Task<IEnumerable<Order>> GetAllInvoiceBySellerAsync(string accoundId)
        {
            var invoiceList = await _db.Orders

             .Where(o => o.Location != null && o.Location.AccountId == accoundId)
             .Include(o => o.Account )
                 .ThenInclude(a => a.DetailAccount)

             .Include(o => o.OrderDetails)
                 .ThenInclude(od => od.Food)
                     .ThenInclude(f => f.TypeFood)

             .Include(o => o.OrderDetails)
                 .ThenInclude(od => od.Food)
                     .ThenInclude(f => f.FoodSizes)
                         .ThenInclude(fs => fs.Size)
             .AsNoTracking()
             .ToListAsync();

            return invoiceList;
        }

      

    }
}
