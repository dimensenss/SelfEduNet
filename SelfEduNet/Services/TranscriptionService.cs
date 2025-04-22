using SelfEduNet.Repositories;

namespace SelfEduNet.Services;
public interface ITranscriptionService
{
	Task AddFileToQueue(IFormFile? file);
	Task<string> AddURLToQueue(string url);
	Task<TranscriptionResult> GetContentByTaskId(string taskId);
}

public class TranscriptionService(ITranscriptionRepository transcriptionRepository) : ITranscriptionService
{
	public async Task AddFileToQueue(IFormFile? file)
	{
		await transcriptionRepository.AddFileToQueue(file);
	}
	public async Task<string> AddURLToQueue(string url)
	{
		return await transcriptionRepository.AddURLToQueue(url);
	}

	public async Task<TranscriptionResult> GetContentByTaskId(string taskId)
	{
		return await transcriptionRepository.GetContentByTaskId(taskId);
	}
}
