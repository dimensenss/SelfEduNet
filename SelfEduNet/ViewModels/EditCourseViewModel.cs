using SelfEduNet.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SelfEduNet.Data.Enum;

namespace SelfEduNet.ViewModels
{
	using System.ComponentModel.DataAnnotations;

	public class EditCourseViewModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Оберіть мову курсу.")]
		public LanguageType Language { get; set; }

		public DifficultyType? Difficulty { get; set; }

		[Required(ErrorMessage = "Назва курсу є обов'язковою.")]
		[StringLength(100, ErrorMessage = "Назва курсу не може перевищувати 100 символів.")]
		public string CourseName { get; set; }

		[Required(ErrorMessage = "Опис курсу є обов'язковим.")]
		[StringLength(500, ErrorMessage = "Опис курсу не може перевищувати 500 символів.")]
		public string Description { get; set; }
		public string? PromoText { get; set; }

		[Range(0, int.MaxValue, ErrorMessage = "Ціна повинна бути додатним числом.")]
		public int FullPrice { get; set; } = 0;

		public bool HaveCertificate { get; set; } = false;

		public IFormFile? Preview { get; set; }

		[Url(ErrorMessage = "Некоректна URL-адреса для попереднього перегляду.")]
		public string? PreviewURL { get; set; }

		public Category? Category { get; set; } = default!;

		public ICollection<CourseModules> Modules { get; set; } = new List<CourseModules>();

		[Range(0, int.MaxValue, ErrorMessage = "Завантаження повинно бути додатним числом.")]
		public int Workload { get; set; } = 0;
		public ICollection<AppUser> Authors { get; set; } = new List<AppUser>();

		[Url(ErrorMessage = "Некоректна URL-адреса для попереднього відео.")]
		public string? PreviewVideo { get; set; }

		public bool IsPublished { get; set; } = false;
	}

}
