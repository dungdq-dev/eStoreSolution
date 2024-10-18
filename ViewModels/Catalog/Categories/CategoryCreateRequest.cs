namespace ViewModels.Catalog.Categories
{
    public class CategoryCreateRequest
    {
        public string Name { get; set; }
        public string SeoDescription { get; set; }
        public string SeoTitle { get; set; }
        public int SortOrder { get; set; }
        public bool IsShowOnHome { get; set; }
        public string LanguageId { get; set; }
    }
}