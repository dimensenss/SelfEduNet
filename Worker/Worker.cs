using System.Text.Json;
using OpenAI.Audio;
using SelfEduNet.Models;
using StackExchange.Redis;
using Worker.Helpers;
using Worker.Services;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Worker;

public class TranscriptionTask
{
	public string? Id { get; set; }
	public string? Url { get; set; }
	public string? Status { get; set; }
}

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
	IYoutubeClientService youtubeClient,
	IVideoProcessor videoProcessor,
	ILogger<Worker> logger) : BackgroundService
{
	private readonly IDatabase _redisDb = redis.GetDatabase();
	private readonly SemaphoreSlim _semaphore = new(1, 4);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			string? taskJson = await _redisDb.ListLeftPopAsync(QueueKeys.TranscriptionQueue);

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

			await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
		}
	}

	private async Task ProcessTaskAsync(TranscriptionTask task, CancellationToken token)
	{
		try
		{
			await _semaphore.WaitAsync(token);
			logger.LogInformation($"Remaining threads in semaphore {_semaphore.CurrentCount}");

			logger.LogInformation($"Start downloading video from {task.Url}");

			string audioPath = await youtubeClient.DownloadAudioAsync(task.Url);

			logger.LogInformation($"End downloading video from {task.Url}");

			logger.LogInformation($"Start trim silence from {audioPath}");

			string processedAudioPath = await videoProcessor.RemoveSilenceAsync(audioPath);

			logger.LogInformation($"End trim silence from {audioPath}");

			logger.LogInformation($"Start processing audio {processedAudioPath}");

			await foreach (var transcript in videoProcessor.SplitAndTranscribeAudioAsync(processedAudioPath, TimeSpan.FromSeconds(30)))
			{
				// Save each part of the transcription to Redis (streaming approach)
				string segmentKey = $"{QueueKeys.TranscriptionResultPrefix}{task.Id}_part{Guid.NewGuid()}";
				await _redisDb.StringSetAsync(segmentKey, transcript);

				logger.LogInformation($"Saved transcript segment: {segmentKey}");
			}

			logger.LogInformation($"End processing audio {processedAudioPath}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка обработки задачи {task.Id}: {ex.Message}");
		}
		finally
		{
			_semaphore.Release();
		}
	}
}
