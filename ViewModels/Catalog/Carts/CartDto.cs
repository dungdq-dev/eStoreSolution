namespace ViewModels.Catalog.Carts
{
    public class CartDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Guid UserId { get; set; }
    }
}