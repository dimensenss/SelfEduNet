using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Models;
using System.Linq;

namespace SelfEduNet.Repositories
{
    public interface ICourseRepository
    {
	    public Task<IEnumerable<Course>> GetAllCoursesAsync();
	    public IQueryable<Course> GetAllCoursesQuery();
	    public IQueryable<Course> GetAllCoursesByOwnerQuery(string userId);
        public Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int catId);
        public Task<Course> GetCourseByIdAsync(int courseId);
        public Task<Course> GetCourseWithInfoByIdAsync(int courseId);
        public Task<bool> IsCourseOwnedByUser(int courseId, string userId);
        public Task<List<AppUser>> GetCourseAuthorsAsync(int courseId);
        public Task<IEnumerable<Course>> GetCoursesByOwnerAsync(string userId);

        public bool Update(Course course);
		public bool Delete(Course course);
        public bool Save();


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

        public IQueryable<Course> GetAllCoursesByOwnerQuery(string userId)
        {
			return _context.Courses
				.Where(c => c.OwnerId.Contains(userId))
				.ToList()
				.AsQueryable();
        }

		public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<Course> GetCourseWithInfoByIdAsync(int courseId)
        {
            return await _context.Courses
                .Include(c => c.Info)
                .ThenInclude(a => a.Authors)
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<IEnumerable<Course>> GetCoursesByOwnerAsync(string userId)
        {
            return await _context.Courses
                .Where(c => c.OwnerId.Contains(userId))
                .ToListAsync();
        }

        public async Task<bool> IsCourseOwnedByUser(int courseId, string userId)
        {
			var course = await GetCourseWithInfoByIdAsync(courseId);
			return course.Info.Authors.Any(author => author.Id == userId) ;
		}

        public async Task<List<AppUser>> GetCourseAuthorsAsync(int courseId)
        {
            return await _context.Courses
	            .Where(c => c.Id == courseId)
	            .SelectMany(c => c.Info.Authors)
	            .ToListAsync();
        }

        public bool Delete(Course course)
        {
	        _context.Courses.Remove(course);
	        return Save();
        }
		public bool Update(Course course)
        {
            _context.Update(course);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
