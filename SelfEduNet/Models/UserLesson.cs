namespace SelfEduNet.Models
{
	public class UserLesson
	{
		public string UserId { get; set; }
		public AppUser User { get; set; }
		public int LessonId { get; set; }
		public Lesson Lesson { get; set; }
		public bool IsCompleted { get; set; } = false;
		public DateTime? StartedAt { get; set; }
		public DateTime? CompletedAt { get; set; }
	}
}
