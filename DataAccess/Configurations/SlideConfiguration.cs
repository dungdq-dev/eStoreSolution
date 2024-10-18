using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class SlideConfiguration : IEntityTypeConfiguration<Slide>
    {
        public void Configure(EntityTypeBuilder<Slide> builder)
        {
            builder.ToTable("Slides");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();

            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Url).IsRequired();
            builder.Property(x => x.Image).IsRequired();
        }
    }
}