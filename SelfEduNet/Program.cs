using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllersWithViews();
builder.Services.AddIdentity<AppUser, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    options =>
    {
        options.LoginPath = "/Account/Login";
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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
