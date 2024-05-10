namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Sequence of expressions that returns results as <see cref="object?[]"/>.
/// </summary>
public sealed class ArrayExpression : AbstractExpression<Task<object?[]>>
{
    private readonly IReadOnlyList<IExpression<Task<object?>>> _expressions;

    /// <exception cref="ArgumentNullException"></exception>
    public ArrayExpression(IReadOnlyList<IExpression<Task<object?>>> expressions)
    {
        _expressions = expressions ?? throw new ArgumentNullException(nameof(expressions));

        if (expressions.Any(e => e == null))
        {
            throw new ArgumentNullException(nameof(expressions));
        }
    }

    public override async Task<object?[]> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            object?[] result = new object?[_expressions.Count];

            for (int i = 0; i < _expressions.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                result[i] = await _expressions[i].InterpretAsync(context, cancellationToken).ConfigureAwait(false);
            }

            return result;
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
