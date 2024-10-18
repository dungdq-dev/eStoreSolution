using BusinessLogic.Catalog.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Catalog.ProductImages;
using ViewModels.Catalog.Products;

namespace WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productId = await _productService.Create(request);
            if (productId == 0)
                return BadRequest();

            var product = await _productService.GetById(productId, request.LanguageId);

            return CreatedAtAction(nameof(GetById), 
                new { id = productId, languageId = request.LanguageId }, 
                product);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductPagingRequest request)
        {
            if (request.PageIndex == 0) request.PageIndex = 1;
            if (request.PageSize == 0) request.PageSize = int.MaxValue;

            var products = await _productService.GetAll(request);
            return Ok(products);
        }

        [HttpGet("{id}/{languageId}")]
        [ActionName(nameof(GetById))]
        public async Task<IActionResult> GetById([FromRoute] int id, string languageId)
        {
            var result = await _productService.GetById(id, languageId);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("featured/{take}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeaturedProducts([FromRoute] int take, [FromQuery] string languageId)
        {
            var products = await _productService.GetFeaturedProducts(languageId, take);
            return Ok(products);
        }

        [HttpGet("latest/{take}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLatestProducts([FromRoute] int take, [FromQuery] string languageId)
        {
            var products = await _productService.GetLatestProducts(languageId, take);
            return Ok(products);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] ProductUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = id;
            var affectedResult = await _productService.Update(request);
            if (affectedResult == 0)
                return BadRequest();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var affectedResult = await _productService.Delete(id);
            if (affectedResult == 0)
                return BadRequest();

            return Ok();
        }

        [HttpPut("{id}/category-assign")]
        [Authorize]
        public async Task<IActionResult> CategoryAssign([FromRoute] int id, [FromBody] CategoryAssignRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.CategoryAssign(id, request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPatch("stock/{productId}/{amount}")]
        [Authorize]
        public async Task<IActionResult> UpdateStock(int productId, int amount)
        {
            var isSuccess = await _productService.UpdateStock(productId, amount);
            if (isSuccess)
                return Ok();

            return BadRequest();
        }

        //Images
        [HttpPost("{productId}/images")]
        public async Task<IActionResult> CreateImage(int productId, [FromForm] ProductImageCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var imageId = await _productService.AddImage(productId, request);
            if (imageId == 0)
                return BadRequest();

            var image = await _productService.GetImageById(imageId);

            return CreatedAtAction(nameof(GetImageById), new { id = imageId }, image);
        }

        [HttpPut("{productId}/images/{imageId}")]
        [Authorize]
        public async Task<IActionResult> UpdateImage(int imageId, [FromForm] ProductImageUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.UpdateImage(imageId, request);
            if (result == 0)
                return BadRequest();

            return Ok();
        }

        [HttpDelete("{productId}/images/{imageId}")]
        [Authorize]
        public async Task<IActionResult> RemoveImage(int imageId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.RemoveImage(imageId);
            if (result == 0)
                return BadRequest();

            return Ok();
        }

        [HttpGet("{productId}/images/{imageId}")]
        public async Task<IActionResult> GetImageById(int productId, int imageId)
        {
            var image = await _productService.GetImageById(imageId);
            if (image == null)
                return BadRequest("Cannot find product");
            return Ok(image);
        }
    }
}
