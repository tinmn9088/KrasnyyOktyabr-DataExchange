namespace KrasnyyOktyabr.JsonTransform.Expressions;

public interface IExpression<out T> where T : Task
{
    string? Mark { get; set; }

    /// <exception cref="OperationCanceledException"></exception>
    T InterpretAsync(IContext context, CancellationToken cancellationToken);
}
