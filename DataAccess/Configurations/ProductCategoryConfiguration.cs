using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.ToTable("ProductCategories");

            builder.HasKey(t => new { t.CategoryId, t.ProductId });

            builder.HasOne(t => t.Product).WithMany(pc => pc.ProductCategories)
                .HasForeignKey(pc => pc.ProductId);

            builder.HasOne(t => t.Category).WithMany(pc => pc.ProductCategories)
              .HasForeignKey(pc => pc.CategoryId);
        }
    }
}