using System.Text.RegularExpressions;

namespace Conservices.Screen.Util;

public static class UserNameHelper
{
	public static string FormatUserName(this string input)
	{
		IEnumerable<string>? split = null;
		
		if (input.ToLower() != input)
		{
			split = Regex.Split(input, @"(?<!^)(?=[A-Z])");

			if (split.Count() == 1)
				split = null;
		}
		
		split ??= SplitAtHyphenations(input);
		
		return string.Join("<wbr>", split.Where(part => part.Length > 0));
	}

	private static IEnumerable<string> SplitAtHyphenations(string input)
	{
		if (input.Contains("en"))
		{
			var index = input.IndexOf("en") + 2;
			return [input[..index], input[index..]];
		}

		if (input.Contains("sch"))
		{
			var index = input.IndexOf("sch");
			return [input[..index], input[index..]];
		}
		
		return input.Split("-");
	}
}