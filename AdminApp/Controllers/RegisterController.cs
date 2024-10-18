using ApiIntegration;
using Microsoft.AspNetCore.Mvc;
using ViewModels.System.Users;

namespace AdminApp.Controllers
{
    [Controller]
    public class RegisterController : Controller
    {
        private readonly IUserApiClient _userApiClient;

        public RegisterController(IUserApiClient userApiClient)
        {
            _userApiClient = userApiClient;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _userApiClient.Register(request);
            if (result == null)
                return RedirectToAction("Register");
            if (result.IsSuccess)
            {
                TempData["result"] = "Tạo tài khoản thành công";
                return RedirectToAction("Index", "Login");
            }
            ModelState.AddModelError("", result.Message ?? "Unhandled error");

            return View(request);
        }
    }
}
