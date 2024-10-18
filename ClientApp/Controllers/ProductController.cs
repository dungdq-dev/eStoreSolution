using ApiIntegration;
using ClientApp.Models;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Catalog.Products;

namespace ClientApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductApiClient _productApiClient;
        private readonly ICategoryApiClient _categoryApiClient;

        public ProductController(IProductApiClient productApiClient, ICategoryApiClient categoryApiClient)
        {
            _productApiClient = productApiClient;
            _categoryApiClient = categoryApiClient;
        }

        public async Task<IActionResult> Detail(int id, string culture)
        {
            var product = await _productApiClient.GetById(id, culture);
            return View(new ProductDetailViewModel()
            {
                Product = product.Data
            });
        }

        public async Task<IActionResult> Category(int id, string culture, int page = 1)
        {
            var products = await _productApiClient.GetAll(new GetProductPagingRequest()
            {
                CategoryId = id,
                PageIndex = page,
                LanguageId = culture,
                PageSize = 10
            });

            var category = await _categoryApiClient.GetById(id, culture);
            return View(new ProductCategoryViewModel()
            {
                Category = category.Data,
                Products = products
            }); ;
        }
    }
}