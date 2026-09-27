using Conservices.Screen.Interfaces.Conservices;
using Conservices.Screen.Interfaces.Repositories;
using Conservices.Screen.Models.Program;
using Microsoft.Extensions.Caching.Memory;

namespace Conservices.Screen.Services.Conservices;

public class ProgramService(IProgramRepository programRepository, IMemoryCache cache)
	: AbstractKeyedConService<ProgramItem>(cache), IProgramService
{
	protected override TimeSpan RefreshInterval => TimeSpan.FromMinutes(10);

	protected override Task<IEnumerable<ProgramItem>> GetItemsForConventionAsync(string eventId)
		=> programRepository.GetAllAsync(eventId);

	public Task<IEnumerable<ProgramItem>> GetAllAsync(string eventId) => GetItemsAsync(eventId);
}