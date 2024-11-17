using System.ComponentModel.DataAnnotations;

namespace SelfEduNet.Models
{
    public class Lesson
    {
        public int Id { get; set; }

        [Required]
        public int CourseModuleId { get; set; }
        public CourseModules CourseModule { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string Content { get; set; } // Основной текст урока

        public string VideoUrl { get; set; } // URL для видео

        public override string ToString()
        {
            return $"Lesson: {Title}";
        }
    }
}
