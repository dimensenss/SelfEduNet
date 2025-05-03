namespace SelfEduNet.Models;
public class UserTestResult
{
	public int Id { get; set; }
	public int StepTestId { get; set; }
	public StepTest StepTest { get; set; } = null!;
	public int UserStepId { get; set; }
	public UserStep UserStep { get; set; } = null!;

	public string UserId { get; set; }
	public int StepId { get; set; }

	public int Score { get; set; }
	public int TotalScore { get; set; }
	public DateTime Timestamp { get; set; }
	public int AttemptsCount { get; set; }
	public string BiggestScore { get; set; } = null!;
	public bool IsPassed { get; set; } = false;
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

