using SelfEduNet.Models;

namespace SelfEduNet.ViewModels;

public class CourseCatalogViewModel
{
	public List<Course> Courses { get; set; }
	public CourseFilter Filter { get; set; }
	public int TotalItems { get; set; }

	public int TotalPages => (int)Math.Ceiling((double)TotalItems / Filter.PageSize);
}

