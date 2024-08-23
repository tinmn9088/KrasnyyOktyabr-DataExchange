using KrasnyyOktyabr.JsonTransform.Structures;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class ValueTableGetValueExpression(
    IExpression<Task<IValueTable>> valueTableExpression,
    IExpression<Task<string>> columnExpression)
    : AbstractExpression<Task<object?>>
{
    private readonly IExpression<Task<IValueTable>> _valueTableExpression = valueTableExpression ?? throw new ArgumentNullException(nameof(valueTableExpression));

    private readonly IExpression<Task<string>> _columnExpression = columnExpression ?? throw new ArgumentNullException(nameof(columnExpression));

    public override async Task<object?> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            IValueTable valueTable = await _valueTableExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

            string column = await _columnExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

            return valueTable.GetValue(column);
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
