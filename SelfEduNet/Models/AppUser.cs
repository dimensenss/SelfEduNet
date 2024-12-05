using Microsoft.AspNetCore.Identity;

namespace SelfEduNet.Models
{
    public class AppUser: IdentityUser
    {
        public ICollection<Course> OwnedCourses { get; set; }
        public ICollection<CourseUserRelation> CourseRelations { get; set; }
    }
}
