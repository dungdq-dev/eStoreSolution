using DataAccess.Configurations;
using DataAccess.Entities;
using DataAccess.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data
{
    public class EStoreDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public EStoreDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Configure using Fluent API
            builder.ApplyConfiguration(new AppConfigConfiguration());
            builder.ApplyConfiguration(new AppUserConfiguration());
            builder.ApplyConfiguration(new AppRoleConfiguration());

            builder.ApplyConfiguration(new ProductConfiguration());
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new ProductCategoryConfiguration());
            builder.ApplyConfiguration(new ProductImageConfiguration());
            builder.ApplyConfiguration(new CategoryTranslationConfiguration());
            builder.ApplyConfiguration(new ContactConfiguration());
            builder.ApplyConfiguration(new LanguageConfiguration());
            builder.ApplyConfiguration(new ProductTranslationConfiguration());
            builder.ApplyConfiguration(new PromotionConfiguration());
            builder.ApplyConfiguration(new ProductReviewConfiguration());

            builder.ApplyConfiguration(new CartConfiguration());
            builder.ApplyConfiguration(new OrderConfiguration());
            builder.ApplyConfiguration(new OrderDetailConfiguration());
            builder.ApplyConfiguration(new TransactionConfiguration());

            builder.ApplyConfiguration(new SlideConfiguration());

            builder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaims");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRoles").HasKey(x => new { x.UserId, x.RoleId });
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("AppUserLogins").HasKey(x => x.UserId);

            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppRoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("AppUserTokens").HasKey(x => x.UserId);

            builder.Seed();

            base.OnModelCreating(builder);
        }

        public virtual DbSet<AppConfig> AppConfigs { get; set; }

        public virtual DbSet<AppUser> AppUsers { get; set; }

        public virtual DbSet<AppRole> AppRoles { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Cart> Carts { get; set; }

        public virtual DbSet<CategoryTranslation> CategoryTranslations { get; set; }

        public virtual DbSet<ProductCategory> ProductCategories { get; set; }

        public virtual DbSet<Contact> Contacts { get; set; }

        public virtual DbSet<Language> Languages { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderDetail> OrderDetails { get; set; }

        public virtual DbSet<ProductTranslation> ProductTranslations { get; set; }

        public virtual DbSet<ProductReview> ProductReviews { get; set; }

        public virtual DbSet<Promotion> Promotions { get; set; }

        public virtual DbSet<Transaction> Transactions { get; set; }

        public virtual DbSet<ProductImage> ProductImages { get; set; }

        public virtual DbSet<Slide> Slides { get; set; }
    }
}