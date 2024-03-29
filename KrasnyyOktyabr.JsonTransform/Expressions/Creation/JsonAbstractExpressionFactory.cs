using KrasnyyOktyabr.JsonTransform.Numerics;
using Newtonsoft.Json.Linq;

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

        if (uncheckedExpression is TOut checkedExpression)
        {
            return checkedExpression;
        }
        else if (typeof(TOut) == typeof(IExpression<Task<Number>>))
        {
            if (uncheckedExpression is IExpression<Task<int>> intExpression)
            {
                return (TOut)(object)new NumberCastExpression(intExpression);
            }

            if (uncheckedExpression is IExpression<Task<double>> doubleExpression)
            {
                return (TOut)(object)new NumberCastExpression(doubleExpression);
            }
        }
        else if (typeof(TOut) == typeof(IExpression<Task<object?>>))
        {
            return (TOut)(object)new ObjectCastExpression(uncheckedExpression);
        }

        throw new InvalidExpressionReturnTypeException(typeof(TOut), uncheckedExpression.GetType());
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

    public class UnknownInstructionException : Exception
    {
        internal UnknownInstructionException(JToken value) : base(value.ToString())
        {
        }
    }

    public class InvalidExpressionReturnTypeException : Exception
    {
        internal InvalidExpressionReturnTypeException(Type expected, Type actual) : base($"Got '{ExceptionsHelper.BuildTypeNameWithParameters(actual)}' instead of '{ExceptionsHelper.BuildTypeNameWithParameters(expected)}'")
        {
        }
    }
}
