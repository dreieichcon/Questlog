using Questlog.Core.Text;

namespace Questlog.Core.Tests.Text;

[TestClass]
public class WordBreakerTests
{
	[TestMethod]
	[DataRow("AnnaLena", "Anna|Lena")]
	[DataRow("AnnaLenaMarie", "Anna|Lena|Marie")]
	[DataRow("Anna-Lena", "Anna-|Lena")]
	public void Split_MultipleCapitals_BreaksBeforeEachCapital(string input, string expected)
	{
		Assert.AreEqual(expected, Join(input));
	}

	[TestMethod]
	[DataRow("jensen", "jen|sen")]
	[DataRow("Tobiasen", "Tobiasen")]
	[DataRow("Stephanie", "Stephanie")]
	public void Split_SingleCapitalOrLowercaseWithEn_BreaksAfterFirstEn(string input, string expected)
	{
		Assert.AreEqual(expected, Join(input));
	}

	[TestMethod]
	public void Split_LowercaseWithSch_BreaksBeforeSch()
	{
		Assert.AreEqual("ku|schel", Join("kuschel"));
	}

	[TestMethod]
	public void Split_LowercaseWithHyphen_ReplacesHyphenWithBreak()
	{
		Assert.AreEqual("max|mustermann", Join("max-mustermann"));
	}

	[TestMethod]
	[DataRow("tobi")]
	[DataRow("Anna")]
	public void Split_NoBreakPoint_ReturnsInputAsSinglePart(string input)
	{
		Assert.AreEqual(input, Join(input));
	}

	[TestMethod]
	[DataRow("Stephen")]
	[DataRow("schmidt")]
	public void Split_BreakPointAtEdge_DoesNotProduceEmptyPart(string input)
	{
		Assert.AreEqual(input, Join(input));
	}

	[TestMethod]
	public void Split_EmptyString_ReturnsNoParts()
	{
		Assert.AreEqual(0, WordBreaker.Split(string.Empty).Count);
	}

	[TestMethod]
	public void Split_MarkupCharacters_AreKeptAsText()
	{
		Assert.AreEqual("<b>Anna</b>", string.Concat(WordBreaker.Split("<b>Anna</b>")));
	}

	private static string Join(string input) => string.Join('|', WordBreaker.Split(input));
}
