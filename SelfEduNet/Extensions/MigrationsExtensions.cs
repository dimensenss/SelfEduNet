using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;

namespace SelfEduNet.Extensions
{
	public static class MigrationsExtensions
	{
		public static void ApplyMigrations(this IApplicationBuilder app)
		{
			using IServiceScope serviceScope = app.ApplicationServices.CreateScope();
			using ApplicationDbContext context = 
				serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			context.Database.Migrate();
		}
	}
}
