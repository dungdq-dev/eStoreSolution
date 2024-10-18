using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public string Description { get; set; }
    }
}