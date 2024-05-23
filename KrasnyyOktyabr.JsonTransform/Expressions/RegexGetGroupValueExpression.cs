using System.Text.RegularExpressions;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Returns first value of the first group in regular expression or an empty string.
/// </summary>
public sealed class RegexGetGroupValueExpression : AbstractExpression<Task<string>>
{
    private readonly IExpression<Task<string>> _regexExpression;

    private readonly IExpression<Task<string>> _inputExpression;

    private readonly IExpression<Task<long>>? _groupNumberExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public RegexGetGroupValueExpression(
        IExpression<Task<string>> regexExpression,
        IExpression<Task<string>> inputExpression,
        IExpression<Task<long>>? groupNumberExpression = null)
    {
        _regexExpression = regexExpression ?? throw new ArgumentNullException(nameof(regexExpression));
        _inputExpression = inputExpression ?? throw new ArgumentNullException(nameof(inputExpression));

        if (groupNumberExpression != null)
        {
            _groupNumberExpression = groupNumberExpression;
        }
    }

    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    public override async Task<string> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            string regexString = await _regexExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();
            string input = await _inputExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

            int groupNumber = _groupNumberExpression != null
                ? Convert.ToInt32(await _groupNumberExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false))
                : 1;

            Regex regex = new(regexString);
            Match match = regex.Match(input);

            return match.Success
                ? match.Groups[groupNumber].Value
                : string.Empty;
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
