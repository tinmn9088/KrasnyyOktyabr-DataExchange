namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

/// <summary>
/// Abstract factory that delegates creation to <see cref="IExpressionFactory{TIn, TOut}"/>.
/// </summary>
public interface IAbstractExpressionFactory<TIn>
{
    TOut Create<TOut>(TIn instruction) where TOut : IExpression<Task>;
}
