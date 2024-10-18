using Common.Enums;

namespace DataAccess.Entities
{
    public class Category
    {
        public int Id { set; get; }
        public int SortOrder { set; get; }
        public bool IsShowOnHome { set; get; }
        public int? ParentId { set; get; }
        public Status Status { set; get; }

        public ICollection<ProductCategory> ProductCategories { get; set; }

        public ICollection<CategoryTranslation> CategoryTranslations { get; set; }
    }
}