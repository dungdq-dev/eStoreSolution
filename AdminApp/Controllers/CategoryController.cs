using ApiIntegration;
using Common.Constants;
using Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ViewModels.Catalog.Categories;

namespace AdminApp.Controllers
{
    [Controller]
    public class CategoryController : ProtectedController
    {
        private readonly ICategoryApiClient _categoryApiClient;

        public CategoryController(ICategoryApiClient categoryApiClient)
        {
            _categoryApiClient = categoryApiClient;
        }

        public async Task<IActionResult> Index(string keyword = "", int pageIndex = 1, int pageSize = 25)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId) ?? "vi";
            var request = new GetCategoryPagingRequest()
            {
                LanguageId = languageId,
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _categoryApiClient.GetAllPaged(request);

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }

            return View(data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId) ?? "vi";
            var result = await _categoryApiClient.GetById(id, languageId);
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId) ?? "vi";
            var createRequest = new CategoryCreateRequest()
            {
                LanguageId = languageId
            };

            var statuses = from Status s in Enum.GetValues(typeof(Status))
                           select new
                           {
                               ID = (int)s,
                               Name = s.ToString()
                           };
            ViewBag.Status = new SelectList(statuses, "ID", "Name");

            return View(createRequest);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CategoryCreateRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return View();

                var result = await _categoryApiClient.Create(request);
                if (result)
                {
                    TempData["result"] = "Thêm loại sản phẩm thành công";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Thêm loại sản phẩm thất bại");

                return View(request);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId) ?? "vi";

            var category = await _categoryApiClient.GetById(id, languageId);
            var updateRequest = new CategoryUpdateRequest()
            {
                Id = category.Data.Id,
                Name = category.Data.Name,
                SeoAlias = category.Data.SeoAlias,
                SeoDescription = category.Data.SeoDescription,
                SeoTitle = category.Data.SeoTitle,
                SortOrder = category.Data.SortOrder,
                IsShowOnHome = category.Data.IsShowOnHome,
                Status = category.Data.Status,
                LanguageId = languageId,
            };

            var statuses = from Status s in Enum.GetValues(typeof(Status))
                           select new
                           {
                               ID = (int)s,
                               Name = s.ToString()
                           };
            ViewBag.Status = new SelectList(statuses, "ID", "Name");

            return View(updateRequest);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] CategoryUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(request);

                var result = await _categoryApiClient.Update(request);
                if (result)
                {
                    TempData["result"] = "Cập nhật loại sản phẩm thành công";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Cập nhật loại sản phẩm thất bại");
                return View(request);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return View(new CategoryDeleteRequest()
            {
                Id = id
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(CategoryDeleteRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _categoryApiClient.Delete(request.Id);
            if (result)
            {
                TempData["result"] = "Xóa loại sản phẩm thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Xóa loại sản phẩm thất bại");
            return View(request);
        }
    }
}
