using SelfEduNet.Data.Enum;
using SelfEduNet.Models;
using SelfEduNet.Repositories;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Services
{
	public interface IUserStepService
	{
		public Task<bool> StepExistsAsync(string userId, int stepId);
		public Task<bool> MarkStepAsCompletedAsync(string userId, int stepId);
		public Task<bool> MarkStepAsViewedAsync(string userId, int stepId);
		public Task<bool> CheckViewedStepAsync(string userId, int stepId);
		public Task<UserStep> GetOrCreateUserStepAsync(string userId, int stepId);
		Task<bool> CreateOrUpdateUserTestResultAsync(StepTestResult stepTestResult, int stepId, string userId);

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
			var userStep = await GetOrCreateUserStepAsync(userId, stepId);
			if (userStep.Step is { StepType: StepType.Test, StepTest: not null })
			{
				if (userStep.UserTestResult == null || !userStep.UserTestResult.IsPassed)
				{
					return false;
				}
			}
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
		public async Task<bool> CreateOrUpdateUserTestResultAsync(StepTestResult stepTestResult, int stepId, string userId)
		{
			var userStep = await GetOrCreateUserStepAsync(userId, stepId);
			if (userStep == null)
				return false;

			var existingResult = await _userStepRepository.GetUserTestResultAsync(userId, stepId);

			if (existingResult != null)
			{
				// Обновляем существующий результат
				existingResult.Score = stepTestResult.Score;
				existingResult.TotalScore = stepTestResult.TotalScore;
				existingResult.Timestamp = stepTestResult.Timestamp;
				existingResult.AttemptsCount = stepTestResult.AttemptsCount;
				existingResult.BiggestScore = stepTestResult.BiggestScore;
				existingResult.IsPassed = stepTestResult.IsPassed;

				return _userStepRepository.UpdateUserTestResultAsync(existingResult);
			}
			else
			{
				// Создаем новый результат
				var userTestResult = new UserTestResult()
				{
					StepTest = userStep.Step.StepTest,
					StepTestId = userStep.Step.StepTest.Id,
					UserStep = userStep,
					UserStepId = userStep.Id,
					UserId = userId,
					StepId = stepId,
					Score = stepTestResult.Score,
					TotalScore = stepTestResult.TotalScore,
					Timestamp = stepTestResult.Timestamp,
					AttemptsCount = stepTestResult.AttemptsCount,
					BiggestScore = stepTestResult.BiggestScore,
					IsPassed = stepTestResult.IsPassed,
				};

				var result = await _userStepRepository.AddUserTestResultAsync(userTestResult);
				if (!result)
					return false;

				userStep.UserTestResult = userTestResult;
				return _userStepRepository.Update(userStep);
			}
		}

		public bool Update(UserStep userStep)
		{
			return _userStepRepository.Update(userStep);
		}

		
	}
}
