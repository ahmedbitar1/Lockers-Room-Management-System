using LockerRoom.Core.Interfaces;
using LockerRoom.Infrastructure.Data;
using LockerRoom.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// ✅ Add MVC Controllers and Views
builder.Services.AddControllersWithViews();

// ✅ Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<MainDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("LockerRoom.Infrastructure")
    ));

// ✅ Register UnitOfWork (Dependency Injection)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHttpClient<IPOSService, POSService>();

// ✅ Enable Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ✅ Configure the HTTP Request Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Enable session before using authentication or custom middleware
app.UseSession();

// ✅ Middleware: block access if user is not logged in
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();

    // Allow login & static files without authentication
    if (path!.Contains("/login") || path.Contains("/css") || path.Contains("/js") || path.Contains("/lib"))
    {
        await next();
        return;
    }

    var username = context.Session.GetString("Username");

    if (string.IsNullOrEmpty(username))
    {
        context.Response.Redirect("/Login/Index");
        return;
    }

    await next();
});

app.UseAuthorization();

// ✅ Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Reception}/{action=Index}/{id?}");

app.Run();
