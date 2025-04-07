using System.ComponentModel.DataAnnotations;
using SelfEduNet.Models;

namespace SelfEduNet.ViewModels
{
	public class EditLessonViewModel
	{
		[Required]
		public CourseContentViewModel? Course { get; set; }
		[Required]
		public Lesson? Lesson { get; set; }
		[Required]
		public List<Step>? Steps { get; set; }
	}
}
