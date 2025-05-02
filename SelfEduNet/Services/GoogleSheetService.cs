using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Options;
using SelfEduNet.Configurations;

namespace SelfEduNet.Services;
public interface IGoogleSheetService
{
	Task<IList<IList<object>>> GetRangeAsync(string spreadsheetId, string rangeA1);
}
public class GoogleSheetService(IOptions<GoogleSheetService> configurations) : IGoogleSheetService
{
	private readonly SheetsService _service;
	private readonly GoogleSheetService _configurations = configurations.Value;


	public SheetsService CreateSh(string apiKey, string applicationName = "GoogleSheetApp")
	{
		_service = new SheetsService(new BaseClientService.Initializer
		{
			ApiKey = _configurations.A,
			ApplicationName = applicationName
		});
	}

	public async Task<IList<IList<object>>> GetRangeAsync(string spreadsheetId, string rangeA1)
	{
		var request = _service.Spreadsheets.Values.Get(spreadsheetId, rangeA1);
		var response = await request.ExecuteAsync();
		return response.Values ?? new List<IList<object>>();
	}
}

