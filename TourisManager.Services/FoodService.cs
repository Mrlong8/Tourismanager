using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourisManager.Core.Entities;
using TourisManager.Core.Interfaces;
using TourisManager.Data;
using TourisManager.Models;

namespace TourisManager.Services
{
    public class FoodService : IFoodService
    {
        private AppDbContext _db;
        public FoodService(AppDbContext db)
        {
            _db = db;
        }

        // Lấy danh sách toàn bộ món ăn
        public async Task<List<Food>> GetListFoodAsync()
        {
            return await _db.Foods
                .Include(f => f.TypeFood)
                .Include(f => f.FoodSizes)
                .Include(f => f.Location)
                .AsNoTracking()
                .ToListAsync();
        }

        // Lọc danh sách món ăn theo một mã phân loại
        public async Task<IEnumerable<Food>> FilterFoodsAsync(string typeId)
        {
            var query = _db.Foods
                .Include(f => f.TypeFood)
                .Include(f => f.FoodSizes)
                .Include(f => f.Location) // <--- BỔ SUNG DÒNG NÀY ĐỂ LẤY ĐÚNG TỌA ĐỘ QUÁN
                .AsQueryable();

            if (!string.IsNullOrEmpty(typeId))
                query = query.Where(f => typeId.Contains(f.TypeFoodId));

            return await query.ToListAsync();
        }

        // Lọc danh sách món ăn theo nhiều mã phân loại
        public async Task<IEnumerable<Food>> FilterFoodsByMultiTypeAsync(List<string> typeIds)
        {
            var query = _db.Foods
                .Include(f => f.TypeFood)
                .Include(f => f.FoodSizes)
                .Include(f => f.Location) // <--- BỔ SUNG DÒNG NÀY ĐỂ LẤY ĐÚNG TỌA ĐỘ QUÁN
                .AsQueryable();

            if (typeIds != null && typeIds.Any())
                query = query.Where(f => typeIds.Contains(f.TypeFoodId));

            return await query.ToListAsync();
        }

        // Lấy thông tin chi tiết của món ăn dựa trên mã món ăn
        public async Task<Food?> GetFoodByIdAsync(string id)
        {
            return await _db.Foods
                .Include(f => f.TypeFood)
                .Include(f => f.Location) // THÊM DÒNG NÀY ĐỂ LẤY TỌA ĐỘ QUÁN
                .Include(f => f.FoodSizes)
                    .ThenInclude(fs => fs.Size)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FoodId == id);
        }

        // Tạo hóa đơn mới, tính toán chi phí và lưu trữ vào cơ sở dữ liệu
        // Tạo hóa đơn mới, tính toán chi phí và lưu trữ vào cơ sở dữ liệu với trạng thái chưa thanh toán
        public async Task<Order?> InsertInvoidAsync(string accountId, CheckoutReques request)
        {
            var food = await _db.Foods.FirstOrDefaultAsync(f => f.FoodId == request.FoodId);
            if (food == null) return null;

            var newOrder = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                AccountId = accountId,
                LocationId = food.LocationId,
                TotalAmount = request.TotalPrice,
                DeliveryFee = request.DeliveryFee,
                PaymentMethodId = request.PaymentMethodID.ToString(),

                // SỬA TẠI ĐÂY: Mặc định đơn hàng mới tạo luôn là CHƯA THANH TOÁN (false)
                PaymentStatus = false,

                OrderStatus = 0,
                CreateAt = DateTime.Now,
                OrderDetails = new List<OrderDetail>
        {
            new OrderDetail
            {
                FoodId = request.FoodId,
                Quantity = request.Quantity,
                TotalPrice = request.TotalPrice - (decimal)(request.DeliveryFee ?? 0)
            }
        }
            };
            try
            {
                await _db.Orders.AddAsync(newOrder);
                await _db.SaveChangesAsync();
                return newOrder;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Xác nhận thanh toán QR, khởi tạo ví nếu chưa có và cộng tiền vào ví người bán
        // Xác nhận thanh toán QR, khởi tạo ví nếu chưa có và cộng tiền vào ví người bán
        public async Task<bool> ProcessQRPaymentSuccessAsync(string orderId)
        {
            var order = await _db.Orders.Include(o => o.Location).FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null) return false;

            // Luôn cập nhật trạng thái đơn hàng thành đã thanh toán trước tiên
            order.PaymentStatus = true;

            var sellerAccountId = order.Location?.AccountId;
            if (string.IsNullOrEmpty(sellerAccountId))
            {
                var fallbackSeller = await _db.Accounts.FirstOrDefaultAsync(a => a.Role == "Seller" || a.Role == "Admin");
                sellerAccountId = fallbackSeller?.AccountId;
            }

            if (!string.IsNullOrEmpty(sellerAccountId))
            {
                var sellerWallet = await _db.Wallets.FirstOrDefaultAsync(w => w.AccountId == sellerAccountId);

                if (sellerWallet == null)
                {
                    sellerWallet = new Wallet
                    {
                        WalletId = Guid.NewGuid().ToString(),
                        AccountId = sellerAccountId,
                        Balance = 0,
                        FrozenBalance = 0,
                        Currency = "VND",
                        UpdatedAt = DateTime.Now
                    };
                    _db.Wallets.Add(sellerWallet);
                    await _db.SaveChangesAsync();
                }

                decimal platformFee = 0;
                decimal amountToSeller = (decimal)(order.TotalAmount - platformFee);

                sellerWallet.Balance += amountToSeller;
                sellerWallet.UpdatedAt = DateTime.Now;

                var transaction = new WalletTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    WalletId = sellerWallet.WalletId,
                    OrderId = orderId,
                    Amount = amountToSeller,
                    Type = "NHAN_TIEN_DON_HANG",
                    Status = "THANH_CONG",
                    Description = $"Nhận thanh toán QR cho đơn hàng #{orderId}",
                    CreatedAt = DateTime.Now
                };

                _db.WalletTransactions.Add(transaction);
            }

