using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using Microsoft.AspNetCore.Mvc;
using SelfEduNet.Services;
using SelfEduNet.Extensions;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using SelfEduNet.ViewModels;
using SelfEduNet.Models;

namespace SelfEduNet.Areas.Teach.Controllers;
[Area("Teach")]
[Authorize]
public class StepTestController(IStepService stepService, IUserStepService userStepService,
	IGoogleSheetService googleSheetService) : Controller
{
	private readonly IStepService _stepService = stepService;
	private readonly IUserStepService _userStepService = userStepService;
	private readonly IGoogleSheetService _googleSheetService = googleSheetService;

	public async Task<IActionResult> GetUserAnswers(int id)
	{
		string userId = User.GetUserId();
		string userEmail = User.GetUserEmail();

		if (!await _userStepService.StepExistsAsync(userId, id))
		{
			return NotFound(new { message = "Крок не знайдено" });
		}

		var step = await _stepService.GetStepByIdAsync(id);

		if (step == null)
		{
			return NotFound(new { message = "Крок не знайдено" });
		}

		string sheetId = _googleSheetService.GetSheetIdFromUrl(step?.StepTest?.GoogleSheetUrl ?? String.Empty);
		string sheetName;
		if (string.IsNullOrWhiteSpace(sheetId) || string.IsNullOrWhiteSpace(userEmail))
			return BadRequest("Missing sheetId or userEmail");

		try
		{
			sheetName = await _googleSheetService.GetSheetNameAsync(sheetId);
		}
		catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
		{
			return BadRequest(new { message = "Сталася проблема з доступ для отриманням відповіді. Зв'яжіться з автором курсу." });
		}
		catch (Exception ex)
		{
			return BadRequest(new { message = "Сталася помилка з отриманням відповіді" });

		}
		string range = sheetName;

		var values = await _googleSheetService.GetValuesById(sheetId, range);

		if (values == null || values.Count < 2)
			return NotFound("No data found.");

		var headers = values[0].ToList();

		int emailColumnIndex = headers.FindIndex(h =>
			string.Equals(h?.ToString()?.Trim(), "Email", StringComparison.OrdinalIgnoreCase));
		int scoreColumnIndex = headers.FindIndex(h =>
			string.Equals(h?.ToString()?.Trim(), "Score", StringComparison.OrdinalIgnoreCase));
		int timestampColumnIndex = headers.FindIndex(h =>
			string.Equals(h?.ToString()?.Trim(), "Timestamp", StringComparison.OrdinalIgnoreCase));

		if (emailColumnIndex == -1)
			return NotFound(new { message = "Колонку Email не знайдено" });
		if (scoreColumnIndex == -1)
			return NotFound(new { message = "Колонку Score не знайдено" });
		if (timestampColumnIndex == -1)
			return NotFound(new { message = "Колонку Timestamp не знайдено" });

		var userRows = values
			.Skip(1)
			.Where(row =>
				row.Count > emailColumnIndex &&
				string.Equals(row[emailColumnIndex]?.ToString()?.Trim(), userEmail, StringComparison.OrdinalIgnoreCase))
			.ToList();

		if (userRows.Count == 0)
			return NotFound(new { message = "Ви ще не проходили тест" });

		Regex scoreRegex = new Regex(@"(?<got>\d+)\s*/\s*(?<total>\d+)", RegexOptions.Compiled);

		// Найдём лучшую попытку
		IList<object> bestAttemptRow = null;
		int bestGot = int.MinValue;
		int bestTotal = 0;
		string bestScoreRaw = null;

		foreach (var row in userRows)
		{
			if (row.Count > scoreColumnIndex)
			{
				var scoreStr = row[scoreColumnIndex]?.ToString();
				var m = scoreRegex.Match(scoreStr ?? "");
				if (m.Success)
				{
					int got = int.Parse(m.Groups["got"].Value);
					int total = int.Parse(m.Groups["total"].Value);

					if (got > bestGot || (got == bestGot && total > bestTotal))
					{
						bestGot = got;
						bestTotal = total;
						bestScoreRaw = scoreStr;
						bestAttemptRow = row;
					}
				}
			}
		}

		var latestRow = userRows
			.OrderByDescending(row =>
			{
				if (row.Count > timestampColumnIndex &&
					DateTime.TryParse(row[timestampColumnIndex]?.ToString(), out var ts))
				{
					return ts;
				}
				return DateTime.MinValue;
			})
			.First();

		string lastScoreStr = latestRow.Count > scoreColumnIndex ? latestRow[scoreColumnIndex]?.ToString() : null;
		int lastScoreGot = 0;
		int lastScoreTotal = 0;
		if (!string.IsNullOrWhiteSpace(lastScoreStr))
		{
			var m = scoreRegex.Match(lastScoreStr);
			if (m.Success)
			{
				lastScoreGot = int.Parse(m.Groups["got"].Value);
				lastScoreTotal = int.Parse(m.Groups["total"].Value);
			}
		}

		var stepTestResult = new StepTestResult
		{
			Timestamp = DateTime.TryParse(latestRow[timestampColumnIndex]?.ToString(), out var ts) ? ts.ToUniversalTime() : throw new InvalidOperationException("Invalid date format"),
			AttemptsCount = userRows.Count,
			Score = lastScoreGot,
			BiggestScore = bestScoreRaw,
			TotalScore = lastScoreTotal,
			IsPassed = lastScoreTotal > 0 && ((double)lastScoreGot / lastScoreTotal) >= 0.8
		};
		
		bool result = await _userStepService.CreateOrUpdateUserTestResultAsync(stepTestResult, id, userId);
		if (!result)
		{
			return BadRequest(new { message = "Не вдалось отримати результати тесту" });
		}

		if (!stepTestResult.IsPassed)
		{
			return BadRequest(new { message = "Не вдалось отримати результати тесту" });
		}
		var redirectUrl = Url.Action("ViewLesson", "Lesson", new
		{
			courseId = step.Lesson.CourseId,
			lessonId = step.LessonId,
			stepId = step.Id
		});

		return Ok(new { redirectUrl = redirectUrl, message = "Тест пройдено." });
	}
	public async Task<IActionResult> CreateTest(CreateTestViewModel createTestVM)
	{
		var step = await _stepService.GetStepByIdAsync(createTestVM.Id);

		if (!ModelState.IsValid)
		{
			TempData["NotifyType"] = "danger";
			TempData["NotifyMessage"] = "Не вдалось зберегти дані про тест";

			return Redirect($"/EditLesson/{step.Lesson.CourseId}/{step.LessonId}?stepId={step.Id}");
		}

		var stepTest = new StepTest
		{
			GoogleFormUrl = createTestVM.GoogleFormUrl,
			GoogleSheetUrl = createTestVM.GoogleSheetUrl,
		};
		bool result;
		if (step.StepTest == null)
		{
			result = await _stepService.CreateStepTestAsync(stepTest, step.Id);
		}
		else
		{
			step.StepTest = stepTest;
			result = _stepService.Update(step);
		}

		if (!result)
		{
			TempData["NotifyType"] = "danger";
			TempData["NotifyMessage"] = "Не вдалось створити тест";
		}
		else
		{
			TempData["NotifyType"] = "success";
			TempData["NotifyMessage"] = "Дані збережено";
		}

		return Redirect($"/EditLesson/{step.Lesson.CourseId}/{step.LessonId}?stepId={step.Id}");
	}
}

