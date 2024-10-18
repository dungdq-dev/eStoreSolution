using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();

            builder.Property(x => x.Price).HasColumnType("decimal(18,2)").IsRequired();

            builder.Property(x => x.OriginalPrice).HasColumnType("decimal(18,2)").IsRequired();

            builder.Property(x => x.Stock).HasDefaultValue(0);

            builder.Property(x => x.ViewCount).HasDefaultValue(0);
        }
    }
}