using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Models;
using System.Linq;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Repositories
{
    public interface ICourseRepository
    {
	    Task<IEnumerable<Course>> GetAllCoursesAsync();
	    IQueryable<Course> GetAllCoursesQuery();
	    IQueryable<Course> GetAllCoursesByOwnerQuery(string userId);
        Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int catId);
        Task<CourseModules> GetModuleByIdAsync(int moduleId);
        Task<IEnumerable<CourseModules>> GetAllModulesByCourseId(int courseId);
        Task<Course> GetCourseWithModulesByIdAsync(int courseId);
		Task<Course?> GetCourseByIdAsync(int courseId);
		Task<Lesson?> GetLessonByIdAsync(int? lessonId, string? userId);
		Task<Lesson?> GetNextLessonAsync(int courseId, int lessonId);
		Task<List<Lesson>> GetLessonsByCourseIdAsync(int courseId, string? userId);
		LessonStatistics GetLessonStatisticsByIdAsync(List<Step> userSteps);
		CourseStatistics GetCourseStatisticsByLessons(List<Lesson> lessons);
		Task<Course?> GetCourseWithInfoByIdAsync(int courseId);
        Task<bool> IsCourseOwnedByUser(int courseId, string userId);
        Task<List<AppUser>> GetCourseAuthorsAsync(int courseId);
        Task<IEnumerable<Course>> GetCoursesByOwnerAsync(string userId);
        Task<bool> AddModuleAsync(CourseModules module);
        Task<bool> AddLessonAsync(Lesson lesson);
        Task<bool> AddAsync(Course course);
		bool Add(Course course);
        bool Update(Course course);
        Task<bool> UpdateModuleAsync(CourseModules module);
        Task<bool> UpdateLessonAsync(Lesson lesson);
        Task<int> GetMaxLessonOrderAsync(int moduleId);
        Task<int> GetMaxModuleOrderAsync(int courseId);
        Task<Course?> GetCourseWithCheckListAsync(int courseId);
		bool Delete(Course course);
		bool DeleteModule(CourseModules module);
		bool DeleteLesson(Lesson lesson);
        bool Save();
    }
    public class CourseRepository(ApplicationDbContext context) : ICourseRepository
    {
        private readonly ApplicationDbContext _context = context;

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
		        .Where(c => c.IsPublished == true 
		                    && c.CategoryId == catId)
				.Include(c => c.Owner)
		        .OrderBy(c => c.CreatedAt)
		        .ToListAsync();
        }

		public IQueryable<Course> GetAllCoursesQuery()
        {
			return _context.Courses
				.Include(c => c.Owner)
				.Include(c => c.Info)
				.ThenInclude(ci => ci.Authors) 
				.Where(c => c.IsPublished == true)
				.AsQueryable();
		}

        public IQueryable<Course> GetAllCoursesByOwnerQuery(string userId)
        {
	        return _context.Courses
		        .Where(c => c.OwnerId.Contains(userId));

        }
        public async Task<CourseModules> GetModuleByIdAsync(int moduleId)
        {
            return await _context.CourseModules.FirstOrDefaultAsync(c => c.Id == moduleId);
        }
        public async Task<IEnumerable<CourseModules>> GetAllModulesByCourseId(int courseId)
        {
	        return await _context.CourseModules
		        .Where(c => c.CourseId == courseId)
		        .ToListAsync();
		}
		public async Task<Course?> GetCourseByIdAsync(int courseId)
        {
            return await _context.Courses
	            .Include(c=>c.Info)
	            .Include(c=> c.Modules)
	            .ThenInclude(m=>m.Lessons)
	            .FirstOrDefaultAsync(c => c.Id == courseId);
        }
        public async Task<Lesson?> GetLessonByIdAsync(int? lessonId, string? userId)
        {
			if (lessonId == null)
				return null;

			var query = context.Lessons
				.AsQueryable();

			if (userId != null)
			{
				query = query
					.Include(l => l.UserLessons.Where(ul => ul.UserId == userId));
			}

			var lesson = await query
				.FirstOrDefaultAsync(l => l.Id == lessonId);

			return lesson;
		}

		public async Task<Lesson?> GetNextLessonAsync(int courseId, int lessonId)
		{
			var lessons = await _context.Lessons
				.Include(l => l.CourseModule)
				.Where(l => l.CourseModule!.CourseId == courseId)
				.OrderBy(l => l.CourseModule!.Order)
				.ThenBy(l => l.Order)
				.ToListAsync();

			var currentIndex = lessons.FindIndex(l => l.Id == lessonId);

			if (currentIndex == -1 || currentIndex + 1 >= lessons.Count)
				return null;

			return lessons[currentIndex + 1];
		}

		public LessonStatistics GetLessonStatisticsByIdAsync(List<Step> steps)
        {
	        var userSteps = steps.SelectMany(s => s.UserSteps).ToList();
			if (!userSteps.Any())
	        {
		        return new LessonStatistics
		        {
			        StartLessonTime = DateTime.MinValue,
			        EndLessonTime = DateTime.MinValue,
			        CompletedStepsCount = 0,
			        CompletedStepsPercentage = 0
		        };
	        }
	        var completedSteps = userSteps.Where(us => us.IsCompleted).ToList();
	        int totalSteps = userSteps.Count;
	        int completedStepsCount = completedSteps.Count;
	        int completedStepsPercentage = totalSteps > 0 ? (completedStepsCount * 100 / totalSteps) : 0;

	        return new LessonStatistics
	        {
		        StartLessonTime = userSteps.Min(us => us.ViewedAt) ?? DateTime.MinValue,
		        EndLessonTime = completedSteps.Any()
			        ? completedSteps.Max(us => us.CompletedAt) ?? DateTime.MinValue
			        : DateTime.MinValue,
		        CompletedStepsCount = completedStepsCount,
		        CompletedStepsPercentage = completedStepsPercentage
	        };
			//TODO save to db?
        }
        public async Task<List<Lesson>> GetLessonsByCourseIdAsync(int courseId, string? userId)
        {
	        var query = context.Lessons
		        .Where(s => s.CourseId == courseId)
		        .OrderBy(s => s.Order)
		        .AsQueryable();

	        if (userId != null)
	        {
		        query = query
			        .Include(s => s.UserLessons
				        .Where(us => us.UserId == userId))
			        .Where(s => s.UserLessons.Any(us => us.UserId == userId));
	        }

	        return await query.ToListAsync();
        }
		public CourseStatistics GetCourseStatisticsByLessons(List<Lesson> lessons)
        {
	        var userLessons = lessons.SelectMany(l => l.UserLessons).ToList();
	        if (!userLessons.Any())
	        {
		        return new CourseStatistics
				{
			        StartCourseTime = DateTime.MinValue,
			        EndCourseTime = DateTime.MinValue,
			        CompletedLessonsCount = 0,
			        CompletedLessonsPercentage = 0
		        };
	        }

	        var completedLessons = userLessons.Where(ul => ul.IsCompleted).ToList();
	        int totalLessons = userLessons.Count;
	        int completedLessonsCount = completedLessons.Count;
	        int completedLessonsPercentage = totalLessons > 0 ? (completedLessonsCount * 100 / totalLessons) : 0;

	        return new CourseStatistics
			{
		        StartCourseTime = userLessons.Min(ul => ul.StartedAt) ?? DateTime.MinValue,
		        EndCourseTime = completedLessons.Any()
			        ? completedLessons.Max(ul => ul.CompletedAt) ?? DateTime.MinValue
			        : DateTime.MinValue,
		        CompletedLessonsCount = completedLessonsCount,
		        CompletedLessonsPercentage = completedLessonsPercentage
	        };
	        //TODO save to db
		}
		public async Task<Course?> GetCourseWithInfoByIdAsync(int courseId)
        {
			return await _context.Courses
				.Include(c => c.Info)
				.ThenInclude(a => a.Authors)
				.Include(c => c.Modules)  // Загружаем связанные модули
				.ThenInclude(m => m.Lessons)  // Загружаем уроки в этих модулях
				.Include(c => c.Category)
				.FirstOrDefaultAsync(c => c.Id == courseId);
		}
        public async Task<Course> GetCourseWithModulesByIdAsync(int courseId)
        {
	        var course = await _context.Courses
		        .Include(c => c.Modules)
		        .ThenInclude(m => m.Lessons)
		        .FirstOrDefaultAsync(c => c.Id == courseId);

	        if (course != null)
	        {
		        course.Modules = course.Modules.OrderBy(m => m.Order).ToList();

		        foreach (var module in course.Modules)
		        {
			        module.Lessons = module.Lessons.OrderBy(l => l.Title).ToList();
		        }
	        }

	        return course;
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

        public async Task<bool> AddModuleAsync(CourseModules module)
        {
            await _context.CourseModules.AddAsync(module);
            return await SaveAsync();
        }

        public async Task<bool> AddLessonAsync(Lesson lesson)
        {
			await _context.Lessons.AddAsync(lesson);
			return await SaveAsync();
		}

        public async Task<bool> AddAsync(Course course)
		{
			await _context.Courses.AddAsync(course);
			return await SaveAsync();
		}

		public bool Add(Course course)
        {
	        _context.Courses.Add(course);
	        return Save();
        }

        public async Task<int> GetMaxLessonOrderAsync(int moduleId)
        {
	        var maxOrder = await _context.Lessons
		        .Where(l => l.CourseModuleId == moduleId)
		        .Select(s => (int?)s.Order)
		        .MaxAsync();

	        return maxOrder ?? 0;
		}

        public async Task<int> GetMaxModuleOrderAsync(int courseId)
        {
			var maxOrder = await _context.CourseModules
				.Where(m => m.CourseId == courseId)
				.Select(m => (int?)m.Order)
				.MaxAsync();

			return maxOrder ?? 0;
		}

        public async Task<Course?> GetCourseWithCheckListAsync(int courseId)
        {
			try
			{
				return await _context.Courses
					.Include(c => c.Modules)
					.ThenInclude(m => m.Lessons)
					.ThenInclude(l => l.Steps)
					.ThenInclude(s => s.StepTest)
					.Include(c => c.Category)
					.Include(c => c.Info)
					.FirstOrDefaultAsync(c => c.Id == courseId);
			}
			catch (Exception ex)
			{
				return null;
			}
		}

        public bool Delete(Course course)
        {
	        _context.Courses.Remove(course);
	        return Save();
        }
        public bool DeleteModule(CourseModules module)
        {
	        _context.CourseModules.Remove(module);
	        return Save();
        }
		public bool DeleteLesson(Lesson lesson)
        {
            _context.Lessons.Remove(lesson);
            return Save();
		}

		public async Task<bool> UpdateModuleAsync(CourseModules module)
        {
	        _context.Update(module);
			return await SaveAsync();
		}
        public async Task<bool> UpdateLessonAsync(Lesson lesson)
        {
	        _context.Update(lesson);
	        return await SaveAsync();
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

        public async Task<bool> SaveAsync()
        {
			return await _context.SaveChangesAsync() > 0;
		}
	}
}
