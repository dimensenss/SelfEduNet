using Microsoft.AspNetCore.Mvc.ModelBinding;
using OpenAI;
using OpenAI.Audio;
using OpenAI.Chat;
using Org.BouncyCastle.Asn1.Crmf;
using StackExchange.Redis;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker.Services;

public interface IContextProcessorService
{
	IAsyncEnumerable<string> GenerateResumeAsync(string context);
}

public class ContextProcessorService(IOpenAIClientFactory openAiClientFactory, ILogger<ContextProcessorService> logger) : IContextProcessorService
{
	private readonly ChatClient _chatClient = openAiClientFactory.CreateChatClient();
	public async IAsyncEnumerable<string> GenerateResumeAsync(string context)
	{
		List<ChatMessage> messages = [
		
			new SystemChatMessage("You are a helpful assistant that summarizes text."),
			new UserChatMessage($"Please summarize the following context:\n{context}")
		];

		AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = _chatClient.CompleteChatStreamingAsync(messages);

		StringBuilder accumulatedText = new StringBuilder();
		int maxChunkSize = 1000; // Set a max chunk size (in characters)

		await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
		{
			if (completionUpdate.ContentUpdate.Count > 0)
			{
				accumulatedText.Append(completionUpdate.ContentUpdate[0].Text);

				// If the accumulated text exceeds the chunk size, yield it and reset the buffer
				if (accumulatedText.Length >= maxChunkSize)
				{
					yield return accumulatedText.ToString();
					accumulatedText.Clear();
				}
			}
		}

		// Yield any remaining text after the streaming completes
		if (accumulatedText.Length > 0)
		{
			yield return accumulatedText.ToString();
		}
	}
}



