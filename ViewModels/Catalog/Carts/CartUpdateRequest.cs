namespace ViewModels.Catalog.Carts
{
    public class CartUpdateRequest
    {
        public int Id { get; set; }
        public int Quantity { set; get; }
        public decimal Price { set; get; }

        public int ProductId { set; get; }

        public Guid UserId { get; set; }
    }
}