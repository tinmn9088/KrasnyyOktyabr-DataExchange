﻿namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class MapExpression : AbstractExpression<Task<Dictionary<string, object?>>>
{
    private readonly IReadOnlyDictionary<string, IExpression<Task<object?>>> _keysAndExpressions;

    /// <exception cref="ArgumentNullException"></exception>
    public MapExpression(IReadOnlyDictionary<string, IExpression<Task<object?>>> keysAndExpressions)
    {
        ArgumentNullException.ThrowIfNull(keysAndExpressions);

        if (keysAndExpressions.Any(e => e.Value == null))
        {
            throw new ArgumentNullException(nameof(keysAndExpressions));
        }

        _keysAndExpressions = keysAndExpressions;
    }

    protected override async Task<Dictionary<string, object?>> InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        Dictionary<string, object?> keysAndResults = [];

        foreach (KeyValuePair<string, IExpression<Task<object?>>> keyAndExpression in _keysAndExpressions)
        {
            cancellationToken.ThrowIfCancellationRequested();

            object? result = await keyAndExpression.Value.InterpretAsync(context, cancellationToken);

            keysAndResults.Add(keyAndExpression.Key, result);
        }

        return keysAndResults;
    }
}