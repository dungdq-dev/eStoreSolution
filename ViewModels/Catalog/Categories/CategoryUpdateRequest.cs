using Common.Enums;

namespace ViewModels.Catalog.Categories
{
    public class CategoryUpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SeoDescription { get; set; }
        public string SeoTitle { get; set; }
        public int SortOrder { get; set; }
        public bool IsShowOnHome { get; set; }
        public int? ParentId { get; set; }
        public Status Status { get; set; }
        public string LanguageId { get; set; }
    }
}