namespace KrasnyyOktyabr.JsonTransform.Expressions;

public class ForeachExpression : AbstractExpression<Task>
{
    private readonly IExpression<Task<object?[]>> _itemsExpression;

    private readonly IExpression<Task> _innerExpression;

    private readonly string? _name;

    /// <param name="name">May cause collision in cursor names in <see cref="IContext"/>.</param>
    public ForeachExpression(IExpression<Task<object?[]>> itemsExpression, IExpression<Task> innerExpression, string? name = null)
    {
        ArgumentNullException.ThrowIfNull(itemsExpression);
        ArgumentNullException.ThrowIfNull(innerExpression);

        _itemsExpression = itemsExpression;
        _innerExpression = innerExpression;

        if (name != null)
        {
            _name = name;
        }
    }

    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    protected override async Task InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        object?[] items = await _itemsExpression.InterpretAsync(context, cancellationToken) ?? throw new NullReferenceException();

        string cursorName = _name ?? Mark ?? string.Empty;

        for (int i = 0; i < items.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            context.UpdateCursor(cursorName, items[i], i);

            await _innerExpression.InterpretAsync(context, cancellationToken);
        }

        context.RemoveCursor(cursorName);
    }
}
