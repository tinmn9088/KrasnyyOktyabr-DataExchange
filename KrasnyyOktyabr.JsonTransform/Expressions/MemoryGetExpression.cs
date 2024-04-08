namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class MemoryGetExpression : AbstractExpression<Task<object?>>
{
    private readonly IExpression<Task<string>> _nameExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public MemoryGetExpression(IExpression<Task<string>> nameExpression)
    {
        ArgumentNullException.ThrowIfNull(nameExpression);

        _nameExpression = nameExpression;
    }

    /// <exception cref="NullReferenceException"></exception>
    protected override async Task<object?> InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        string name = await _nameExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

        return context.MemoryGet(name);
    }
}
