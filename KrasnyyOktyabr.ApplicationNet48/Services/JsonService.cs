using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using KrasnyyOktyabr.JsonTransform;
using KrasnyyOktyabr.JsonTransform.Expressions;
using KrasnyyOktyabr.JsonTransform.Expressions.Creation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.ApplicationNet48.Services.IJsonService;
using static KrasnyyOktyabr.JsonTransform.JsonHelper;

namespace KrasnyyOktyabr.ApplicationNet48.Services;

public sealed class JsonService(IJsonAbstractExpressionFactory factory) : IJsonService
{
    public static string ConsumerInstructionsPath => Path.Combine("Properties", "ConsumerInstructions");

    public static string InstructionsPropertyName => "instructions";

    public static string InputPropertyName => "input";

    private readonly ConcurrentDictionary<string, IExpression<Task>> _instructionNamesExpressions = [];

    public int ClearCachedExpressions()
    {
        int instructionsCount = _instructionNamesExpressions.Count;

        _instructionNamesExpressions.Clear();

        return instructionsCount;
    }

    public KafkaProducerMessageData BuildKafkaProducerMessageData(
        string objectJson,
        Dictionary<string, object?> propertiesToAdd,
        string dataTypePropertyName)
    {
        if (objectJson == null)
        {
            throw new ArgumentNullException(nameof(objectJson));
        }

        if (propertiesToAdd == null)
        {
            throw new ArgumentNullException(nameof(propertiesToAdd));
        }

        if (dataTypePropertyName == null)
        {
            throw new ArgumentNullException(nameof(dataTypePropertyName));
        }

        JObject jObject = ParseObjectJson(objectJson);

        RemoveEmptyProperties(jObject);

        AddProperties(jObject, propertiesToAdd);

        string dataType = (jObject[dataTypePropertyName] ?? throw new FailedToGetDataTypeException(dataTypePropertyName))
            .Value<string>() ?? throw new FailedToGetDataTypeException(dataTypePropertyName);

        return new(
            objectJson: jObject.ToString(Formatting.None),
            dataType: dataType
        );
    }

    /// <param name="outputStream">Is written synchronously.</param>
    public async ValueTask RunJsonTransformAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken)
    {
#nullable enable
        JObject? request = null;
#nullable disable

        using (StreamReader inputStreamReader = new(inputStream))
        {
            request = await JObject.LoadAsync(new JsonTextReader(inputStreamReader), cancellationToken);
        }

        if (!request.ContainsKey(InstructionsPropertyName))
        {
            throw new ArgumentException($"'{InstructionsPropertyName}' property missing");
        }

        if (!request.ContainsKey(InputPropertyName))
        {
            throw new ArgumentException($"'{InputPropertyName}' property missing");
        }

        IExpression<Task> expression = factory.Create<IExpression<Task>>(request[InstructionsPropertyName]!);

        JObject input = JObject.FromObject(request[InputPropertyName] ?? throw new ArgumentException($"'{InputPropertyName}' is empty"));

        Context context = new(input);

        await expression.InterpretAsync(context, cancellationToken);

        StreamWriter writer = new(outputStream);

        // Writes to stream synchronously
        JsonSerializer.CreateDefault().Serialize(writer, new JArray(context.OutputGet()));

        await writer.FlushAsync();
    }

    /// <exception cref="ArgumentNullException"></exception>
    public async ValueTask<List<JsonTransformMsSqlResult>> RunJsonTransformOnConsumedMessageMsSqlAsync(
        string instructionName,
        string jsonObject,
        string tablePropertyName,
        CancellationToken cancellationToken = default)
    {
        List<JObject> jsonTransformResults = await RunJsonTransformOnConsumedMessageAsync(
            instructionName,
            jsonObject,
            cancellationToken);

        List<JsonTransformMsSqlResult> results = new(jsonTransformResults.Count);

        foreach (JObject jsonTransformResult in jsonTransformResults)
        {
            // Extract table name
            string tableName = jsonTransformResult[tablePropertyName]?.Value<string>() ?? throw new TablePropertyNotFoundException(tablePropertyName);
            jsonTransformResult.Remove(tablePropertyName);

            results.Add(new JsonTransformMsSqlResult(
                table: tableName,
                columnValues: jsonTransformResult.ToObject<Dictionary<string, dynamic>>()!
            ));
        }

        return results;
    }

    /// <exception cref="ArgumentNullException"></exception>
    public async ValueTask<List<string>> RunJsonTransformOnConsumedMessageVApplicationAsync(
        string instructionName,
        string jsonObject,
        CancellationToken cancellationToken = default)
    {
        List<JObject> jsonTransformResults = await RunJsonTransformOnConsumedMessageAsync(
            instructionName,
            jsonObject,
            cancellationToken);

        List<string> results = new(jsonTransformResults.Count);

        foreach (JObject jsonTransformResult in jsonTransformResults)
        {
            results.Add(jsonTransformResult.ToString(Formatting.None));
        }

        return results;
    }

    /// <exception cref="ArgumentNullException"></exception>
    private async ValueTask<List<JObject>> RunJsonTransformOnConsumedMessageAsync(
        string instructionName,
        string objectJson,
        CancellationToken cancellationToken)
    {
        if (instructionName == null)
        {
            throw new ArgumentNullException(nameof(instructionName));
        }

        if (objectJson == null)
        {
            throw new ArgumentNullException(nameof(objectJson));
        }

        JObject input = ParseObjectJson(objectJson);

        IExpression<Task> expression = await GetExpressionAsync(instructionName);

        Context context = new(input);

        await expression.InterpretAsync(context, cancellationToken);

        return context.OutputGet();
    }

    /// <exception cref="ArgumentException"></exception>
    private static JObject ParseObjectJson(string objectJson)
    {
        if (objectJson == null)
        {
            throw new ArgumentNullException(nameof(objectJson));
        }

        try
        {
            return JObject.Parse(objectJson);
        }
        catch (JsonReaderException ex)
        {
            throw new ArgumentException("Failed to parse JSON", ex);
        }
    }

    private static void AddProperties(JObject jObject, Dictionary<string, object?> propertiesToAdd)
    {
        foreach (KeyValuePair<string, object?> property in propertiesToAdd)
        {
            jObject[property.Key] = JToken.FromObject(property.Value ?? JValue.CreateNull());
        }
    }

    private async ValueTask<IExpression<Task>> GetExpressionAsync(string instructionName)
    {
        if (_instructionNamesExpressions.TryGetValue(instructionName, out IExpression<Task>? cachedExpression))
        {
            return cachedExpression;
        }

        JToken instructions = await LoadInstructionAsync(Path.Combine(ConsumerInstructionsPath, instructionName));

        IExpression<Task> expression = factory.Create<IExpression<Task>>(instructions);

        _instructionNamesExpressions.TryAdd(instructionName, expression); // Race condition possible

        return expression;
    }

    private static async Task<JToken> LoadInstructionAsync(string filePath)
    {
        using StreamReader reader = File.OpenText(filePath);

        return await JToken.LoadAsync(new JsonTextReader(reader));
    }

    public class TablePropertyNotFoundException(string tablePropertyName) : Exception($"'{tablePropertyName}' property not found")
    {
    }
}
