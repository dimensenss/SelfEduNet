using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Options;
using SelfEduNet.Configurations;
using System.Text.RegularExpressions;
using SelfEduNet.Data.Regex;
using Google.Apis.Sheets.v4.Data;

namespace SelfEduNet.Services;
public interface IGoogleSheetService
{
	string GetSheetIdFromUrl(string url);
	Task<string> GetSheetNameAsync(string sheetId);
	Task<IList<IList<object>>> GetValuesById(string sheetId, string range);

}
public class GoogleSheetService(IGoogleSheetFactory googleSheetFactory) : IGoogleSheetService
{
	private readonly SheetsService _sheetsService = googleSheetFactory.CreateClient();

	public string GetSheetIdFromUrl(string url)
	{
		var match = Regex.Match(url, CommonRegex.GoogleSheetRegex.ToString());
		return match.Success ? match.Value : null;
	}
	public async Task<string> GetSheetNameAsync(string sheetId)
	{
		try
		{
			var spreadsheet = await _sheetsService.Spreadsheets.Get(sheetId).ExecuteAsync();
			return spreadsheet.Sheets
				.Select(s => s.Properties.Title)
				.FirstOrDefault(title => !string.IsNullOrEmpty(title));
		}
		catch
		{
			return String.Empty;
		}
	}

	public async Task<IList<IList<object>>> GetValuesById(string sheetId, string range)
	{
		var request = _sheetsService.Spreadsheets.Values.Get(sheetId, range);
		ValueRange response = await request.ExecuteAsync();
		return response.Values;
	}
}

