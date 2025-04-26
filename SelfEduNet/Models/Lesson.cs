using System.ComponentModel.DataAnnotations;

namespace SelfEduNet.Models
{
    public class Lesson
    {
        public int Id { get; set; }

        [Required]
        public int CourseModuleId { get; set; }
        public int? CourseId { get; set; }
        public CourseModules? CourseModule { get; set; }
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        public int Order { get; set; }
		public ICollection<Step>? Steps { get; set; } = new List<Step>();
		public ICollection<UserLesson> UserLessons { get; set; } = new List<UserLesson>();

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
		public override string ToString()
        {
            return $"Lesson: {Title}";
        }
    }
}
