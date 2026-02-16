using InventoryManagementSoftware.Api.Data;
using InventoryManagementSoftware.Api.Hubs;
using InventoryManagementSoftware.Api.Models;
using InventoryManagementSoftware.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with Npgsql
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Authentication: register Google/Facebook providers in configuration (placeholders here)
builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        // configure via configuration: ClientId, ClientSecret
    })
    .AddFacebook(fbOptions =>
    {
        // configure via configuration: AppId, AppSecret
    });

// SignalR
builder.Services.AddSignalR();

// DI
builder.Services.AddScoped<ICustomIdGenerator, CustomIdGenerator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapHub<RealtimeHub>("/realtime");

app.Run();