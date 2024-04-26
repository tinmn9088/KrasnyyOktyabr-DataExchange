namespace KrasnyyOktyabr.JsonTransform.Expressions;

public class WhileExpression(IExpression<Task<bool>> conditionExpression, IExpression<Task> innerExpression) : AbstractExpression<Task>
{
    private readonly IExpression<Task<bool>> _conditionExpression = conditionExpression ?? throw new ArgumentNullException(nameof(conditionExpression));

    private readonly IExpression<Task> _innerExpression = innerExpression ?? throw new ArgumentNullException(nameof(innerExpression));

    /// <exception cref="OperationCanceledException"></exception>
    protected override async Task InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        while (await _conditionExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _innerExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false);
        }
    }
}
