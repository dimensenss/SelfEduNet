using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SelfEduNet.Data.Regex;
using YoutubeExplode;
using YoutubeExplode.Videos.ClosedCaptions;
using YoutubeExplode.Videos.Streams;

namespace Worker.Services;

public interface IFileService
{
	Task<string> DownloadAudioFromURLAsync(string videoUrl);
	Task<string> DownloadAudioFromFileAsync(IFormFile file);
}

public class FileService(ILogger<IFileService> logger) : IFileService
{
	private readonly ILogger<IFileService> _logger = logger;
	public async Task<string> DownloadAudioFromURLAsync(string? videoUrl)
	{
		bool isYouTube = CommonRegex.YoutubeRegex.IsMatch(videoUrl);
		_logger.LogInformation($"Start downloading video from {videoUrl}");

		if (isYouTube)
		{
			YoutubeClient _youtubeClient = new();

			var video = await _youtubeClient.Videos.GetAsync(videoUrl);
			var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id);
			var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
			string tempPath = Path.Combine(Path.GetTempPath(), $"{video.Id}.mp3");

			await _youtubeClient.Videos.Streams.DownloadAsync(audioStreamInfo, tempPath);
			logger.LogInformation($"End downloading video from {videoUrl}");

			return tempPath;
		}
		else
		{
			if (string.IsNullOrEmpty(videoUrl))
				throw new ArgumentException("URL cannot be null or empty");

			using var httpClient = new HttpClient();
			var uri = new Uri(videoUrl);
			var fileName = Path.GetFileName(uri.LocalPath);
			var extension = Path.GetExtension(fileName);
			string tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{extension}");
			var fileBytes = await httpClient.GetByteArrayAsync(videoUrl);
			await File.WriteAllBytesAsync(tempPath, fileBytes);
			logger.LogInformation($"End downloading video from {videoUrl}");

			return tempPath;
		}
	}

	public async Task<string> DownloadAudioFromFileAsync(IFormFile file)
	{
		_logger.LogInformation($"Start downloading video from file {file.FileName}");
		string tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
		using (var stream = new FileStream(tempPath, FileMode.Create))
		{
			await file.CopyToAsync(stream);
		}
		_logger.LogInformation($"End downloading video from file {file.FileName}");

		return tempPath;
	}
}

