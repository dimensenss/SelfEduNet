namespace SelfEduNet.ViewModels;
public class StepTestResult
{
	public int Score { get; set; }
	public int TotalScore { get; set; }
	public DateTime Timestamp { get; set; }
	public int AttemptsCount { get; set; }
	public string BiggestScore { get; set; } = null!;
	public bool IsPassed { get; set; } = false;
}

