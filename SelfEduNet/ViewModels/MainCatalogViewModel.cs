using SelfEduNet.Models;

namespace SelfEduNet.ViewModels
{
    public class MainCatalogViewModel
    {
        public IEnumerable<Course> Courses { get; set; }
        public CourseFilter CourseFilter { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
