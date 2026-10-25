namespace Conservices.Screen.Util.Tests;

[TestClass]
public class UserNameHelperTests
{
	[TestMethod]
	[DataRow("AnnaLena", "Anna<wbr>Lena")]
	[DataRow("AnnaLenaMarie", "Anna<wbr>Lena<wbr>Marie")]
	[DataRow("Anna-Lena", "Anna-<wbr>Lena")]
	public void FormatUserName_MultipleCapitals_BreaksBeforeEachCapital(string input, string expected)
	{
		Assert.AreEqual(expected, input.FormatUserName());
	}

	[TestMethod]
	[DataRow("jensen", "jen<wbr>sen")]
	[DataRow("Tobiasen", "Tobiasen")]
	[DataRow("Stephanie", "Stephanie")]
	public void FormatUserName_SingleCapitalOrLowercaseWithEn_BreaksAfterFirstEn(string input, string expected)
	{
		var result = input.FormatUserName();

		Assert.AreEqual(expected, result);
	}

	[TestMethod]
	public void FormatUserName_LowercaseWithSch_BreaksBeforeSch()
	{
		Assert.AreEqual("ku<wbr>schel", "kuschel".FormatUserName());
	}

	[TestMethod]
	public void FormatUserName_LowercaseWithHyphen_ReplacesHyphenWithBreak()
	{
		Assert.AreEqual("max<wbr>mustermann", "max-mustermann".FormatUserName());
	}

	[TestMethod]
	[DataRow("tobi")]
	[DataRow("Anna")]
	public void FormatUserName_NoBreakPoint_ReturnsInputUnchanged(string input)
	{
		Assert.AreEqual(input, input.FormatUserName());
	}

	[TestMethod]
	[DataRow("Stephen")]
	[DataRow("schmidt")]
	public void FormatUserName_BreakPointAtEdge_DoesNotProduceEmptyPart(string input)
	{
		Assert.AreEqual(input, input.FormatUserName());
	}
}
