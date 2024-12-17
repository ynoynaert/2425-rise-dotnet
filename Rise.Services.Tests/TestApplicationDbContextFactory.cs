using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Rise.Persistence;

namespace Rise.Services.Tests
{
    public class TestApplicationDbContextFactory
    {
        public static ApplicationDbContext CreateDbContext()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<TestApplicationDbContextFactory>()
                .Build();

            var connectionString = configuration.GetConnectionString("SqlServer");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
