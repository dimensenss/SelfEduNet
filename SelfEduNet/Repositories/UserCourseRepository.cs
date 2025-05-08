using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Models;

namespace SelfEduNet.Repositories;

public interface IUserCourseRepository
{
	Task<UserCourse> GetOrCreateUserCourseAsync(string userId, int courseId);
	Task<bool> MarkCourseAsEnrolledAsync(string userId, int courseId);
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

		return Update(userCourse);
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

