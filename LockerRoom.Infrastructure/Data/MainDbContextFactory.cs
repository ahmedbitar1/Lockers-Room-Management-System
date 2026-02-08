using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace LockerRoom.Infrastructure.Data
{
    public class MainDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>
    {
        public MainDbContext CreateDbContext(string[] args)
        {
            // نقرأ ملف appsettings.json من مشروع الويب
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../LockerRoom.Web");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<MainDbContext>();
            optionsBuilder.UseSqlServer(connectionString,
                b => b.MigrationsAssembly("LockerRoom.Infrastructure"));

            return new MainDbContext(optionsBuilder.Options);
        }
    }
}
