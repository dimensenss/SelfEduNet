namespace SelfEduNet.ViewModels;
public class CourseStatistics
{
	public DateTime StartCourseTime { get; set; }
	public DateTime EndCourseTime { get; set; }
	public int CompletedLessonsCount { get; set; }
	public int CompletedLessonsPercentage { get; set; }
}

