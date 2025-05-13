namespace SelfEduNet.Models;
public class Review
{
	public int Id { get; set; }
	public int UserCourseId { get; set; }
	public UserCourse UserCourse { get; set; }
	public string UserId { get; set; }
	public AppUser User { get; set; }
	public string Text { get; set; }
	public int Rate { get; set; } = default;
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

