using System;
using System.Text.RegularExpressions;

public static class YouTubeHelper
{
    public static string GetYouTubeVideoId(string url)
    {
        var pattern = @"(?:v=|\/)([0-9A-Za-z_-]{11}).*";
        var match = Regex.Match(url, pattern);
        return match.Success ? match.Groups[1].Value : null;
    }

    public static string GetYouTubeEmbedUrl(string youtubeUrl)
    {
        var videoId = GetYouTubeVideoId(youtubeUrl);
        return videoId != null ? $"https://www.youtube.com/embed/{videoId}" : null;
    }
}