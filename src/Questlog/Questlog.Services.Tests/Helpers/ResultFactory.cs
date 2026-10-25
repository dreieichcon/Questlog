using Demolite.Core.Models;

namespace Questlog.Services.Tests.Helpers;

public static class ResultFactory
{
	public static Result<IReadOnlyList<T>> Success<T>(params T[] items)
		=> Result<IReadOnlyList<T>>.Success(items);

	public static Result<IReadOnlyList<T>> Failure<T>()
		=> Result<IReadOnlyList<T>>.Failure(new Error("Test.Failure", "failed"));
}
