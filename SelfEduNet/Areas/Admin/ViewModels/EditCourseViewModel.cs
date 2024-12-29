using SelfEduNet.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SelfEduNet.Data.Enum;

namespace SelfEduNet.Areas.Admin.ViewModels
{
    public class EditCourseViewModel
    {
        public int Id { get; set; }
        public LanguageType Language { get; set; }
        public DifficultyType? Difficulty { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        //public int? CategoryId { get; set; }
        public int FullPrice { get; set; } = 0;
        //public Category Category { get; set; }
        public bool HaveCertificate { get; set; } = false;

        public IFormFile? Preview { get; set; }
        public string? PreviewURL { get; set; }
        public ICollection<CourseModules> Modules { get; set; } = new List<CourseModules>();
        public int Workload { get; set; } = 0;
        public ICollection<AppUser> Authors { get; set; } = new List<AppUser>();
        public string? PreviewVideo { get; set; }
        public bool IsPublished { get; set; } = false;

    }
}
