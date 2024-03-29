using System.Reflection;

namespace KrasnyyOktyabr.JsonTransform.Tests;

internal static class TestsHelper
{
    public static bool SkipTestInitialize(Type testClass, string testName)
    {
        return testClass
            .GetMethod(testName)
            ?.GetCustomAttributes<SkipTestInitializeAttribute>()
            .Any() == true;
    }

    /// <returns>New instance of <see cref="Context"/>.</returns>
    public static Context GetEmptyExpressionContext()
    {
        return new([]);
    }
}
