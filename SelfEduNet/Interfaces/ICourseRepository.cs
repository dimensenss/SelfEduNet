using SelfEduNet.Models;

namespace SelfEduNet.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        IQueryable<Course> GetAllCoursesQuery();
        Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int catId);


    }
}
