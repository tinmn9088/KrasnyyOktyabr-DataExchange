using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Wraps inner expression result to <see cref="Number"/>.
/// </summary>
public sealed class NumberCastExpression : AbstractExpression<Task<Number>>
{
    private readonly IExpression<Task<long>>? _innerLongExpression;

    private readonly IExpression<Task<decimal>>? _innerDecimalExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public NumberCastExpression(IExpression<Task<long>> innerLongExpression)
    {
        _innerLongExpression = innerLongExpression ?? throw new ArgumentNullException(nameof(innerLongExpression));
    }

    /// <exception cref="ArgumentNullException"></exception>
    public NumberCastExpression(IExpression<Task<decimal>> innerDecimalExpression)
    {
        _innerDecimalExpression = innerDecimalExpression ?? throw new ArgumentNullException(nameof(innerDecimalExpression));
    }

    public override async Task<Number> InterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        try
        {
            if (_innerLongExpression != null)
            {
                return new Number(await _innerLongExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false));
            }

            if (_innerDecimalExpression != null)
            {
                return new Number(await _innerDecimalExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false));
            }

            throw new NotImplementedException();
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
