using KrasnyyOktyabr.ApplicationNet48.Services.DataResolve;
using KrasnyyOktyabr.JsonTransform.Expressions.Creation;
using KrasnyyOktyabr.JsonTransform.Expressions.DataResolve;
using Microsoft.Extensions.DependencyInjection;

namespace KrasnyyOktyabr.ApplicationNet48.DependencyInjection;

public static class JsonTransformDependencyInjectionHelper
{
    /// <summary>
    /// Set up <see cref="IJsonAbstractExpressionFactory"/>.
    /// </summary>
    public static void AddJsonTransform(this IServiceCollection services)
    {
        services.AddSingleton<IDataResolveService, DataResolveService>();

        services.AddSingleton<IJsonAbstractExpressionFactory, JsonAbstractExpressionFactory>(provider =>
        {
            JsonAbstractExpressionFactory factory = new();

            factory.ExpressionFactories = [

                new JsonDataResolveExpressionFactory(factory, provider.GetRequiredService<IDataResolveService>()),

                // Containers
                new JsonExpressionsBlockFactory(factory), 
                new JsonArrayExpressionFactory(factory), // Must be below JsonExpressionsBlockFactory (JSON Schema overlaps)
                new JsonMapExpressionFactory(factory),

                // Const
                new JsonConstExpressionFactory(),
                new JsonConstBoolExpressionFactory(),
                new JsonConstLongExpressionFactory(),
                new JsonConstDecimalExpressionFactory(), // Must be below JsonConstIntExpressionFactory (JSON Schema overlaps)
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

                // ValueTable
                new JsonValueTableCreateExpressionFactory(factory),
                new JsonValueTableSelectLineExpressionFactory(factory),
                new JsonValueTableAddLineExpressionFactory(factory),
                new JsonValueTableSetValueExpressionFactory(factory),
                new JsonValueTableGetValueExpressionFactory(factory),
                new JsonValueTableCollapseExpressionFactory(factory),
                new JsonValueTableCountExpressionFactory(factory),
            ];

            return factory;
        });
    }
}
