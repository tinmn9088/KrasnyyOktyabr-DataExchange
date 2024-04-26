namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class IfElseExpression : AbstractExpression<Task>
{
    private readonly IExpression<Task<bool>> _conditionExpression;

    private readonly IExpression<Task> _thenExpression;

    private readonly IExpression<Task>? _elseExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public IfElseExpression(IExpression<Task<bool>> conditionExpression, IExpression<Task> thenExpression, IExpression<Task>? elseExpression = null)
    {
        _conditionExpression = conditionExpression ?? throw new ArgumentNullException(nameof(conditionExpression));
        _thenExpression = thenExpression ?? throw new ArgumentNullException(nameof(thenExpression));

        if (elseExpression != null)
        {
            _elseExpression = elseExpression;
        }
    }

    /// <exception cref="OperationCanceledException"></exception>
    protected override async Task InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (await _conditionExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _thenExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false);
        }
        else if (_elseExpression != null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _elseExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false);
        }
    }
}
