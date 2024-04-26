namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <exception cref="ArgumentNullException"></exception>
public sealed class MemoryGetExpression(IExpression<Task<string>> nameExpression) : AbstractExpression<Task<object?>>
{
    private readonly IExpression<Task<string>> _nameExpression = nameExpression ?? throw new ArgumentNullException(nameof(nameExpression));

    /// <exception cref="NullReferenceException"></exception>
    protected override async Task<object?> InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        string name = await _nameExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

        return context.MemoryGet(name);
    }
}
