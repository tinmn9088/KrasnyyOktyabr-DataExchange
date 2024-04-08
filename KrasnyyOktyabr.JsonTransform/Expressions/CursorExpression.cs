namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class CursorExpression : AbstractExpression<Task<object?>>
{
    private readonly IExpression<Task<string>>? _cursorNameExpression;

    public CursorExpression(IExpression<Task<string>>? cursorNameExpression = null)
    {
        if (cursorNameExpression != null)
        {
            _cursorNameExpression = cursorNameExpression;
        }
    }

    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    protected override async Task<object?> InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_cursorNameExpression != null)
        {
            string name = await _cursorNameExpression.InterpretAsync(context, cancellationToken) ?? throw new NullReferenceException();

            return context.GetCursor(name);
        }
        else
        {
            return context.GetCursor();
        }
    }
}
