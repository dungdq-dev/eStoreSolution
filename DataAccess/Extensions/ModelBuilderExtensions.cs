using Common.Enums;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppConfig>().HasData(
               new AppConfig() { Key = "HomeTitle", Value = "This is home page of Electronix Store" },
               new AppConfig() { Key = "HomeKeyword", Value = "This is keyword of Electronix Store" },
               new AppConfig() { Key = "HomeDescription", Value = "This is description of Electronix Store" }
               );
            modelBuilder.Entity<Language>().HasData(
                new Language() { Id = "vi", Name = "Tiếng Việt", IsDefault = true },
                new Language() { Id = "en", Name = "English", IsDefault = false });

            modelBuilder.Entity<Category>().HasData(
                new Category()
                {
                    Id = 1,
                    IsShowOnHome = true,
                    ParentId = null,
                    SortOrder = 1,
                    Status = Status.Active,
                },
                 new Category()
                 {
                     Id = 2,
                     IsShowOnHome = true,
                     ParentId = null,
                     SortOrder = 2,
                     Status = Status.Active
                 });

            modelBuilder.Entity<CategoryTranslation>().HasData(
                  new CategoryTranslation() { Id = 1, CategoryId = 1, Name = "Máy tính", LanguageId = "vi", SeoAlias = "may-tinh", SeoDescription = "Sản phẩm máy tính để bàn", SeoTitle = "Sản phẩm máy tính để bàn" },
                  new CategoryTranslation() { Id = 2, CategoryId = 1, Name = "Computer", LanguageId = "en", SeoAlias = "computer", SeoDescription = "The computer products", SeoTitle = "The computer products" },
                  new CategoryTranslation() { Id = 3, CategoryId = 2, Name = "Tai nghe", LanguageId = "vi", SeoAlias = "tai-nghe", SeoDescription = "Sản phẩm tai nghe", SeoTitle = "Sản phẩm tai nghe" },
                  new CategoryTranslation() { Id = 4, CategoryId = 2, Name = "Headphone", LanguageId = "en", SeoAlias = "headphone", SeoDescription = "The headphone products", SeoTitle = "The headphone products" }
                    );

            // seed data slides
            modelBuilder.Entity<Slide>().HasData(
                new Slide() { Id = 1, Name = "First Thumbnail label", Description = "", SortOrder = 1, Url = "#", Image = "/images/carousel/slide_1.png", Status = Status.Active },
                new Slide() { Id = 2, Name = "Second Thumbnail label", Description = "", SortOrder = 2, Url = "#", Image = "/images/carousel/slide_2.png", Status = Status.Active },
                new Slide() { Id = 3, Name = "Third Thumbnail label", Description = "", SortOrder = 3, Url = "#", Image = "/images/carousel/slide_3.png", Status = Status.Active },
                new Slide() { Id = 4, Name = "Fourth Thumbnail label", Description = "", SortOrder = 4, Url = "#", Image = "/images/carousel/slide_4.png", Status = Status.Active }
            );

            // random guid
            var AdminRoleId = new Guid("8D04DCE2-969A-435D-BBA4-DF3F325983DC");
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = AdminRoleId,
                Name = "admin",
                NormalizedName = "admin",
                Description = "Administrator role"
            });

            // create root user
            var RootUserId = new Guid("69BD714F-9576-45BA-B5B7-F00649BE00DE");
            var hasher = new PasswordHasher<AppUser>();
            modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
                Id = RootUserId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "eshopadmin@yahoo.com",
                NormalizedEmail = "eshopadmin@yahoo.com",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Admin_123"),
                SecurityStamp = string.Empty,
                FirstName = "Dung",
                LastName = "Dau",
                Dob = new DateTime(year: 2000, month: 10, day: 17)
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
            {
                RoleId = AdminRoleId,
                UserId = RootUserId
            });
        }
    }
}