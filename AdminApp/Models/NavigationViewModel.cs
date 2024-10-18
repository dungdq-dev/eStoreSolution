using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminApp.Models
{
    public class NavigationViewModel
    {
        public List<SelectListItem> Languages { get; set; }

        public string CurrentLanguageId { get; set; } = string.Empty;

        public string ReturnUrl { set; get; } = string.Empty;
    }
}