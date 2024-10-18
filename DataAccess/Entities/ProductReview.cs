namespace DataAccess.Entities
{
    public class ProductReview
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public int ProductId { get; set; }

        public AppUser AppUser { get; set; }
        public Product Product { get; set; }
    }
}