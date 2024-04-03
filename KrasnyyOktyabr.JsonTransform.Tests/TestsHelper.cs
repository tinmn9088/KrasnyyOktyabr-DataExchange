using System.Reflection;

namespace KrasnyyOktyabr.JsonTransform.Tests;

internal static class TestsHelper
{
    public static string ResourcesDirectoryPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");

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

    /// <exception cref="FileNotFoundException"></exception>
    public static string GetTestInputInstruction(Type testClass, string testName)
    {
        string fileName = $"{testClass.Name}__{testName}__InputInstruction.json";
        string absolutePath = Path.Combine(ResourcesDirectoryPath, fileName);

        FileInfo fileInfo = new(absolutePath);

        using (StreamReader reader = fileInfo.OpenText())
        {
            return reader.ReadToEnd();
        }
    }
}
