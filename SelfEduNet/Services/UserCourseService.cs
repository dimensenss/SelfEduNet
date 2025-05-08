using SelfEduNet.Models;
using SelfEduNet.Repositories;

namespace SelfEduNet.Services;

public interface IUserCourseService
{
	Task<UserCourse> GetOrCreateUserCourseAsync(string userId, int courseId);
	Task<bool> MarkCourseAsEnrolledAsync(string userId, int courseId);
}
public class UserCourseService(IUserCourseRepository userCourseRepository) : IUserCourseService
{
	private readonly IUserCourseRepository _userCourseRepository = userCourseRepository;

	public async Task<UserCourse> GetOrCreateUserCourseAsync(string userId, int courseId)
	{
		return await _userCourseRepository.GetOrCreateUserCourseAsync(userId, courseId);
	}

	public async Task<bool> MarkCourseAsEnrolledAsync(string userId, int courseId)
	{
		return await _userCourseRepository.MarkCourseAsEnrolledAsync(userId, courseId);
	}
}

