namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

/// <summary>
/// Matcher of a particular expression.
/// </summary>
public interface IExpressionMatcher<TIn>
{
    bool Match(TIn value);
}
