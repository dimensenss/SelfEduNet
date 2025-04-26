using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using SelfEduNet.Data.Enum;
using SelfEduNet.ViewModels;
using SelfEduNet.Models;

[ApiController]
[Route("api/v1/transcription")]
public class TranscriptionController(IConnectionMultiplexer redis, ILogger<TranscriptionController> logger) : ControllerBase
{
	private readonly IDatabase _redisQueue = redis.GetDatabase();

	//[HttpPost("enqueue")]
	//public IActionResult EnqueueTranscription([FromBody] VideoRequest request)
	//{
	//	if (string.IsNullOrWhiteSpace(request.Url))
	//		return BadRequest("URL cannot be empty");

	//	var taskId = Guid.NewGuid().ToString();
	//	var taskData = new WorkerTask
	//	{
	//		Id = taskId,
	//		Url = request.Url,
	//		Status = "Queued"
	//	};

	//	_redisQueue.ListRightPush(QueueKeys.TranscriptionQueue, JsonSerializer.Serialize(taskData));
	//	logger.LogInformation($"Added URL {request.Url} to transcription queue with task ID {taskId}");

	//	return Ok(new { TaskId = taskId, Message = "URL added to queue" });
	//}
	[HttpGet("queue/messages")]
	public IActionResult GetQueueMessages()
	{
		var messages = _redisQueue.ListRange("transcription_queue", 0, -1).Select(m => m.ToString()).ToList();
		return Ok(messages);
	}
	[HttpGet("queue/{taskId}")]
	public async Task<IActionResult> GetTranscription(string taskId)
	{
		string listKey = $"{QueueKeys.TranscriptionResultPrefix}{taskId}";
		var segments = await _redisQueue.ListRangeAsync(listKey, 0, -1);

		if (segments == null || segments.Length == 0)
			return NotFound("No transcription segments found.");

		var fullTranscript = string.Join(" ", segments.Select(x => x.ToString()));

		return Ok(new { TaskId = taskId, Transcript = fullTranscript });
	}
}

public class TranscriptionRequest
{
	public string? Url { get; set; }
	public IFormFile? File { get; set; } = default;
}
public class ResumeRequest
{
	public string Context { get; set; }
}
public class WorkerTask<T>
{
	public string Id { get; set; }
	public T Data { get; set; }
	public string Status { get; set; }
	public required WorkerTaskType Type { get; set; }
}

public class WorkerResult
{
	public string? Content { get; set; }
	public bool IsEnd { get; set; }
}
