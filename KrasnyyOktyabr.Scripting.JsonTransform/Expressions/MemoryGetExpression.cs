namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <exception cref="ArgumentNullException"></exception>
public sealed class MemoryGetExpression(IExpression<Task<string>> nameExpression) : AbstractExpression<Task<object?>>
{
    private readonly IExpression<Task<string>> _nameExpression = nameExpression ?? throw new ArgumentNullException(nameof(nameExpression));

    /// <exception cref="NullReferenceException"></exception>
    public override async Task<object?> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            string name = await _nameExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

            return context.MemoryGet(name);
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
