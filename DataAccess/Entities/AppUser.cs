using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime Dob { get; set; }

        public ICollection<Cart> Carts { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<ProductReview> ProductReviews { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}