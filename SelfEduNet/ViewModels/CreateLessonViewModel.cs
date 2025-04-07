using System.ComponentModel.DataAnnotations;
using SelfEduNet.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfEduNet.ViewModels
{
	public class CreateLessonViewModel
	{
		public int ModuleId { get; set; }
		[Required]
		public string Title { get; set; }
	}
}
