using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.ToTable("ProductReviews");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();

            builder.Property(x => x.Content).HasMaxLength(2000).IsRequired();

            builder.HasOne(x => x.AppUser).WithMany(x => x.ProductReviews).HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Product).WithMany(x => x.ProductReviews).HasForeignKey(x => x.ProductId);
        }
    }
}