using ViewModels.Catalog.Categories;
using ViewModels.Catalog.ProductImages;
using ViewModels.Catalog.Products;

namespace ClientApp.Models
{
    public class ProductDetailViewModel
    {
        public CategoryDto Category { get; set; }

        public ProductDto Product { get; set; }

        public List<ProductDto> RelatedProducts { get; set; }

        public List<ProductImageDto> ProductImages { get; set; }
    }
}