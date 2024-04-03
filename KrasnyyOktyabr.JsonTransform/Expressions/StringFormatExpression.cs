namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Runs <see cref="string.Format(string, object?[])"/>.
/// </summary>
public sealed class StringFormatExpression : AbstractExpression<Task<string>>
{
    private readonly IExpression<Task<string>> _formatExpression;

    private readonly IReadOnlyList<IExpression<Task<object?>>> _argsExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public StringFormatExpression(IExpression<Task<string>> formatExpression, IReadOnlyList<IExpression<Task<object?>>> argExpressions)
    {
        ArgumentNullException.ThrowIfNull(formatExpression);
        ArgumentNullException.ThrowIfNull(argExpressions);

        if (argExpressions.Any(e => e == null))
        {
            throw new ArgumentNullException(nameof(argExpressions));
        }

        _formatExpression = formatExpression;
        _argsExpression = argExpressions;
    }

    /// <exception cref="NullReferenceException"></exception>
    public override async Task<string> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string format = await _formatExpression.InterpretAsync(context, cancellationToken) ?? throw new NullReferenceException();

        int argsCount = _argsExpression.Count;

        object?[] args = new object?[argsCount];

        for (int i = 0; i < argsCount; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            args[i] = await _argsExpression[i].InterpretAsync(context, cancellationToken);
        }

        return string.Format(format, args);
    }
}
