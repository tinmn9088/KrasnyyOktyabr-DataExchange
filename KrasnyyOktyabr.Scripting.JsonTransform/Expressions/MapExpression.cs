namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class MapExpression : AbstractExpression<Task<Dictionary<string, object?>>>
{
    private readonly IReadOnlyDictionary<string, IExpression<Task<object?>>> _keysAndExpressions;

    /// <exception cref="ArgumentNullException"></exception>
    public MapExpression(IReadOnlyDictionary<string, IExpression<Task<object?>>> keysAndExpressions)
    {
        _keysAndExpressions = keysAndExpressions ?? throw new ArgumentNullException(nameof(keysAndExpressions));

        if (keysAndExpressions.Any(e => e.Value is null))
        {
            throw new ArgumentNullException(nameof(keysAndExpressions));
        }
    }

    public override async Task<Dictionary<string, object?>> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            Dictionary<string, object?> keysAndResults = [];

            foreach (KeyValuePair<string, IExpression<Task<object?>>> keyAndExpression in _keysAndExpressions)
            {
                cancellationToken.ThrowIfCancellationRequested();

                object? result = await keyAndExpression.Value.InterpretAsync(context, cancellationToken).ConfigureAwait(false);

                keysAndResults.Add(keyAndExpression.Key, result);
            }

            return keysAndResults;
        }
        catch (InterpretException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InterpretException(ex.Message, Mark);
        }
    }
}
