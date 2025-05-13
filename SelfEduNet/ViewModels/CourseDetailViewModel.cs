using SelfEduNet.Models;

namespace SelfEduNet.ViewModels;
public class CourseWithUserViewModel
{
	public Course Course { get; set; }
	public UserCourse? UserCourse { get; set; }
	public int EnrolledCount { get; set; }
	public double CourseRate { get; set; }
	public int CourseReviewsCount { get; set; }
}

