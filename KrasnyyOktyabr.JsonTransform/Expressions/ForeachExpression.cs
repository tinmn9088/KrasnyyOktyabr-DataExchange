namespace KrasnyyOktyabr.JsonTransform.Expressions;

public class ForeachExpression : AbstractExpression<Task>
{
    private readonly IExpression<Task<object?[]>> _itemsExpression;

    private readonly IExpression<Task> _innerExpression;

    private readonly string? _name;

    /// <param name="name">May cause collision in cursor names in <see cref="IContext"/>.</param>
    public ForeachExpression(IExpression<Task<object?[]>> itemsExpression, IExpression<Task> innerExpression, string? name = null)
    {
        _itemsExpression = itemsExpression ?? throw new ArgumentNullException(nameof(itemsExpression));
        _innerExpression = innerExpression ?? throw new ArgumentNullException(nameof(innerExpression));

        if (name != null)
        {
            _name = name;
        }
    }

    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    public override async Task InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            object?[] items = await _itemsExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

            string cursorName = _name ?? Mark ?? string.Empty;

            for (int i = 0; i < items.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                context.UpdateCursor(cursorName, items[i], i);

                await _innerExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false);
            }

            context.RemoveCursor(cursorName);
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
