using System.Globalization;
using Worker.Configurations;

namespace Worker.Extensions
{
	internal static class StartupExtensions
	{
		public static IServiceCollection AddOptionsInjection(this IServiceCollection services, IConfiguration configuration)
		{
			return services
				.Configure<OpenAISettings>(configuration.GetSection(nameof(OpenAISettings)));
		}
	}
}