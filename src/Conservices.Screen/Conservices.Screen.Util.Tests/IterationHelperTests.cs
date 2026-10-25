namespace Conservices.Screen.Util.Tests;

[TestClass]
public class IterationHelperTests
{
	[TestMethod]
	public void WithIterator_EmptySequence_ReturnsEmptySequence()
	{
		var items = Array.Empty<string>();

		var result = items.WithIterator().ToList();

		Assert.AreEqual(0, result.Count);
	}

	[TestMethod]
	public void WithIterator_FilledSequence_PairsItemsWithZeroBasedIndex()
	{
		var items = new[] { "a", "b", "c" };

		var result = items.WithIterator().ToList();

		CollectionAssert.AreEqual(
			new[] { ("a", 0), ("b", 1), ("c", 2) },
			result.Select(pair => (pair.Item1, pair.Item2)).ToArray());
	}
}
