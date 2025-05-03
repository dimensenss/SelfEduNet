using System.ComponentModel.DataAnnotations;

namespace SelfEduNet.ViewModels;

public class CreateTestViewModel
{
	[Required]
	public int Id { get; set; }
	[Required]
	[RegularExpression(@"^https:\/\/docs\.google\.com\/forms\/d\/e\/[a-zA-Z0-9_-]+\/viewform(\?.*)?$", ErrorMessage = "Недійсне посилання на Google Form")]
	public string GoogleFormUrl { get; set; } = string.Empty;
	[Required]
	[RegularExpression(@"^https:\/\/docs\.google\.com\/spreadsheets\/d\/[a-zA-Z0-9_-]+\/edit(\?.*)?(#.*)?$", ErrorMessage = "Недійсне посилання на Google Sheet")]
	public string GoogleSheetUrl { get; set; } = string.Empty;
}

