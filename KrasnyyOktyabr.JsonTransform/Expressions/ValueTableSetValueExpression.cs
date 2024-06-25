using KrasnyyOktyabr.JsonTransform.Structures;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class ValueTableSetValueExpression(
    IExpression<Task<IValueTable>> valueTableExpression,
    IExpression<Task<string>> columnExpression,
    IExpression<Task<object?>> valueExpression)
    : AbstractExpression<Task>
{
    private readonly IExpression<Task<IValueTable>> _valueTableExpression = valueTableExpression ?? throw new ArgumentNullException(nameof(valueTableExpression));

    private readonly IExpression<Task<string>> _columnExpression = columnExpression ?? throw new ArgumentNullException(nameof(columnExpression));

    private readonly IExpression<Task<object?>> _valueExpression = valueExpression ?? throw new ArgumentNullException(nameof(valueExpression));

    public override async Task InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            IValueTable valueTable = await _valueTableExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

            string column = await _columnExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

            object? value = await _valueExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

            valueTable.SetValue(column, value);
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
