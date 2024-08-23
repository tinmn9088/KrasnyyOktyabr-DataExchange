using KrasnyyOktyabr.ApplicationNet48.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrasnyyOktyabr.ApplicationNet48.Tests.Services;

[TestClass]
public class TransliterationHelperTests
{
    [TestMethod]
    public void TransliterateToLatin_ShouldTransliterate()
    {
        Assert.AreEqual("London", TransliterationHelper.TransliterateToLatin("Лондон"));
        Assert.AreEqual("Palm", TransliterationHelper.TransliterateToLatin("Палм"));
        Assert.AreEqual("Heine", TransliterationHelper.TransliterateToLatin("Heine"));
    }
}
