using SelfEduNet.Data.Enum;
using SelfEduNet.Repositories;

namespace SelfEduNet.Services;
public interface ITranscriptionService
{
	Task AddFileToQueue(IFormFile? file);
	Task<string> AddURLToQueue(string url);
	Task<string> AddResumeRequestToQueue(string context);
	Task<WorkerResult> GetContentByTaskId(string taskId, WorkerTaskType keyType);
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

	public async Task<WorkerResult> GetContentByTaskId(string taskId, WorkerTaskType keyType)
	{
		return await transcriptionRepository.GetContentByTaskId(taskId, keyType);
	}

	public async Task<string> AddResumeRequestToQueue(string context)
	{
		return await transcriptionRepository.AddResumeRequestToQueue(context);
	}
}
