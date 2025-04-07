namespace SelfEduNet.ViewModels
{
	public class LessonStatistics
	{
		public DateTime StartLessonTime { get; set; }
		public DateTime EndLessonTime { get; set; }
		public int CompletedStepsCount { get; set; }
		public int CompletedStepsPercentage { get; set; }
	}
}
