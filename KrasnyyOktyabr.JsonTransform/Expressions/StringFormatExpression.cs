namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Runs <see cref="string.Format(string, object?[])"/>.
/// </summary>
public sealed class StringFormatExpression : AbstractExpression<Task<string>>
{
    private readonly IExpression<Task<string>> _formatExpression;

    private readonly IReadOnlyList<IExpression<Task<object?>>> _argsExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public StringFormatExpression(IExpression<Task<string>> formatExpression, IReadOnlyList<IExpression<Task<object?>>> argExpressions)
    {
        _formatExpression = formatExpression ?? throw new ArgumentNullException(nameof(formatExpression));
        _argsExpression = argExpressions ?? throw new ArgumentNullException(nameof(argExpressions));

        if (argExpressions.Any(e => e == null))
        {
            throw new ArgumentNullException(nameof(argExpressions));
        }
    }

    /// <exception cref="NullReferenceException"></exception>
    public override async Task<string> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            string format = await _formatExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

            int argsCount = _argsExpression.Count;

            object?[] args = new object?[argsCount];

            for (int i = 0; i < argsCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                args[i] = await _argsExpression[i].InterpretAsync(context, cancellationToken).ConfigureAwait(false);
            }

            return string.Format(format, args);
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
