namespace SelfEduNet.Models
{
	public class UserStep
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public AppUser User { get; set; }
		public int StepId { get; set; }
		public Step Step { get; set; }
		public bool IsCompleted { get; set; } = false;
		public bool IsViewed { get; set; } = false;
		public DateTime? ViewedAt { get; set; }
		public DateTime? CompletedAt { get; set; }
		public UserTestResult? UserTestResult { get; set; }
	}
}
