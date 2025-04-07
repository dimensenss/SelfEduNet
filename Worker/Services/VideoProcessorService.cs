using OpenAI.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worker.Helpers;

namespace Worker.Services
{
	public interface IVideoProcessor
	{
		IAsyncEnumerable<string> SplitAndTranscribeAudioAsync(string audioPath, TimeSpan segmentDuration);
		Task<string> RemoveSilenceAsync(string audioPath);
		Task<string> GenerateSummaryAsync(string fullTranscript);
	}

	public class VideoProcessor(ILogger<VideoProcessor> logger, IAudioClientFactory audioClientFactory) : IVideoProcessor
	{
		private readonly AudioClient _audioClient = audioClientFactory.CreateAudioClient();

		public async Task<string> RemoveSilenceAsync(string audioPath)
		{
			string outputFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp3");

			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "ffmpeg",
					Arguments = $"-i \"{audioPath}\" -af silenceremove=stop_periods=-1:stop_duration=1:stop_threshold=-50dB \"{outputFilePath}\"",
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};

			process.Start();
			await process.WaitForExitAsync();

			return outputFilePath;
		}

		public async IAsyncEnumerable<string> SplitAndTranscribeAudioAsync(string audioPath, TimeSpan segmentDuration)
		{
			string outputDirectory = Path.GetTempPath();
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(audioPath);
			string fileExtension = Path.GetExtension(audioPath);

			int segmentIndex = 0;
			TimeSpan currentTime = TimeSpan.Zero;

			while (true)
			{
				string outputFilePath = Path.Combine(outputDirectory, $"{fileNameWithoutExtension}_part{segmentIndex}{fileExtension}");

				var process = new Process
				{
					StartInfo = new ProcessStartInfo
					{
						FileName = "ffmpeg",
						Arguments = $"-i \"{audioPath}\" -ss {currentTime} -t {segmentDuration} -y \"{outputFilePath}\"",
						RedirectStandardError = true,
						UseShellExecute = false,
						CreateNoWindow = true
					}
				};

				process.Start();
				await process.WaitForExitAsync();

				// Check if FFmpeg failed (probably end of file)
				if (process.ExitCode != 0 || !File.Exists(outputFilePath))
				{
					yield break; // Stop iteration if no more segments
				}
				TimeSpan actualDuration = await GetAudioDurationAsync(outputFilePath);
				if (actualDuration < segmentDuration && segmentIndex > 0) // Allow first segment to process even if short
				{
					logger.LogInformation($"Last segment ({actualDuration}). Stopping transcription.");
					File.Delete(outputFilePath);
					yield break; // Stop iteration if the last segment is shorter than expected
				}

				string transcript = null;
				try
				{
					// Transcribe segment asynchronously with retries
					transcript = await RetryHelper.ExecuteWithRetryAsync(
						() => TranscribeSegmentAsync(outputFilePath),
						maxRetries: 3,
						retryDelay: TimeSpan.FromSeconds(2)
					);
				}
				catch (Exception ex)
				{
					logger.LogError($"Error during transcription fragment {segmentIndex}: {ex.Message}");
				}
				finally
				{
					File.Delete(outputFilePath);
					logger.LogInformation($"Clean up segment after transcription {segmentIndex}: {outputFilePath}");
				}

				if (transcript != null)
				{
					yield return transcript;
				}

				segmentIndex++;
				currentTime += segmentDuration;
			}
		}

		private async Task<string> TranscribeSegmentAsync(string audioPath)
		{
			AudioTranscriptionOptions options = new()
			{
				ResponseFormat = AudioTranscriptionFormat.Verbose,
				TimestampGranularities = AudioTimestampGranularities.Word | AudioTimestampGranularities.Segment,
			};

			AudioTranscription transcription = await _audioClient.TranscribeAudioAsync(audioPath, options);
			return transcription.Text;

			//await Task.Delay(TimeSpan.FromSeconds(1));
			//return "Test Transcription";
		}
		private async Task<TimeSpan> GetAudioDurationAsync(string filePath)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "ffprobe",
					Arguments = $"-i \"{filePath}\" -show_entries format=duration -v quiet -of csv=\"p=0\"",
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};

			process.Start();
			string output = await process.StandardOutput.ReadToEndAsync();
			await process.WaitForExitAsync();

			if (double.TryParse(output, out double duration))
			{
				return TimeSpan.FromSeconds(duration);
			}

			return TimeSpan.Zero; // If we can't get duration, return 0
		}
		public async Task<string> GenerateSummaryAsync(string fullTranscript)
		{
			// Call OpenAI API or another NLP service to generate a summary
			return "Generated Summary";
		}
	}

}
