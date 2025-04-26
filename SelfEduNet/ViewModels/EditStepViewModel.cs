using System.ComponentModel.DataAnnotations;

namespace SelfEduNet.ViewModels;

public class EditStepViewModel
{
	public required int Id { get; set; }
	public string? Content { get; set; } = string.Empty;
	[MaxLength(10000)]
	public string? Resume { get; set; } = string.Empty;

	[MaxLength(65000)]
	public string? Context { get; set; } = string.Empty;
	public string? VideoUrl { get; set; } = string.Empty;
}
