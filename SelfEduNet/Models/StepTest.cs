using System.ComponentModel.DataAnnotations;

namespace SelfEduNet.Models;

public class StepTest
{
	public int Id { get; set; }

	[Required]
	public int StepId { get; set; }
	public Step? Step { get; set; }

	[Required]
	[Url]
	public string GoogleFormUrl { get; set; } = string.Empty;

	[Required]
	[Url]
	public string GoogleSheetUrl { get; set; } = string.Empty;

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
	public ICollection<UserTestResult> UserTestResults { get; set; } = new List<UserTestResult>();
}
