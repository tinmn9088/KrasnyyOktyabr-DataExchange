using KrasnyyOktyabr.JsonTransform;
using KrasnyyOktyabr.JsonTransform.Expressions;
using KrasnyyOktyabr.JsonTransform.Expressions.Creation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.Application.Services.IJsonService;

namespace KrasnyyOktyabr.Application.Services;

public sealed class JsonService(IJsonAbstractExpressionFactory factory) : IJsonService
{
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

    public async Task RunJsonTransformAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken)
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

    // TODO: implement (+ reading instructions from file)
    public Task<RunJsonTransformMsSqlResult> RunJsonTransformOnConsumedMessageMsSqlAsync(string producerName, string consumerName, string message, CancellationToken cancellationToken)
        => throw new NotImplementedException();

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
}
