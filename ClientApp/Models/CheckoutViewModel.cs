using ViewModels.Sales.Orders;

namespace ClientApp.Models
{
    public class CheckoutViewModel
    {
        public List<CartItemViewModel> CartItems { get; set; }

        public OrderCreateRequest CheckoutModel { get; set; }
    }
}