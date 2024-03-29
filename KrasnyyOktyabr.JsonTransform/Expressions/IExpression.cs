namespace KrasnyyOktyabr.JsonTransform.Expressions;

public interface IExpression<out T> where T : Task
{
    /// <exception cref="OperationCanceledException"></exception>
    T InterpretAsync(IContext context, CancellationToken cancellationToken);
}
