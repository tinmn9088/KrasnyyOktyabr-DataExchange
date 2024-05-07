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
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            List<IJsonExpressionFactory<IExpression<Task>>> newValue = [];

            foreach (IJsonExpressionFactory<IExpression<Task>> expressionFactory in value)
            {
                if (expressionFactory == null)
                {
                    throw new ArgumentNullException(nameof(expressionFactory));
                }

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
        List<IExpression<Task>> uncheckedExpressions = CreateMatchingExpressions<TOut>(instruction);

        IExpression<Task>? resultExpression = null;

        foreach (IExpression<Task> uncheckedExpression in uncheckedExpressions)
        {
            // Mark before it is wrapped into any other expression
            uncheckedExpression.Mark = instruction.Path;

            if (uncheckedExpression is TOut checkedExpression)
            {
                resultExpression = checkedExpression;
            }
            else if (ExpressionReturnsValue(uncheckedExpression))
            {
                if (typeof(TOut) == typeof(IExpression<Task<Number>>))
                {
                    if (TryWrapInNumberCastExpression(uncheckedExpression, out NumberCastExpression? numberCastExpression))
                    {
                        numberCastExpression!.Mark = instruction.Path;

                        resultExpression = numberCastExpression!;
                    }
                }
                else if (typeof(TOut) == typeof(IExpression<Task<object?>>))
                {
                    if (TryWrapInNumberCastExpression(uncheckedExpression, out NumberCastExpression? numberCastExpression))
                    {
                        numberCastExpression!.Mark = instruction.Path;

                        resultExpression = numberCastExpression!;
                    }

                    resultExpression = new ObjectCastExpression(resultExpression ?? uncheckedExpression);
                }
            }
        }

        if (resultExpression == null)
        {
            throw new InvalidExpressionReturnTypeException(instruction, typeof(TOut), uncheckedExpressions[0].GetType());
        }

        // Mark expression before return
        resultExpression.Mark = instruction.Path;

        return (TOut)resultExpression;
    }

    /// <returns>
    /// <see cref="List{T}"/> where the last exression has the highest priority (always contains at least one item).
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="UnknownInstructionException"></exception>
    /// <exception cref="CannotCreateExpressionException"></exception>
    private List<IExpression<Task>> CreateMatchingExpressions<TOut>(JToken instruction)
    {
        List<IJsonExpressionFactory<IExpression<Task>>> expressionFactories = GetMatchingJsonExpressionFactories(instruction);

        List<IExpression<Task>> matchingExpressions = [];

        for (int i = expressionFactories.Count - 1; i >= 0; i--)
        {
            try
            {
                matchingExpressions.Add(expressionFactories[i].Create(instruction));
            }
            catch (Exception)
            {
                if (typeof(TOut) == GetExpressionFactoryReturnType(expressionFactories[i])) // When factory return type is exactly what is asked to create, but creation failed
                {
                    throw;
                }

                if (matchingExpressions.Count == 0 && i == 0) // When tried all factories but the first and all they failed
                {
                    throw;
                }
            }
        }

        if (matchingExpressions.Count == 0)
        {
            throw new CannotCreateExpressionException(instruction, typeof(TOut), expressionFactories.Count);
        }

        return matchingExpressions;
    }

    /// <returns>
    /// <see cref="List{T}"/> where the first factory has the highest priority (always contains at least one item).
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="UnknownInstructionException"></exception>
    private List<IJsonExpressionFactory<IExpression<Task>>> GetMatchingJsonExpressionFactories(JToken instruction)
    {
        if (instruction == null)
        {
            throw new ArgumentNullException(nameof(instruction));
        }

        List<IJsonExpressionFactory<IExpression<Task>>> matchingExpressionFactories = [];

        foreach (IJsonExpressionFactory<IExpression<Task>> expressionFactory in _expressionFactories)
        {
            if (expressionFactory.Match(instruction))
            {
                matchingExpressionFactories.Add(expressionFactory);
            }
        }

        if (matchingExpressionFactories.Count == 0)
        {
            throw new UnknownInstructionException(instruction);
        }

        return matchingExpressionFactories;
    }

    /// <summary>
    /// Determines <see cref="IJsonExpressionFactory{TOut}"/> parameter type.
    /// </summary>
    private static Type GetExpressionFactoryReturnType(IJsonExpressionFactory<IExpression<Task>> factory)
    {
        return factory.GetType()
            .GetInterfaces()
            .Where(i => typeof(IJsonExpressionFactory<IExpression<Task>>).IsAssignableFrom(i)) // Is guaranteed to match one interface 
            .First()
            .GetGenericArguments()
            .First();
    }

    /// <summary>
    /// Determines if expression returns parameterized <see cref="Task"/> i.e. returns value.
    /// </summary>
    private static bool ExpressionReturnsValue(IExpression<Task> expression)
    {
        return expression.GetType()
            .GetInterfaces()
            .Where(i => typeof(IExpression<Task>).IsAssignableFrom(i)) // Is guaranteed to match one interface 
            .First()
            .GetGenericArguments()
            .First()
            .IsGenericType;
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

    public class CannotCreateExpressionException : Exception
    {
        internal CannotCreateExpressionException(JToken value, Type expected, int factoriesCount) : base($"At '{value.Path}' cannot create expression '{BuildTypeNameWithParameters(expected)}' ({factoriesCount} failed)")
        {
        }
    }
}
