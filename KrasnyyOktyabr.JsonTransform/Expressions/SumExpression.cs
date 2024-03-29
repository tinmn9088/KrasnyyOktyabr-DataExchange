using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Summarizes 2 numbers.
/// </summary>
public sealed class SumExpression : IExpression<Task<Number>>
{
    private readonly IExpression<Task<Number>> _leftExpression;

    private readonly IExpression<Task<Number>> _rightExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public SumExpression(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression)
    {
        ArgumentNullException.ThrowIfNull(leftExpression);
        ArgumentNullException.ThrowIfNull(rightExpression);

        _leftExpression = leftExpression;
        _rightExpression = rightExpression;
    }

    public async Task<Number> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        Number left = await _leftExpression.InterpretAsync(context, cancellationToken);
        Number right = await _rightExpression.InterpretAsync(context, cancellationToken);

        return left + right;
    }
}
