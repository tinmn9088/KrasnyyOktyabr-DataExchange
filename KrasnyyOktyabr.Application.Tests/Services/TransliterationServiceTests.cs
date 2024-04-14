namespace KrasnyyOktyabr.Application.Services.Tests;

[TestClass]
public class TransliterationServiceTests
{
    private static readonly TransliterationService s_transliterationService = new();

    [TestMethod]
    public void TransliterateToLatin_ShouldTransliterate()
    {
        Assert.AreEqual("London", s_transliterationService.TransliterateToLatin("Лондон"));
        Assert.AreEqual("Palm", s_transliterationService.TransliterateToLatin("Палм"));
        Assert.AreEqual("Heine", s_transliterationService.TransliterateToLatin("Heine"));
    }
}
