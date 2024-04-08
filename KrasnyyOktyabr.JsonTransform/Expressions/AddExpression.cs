namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class AddExpression : AbstractExpression<Task>
{
    private readonly IExpression<Task<string>> _keyExpression;

    private readonly IExpression<Task<object?>> _valueExpression;

    private readonly IExpression<Task<int>>? _indexExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public AddExpression(
        IExpression<Task<string>> keyExpression,
        IExpression<Task<object?>> valueExpression,
        IExpression<Task<int>>? indexExpression = null)
    {
        ArgumentNullException.ThrowIfNull(keyExpression);
        ArgumentNullException.ThrowIfNull(valueExpression);

        _keyExpression = keyExpression;
        _valueExpression = valueExpression;

        if (indexExpression != null)
        {
            _indexExpression = indexExpression;
        }
    }

    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    protected override async Task InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string key = await _keyExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();
        object? value = await _valueExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false);

        int index = _indexExpression != null
            ? await _indexExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false)
            : 0;

        context.OutputAdd(key, value, index);
    }
}
