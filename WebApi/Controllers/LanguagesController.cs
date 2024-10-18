using BusinessLogic.System.Languages;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/languages")]
    [ApiController]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        // GET: api/Languages
        [HttpGet]
        public async Task<IActionResult> GetLanguages()
        {
            var languages = await _languageService.GetAll();
            return Ok(languages);
        }
    }
}
