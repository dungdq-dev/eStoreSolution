using BusinessLogic.Utilities.Slides;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Slides")]
    [ApiController]
    public class SlidesController : ControllerBase
    {
        private readonly ISlideService _slideService;

        public SlidesController(ISlideService slideService)
        {
            _slideService = slideService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetSlides()
        {
            var slides = await _slideService.GetAll();
            return Ok(slides);
        }
    }
}