
using System.Text.RegularExpressions;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Returns first value of the first group in regular expression or an empty string.
/// </summary>
public sealed class RegexGetGroupValueExpression : AbstractExpression<Task<string>>
{
    private readonly IExpression<Task<string>> _regexExpression;

    private readonly IExpression<Task<string>> _inputExpression;

    private readonly IExpression<Task<int>>? _groupNumberExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public RegexGetGroupValueExpression(
        IExpression<Task<string>> regexExpression,
        IExpression<Task<string>> inputExpression,
        IExpression<Task<int>>? groupNumberExpression = null)
    {
        ArgumentNullException.ThrowIfNull(regexExpression);
        ArgumentNullException.ThrowIfNull(inputExpression);

        _regexExpression = regexExpression;
        _inputExpression = inputExpression;

        if (groupNumberExpression != null)
        {
            _groupNumberExpression = groupNumberExpression;
        }
    }

    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    protected override async Task<string> InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string regexString = await _regexExpression.InterpretAsync(context, cancellationToken) ?? throw new NullReferenceException();
        string input = await _inputExpression.InterpretAsync(context, cancellationToken) ?? throw new NullReferenceException();

        int groupNumber = _groupNumberExpression != null
            ? await _groupNumberExpression.InterpretAsync(context, cancellationToken)
            : 1;
        
        Regex regex = new(regexString);
        Match match = regex.Match(input);

        return match.Success
            ? match.Groups[groupNumber].Value
            : string.Empty;
    }
}
