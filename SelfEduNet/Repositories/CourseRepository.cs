using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Models;

namespace SelfEduNet.Repositories
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        IQueryable<Course> GetAllCoursesQuery();
        Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int catId);
        Task<Course> GetCourseByIdAsync(int courseId);
    }
    public class CourseRepository: ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
	            .Where(c => c.IsPublished == true)
	            .Include(c => c.Owner)
	            .OrderBy(c => c.CreatedAt)
	            .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int catId)
        {
	        return await _context.Courses
		        .Where(c => c.IsPublished == true && c.CategoryId == catId)
				.Include(c => c.Owner)
		        .OrderBy(c => c.CreatedAt)
		        .ToListAsync();
        }

		public IQueryable<Course> GetAllCoursesQuery()
        {
            return _context.Courses.AsQueryable();
        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
        }
    }
}
