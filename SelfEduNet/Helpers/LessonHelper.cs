namespace SelfEduNet.Helpers
{
	public static class LessonHelper
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
	}
}
