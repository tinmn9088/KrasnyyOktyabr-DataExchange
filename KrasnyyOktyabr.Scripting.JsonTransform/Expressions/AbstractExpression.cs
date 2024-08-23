
namespace KrasnyyOktyabr.JsonTransform.Expressions;

public abstract class AbstractExpression<T> : IExpression<T> where T : Task
{
    public string? Mark { get; set; }

    public abstract T InterpretAsync(IContext context, CancellationToken cancellationToken = default);
}
