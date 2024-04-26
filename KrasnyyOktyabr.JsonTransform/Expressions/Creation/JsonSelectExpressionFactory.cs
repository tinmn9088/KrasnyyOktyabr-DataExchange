using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonSelectExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<SelectExpression>(@"{
            'type': 'object',
            'additionalProperties': false,
            'properties': {
            '" + JsonSchemaPropertyComment + @"': {
                'type': 'string'
            },
            '" + JsonSchemaPropertySelect + @"': {
                'type': 'object',
                'additionalProperties': false,
                'properties': {
                '" + JsonSchemaPropertyPath + @"': {},
                '" + JsonSchemaPropertyOptional + @"': {}
                },
                'required': [
                '" + JsonSchemaPropertyPath + @"'
                ]
            }
            },
            'required': [
            '" + JsonSchemaPropertySelect + @"'
            ]
        }")
{
    public static string JsonSchemaPropertySelect => "$select";

    public static string JsonSchemaPropertyPath => "path";

    public static string JsonSchemaPropertyOptional => "optional";

    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"></exception>
    public override SelectExpression Create(JToken input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertySelect]!;
        JToken pathInstruction = instruction[JsonSchemaPropertyPath]!;
        JToken? isOptionalInstruction = instruction[JsonSchemaPropertyOptional];

        IExpression<Task<string>> pathExpression = _factory.Create<IExpression<Task<string>>>(pathInstruction);

        if (isOptionalInstruction != null)
        {
            IExpression<Task<bool>> isOptionalExpression = _factory.Create<IExpression<Task<bool>>>(isOptionalInstruction!);

            return new(pathExpression, isOptionalExpression);
        }
        else
        {
            return new(pathExpression);
        }
    }
}
