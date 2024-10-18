using ViewModels.Catalog.Products;
using ViewModels.Utilities.Slides;

namespace ClientApp.Models
{
    public class HomeViewModel
    {
        public List<SlideDto> Slides { get; set; }

        public List<ProductDto> FeaturedProducts { get; set; }

        public List<ProductDto> LatestProducts { get; set; }
    }
}