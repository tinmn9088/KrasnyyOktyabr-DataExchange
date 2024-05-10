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
    public override async Task<object?> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_cursorNameExpression != null)
            {
                string name = await _cursorNameExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

                return context.GetCursor(name);
            }
            else
            {
                return context.GetCursor();
            }
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
