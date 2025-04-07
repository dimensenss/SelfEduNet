using System.ComponentModel.DataAnnotations;
using SelfEduNet.Data.Enum;
using SelfEduNet.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfEduNet.ViewModels
{
    public class CreateCourseViewModel
    {
        public int Id { get; set; }
        [Required]
        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public AppUser? Owner { get; set; }
		[Required]
		public string CourseName { get; set; }
    }
}
