namespace SelfEduNet.Helpers
{
	public static class CourseHelper
	{
		public static string GetLessonText(int count)
		{
			if (count % 10 == 1 && count % 100 != 11)
			{
				return "урок";
			}
			else if ((count % 10 >= 2 && count % 10 <= 4) && !(count % 100 >= 12 && count % 100 <= 14))
			{
				return "уроки";
			}
			else
			{
				return "уроків";
			}
		}
		public static string GetStudentsCountText(int count)
		{
			if (count % 10 == 1 && count % 100 != 11)
			{
				return "учень";
			}
			else if ((count % 10 >= 2 && count % 10 <= 4) && !(count % 100 >= 12 && count % 100 <= 14))
			{
				return "учні";
			}
			else
			{
				return "учнів";
			}
		}
		public static string GetReviewsCountText(int count)
		{
			if (count % 10 == 1 && count % 100 != 11)
			{
				return "відгук";
			}
			else if ((count % 10 >= 2 && count % 10 <= 4) && !(count % 100 >= 12 && count % 100 <= 14))
			{
				return "відгуки";
			}
			else
			{
				return "відгуків";
			}
		}
	}
}
