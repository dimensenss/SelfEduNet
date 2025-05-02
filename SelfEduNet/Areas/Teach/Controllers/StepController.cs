using System.Reflection.Metadata.Ecma335;
using CloudinaryDotNet;
using EduProject.Services;
using Ganss.Xss;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using SelfEduNet.Services;
using System.Text.RegularExpressions;
using SelfEduNet.Data.Regex;
using System.Threading.Tasks;
using SelfEduNet.Data.Enum;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Areas.Teach.Controllers
{
	[Area("Teach")]
	public class StepController(IStepService stepService, IPhotoService photoService, IHtmlSanitizer sanitizer, 
		IUserStepService userStepService, ITranscriptionService transcriptionService) : Controller
	{
		private readonly IStepService _stepService = stepService;
		private readonly IPhotoService _photoService = photoService;
		private readonly IHtmlSanitizer _sanitizer = sanitizer;
		private readonly IUserStepService _userStepService = userStepService;
		private readonly ITranscriptionService _transcriptionService = transcriptionService;

		[HttpPost]
		public async Task<IActionResult> UpdateStepContent(int id, [FromBody] string content)
		{
			if (content == null)
			{
				return BadRequest();
			}
			var step = await _stepService.GetStepByIdAsync(id, null);
			if (step == null)
			{
				return NotFound(new { message = "Крок не знайдено" });
			}
			//var sanitizedContent = _sanitizer.Sanitize(content);

			var imageUrls = _photoService.ExtractImageUrls(content);
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
					// Логируем ошибку или игнорируем
				}
			}

			step.Content = content;
			step.UpdatedAt = DateTime.UtcNow;
			bool result = _stepService.Update(step);

			return result
				? Ok()
				: BadRequest();
		}

		[HttpPost]
		public async Task<IActionResult> UpdateStepResume(int id, [FromBody] string content)
		{
			if (content == null)
			{
				return BadRequest();
			}
			var step = await _stepService.GetStepByIdAsync(id, null);
			if (step == null)
			{
				return NotFound(new { message = "Крок не знайдено" });
			}
			//var sanitizedContent = _sanitizer.Sanitize(content);

			step.Content = content;
			step.UpdatedAt = DateTime.UtcNow;
			bool result = _stepService.Update(step);

			return result
				? Ok()
				: BadRequest();
		}

		[HttpPost]
		public async Task<IActionResult> UploadImage(IFormFile upload)
		{
			if (upload == null || upload.Length == 0)
			{
				return BadRequest(new { message = "Файл не завантажено." });
			}

			try
			{
				// Викликаємо ваш метод для завантаження зображення
				var uploadResult = await _photoService.AddPhotoAsync(upload);

				if (uploadResult != null && !string.IsNullOrEmpty(uploadResult.Url?.ToString()))
				{
					return Json(new { uploaded = true, url = uploadResult.Url.ToString() });
				}
				else
				{
					return Json(new { uploaded = false, error = new { message = "Помилка при завантаженні зображення." } });
				}
			}
			catch (Exception ex)
			{
				return Json(new { uploaded = false, error = new { message = $"Помилка завантаження файлу: {ex.Message}" } });
			}
		}

		public async Task<IActionResult> UploadVideo(int id, IFormFile? videoFile, string? videoUrl)
		{
			var step = await _stepService.GetStepByIdAsync(id, null);
			if (step == null)
			{
				return NotFound(new { message = "Крок не знайдено" });
			}

			if (videoFile == null && string.IsNullOrEmpty(videoUrl))
			{
				return BadRequest(new { uploaded = false, message = "Необхідно завантажити файл або вказати посилання." });
			}

			try
			{
				string? videoUrlResult = null;

				//delete existing video
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

				//file upload
				if (videoFile is { Length: > 0 })
				{
					var uploadResult = await _photoService.AddVideoAsync(videoFile);

					if (uploadResult != null && !string.IsNullOrEmpty(uploadResult.Url?.ToString()))
					{
						videoUrlResult = uploadResult.Url.ToString();
					}
					else
					{
						return BadRequest(new { uploaded = false, message = "Помилка при завантаженні файлу." });
					}
				}

				//url upload
				else if (!string.IsNullOrEmpty(videoUrl))
				{
					if (!CommonRegex.YoutubeRegex.IsMatch(videoUrl))
					{
						return BadRequest(new { uploaded = false, message = "Некоректна URL-адреса" });
					}

					videoUrlResult = videoUrl;
				}

				step.VideoUrl = videoUrlResult;
				step.UpdatedAt = DateTime.UtcNow;
				bool result = _stepService.Update(step);

				return result
					? Ok(new { message = "Відео прив'язано", videoUrl = step.VideoUrl })
					: BadRequest(new { message = "Помилка завантаження файлу" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { uploaded = false, message = "Помилка завантаження файлу: {ex.Message}" });
			}

			return BadRequest(new { uploaded = false, message = "Невідома помилка." });
		}
		public async Task<IActionResult> GenerateContext(int id)
		{
			var step = await _stepService.GetStepByIdAsync(id, null);
			
			if (step == null)
			{
				return NotFound(new { message = "Крок не знайдено" });
			}

			string url = step.VideoUrl;

			if (url == null || url.Length < 0)
			{
				return BadRequest(new { message = "Відео не знайдено" });
			}

			if (CommonRegex.YoutubeRegex.IsMatch(url) && !User.IsInRole("Admin"))
			{
				return BadRequest(new { message = "Неможливо згенерувати контекст для цього відео" });
			}
			try
			{
				string taskId = await _transcriptionService.AddURLToQueue(url);

				return Ok(new { taskId = taskId, message = "Запит на контекст створено" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		public async Task<IActionResult> GetContent(int id, string taskId, WorkerTaskType keyType)
		{

			if (taskId.Length < 0)
			{
				return BadRequest(new { message = "Помилка при отриманні контексту" });
			}
			try
			{
				WorkerResult result = await _transcriptionService.GetContentByTaskId(taskId, keyType);

				if (result.IsEnd)
				{
					var step = await _stepService.GetStepByIdAsync(id, null);
					if (step == null)
					{
						return NotFound(new { message = "Крок не знайдено" });
					}
					string queueKey = keyType switch
					{
						WorkerTaskType.Transcription => step.Context = result.Content,
						WorkerTaskType.Resume => step.Resume = result.Content,
						_ => throw new ArgumentException("Invalid WorkerTaskType")
					};
					
					step.UpdatedAt = DateTime.UtcNow;
					_stepService.Update(step);
				}
				return result.Content.Length > 0
					? Ok(new { isSuccess = true, isEnd = result.IsEnd, content = result.Content })
					: BadRequest(new { isSuccess = false, message = "Помилка при отриманні контексту" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
		public async Task<IActionResult> GenerateResume(int id)
		{
			var step = await _stepService.GetStepByIdAsync(id, null);

			if (step == null)
			{
				return NotFound(new { message = "Крок не знайдено" });
			}

			string context = step.Context;

			if (string.IsNullOrWhiteSpace(context))
			{
				return BadRequest(new { message = "Контекст не знайдено." });
			}
			
			try
			{
				string taskId = await _transcriptionService.AddResumeRequestToQueue(context);

				return Ok(new { taskId = taskId, message = "Запит на резюме створено" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	
		public async Task<IActionResult> SubmitStep(int id)
		{
			string userId = User.GetUserId();

			if (!await _userStepService.StepExistsAsync(userId, id))
			{
				return NotFound(new { message = "Крок не знайдено" });
			}

			var result = await _userStepService.MarkStepAsCompletedAsync(userId, id);
			return result
				? Ok(new { message = "Крок пройдено" })
				: BadRequest(new { message = "Помилка при проходженні кроку" });
		}

		public async Task<IActionResult> SetViewedStep(int id)
		{
			string userId = User.GetUserId();

			if (!await _userStepService.StepExistsAsync(userId, id))
			{
				return NotFound(new { message = "Крок не знайдено" });
			}

			var result = await _userStepService.MarkStepAsViewedAsync(userId, id);
			return result
				? Ok(new { message = "Крок переглянуто" })
				: BadRequest(new { message = "Помилка при перегляді кроку" });
		}
		public async Task<IActionResult> CheckViewedStep(int id)
		{
			string userId = User.GetUserId();

			if (!await _userStepService.StepExistsAsync(userId, id))
			{
				return NotFound(new { message = "Крок не знайдено" });
			}

			var result = await _userStepService.CheckViewedStepAsync(userId, id);
			return result
				? Ok(new { message = "Крок переглянуто" })
				: BadRequest(new { message = "Помилка при перегляді кроку" });
		}

		public async Task<IActionResult> SaveStep(EditStepViewModel stepVM)
		{
			var step = await _stepService.GetStepByIdAsync(stepVM.Id);

			if (step == null)
			{
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				TempData["NotifyType"] = "danger";
				TempData["NotifyMessage"] = "Невдалося зберегти крок";

				return Redirect($"/EditLesson/{step.Lesson.CourseId}/{step.LessonId}?stepId={step.Id}");
			}

			if (!string.IsNullOrWhiteSpace(stepVM.Content))
			{
				step.Content = stepVM.Content;
			}

			if (!string.IsNullOrWhiteSpace(stepVM.Resume))
			{
				step.Resume = stepVM.Resume;
			}

			if (!string.IsNullOrWhiteSpace(stepVM.Context))
			{
				step.Context = stepVM.Context;
			}

			if (!string.IsNullOrWhiteSpace(stepVM.VideoUrl))
			{
				step.VideoUrl = stepVM.VideoUrl;
			}

			_stepService.Update(step);
			TempData["NotifyType"] = "success";
			TempData["NotifyMessage"] = "Дані збережено";

			return Redirect($"/EditLesson/{step.Lesson.CourseId}/{step.LessonId}?stepId={step.Id}");
		}
	}
}
