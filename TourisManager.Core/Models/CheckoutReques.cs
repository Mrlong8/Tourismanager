using System.ComponentModel.DataAnnotations;

namespace TourisManager.Services
{
    public class CheckoutReques
    {
       public string FoodId { get; set; } = string.Empty;
       public string SizeId { get; set; } = string.Empty;
       public int Quantity { get; set; }
       public decimal TotalPrice { get; set; }
       public int PaymentMethodID { get; set; }
    }
}
