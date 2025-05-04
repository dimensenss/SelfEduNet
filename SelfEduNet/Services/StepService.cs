using CloudinaryDotNet;
using EduProject.Services;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using SelfEduNet.Data.Enum;
using SelfEduNet.Data.Regex;
using SelfEduNet.Models;
using SelfEduNet.Repositories;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Services
{
	public interface IStepService
	{
		Task<Step> GetStepByIdAsync(int stepId, string? userId = null);
		Task<bool> CreateStepAsync(Step step);
		Task<bool> IsLastStepInLesson(int lessonId, int stepId);
		Task<List<Step>> GetStepsByLessonIdAsync(int lessonId, string? userId = null);
		Task<int> GetMaxOrderAsync(int lessonId);
		Task<bool> CreateStepTestAsync(StepTest stepTest, int stepId);
		Task<bool> DeleteStep(Step step);
		public bool Update(Step step);
	}

	public class StepService(IStepRepository stepRepository, IPhotoService photoService) : IStepService
	{
		private readonly IStepRepository _stepRepository = stepRepository;
		private readonly IPhotoService _photoService = photoService;

		public async Task<Step> GetStepByIdAsync(int stepId, string? userId = null)
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

		public async Task<bool> CreateStepTestAsync(StepTest stepTest, int stepId)
		{
			stepTest.StepId = stepId;

			var result = await _stepRepository.AddStepTestAsync(stepTest);

			if (!result)
				return false;

			var step = await _stepRepository.GetStepByIdAsync(stepId, null);
			step.StepTest = stepTest;

			return _stepRepository.Update(step);
		}

		public async Task<bool> DeleteStep(Step step)
		{
			switch (step.StepType)
			{
				case StepType.Text:
					var imageUrls = _photoService.ExtractImageUrls(step.Content);
					var oldImageUrls = _photoService.ExtractImageUrls(step.Content ?? "");
					foreach (var oldImageUrl in oldImageUrls.Except(imageUrls))
					{
						try
						{
							var fileInfo = new FileInfo(oldImageUrl);
							string publicId = Path.GetFileNameWithoutExtension(fileInfo.Name);
							await _photoService.DeleteFileAsync(publicId);
						}
						catch
						{
							//TODO log
						}
					}
					break;
				case StepType.Video:
					if (step.VideoUrl != null && !CommonRegex.YoutubeRegex.IsMatch(step.VideoUrl))
					{
						try
						{
							var fileInfo = new FileInfo(step.VideoUrl);
							string publicId = Path.GetFileNameWithoutExtension(fileInfo.Name);
							await _photoService.DeleteFileAsync(publicId);
						}
						catch
						{
							//TODO log
						}
					}
					break;
			}
			
			return await _stepRepository.DeleteStep(step);
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
