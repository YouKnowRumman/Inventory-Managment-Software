using InventoryManagementSoftware.api.Data;
using InventoryManagementSoftware.api.Services;
using InventoryManagementSoftware.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =========================
// CONFIGURATION
// =========================

// Configure environment-specific settings
var environment = builder.Environment.EnvironmentName;
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// =========================
// PORT CONFIGURATION
// =========================

// Configure Kestrel to listen on the PORT environment variable or default ports
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port) && int.TryParse(port, out var portNumber))
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(portNumber);
    });
}

// =========================
// DATABASE
// =========================

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found. Ensure it is configured in appsettings or environment variables.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// =========================
// IDENTITY
// =========================

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
});

// =========================
// SERVICES
// =========================

builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<ICustomIdGeneratorService, CustomIdGeneratorService>();
builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddControllers();

// =========================
// CORS
// =========================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = new[] 
        { 
            "https://inventory-managment-software.vercel.app",
            "http://localhost:5173",
            "http://localhost:3000"
        };

        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// =========================
// AUTH
// =========================

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// =========================
// BUILD
// =========================

var app = builder.Build();

// =========================
// DATABASE SEEDING
// =========================

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration or seeding");
        throw;
    }
}


// =========================
// MIDDLEWARE
// =========================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => "API is running");

app.Run();
