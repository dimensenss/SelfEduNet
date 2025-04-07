using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Models;

namespace SelfEduNet.Repositories
{
	public interface IStepRepository
	{
		Task<Step> GetStepByIdAsync(int stepId, string? userId);
		Task<List<Step>> GetStepsByLessonIdAsync(int lessonId, string? userId);
		Task<int> GetMaxOrderAsync(int lessonId);
		Task<bool> IsLastStepInLesson(int lessonId, int stepId);
		Task<bool> AddAsync(Step step);
		Task<bool> SaveAsync();
		public bool Update(Step step);
		public bool Save();
	}
	public class StepRepository(ApplicationDbContext context) : IStepRepository
	{
		private readonly ApplicationDbContext _context = context;

		public async Task<Step> GetStepByIdAsync(int stepId, string? userId)
		{
			var query = context.Steps.AsQueryable();

			if (userId != null)
			{
				query = query
					.Include(s => s.UserSteps)
					.Where(s => s.UserSteps.Any(us => us.UserId == userId));
			}

			return await query.FirstOrDefaultAsync(s => s.Id == stepId);
		}
		public async Task<List<Step>> GetStepsByLessonIdAsync(int lessonId, string? userId)
		{
			var query = context.Steps
				.Where(s => s.LessonId == lessonId)
				.OrderBy(s => s.Order)
				.AsQueryable();

			if (userId != null)
			{
				query = query
					.Include(s => s.UserSteps)
					.Where(s => s.UserSteps.Any(us => us.UserId == userId));
			}

			return await query.ToListAsync();
		}

		public async Task<int> GetMaxOrderAsync(int lessonId)
		{
			var maxOrder = await _context.Steps
				.Where(s => s.LessonId == lessonId)
				.Select(s => (int?)s.Order)
				.MaxAsync();

			return maxOrder ?? 0;
		}

		public async Task<bool> IsLastStepInLesson(int lessonId, int stepId)
		{
			var maxOrder = await GetMaxOrderAsync(lessonId);

			return await _context.Steps
				.AnyAsync(s => s.Id == stepId && s.Order == maxOrder);
		}

		public async Task<bool> AddAsync(Step step)
		{
			await _context.Steps.AddAsync(step);
			return await SaveAsync();
		}
		public bool Update(Step step)
		{
			_context.Update(step);
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
