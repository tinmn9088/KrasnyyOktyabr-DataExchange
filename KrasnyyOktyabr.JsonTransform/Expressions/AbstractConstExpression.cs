namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Returns passed <paramref name="value"/>.
/// </summary>
public abstract class AbstractConstExpression<T>(T value) : AbstractExpression<Task<T>>
{
    private readonly Task<T> _valueTask = Task.FromResult(value);

    protected sealed override Task<T> InnerInterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        return _valueTask;
    }
}
