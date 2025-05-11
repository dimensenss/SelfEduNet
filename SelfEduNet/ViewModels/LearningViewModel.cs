using SelfEduNet.Models;

namespace SelfEduNet.ViewModels;
public class LearningViewModel
{
	public Dictionary<UserCourse, CourseCompletedSteps> EnrolledCourses { get; set; } = new();
}

