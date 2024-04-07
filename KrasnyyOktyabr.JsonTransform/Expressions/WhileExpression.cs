
namespace KrasnyyOktyabr.JsonTransform.Expressions;

public class WhileExpression : AbstractExpression<Task>
{
    private readonly IExpression<Task<bool>> _conditionExpression;

    private readonly IExpression<Task> _innerExpression;

    public WhileExpression(IExpression<Task<bool>> conditionExpression, IExpression<Task> innerExpression)
    {
        ArgumentNullException.ThrowIfNull(conditionExpression);
        ArgumentNullException.ThrowIfNull(innerExpression);

        _conditionExpression = conditionExpression;
        _innerExpression = innerExpression;
    }

    /// <exception cref="OperationCanceledException"></exception>
    protected override async Task InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        while (await _conditionExpression.InterpretAsync(context, cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _innerExpression.InterpretAsync(context, cancellationToken);
        }
    }
}
