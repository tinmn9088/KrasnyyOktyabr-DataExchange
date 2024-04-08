namespace KrasnyyOktyabr.JsonTransform.Expressions;

public class ForeachExpression : AbstractExpression<Task>
{
    private readonly IExpression<Task<object?[]>> _itemsExpression;

    private readonly IExpression<Task> _innerExpression;

    public ForeachExpression(IExpression<Task<object?[]>> itemsExpression, IExpression<Task> innerExpression)
    {
        ArgumentNullException.ThrowIfNull(itemsExpression);
        ArgumentNullException.ThrowIfNull(innerExpression);

        _itemsExpression = itemsExpression;
        _innerExpression = innerExpression;
    }

    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    protected override async Task InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        object?[] items = await _itemsExpression.InterpretAsync(context, cancellationToken) ?? throw new NullReferenceException();

        for (int i = 0; i < items.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            context.UpdateForeachCursor(Mark ?? string.Empty, items[i], i);

            await _innerExpression.InterpretAsync(context, cancellationToken);
        }

        context.ClearForeachCursor(Mark ?? string.Empty);
    }
}
