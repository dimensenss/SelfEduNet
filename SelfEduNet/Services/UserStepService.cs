using SelfEduNet.Models;
using SelfEduNet.Repositories;

namespace SelfEduNet.Services
{
	public interface IUserStepService
	{
		public Task<bool> StepExistsAsync(string userId, int stepId);
		public Task<bool> MarkStepAsCompletedAsync(string userId, int stepId);
		public Task<bool> MarkStepAsViewedAsync(string userId, int stepId);
		public Task<bool> CheckViewedStepAsync(string userId, int stepId);
		public Task<UserStep> GetOrCreateUserStepAsync(string userId, int stepId);
		public bool Update(UserStep userStep);
	}

	public class UserStepService(IUserStepRepository userStepRepository) : IUserStepService
	{
		private readonly IUserStepRepository _userStepRepository = userStepRepository;

		public async Task<bool> StepExistsAsync(string userId, int stepId)
		{
			return await _userStepRepository.StepExistsAsync(userId, stepId);
		}
		public async Task<bool> MarkStepAsCompletedAsync(string userId, int stepId)
		{
			return await _userStepRepository.MarkStepAsCompletedAsync(userId, stepId);
		}
		public async Task<bool> MarkStepAsViewedAsync(string userId, int stepId)
		{
			return await _userStepRepository.MarkStepAsViewedAsync(userId, stepId);
		}

		public async Task<bool> CheckViewedStepAsync(string userId, int stepId)
		{
			return await _userStepRepository.CheckViewedStepAsync(userId, stepId);
		}

		public async Task<UserStep> GetOrCreateUserStepAsync(string userId, int stepId)
		{
			return await _userStepRepository.GetOrCreateUserStepAsync(userId, stepId);
		}

		public bool Update(UserStep userStep)
		{
			return _userStepRepository.Update(userStep);
		}
	}
}
