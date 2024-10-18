using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ViewModels.Catalog.Products
{
    public class ProductCreateRequest
    {
        [Required(ErrorMessage = "Bạn phải nhập tên sản phẩm")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Details { get; set; }

        public string SeoDescription { get; set; }

        public string SeoTitle { get; set; }

        public decimal Price { get; set; }

        public decimal OriginalPrice { get; set; }

        public int Stock { get; set; }

        public string LanguageId { get; set; }

        public bool? IsFeatured { get; set; }

        public IFormFile ThumbnailImage { get; set; }
    }
}