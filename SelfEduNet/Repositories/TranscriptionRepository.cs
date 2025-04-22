using System.Text.Json;
using SelfEduNet.Models;
using StackExchange.Redis;
using System.Threading.Tasks;
using SelfEduNet.Data.Regex;
using System.Text;

namespace SelfEduNet.Repositories;

public interface ITranscriptionRepository
{
	Task AddFileToQueue(IFormFile? file);
	Task<string> AddURLToQueue(string url);
	Task<TranscriptionResult> GetContentByTaskId(string taskId);
}
public class TranscriptionRepository: ITranscriptionRepository
{
	private readonly IDatabase _redisQueue;
	private readonly ILogger<TranscriptionRepository> _logger;
	public TranscriptionRepository(IConnectionMultiplexer redis, ILogger<TranscriptionRepository> logger)
	{
		_redisQueue = redis.GetDatabase();
		_logger = logger;
	}

	public Task AddFileToQueue(IFormFile? file)
	{
		if (file == null || file.Length < 0)
		{
			return Task.FromException(new ArgumentException("Invalid file"));
		}

		var taskId = Guid.NewGuid().ToString();
		var taskData = new TranscriptionTask
		{
			Id = taskId,
			File = file,
			Status = "Queued"
		};

		_redisQueue.ListRightPush(QueueKeys.TranscriptionQueue, JsonSerializer.Serialize(taskData));
		_logger.LogInformation($"Added file with length {file.Length} to transcription queue with task ID {taskId}");

		return Task.CompletedTask;
	}
	public async Task<string> AddURLToQueue(string url)
	{
		if (CommonRegex.YoutubeRegex.IsMatch(url))
		{
			throw new ArgumentException("Посилання з YouTube не дозволяються");
		}

		var taskId = Guid.NewGuid().ToString();
		var taskData = new TranscriptionTask
		{
			Id = taskId,
			Url = url,
			Status = "Queued"
		};

		await _redisQueue.ListRightPopLeftPushAsync(QueueKeys.TranscriptionQueue, JsonSerializer.Serialize(taskData));
		_logger.LogInformation($"Added url with length to transcription queue with task ID {taskId}");

		return taskId.ToString();
	}

	public async Task<TranscriptionResult> GetContentByTaskId(string taskId)
	{
		string key = $"{QueueKeys.TranscriptionResultPrefix}{taskId}";
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

		return new TranscriptionResult
		{
			Content = sb.ToString(),
			IsEnd = isEnd
		};

	}
}

