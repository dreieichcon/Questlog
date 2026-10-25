using Questlog.Core.Models;

namespace Questlog.Core.Tests.Models;

[TestClass]
public class ConventionTests
{
	private static Convention CreateConvention(string? logo)
		=> new() { Id = "1", Name = "Con", Logo = logo };

	[TestMethod]
	[DataRow(null)]
	[DataRow("")]
	public void LogoUrl_NoLogo_IsNull(string? logo)
	{
		Assert.IsNull(CreateConvention(logo).LogoUrl);
	}

	[TestMethod]
	public void LogoUrl_RelativePath_IsPrefixedWithSiteUrl()
	{
		Assert.AreEqual("https://conservices.de/media/logo.png", CreateConvention("/media/logo.png").LogoUrl);
	}

	[TestMethod]
	public void LogoUrl_AbsoluteUrl_IsKeptUnchanged()
	{
		Assert.AreEqual("https://example.org/logo.png", CreateConvention("https://example.org/logo.png").LogoUrl);
	}
}
