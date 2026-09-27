using System.Text.Json;
using Demolite.Http.Serialization;

namespace Conservices.Screen.Repositories.Serialization.Conservices;

public class ConservicesSerializer : AbstractSerializer
{
	public override JsonSerializerOptions CreateSerializerOptions()
	{
		var options = new JsonSerializerOptions()
		{
			WriteIndented = true,
			Converters =
			{
				// new ConservicesBoolConverter(),
				new ConservicesDatetimeConverter(),
				new ConservicesTimeSpanConverter(),
			}
		};
		
		return options;
	}
}