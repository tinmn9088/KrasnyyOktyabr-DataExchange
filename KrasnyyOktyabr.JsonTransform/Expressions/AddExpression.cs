namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class AddExpression : AbstractExpression<Task>
{
    private readonly IExpression<Task<string>> _keyExpression;

    private readonly IExpression<Task<object?>> _valueExpression;

    private readonly IExpression<Task<long>>? _indexExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public AddExpression(
        IExpression<Task<string>> keyExpression,
        IExpression<Task<object?>> valueExpression,
        IExpression<Task<long>>? indexExpression = null)
    {
        _keyExpression = keyExpression ?? throw new ArgumentNullException(nameof(keyExpression));
        _valueExpression = valueExpression ?? throw new ArgumentNullException(nameof(valueExpression));

        if (indexExpression is not null)
        {
            _indexExpression = indexExpression;
        }
    }

    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    public override async Task InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            string key = await _keyExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();
            object? value = await _valueExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false);

            int index = _indexExpression is not null
                ? Convert.ToInt32(await _indexExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false))
                : 0;

            context.OutputAdd(key, value, index);
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
