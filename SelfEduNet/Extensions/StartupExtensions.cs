using System.Globalization;
using EduProject.Helpers;
using Microsoft.AspNetCore.Localization;
using SelfEduNet.Configurations;

namespace SelfEduNet.Extensions
{
	internal static class StartupExtensions
	{
		public static IServiceCollection AddOptionsInjection(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddLocalization(options => options.ResourcesPath = "Resources");
			return services
				.Configure<EmailSMTPSettings>(configuration.GetSection(nameof(EmailSMTPSettings)))
				.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"))
				.Configure<RequestLocalizationOptions>(options =>
				{
					var supportedCultures = new[] { new CultureInfo("uk-UA") };
					options.DefaultRequestCulture = new RequestCulture("uk-UA");
					options.SupportedCultures = supportedCultures;
					options.SupportedUICultures = supportedCultures;
				});
		}
	}
}
