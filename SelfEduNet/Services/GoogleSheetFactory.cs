using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Options;
using SelfEduNet.Configurations;

namespace SelfEduNet.Services;

public interface IGoogleSheetFactory
{
	SheetsService CreateClient();
}
public class GoogleSheetFactory(IOptions<GoogleSheetSettings> configurations) : IGoogleSheetFactory
{
	private readonly GoogleSheetSettings _configurations = configurations.Value;
	public SheetsService CreateClient()
	{
		return new SheetsService(new BaseClientService.Initializer
		{
			ApiKey = _configurations.ApiKey,
			ApplicationName = "GoogleSheetApp"
		});
	}
}


