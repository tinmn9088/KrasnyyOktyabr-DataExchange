namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class CursorIndexExpression : AbstractExpression<Task<int>>
{
    private readonly IExpression<Task<string>>? _cursorNameExpression;

    public CursorIndexExpression(IExpression<Task<string>>? cursorNameExpression = null)
    {
        if (cursorNameExpression != null)
        {
            _cursorNameExpression = cursorNameExpression;
        }
    }

    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    protected override async Task<int> InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_cursorNameExpression != null)
        {
            string name = await _cursorNameExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

            return context.GetCursorIndex(name);
        }
        else
        {
            return context.GetCursorIndex();
        }
    }
}
