namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Shows where exception was raised using <see cref="Mark"/>.
/// </summary>
public class InterpretException(string errorMessage, string? mark) : Exception($"At '{mark}': {errorMessage}")
{
}
