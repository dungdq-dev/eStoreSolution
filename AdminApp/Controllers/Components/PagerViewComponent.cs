using Microsoft.AspNetCore.Mvc;
using ViewModels.Common;

namespace AdminApp.Controllers.Components
{
    public class PagerViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(BasePagedResponse result)
        {
            return Task.FromResult((IViewComponentResult)View("Default", result));
        }
    }
}