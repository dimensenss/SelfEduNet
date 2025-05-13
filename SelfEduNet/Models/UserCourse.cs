using System.ComponentModel.DataAnnotations;

namespace SelfEduNet.Models;
public class UserCourse
{
	public int Id { get; set; }
	public int CourseId { get; set; }
	public Course Course { get; set; }
	public string UserId { get; set; }
	public AppUser User { get; set; }

	[Range(1, 5, ErrorMessage = "Рейтинг повинен бути між {1} і {2}.")]
	public int Rate { get; set; } = default;
	public bool IsWish { get; set; } = false;
	public bool IsEnrolled { get; set; } = false;
	public bool IsCompleted { get; set; } = false;
	public DateTime? CompletedAt { get; set; } = DateTime.UtcNow;
	public DateTime? EnrolledAt { get; set; } = DateTime.UtcNow;
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public Review? Review { get; set; }
}

