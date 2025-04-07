using SelfEduNet.Models;
using SelfEduNet.Repositories;

namespace SelfEduNet.Services;

public interface IUserLessonService
{
	public Task<bool> LessonExistsAsync(string userId, int lessonId);
	public Task<bool> MarkLessonAsCompletedAsync(string userId, int lessonId);
	public Task<UserLesson> GetOrCreateUserLessonAsync(string userId, int lessonId);
	public bool Update(Lesson userLesson);
}

public class UserLessonService(IUserLessonRepository userLessonRepository) : IUserLessonService
{
	private readonly IUserLessonRepository _userLessonRepository = userLessonRepository;

	public Task<bool> LessonExistsAsync(string userId, int lessonId)
	{
		throw new NotImplementedException();
	}

	public async Task<bool> MarkLessonAsCompletedAsync(string userId, int lessonId)
	{
		return await _userLessonRepository.MarkLessonAsCompletedAsync(userId, lessonId);
	}

	public async Task<UserLesson> GetOrCreateUserLessonAsync(string userId, int lessonId)
	{
		return await _userLessonRepository.GetOrCreateUserLessonAsync(userId, lessonId);
	}

	public bool Update(Lesson userLesson)
	{
		throw new NotImplementedException();
	}
}
