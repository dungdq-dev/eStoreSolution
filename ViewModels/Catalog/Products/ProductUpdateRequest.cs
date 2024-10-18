using Microsoft.AspNetCore.Http;

namespace ViewModels.Catalog.Products
{
    public class ProductUpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string SeoDescription { get; set; }
        public string SeoTitle { get; set; }

        public string LanguageId { get; set; }

        public decimal Price { get; set; }

        public bool? IsFeatured { get; set; }

        public IFormFile? ThumbnailImage { get; set; }
    }
}