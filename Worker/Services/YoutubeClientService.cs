using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Worker.Services
{
	public interface IYoutubeClientService
	{
		Task<string> DownloadAudioAsync(string videoUrl);
	}

	public class YoutubeClientService : IYoutubeClientService
	{
		private readonly YoutubeClient _youtubeClient = new();

		public async Task<string> DownloadAudioAsync(string videoUrl)
		{
			var video = await _youtubeClient.Videos.GetAsync(videoUrl);
			var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id);
			var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
			string tempPath = Path.Combine(Path.GetTempPath(), $"{video.Id}.mp3");

			await _youtubeClient.Videos.Streams.DownloadAsync(audioStreamInfo, tempPath);
			return tempPath;
		}
	}

}
