using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using SelfEduNet.ViewModels;
using SelfEduNet.Models;

[ApiController]
[Route("api/v1/transcription")]
public class TranscriptionController(IConnectionMultiplexer redis, ILogger<TranscriptionController> logger) : ControllerBase
{
	private readonly IDatabase _redisQueue = redis.GetDatabase();

	[HttpPost("enqueue")]
	public IActionResult EnqueueTranscription([FromBody] VideoRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.Url))
			return BadRequest("URL cannot be empty");

		var taskId = Guid.NewGuid().ToString();
		var taskData = new TranscriptionTask
		{
			Id = taskId,
			Url = request.Url,
			Status = "Queued"
		};

		_redisQueue.ListRightPush(QueueKeys.TranscriptionQueue, JsonSerializer.Serialize(taskData));
		logger.LogInformation($"Added URL {request.Url} to transcription queue with task ID {taskId}");

		return Ok(new { TaskId = taskId, Message = "URL added to queue" });
	}
	[HttpGet("queue/messages")]
	public IActionResult GetQueueMessages()
	{
		var messages = _redisQueue.ListRange("transcription_queue", 0, -1).Select(m => m.ToString()).ToList();
		return Ok(messages);
	}
}


public class TranscriptionTask
{
	public string Id { get; set; }
	public string Url { get; set; }
	public string Status { get; set; }
}

//TODO : Implement the GetTranscription method to fetch the transcription results from Redis
//[ApiController]
//[Route("api/transcription")]
//public class TranscriptionController(IConnectionMultiplexer redis) : ControllerBase
//{
//	private readonly IDatabase _redisDb = redis.GetDatabase();

//	[HttpGet("{taskId}")]
//	public async Task<IActionResult> GetTranscription(string taskId)
//	{
//		if (string.IsNullOrEmpty(taskId))
//			return BadRequest("Invalid Task ID");

//		// Get all keys related to this transcription
//		string pattern = $"{QueueKeys.TranscriptionResultPrefix}{taskId}_part*";
//		var server = redis.GetServer(redis.GetEndPoints().First());
//		var keys = server.Keys(pattern: pattern).ToArray();

//		if (keys.Length == 0)
//			return NotFound("Transcription not found or still processing.");

//		// Fetch all transcription parts
//		var tasks = keys.Select(key => _redisDb.StringGetAsync(key));
//		var transcripts = await Task.WhenAll(tasks);

//		return Ok(new { TaskId = taskId, Transcription = transcripts });
//	}
//}
//function fetchTranscription(taskId)
//{
//	$.ajax({
//		url: `/ api / transcription /${ taskId}`,
//		method: "GET",
//		success: function(response) {
//			$("#transcriptionResult").html("");
//			response.Transcription.forEach((text, index) => {
//				$("#transcriptionResult").append(`< p >< strong > Segment ${ index + 1}:</ strong > ${ text}</ p >`);
//			});
//		},
//		error: function() {
//			$("#transcriptionResult").html("<p>Error fetching transcription.</p>");
//		}
//	});
//}

//$(document).ready(function() {
//	$("#fetchTranscription").click(function() {
//		let taskId = $("#taskIdInput").val().trim();
//		if (taskId === "")
//		{
//			alert("Please enter a Task ID");
//			return;
//		}

//		fetchTranscription(taskId); // Fetch once

//		// Start polling every 5 seconds
//		setInterval(function() {
//			fetchTranscription(taskId);
//		}, 5000);
//	});
//});
//< input type = "text" id = "taskIdInput" placeholder = "Enter Task ID" >

//	< button id = "fetchTranscription" > Get Transcription </ button >


//	< div id = "transcriptionResult" ></ div >