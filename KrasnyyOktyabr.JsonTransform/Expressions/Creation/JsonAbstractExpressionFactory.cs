using KrasnyyOktyabr.JsonTransform.Numerics;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.ExceptionsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonAbstractExpressionFactory : IJsonAbstractExpressionFactory
{
    private List<IJsonExpressionFactory<IExpression<Task>>> _expressionFactories = [];

    /// <summary>
    /// Most expression factories have an abstract expressionFactory as a dependency. To create an instance 
    /// of an expression expressionFactory an abstract expressionFactory instance must to be created first. And when all
    /// expression factories are created they are passed to the abstract expressionFactory.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public IEnumerable<IJsonExpressionFactory<IExpression<Task>>> ExpressionFactories
    {
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            List<IJsonExpressionFactory<IExpression<Task>>> newValue = [];

            foreach (IJsonExpressionFactory<IExpression<Task>> expressionFactory in value)
            {
                ArgumentNullException.ThrowIfNull(expressionFactory);

                newValue.Add(expressionFactory);
            }

            _expressionFactories = newValue;
        }
    }

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidExpressionReturnTypeException"></exception>
    /// <exception cref="UnknownInstructionException"></exception>
    public TOut Create<TOut>(JToken instruction) where TOut : IExpression<Task>
    {
        IJsonExpressionFactory<IExpression<Task>> expressionFactory = GetJsonExpressionFactory(instruction);

        IExpression<Task> uncheckedExpression = expressionFactory.Create(instruction);

        // Mark expression before return
        uncheckedExpression.Mark = instruction.Path;

        if (uncheckedExpression is TOut checkedExpression)
        {
            return checkedExpression;
        }
        else if (typeof(TOut) == typeof(IExpression<Task<Number>>))
        {
            if (TryWrapInNumberCastExpression(uncheckedExpression, out NumberCastExpression? numberCastExpression))
            {
                return (TOut)(object)numberCastExpression!;
            }
        }
        else if (typeof(TOut) == typeof(IExpression<Task<object?>>))
        {
            if (TryWrapInNumberCastExpression(uncheckedExpression, out NumberCastExpression? numberCastExpression))
            {
                uncheckedExpression = numberCastExpression!;
            }

            return (TOut)(object)new ObjectCastExpression(uncheckedExpression);
        }

        throw new InvalidExpressionReturnTypeException(instruction, typeof(TOut), uncheckedExpression.GetType());
    }

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="UnknownInstructionException"></exception>
    private IJsonExpressionFactory<IExpression<Task>> GetJsonExpressionFactory(JToken instruction)
    {
        ArgumentNullException.ThrowIfNull(instruction);

        foreach (IJsonExpressionFactory<IExpression<Task>> expressionFactory in _expressionFactories)
        {
            if (expressionFactory.Match(instruction))
            {
                return expressionFactory;
            }
        }

        throw new UnknownInstructionException(instruction);
    }

    /// <param name="numberCastExpression"><paramref name="uncheckedExpression"/> wrapped in <see cref="NumberCastExpression"/> or <c>null</c>.</param>
    private static bool TryWrapInNumberCastExpression(IExpression<Task> uncheckedExpression, out NumberCastExpression? numberCastExpression)
    {
        numberCastExpression = null;

        if (uncheckedExpression is IExpression<Task<int>> intExpression)
        {
            numberCastExpression = new NumberCastExpression(intExpression);
            return true;
        }

        if (uncheckedExpression is IExpression<Task<double>> doubleExpression)
        {
            numberCastExpression = new NumberCastExpression(doubleExpression);
            return true;
        }

        return false;
    }

    public class UnknownInstructionException : Exception
    {
        internal UnknownInstructionException(JToken value) : base($"At '{value.Path}': '{value.ToString(Newtonsoft.Json.Formatting.None)}'")
        {
        }
    }

    public class InvalidExpressionReturnTypeException : Exception
    {
        internal InvalidExpressionReturnTypeException(JToken value, Type expected, Type actual) : base($"At '{value.Path}' got '{BuildTypeNameWithParameters(actual)}' instead of '{BuildTypeNameWithParameters(expected)}'")
        {
        }
    }
}
