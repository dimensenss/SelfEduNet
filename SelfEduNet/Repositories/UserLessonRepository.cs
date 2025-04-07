using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Models;

namespace SelfEduNet.Repositories;

public interface IUserLessonRepository
{
	public Task<bool> LessonExistsAsync(string userId, int lessonId);
	public Task<bool> MarkLessonAsCompletedAsync(string userId, int lessonId);
	public Task<UserLesson> GetOrCreateUserLessonAsync(string userId, int lessonId);
	Task<bool> AddAsync(UserLesson step);
	Task<bool> SaveAsync();
	public bool Update(UserLesson step);
	public bool Save();
}

public class UserLessonRepository(ApplicationDbContext context) : IUserLessonRepository
{
	private readonly ApplicationDbContext _context = context;
	public async Task<bool> LessonExistsAsync(string userId, int lessonId)
	{
		return await _context.UserLessons.AnyAsync(s => s.LessonId == lessonId && s.UserId == userId);
	}

	public async Task<bool> MarkLessonAsCompletedAsync(string userId, int lessonId)
	{
		var userLesson = await _context.UserLessons
			.FirstOrDefaultAsync(us => us.UserId == userId && us.LessonId == lessonId);
		if (userLesson == null)
		{
			return false;
		}

		userLesson.IsCompleted = true;
		userLesson.CompletedAt = DateTime.UtcNow;

		return Update(userLesson);
	}

	public async Task<UserLesson> GetOrCreateUserLessonAsync(string userId, int lessonId)
	{
		var userLesson = await _context.UserLessons
			.FirstOrDefaultAsync(us => us.UserId == userId && us.LessonId == lessonId);

		if (userLesson == null)
		{
			userLesson = new UserLesson()
			{
				UserId = userId,
				LessonId = lessonId,
				IsCompleted = false,
				StartedAt = DateTime.UtcNow
			};

			_context.UserLessons.Add(userLesson);
			await SaveAsync();
		}

		return userLesson;
	}

	public Task<bool> AddAsync(UserLesson lesson)
	{
		throw new NotImplementedException();
	}

	public async Task<bool> SaveAsync()
	{
		return await _context.SaveChangesAsync() > 0;
	}

	public bool Update(UserLesson userLesson)
	{
		_context.Update(userLesson);
		return Save();
	}

	public bool Save()
	{
		var saved = _context.SaveChanges();
		return saved > 0 ? true : false;
	}
}