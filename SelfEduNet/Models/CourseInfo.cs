using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfEduNet.Models
{
    public class CourseInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        [Range(0, int.MaxValue)]
        public int Workload { get; set; } = 0;

        public ICollection<AppUser> Authors { get; set; } = new List<AppUser>();

        public string? PreviewVideo { get; set; } // URL для превью-видео
    }
}
