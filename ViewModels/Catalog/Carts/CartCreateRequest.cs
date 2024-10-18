namespace ViewModels.Catalog.Carts
{
    public class CartCreateRequest
    {
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public int ProductId { get; set; }

        public Guid UserId { get; set; }
    }
}