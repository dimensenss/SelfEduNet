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

//public class Worker(IConnectionMultiplexer redis, IAudioClientFactory audioClientFactory, ILogger<Worker> logger) : BackgroundService
//{
//	private readonly IDatabase _redisDb = redis.GetDatabase();
//	private readonly YoutubeClient _youtubeClient = new();
//	private readonly AudioClient _audioClient = audioClientFactory.CreateAudioClient();
//	private readonly SemaphoreSlim _semaphore = new(1, 1); // Ограничиваем число одновременных запросов

//	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//	{
//		while (!stoppingToken.IsCancellationRequested)
//		{
//			string? taskJson = await _redisDb.ListLeftPopAsync("transcription_queue");

//			if (!string.IsNullOrEmpty(taskJson))
//			{
//				var task = JsonSerializer.Deserialize<TranscriptionTask>(taskJson);
//				if (task != null)
//				{
//					try
//					{
//						await _semaphore.WaitAsync(stoppingToken); // Ограничиваем конкурентные запросы
//						string audioPath = await DownloadAudioAsync(task.Url);
//						await Task.Delay(1000, stoppingToken); // Задержка перед отправкой запроса
//						string transcript = await TranscribeAudioWithRetryAsync(audioPath);

//						// Сохранение результата
//						await _redisDb.StringSetAsync($"transcription_result:{task.Id}", transcript);
//					}
//					catch (Exception ex)
//					{
//						Console.WriteLine($"Ошибка обработки задачи {task.Id}: {ex.Message}");
//					}
//					finally
//					{
//						_semaphore.Release();
//					}
//				}
//			}

//			await Task.Delay(1000, stoppingToken); // Пауза перед новой проверкой очереди
//		}
//	}

//	private async Task<string> DownloadAudioAsync(string videoUrl)
//	{
//		var video = await _youtubeClient.Videos.GetAsync(videoUrl);
//		var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id);
//		var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
//		string tempPath = Path.Combine(Path.GetTempPath(), $"{video.Id}.mp3");

//		await _youtubeClient.Videos.Streams.DownloadAsync(audioStreamInfo, tempPath);
//		return tempPath;
//	}

//	private async Task<string> TranscribeAudioWithRetryAsync(string audioPath, int maxRetries = 3)
//	{
//		int attempt = 0;
//		TimeSpan delay = TimeSpan.FromSeconds(2);

//		while (attempt < maxRetries)
//		{
//			try
//			{
//				return await TranscribeAudioAsync(audioPath);
//			}
//			catch (Exception ex) when (ex.Message.Contains("insufficient_quota") || ex.Message.Contains("HTTP 429"))
//			{
//				attempt++;
//				if (attempt >= maxRetries)
//				{
//					Console.WriteLine("Достигнут лимит повторных попыток транскрибации.");
//					throw;
//				}

//				Console.WriteLine($"Попытка {attempt}: API перегружен, ждем {delay.Seconds} секунд...");
//				await Task.Delay(delay);
//				delay *= 2; // Увеличиваем задержку (экспоненциальный рост)
//				delay *= 2; // Увеличиваем задержку (экспоненциальный рост)
//			}
//		}

//		throw new Exception("Не удалось выполнить транскрибацию после нескольких попыток.");
//	}

//	private async Task<string> TranscribeAudioAsync(string audioPath)
//	{
//		AudioTranscriptionOptions options = new()
//		{
//			ResponseFormat = AudioTranscriptionFormat.Verbose,
//			TimestampGranularities = AudioTimestampGranularities.Word | AudioTimestampGranularities.Segment,
//		};
//		AudioTranscription transcription = await _audioClient.TranscribeAudioAsync(audioPath, options);

//		return transcription.Text;
//	}
//}
public class Worker(
	IConnectionMultiplexer redis,
	IFileService file,
	IVideoProcessor videoProcessor,
	ILogger<Worker> logger,
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
					var task = JsonSerializer.Deserialize<TranscriptionTask>(taskJson);
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

	private async Task ProcessTaskAsync(TranscriptionTask task, CancellationToken token)
	{
		try
		{
			await _semaphore.WaitAsync(token);
			logger.LogInformation($"Remaining threads in semaphore {_semaphore.CurrentCount}");

			string audioPath = null;
			if (task.File != null && task.File.Length > 0)
			{
				audioPath = await file.DownloadAudioFromFileAsync(task.File);
			}
			else if(task.Url != null && task.Url.Length > 0) 
			{
				audioPath = await file.DownloadAudioFromURLAsync(task.Url);
			}

			logger.LogInformation($"Start trim silence from {audioPath}");

			string processedAudioPath = await videoProcessor.RemoveSilenceAsync(audioPath);

			logger.LogInformation($"End trim silence from {audioPath}");

			logger.LogInformation($"Start processing audio {processedAudioPath}");

			string segmentKey = $"{QueueKeys.TranscriptionResultPrefix}{task.Id}";
			var segmentDuration = TimeSpan.FromSeconds(_transcriptionSettings.Value.SegmentDuration);

			await foreach (var transcript in videoProcessor.SplitAndTranscribeAudioAsync(processedAudioPath, segmentDuration).WithCancellation(token))
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
