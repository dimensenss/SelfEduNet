using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Models;

namespace SelfEduNet.Repositories;

public interface IUserCourseRepository
{
	Task<UserCourse> GetOrCreateUserCourseAsync(string userId, int courseId);
	Task<bool> MarkCourseAsEnrolledAsync(string userId, int courseId);
	Task<bool> MarkCourseAsCompletedAsync(string userId, int courseId);
	Task<IEnumerable<UserCourse>> GetAllUserCoursesAsync(string userId);
	Task<IEnumerable<UserCourse>> GetWishedUserCoursesAsync(string userId);
	Task<IQueryable<UserCourse>> GetAllUserCoursesQueryAsync(string userId);
	Task<IEnumerable<UserCourse>> GetAllUserCoursesByNameAsync(string userId, string name);
	Task<Dictionary<UserCourse, CourseCompletedSteps>> GetUserCoursesWithCompletionAsync(string userId);
	Task<Dictionary<UserCourse, CourseCompletedSteps>> GetAllCompletedUserCoursesAsync(string userId);
	Task<UserCourse> GetLastUserCoursesAsync(string userId);

	bool Update(UserCourse userCourse);
	bool Save();
	Task<bool> SaveAsync();
}
public class UserCourseRepository(ApplicationDbContext context) : IUserCourseRepository
{
	private readonly ApplicationDbContext _context = context;

	public async Task<UserCourse> GetOrCreateUserCourseAsync(string userId, int courseId)
	{
		var userCourse = await _context.UserCourses
			.Include(s => s.Course)
			.FirstOrDefaultAsync(us => us.UserId == userId && us.CourseId == courseId);

		if (userCourse == null)
		{
			userCourse = new UserCourse
			{
				UserId = userId,
				CourseId = courseId,
			};

			_context.UserCourses.Add(userCourse);
			await SaveAsync();
		}

		return userCourse;
	}
	public async Task<bool> MarkCourseAsEnrolledAsync(string userId, int courseId)
	{
		var userCourse = await _context.UserCourses
			.FirstOrDefaultAsync(us => us.UserId == userId && us.CourseId == courseId);

		if (userCourse == null)
		{
			return false;
		}

		userCourse.IsEnrolled = true;
		userCourse.EnrolledAt = DateTime.UtcNow;

		return Update(userCourse);
	}

	public async Task<bool> MarkCourseAsCompletedAsync(string userId, int courseId)
	{
		var userCourse = await _context.UserCourses
			.FirstOrDefaultAsync(us => us.UserId == userId && us.CourseId == courseId);

		if (userCourse == null)
		{
			return false;
		}

		userCourse.IsCompleted = true;
		userCourse.CompletedAt = DateTime.UtcNow;

		return Update(userCourse);
	}

	public async Task<IEnumerable<UserCourse>> GetAllUserCoursesAsync(string userId)
	{
		return await _context.UserCourses
			.Where(us => us.UserId == userId && us.IsEnrolled == true && us.IsCompleted == false)
			.Include(c => c.Course)
			.ToListAsync();
	}

	public async Task<IEnumerable<UserCourse>> GetWishedUserCoursesAsync(string userId)
	{
		return await _context.UserCourses
			.Where(us => us.UserId == userId && us.IsWish == true)
			.Include(c => c.Course)
			.ThenInclude(c => c.Owner)
			.ToListAsync();
	}

	public async Task<IQueryable<UserCourse>> GetAllUserCoursesQueryAsync(string userId)
	{
		return _context.UserCourses
			.Where(c => c.UserId == userId && c.IsEnrolled == true);
	}

	public async Task<IEnumerable<UserCourse>> GetAllUserCoursesByNameAsync(string userId, string name)
	{
		return await _context.UserCourses
			.Where(us => us.UserId == userId 
			             && us.Course.CourseName.ToLower().Contains(name.ToLower())
			             && us.IsEnrolled == true)
			.Include(c => c.Course)
			.ToListAsync();
	}

	public async Task<Dictionary<UserCourse, CourseCompletedSteps>> GetUserCoursesWithCompletionAsync(string userId)
	{
		var userCoursesWithCompletion = await _context.UserCourses
			.Where(uc => uc.UserId == userId && uc.IsEnrolled == true)
			.Include(uc => uc.Course)
			.Select(uc => new
			{
				UserCourse = uc,
				TotalSteps = _context.Steps
					.Count(s => s.Lesson.CourseId == uc.CourseId),
				CompletedSteps = _context.UserSteps
					.Count(us => us.UserId == userId && us.Step.Lesson.CourseId == uc.CourseId && us.IsCompleted == true)
			})
			.OrderByDescending(uc => uc.CompletedSteps)
			.ToListAsync();

		return userCoursesWithCompletion.ToDictionary(
			x => x.UserCourse,
			x => new CourseCompletedSteps
			{
				TotalSteps = x.TotalSteps,
				CompletedSteps = x.CompletedSteps,
				CourseCompletionPercent = x.TotalSteps > 0 ? (x.CompletedSteps * 100) / x.TotalSteps : 0
			});
	}

	public async Task<Dictionary<UserCourse, CourseCompletedSteps>> GetAllCompletedUserCoursesAsync(string userId)
	{
		var userCourses = await GetUserCoursesWithCompletionAsync(userId);
		return userCourses
			.Where(c => c.Key.IsCompleted)
			.ToDictionary(
				c => c.Key,
				c => c.Value
			);
	}

	public async Task<UserCourse> GetLastUserCoursesAsync(string userId)
	{
		return await _context.UserCourses
			.Where(uc => uc.UserId == userId && uc.IsEnrolled == true)
			.Include(uc => uc.Course)
			.OrderByDescending(uc => uc.EnrolledAt)
			.FirstAsync();
	}

	public bool Update(UserCourse userCourse)
	{
		_context.Update(userCourse);
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

