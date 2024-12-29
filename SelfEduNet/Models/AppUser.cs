using Microsoft.AspNetCore.Identity;

namespace SelfEduNet.Models
{
    public class AppUser: IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public ICollection<CourseInfo> AuthoredCourses { get; set; } = new List<CourseInfo>();
		public ICollection<Course> OwnedCourses { get; set; }
        public ICollection<CourseUserRelation> CourseRelations { get; set; }
    }
}
