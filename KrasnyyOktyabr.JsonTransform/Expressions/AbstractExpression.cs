namespace KrasnyyOktyabr.JsonTransform.Expressions;

public abstract class AbstractExpression<T> : IExpression<T> where T : Task
{
    public string? Mark { get; set; }

    /// <summary>
    /// Wraps <see cref="InnerInterpretAsync"/> with try-catch block to wrap all raised exceptions
    /// with <see cref="InterpretException"/>.
    /// </summary>
    /// <exception cref="InterpretException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    public T InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            return InnerInterpretAsync(context, cancellationToken);
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

    /// <summary>
    /// Is wrapped by <see cref="InterpretAsync"/> with try-catch block to wrap all raised exceptions
    /// with <see cref="InterpretException"/>.
    /// </summary>
    protected abstract T InnerInterpretAsync(IContext context, CancellationToken cancellationToken);

    /// <summary>
    /// Shows where exception was raised using <see cref="Mark"/>.
    /// </summary>
    public class InterpretException(string errorMessage, string? mark)
    : Exception($"At '{mark}': {errorMessage}")
    {
    }
}
