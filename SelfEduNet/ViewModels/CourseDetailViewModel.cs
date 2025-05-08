using SelfEduNet.Models;

namespace SelfEduNet.ViewModels;
public class CourseWithUserViewModel
{
	public Course Course { get; set; }
	public UserCourse? UserCourse { get; set; } // null — не записан
}

