using Common.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Data
{
    public class EStoreDbContextFactory : IDesignTimeDbContextFactory<EStoreDbContext>
    {
        public EStoreDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString(SystemConstants.DefaultConnection);

            var optionsBuilder = new DbContextOptionsBuilder<EStoreDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new EStoreDbContext(optionsBuilder.Options);
        }
    }
}