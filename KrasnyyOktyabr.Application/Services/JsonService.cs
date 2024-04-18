using System.Collections.Concurrent;
using KrasnyyOktyabr.JsonTransform;
using KrasnyyOktyabr.JsonTransform.Expressions;
using KrasnyyOktyabr.JsonTransform.Expressions.Creation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.Application.Services.IJsonService;

namespace KrasnyyOktyabr.Application.Services;

public sealed class JsonService(IJsonAbstractExpressionFactory factory) : IJsonService
{
    public static string ConsumerInstructionsPath => Path.Combine("Properties", "ConsumerInstructions");

    private readonly ConcurrentDictionary<string, IExpression<Task>> _instructionNamesExpressions = [];

    public void ClearCachedExpressions() => _instructionNamesExpressions.Clear();

    public KafkaProducerMessageData BuildKafkaProducerMessageData(
        string objectJson,
        Dictionary<string, object?> propertiesToAdd,
        string dataTypePropertyName)
    {
        ArgumentNullException.ThrowIfNull(objectJson);
        ArgumentNullException.ThrowIfNull(propertiesToAdd);
        ArgumentNullException.ThrowIfNull(dataTypePropertyName);

        JObject jObject = ParseJsonObject(objectJson);

        JsonHelper.RemoveEmptyProperties(jObject);

        AddProperties(jObject, propertiesToAdd);

        string dataType = (jObject[dataTypePropertyName] ?? throw new FailedToGetDataTypeException(dataTypePropertyName))
            .Value<string>() ?? throw new FailedToGetDataTypeException(dataTypePropertyName);

        return new()
        {
            ObjectJson = jObject.ToString(Formatting.None),
            DataType = dataType,
        };
    }

    public async ValueTask RunJsonTransformAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken)
    {
        JObject? request = null;

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

        await using StreamWriter streamWriter = new(outputStream);
        JsonSerializer.CreateDefault().Serialize(streamWriter, new JArray(context.OutputGet()));
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

        List<JsonTransformMsSqlResult> result = new(jsonTransformResults.Count);

        foreach (JObject jsonTransformResult in jsonTransformResults)
        {
            // Extract table name
            string tableName = jsonTransformResult[tablePropertyName]?.Value<string>() ?? throw new TablePropertyNotFoundException(tablePropertyName);
            jsonTransformResult.Remove(tablePropertyName);

            result.Add(new JsonTransformMsSqlResult()
            {
                Table = tableName,
                ColumnValues = jsonTransformResult.ToObject<Dictionary<string, dynamic>>()!,
            });
        }

        return result;
    }

    /// <exception cref="ArgumentNullException"></exception>
    private async ValueTask<List<JObject>> RunJsonTransformOnConsumedMessageAsync(
        string instructionName,
        string jsonObject,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(instructionName);
        ArgumentNullException.ThrowIfNull(jsonObject);

        JObject input = ParseJsonObject(jsonObject);

        IExpression<Task> expression = await GetExpressionAsync(instructionName);

        Context context = new(input);

        await expression.InterpretAsync(context, cancellationToken);

        return context.OutputGet();
    }

    /// <exception cref="ArgumentException"></exception>
    private static JObject ParseJsonObject(string jsonObject)
    {
        ArgumentNullException.ThrowIfNull(jsonObject);

        try
        {
            return JObject.Parse(jsonObject);
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
