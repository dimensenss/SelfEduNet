using System.Text.Json;
using SelfEduNet.Models;
using StackExchange.Redis;
using System.Threading.Tasks;
using SelfEduNet.Data.Regex;
using System.Text;
using SelfEduNet.Data.Enum;

namespace SelfEduNet.Repositories;

public interface ITranscriptionRepository
{
	Task AddFileToQueue(IFormFile? file);
	Task<string> AddURLToQueue(string url);
	Task<string> AddResumeRequestToQueue(string context);
	Task<WorkerResult> GetContentByTaskId(string taskId, WorkerTaskType keyType);
}
public class TranscriptionRepository(IConnectionMultiplexer redis, ILogger<TranscriptionRepository> logger)
	: ITranscriptionRepository
{
	private readonly IDatabase _redisQueue = redis.GetDatabase();

	public Task AddFileToQueue(IFormFile? file)
	{
		if (file == null || file.Length < 0)
		{
			return Task.FromException(new ArgumentException("Invalid file"));
		}

		var taskId = Guid.NewGuid().ToString();
		var taskData = new WorkerTask<TranscriptionRequest>
		{
			Id = taskId,
			Data = new() { File = file },
			Status = "Queued",
			Type = WorkerTaskType.Transcription
		};

		_redisQueue.ListRightPush(QueueKeys.TranscriptionQueue, JsonSerializer.Serialize(taskData));
		logger.LogInformation($"Added file with length {file.Length} to transcription queue with task ID {taskId}");

		return Task.CompletedTask;
	}
	public async Task<string> AddURLToQueue(string url)
	{
		var taskId = Guid.NewGuid().ToString();
		var taskData = new WorkerTask<TranscriptionRequest>
		{
			Id = taskId,
			Data = new() { Url = url },
			Status = "Queued",
			Type = WorkerTaskType.Transcription
		};

		await _redisQueue.ListRightPushAsync(QueueKeys.TranscriptionQueue, JsonSerializer.Serialize(taskData));
		logger.LogInformation($"Added url with length to transcription queue with task ID {taskId}");

		return taskId.ToString();
	}

	public async Task<WorkerResult> GetContentByTaskId(string taskId, WorkerTaskType keyType)
	{
		// Determine the queue key based on the WorkerTaskType enum
		string queueKey = keyType switch
		{
			WorkerTaskType.Transcription => QueueKeys.TranscriptionResultPrefix,
			WorkerTaskType.Resume => QueueKeys.ResumeResultPrefix,
			_ => throw new ArgumentException("Invalid WorkerTaskType")
		};

		string key = $"{queueKey}{taskId}";
		var values = await _redisQueue.ListRangeAsync(key, 0, -1);

		StringBuilder sb = new StringBuilder();
		bool isEnd = false;

		foreach (var elem in values)
		{
			var str = elem.ToString();
			if (str == "__END__")
			{
				isEnd = true;
				break;
			}
			sb.Append(str);
		}

		return new WorkerResult
		{
			Content = sb.ToString(),
			IsEnd = isEnd
		};
	}
	public async Task<string> AddResumeRequestToQueue(string context)
	{
		if (string.IsNullOrWhiteSpace(context))
		{
			throw new ArgumentException("Контекст пустий");
		}

		var taskId = Guid.NewGuid().ToString();
		var taskData = new WorkerTask<ResumeRequest>
		{
			Id = taskId,
			Data = new() { Context = context },
			Status = "Queued",
			Type = WorkerTaskType.Resume
		};

		await _redisQueue.ListRightPushAsync(QueueKeys.ResumeQueue, JsonSerializer.Serialize(taskData));
		logger.LogInformation($"Added request for resume to resume queue with task ID {taskId}");

		return taskId.ToString();
	}
}

