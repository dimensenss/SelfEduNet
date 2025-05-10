using Microsoft.AspNetCore.Identity;

namespace SelfEduNet.Models
{
    public class AppUser: IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhotoURL { get; set; }
        public ICollection<CourseInfo> AuthoredCourses { get; set; } = new List<CourseInfo>();
		public ICollection<Course> OwnedCourses { get; set; } = new List<Course>();
		public ICollection<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
		public ICollection<UserStep> UserSteps { get; set; } = new List<UserStep>();
        public ICollection<UserLesson> UserLessons{ get; set; } = new List<UserLesson>();
	}
}
