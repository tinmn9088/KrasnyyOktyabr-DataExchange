using KrasnyyOktyabr.JsonTransform.Structures;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class ValueTableCountExpression(IExpression<Task<IValueTable>> valueTableExpression) : AbstractExpression<Task<int>>
{
    private readonly IExpression<Task<IValueTable>> _valueTableExpression = valueTableExpression ?? throw new ArgumentNullException(nameof(valueTableExpression));

    public override async Task<int> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            IValueTable valueTable = await _valueTableExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false);

            return valueTable.Count;
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
