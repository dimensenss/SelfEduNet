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
		//todo add lang support
		List<ChatMessage> messages = [
			new SystemChatMessage(
				"You are a highly skilled educational content analyst. "
				+ "Your job is to carefully analyze the provided transcript of a video lecture. "
				+ "Your output must be clearly structured into the following sections"
				+ "\n1. Summary: Briefly summarize the overall content in 5-7 sentences."
				+ "\n2. Key Points: List the main points or concepts covered."
				+ "\n3. Complex Topics: Identify and explain any parts that may be difficult to understand."
				+ "\n4. Recommendations: Suggest additional resources or next learning steps based on the lecture.\n"
				+ "\nUse semantic HTML template so it can be cleanly rendered in CKEditor."
			),
			new SystemChatMessage("All outputs including must be in the same language as the provided transcript."),
			new UserChatMessage($"Here is the video lecture transcript:\n{context}")
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



