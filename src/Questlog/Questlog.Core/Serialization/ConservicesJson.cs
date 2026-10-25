using System.Text.Json;

namespace Questlog.Core.Serialization;

public static class ConservicesJson
{
	public static JsonSerializerOptions Options { get; } = CreateOptions();

	private static JsonSerializerOptions CreateOptions()
		=> new()
		{
			Converters = { new EmptyAsNullDateTimeOffsetConverter() },
		};
}
