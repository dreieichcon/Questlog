using System.Text.RegularExpressions;

namespace Questlog.Core.Text;

public static partial class WordBreaker
{
	/// <summary>
	/// Splits a name at the positions where it may be wrapped.
	/// </summary>
	public static IReadOnlyList<string> Split(string input)
	{
		var parts = SplitAtCapitals(input) ?? SplitAtSyllables(input);

		return [.. parts.Where(part => part.Length > 0)];
	}

	private static string[]? SplitAtCapitals(string input)
	{
		if (input.ToLowerInvariant() == input)
			return null;

		var parts = CapitalPattern().Split(input);

		return parts.Length == 1 ? null : parts;
	}

	private static string[] SplitAtSyllables(string input)
	{
		var enIndex = input.IndexOf("en", StringComparison.Ordinal);
		if (enIndex >= 0)
			return [input[..(enIndex + 2)], input[(enIndex + 2)..]];

		var schIndex = input.IndexOf("sch", StringComparison.Ordinal);
		if (schIndex >= 0)
			return [input[..schIndex], input[schIndex..]];

		return input.Split('-');
	}

	[GeneratedRegex("(?<!^)(?=[A-Z])")]
	private static partial Regex CapitalPattern();
}
