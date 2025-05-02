using System.ClientModel;
using OpenAI.Audio;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using Worker.Configurations;
namespace Worker.Services
{

	public interface IOpenAIClientFactory
	{
		AudioClient CreateAudioClient();
		ChatClient CreateChatClient();
	}
	public class OpenAiClientFactory(IOptions<OpenAISettings> openAiSettings) : IOpenAIClientFactory
	{
		private readonly OpenAISettings _openAiSettings = openAiSettings.Value;

		private OpenAIClient CreateOpenAIClient()
		{
			if (string.IsNullOrEmpty(_openAiSettings.ApiKey))
			{
				throw new ArgumentException("API Key or Model Name is missing in configuration.");
			}

			return new OpenAIClient(_openAiSettings.ApiKey);
		}
		public ChatClient CreateChatClient()
		{
			var client = CreateOpenAIClient();
			return client.GetChatClient("gpt-4.1-mini");
		}

		public AudioClient CreateAudioClient()
		{
			var client = CreateOpenAIClient();
			return client.GetAudioClient("whisper-1");
		}
		
	}
}
