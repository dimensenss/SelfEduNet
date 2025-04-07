using EduProject.Services;
using Ganss.Xss;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using SelfEduNet.Services;
using System.Text.RegularExpressions;

namespace SelfEduNet.Areas.Teach.Controllers
{
	[Area("Teach")]
	public class StepController(IStepService stepService, IPhotoService photoService, IHtmlSanitizer sanitizer, IUserStepService userStepService) : Controller
	{
		private readonly IStepService _stepService = stepService;
		private readonly IPhotoService _photoService = photoService;
		private readonly IHtmlSanitizer _sanitizer = sanitizer;
		private readonly IUserStepService _userStepService = userStepService;


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

		public async Task<IActionResult> UploadVideo(int id, IFormFile? upload, string? videoUrl)
		{
			var step = await _stepService.GetStepByIdAsync(id, null);
			if (step == null)
			{
				return NotFound(new { message = "Крок не знайдено" });
			}

			if (upload == null && string.IsNullOrEmpty(videoUrl))
			{
				return BadRequest(new { uploaded = false, message = "Необхідно завантажити файл або вказати посилання." });
			}

			try
			{
				string? videoUrlResult = null;
				if (upload != null && upload.Length > 0)
				{
					var uploadResult = await _photoService.AddVideoAsync(upload);

					if (uploadResult != null && !string.IsNullOrEmpty(uploadResult.Url?.ToString()))
					{
						videoUrlResult = uploadResult.Url.ToString();
					}
					else
					{
						return BadRequest(new { uploaded = false, message = "Помилка при завантаженні файлу." });
					}
				}

				if (!string.IsNullOrEmpty(videoUrl))
				{
					var youtubeRegex = new Regex(@"^(https?://)?(www\.)?(youtube|youtu|youtube-nocookie)\.(com|be|fr|co\.uk)/.*$", RegexOptions.IgnoreCase);

					if (!youtubeRegex.IsMatch(videoUrl))
					{
						return BadRequest(new { uploaded = false, message = "Некоректна URL-адреса" });
					}

					videoUrlResult = videoUrl;
				}

				step.VideoUrl = videoUrlResult;
				step.UpdatedAt = DateTime.UtcNow;
				bool result = _stepService.Update(step);

				return result
					? Ok(new { message = "Відео прив'язано" })
					: BadRequest(new { message = "Помилка завантаження файлу" });
			}
			catch (Exception ex)
			{
				return Json(new { uploaded = false, message = "Помилка завантаження файлу: {ex.Message}" });
			}

			return BadRequest(new { uploaded = false, message = "Невідома помилка." });
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
	}
}
