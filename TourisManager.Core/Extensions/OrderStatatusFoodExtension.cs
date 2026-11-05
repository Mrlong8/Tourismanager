using TourisManager.Core.Entities;

namespace TourisManager.Core.Extensions
{
    public static class OrderStatusFoodExtension
    {
        public static string ToDisplayName(this OrderStatus status) => status switch
        {
            OrderStatus.Pending => "Chờ xác nhận",
            OrderStatus.Shipping => "Đang giao hàng",
            OrderStatus.Completed => "Hoàn thành",
            OrderStatus.Cancelled => "Đã hủy",
            _ => status.ToString()
        };

        public static string ToBadgeClass(this OrderStatus status) => status switch
        {
            OrderStatus.Pending => "badge-orange-light",
            OrderStatus.Shipping => "bg-info text-white",
            OrderStatus.Completed => "bg-success text-white",
            OrderStatus.Cancelled => "bg-danger text-white",
            _ => "bg-secondary text-white"
        };
    }
}