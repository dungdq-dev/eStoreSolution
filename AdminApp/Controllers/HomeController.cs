using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AdminApp.Models;
using Common.Constants;
using ApiIntegration;

namespace AdminApp.Controllers
{
    [Controller]
    public class HomeController : ProtectedController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserApiClient _userApiClient;

        private static Guid UserId;
        private static Uri? BaseAddress;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IUserApiClient userApiClient)
        {
            _logger = logger;
            _configuration = configuration;
            _userApiClient = userApiClient;
        }

        public IActionResult Index()
        {
            var user = _userApiClient.GetByName(User.Identity.Name);
            var resultObj = user.Result.Data;

            UserId = resultObj.Id;

            BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SwitchLanguage(NavigationViewModel viewModel)
        {
            HttpContext.Session.SetString(SystemConstants.AppSettings.DefaultLanguageId, viewModel.CurrentLanguageId);
            return Redirect(viewModel.ReturnUrl);
        }

        public static Guid GetUserId() => UserId;

        public static Uri GetBaseAddress() => BaseAddress;
    }
}