using System;
using System.Text.RegularExpressions;
using SelfEduNet.Data.Regex;

public static class YouTubeHelper
{
    public static string GetYouTubeVideoId(string url)
    {
        var pattern = @"(?:v=|\/)([0-9A-Za-z_-]{11}).*";
        var match = Regex.Match(url, pattern);
        return match.Success ? match.Groups[1].Value : null;
    }
    public static bool IsYouTubeUrl(string url)
    {
	    if (string.IsNullOrEmpty(url))
		    return false;

	    return CommonRegex.YoutubeRegex.IsMatch(url);
    }
	public static string GetYouTubeEmbedUrl(string youtubeUrl)
    {
        var videoId = GetYouTubeVideoId(youtubeUrl);
        return videoId != null ? $"https://www.youtube.com/embed/{videoId}" : null;
    }
    public static async Task<string> GetYouTubeVideoTitleAsync(string youtubeUrl)
    {
	    var videoId = GetYouTubeVideoId(youtubeUrl);
	    if (videoId == null)
		    return null;

	    using var httpClient = new HttpClient();
	    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0"); // Эмулируем браузер
	    var response = await httpClient.GetStringAsync($"https://www.youtube.com/watch?v={videoId}");

	    var match = Regex.Match(response, @"<title>(.*?)<\/title>");
	    if (match.Success)
	    {
		    var title = match.Groups[1].Value;
		    return title.Replace(" - YouTube", "").Trim();
	    }

	    return "Video not found";
    }
}