namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Casts inner expression result to <see cref="object?"/>.
/// </summary>
public sealed class ObjectCastExpression(IExpression<Task> innerExpression) : AbstractCastExpression<object?>(innerExpression)
{
    public override object? Cast(object? innerExpressionTaskResult) => innerExpressionTaskResult;
}
