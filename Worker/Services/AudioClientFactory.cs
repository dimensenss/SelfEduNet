using OpenAI.Audio;
using Microsoft.Extensions.Options;
using Worker.Configurations;
namespace Worker.Services
{

	public interface IAudioClientFactory
	{
		AudioClient CreateAudioClient();
	}
	public class AudioClientFactory(IOptions<OpenAISettings> openAiSettings) : IAudioClientFactory
	{
		private readonly OpenAISettings _openAiSettings = openAiSettings.Value;

		public AudioClient CreateAudioClient()
		{
			if (string.IsNullOrEmpty(_openAiSettings.ApiKey) || string.IsNullOrEmpty(_openAiSettings.ModelName))
			{
				throw new ArgumentException("API Key or Model Name is missing in configuration.");
			}

			return new AudioClient(_openAiSettings.ModelName, _openAiSettings.ApiKey);
		}
	}
}
