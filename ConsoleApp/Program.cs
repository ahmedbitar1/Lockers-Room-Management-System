using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LockerRoom.Infrastructure.Data;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = builder.Build();

// تجهيز DI container
var serviceCollection = new ServiceCollection();
serviceCollection.AddDbContext<MainDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

var serviceProvider = serviceCollection.BuildServiceProvider();

Console.WriteLine("🔍 Testing database connection...");

try
{
    var context = serviceProvider.GetRequiredService<MainDbContext>();

    // نحاول نقرأ عدد الجداول أو نحسب عدد العناصر من أي جدول
    var lockersCount = context.Lockers.Count();
    Console.WriteLine($"✅ Connection successful! Found {lockersCount} lockers in DB.");
}
catch (Exception ex)
{
    Console.WriteLine("❌ Connection failed:");
    Console.WriteLine(ex.Message);
}

Console.ReadLine();
