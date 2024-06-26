﻿using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    public static Context CreateEmptyExpressionContext()
    {
        return new([]);
    }

    /// <returns>
    /// Content of file with name <c>$"{testClass.Name}__{testName}__InputInstruction.json"</c>.
    /// </returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static async Task<JToken> LoadTestInputInstructionAsync(Type testClass, string testName)
    {
        string fileName = $"{testClass.Name}__{testName}__InputInstruction.json";
        string absolutePath = Path.Combine(ResourcesDirectoryPath, fileName);

        using (StreamReader reader = File.OpenText(absolutePath))
        {
            return await JToken.LoadAsync(new JsonTextReader(reader));
        }
    }

    /// <returns>
    /// Content of file with name <c>$"{testClass.Name}__{testName}.json"</c>.
    /// </returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static async Task<T> LoadJsonAsync<T>(Type testClass, string testName) where T : JToken
    {
        string fileName = $"{testClass.Name}__{testName}.json";
        string absolutePath = Path.Combine(ResourcesDirectoryPath, fileName);

        using (StreamReader reader = File.OpenText(absolutePath))
        {
            return await JToken.LoadAsync(new JsonTextReader(reader)) as T
                ?? throw new ArgumentException($"Failed to read '{typeof(T).Name}' from '{fileName}'");
        }
    }
}
