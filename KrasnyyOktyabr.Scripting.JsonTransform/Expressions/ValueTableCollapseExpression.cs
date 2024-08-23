using KrasnyyOktyabr.JsonTransform.Structures;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class ValueTableCollapseExpression : AbstractExpression<Task>
{
    private readonly IExpression<Task<IValueTable>> _valueTableExpression;

    private readonly IExpression<Task<string>> _columnsToGroupStringExpression;

    private readonly IExpression<Task<string>>? _columnsToSumStringExpression;

    public ValueTableCollapseExpression(
        IExpression<Task<IValueTable>> valueTableExpression,
        IExpression<Task<string>> columnsToGroupStringExpression,
        IExpression<Task<string>>? columnsToSumStringExpression = null)
    {
        _valueTableExpression = valueTableExpression ?? throw new ArgumentNullException(nameof(valueTableExpression));
        _columnsToGroupStringExpression = columnsToGroupStringExpression ?? throw new ArgumentNullException(nameof(columnsToGroupStringExpression));

        if (columnsToSumStringExpression is not null)
        {
            _columnsToSumStringExpression = columnsToSumStringExpression;
        }
    }

    public override async Task InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            IValueTable valueTable = await _valueTableExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

            string columnsToGroupString = await _columnsToGroupStringExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

            IEnumerable<string> columnsToGroup = columnsToGroupString
                    .Split(',')
                    .Select(c => c.Trim());

            if (_columnsToSumStringExpression is not null)
            {
                string columnsToSumString = await _columnsToSumStringExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

                IEnumerable<string> columnsToSum = columnsToSumString
                    .Split(',')
                    .Select(c => c.Trim());

                valueTable.Collapse(columnsToGroup, columnsToSum);
            }
            else
            {
                valueTable.Collapse(columnsToGroup);
            }
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
