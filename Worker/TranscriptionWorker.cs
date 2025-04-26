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

public class TranscriptionWorker(
	IConnectionMultiplexer redis,
	IFileService file,
	IVideoProcessorService videoProcessorService,
	ILogger<TranscriptionWorker> logger,
	IOptions<TranscriptionSettings> transcriptionSettings) : BackgroundService
{
	private readonly IDatabase _redisDb = redis.GetDatabase();
	private readonly SemaphoreSlim _semaphore = new(1, 4);
	private readonly IOptions<TranscriptionSettings> _transcriptionSettings = transcriptionSettings;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			string? taskJson = await _redisDb.ListLeftPopAsync(QueueKeys.TranscriptionQueue);
			try
			{
				if (!string.IsNullOrEmpty(taskJson))
				{
					var task = JsonSerializer.Deserialize<WorkerTask<TranscriptionRequest>>(taskJson);
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

	private async Task ProcessTaskAsync(WorkerTask<TranscriptionRequest> task, CancellationToken token)
	{
		try
		{
			await _semaphore.WaitAsync(token);
			logger.LogInformation($"Remaining threads in semaphore {_semaphore.CurrentCount}");

			string audioPath = null;
			if (task.Data.File != null && task.Data.File.Length > 0)
			{
				audioPath = await file.DownloadAudioFromFileAsync(task.Data.File);
			}
			else if(task.Data.Url != null && task.Data.Url.Length > 0) 
			{
				audioPath = await file.DownloadAudioFromURLAsync(task.Data.Url);
			}

			logger.LogInformation($"Start trim silence from {audioPath}");

			string processedAudioPath = await videoProcessorService.RemoveSilenceAsync(audioPath);

			logger.LogInformation($"End trim silence from {audioPath}");

			logger.LogInformation($"Start processing audio {processedAudioPath}");

			string segmentKey = $"{QueueKeys.TranscriptionResultPrefix}{task.Id}";
			var segmentDuration = TimeSpan.FromSeconds(_transcriptionSettings.Value.SegmentDuration);

			await foreach (var transcript in videoProcessorService.SplitAndTranscribeAudioAsync(processedAudioPath, segmentDuration).WithCancellation(token))
			{
				await _redisDb.ListRightPushAsync(segmentKey, transcript);

				logger.LogInformation($"Saved transcript segment: {segmentKey}");
			}
			await _redisDb.ListRightPushAsync(segmentKey, "__END__");
			logger.LogInformation($"End processing audio {processedAudioPath}");
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
