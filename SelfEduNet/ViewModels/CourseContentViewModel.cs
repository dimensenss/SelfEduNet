using SelfEduNet.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfEduNet.ViewModels
{
	public class CourseContentViewModel
	{
		public int Id { get; set; }
		public string? CourseName { get; set; }
		public Course Course { get; set; }
		public ICollection<CourseModules> Modules { get; set; } = new List<CourseModules>();
		public int? Order { get; set; } = 0;
		public ICollection<Lesson>? Lessons { get; set; } = new List<Lesson>();

	}
}
