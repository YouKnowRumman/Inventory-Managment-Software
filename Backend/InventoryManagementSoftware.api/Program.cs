using InventoryManagementSoftware.api.Data;
using InventoryManagementSoftware.api.Services;
using InventoryManagementSoftware.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllers();

// 2. Register Services
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<ICustomIdGeneratorService, CustomIdGeneratorService>();

// 3. Add DbContext (Connect to Supabase)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 4. FIX: Correct CORS Setup
// We use a specific name "AllowedOrigins" and use it consistently.
var allowedOrigins = "AllowedOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin() // For testing, allow ALL origins. Secure this later if needed.
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// 5. Add Authentication
builder.Services.AddAuthentication()
    .AddCookie("Cookies");

builder.Services.AddAuthorization();

var app = builder.Build();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));


// 6. Middleware Pipeline
app.UseRouting();

// FIX: Use the SAME name defined above
app.UseCors(allowedOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 7. FIX: REMOVED THE MIGRATION BLOCK
// The automatic migration/seeding block has been removed to prevent
// the "Timeout" and "UUID" errors on Render's Transaction Pooler.
// You have already manually migrated using the SQL script.

app.Run();