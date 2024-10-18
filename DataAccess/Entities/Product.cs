namespace DataAccess.Entities
{
    public class Product
    {
        public int Id { set; get; }
        public decimal Price { set; get; }
        public decimal OriginalPrice { set; get; }
        public int Stock { set; get; }
        public int ViewCount { set; get; }
        public DateTime DateCreated { set; get; }

        public bool? IsFeatured { get; set; }

        public ICollection<ProductCategory> ProductCategories { get; set; }

        public ICollection<ProductReview> ProductReviews { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }

        public ICollection<Cart> Carts { get; set; }

        public ICollection<ProductTranslation> ProductTranslations { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; }
    }
}