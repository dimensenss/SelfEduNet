using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Models;

namespace SelfEduNet.Repositories
{
	public interface IUserStepRepository
	{
		Task<bool> StepExistsAsync(string userId, int stepId);
		Task<bool> MarkStepAsCompletedAsync(string userId, int stepId);
		Task<bool> MarkStepAsViewedAsync(string userId, int stepId);
		Task<bool> CheckViewedStepAsync(string userId, int stepId);
		Task<UserStep> GetOrCreateUserStepAsync(string userId, int stepId);
		Task<bool> AddUserTestResultAsync(UserTestResult userTestResult);
		Task<bool> AddAsync(UserStep step);
		Task<bool> SaveAsync();
		bool Update(UserStep step);
		bool Save();
	}
	public class UserStepRepository(ApplicationDbContext context) : IUserStepRepository
	{
		private readonly ApplicationDbContext _context = context;


		public async Task<bool> StepExistsAsync(string userId, int stepId)
		{
			return await _context.UserSteps.AnyAsync(s => s.StepId == stepId && s.UserId == userId);
		}

		public async Task<bool> MarkStepAsCompletedAsync(string userId, int stepId)
		{
			var userStep = await _context.UserSteps
				.FirstOrDefaultAsync(us => us.UserId == userId && us.StepId == stepId);
			if (userStep == null)
			{
				return false;
			}

			userStep.IsViewed = true;
			userStep.IsCompleted = true;
			userStep.CompletedAt = DateTime.UtcNow;
			return Update(userStep);
		}

		public async Task<bool> MarkStepAsViewedAsync(string userId, int stepId)
		{
			var userStep = await _context.UserSteps
				.FirstOrDefaultAsync(us => us.UserId == userId && us.StepId == stepId);
			if (userStep == null)
			{
				return false;
			}

			userStep.IsViewed = true;
			userStep.ViewedAt = DateTime.UtcNow;

			return Update(userStep);
		}

		public async Task<bool> CheckViewedStepAsync(string userId, int stepId)
		{
			var userStep = await _context.UserSteps
				.FirstOrDefaultAsync(us => us.UserId == userId && us.StepId == stepId);
			if (userStep == null)
			{
				return false;
			}

			return userStep.IsViewed ? true : false;
		}

		public async Task<UserStep> GetOrCreateUserStepAsync(string userId, int stepId)
		{
			var userStep = await _context.UserSteps
				.FirstOrDefaultAsync(us => us.UserId == userId && us.StepId == stepId);

			if (userStep == null)
			{
				userStep = new UserStep
				{
					UserId = userId,
					StepId = stepId,
					IsCompleted = false,
					ViewedAt = DateTime.UtcNow,
					IsViewed = false
				};

				_context.UserSteps.Add(userStep);
				await SaveAsync();
			}

			return userStep;
		}

		public async Task<bool> AddUserTestResultAsync(UserTestResult userTestResult)
		{
			await _context.UserTestResults.AddAsync(userTestResult);
			return await _context.SaveChangesAsync() > 0;
		}

		public Task<bool> AddAsync(UserStep step)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> SaveAsync()
		{
			return await _context.SaveChangesAsync() > 0;
		}

		public bool Update(UserStep userStep)
		{
			_context.Update(userStep);
			return Save();
		}

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}
	}
}
