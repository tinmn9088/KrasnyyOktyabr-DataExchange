namespace KrasnyyOktyabr.JsonTransform.Expressions;

public class WhileExpression(IExpression<Task<bool>> conditionExpression, IExpression<Task> innerExpression) : AbstractExpression<Task>
{
    private readonly IExpression<Task<bool>> _conditionExpression = conditionExpression ?? throw new ArgumentNullException(nameof(conditionExpression));

    private readonly IExpression<Task> _innerExpression = innerExpression ?? throw new ArgumentNullException(nameof(innerExpression));

    /// <exception cref="OperationCanceledException"></exception>
    public override async Task InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            while (await _conditionExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                await _innerExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false);
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
