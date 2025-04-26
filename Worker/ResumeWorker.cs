using System.Text.Json;
using Microsoft.Extensions.Options;
using OpenAI.Audio;
using SelfEduNet.Models;
using StackExchange.Redis;
using Worker.Configurations;
using Worker.Helpers;
using Worker.Services;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Worker;

public class ResumeWorker(
	IConnectionMultiplexer redis,
	IContextProcessorService contextProcessorService,
	ILogger<ResumeWorker> logger) : BackgroundService
{
	private readonly IDatabase _redisDb = redis.GetDatabase();
	private readonly SemaphoreSlim _semaphore = new(1, 4);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			string? taskJson = await _redisDb.ListLeftPopAsync(QueueKeys.ResumeQueue);
			try
			{
				if (!string.IsNullOrEmpty(taskJson))
				{
					var task = JsonSerializer.Deserialize<WorkerTask<ResumeRequest>>(taskJson);
					if (task != null)
					{
						logger.LogInformation($"Start processing task {task.Id}");
						await ProcessTaskAsync(task, stoppingToken);
						logger.LogInformation($"End processing task {task.Id}");
					}
				}
			}
			catch(Exception ex)
			{
				logger.LogError($"Error processing task: {ex.Message}");
			}
			finally
			{
				if (_semaphore.CurrentCount == 0)
				{
					_semaphore.Release();
				}
			}
			await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
		}
	}

	private async Task ProcessTaskAsync(WorkerTask<ResumeRequest> task, CancellationToken token)
	{
		try
		{
			await _semaphore.WaitAsync(token);
			logger.LogInformation($"Remaining threads in semaphore {_semaphore.CurrentCount}");



			logger.LogInformation($"Start processing resume request task id: {task.Id}");

			string segmentKey = $"{QueueKeys.ResumeResultPrefix}{task.Id}";
			await foreach (var chatCompletionPart in contextProcessorService.GenerateResumeAsync(task.Data.Context).WithCancellation(token))
			{
				await _redisDb.ListRightPushAsync(segmentKey, chatCompletionPart);

				logger.LogInformation($"Saved resume segment: {segmentKey}");
			}
			await _redisDb.ListRightPushAsync(segmentKey, "__END__");

			logger.LogInformation($"End processing resume request  task id: {task.Id}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error during task processing {task.Id}: {ex.Message}");
		}
		finally
		{
			_semaphore.Release();
		}
	}
}