            // Đưa lệnh lưu ra ngoài để đảm bảo Order luôn được cập nhật PaymentStatus = true xuống DB
            await _db.SaveChangesAsync();

            return true;
        }

        // Truy vấn cơ sở dữ liệu bảng Order để kiểm tra PaymentStatus của đơn hàng
        public async Task<bool> IsOrderPaidAsync(string orderId)
        {
            var orders = await _db.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            return orders != null && orders.PaymentStatus == true;
        }
        // Tìm kiếm và trả về đơn hàng đầu tiên có mã bắt đầu bằng chuỗi được cung cấp
        // Tìm kiếm và trả về đơn hàng đầu tiên có mã bắt đầu bằng chuỗi được cung cấp (không phân biệt hoa/thường)
        public async Task<Order?> GetOrderStartsWithAsync(string shortOrderId)
        {
            string lowerId = shortOrderId.ToLower();

            return await _db.Orders
                .FirstOrDefaultAsync(o => o.OrderId.ToLower().StartsWith(lowerId));
        }


        // Thêm vào FoodService.cs
        public async Task<Order?> CreateOrderFromCartAsync(string accountId, CartCheckoutRequest request)
        {
            // 1. Khởi tạo Đơn hàng chính (Order)
            var newOrder = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                AccountId = accountId,
                LocationId = request.LocationId,
                TotalAmount = request.TotalAmount + request.DeliveryFee,
                DeliveryFee = request.DeliveryFee,
                PaymentMethodId = request.PaymentMethodID.ToString(),
                PaymentStatus = false, // Luôn false khi mới tạo
                OrderStatus = 0, // Trạng thái chờ xác nhận
                CreateAt = DateTime.Now,
                OrderDetails = new List<OrderDetail>()
            };

            // 2. Thêm từng món ăn vào danh sách Chi tiết đơn hàng (OrderDetail)
            foreach (var item in request.Items)
            {
                newOrder.OrderDetails.Add(new OrderDetail
                {
                    FoodId = item.FoodId,
                    // Sơ đồ ERD không ghi rõ SizeId trong OrderDetail, nhưng nếu bạn có bổ sung thì gán vào đây
                    // SizeId = item.SizeId, 
                    Quantity = item.Quantity,
                    TotalPrice = item.UnitPrice * item.Quantity
                });
            }

            try
            {
                // Entity Framework Core sẽ tự động insert cả Order và các OrderDetail liên quan
                await _db.Orders.AddAsync(newOrder);
                await _db.SaveChangesAsync();
                return newOrder;
            }
            catch (Exception ex)
            {
                // Có thể log lỗi ex.Message ở đây
                return null;
            }
        }
    }
    
}
