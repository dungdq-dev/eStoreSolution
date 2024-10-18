using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();

            builder.Property(x => x.Price).HasColumnType("decimal(18,2)").IsRequired();

            builder.HasOne(x => x.Product).WithMany(x => x.Carts).HasForeignKey(x => x.ProductId);
            builder.HasOne(x => x.AppUser).WithMany(x => x.Carts).HasForeignKey(x => x.UserId);
        }
    }
}