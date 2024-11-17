using System.ComponentModel.DataAnnotations;

namespace SelfEduNet.Models
{
    public class CourseModules
    {
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [MaxLength(255)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Range(0, int.MaxValue)]
        public int Order { get; set; } = 0;

        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

        public override string ToString()
        {
            return $"Course: {Course?.CourseName} - Module: {Title}";
        }
    }
}
