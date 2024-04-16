using KrasnyyOktyabr.JsonTransform.Expressions.Creation;

namespace KrasnyyOktyabr.Application.DependencyInjection;

public static class JsonTransformDependencyInjectionHelper
{
    /// <summary>
    /// Set up <see cref="IJsonAbstractExpressionFactory"/>.
    /// </summary>
    public static void AddJsonTransform(this IServiceCollection services)
    {
        services.AddSingleton<IJsonAbstractExpressionFactory, JsonAbstractExpressionFactory>(_ =>
        {
            JsonAbstractExpressionFactory factory = new();

            factory.ExpressionFactories = [

                // Containers
                new JsonArrayExpressionFactory(factory),
                new JsonExpressionsBlockFactory(factory), // Must be below JsonArrayExpressionFactory (JSON Schema overlaps)
                new JsonMapExpressionFactory(factory),

                // Const
                new JsonConstExpressionFactory(),
                new JsonConstBoolExpressionFactory(),
                new JsonConstIntExpressionFactory(),
                new JsonConstDoubleExpressionFactory(), // Must be below JsonConstIntExpressionFactory (JSON Schema overlaps)
                new JsonConstStringExpressionFactory(),

                // Memory
                new JsonMemoryGetExpressionFactory(factory),
                new JsonMemorySetExpressionFactory(factory),

                // Arithmetic
                new JsonSumExpressionFactory(factory),
                new JsonSubstractExpressionFactory(factory),
                new JsonMultiplyExpressionFactory(factory),
                new JsonDivideExpressionFactory(factory),
                new JsonRoundExpressionFactory(factory),

                // Logical
                new JsonAndExpressionFactory(factory),
                new JsonAreEqualExpressionFactory(factory),
                new JsonAreNotEqualExpressionFactory(factory),
                new JsonOrExpressionFactory(factory),
                new JsonNotExpressionFactory(factory),
                new JsonIsGreaterExpressionFactory(factory),
                new JsonIsGreaterOrEqualExpressionFactory(factory),

                // Context
                new JsonAddExpressionFactory(factory),
                new JsonSelectExpressionFactory(factory),

                // Strings
                new JsonRegexGetGroupValueExpressionFactory(factory),
                new JsonStringFormatExpressionFactory(factory),

                // Types
                new JsonCastExpressionsFactory(factory),

                // Conditions
                new JsonIfElseExpressionFactory(factory),

                // Loops
                new JsonForeachExpressionFactory(factory),
                new JsonWhileExpressionFactory(factory),
                new JsonCursorExpressionFactory(factory),
                new JsonCursorIndexExpressionFactory(factory),
            ];

            return factory;
        });
    }
}
