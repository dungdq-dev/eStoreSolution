using ViewModels.Catalog.Categories;
using ViewModels.Catalog.Products;
using ViewModels.Common;

namespace ClientApp.Models
{
    public class ProductCategoryViewModel
    {
        public CategoryDto Category { get; set; }

        public PagedResponse<ProductDto> Products { get; set; }
    }
}