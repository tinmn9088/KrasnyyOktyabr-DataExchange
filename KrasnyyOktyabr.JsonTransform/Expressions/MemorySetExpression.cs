
namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class MemorySetExpression : AbstractExpression<Task>
{
    private readonly IExpression<Task<string>> _nameExpression;

    private readonly IExpression<Task<object?>> _valueExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public MemorySetExpression(IExpression<Task<string>> nameExpression, IExpression<Task<object?>> valueExpression)
    {
        ArgumentNullException.ThrowIfNull(nameExpression);
        ArgumentNullException.ThrowIfNull(valueExpression);

        _nameExpression = nameExpression;
        _valueExpression = valueExpression;
    }

    /// <exception cref="NullReferenceException"></exception>
    protected override async Task InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        string name = await _nameExpression.InterpretAsync(context, cancellationToken) ?? throw new NullReferenceException();
        object? value = await _valueExpression.InterpretAsync(context, cancellationToken);

        context.MemorySet(name, value);
    }
}
