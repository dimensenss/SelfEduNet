using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using EduProject.Services;
using Ganss.Xss;
using Microsoft.AspNetCore.Identity.UI.Services;
using SelfEduNet.Data;
using SelfEduNet.Repositories;
using SelfEduNet.Services;

namespace SelfEduNet.Extensions;

public static class DependencyInjection
{
	public static IServiceCollection InjectDependencies(this IServiceCollection services)
	{
		return services
			.AddScoped<IEmailSender, EmailSenderService>()
			.AddScoped<ICourseRepository, CourseRepository>()
			.AddScoped<ICategoryRepository, CategoryRepository>()
			.AddScoped<IStepRepository, StepRepository>()
			.AddScoped<IUserStepRepository, UserStepRepository>()
			.AddScoped<IPhotoService, PhotoService>()
			.AddScoped<ICourseService, CourseService>()
			.AddScoped<ICategoryService, CategoryService>()
			.AddScoped<IStepService, StepService>()
			.AddScoped<IUserStepService, UserStepService>()
			.AddScoped<IUserLessonService, UserLessonService>()
			.AddScoped<IUserLessonRepository, UserLessonRepository>()
			.AddScoped<ITranscriptionService, TranscriptionService>()
			.AddScoped<ITranscriptionRepository, TranscriptionRepository>()
			.AddSingleton<IHtmlSanitizer, HtmlSanitizer>()
			.AddTransient<Seeder>();
	}
}
