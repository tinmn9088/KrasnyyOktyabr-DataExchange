namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <exception cref="ArgumentNullException"></exception>
public sealed class MemorySetExpression(IExpression<Task<string>> nameExpression, IExpression<Task<object?>> valueExpression) : AbstractExpression<Task>
{
    private readonly IExpression<Task<string>> _nameExpression = nameExpression ?? throw new ArgumentNullException(nameof(nameExpression));

    private readonly IExpression<Task<object?>> _valueExpression = valueExpression ?? throw new ArgumentNullException(nameof(valueExpression));

    /// <exception cref="NullReferenceException"></exception>
    protected override async Task InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        string name = await _nameExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();
        object? value = await _valueExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false);

        context.MemorySet(name, value);
    }
}
