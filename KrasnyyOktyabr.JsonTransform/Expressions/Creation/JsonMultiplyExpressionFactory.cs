﻿using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonMultiplyExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonBinaryNumberExpressionFactory<MultiplyExpression>(JsonSchemaPropertyMultiply, factory)
{
    public static string JsonSchemaPropertyMultiply => "$mul";

    protected override MultiplyExpression CreateExpressionInstance(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression) => new(leftExpression, rightExpression);
}
