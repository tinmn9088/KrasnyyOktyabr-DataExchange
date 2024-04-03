namespace KrasnyyOktyabr.JsonTransform.Expressions;

public abstract class AbstractExpression<T> : IExpression<T> where T : Task
{
    public abstract T InterpretAsync(IContext context, CancellationToken cancellationToken);
}
