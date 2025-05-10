using System.ComponentModel.DataAnnotations;

namespace SelfEduNet.ViewModels;
public class EditUserProfileViewModel
{

	[Required(ErrorMessage = "Ім'я є обов'язковим")]
	[StringLength(50, ErrorMessage = "Ім'я не може перевищувати 50 символів")]
	[Display(Name = "Ім'я")]
	public string? FirstName { get; set; }

	[Required(ErrorMessage = "Прізвище є обов'язковим")]
	[StringLength(50, ErrorMessage = "Прізвище не може перевищувати 50 символів")]
	[Display(Name = "Прізвище")]
	public string? LastName { get; set; }

	[Display(Name = "Фото профілю")]
	public IFormFile? PhotoFile { get; set; }
	public string? PhotoURL { get; set; }
}

