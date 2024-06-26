﻿using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonConstIntExpressionFactoryTests
{
    private static readonly JsonConstLongExpressionFactory s_constIntExpressionFactory = new();

    [TestMethod]
    public void Create_ShouldCreateConstIntExpression()
    {
        JToken input = 66;

        ConstLongExpression expression = s_constIntExpressionFactory.Create(input);

        Assert.IsNotNull(expression);
    }

    [TestMethod]
    public void Match_WhenInputInvalid_ShouldReturnFalse()
    {
        JObject input = new()
        {
            { "IllegalProperty", null },
        };

        bool isMatch = s_constIntExpressionFactory.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JToken input = 99;

        bool isMatch = s_constIntExpressionFactory.Match(input);

        Assert.IsTrue(isMatch);
    }
}
