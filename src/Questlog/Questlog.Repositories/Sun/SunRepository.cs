using System.Globalization;
using Demolite.Core.Models;
using Demolite.Http.Client;
using Microsoft.Extensions.Logging;
using Questlog.Core.Configuration;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;

namespace Questlog.Repositories.Sun;

public class SunRepository(HttpClient httpClient, GeoLocation location, ILogger<SunRepository>? logger = null)
	: ApiClient(httpClient, logger: logger), ISunRepository
{
	public const string BaseAddress = "https://api.sunrisesunset.io/";

	private static readonly Error InvalidResponse = new("Sun.InvalidResponse", "The sun api did not return usable data.");

	/// <summary>
	/// Loads today's sunrise and sunset in UTC for the configured location.
	/// </summary>
	public async Task<Result<SunState>> GetSunStateAsync(CancellationToken ct = default)
	{
		var latitude = location.Latitude.ToString(CultureInfo.InvariantCulture);
		var longitude = location.Longitude.ToString(CultureInfo.InvariantCulture);

		var result = await GetAsync<SunStateResponse>($"json?lat={latitude}&lng={longitude}&timezone=UTC&time_format=24", ct);

		if (!result.IsSuccess)
			return Result<SunState>.Failure(result.Error!);

		var response = result.Value;

		return response is { Status: "OK", Results: not null }
			? Result<SunState>.Success(response.Results)
			: Result<SunState>.Failure(InvalidResponse);
	}
}
