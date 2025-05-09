﻿using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Models;
using System.Linq;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Repositories
{
    public interface ICourseRepository
    {
	    public Task<IEnumerable<Course>> GetAllCoursesAsync();
	    public IQueryable<Course> GetAllCoursesQuery();
	    public IQueryable<Course> GetAllCoursesByOwnerQuery(string userId);
        public Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int catId);
        public Task<CourseModules> GetModuleByIdAsync(int moduleId);
        public Task<IEnumerable<CourseModules>> GetAllModulesByCourseId(int courseId);
        public Task<Course> GetCourseWithModulesByIdAsync(int courseId);
		public Task<Course?> GetCourseByIdAsync(int courseId);
		public Task<Lesson?> GetLessonByIdAsync(int? lessonId, string? userId);
		public Task<Lesson?> GetNextLessonAsync(int courseId, int lessonId);
		public LessonStatistics GetLessonStatisticsByIdAsync(List<Step> userSteps);
		public Task<Course?> GetCourseWithInfoByIdAsync(int courseId);
        public Task<bool> IsCourseOwnedByUser(int courseId, string userId);
        public Task<List<AppUser>> GetCourseAuthorsAsync(int courseId);
        public Task<IEnumerable<Course>> GetCoursesByOwnerAsync(string userId);
        public Task<bool> AddModuleAsync(CourseModules module);
        public Task<bool> AddLessonAsync(Lesson lesson);
        public Task<bool> AddAsync(Course course);
		public bool Add(Course course);
        public bool Update(Course course);
        public Task<bool> UpdateModuleAsync(CourseModules module);
        public Task<bool> UpdateLessonAsync(Lesson lesson);
        public Task<int> GetMaxLessonOrderAsync(int moduleId);
        public Task<int> GetMaxModuleOrderAsync(int courseId);
		public bool Delete(Course course);
		public bool DeleteModule(CourseModules module);
		public bool DeleteLesson(Lesson lesson);
        public bool Save();
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
