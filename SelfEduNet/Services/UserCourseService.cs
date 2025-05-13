using Microsoft.Extensions.Caching.Memory;
using SelfEduNet.Models;
using SelfEduNet.Repositories;

namespace SelfEduNet.Services;

public interface IUserCourseService
{
	Task<UserCourse> GetOrCreateUserCourseAsync(string userId, int courseId);
	Task<IEnumerable<UserCourse>> GetAllUserCoursesAsync(string userId);
	Task<IQueryable<UserCourse>> GetAllUserCoursesQueryAsync(string userId);
	Task<IEnumerable<UserCourse>> GetAllUserCoursesByNameAsync(string userId, string name);
	Task<IEnumerable<UserCourse>> GetWishedUserCoursesAsync(string userId);
	Task<IEnumerable<UserCourse>> GetEnrolledUserCoursesAsync(int courseId);
	Task<Dictionary<UserCourse, CourseCompletedSteps>> GetAllUserCoursesWithCompletionAsync(string userId);
	Task<Dictionary<UserCourse, CourseCompletedSteps>> GetAllCompletedUserCoursesAsync(string userId);
	Task<UserCourse> GetLastUserCoursesAsync(string userId);
	Task<bool> MarkCourseAsEnrolledAsync(string userId, int courseId);
	Task<bool> MarkCourseAsCompletedAsync(string userId, int courseId);
	Task<bool> CreateReviewAsync(Review review);
	bool Update(UserCourse userCourse);

}
public class UserCourseService(IUserCourseRepository userCourseRepository, IMemoryCache cache) : IUserCourseService
{
	private readonly IUserCourseRepository _userCourseRepository = userCourseRepository;
	private readonly IMemoryCache _cache = cache;

	public async Task<UserCourse> GetOrCreateUserCourseAsync(string userId, int courseId)
	{
		return await _userCourseRepository.GetOrCreateUserCourseAsync(userId, courseId);
	}

	public async Task<IEnumerable<UserCourse>> GetAllUserCoursesAsync(string userId)
	{
		return await _userCourseRepository.GetAllUserCoursesAsync(userId);
	}

	public Task<IQueryable<UserCourse>> GetAllUserCoursesQueryAsync(string userId)
	{
		return _userCourseRepository.GetAllUserCoursesQueryAsync(userId);
	}

	public async Task<IEnumerable<UserCourse>> GetAllUserCoursesByNameAsync(string userId, string name)
	{
		return await _userCourseRepository.GetAllUserCoursesByNameAsync(userId, name);
	}

	public async Task<IEnumerable<UserCourse>> GetWishedUserCoursesAsync(string userId)
	{
		return await _userCourseRepository.GetWishedUserCoursesAsync(userId);
	}

	public async Task<IEnumerable<UserCourse>> GetEnrolledUserCoursesAsync(int courseId)
	{
		return await _userCourseRepository.GetEnrolledUserCoursesAsync(courseId);
	}

	public async Task<Dictionary<UserCourse, CourseCompletedSteps>> GetAllUserCoursesWithCompletionAsync(string userId)
	{
		return await _userCourseRepository.GetUserCoursesWithCompletionAsync(userId);
	}

	public async Task<Dictionary<UserCourse, CourseCompletedSteps>> GetAllCompletedUserCoursesAsync(string userId)
	{
		return await _userCourseRepository.GetAllCompletedUserCoursesAsync(userId);
	}

	public async Task<UserCourse> GetLastUserCoursesAsync(string userId)
	{
		return await _userCourseRepository.GetLastUserCoursesAsync(userId);
	}

	public async Task<bool> MarkCourseAsEnrolledAsync(string userId, int courseId)
	{
		var result = await _userCourseRepository.MarkCourseAsEnrolledAsync(userId, courseId);
		if (result)
		{
			string cacheKey = $"CourseEnrolledCount_{courseId}";
			_cache.Remove(cacheKey);
		}

		return result;
	}

	public async Task<bool> MarkCourseAsCompletedAsync(string userId, int courseId)
	{
		return await _userCourseRepository.MarkCourseAsCompletedAsync(userId, courseId);
	}

	public async Task<bool> CreateReviewAsync(Review review)
	{
		var result = await _userCourseRepository.CreateReviewAsync(review);
		if (result)
		{
			string rateCacheKey = $"CourseRate_{review.UserCourse.CourseId}";
			_cache.Remove(rateCacheKey);

			string reviewsCountCacheKey = $"CourseReviewsCount_{review.UserCourse.CourseId}";
			_cache.Remove(reviewsCountCacheKey);

		}

		return result;
	}

	public bool Update(UserCourse userCourse)
	{
		return _userCourseRepository.Update(userCourse);

	}
}

