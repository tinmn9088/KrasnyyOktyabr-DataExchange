using KrasnyyOktyabr.JsonTransform.Structures;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class ValueTableSelectLineExpression(
    IExpression<Task<IValueTable>> valueTableExpression,
    IExpression<Task<long>> indexExpression)
    : AbstractExpression<Task>
{
    private readonly IExpression<Task<IValueTable>> _valueTableExpression = valueTableExpression ?? throw new ArgumentNullException(nameof(valueTableExpression));

    private readonly IExpression<Task<long>> _indexExpression = indexExpression ?? throw new ArgumentNullException(nameof(indexExpression));

    public override async Task InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            IValueTable valueTable = await _valueTableExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

            long index = await _indexExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false);

            valueTable.SelectLine(Convert.ToInt32(index));
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
