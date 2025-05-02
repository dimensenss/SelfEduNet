using System.Globalization;
using Microsoft.AspNetCore.Localization;
using SelfEduNet.Configurations;
using SelfEduNet.Services;

namespace SelfEduNet.Extensions
{
	internal static class StartupExtensions
	{
		public static IServiceCollection AddOptionsInjection(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddLocalization(options => options.ResourcesPath = "Resources");
			return services
				.Configure<EmailSMTPSettings>(configuration.GetSection(nameof(EmailSMTPSettings)))
				.Configure<CloudinarySettings>(configuration.GetSection(nameof(CloudinarySettings)))
				.Configure<GoogleSheetService>(configuration.GetSection(nameof(GoogleSheetService)))
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
