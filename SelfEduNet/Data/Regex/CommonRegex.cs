using System.Text.RegularExpressions;

namespace SelfEduNet.Data.Regex;

public static class CommonRegex
{
	public static readonly System.Text.RegularExpressions.Regex YoutubeRegex =
		new System.Text.RegularExpressions.Regex(@"^(https?://)?(www\.)?(youtube|youtu|youtube-nocookie)\.(com|be|fr|co\.uk)/.*$", RegexOptions.IgnoreCase);
}

