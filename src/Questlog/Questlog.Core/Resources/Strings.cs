using System.Globalization;
using System.Resources;

namespace Questlog.Core.Resources;

public static class Strings
{
	private static readonly ResourceManager Manager = new("Questlog.Core.Resources.Strings", typeof(Strings).Assembly);

	public static string Get(string key)
		=> Manager.GetString(key, CultureInfo.CurrentUICulture) ?? key;

	public static string Format(string key, params object[] arguments)
		=> string.Format(CultureInfo.CurrentUICulture, Get(key), arguments);
}
