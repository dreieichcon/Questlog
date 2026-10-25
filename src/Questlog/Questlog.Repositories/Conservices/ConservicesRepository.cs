using System.Net.Http.Headers;
using Demolite.Core.Models;
using Demolite.Http.Client;
using Microsoft.Extensions.Logging;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Core.Serialization;

namespace Questlog.Repositories.Conservices;

public class ConservicesRepository : ApiClient, IConservicesRepository
{
	public const string BaseAddress = "https://conservices.de/api/";

	private static readonly Error EmptyResponse = new("Conservices.EmptyResponse", "The api returned an empty response.");

	public ConservicesRepository(HttpClient httpClient, ILogger<ConservicesRepository>? logger = null)
		: base(httpClient, ConservicesJson.Options, logger)
	{
		httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
	}

	/// <summary>
	/// Loads all conventions.
	/// </summary>
	public Task<Result<IReadOnlyList<Convention>>> GetConventionsAsync(CancellationToken ct = default)
		=> GetListAsync<Convention>("convention", ct);

	/// <summary>
	/// Loads all games of a convention.
	/// </summary>
	public Task<Result<IReadOnlyList<Game>>> GetGamesAsync(string eventId, CancellationToken ct = default)
		=> GetListAsync<Game>($"convention/{Uri.EscapeDataString(eventId)}/games", ct);

	/// <summary>
	/// Loads all program items of a convention.
	/// </summary>
	public Task<Result<IReadOnlyList<ProgramItem>>> GetProgramItemsAsync(string eventId, CancellationToken ct = default)
		=> GetListAsync<ProgramItem>($"convention/{Uri.EscapeDataString(eventId)}/programms", ct);

	private async Task<Result<IReadOnlyList<T>>> GetListAsync<T>(string requestUri, CancellationToken ct)
	{
		var result = await GetAsync<List<T>>(requestUri, ct);

		if (!result.IsSuccess)
			return Result<IReadOnlyList<T>>.Failure(result.Error!);

		return result.Value is null
			? Result<IReadOnlyList<T>>.Failure(EmptyResponse)
			: Result<IReadOnlyList<T>>.Success(result.Value);
	}
}
