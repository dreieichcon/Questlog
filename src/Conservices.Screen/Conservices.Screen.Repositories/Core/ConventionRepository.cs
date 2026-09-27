using Conservices.Screen.Interfaces.Repositories;
using Conservices.Screen.Models.Convention;
using Conservices.Screen.Repositories.Requests;

namespace Conservices.Screen.Repositories.Core;

public class ConventionRepository : AbstractConservicesRepository, IConventionRepository
{
	public async Task<IEnumerable<Convention>> GetAllAsync()
	{
		var uri = new ConservicesRequestUri().WithSegments("convention");
		
		var result = await GetAsync(uri);

		if (!result.IsSuccess)
			return [];

		return Serializer.Deserialize<IEnumerable<Convention>>(result.ResponseBody!) ?? [];
	}
}