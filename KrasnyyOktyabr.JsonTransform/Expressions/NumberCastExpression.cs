using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Wraps inner expression result to <see cref="Number"/>.
/// </summary>
public sealed class NumberCastExpression : AbstractExpression<Task<Number>>
{
    private readonly IExpression<Task<int>>? _innerIntExpression;

    private readonly IExpression<Task<double>>? _innerDoubleExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public NumberCastExpression(IExpression<Task<int>> innerIntExpression)
    {
        _innerIntExpression = innerIntExpression ?? throw new ArgumentNullException(nameof(innerIntExpression));
    }

    /// <exception cref="ArgumentNullException"></exception>
    public NumberCastExpression(IExpression<Task<double>> innerDoubleExpression)
    {
        _innerDoubleExpression = innerDoubleExpression ?? throw new ArgumentNullException(nameof(innerDoubleExpression));
    }

    protected override async Task<Number> InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        if (_innerIntExpression != null)
        {
            return new Number(await _innerIntExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false));
        }

        if (_innerDoubleExpression != null)
        {
            return new Number(await _innerDoubleExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false));
        }

        throw new NotImplementedException();
    }
}
