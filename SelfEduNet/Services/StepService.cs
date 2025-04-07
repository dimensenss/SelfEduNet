using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using SelfEduNet.Models;
using SelfEduNet.Repositories;

namespace SelfEduNet.Services
{
	public interface IStepService
	{
		Task<Step> GetStepByIdAsync(int stepId, string? userId = null);
		Task<bool> CreateStepAsync(Step step);
		Task<bool> IsLastStepInLesson(int lessonId, int stepId);
		Task<List<Step>> GetStepsByLessonIdAsync(int lessonId, string? userId = null);
		Task<int> GetMaxOrderAsync(int lessonId);
		public bool Update(Step step);
	}

	public class StepService(IStepRepository stepRepository) : IStepService
	{
		private readonly IStepRepository _stepRepository = stepRepository;

		public async Task<Step> GetStepByIdAsync(int stepId, string? userId)
		{
			return await _stepRepository.GetStepByIdAsync(stepId, userId);
		}

		public async Task<bool> IsLastStepInLesson(int lessonId, int stepId)
		{
			return await _stepRepository.IsLastStepInLesson(lessonId, stepId);
		}

		public async Task<List<Step>> GetStepsByLessonIdAsync(int lessonId, string? userId)
		{
			return await _stepRepository.GetStepsByLessonIdAsync(lessonId, userId);
		}
		public async Task<int> GetMaxOrderAsync(int lessonId)
		{
			return await _stepRepository.GetMaxOrderAsync(lessonId);
		}
		public bool Update(Step step)
		{
			return _stepRepository.Update(step);
		}
		public async Task<bool> CreateStepAsync(Step step)
		{
			return await stepRepository.AddAsync(step);
		}
	}
}
