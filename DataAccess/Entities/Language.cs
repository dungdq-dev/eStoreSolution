namespace DataAccess.Entities
{
    public class Language
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }

        public ICollection<ProductTranslation> ProductTranslations { get; set; }

        public ICollection<CategoryTranslation> CategoryTranslations { get; set; }
    }
}