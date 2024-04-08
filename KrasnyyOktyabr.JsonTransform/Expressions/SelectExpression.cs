using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class SelectExpression : AbstractExpression<Task<JToken?>>
{
    private readonly IExpression<Task<string>> _pathExpression;

    private readonly IExpression<Task<bool>>? _isOptionalExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public SelectExpression(IExpression<Task<string>> pathExpression, IExpression<Task<bool>>? isOptionalExpression = null)
    {
        ArgumentNullException.ThrowIfNull(pathExpression);

        _pathExpression = pathExpression;

        if (isOptionalExpression != null)
        {
            _isOptionalExpression = isOptionalExpression;
        }
    }

    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    protected override async Task<JToken?> InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string path = await _pathExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

        bool isOptional = _isOptionalExpression != null
            && await _isOptionalExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false);

        return context.InputSelect(path) ?? (isOptional ? null : throw new PathReturnedNothingException(path, Mark));
    }

    public class PathReturnedNothingException(string path, string? mark)
        : InterpretException($"Path '{path}' returned nothing", mark)
    {
    }
}
