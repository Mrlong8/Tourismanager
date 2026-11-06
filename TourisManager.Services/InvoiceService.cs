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

        public async Task<IEnumerable<Order>> GetAllInvoiceByIDAsync(string accoundId)
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

        // Tải danh sách đơn hàng đang mua của Người dùng
        public async Task<IEnumerable<Order>> GetActiveOrdersByCustomerAsync(string accountId)
        {
            var activeOrders = await _db.Orders
                .Where(o => o.AccountId == accountId)
                .Include(o => o.Account)
                    .ThenInclude(a => a.DetailAccount)
                .Include(o => o.Location)
                .Include(o => o.PaymentMethod)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                        .ThenInclude(f => f.TypeFood)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                        .ThenInclude(f => f.FoodSizes)
                            .ThenInclude(fs => fs.Size)
                .OrderByDescending(o => o.CreateAt)
                .AsNoTracking()
                .ToListAsync();

            return activeOrders;
        }

        public async Task<bool> CancelOrderAsync(string orderId, string accountId)
        {
            var orrder = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId && o.AccountId == accountId);
            if (orrder == null || orrder.OrderStatus == OrderStatus.Pending )
            {
                return false;
            }

            orrder.OrderStatus = OrderStatus.Cancelled;
            await _db.SaveChangesAsync();
            return true;
        }


    }
}
