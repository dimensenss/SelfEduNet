using System.ComponentModel.DataAnnotations;

namespace SelfEduNet.Models
{
    public class CourseModules
    {
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [MaxLength(255)]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁіїІЇєЄґҐ0-9\s\-\.\@\#\$\%\^\&\*\(\)\+\=\:;,.]+$", ErrorMessage = "Назва модуля може містити лише літери, цифри, пробіли, дефіси та спеціальні символи @#$%^&*()_+=:;,.")]
		public string? Title { get; set; }
        [MaxLength(1024)]
		public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int? Order { get; set; } = 0;

        public ICollection<Lesson>? Lessons { get; set; } = new List<Lesson>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


		public override string ToString()
        {
            return $"Course: {Course?.CourseName} - Module: {Title}";
        }
    }
}
