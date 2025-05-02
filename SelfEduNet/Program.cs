using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using System.Globalization;
using EduProject.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using SelfEduNet.Services;
using SelfEduNet.Repositories;
using Ganss.Xss;
using System;
using StackExchange.Redis;
using OpenAI;
using SelfEduNet.Configurations;
using Microsoft.Extensions.Hosting;

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
var builder = WebApplication.CreateBuilder(args);
builder.Configuration
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
	.AddEnvironmentVariables();

builder.Services.InjectDependencies();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection"));

// IConnectionMultiplexer для работы с очередью
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
	ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
    })

    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Identity/Account/Login";
	options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(
    options =>
    {
	    options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied"; 
	})
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration.GetValue<string>("GoogleOAuthSettings:ClientId");
        googleOptions.ClientSecret = builder.Configuration.GetValue<string>("GoogleOAuthSettings:ClientSecret");
        googleOptions.Scope.Add("profile");
        googleOptions.Scope.Add("email");
    });

builder.Services.AddOptionsInjection(builder.Configuration);


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await RoleInitializer.InitializeAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in roles initialization: {ex.Message}");
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();

    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetService<Seeder>();
        await seeder.SeedAsync();
    }
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapRazorPages();


app.Run();
