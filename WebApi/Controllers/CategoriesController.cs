using BusinessLogic.Catalog.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Catalog.Categories;

namespace WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] CategoryCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var categoryId = await _categoryService.Create(request);
            if (categoryId == 0)
                return BadRequest();

            var category = await _categoryService.GetById(categoryId, request.LanguageId);

            return CreatedAtAction(nameof(GetById), 
                new { id = categoryId, languageId = request.LanguageId }, 
                category);
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] string languageId)
        {
            var categories = await _categoryService.GetAll(languageId);
            return Ok(categories);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetAllPaged([FromQuery] GetCategoryPagingRequest request)
        {
            var categories = await _categoryService.GetAllPaged(request);
            return Ok(categories);
        }

        [HttpGet("{id}/{languageId}")]
        [ActionName(nameof(GetById))]
        public async Task<IActionResult> GetById([FromRoute] int id, string languageId)
        {
            var result = await _categoryService.GetById(id, languageId);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] CategoryUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = id;
            var affectedResult = await _categoryService.Update(request);
            if (affectedResult == 0)
                return BadRequest();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var affectedResult = await _categoryService.Delete(id);
            if (affectedResult == 0)
                return BadRequest();

            return Ok();
        }
    }
}
