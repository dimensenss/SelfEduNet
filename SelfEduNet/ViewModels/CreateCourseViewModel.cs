using System.ComponentModel.DataAnnotations;
using SelfEduNet.Data.Enum;
using SelfEduNet.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json.Serialization;

namespace SelfEduNet.ViewModels
{
    public class CreateCourseViewModel
    {
        public int Id { get; set; }
        [Required]
        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public AppUser? Owner { get; set; }
		[Required(ErrorMessage = "Назва курсу є обов'язковим полем")]
        
		public string CourseName { get; set; }
    }
}
