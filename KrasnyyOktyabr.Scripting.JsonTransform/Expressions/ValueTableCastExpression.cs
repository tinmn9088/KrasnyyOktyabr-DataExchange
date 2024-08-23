using KrasnyyOktyabr.JsonTransform.Structures;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class ValueTableCastExpression(IExpression<Task> innerExpression) : AbstractCastExpression<IValueTable>(innerExpression)
{
    public override IValueTable Cast(object? innerExpressionTaskResult)
    {
        if (innerExpressionTaskResult is IValueTable valueTableResult)
        {
            return valueTableResult;
        }
        else
        {
            throw new ValueTableCastExpressionException(innerExpressionTaskResult, Mark);
        }
    }

    public class ValueTableCastExpressionException : AbstractCastExpressionException
    {
        internal ValueTableCastExpressionException(object? value, string? mark) : base(value, typeof(IValueTable), mark)
        {
        }
    }
}
